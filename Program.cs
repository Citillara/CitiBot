using CitiBot.Plugins;
using CitiBot.Plugins.CookieGiver;
using CitiBot.Plugins.CookieGiver.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot
{
    class Program
    {

        public struct Activities
        {
            public string Name;
            public int Amount;
        }

        static void Main(string[] args)
        {
            new Program().MainLoop();
        }

        string name = ConfigurationManager.ConnectionStrings["TwitchLogin"].ConnectionString;
        string password = ConfigurationManager.ConnectionStrings["TwitchPassword"].ConnectionString;
        TwitchClient client;
        CookieGiver cookieGiver = new CookieGiver();
        Regex r = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);

        void MainLoop()
        {
            client = new TwitchClient(name, password);
            client.OnMessage += client_OnMessage;
            client.OnPerform += client_OnPerform;
            client.OnPart += client_OnPart;
            client.AutoDetectSendWhispers = true;
            client.Connect();

            while (true)
            {
                string line = Console.ReadLine();
            }
        }


        void client_OnPart(TwitchClient sender, Twitch.Models.TwitchClientOnPartEventArgs args)
        {

        }

        void client_OnPerform(TwitchClient sender)
        {
            sender.Join("#citillara");
            sender.Join("#elbodykso");
            //sender.Join("#infoutlaw");
            //sender.Join("#nickynoel");
            //sender.Join("#crumps2");

            //sender.Join("#mlg");
            /*
            sender.Join("#daopa");
            sender.Join("#zarvoxtoral");
            sender.Join("#themittanidotcom");
            sender.Join("#crasskitty");
            sender.Join("#angerbeard");*/
        }


        void client_OnMessage(TwitchClient sender, TwitchMessage message)
        {
            if(DoGenericCommands(sender, message))
                return;
            cookieGiver.OnMessage(sender, message);

        }


        bool DoGenericCommands(TwitchClient sender, TwitchMessage message)
        {
            if (!message.Message.StartsWith("!"))
                return false;
            var split = message.Message.Split(' ');
            string msg = split[0];

            switch (msg)
            {
                case "!join" :
                    if (message.UserType < TwitchUserTypes.BotMaster)
                        sender.SendMessage(message.Channel, "Sorry {0}, but that command is currently restricted to Bot Admins", message.SenderDisplayName);
                    else
                        if (split.Length > 1)
                            if (split[1].StartsWith("#"))
                            {
                                sender.Join(split[1]);
                                sender.SendMessage("#citillara", "Joining {0} on behalf of {1}", split[1], message.SenderDisplayName);
                            }
                            else
                                sender.SendMessage(message.Channel, "Please specify a correct channel");
                        else
                        {
                            sender.Join("#" + message.SenderName);
                            sender.SendMessage("#citillara", "Joining {0} on behalf of {1}", "#" + message.SenderName, message.SenderDisplayName);
                        }
                    return true;
                case "!part":
                    if (message.UserType < TwitchUserTypes.Broadcaster)
                        sender.SendMessage(message.Channel, "Sorry {0}, but this command is rectricted to Broadcaster and above", message.SenderDisplayName);
                    else
                    {
                        sender.Part(message.Channel);
                        sender.SendMessage("#citillara", "Parting {0} on behalf of {1}", message.Channel, message.SenderDisplayName);
                    }
                    return true;

                case "!pyramid":
                    if (message.UserType < TwitchUserTypes.Citillara)
                        sender.SendMessage(message.Channel, "Sorry {0}, but this command is rectricted to Citillara and above", message.SenderDisplayName);
                    else
                    {
                        var icon = split[1] + " ";
                        sender.SendMessage(message.Channel, icon);
                        sender.SendMessage(message.Channel, icon + icon);
                        sender.SendMessage(message.Channel, icon + icon + icon);
                        sender.SendMessage(message.Channel, icon + icon + icon + icon);
                        sender.SendMessage(message.Channel, icon + icon + icon);
                        sender.SendMessage(message.Channel, icon + icon);
                        sender.SendMessage(message.Channel, icon);
                    }
                    return true;
                    break;
                default: return false;
            }
        }
    }

    public static class Extensions
    {
        public static string[] Split(this string s, char c)
        {
            return s.Split(new char[] { c }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
