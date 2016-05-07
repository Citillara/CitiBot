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
            //sender.Join("#elbodykso");
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

        DateTime nextAllowed = DateTime.Now;

        void client_OnMessage(TwitchClient sender, TwitchClientOnMessageEventArgs args)
        {
            Console.WriteLine(args.TwitchMessage.ToString());
            //cookieGiver.OnMessage(sender, args);

            if (nextAllowed > DateTime.Now)
                return;

            if (args.TwitchMessage.Message.StartsWith("!randomtopic"))
            {
                sender.SendMessage(args.TwitchMessage.Channel, GetARandomTopic().Substring(39));
            }

            nextAllowed = DateTime.Now.AddSeconds(1);
        }


        string GetARandomTopic()
        {
            string response;
            HttpWebRequest request = WebRequest.CreateHttp("http://www.conversationstarters.com/random.php");
            using (StreamReader sr = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                response = sr.ReadToEnd();
            }
            return response;
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
