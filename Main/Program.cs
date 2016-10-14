using CitiBot.Plugins;
using CitiBot.Plugins.CookieGiver;
using CitiBot.Plugins.CookieGiver.Models;
using CitiBot.Plugins.Dog;
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

namespace CitiBot.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program().MainLoop();
        }

        Regex r = new Regex(@"[^\u0000-\u007F]", RegexOptions.Compiled);


        void MainLoop()
        {
            Bot.StartAllBots();

            while (true)
                Console.ReadKey();

        }


    }

}
