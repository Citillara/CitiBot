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
                new PluginManager.OnMessageAction(this, SetDelays,
                    "!bribedelay",
                    "!cookiedelay"
                    ) { UserChannelCooldown = 5 });

            
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, ChangeStringSettings,
                    "!setsubgreetings"
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

        public override void OnNotice(TwitchClient sender, TwitchNotice notice)
        {
            if (notice.IsSub)
            {
                OnSub(sender, notice);
            }
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
            CookieUser user;
            if (message.Args.Count() > 1)
            {
                 user = CookieUser.GetUserByDisplayName(channel, message.Args[1]);
                 if (user != null)
                 {
                     if (user.CookieReceived == 0)
                     {
                         client.SendMessage(message.Channel, "{0} didn't received any cookies so far. Type !cookies {0} to give some", user.TwitchUser.BusinessDisplayName);
                         return;
                     }
                     else
                     {
                         var ranking = CookieUser.GetUserRankingInChannel(channel, user.TwitchUser.Name);
                         client.SendMessage(message.Channel, "{0} has {1} cookies and is ranked {2}",
                             user.TwitchUser.BusinessDisplayName,
                             user.CookieReceived,
                             Ranking(ranking));
                         return;
                     }
                 }
                 else
                 {
                     client.SendMessage(message.Channel, "{0} didn't received any cookies so far. Type !cookies {0} to give some", message.Args[1]);
                     return;
                 }
            }

            string username = message.SenderName;

            user = CookieUser.GetUser(channel, username);
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
            client.SendMessage(message.Channel, "https://www.citillara.fr/citibot/c/{0}/cookies", message.Channel.ToLowerInvariant().Replace("#", ""));
        }
        private void DisplayCookieFlavours(TwitchClient client, TwitchMessage message)
        {
            if (message.IsWhisper)
            {
                client.SendWhisper(message.Channel, "Sorry but that command is not supported over whisper");
                return;
            }
            client.SendMessage(message.Channel, "https://www.citillara.fr/citibot/c/{0}/cookies/flavours", message.Channel.ToLowerInvariant().Replace("#", ""));
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
        /*private void OnBitsSent(TwitchClient client, TwitchMessage message)
        {

            int cookieCheers = CookieChannel.GetChannel(message.Channel).CookieCheers;
            string senderDatabaseKey = message.SenderName;
            string channel = message.Channel;
            var sender_user_database = CookieUser.GetUser(channel, senderDatabaseKey, message.UserId, message.SenderDisplayName);
            if (cookieCheers > 0)
            {
                int amount = cookieCheers * (int)message.BitsSent;
                GiveCookies(client, channel, sender_user_database, sender_user_database, amount);
            }
        }*/
        private void OnSub(TwitchClient client, TwitchNotice notice)
        {
            var channel = CookieChannel.GetChannel(notice.Channel);
            if (!string.IsNullOrEmpty(channel.SubGreetings))
            {
                string greetings = channel.SubGreetings.Replace("$user", notice.DisplayName);
                client.SendMessage(notice.Channel, greetings);
            }
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

                var chan = CookieChannel.GetChannel(message.Channel);
                int delay = chan.CookieDelay;



                // Sender is in database
                if (sender_user_database != null)
                {
                    int delay_in_seconds = 60; // default;

                    // Channel may have custom delay
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


            var target_user_database = CookieUser.GetUser(channel, targetDatabaseKey);

            GiveCookies(client, channel, sender_user_database, target_user_database, forcedCookies, split[0]);
        }

        private void GiveCookies(TwitchClient client, string channel, CookieUser sender, CookieUser target, 
            int forcedCookies = -1, string forcedFlavour = "")
        {
            // Set the last sent
            if (sender.Id == target.Id)
            {
                target = sender;
            }


            // Select a cookie
            IEnumerable<Int32> m_list_of_cookies_ids;
            if (RNG.Next(0, 100) > 75)
            {
                m_list_of_cookies_ids = CookieFlavour.GetCommonCookies();
            }
            else
            {
                m_list_of_cookies_ids = CookieFlavour.GetChannelCookies(channel);
                if (m_list_of_cookies_ids.Count() == 0)
                    m_list_of_cookies_ids = CookieFlavour.GetCommonCookies();
            }


            int next = RNG.Next(0, m_list_of_cookies_ids.Count()-1);

            // Select a quantity
            int quantity = 0;

            int picker = RNG.Next(1, 1000);

            if (picker < 500)
                quantity = RNG.Next(1, 10);
            else if (picker < 700)
                quantity = RNG.Next(10, 20);
            else if (picker < 850)
                quantity = RNG.Next(20, 40);
            else if (picker < 975)
                quantity = RNG.Next(40, 90);
            else
                quantity = RNG.Next(90, 100);

            if (forcedCookies != -1)
                quantity = forcedCookies;


            // User is in the database
            target.LastReceived = DateTime.Now;
            target.CookieReceived += quantity;
            sender.LastSend = DateTime.Now;
            if (sender.Id != target.Id)
            {
                sender.CookiesSent += quantity;
                target.CookiesReceivedByOthers += quantity;
            }
            sender.CookiesGenerated += quantity;

            target.Save();
            sender.Save();

            string modifier = "";
            if (quantity > 95)
                modifier = ". WHAT THE PEMP !";
            else if (quantity > 90)
                modifier = ". ROOOOOOO !";
            else if (quantity > 40)
                modifier = ". Incredible !";
            else if (quantity > 20)
                modifier = ". Awesome !";
            else if (quantity > 10)
                modifier = ". Not bad !";

            int cookie_id_selected = m_list_of_cookies_ids.ToArray()[next];

            string flavor = CookieFlavour.GetCookie(cookie_id_selected).Text.ToLowerInvariant();
            if (forcedFlavour.Contains("welcomecookie"))
                flavor = "welcome cookie";
            else if (forcedFlavour.Contains("wrcookie"))
                flavor = "World Record cookie";
            else if (forcedFlavour.Contains("lovecookie"))
                flavor = "cookie of Love <3 ";
            else if (forcedFlavour.Contains("crashcookie"))
                flavor = "crashing cookie";
            else if (forcedFlavour.Contains("morningcookie"))
                flavor = "morning cookie";
            else if (forcedFlavour.Contains("eveningcookie"))
                flavor = "evening cookie";

            string msg;
            if (flavor.Contains("[number]") && flavor.Contains("[target]"))
            {
                msg = flavor.Replace("[number]", NumberToWords(quantity)).Replace("[target]", target.TwitchUser.BusinessDisplayName) + modifier;
            }
            else
            {
                msg = string.Format("gives {0} {1} to {2} NomNom {3}", NumberToWords(quantity), flavor, target.TwitchUser.BusinessDisplayName, modifier);
            }
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
            var target = CookieUser.GetUser(message.Channel, message.Args[1].Replace("@", ""));

            if (sender == null || sender.CookieReceived < amount)
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you don't have enough cookies", message.SenderDisplayName);
                return;
            }

            if (sender.Id == target.Id)
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you can't send cookies to yourself, use !cookie instead", message.SenderDisplayName);
                return;
            }

            target.CookieReceived += amount;
            target.CookiesReceivedByOthers += amount;
            target.Save();
            sender.CookieReceived -= amount;
            sender.CookiesSent += amount;
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

            var target = CookieUser.GetUser(message.Channel, message.Args[1].ToLowerInvariant().Replace("@", ""));

            if (target == null || target.CookieReceived <= 0)
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but {1} doesn't have any cookies", message.SenderDisplayName, message.Args[1]);
                return;
            }

            int quantity = 0;
            quantity = RNG.Next(1, 3 * bribe_amount);


            if (target.Id == briber.Id)
            {
                if (quantity > briber.CookieReceived)
                    quantity = briber.CookieReceived;

                briber.CookieReceived -= bribe_amount;
                briber.CookieReceived -= quantity;
                briber.CookiesGivenToYoshi += bribe_amount;
                briber.CookiesDestroyedByYoshi += quantity;
                briber.CookiesLostToYoshi += quantity;


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
                if (quantity > target.CookieReceived)
                    quantity = target.CookieReceived;

                target.CookieReceived -= quantity;
                target.CookiesLostToYoshi += quantity;
                briber.CookieReceived -= bribe_amount;
                briber.CookiesGivenToYoshi += bribe_amount;
                briber.CookiesDestroyedByYoshi += quantity;

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


        private void EnableCookies(TwitchClient client, TwitchMessage message)
        {
            var channel = message.Channel;
            if (message.UserType >= TwitchUserTypes.Mod && !message.IsWhisper)
            {
                var chan = CookieChannel.GetChannel(channel);
                chan.Status = CookieChannel.CookieChannelStates.Enabled;
                chan.Save();
                client.SendMessage(message.Channel, "Cookies have been enabled");
            }
            else
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you are not the broadcaster", message.SenderDisplayName);
            }
        }
        private void DisableCookies(TwitchClient client, TwitchMessage message)
        {
            var channel = message.Channel;
            if (message.UserType >= TwitchUserTypes.Mod && !message.IsWhisper)
            {
                var chan = CookieChannel.GetChannel(channel);
                chan.Status = CookieChannel.CookieChannelStates.Disabled;
                chan.Save();
                client.SendMessage(message.Channel, "Cookies have been disabled");
            }
            else
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you are not the broadcaster", message.SenderDisplayName);
            }
        }

        private void SetDelays(TwitchClient client, TwitchMessage message)
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
                            case "!bribedelay":
                                channelp.BribeDelay = time;
                                client.SendMessage(message.Channel, "Bribe delay has been set to {0} seconds", time);
                                break;
                            default: break;
                        }
                        channelp.Save();
                    }
                }
                else
                {
                    var channelp = CookieChannel.GetChannel(channel); switch (message.Command)
                    {
                        case "!cookiedelay":
                            time = channelp.CookieDelay;
                            client.SendMessage(message.Channel, "Cookie delay is currently set to {0} seconds", time);
                            break;
                        case "!bribedelay":
                            time = channelp.BribeDelay;
                            client.SendMessage(message.Channel, "Bribe delay is currently set to {0} seconds", time);
                            break;
                        default: break;
                    }
                }
            }
            else
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, but you are not the broadcaster", message.SenderDisplayName);
            }
        }
        private void ChangeStringSettings(TwitchClient client, TwitchMessage message)
        {
            if (message.IsWhisper)
            {
                client.SendWhisper(message.Channel, "Sorry by that command is not supported over whisper");
                return;
            }

            if (message.UserType < TwitchUserTypes.Broadcaster)
            {
                client.SendWhisper(message.SenderName, "Sorry {0}, this command is only for the broadcaster.", message.SenderDisplayName);
                return;
            }


            string msg = message.Message.Replace("\"", "").Trim();

            if (msg.IndexOf(' ') == -1)
            {
                client.SendMessage(message.Channel, "You must specify a non-empty value");
                return;
            }

            string sub = msg.Substring(msg.IndexOf(' '));

            var channelp = CookieChannel.GetChannel(message.Channel);
            switch (message.Command)
            {
                case "!setsubgreetings":
                    channelp.SubGreetings = sub;
                    client.SendMessage(message.Channel, "New subscribers message has been set to : " + sub);
                    break;
                default: break;
            }
            channelp.Save();

        }
        private void ChangeNumericSettings(TwitchClient client, TwitchMessage message)
        {
            if (message.IsWhisper)
            {
                client.SendWhisper(message.Channel, "Sorry by that command is not supported over whisper");
                return;
            }

            if (message.UserType >= TwitchUserTypes.Broadcaster)
            {
                string[] split = message.Message.Split(' ');
                int val = -1;
                if (split.Length > 1 && int.TryParse(split[1], out val))
                {
                    if (val > -1)
                    {
                        var channelp = CookieChannel.GetChannel(message.Channel);
                        switch (message.Command)
                        {
                            case "!cookiecheers":
                                channelp.CookieCheers = val;
                                if (val == 0)
                                {
                                    client.SendMessage(message.Channel, "Cookie cheering has been disabled");
                                }
                                else
                                {
                                    client.SendMessage(message.Channel, "Cookie cheering factor has been set to {0}", val);
                                }
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
            if (poll == null || poll.Status == CookiePoll.CookiePollState.Deleted || poll.Status == CookiePoll.CookiePollState.Finished)
            {
                client.SendMessage(message.Channel, "No poll is ready to start. To create a new poll, please use !newpoll");
                return;
            }
            if (poll.Status == CookiePoll.CookiePollState.Running)
            {
                client.SendMessage(message.Channel, "Poll '{0}' is alrady in progress", poll.Title);
                return;
            }
            poll.Start();
            client.SendMessage(message.Channel, "Poll '{0}' started ! use !cookievote <option> <number_of_cookies> to vote for an option !", poll.Title);

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
            int act = RNG.Next(0, m_activities.Count());

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

                    if (poll.Status != CookiePoll.CookiePollState.Running)
                        return;

                    var timeleft = new TimeSpan(poll.CreationTime.AddSeconds(poll.Duration).Ticks - DateTime.Now.Ticks);

                    if (timeleft.TotalSeconds < 0)
                    {
                        poll.Status = CookiePoll.CookiePollState.Finished;
                        Client.SendMessage(Channel, "Poll is now finished");
                        var options_ordered = poll.PollOptions.OrderByDescending(o => o.Votes).ToList();
                        Client.SendMessage(Channel, "1st : {0} with {1} cookies", options_ordered[0].Text, options_ordered[0].Votes);
                        if (options_ordered.Count > 1)
                            Client.SendMessage(Channel, "2nd : {0} with {1} cookies", options_ordered[1].Text, options_ordered[1].Votes);
                        if (options_ordered.Count > 2)
                            Client.SendMessage(Channel, "3rd : {0} with {1} cookies", options_ordered[2].Text, options_ordered[2].Votes);

                        return;
                    }
                    else
                    {
                        int mins = (int)timeleft.TotalMinutes;
                        if (mins == 0)
                        {
                            int seconds_left = (int)Math.Round(timeleft.TotalSeconds, MidpointRounding.AwayFromZero);
                            Client.SendMessage(Channel, "{0} seconds remaining", seconds_left);
                        }
                        else
                        {
                            int minutes_left = (int)timeleft.TotalMinutes;
                            Client.SendMessage(Channel, "{0} minutes remaining", minutes_left);
                        }

                        int waittime =(int) timeleft.TotalMilliseconds / 2;
                        wait.WaitOne(waittime);
                    }

                }

            }
        }
    }
}
