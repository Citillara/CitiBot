
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

namespace CitiBot.Main
{
    public class Program
    {
        public static readonly string Version = "2";
        static void Main(string[] args)
        {
            new Program().MainLoop();
        }

        void MainLoop()
        {
            Bot.StartAllBots();
        }


    }

}
