using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot
{
    public class CommandsManager
    {
        private Dictionary<string, Action<TwitchClient, TwitchMessage>> m_commands;

        private List<IPlugin> m_plugins;

        public CommandsManager()
        {
            m_commands = new Dictionary<string, Action<TwitchClient, TwitchMessage>>();
            m_plugins = new List<IPlugin>();
        }

        public void LoadAllPlugins()
        {
            m_plugins.ForEach(p => p.Load(this));
        }


        public void AddPlugin(IPlugin plugin)
        {
            m_plugins.Add(plugin);
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

            if (m_commands.ContainsKey(msg))
                m_commands[msg].Invoke(client, message);
        }
    }
}
