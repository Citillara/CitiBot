using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Plugins.GenericCommands
{
    public class GenericCommands : IPlugin
    {
        public void Load(CommandsManager commandsManager)
        {
            commandsManager.RegisterCommand("!join", DoJoin);
            commandsManager.RegisterCommand("!part", DoPart);
            commandsManager.RegisterCommand("!pyramid", DoPyramid);
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
    }

}
