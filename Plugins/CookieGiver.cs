using Irc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;
using Twitch.Tools;

namespace CitiBot.Plugins
{
    public class CookieGiver
    {
       
    //    private const int CAL_PER_COOKIE = 31;
    //    private Random m_random = new Random();
    //    private DateTime m_previousSend = DateTime.MinValue;

    //    private Dictionary<string, CookieUser> m_user_database = new Dictionary<string, CookieUser>();
    //    private Dictionary<string, int> m_delay_database = new Dictionary<string, int>();
    //    private List<string> m_list_of_cookies;
    //    private List<Activities> m_activities;
        

    //    public CookieGiver()
    //    {
    //        Load();
    //    }

    //    public void OnMessage(TwitchClient client, TwitchClientOnMessageEventArgs message)
    //    {
    //        // No more than delay (1s)
    //        if (DateTime.Now < m_previousSend)
    //            return;

    //        string msg = message.TwitchMessage.Split(' ')[0];

    //        switch (msg)
    //        {
    //            case "!cookie":
    //            case "!cookies":
    //            case "!welcomecookie":
    //            case "!lovecookie":
    //            case "!wrcookie":
    //                GiveCookie(client, message);
    //                break;
    //            case "!cookiecount":
    //                DisplayCookieCount(client, message);
    //                break;
    //            case "!cookiedelay":
    //                ChangeCookieDelay(client, message);
    //                break;
    //            case "!commands":
    //                DisplayCommands(client, message);
    //                break;
    //            case "!help":
    //                DisplayHelp(client, message);
    //                break;
    //            case "!addcookie":
    //            case "!newcookie":
    //                AddCokkie(client, message);
    //                break;
    //            case "!rehash":
    //                Rehash(client, message);
    //                break;
    //            case "!dbcookiecount":
    //                DisplayDatabaseCookieCount(client, message);
    //                break;
    //            default:
    //                break;

    //        }
    //        m_previousSend = DateTime.Now.AddSeconds(1); // Hardcoded limitation
    //    }
		
		
    //    private void DisplayDatabaseCookieCount(TwitchClient client, TwitchMessage message)
    //    {
    //        client.SendMessage(message.Channel, "Database contains {0} cookies", m_list_of_cookies.Count());
    //    }

    //    private void AddCokkie(TwitchClient client, TwitchMessage message)
    //    {
    //        if (message.UserType >= TwitchUserTypes.Mod)
    //        {
    //            string msg = message.Message.Replace("\"", "") ;
    //            if (msg.Length > "!newcookie ".Length)
    //            {
    //                string sub = msg.Substring("!newcookie ".Length);
    //                if (sub.ToLowerInvariant().Contains("cookie"))
    //                {
    //                    if(CheckAllowedCookie(sub))
    //                    {
    //                        m_list_of_cookies.Add(sub);
    //                        File.WriteAllLines("Plugins/Databases/Cookies.txt", m_list_of_cookies);
    //                        client.SendMessage(message.Channel, "\"{0}\" have been added to the cookie database", sub);
    //                    }
    //                    else 
    //                    {
    //                        client.SendMessage(message.Channel, "Sorry {0}, but you're not allowed to add this.", message.SenderDisplayName);
    //                    }
    //                }
    //                else
    //                {
    //                    client.SendMessage(message.Channel, "Sorry {0}, but the new flavor must contain the word \"cookie\"", message.SenderDisplayName);
    //                }
    //            }
    //            else
    //            {
    //                client.SendMessage(message.Channel, "Please specify what you want to add in the cookie database");
    //            }
    //        }
    //        else
    //        {
    //            client.SendMessage(message.Channel, "Sorry {0}, this command is for mods and above only.", message.SenderDisplayName);
    //        }
    //    }

    //    private void DisplayCommands(TwitchClient client, TwitchMessage message)
    //    {
    //        client.SendMessage(message.Channel, "Commands are : !cookie !cookiecount !cookiedelay !randomtopic. Type !help <command> for usage");
    //    }

