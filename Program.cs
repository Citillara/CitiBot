using CitiBot.Plugins;
using CitiBot.Plugins.CookieGiver;
using CitiBot.Plugins.CookieGiver.Models;
using CitiBot.Plugins.GenericCommands;
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
        
        Regex r = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);
        public static Dictionary<string, List<string>> Channels = new Dictionary<string, List<string>>();

        CommandsManager m_commandManager;


        void MainLoop()
        {
            Load();
            client = new TwitchClient(name, password);
            client.OnMessage += client_OnMessage;
            client.OnPerform += client_OnPerform;
            client.OnPart += client_OnPart;
            client.OnJoin += client_OnJoin;
            client.AutoDetectSendWhispers = true;
            client.LogLevel = Irc.MessageLevel.Info;
            client.Connect();

            while (true)
            {
                string line = Console.ReadLine();
            }
        }

        void client_OnJoin(TwitchClient sender, TwitchClientOnJoinEventArgs args)
        {
            if (!Channels.ContainsKey(args.Channel))
                Channels.Add(args.Channel, new List<string>());
            if (!Channels[args.Channel].Contains(args.Name))
                Channels[args.Channel].Add(args.Name);
        }

        void Load()
        {
            m_commandManager = new CommandsManager();
            m_commandManager.AddPlugin(new CookieGiver());
            m_commandManager.AddPlugin(new GenericCommands());
            m_commandManager.LoadAllPlugins();
        }

        void client_OnPart(TwitchClient sender, Twitch.Models.TwitchClientOnPartEventArgs args)
        {
            if (Channels.ContainsKey(args.Channel))
                if (Channels[args.Channel].Contains(args.Name))
                    Channels[args.Channel].Remove(args.Name);
        }

        void client_OnPerform(TwitchClient sender)
        {
            sender.Join("#citillara");
            sender.Join("#elbodykso");
            //sender.Join("#infoutlaw");
            sender.Join("#nickynoel");
            //sender.Join("#crumps2");
            //sender.Join("#twitch");
            //sender.Join("#ea");

            //sender.Join("#mlg");
            //sender.Join("#dansgaming");
            /*
            sender.Join("#daopa");
            sender.Join("#zarvoxtoral");
            sender.Join("#themittanidotcom");
            sender.Join("#crasskitty");
            sender.Join("#angerbeard");*/
        }

        void client_OnMessage(TwitchClient sender, TwitchMessage message)
        {
            try
            {
                //Console.WriteLine(message);
                m_commandManager.OnMessage(sender, message);
                DoMisc(sender, message);
            }
            catch (Exception e)
            {
                File.AppendAllText("error.log", message.ToString());
                Console.WriteLine(message);
                File.AppendAllText("error.log", e.ToString());
                Console.WriteLine(e.ToString());
            }
        }
        int progress = 0;
        void DoMisc(TwitchClient sender, TwitchMessage message)
        {

            if (message.Message.Equals("Prepare for trouble!"))
            {
                sender.SendMessage(message.Channel, "Make it double!");
                progress = 1;
            }
            else if (progress == 1 && message.Message.Equals("To protect the world from devastation!"))
            {
                sender.SendMessage(message.Channel, "To unite all peoples within our nation!");
                progress = 2;
            }
            else if (progress == 2 && message.Message.Equals("To denounce the evils of truth and love!"))
            {
                sender.SendMessage(message.Channel, "To extend our reach to the stars above!");
                progress = 3;
            }
            else if (progress == 3 && message.Message.Equals("Jessie!"))
            {
                sender.SendMessage(message.Channel, "James!");
                progress = 4;
            }
            else if (progress == 4 && message.Message.Equals("Team Rocket, blast off at the speed of light!"))
            {
                sender.SendMessage(message.Channel, "Surrender now, or prepare to fight!");
                progress = 5;
            }
            else if (progress == 5 && message.Message.Equals("Meowth!"))
            {
                sender.SendMessage(message.Channel, "That's right!");
                progress = 0;
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
