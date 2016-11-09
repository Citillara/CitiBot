using CitiBot.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Plugins.GenericCommands
{
    public class GenericCommands : IPlugin
    {
        private Random m_random = new Random();
        public readonly string TWITCH_URL = "https://twitch.tv/";
        public readonly int RAID_REPEAT = 4;
        private Dictionary<string, string> m_raidMessages = new Dictionary<string, string>();
        private DateTime m_startTime = DateTime.Now;


        public void OnLoad(PluginManager pluginManager)
        {
            pluginManager.RegisterCommand("!join", DoJoin);
            pluginManager.RegisterCommand("!part", DoPart);
            pluginManager.RegisterCommand("!pyramid", DoPyramid);
            pluginManager.RegisterCommand("!raid", DoRaid);
            pluginManager.RegisterCommand("!raidmessage", DoRaidMessage);
            pluginManager.RegisterCommand("!d2", DoRoll);
            pluginManager.RegisterCommand("!d4", DoRoll);
            pluginManager.RegisterCommand("!d6", DoRoll);
            pluginManager.RegisterCommand("!d8", DoRoll);
            pluginManager.RegisterCommand("!d10", DoRoll);
            pluginManager.RegisterCommand("!d12", DoRoll);
            pluginManager.RegisterCommand("!d20", DoRoll);
            pluginManager.RegisterCommand("!d100", DoRoll);
            pluginManager.RegisterCommand("!roll", DoRoll);
            pluginManager.RegisterCommand("!timediff", DoTimeDiff);
            pluginManager.RegisterCommand("!uptime", DoUptime);
            pluginManager.RegisterCommand("!version", DoVersion);
        }


        public void DoJoin(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Developper)
            {
                sender.SendWhisper(message.SenderName, "Sorry {0}, but that command is currently restricted to Bot Admins", message.SenderDisplayName);
            }
            else
            {
                if (message.Args.Length > 1)
                {
                    if (message.Args[1].StartsWith("#"))
                    {
                        sender.Join(message.Args[1]);
                        sender.SendMessage("#citillara", "Joining {0} on behalf of {1}", message.Args[1], message.SenderDisplayName);
                    }
                    else
                    {
                        sender.SendMessage(message.Channel, "Please specify a correct channel");
                    }
                }
                else
                {
                    sender.Join("#" + message.SenderName);
                    sender.SendMessage("#citillara", "Joining {0} on behalf of {1}", "#" + message.SenderName, message.SenderDisplayName);
                }
            }
        }
        public void DoPart(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Broadcaster)
            {
                sender.SendWhisper(message.SenderName, "Sorry {0}, but this command is rectricted to Broadcaster and above", message.SenderDisplayName);
            }
            else
            {
                sender.Part(message.Channel);
                sender.SendMessage("#citillara", "Parting {0} on behalf of {1}", message.Channel, message.SenderDisplayName);
            }
        }
        public void DoPyramid(TwitchClient sender, TwitchMessage message)
        {

            if (message.UserType < TwitchUserTypes.Citillara)
            {
                //sender.SendMessage(message.Channel, "Sorry {0}, but this command is rectricted to Citillara", message.SenderDisplayName);
            }
            else
            {
                var icon = message.Args[1] + " ";
                sender.SendMessage(message.Channel, icon);
                sender.SendMessage(message.Channel, icon + icon);
                sender.SendMessage(message.Channel, icon + icon + icon);
                sender.SendMessage(message.Channel, icon + icon + icon + icon);
                sender.SendMessage(message.Channel, icon + icon + icon);
                sender.SendMessage(message.Channel, icon + icon);
                sender.SendMessage(message.Channel, icon);
            }
        }
        public void DoRaid(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType >= TwitchUserTypes.Broadcaster)
            {
                if (message.Args.Length > 1)
                {
                    if (m_raidMessages.ContainsKey(message.Channel) && !String.IsNullOrEmpty(m_raidMessages[message.Channel]))
                    {
                        RepeatAction(RAID_REPEAT, () => sender.SendMessage(message.Channel, m_raidMessages[message.Channel]));
                    }
                    RepeatAction(RAID_REPEAT, () => sender.SendMessage(message.Channel, TWITCH_URL + message.Args[1].ToLowerInvariant()));
                }
            }
        }
        public void DoRaidMessage(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType >= TwitchUserTypes.Broadcaster)
            {
                if (message.Args.Length > 1)
                {
                    string msg = message.Message.Substring(message.Args[0].Length + 1).Trim();
                    m_raidMessages[message.Channel] = msg;
                    sender.SendMessage(message.Channel, "Raid message has been set to : " + msg);
                }
            }
        }
        public void DoRoll(TwitchClient sender, TwitchMessage message)
        {
            int roll = 0;
            switch (message.Command) {
                case "!d2": roll = 2; break;
                case "!d3": roll = 3; break;
                case "!d4": roll = 4; break;
                case "!d6": roll = 6; break;
                case "!d8": roll = 8; break;
                case "!d10": roll = 10; break;
                case "!d12": roll = 12; break;
                case "!d20": roll = 20; break;
                case "!d100": roll = 100; break;
                case "!roll":
                    if (message.Args.Length < 2 || !int.TryParse(message.Args[1], out roll) || roll <= 0)
                        return;
                    else break;
                default: return;
            }
            sender.AutoDetectSendWhispers = true;
            sender.SendMessage(message.Channel, string.Format("{0} rolls a {1}", message.SenderDisplayName, m_random.Next(1, roll + 1)));
        }
        public void DoTimeDiff(TwitchClient sender, TwitchMessage message)
        {
            if (message.Args.Length != 3)
            {
                return;
            }
            TimeSpan ts1, ts2;
            bool bts1 = TimeSpan.TryParse(message.Args[1], out ts1);
            bool bts2 = TimeSpan.TryParse(message.Args[2], out ts2);
            if (!bts1 || !bts2)
            {
                return;
            }
            var tsr = new TimeSpan(Math.Abs(ts1.Ticks - ts2.Ticks));
            sender.SendMessage(message.Channel, tsr.ToString());
        }
        public void DoUptime(TwitchClient sender, TwitchMessage message)
        {
            var d = new TimeSpan(DateTime.Now.Ticks - m_startTime.Ticks);
            sender.SendMessage(message.Channel, "{0}d {1}h {2}m {3}s", d.Days, d.Hours, d.Minutes, d.Seconds);
        }
        public void DoVersion(TwitchClient sender, TwitchMessage message)
        {
            sender.SendMessage(message.Channel, "IRC {0}, Twitch {1}, CitiBot {2}", Irc.IrcClient.Version, TwitchClient.Version, Program.Version);
        }

        private static void RepeatAction(int repeatCount, Action action, int delay = 200)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                Thread.Sleep(delay);
                action();
            }
        }
    }

}
