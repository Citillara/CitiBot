using CitiBot.Main.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Main
{
    class BotManager
    {
        static List<Bot> list;

        public static IEnumerable<Bot> StartAllBots()
        {
            var settings = BotSettings.GetAllBots();
            foreach (var bs in settings)
            {
                list = new List<Bot>();
                Console.WriteLine("[" + DateTime.Now.ToString() + "] + Preparing " + bs.Name);
                var bot = new Bot(bs);
                list.Add(bot);
                bot.Start();
            }
            return list;
        }
    }
}
