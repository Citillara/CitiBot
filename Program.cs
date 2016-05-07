using CitiBot.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            cookieGiver.OnMessage(sender, message);
            if (message.Message.StartsWith("!now"))
            {
                sender.SendMessage(message.Channel, DateTime.Now.ToLongTimeString());
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
