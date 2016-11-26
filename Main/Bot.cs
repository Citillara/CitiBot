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
        private BotSettings m_BotSettings;
        private PluginManager m_PluginManager;
        private TwitchClient m_TwitchClient;

        private int m_ReconnectAttempts = 0;
        private const int MAX_RECONNECT_ATTEMPS = 3;

        public Bot(BotSettings settings)
        {
            m_PluginManager = new PluginManager();
            m_BotSettings = settings;
            m_BotSettings.Plugins.ToList().ForEach(p => m_PluginManager.AddPlugin(p.PluginName));
            m_PluginManager.LoadAllPlugins();
        }

        public void Start()
        {
            m_TwitchClient = new TwitchClient(m_BotSettings.Name, m_BotSettings.Password);
            m_TwitchClient.OnDisconnect += m_TwitchClient_OnDisconnect;
            m_TwitchClient.OnJoin += m_TwitchClient_OnJoin;
            m_TwitchClient.OnMessage += m_TwitchClient_OnMessage;
            m_TwitchClient.OnPart += m_TwitchClient_OnPart;
            m_TwitchClient.OnPerform += m_TwitchClient_OnPerform;
            m_TwitchClient.LogLevel = Twitch.Models.MessageLevel.Debug;
            m_TwitchClient.AutoDetectSendWhispers = true;
            m_TwitchClient.Connect();
        }

        void m_TwitchClient_OnPerform(TwitchClient sender)
        {
            m_BotSettings.Channels.ToList().ForEach(c => sender.Join(c.Channel));
        }

        void m_TwitchClient_OnMessage(TwitchClient sender, Twitch.Models.TwitchMessage args)
        {
            m_PluginManager.OnMessage(sender, args);
        }

        void m_TwitchClient_OnJoin(TwitchClient sender, Twitch.Models.TwitchClientOnJoinEventArgs args)
        {
            if(args.IsMyself && m_BotSettings.Channels.Where(c => c.Channel == args.Channel).FirstOrDefault().Greetings == 1)
                sender.SendMessage(args.Channel, "Joined");
        }

        void m_TwitchClient_OnPart(TwitchClient sender, Twitch.Models.TwitchClientOnPartEventArgs args)
        {

        }

        
        void m_TwitchClient_OnDisconnect(TwitchClient sender, bool wasManualDisconnect)
        {
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
                Console.WriteLine("Preparing " + bs.Name);
                var bot = new Bot(bs);
                list.Add(bot);
                bot.Start();
            }
            return list;
        }
    }
}
