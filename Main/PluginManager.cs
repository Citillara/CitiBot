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

        private Dictionary<string, Action<TwitchClient, TwitchMessage>> m_commands;

        private List<IPlugin> m_plugins;

        public PluginManager()
        {
            m_commands = new Dictionary<string, Action<TwitchClient, TwitchMessage>>();
            m_plugins = new List<IPlugin>();
        }

        public void LoadAllPlugins()
        {
            m_plugins.ForEach(p => p.OnLoad(this));
        }


        public void AddPlugin(string plugin)
        {
            IPlugin p = (IPlugin)Activator.CreateInstance(m_availablePlugins[plugin]);
            m_plugins.Add(p);
        }

        public void RegisterCommand(string command, Action<TwitchClient, TwitchMessage> action)
        {
            m_commands.Add(command, action);
        }

        public void OnMessage(TwitchClient client, TwitchMessage message)
        {
            if (!message.Message.StartsWith("!"))
                return;
            var split = message.Message.Split(' ');
            string msg = split[0];

            try
            {
                Action<TwitchClient, TwitchMessage> action;
                if (m_commands.TryGetValue(msg, out action))
                    action.Invoke(client, message);
            }
            catch(Exception e)
            {
                Console.WriteLine(message);
                Console.WriteLine(e.ToString());
            }
        }
    }
}
