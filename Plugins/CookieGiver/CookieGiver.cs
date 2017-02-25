using CitiBot.Main;
using CitiBot.Plugins.CookieGiver.Models;
using CitiBot.Plugins.Twitch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Plugins.CookieGiver
{
    public class CookieGiver : Plugin
    {

        private const int CAL_PER_COOKIE = 31;
        private Random m_random = new Random();
        private DateTime m_previousSend = DateTime.MinValue;

        private List<Thread> m_threads = new List<Thread>();

        public CookieGiver()
        {
            Load();
        }

        public void Load()
        {
            Console.WriteLine("Database loaded with " + CookieFlavour.GetCookieCount());
        }

        public override void OnLoad(PluginManager pluginManager)
        {
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, GiveCookie,
                    "!cookie",
                    "!cookies",
                    "!welcomecookie",
                    "!lovecookie",
                    "!wrcookie",
                    "!crashcookie",
                    "!morningcookie",
                    "!eveningcookie"
                    ));

            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DisplayCookieCount,
                    "!rank",
                    "!cookierank",
                    "!cookiecount"
                    ) { UserCooldown = 30 });

            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DisplayCookieCountLink,
                    "!ranks",
                    "!globalrank"
                    ) { ChannelCooldown = 30 });

            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DisplayCookieFlavours,
                    "!flavours",
                    "!flavors"
                    ) { ChannelCooldown = 30 });

            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, SendCookies,
                    "!sendcookie",
                    "!givecookie",
                    "!cookiesend",
                    "!cookiegive"
                    ));


            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, SendYoshi,
                    "!bribe",
                    "!yoshi",
                    "!bribeyoshi",
                    "!yoshibribe"
                    ));

            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, ChangeDelays,
                    "!bribedelay",
                    "!cookiedelay"
                    ) { UserChannelCooldown = 5 });

            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, AddCookieFlavor,
                    "!addcookie",
                    "!newcookie"
                    ) { UserChannelCooldown = 5 });

            pluginManager.RegisterCommand(
                 new PluginManager.OnMessageAction(this, DisplayDatabaseCookieCount,
                     "!dbcookiecount"
                     ));

            pluginManager.RegisterCommand(
             new PluginManager.OnMessageAction(this, DisplayTop10,
                 "!top10"
                 ) { ChannelCooldown = 30 });


        }

        private void AddCookieFlavor(TwitchClient client, TwitchMessage message)
        {
            if (message.IsWhisper)
            {
                client.SendWhisper(message.Channel, "Sorry by that command is not supported over whisper");
                return;
            }
            if (message.UserType < TwitchUserTypes.Mod)
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, this command is only for mods and above.", message.SenderDisplayName);
                return;
            }

            string msg = message.Message.Replace("\"", "").Trim();

            if (msg.IndexOf(' ') == -1)
            {
                client.SendWhisper(message.SenderName, "Please specify what you want to add to the cookie database");
                return;
            }

            string sub = msg.Substring(msg.IndexOf(' '));
            if (!sub.ToLowerInvariant().Contains("cookie"))
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but the new flavor must contain the word \"cookie\" (singular)", message.SenderDisplayName);
                return;
            }
            if (!CheckAllowedCookie(sub))
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you're not allowed to add this.", message.SenderDisplayName);
                return;
            }

            CookieFlavour.AddNewCookieFlavor(message.Channel, sub, message.SenderName);


            client.SendMessage(message.Channel, "\"{0}\" have been added to the cookie database", sub);


        }
        private void DisplayCookieCount(TwitchClient client, TwitchMessage message)
        {
            string channel = message.Channel;
            string username = message.SenderName;

            var user = CookieUser.GetUser(channel, username);
            if (user != null && user.CookieReceived > 0)
            {
                var ranking = CookieUser.GetUserRankingInChannel(channel, username);
                var number_of_users = CookieUser.GetChannelUserCount(channel);

                int cookies = user.CookieReceived;
                client.SendWhisper(message.SenderName, "{0}, you received {1} cookies so far which places you {2} out of {3}. It represents {4} which you can burn by doing {5}",
                    message.SenderDisplayName,
                    user.CookieReceived,
                    Ranking(ranking),
                    number_of_users,
                    Calories(cookies),
                    GetActivity(cookies)
                    );
            }
            else
            {
                client.SendMessage(message.Channel, "Sorry {0}, but you havn't received any cookies so far. Type !cookies to get some", message.SenderDisplayName);
            }
        }
        private void DisplayDatabaseCookieCount(TwitchClient client, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Citillara)
                return;
            client.SendMessage(message.Channel, "Database contains {0} cookies", CookieFlavour.GetCookieCount());
        }
        private void DisplayTop10(TwitchClient client, TwitchMessage message)
        {
            DisplayTop(client, message, 10);
        }
        private void DisplayCookieCountLink(TwitchClient client, TwitchMessage message)
        {
            if (message.IsWhisper)
            {
                client.SendWhisper(message.Channel, "Sorry but that command is not supported over whisper");
                return;
            }
            client.SendMessage(message.Channel, "https://www.citillara.fr/citibot/{0}/cookies", message.Channel.ToLowerInvariant().Replace("#", ""));
        }
        private void DisplayCookieFlavours(TwitchClient client, TwitchMessage message)
        {
            if (message.IsWhisper)
            {
                client.SendWhisper(message.Channel, "Sorry but that command is not supported over whisper");
                return;
            }
            client.SendMessage(message.Channel, "https://www.citillara.fr/citibot/{0}/cookies/flavours", message.Channel.ToLowerInvariant().Replace("#", ""));
        }
        private void DisplayTop(TwitchClient client, TwitchMessage message, int count)
        {

            var top_10 = CookieUser.GetChannelTopUsers(message.Channel, count);

            var sorted = from db in top_10
                         orderby db.CookieReceived descending
                         select new
                         {
                             Key = db.DisplayName,
                             Rank = (from dbb in top_10
                                     where dbb.CookieReceived > db.CookieReceived
                                     select dbb).Count() + 1,
                             CookieReceived = db.CookieReceived

                         };
            var sorted_ordered = from sr in sorted
                                 orderby sr.Rank ascending
                                 select sr;
            StringBuilder sb = new StringBuilder();
            foreach (var el in sorted_ordered)
                sb.AppendFormat("{0}. {1} ({2}) - ", el.Rank, el.Key, el.CookieReceived);
            client.SendMessage(message.Channel, sb.ToString());

        }
        private void GiveCookie(TwitchClient client, TwitchMessage message)
        {
            int forcedCookies = -1;
            bool targetIsNotSender = false;
            bool allowedThroughWhisper = false;
            string target = message.SenderDisplayName;
            string channel = message.Channel;
            string[] split = message.Message.Split(' ');            
            if (split.Length > 1)
            {
                // Sends the cookie to someone else. Usage : !cookie <target>
                target = split[1].Trim().Replace("@", "");
                targetIsNotSender = true;
            }
            if (split.Length > 2)
            {
                // Sends the cookies on another channel. Usage : !cookie <target> <channel>
                if (message.UserType >= TwitchUserTypes.Developper && split[2].StartsWith("#"))
                {
                    channel = split[2];
                    allowedThroughWhisper = true;
                }
                // Sends defined amount of cookies to a target in the channel. Usage !cookie <target> <amount>
                else if (message.UserType >= TwitchUserTypes.Citillara && int.TryParse(split[2], out forcedCookies))
                {
                    allowedThroughWhisper = false;
                }
            }
            if (split.Length == 4)
            {
                // Sends the cookies on another channel. Usage : !cookie <target> <channel> <amount>
                if (message.UserType >= TwitchUserTypes.Citillara)
                {
                    int.TryParse(split[3], out forcedCookies);
                    allowedThroughWhisper = true;
                }
            }
            if (message.IsWhisper && !allowedThroughWhisper)
            {
                client.SendWhisper(message.Channel, "Sorry but that command is not supported over whisper");
                return;
            }
            // Sets the target depending if it's the sender or not
            string targetDatabaseKey = targetIsNotSender ? target.ToLowerInvariant() : message.SenderName;
            string senderDatabaseKey = message.SenderName;

            var sender_user_database = CookieUser.GetUser(channel, senderDatabaseKey, message.UserId, message.SenderDisplayName);

            // Ignores checks if sender is Developper or above
            if (message.UserType < TwitchUserTypes.Developper)
            {

                // Sender is in database
                if (sender_user_database != null)
                {
                    int delay_in_seconds = 60; // default;

                    // Channel may have custom delay
                    int delay = CookieChannel.GetChannel(message.Channel).CookieDelay;
                    if (delay > 0)
                        delay_in_seconds = delay;

                    // Sender is sending before the delay
                    var next_allowance_date = sender_user_database.LastSend.AddSeconds(delay_in_seconds);
                    if (next_allowance_date > DateTime.Now)
                    {
                        var next_allowance_seconds = (int)(next_allowance_date - DateTime.Now).TotalSeconds;
                        client.SendWhisper(message.SenderName, "Sorry {0} but you can get/send cookies only every {1} seconds (next in {2} seconds)", message.SenderDisplayName, delay_in_seconds, next_allowance_seconds);
                        return; // Prevent him from sending
                    }
                }
            }

            // Sender is in database
            sender_user_database.LastSend = DateTime.Now;
            sender_user_database.Save();

            // Select a cookie
            IEnumerable<Int32> m_list_of_cookies_ids;
            if (m_random.Next(0, 100) > 75)
            {
                m_list_of_cookies_ids = CookieFlavour.GetCommonCookies();
            }
            else
            {
                m_list_of_cookies_ids = CookieFlavour.GetChannelCookies(message.Channel);
                if (m_list_of_cookies_ids.Count() == 0)
                    m_list_of_cookies_ids = CookieFlavour.GetCommonCookies();
            }
            

            int next = m_random.Next(0, m_list_of_cookies_ids.Count());

            // Select a quantity
            int quantity = 0;

            int picker = m_random.Next(1, 100);

            if (picker < 50)
                quantity = m_random.Next(1, 10);
            else if (picker < 70)
                quantity = m_random.Next(10, 20);
            else if (picker < 90)
                quantity = m_random.Next(20, 40);
            else if( picker < 95)
                quantity = m_random.Next(40, 80);
            else
                quantity = m_random.Next(80, 101);

            if (forcedCookies != -1)
                quantity = forcedCookies;


            var target_user_database = CookieUser.GetUser(channel, targetDatabaseKey);
            // User is in the database
            target_user_database.LastReceived = DateTime.Now;
            target_user_database.CookieReceived += quantity;
            target_user_database.Save();


            string modifier = "";
            if (quantity > 40)
                modifier = ". Incredible !";
            else if (quantity > 20)
                modifier = ". Awesome !";
            else if (quantity > 10)
                modifier = ". Not bad !";

            int cookie_id_selected = m_list_of_cookies_ids.ToArray()[next];

            string flavor = CookieFlavour.GetCookie(cookie_id_selected).Text.ToLowerInvariant();
            if (split[0].Contains("welcomecookie"))
                flavor = "welcome cookie";
            else if (split[0].Contains("wrcookie"))
                flavor = "World Record cookie";
            else if (split[0].Contains("lovecookie"))
                flavor = "cookie of Love <3 ";
            else if (split[0].Contains("crashcookie"))
                flavor = "crashing cookie";
            else if (split[0].Contains("morningcookie"))
                flavor = "morning cookie";
            else if (split[0].Contains("eveningcookie"))
                flavor = "evening cookie";


            string msg = string.Format("gives {0} {1} to {2} NomNom {3}", NumberToWords(quantity), flavor, target, modifier);
            if (quantity > 1)
                msg = msg.Replace("cookie", "cookies");
            client.SendAction(channel, msg);

        }
        private void SendCookies(TwitchClient client, TwitchMessage message)
        {
            int amount = 0;

            if (message.Args.Length < 3 || !int.TryParse(message.Args[2], out amount) || amount <= 0)
            {
                client.SendMessage(message.Channel, "Usage : !sendcookie <target> <amount>");
                return;
            }
            var sender = CookieUser.GetUser(message.Channel, message.SenderName, message.UserId, message.SenderDisplayName);
            var target = CookieUser.GetUser(message.Channel, message.Args[1]);

            if (sender == null || sender.CookieReceived < amount)
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you don't have enough cookies", message.SenderDisplayName);
                return;
            }

            target.CookieReceived += amount;
            target.Save();
            sender.CookieReceived -= amount;
            sender.Save();

            client.SendMessage(message.Channel, "{0} gave {1} cookies to {2}", message.SenderDisplayName, amount, message.Args[1]);
        }
        private void SendYoshi(TwitchClient client, TwitchMessage message)
        {
            int bribe_amount = -1;
            var briber = CookieUser.GetUser(message.Channel, message.SenderName, message.UserId, message.SenderDisplayName);


            if (message.IsWhisper)
            {
                client.SendWhisper(message.Channel, "Sorry but that command is not supported over whisper");
            }
            if (message.Args.Length < 3)
            {
                client.SendMessage(message.Channel, "Usage : !yoshi <target> <bribe_amount>");
                return;
            }
            if (!int.TryParse(message.Args[2], out bribe_amount) || bribe_amount <= 0)
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you must specify a non negative number of cookies", message.SenderDisplayName);
                return;
            }
            if (briber == null)
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you don't have any cookies to bribe Yoshi with", message.SenderDisplayName);
                return;
            }
            if (bribe_amount > briber.CookieReceived)
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you don't have enough cookies to bribe Yoshi with", message.SenderDisplayName);
                return;
            }
            var channel = CookieChannel.GetChannel(message.Channel);
            var nextbribe = briber.LastYoshiBribe.AddSeconds(channel.BribeDelay);
            if (nextbribe > DateTime.Now)
            {
                var sec = (int)(nextbribe - DateTime.Now).TotalSeconds;
                client.SendWhisper(message.SenderName, "Sorry {0}, but you can only bribe Yoshi every {1} seconds (next attempt in {2} seconds)", message.SenderDisplayName, channel.BribeDelay, sec);
                return;
            }

            var target = CookieUser.GetUser(message.Channel, message.Args[1].ToLowerInvariant());

            if (target == null || target.CookieReceived <= 0)
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but {1} doesn't have any cookies", message.SenderDisplayName, message.Args[1]);
                return;
            }

            int quantity = 0;
            quantity = m_random.Next(1, 3 * bribe_amount);


            if (target.Id == briber.Id)
            {
                briber.CookieReceived -= bribe_amount;
                briber.CookieReceived -= quantity;

                if (briber.CookieReceived < 0)
                    briber.CookieReceived = 0;


                client.SendMessage(message.Channel, "{0} bribed Yoshi, who devoured {1} cookies of {2} ! ({3} cookies left)",
                    message.SenderDisplayName,
                    quantity,
                    briber.TwitchUser.BusinessDisplayName,
                    briber.CookieReceived);

                briber.LastYoshiBribe = DateTime.Now;

                briber.Save();
            }
            else
            {
                target.CookieReceived -= quantity;
                briber.CookieReceived -= bribe_amount;

                if (target.CookieReceived < 0)
                    target.CookieReceived = 0;

                client.SendMessage(message.Channel, "{0} bribed Yoshi, who devoured {1} cookies of {2} ! ({3} cookies left)",
                    message.SenderDisplayName,
                    quantity,
                    target.TwitchUser.BusinessDisplayName,
                    target.CookieReceived);

                briber.LastYoshiBribe = DateTime.Now;

                briber.Save();
                target.Save();
            }
        }

        private void ChangeDelays(TwitchClient client, TwitchMessage message)
        {
            var channel = message.Channel;
            if (message.UserType >= TwitchUserTypes.Broadcaster)
            {
                string[] split = message.Message.Split(' ');
                int time = 0;
                if (split.Length > 1 && int.TryParse(split[1], out time))
                {
                    if (time > 0)
                    {
                        var channelp = CookieChannel.GetChannel(channel);
                        switch (message.Command)
                        {
                            case "!cookiedelay":
                                channelp.CookieDelay = time;
                                client.SendMessage(message.Channel, "Cookie delay has been set to {0} seconds", time);
                                break;
                            case "!stealdelay":
                                channelp.StealDelay = time;
                                client.SendMessage(message.Channel, "Steal delay has been set to {0} seconds", time);
                                break;
                            case "!bribedelay":
                                channelp.BribeDelay = time;
                                client.SendMessage(message.Channel, "Bribe delay has been set to {0} seconds", time);
                                break;
                            default: break;
                        }
                        channelp.Save();
                    }
                }
            }
            else
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you are not the broadcaster", message.SenderDisplayName);
            }
        }

        private void StartPoll(TwitchClient client, TwitchMessage message)
        {
            var poll = CookiePoll.GetLastestPoll(message.Channel);
            if (message.UserType < TwitchUserTypes.Broadcaster)
            {
                return;
            }
            if (poll == null || poll.Status == CookiePoll.State.Deleted || poll.Status == CookiePoll.State.Finished)
            {
                client.SendMessage(message.Channel, "No poll is ready to start. To create a new poll, please use !newpoll");
                return;
            }
            if (poll.Status == CookiePoll.State.Running)
            {
                client.SendMessage(message.Channel, "Poll '{0}' is alrady in progress", poll.Title);
                return;
            }
            poll.Start();
            client.SendMessage(message.Channel, "Poll '{0}' started ! use !cookievote <option> <number_of_cookies> to vote for an option !", poll.Title);

        }
        private void EndPoll(TwitchClient client)
        {

        }


        public static int GetPercentageOfCookies(int cookies, int percentage)
        {
            decimal c = (decimal)cookies;
            decimal p = (decimal)percentage;
            decimal t = cookies * (percentage / 100m);
            return (int)Math.Ceiling(t);
        }
        public static string Ranking(int number)
        {
            string nb = number.ToString();
            if (nb.EndsWith("1"))
                nb = nb + "st";
            else if (nb.EndsWith("2"))
                nb = nb + "nd";
            else if (nb.EndsWith("3"))
                nb = nb + "rd";
            else
                nb = nb + "th";

            return nb;
        }
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number / 1000000) > 0)
            {
                words += NumberToWords(number / 1000000) + " million ";
                number %= 1000000;
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords(number / 1000) + " thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords(number / 100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
                var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }
        public string Calories(int numberOfCookies)
        {
            decimal d = numberOfCookies * CAL_PER_COOKIE;
            if (d > 1000000)
            {
                return string.Format("{0:0.00} Mcal", d / 1000000m);
            }
            else if (d > 1000)
            {
                return string.Format("{0:0.00} kcal", d / 1000m);
            }
            else
            {
                return string.Format("{0} cal", d);
            }
        }
        public string GetActivity(int numberOfCookies)
        {
            var m_activities = CaloriesPerActivity.GetListOfActivities();
            int act = m_random.Next(0, m_activities.Count());

            var activity = CaloriesPerActivity.GetById(act);

            long duration = (long)(((decimal)numberOfCookies * (decimal)CAL_PER_COOKIE / (decimal)activity.Calories) * (decimal)TimeSpan.TicksPerHour);
            TimeSpan ts = new TimeSpan(duration);
            if (ts.TotalDays > 1)
                return string.Format("{0}d {1}h {2}min of {3}", ts.Days, ts.Hours, ts.Minutes, activity.Text.ToLowerInvariant());
            else if (ts.TotalHours > 1)
                return string.Format("{1}h {2}min of {3}", ts.Days, ts.Hours, ts.Minutes, activity.Text.ToLowerInvariant());
            else
                return string.Format("{2}min of {3}", ts.Days, ts.Hours, ts.Minutes, activity.Text.ToLowerInvariant());

        }
        private static bool CheckAllowedCookie(string cookie)
        {
            if (cookie.Contains("http"))
                return false;
            if (cookie.Contains("www"))
                return false;
            if (cookie.Contains(".co"))
                return false;
            if (cookie.Contains("\r"))
                return false;
            if (cookie.Contains("\n"))
                return false;
            if (cookie.Contains("â"))
                return false;
            return true;
        }

        class PollThread
        {
            private ManualResetEvent wait = new ManualResetEvent(false);
            private bool run = true;
            private TwitchClient Client;
            private string Channel;

            public PollThread(TwitchClient client, string channel)
            {
                this.Client = client;
                this.Channel = channel;
                new Thread(new ThreadStart(Tick)).Start();
            }

            public void Close()
            {
                run = false;
                wait.Set();
            }

            public void Tick()
            {
                while (true)
                {
                    if (!run)
                        return;

                    var poll = CookiePoll.GetLastestPoll(Channel);

                    if (poll.Status != CookiePoll.State.Running)
                        return;

                    


                }

            }
        }
    }
}