    //    private void DisplayHelp(TwitchClient client, TwitchMessage message)
    //    {
    //        string[] split = message.Message.Split(' ');
    //        if (split.Length > 1)
    //        {
    //            string command = split[1];
    //            if(command.StartsWith("!"))
    //                command = command.Substring(1);
    //            switch (command)
    //            {
    //                case "cookie":
    //                case "cookies":
    //                    client.Say(message.Channel, "Gives cookies. Usages : !cookie or !cookie <target>");
    //                    break;
    //                case "cookierank":
    //                case "cookiecount":
    //                    client.Say(message.Channel, "Displays your current cookie count and ranking. Usage : !cookiecount");
    //                    break;
    //                case "cookiedelay":
    //                    client.Say(message.Channel, "Changes delay for sending cookies in seconds. Broadcaster only. Usage : !cookiedelay <time_in_seconds>");
    //                    break;
    //                case "randomtopic":
    //                    client.Say(message.Channel, "Gives a random topic. Usage : !randomtopic");
    //                    break;
    //                case "commands":
    //                    client.Say(message.Channel, "Displays the list of availible commands. Usage : !commands");
    //                    break;
    //                case "help":
    //                    client.Say(message.Channel, "Displays help on a command. Usage : !help <command>");
    //                    break;
    //                default: Console.WriteLine(command);
    //                    break;
    //            }
    //        }
    //    }
		
    //    private void Rehash(TwitchClient client, TwitchMessage message)
    //    {
    //        if (message.UserType >= TwitchUserTypes.Citillara)
    //        {
    //            m_list_of_cookies =  File.ReadAllLines("Plugins/Databases/Cookies.txt").ToList();
    //            client.PrivMsg(message.Channel, "Database reloaded with {0} cookies", m_list_of_cookies.Count());
    //        }
    //    }
    //    private void ChangeCookieDelay(TwitchClient client, TwitchMessage message)
    //    {
    //        if (message.UserType >= TwitchUserTypes.Broadcaster)
    //        {
    //            string[] split = message.Message.Split(' ');
    //            int time = 0;
    //            if (split.Length > 1 && int.TryParse(split[1], out time))
    //            {
    //                if (time > 0)
    //                {
    //                    if (m_delay_database.ContainsKey(message.Channel))
    //                        m_delay_database[message.Channel] = time;
    //                    else
    //                        m_delay_database.Add(message.Channel, time);

    //                    client.PrivMsg(message.Channel, "Cookie delay has been set to {0} seconds", time);
    //                }
    //            }
    //        }
    //        else
    //        {
    //            client.PrivMsg(message.Channel, "Sorry {0}, but you are not the broadcaster", message.SenderDisplayName);
    //        }
    //    }

    //    [DataContract]
    //    public struct CookieUser
    //    {
    //        [DataMember]
    //        public int CookieReceived;
    //        [DataMember]
    //        public DateTime LastReceived;
    //        [DataMember(IsRequired=false)]
    //        public DateTime LastSend;
    //        [DataMember]
    //        public string Username;
    //    }

    //    public struct Activities
    //    {
    //        public string Name;
    //        public int Amount;
    //    }

    //    void DisplayCookieCount(TwitchClient client, TwitchMessage message)
    //    {
    //        string key = message.SenderName;
    //        if (m_user_database.ContainsKey(key) && m_user_database[key].CookieReceived > 0)
    //        {
    //            var sorted = from db in m_user_database
    //                         orderby db.Value.CookieReceived descending
    //                         select new
    //                         {
    //                             Key = db.Key,
    //                             Rank = (from dbb in m_user_database
    //                                     where dbb.Value.CookieReceived > db.Value.CookieReceived
    //                                     select dbb).Count() + 1
    //                         };

    //            int cookies = m_user_database[key].CookieReceived;
    //            client.Say(message.Channel, "{0}, you received {1} cookies so far which places you {2} out of {3}. It represents {4} which you can burn by doing {5}", 
    //                message.SenderDisplayName, 
    //                cookies, 
    //                Ranking(sorted.Where(s => s.Key == key).First().Rank),
    //                sorted.Count(),
    //                Calories(cookies),
    //                GetActivity(cookies)
    //                );
    //        }
    //        else
    //        {
    //            client.Say(message.Channel, "Sorry {0}, but you havn't received any cookies so far. Type !cookies to get some", message.SenderDisplayName);
    //        }
    //    }

    //    void GiveCookie(TwitchClient client, TwitchMessage message)
    //    {
    //        int forcedCookies = -1;
    //        bool targetIsNotSender = false;
    //        string target = message.SenderDisplayName;
    //        string channel = message.Channel;
    //        string[] split = message.Message.Split(' ');
    //        if (split.Length > 1)
    //        {
    //            // Sends the cookie to someone else. Usage : !cookie <target>
    //            target = split[1].Trim();
    //            targetIsNotSender = true;
    //        }
    //        if (split.Length > 2)
    //        {
    //            // Sends the cookies on another channel. Usage : !cookie <target> <channel>
    //            if (message.UserType >= TwitchUserTypes.Citillara && split[2].StartsWith("#"))
    //                channel = split[2];
    //        }
    //        if (split.Length > 3)
    //        {
                
