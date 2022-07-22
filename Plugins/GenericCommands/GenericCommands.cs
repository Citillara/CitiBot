using CitiBot.Main;
using CitiBot.Main.Models;
using CitiBot.Plugins.Twitch.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Plugins.GenericCommands
{
    public class GenericCommands : Plugin
    {
        private readonly string TWITCH_URL = "https://twitch.tv/";
        private readonly int RAID_REPEAT = 4;
        private Dictionary<string, string> m_raidMessages = new Dictionary<string, string>();
        private readonly DateTime m_startTime = DateTime.Now;
        private int m_botId;

        public override void OnLoad(PluginManager pluginManager)
        {
            m_botId = pluginManager.BotId;
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoBug, "!bug") { ChannelCooldown = 30 });
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoJoin, "!join"));
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoPart, "!part"));
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoPyramid, "!pyramid"));
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoRaid, "!raid") { ChannelCooldown = 5 });
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoRaidMessage, "!raidmessage") { ChannelCooldown = 5 });


            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoRoll, "!roll") { UserChannelCooldown = 30 });
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoTimeDiff, "!timediff", "!timedif") { ChannelCooldown = 5 });
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoUptime, "!uptime") { ChannelCooldown = 5 });

            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoToAlBhed, "!toal", "!toalbhed", "!toalbed", "!toalbehd") { UserCooldown = 30 });
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoToFromBhed, "!fromal", "!fromalbhed", "!fromalbed", "!fromalbehd") { UserCooldown = 30 });


            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoLearnUser, "!learn"));

            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoBonk, "!bonk", "!boink") { UserChannelCooldown = 5, Bypass = TwitchUserTypes.Founder });

            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoSetAutoJoin, "!setautojoin"));

            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoCelciusToFahrenheit, "!ctof", "!getf") { UserCooldown = 3 });
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoFahrenheitToCelcius, "!ftoc", "!getc") { UserCooldown = 3 });

        }

        public void DoBug(TwitchClient sender, TwitchMessage message)
        {
            sender.SendMessage(message.Channel, "Please report any bugs/suggestion at https://github.com/Citillara/CitiBot/issues");
        }
        public void DoBonk(TwitchClient sender, TwitchMessage message)
        {
            if (message.Args.Count() > 1)
            {
                string bonkType = "bonks";
                if (message.Args[0] == "!boink")
                    bonkType = "boinks";
                if (message.Args[1].ToLowerInvariant() == "citibot")
                {
                    sender.SendAction(message.Channel, $"self{bonkType} citiBoink2");
                }
                else
                {
                    sender.SendAction(message.Channel, bonkType + " " + message.Args[1] + " citiBoink2");
                }
            }
            else
            {
                sender.SendAction(message.Channel, "selfbonks citiBoink2");
            }
        }
        public void DoJoin(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Developer)
            {
                //sender.SendWhisper(message.SenderName, "Sorry {0}, but that command is currently restricted to Bot Admins", message.SenderDisplayName);
            }
            else
            {
                if (message.Args.Length > 1)
                {
                    if (message.Args[1].StartsWith("#"))
                    {

                        sender.Join(message.Args[1]);
                        sender.SendMessage("#citillara", "Joining {0} on behalf of {1}", message.Args[1], message.SenderDisplayName);
                    }
                    else
                    {
                        sender.SendMessage(message.Channel, "Please specify a correct channel");
                    }
                }
                else
                {
                    sender.Join("#" + message.SenderName);

                    Log.AddBusinessLog(DateTime.Now, Log.LogLevel.Info,
                        message.Channel, "Join", "Joining {0} on behalf of {1}", message.Channel, message.SenderDisplayName);
                }
            }
        }
        public void DoPart(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Broadcaster)
            {
                sender.SendWhisper(message.SenderName, "Sorry {0}, but this command is rectricted to Broadcaster and above", message.SenderDisplayName);
            }
            else
            {
                sender.Part(message.Channel);
                var channel = BotSettings.GetById(m_botId).GetChannel(message.Channel);
                channel.AutoJoin = BotChannel.AutoJoinSettings.No;
                channel.Save();
                Log.AddBusinessLog(DateTime.Now, Log.LogLevel.Info,
                    message.Channel, "Part", "Parting {0} on behalf of {1}", message.Channel, message.SenderDisplayName);
            }
        }
        public void DoPyramid(TwitchClient sender, TwitchMessage message)
        {

            if (message.UserType >= TwitchUserTypes.Founder)
            {
                var icon = message.Args[1] + " ";
                sender.SendMessage(message.Channel, icon);
                sender.SendMessage(message.Channel, icon + icon);
                sender.SendMessage(message.Channel, icon + icon + icon);
                sender.SendMessage(message.Channel, icon + icon + icon + icon);
                sender.SendMessage(message.Channel, icon + icon + icon);
                sender.SendMessage(message.Channel, icon + icon);
                sender.SendMessage(message.Channel, icon);
            }
        }
        public void DoRaid(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType >= TwitchUserTypes.Broadcaster)
            {
                if (message.Args.Length > 1)
                {
                    if (m_raidMessages.ContainsKey(message.Channel) && !String.IsNullOrEmpty(m_raidMessages[message.Channel]))
                    {
                        RepeatAction(RAID_REPEAT, () => sender.SendMessage(message.Channel, m_raidMessages[message.Channel]));
                    }
                    RepeatAction(RAID_REPEAT, () => sender.SendMessage(message.Channel, TWITCH_URL + message.Args[1].ToLowerInvariant()));
                }
            }
        }
        public void DoRaidMessage(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType >= TwitchUserTypes.Broadcaster)
            {
                if (message.Args.Length > 1)
                {
                    string msg = message.Message.Substring(message.Args[0].Length + 1).Trim();
                    m_raidMessages[message.Channel] = msg;
                    sender.SendMessage(message.Channel, "Raid message has been set to : " + msg);
                }
            }
        }
        public void DoRoll(TwitchClient sender, TwitchMessage message)
        {
            int roll = 0;

            if (message.Args.Length < 2 || !int.TryParse(message.Args[1], out roll) || roll <= 0)
                return;

            sender.AutoDetectSendWhispers = true;
            sender.SendMessage(message.Channel, string.Format("{0} rolls a {1}", message.SenderDisplayName, RNG.Next(1, roll)));
        }
        public void DoTimeDiff(TwitchClient sender, TwitchMessage message)
        {
            if (message.Args.Length != 3)
            {
                return;
            }
            TimeSpan ts1, ts2;
            bool bts1 = TryParseTimeSpan(message.Args[1], out ts1);
            bool bts2 = TryParseTimeSpan(message.Args[2], out ts2);
            if (!bts1 || !bts2)
            {
                return;
            }
            var tsr = new TimeSpan(Math.Abs(ts1.Ticks - ts2.Ticks));
            sender.SendMessage(message.Channel, tsr.ToString());
        }
        public void DoUptime(TwitchClient sender, TwitchMessage message)
        {
            var d = new TimeSpan(DateTime.Now.Ticks - m_startTime.Ticks);
            sender.SendMessage(message.Channel, "Bot uptime : {0}d {1}h {2}m {3}s", d.Days, d.Hours, d.Minutes, d.Seconds);
        }
        public void DoSetAutoJoin(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Developer)
            {
                return;
            }
            if (message.Args.Count() == 2 && !message.IsWhisper)
            {
                switch (message.Args[1])
                {
                    case "on":
                        {
                            var channel = BotSettings.GetById(m_botId).GetChannel(message.Channel);
                            channel.AutoJoin = BotChannel.AutoJoinSettings.Yes;
                            channel.Save();
                            sender.SendMessage(message.Channel, "Autojoin enabled");
                        }
                        return;
                    case "off":
                        {
                            var channel = BotSettings.GetById(m_botId).GetChannel(message.Channel);
                            channel.AutoJoin = BotChannel.AutoJoinSettings.No;
                            channel.Save();
                            sender.SendMessage(message.Channel, "Autojoin disabled");
                        }
                        return;
                }
            }
        }

        public void DoLearnUser(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Founder)
            {
                return;
            }
            // usage !learn <username> <DisplayName> <FFZ/Twitch> <ID>
            if (message.Args.Count() < 3)
            {
                return;
            }
            var user = TwitchUser.GetOrCreateUser(null, message.Args[1], message.Args[2]);

            user.DisplayName = message.Args[2];
            if (message.Args.Count() >= 5)
            {
                long val = -1;
                bool success = long.TryParse(message.Args[4], out val);

                switch (message.Args[3].ToLowerInvariant())
                {
                    case "ffz":
                        if (success)
                        {
                            user.FFZId = val;
                            user.TwitchIconId = null;
                        }
                        break;
                    case "twitch":
                        if (success)
                        {
                            user.TwitchIconId = val;
                            user.FFZId = null;
                        }
                        break;
                    case "none":
                        user.FFZId = null;
                        user.TwitchIconId = null;
                        break;
                    default: break;
                }
            }


            user.Save();

            sender.SendMessage(message.Channel, "{0} learned successfully", user.DisplayName);
        }

        public static readonly string ALPHABET = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public static readonly string ALBHED = "YPLTAVKREZGMSHUBXNCDIJFQOWypltavkrezgmshubxncdijfqow";

        private void DoToAlBhed(TwitchClient sender, TwitchMessage message)
        {
            if (!message.IsWhisper)
                return;
            sender.SendWhisper(message.Channel, Substitute(message.Message, ALPHABET, ALBHED));
        }

        private void DoToFromBhed(TwitchClient sender, TwitchMessage message)
        {
            if (!message.IsWhisper)
                return;
            sender.SendWhisper(message.Channel, Substitute(message.Message, ALBHED, ALPHABET));
        }

        private string Substitute(string text, string referenceFrom, string referenceTo)
        {
            StringBuilder sb = new StringBuilder(text.Length);
            foreach (char c in text)
            {
                int i = referenceFrom.IndexOf(c);
                if (i != -1)
                    sb.Append(referenceTo[i]);
                else
                    sb.Append(c);
            }
            return sb.ToString();
        }

        private bool TryParseTimeSpan(string str, out TimeSpan ts)
        {
            ts = TimeSpan.MinValue;
            var spl = str.Split(new char[] { ':' });
            bool b1, b2, b3;
            int i1, i2, i3;
            if (spl.Length < 2)
                return false;
            if (spl.Length == 2)
            {
                b1 = int.TryParse(spl[0], out i1);
                b2 = int.TryParse(spl[1], out i2);
                if (!b1 || !b2)
                    return false;
                ts = new TimeSpan(0, i1, i2);
            }
            else
            {
                b1 = int.TryParse(spl[0], out i1);
                b2 = int.TryParse(spl[1], out i2);
                b3 = int.TryParse(spl[2], out i3);
                if (!b1 || !b2 || !b3)
                    return false;
                ts = new TimeSpan(i1, i2, i3);

            }


            return true;

        }

        private static void RepeatAction(int repeatCount, Action action, int delay = 200)
        {
            for (int i = 0; i < repeatCount; i++)
            {
                Thread.Sleep(delay);
                action();
            }
        }

        private void DoCelciusToFahrenheit(TwitchClient sender, TwitchMessage message)
        {
            if (message.IsWhisper)
                return;
            if (message.Args.Count() > 1)
            {
                try
                {
                    checked
                    {
                        decimal c = ParseTemperatureIdenpendently(message.Args[1]);
                        decimal f = ((c * 9m) / 5m) + 32m;
                        sender.SendMessage(message.Channel, "{0:0.0} °F", f);
                    }
                }
                catch { }
            }
        }
        private void DoFahrenheitToCelcius(TwitchClient sender, TwitchMessage message)
        {
            if (message.IsWhisper)
                return;
            if (message.Args.Count() > 1)
            {
                try
                {
                    checked
                    {
                        decimal f = ParseTemperatureIdenpendently(message.Args[1]);
                        decimal c = ((f - 32m) * 5m) / 9m;
                        sender.SendMessage(message.Channel, "{0:0.0} °C", c);
                    }
                }
                catch
                {
                    // Do nothing
                }
            }
        }
        static private decimal ParseTemperatureIdenpendently(string data)
        {
            string nospace = data.ToLowerInvariant().Replace(" ", "").Replace("f", "").Replace("c", "").Replace("°", "");
            bool negative = false;
            if (nospace.IndexOf('-') > -1)
            {
                negative = true;
                nospace = nospace.Replace("-", "");
            }
            int comma = nospace.LastIndexOf(',');
            int dot = nospace.LastIndexOf('.');

            NumberStyles style = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;
            CultureInfo provider = new CultureInfo("fr-FR");

            string parsable = null;
            if (comma > dot)
            {
                parsable = nospace.Replace(".", "");
            }
            else if (dot > comma)
            {
                parsable = nospace.Replace(",", "").Replace(".", ",");
            }
            else
            {
                parsable = nospace;
            }
            decimal v = decimal.Parse(parsable, style, provider);
            if (negative)
                v = -v;

            return v;

        }
    }

}
