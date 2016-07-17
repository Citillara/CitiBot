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
        public const string TWITCH_URL = "https://twitch.tv/";
        public const int RAID_REPEAT = 4;
        private Dictionary<string, string> m_raidMessages = new Dictionary<string, string>();


        public void Load(CommandsManager commandsManager)
        {
            commandsManager.RegisterCommand("!join", DoJoin);
            commandsManager.RegisterCommand("!part", DoPart);
            commandsManager.RegisterCommand("!pyramid", DoPyramid);
            commandsManager.RegisterCommand("!count", DoCount);
            commandsManager.RegisterCommand("!raid", DoRaid);
            commandsManager.RegisterCommand("!raidmessage", DoRaidMessage);
        }

        public void DoJoin(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.BotMaster)
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
        public void DoCount(TwitchClient sender, TwitchMessage message)
        {

            if (message.UserType >= TwitchUserTypes.Citillara)
            {
                sender.SendMessage(message.Channel, message.Args[1] + " = " + Program.Channels[message.Args[1]].Count().ToString());
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

        public static void RepeatAction(int repeatCount, Action action, int delay = 200)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                Thread.Sleep(delay);
                action();
            }
        }

    }

}
