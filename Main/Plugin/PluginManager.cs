//#define BREAK

using CitiBot.Database;
using CitiBot.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Main
{
    public class PluginManager
    {
        private static Dictionary<string, Type> m_availablePlugins = new Dictionary<string, Type>()
        { 
            { "GenericCommands", typeof(CitiBot.Plugins.GenericCommands.GenericCommands) },
            { "CookieGiver", typeof(CitiBot.Plugins.CookieGiver.CookieGiver) },
            { "Dog", typeof(CitiBot.Plugins.Dog.Dog) },
            { "Counter", typeof(CitiBot.Plugins.Counters.CountersCommands) },
            { "Moderation", typeof(CitiBot.Plugins.Moderation.Moderation) },
        };

        private Dictionary<string, OnMessageAction> m_commands;

        private List<Plugin> m_plugins;

        private int m_BotId;
        
        private StatsCounter m_NumberOfParsedMessages;
        private StatsCounter m_NumberOfRunnedCommands;

        public long NumberOfParsedMessagesRecently { get { return m_NumberOfParsedMessages.GetCount(); } }
        public long NumberOfRunnedCommandsRecently { get { return m_NumberOfRunnedCommands.GetCount(); } }

        public int BotId { get { return m_BotId; } }

        public string[] AllCommands { get { return m_commands.Keys.ToArray(); } }

        public PluginManager(Bot bot)
        {
            m_commands = new Dictionary<string, OnMessageAction>();
            m_plugins = new List<Plugin>();
            m_NumberOfParsedMessages = new StatsCounter(new TimeSpan(0, 30, 0));
            m_NumberOfRunnedCommands = new StatsCounter(new TimeSpan(0, 30, 0));
            m_BotId = bot.Id;
        }

        public void LoadAllPlugins()
        {
            m_plugins.ForEach(p => p.OnLoad(this));
        }


        public void AddPlugin(string plugin)
        {
            Plugin p = (Plugin)Activator.CreateInstance(m_availablePlugins[plugin]);
            p.Manager = this;
            m_plugins.Add(p);
        }

        public void RegisterCommand(OnMessageAction command)
        {
            command.Commands.ForEach(c => m_commands.Add(c, command));
        }

        public void OnNotice(TwitchClient client, TwitchNotice notice)
        {
            try
            {
                // TODO : Rework that part to do like the normal message
                m_plugins.ForEach(p => p.OnNotice(client, notice));
            }
            catch (Exception e)
            {
#if BREAK
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
#endif
                Console.Write("[");
                Console.Write(DateTime.Now.ToString());
                Console.Write("] ");
                Console.WriteLine(notice);
                Console.WriteLine();
                Console.WriteLine(e.ToString());
                Console.WriteLine();
            }
        }

        public void OnMessage(TwitchClient client, TwitchMessage message)
        {
            try
            {
                m_NumberOfParsedMessages.IncrementCounter();

                m_plugins.ForEach(p => p.OnMessage(client, message));

                if (message.BitsSent != 0)
                {
                    m_plugins.ForEach(p => p.OnBitsSent(client, message));
                }

                if (message.Message.StartsWith("!"))
                {
                    var split = message.Message.Split(' ');
                    string msg = split[0];

                    OnMessageAction action;
                    if (m_commands.TryGetValue(msg, out action))
                    {
                        m_NumberOfRunnedCommands.IncrementCounter();
                        action.OnMessage(client, message);
                    }
                }
            }
            catch(Exception e)
            {
#if BREAK
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
#endif
                Console.Write("[");
                Console.Write(DateTime.Now.ToString());
                Console.Write("] ");
                Console.WriteLine(message);
                Console.WriteLine();
                Console.WriteLine(e.ToString());
                Console.WriteLine();
            }
        }

        public class OnMessageAction
        {
            protected internal Plugin Plugin;

            protected internal Action<TwitchClient, TwitchMessage> Action;

            protected internal List<string> Commands = new List<string>();

            public int GlobalCooldown { get; set; }
            public int UserCooldown { get; set; }
            public int UserChannelCooldown { get; set; }
            public int ChannelCooldown { get; set; }

            private Dictionary<string, DateTime> m_cooldowns = new Dictionary<string,DateTime>();

            public OnMessageAction(Plugin plugin, Action<TwitchClient, TwitchMessage> action)
            {
                this.Plugin = plugin;
                this.Action = action;
                this.Commands = new List<string>();
            }
            public OnMessageAction(Plugin plugin, Action<TwitchClient, TwitchMessage> action, string command)
            {
                this.Plugin = plugin;
                this.Action = action;
                this.Commands.Add(command);
            }

            public OnMessageAction(Plugin plugin, Action<TwitchClient, TwitchMessage> action, params string[] command)
            {
                this.Plugin = plugin;
                this.Action = action;
                this.Commands.AddRange(command);
            }

            protected internal void OnMessage(TwitchClient client, TwitchMessage message)
            {
                if (GlobalCooldown > 0)
                    if (!CheckKey("######", GlobalCooldown))
                        return;
                if (UserCooldown > 0)
                    if (!CheckKey(message.SenderName, UserCooldown))
                        return;
                if (ChannelCooldown > 0)
                    if (!CheckKey(message.Channel, ChannelCooldown))
                        return;
                if (UserChannelCooldown > 0)
                    if (!CheckKey(message.Channel + message.SenderName, UserChannelCooldown))
                        return;

                if (Registry.Instance != null)
                    Registry.Instance.Close();
                try
                {
                    Log.AddTechnicalLog(DateTime.Now, Log.LogLevel.Debug, this.Plugin.Manager.m_BotId.ToString(), this.Action.Method.Name, message.ToString());

                    bool runNext = this.Plugin.BeforeCommand(client, message);

                    if (runNext)
                    {
                        this.Action.Invoke(client, message);

                        this.Plugin.AfterCommand(client, message);
                    }

                    Registry.Instance.SaveChanges();
                }
                catch (System.IO.IOException)
                {
                    if (Registry.Instance != null)
                        Registry.Instance.Close();
                    throw;
                }
                catch (Exception e)
                {
                    Log.AddTechnicalLog(DateTime.Now, Log.LogLevel.Warn, this.Plugin.Manager.m_BotId.ToString(), this.Action.Method.Name, message.ToString());
                    if (Registry.Instance != null)
                        Registry.Instance.Close();
#if BREAK
                    if (System.Diagnostics.Debugger.IsAttached)
                        System.Diagnostics.Debugger.Break();
#endif
                    Console.Write("[");
                    Console.Write(DateTime.Now.ToString());
                    Console.Write("] ");
                    Console.WriteLine("OnMessageAction.OnMessage exception");
                    Console.WriteLine(message);
                    Console.WriteLine();
                    if (e is System.Data.Entity.Infrastructure.DbUpdateException)
                    {
                        Console.WriteLine("System.Data.Entity.Infrastructure.DbUpdateException");
                        Console.WriteLine(e.Message);
                    }
                    else
                    {
                        Console.WriteLine(e.ToString());
                    }
                    Console.WriteLine();

                }
                if (Registry.Instance != null)
                {
                    Registry.Instance.SaveChanges();
                    Registry.Instance.Close();
                }
            }

            private bool CheckKey(string key, int cooldown)
            {
                DateTime date = DateTime.MinValue;
                DateTime now = DateTime.Now;
                bool found = m_cooldowns.TryGetValue(key, out date);
                if (!found)
                {
                    m_cooldowns.Add(key, now);
                    return true;
                }
                bool result = now > date.AddSeconds(cooldown);
                if (result)
                    m_cooldowns[key] = now;
                return result;
            }
        }
    }
}
