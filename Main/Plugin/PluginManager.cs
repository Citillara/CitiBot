using CitiBot.Database;
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

        };

        private Dictionary<string, OnMessageAction> m_commands;

        private List<Plugin> m_plugins;

        public PluginManager()
        {
            m_commands = new Dictionary<string, OnMessageAction>();
            m_plugins = new List<Plugin>();
        }

        public void LoadAllPlugins()
        {
            m_plugins.ForEach(p => p.OnLoad(this));
        }


        public void AddPlugin(string plugin)
        {
            Plugin p = (Plugin)Activator.CreateInstance(m_availablePlugins[plugin]);
            m_plugins.Add(p);
        }

        public void RegisterCommand(OnMessageAction command)
        {
            command.Commands.ForEach(c => m_commands.Add(c, command));
        }

        public void OnMessage(TwitchClient client, TwitchMessage message)
        {
            if (!message.Message.StartsWith("!"))
                return;
            var split = message.Message.Split(' ');
            string msg = split[0];

            try
            {
                OnMessageAction action;
                if (m_commands.TryGetValue(msg, out action))
                {
                    action.OnMessage(client, message);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine();
                Console.WriteLine(DateTime.Now.ToString());
                Console.WriteLine();
                Console.WriteLine(message);
                Console.WriteLine();
                Console.WriteLine(e.ToString());
                Console.WriteLine();
                Console.WriteLine("--------------------------------------------");
                Console.WriteLine();
            }
        }

        public class OnMessageAction
        {
            protected internal Plugin Plugin;

            protected internal Action<TwitchClient, TwitchMessage> Action;

            protected internal List<string> Commands = new List<string>();

            public int GlobalCooldown = 0;
            public int UserCooldown = 0;
            public int UserChannelCooldown = 0;
            public int ChannelCooldown = 0;

            private Dictionary<string, DateTime> m_cooldowns = new Dictionary<string,DateTime>();

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

                this.Plugin.BeforeCommand(client, message);

                this.Action.Invoke(client, message);

                this.Plugin.AfterCommand(client, message);

                if (Registry.Instance != null)
                    Registry.Instance.Close();
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
