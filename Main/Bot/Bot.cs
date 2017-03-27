using CitiBot.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch;

namespace CitiBot.Main
{
    public class Bot
    {
        private int m_BotId;
        private string m_Name;
        private string m_Password;
        private PluginManager m_PluginManager;
        private TwitchClient m_TwitchClient;

        private int m_ReconnectAttempts = 0;
        private const int MAX_RECONNECT_ATTEMPS = 3;

        public Bot(BotSettings settings)
        {
            m_PluginManager = new PluginManager();
            m_BotId = settings.Id;
            m_Name = settings.Name;
            m_Password = settings.Password;
            settings.Plugins.ToList().ForEach(p => m_PluginManager.AddPlugin(p.PluginName));
            m_PluginManager.LoadAllPlugins();
        }

        public void Start()
        {
            m_TwitchClient = new TwitchClient(m_Name, m_Password);
            m_TwitchClient.OnDisconnect += m_TwitchClient_OnDisconnect;
            m_TwitchClient.OnJoin += m_TwitchClient_OnJoin;
            m_TwitchClient.OnMessage += m_TwitchClient_OnMessage;
            m_TwitchClient.OnPart += m_TwitchClient_OnPart;
            m_TwitchClient.OnPerform += m_TwitchClient_OnPerform;
            if (Environment.MachineName == "KERNEL01")
                m_TwitchClient.LogLevel = Twitch.Models.MessageLevel.Debug;
            else
                m_TwitchClient.LogLevel = Twitch.Models.MessageLevel.Info;
            m_TwitchClient.AutoDetectSendWhispers = true;
            m_TwitchClient.Connect();
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
            if (args.IsMyself)
            {
                if (BotSettings.GetById(m_BotId).GetChannel(args.Channel).Greetings == BotChannel.GreetingsTypes.Simple)
                {
                    sender.SendMessage(args.Channel, "Joined");
                }
            }
        }

        void m_TwitchClient_OnPart(TwitchClient sender, Twitch.Models.TwitchClientOnPartEventArgs args)
        {

        }

        
        void m_TwitchClient_OnDisconnect(TwitchClient sender, bool wasManualDisconnect)
        {
            Console.WriteLine(DateTime.Now.ToString() + " Disconnected : wasManualDisconnect = " + wasManualDisconnect.ToString() + " ; m_ReconnectAttempts = " + m_ReconnectAttempts.ToString());
            if (!wasManualDisconnect && m_ReconnectAttempts < 3)
            {
                Start();
                m_ReconnectAttempts++;
            }
        }

        public static IEnumerable<Bot> StartAllBots()
        {
            var list = new List<Bot>();
            var settings = BotSettings.GetAllBots();
            foreach (var bs in settings)
            {
                Console.WriteLine(DateTime.Now.ToString() +  " + Preparing " + bs.Name);
                var bot = new Bot(bs);
                list.Add(bot);
                bot.Start();
            }
            return list;
        }
    }
}
