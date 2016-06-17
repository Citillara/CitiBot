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

        CommandsManager m_commandManager;


        void MainLoop()
        {
            Load();
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

        void Load()
        {
            m_commandManager = new CommandsManager();
            m_commandManager.AddPlugin(new CookieGiver());
            m_commandManager.AddPlugin(new GenericCommands());
            m_commandManager.LoadAllPlugins();
        }

        void client_OnPart(TwitchClient sender, Twitch.Models.TwitchClientOnPartEventArgs args)
        {

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
                m_commandManager.OnMessage(sender, message);
            }
            catch (Exception e)
            {
                File.AppendAllText("error.log", e.ToString());
                Console.WriteLine(e.ToString());
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