    //            // Sends the cookies on another channel. Usage : !cookie <target> <channel> <amount>>
    //            if (message.UserType >= TwitchUserTypes.Citillara)
    //                int.TryParse(split[3], out forcedCookies);
    //        }
    //        // Sets the target depending if it's the sender or not
    //        string targetDatabaseKey = targetIsNotSender ? target.ToLowerInvariant() : message.SenderName.ToLowerInvariant();
    //        string senderDatabaseKey = message.SenderName.ToLowerInvariant();

    //        // Ignores checks if sender is Broadcaster or above
    //        if (message.UserType < TwitchUserTypes.Broadcaster)
    //        {
    //            // Sender is in database
    //            if (m_user_database.ContainsKey(senderDatabaseKey))
    //            {
    //                int delay_in_seconds = 60; // default;

    //                // Channel may have custom delay
    //                if (m_delay_database.ContainsKey(message.Channel))
    //                    delay_in_seconds = m_delay_database[message.Channel];

    //                // Sender is sending before the delay
    //                if (m_user_database[senderDatabaseKey].LastSend.AddSeconds(delay_in_seconds) > DateTime.Now)
    //                {
    //                    client.PrivMsg(message.Channel, "Sorry {0} buy you can only send cookies every {1} seconds", message.SenderDisplayName, delay_in_seconds);
    //                    return; // Prevent him from sending
    //                }
    //            }
    //        }


    //        if (m_user_database.ContainsKey(senderDatabaseKey))
    //        {
    //            // Sender is in database
    //            var sender = m_user_database[senderDatabaseKey];
    //            sender.LastSend = DateTime.Now;
    //            m_user_database[senderDatabaseKey] = sender;
    //        }
    //        else
    //        {
    //            // Sender is not in the database
    //            m_user_database.Add(senderDatabaseKey, new CookieUser()
    //            {
    //                LastReceived = DateTime.MinValue,
    //                CookieReceived = 0,
    //                Username = senderDatabaseKey,
    //                LastSend = DateTime.Now
    //            });
    //        }

    //        // Select a cookie
    //        int next = m_random.Next(0, m_list_of_cookies.Count);

    //        // Select a quantity
    //        int quantity = 0;

    //        int picker = m_random.Next(1, 100);

    //        if (picker < 50)
    //            quantity = m_random.Next(1, 10);
    //        else if (picker < 70)
    //            quantity = m_random.Next(10, 20);
    //        else if (picker < 90)
    //            quantity = m_random.Next(20, 40);
    //        else
    //            quantity = m_random.Next(40, 80);

    //        if (forcedCookies != -1)
    //            quantity = forcedCookies;


    //        if (m_user_database.ContainsKey(targetDatabaseKey))
    //        {
    //            // User is in the database
    //            var val = m_user_database[targetDatabaseKey];
    //            val.LastReceived = DateTime.Now;
    //            val.CookieReceived += quantity;
    //            m_user_database[targetDatabaseKey] = val;
    //        }
    //        else 
    //        {
    //            // User is not in the database
    //            m_user_database.Add(targetDatabaseKey, new CookieUser() { 
    //                LastReceived = DateTime.Now, 
    //                CookieReceived = quantity,
    //                Username = targetDatabaseKey, 
    //                LastSend = DateTime.MinValue 
    //            });
    //        }

           
    //        string modifier = "";
    //        if (quantity > 40)
    //            modifier = ". Incredible !";
    //        else if (quantity > 20)
    //            modifier = ". Awsome !";
    //        else if (quantity > 10)
    //            modifier = ". Not bad !";

    //        string flavor = m_list_of_cookies[next].ToLowerInvariant();
    //        if(split[0].Contains("welcomecookie"))
    //            flavor = "welcome cookie";
    //        else if (split[0].Contains("wrcookie"))
    //            flavor = "World Record cookie";
    //        else if (split[0].Contains("lovecookie"))
    //            flavor = "cookie of Love";

    //        string msg = string.Format("gives {0} {1} to {2} NomNom {3}", NumberToWords(quantity), flavor, target, modifier);
    //        if (quantity > 1)
    //            msg = msg.Replace("cookie", "cookies");
    //        client.SendAction(channel, msg);

    //        // Save data
    //        Save();
    //    }



