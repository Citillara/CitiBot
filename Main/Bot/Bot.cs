﻿using CitiBot.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twitch;

namespace CitiBot.Main
{
    public class Bot
    {
        private int m_BotId;
        private string m_Name;
        private string m_Password;
        private short m_CallbackPort;
        private PluginManager m_PluginManager;
        private TwitchClient m_TwitchClient;
        private HttpListener m_HttpListener;
        private Thread m_HttpListenerThread;
        private bool m_HttpListenerThreadLoop;
        private ManualResetEvent m_HttpListenerReset;
        private DateTime m_StartTime;


        private int m_ReconnectAttempts = 0;
        private const int MAX_RECONNECT_ATTEMPS = 15;

        public int Id { get { return m_BotId; } }

        public Bot(BotSettings settings)
        {
            m_BotId = settings.Id;
            m_Name = settings.Name;
            m_Password = settings.Password;
            m_CallbackPort = settings.CallbackPort;
            m_PluginManager = new PluginManager(this);
            settings.Plugins.ToList().ForEach(p => m_PluginManager.AddPlugin(p.PluginName));
            m_PluginManager.LoadAllPlugins();
        }

        public void Start()
        {
            if (m_HttpListenerThread == null)
            {
                m_HttpListenerThread = new Thread(new ThreadStart(HttpListenerThread));
                m_HttpListenerThread.Start();
            }

            m_TwitchClient = new TwitchClient(m_Name, m_Password);
            m_TwitchClient.OnDisconnect += m_TwitchClient_OnDisconnect;
            m_TwitchClient.OnJoin += m_TwitchClient_OnJoin;
            m_TwitchClient.OnMessage += m_TwitchClient_OnMessage;
            m_TwitchClient.OnPerform += m_TwitchClient_OnPerform;
            m_TwitchClient.OnNotice += m_TwitchClient_OnNotice;
            if (Environment.MachineName == "KERNEL01")
                m_TwitchClient.LogLevel = Twitch.Models.MessageLevel.Debug;
            else
                m_TwitchClient.LogLevel = Twitch.Models.MessageLevel.Info;
            m_TwitchClient.AutoDetectSendWhispers = true;
            m_TwitchClient.Connect();
            m_StartTime = DateTime.Now;
        }

        void HttpListenerThread()
        {
            if (m_CallbackPort == 0)
                return;
            m_HttpListener = new HttpListener();
            m_HttpListener.Prefixes.Add("http://localhost:" + m_CallbackPort + "/");
            m_HttpListener.Start();
            m_HttpListenerThreadLoop = true;
            m_HttpListenerReset = new ManualResetEvent(false);
            Console.WriteLine("[" + DateTime.Now.ToString() + "] Started HTTPListener on port " + m_CallbackPort.ToString());

            while (m_HttpListenerThreadLoop)
            {
                try
                {
                    m_HttpListenerReset.Reset();
                    IAsyncResult result = m_HttpListener.BeginGetContext(new AsyncCallback(ListenerCallback), this);
                    m_HttpListenerReset.WaitOne();
                }
                catch (Exception e)
                {
                    Console.WriteLine("[" + DateTime.Now.ToString() + "] Exception in HTTPListener on port " + m_CallbackPort.ToString());
                    Console.WriteLine(e);
                    break;
                }
            }
            Console.WriteLine("[" + DateTime.Now.ToString() + "] Closing HTTPListener on port " + m_CallbackPort.ToString());
        }

