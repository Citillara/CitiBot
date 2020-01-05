using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CitiBot.Main
{
    public class Program
    {
        public static readonly string Version = "5";
        static void Main(string[] args)
        {
            var culture = new System.Globalization.CultureInfo("fr-FR");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            new Program().MainLoop();
        }

        void MainLoop()
        {
            BotManager.StartAllBots();
        }


    }

}