    //    public static string Ranking(int number)
    //    {
    //        string nb = number.ToString();
    //        if (nb.EndsWith("1"))
    //            nb = nb + "st";
    //        else if (nb.EndsWith("2"))
    //            nb = nb + "nd";
    //        else if (nb.EndsWith("3"))
    //            nb = nb + "rd";
    //        else
    //            nb = nb + "th";

    //        return nb;
    //    }

    //    public static string NumberToWords(int number)
    //    {
    //        if (number == 0)
    //            return "zero";

    //        if (number < 0)
    //            return "minus " + NumberToWords(Math.Abs(number));

    //        string words = "";

    //        if ((number / 1000000) > 0)
    //        {
    //            words += NumberToWords(number / 1000000) + " million ";
    //            number %= 1000000;
    //        }

    //        if ((number / 1000) > 0)
    //        {
    //            words += NumberToWords(number / 1000) + " thousand ";
    //            number %= 1000;
    //        }

    //        if ((number / 100) > 0)
    //        {
    //            words += NumberToWords(number / 100) + " hundred ";
    //            number %= 100;
    //        }

    //        if (number > 0)
    //        {
    //            if (words != "")
    //                words += "and ";

    //            var unitsMap = new[] { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen" };
    //            var tensMap = new[] { "zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety" };

    //            if (number < 20)
    //                words += unitsMap[number];
    //            else
    //            {
    //                words += tensMap[number / 10];
    //                if ((number % 10) > 0)
    //                    words += "-" + unitsMap[number % 10];
    //            }
    //        }

    //        return words;
    //    }

    //    public string Calories(int numberOfCookies)
    //    {
    //        decimal d = numberOfCookies * CAL_PER_COOKIE;
    //        if(d > 1000000)
    //        {
    //            return string.Format("{0:0.00} Mcal", d / 1000000m);
    //        }
    //        else if (d > 1000)
    //        {
    //            return string.Format("{0:0.00} kcal", d / 1000m);
    //        }
    //        else
    //        {
    //            return string.Format("{0} cal", d);
    //        }
    //    }

    //    public string GetActivity(int numberOfCookies)
    //    {
    //        int act = m_random.Next(0, m_activities.Count());
    //        long duration = (long)(((decimal)numberOfCookies * (decimal)CAL_PER_COOKIE / (decimal)m_activities[0].Amount) * (decimal)TimeSpan.TicksPerHour);
    //        TimeSpan ts = new TimeSpan(duration);
    //        if(ts.TotalDays > 1)
    //            return string.Format("{0}d {1}h {2}min of {3}", ts.Days, ts.Hours, ts.Minutes, m_activities[act].Name.ToLowerInvariant());
    //        else if(ts.TotalHours > 1)
    //            return string.Format("{1}h {2}min of {3}", ts.Days, ts.Hours, ts.Minutes, m_activities[act].Name.ToLowerInvariant());
    //        else
    //            return string.Format("{2}min of {3}", ts.Days, ts.Hours, ts.Minutes, m_activities[act].Name.ToLowerInvariant());
            
    //    }

    //    public void Save()
    //    {
    //        CookieUser[] users = m_user_database.Values.ToArray();
    //        ToolBox.Serialize<CookieUser[]>(base.ToString(), users);

    //    }

    //    public void Load()
    //    {
    //        var t = ToolBox.Deserialize<CookieUser[]>(base.ToString());
    //        if(t != null)
    //            foreach (var tt in t)
    //                m_user_database.Add(tt.Username, tt);

    //        m_list_of_cookies =  File.ReadAllLines("Plugins/Databases/Cookies.txt").ToList();

    //        string[] factivities = File.ReadAllLines("Plugins/Databases/CaloriesPerActivity.txt");
    //        m_activities = new List<Activities>(factivities.Length);
    //        foreach (string s in factivities)
    //        {
    //            string[] split = s.Split('\t');
    //            if (split.Length == 5)
    //                m_activities.Add(new Activities()
    //                {
    //                    Name = split[0],
    //                    Amount = int.Parse(split[2])
    //                });
    //        }


    //    }
		
    //    private bool CheckAllowedCookie(string cookie)
    //    {
    //        if(cookie.Contains("http"))
    //            return false;
    //        if(cookie.Contains("www"))
    //            return false;
    //        if(cookie.Contains(".co"))
    //            return false;
    //        if(cookie.Contains("\r"))
    //            return false;
    //        if(cookie.Contains("\n"))
    //            return false;
    //        if(cookie.Contains("â"))
    //            return false;
    //        return true;
    //    }
    }
}