        public void ListenerCallback(IAsyncResult result)
        {
            Bot bot = (Bot)result.AsyncState;
            // Call EndGetContext to complete the asynchronous operation.
            HttpListenerContext context = bot.m_HttpListener.EndGetContext(result);
            HttpListenerRequest request = context.Request;
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            bool noBody = false;
            StringBuilder sb = new StringBuilder();
            try
            {

                if (request.Url.AbsoluteUri.EndsWith("status"))
                {
                    long time = DateTime.Now.Ticks - bot.m_TwitchClient.NextKeepAlive.Ticks;
                    string ip = null;
                    try
                    {
                        ip = bot.m_TwitchClient.GetIPConnected();
                    }
                    catch (SocketException)
                    {
                        // Handled
                    }
                    if (time <= TimeSpan.TicksPerHour * 2 && ip != null)
                    {
                        sb.AppendLine("[STATUS=OK]");
                    }
                    else
                    {
                        sb.AppendLine("[STATUS=ERROR]");
                    }
                    sb.Append("Start time : ");
                    sb.AppendLine(bot.m_StartTime.ToString());
                    sb.Append("Last keep alive : ");
                    sb.AppendLine(bot.m_TwitchClient.LastKeepAlive.ToString());
                    sb.Append("Next keep alive : ");
                    sb.AppendLine(bot.m_TwitchClient.NextKeepAlive.ToString());
                    sb.Append("Connected to : " + ip ?? "Not connected");
                    sb.AppendLine();
                    sb.Append("Number of parsed messages in the last 30 minutes : ");
                    sb.AppendLine(bot.m_PluginManager.NumberOfParsedMessagesRecently.ToString());
                    sb.Append("Number of runned commands in the last 30 minutes : ");
                    sb.AppendLine(bot.m_PluginManager.NumberOfRunnedCommandsRecently.ToString());
                }
                else if (request.Url.AbsoluteUri.EndsWith("triggerKeepAlive"))
                {
                    sb.AppendLine("Triggering keep alive");
                    bot.m_TwitchClient.TriggerNextKeepAlive();
                }
                else if (request.Url.AbsoluteUri.EndsWith("allCommands"))
                {
                    foreach (string s in bot.m_PluginManager.AllCommands)
                    {
                        sb.AppendLine(s);
                    }
                }
                else
                {
                    noBody = true;
                    response.StatusCode = 404;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[" + DateTime.Now.ToString() + "] Exception in ListenerCallback on port " + m_CallbackPort.ToString());
                Console.WriteLine(ex);

                noBody = true;
                response.StatusCode = 500;
            }

            if (noBody)
            {
                response.OutputStream.Close();
            }
            else
            {
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(sb.ToString()); ;
                // Get a response stream and write the response to it.
                response.ContentType = " text/plain";
                response.ContentLength64 = buffer.Length;

                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);

                // You must close the output stream.
                output.Close();
            }
            bot.m_HttpListenerReset.Set();
        }


        void m_TwitchClient_OnNotice(TwitchClient sender, Twitch.Models.TwitchNotice args)
        {
            m_PluginManager.OnNotice(sender, args);
        }

        void m_TwitchClient_OnPerform(TwitchClient sender)
        {
            var channel_list = BotSettings.GetById(m_BotId).Channels;
            channel_list.Where(c => c.AutoJoin == BotChannel.AutoJoinSettings.Yes)
                .ToList()
                .ForEach(c => sender.Join(c.Channel));
        }

        void m_TwitchClient_OnMessage(TwitchClient sender, Twitch.Models.TwitchMessage args)
        {
            m_PluginManager.OnMessage(sender, args);
        }

        void m_TwitchClient_OnJoin(TwitchClient sender, Twitch.Models.TwitchClientOnJoinEventArgs args)
        {
            m_ReconnectAttempts = 0;
            if (args.IsMyself)
            {
                if (BotSettings.GetById(m_BotId).GetChannel(args.Channel).Greetings == BotChannel.GreetingsTypes.Simple)
                {
                    if (System.Diagnostics.Debugger.IsAttached)
                        sender.SendMessage(args.Channel, "Joined [DEBUG]");
                    else
                        sender.SendMessage(args.Channel, "Joined");
                }
            }
        }

        void m_TwitchClient_OnDisconnect(TwitchClient sender, bool wasManualDisconnect)
        {
            Console.WriteLine("[" + DateTime.Now.ToString() + "] Disconnected : wasManualDisconnect = " + wasManualDisconnect.ToString() + " ; m_ReconnectAttempts = " + m_ReconnectAttempts.ToString());
            if (wasManualDisconnect)
                return;
            if (m_ReconnectAttempts < MAX_RECONNECT_ATTEMPS)
            {
                m_TwitchClient.Disconnect();
                m_ReconnectAttempts++;
                int delay = Math.Min(3600 * 1000, (int)Math.Pow(3, m_ReconnectAttempts) * 1000);
                Console.WriteLine("[" + DateTime.Now.ToString() + "]Next reconnect in " + delay + " ms");
                Thread.Sleep(delay);

                Start();
            }
            else
            {
                Console.WriteLine("[" + DateTime.Now.ToString() + "]Maximum attempts reached, closing program");
                Environment.Exit(1);
            }
        }

    }
}
