using CitiBot.Plugins;
using CitiBot.Plugins.CookieGiver;
using CitiBot.Plugins.CookieGiver.Models;
using System;
using System.Collections.Generic;
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


        static void Main(string[] args)
        {
            new Program().MainLoop();
        }

        string name;
        string password;
        TwitchClient client;
        CookieGiver cookieGiver = new CookieGiver();
        Regex r = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);

        void MainLoop()
        {
            if (!LoadInit())
                return;

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


        bool LoadInit()
        {
            if (!File.Exists("config.ini"))
                return false;
            var l = File.ReadAllLines("config.ini");
            foreach (string line in l)
            {
                if (line.StartsWith("name="))
                {
                    var spl = line.Split('=');
                    if (spl.Length > 1)
                        name = spl[1];
                }
                else if (line.StartsWith("password="))
                {
                    var spl = line.Split('=');
                    if (spl.Length > 1)
                        password = spl[1];
                }
            }
            if (string.IsNullOrEmpty(name))
                return false;
            if (string.IsNullOrEmpty(password))
                return false;

            return true;
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
