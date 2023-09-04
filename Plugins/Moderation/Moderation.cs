using CitiBot.Helpers;
using CitiBot.Main;
using CitiBot.Main.Models;
using CitiBot.Plugins.Moderation.Models;
using CitiBot.Plugins.Twitch.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Plugins.Moderation
{
    public class Moderation : Plugin
    {
        private readonly List<string> m_all_blacklistitems = new List<string>();
        private readonly Dictionary<string, List<string>> m_channel_blacklist = new Dictionary<string, List<string>>();

        private string m_botUsername;
        private string BotUsername
        {
            get
            {
                if (m_botUsername == null)
                {
                    m_botUsername = BotSettings.GetById(this.Manager.BotId).Name;
                }
                return m_botUsername;
            }
        }

        public override void OnLoad(PluginManager pluginManager)
        {
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoAddBlacklistItem, "!addblacklist") { ChannelCooldown = 5 });
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoAddBlacklistItemGlobal, "!addblacklistglobal") { ChannelCooldown = 5 });
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoRehashBlacklist, "!rehashblacklist") { GlobalCooldown = 5 });
            pluginManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoGetLocalBlacklistCount, "!localblacklistcount") { ChannelCooldown = 5 });

            RehashAll();
        }

        public override void OnMessage(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType >= TwitchUserTypes.Mod)
                return;

            string mess = message.Message.ToLowerInvariant();
            if (m_all_blacklistitems.Any(a => mess.Contains(a))
            || (m_channel_blacklist.ContainsKey(message.Channel)
                &&
                 m_channel_blacklist[message.Channel].Any(a => mess.Contains(a))
                )
            )
            {
                ApiHelper.CallBanApi(BotUsername, message.Channel.Replace("#", ""), message.UserId, "Blacklisted words");
                Counters.Models.Counter.Increment(message.Channel, message.Channel + "-ban");
                sender.SendAction(message.Channel, "bonks {0} to oblivion citiBoink2 citiBoink2", message.SenderDisplayName);
                Log.AddBusinessLog(DateTime.Now, Log.LogLevel.Info, message.Channel, "Ban", $"{message.SenderName} was banned from {message.Channel}. Trigger : {mess}");
            }

        }

        public void DoGetLocalBlacklistCount(TwitchClient sender, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Broadcaster)
            {
                return;
            }
            int cnt = m_all_blacklistitems.Count();
            if (m_channel_blacklist.ContainsKey(message.Channel))
                cnt += m_channel_blacklist[message.Channel].Count();

            sender.SendMessage(message.Channel, "{0} blacklist sentences loaded on this channel", cnt);
        }

        public void DoRehashBlacklist(TwitchClient client, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Developer)
            {
                return;
            }
            if (message.IsWhisper)
            {
                ApiHelper.CallWhisperApi(BotUsername, message.UserId, "Sorry by that command is not supported over whisper");
                return;
            }
            RehashAll();

            client.SendMessage(message.Channel, "Blacklist rehashed");
        }

        private void Rehash(string channel)
        {
            var list = ModerationBlacklistItem.GetCommonItems();

            m_all_blacklistitems.Clear();
            m_all_blacklistitems.AddRange(list.Select(a => a.Text.ToLowerInvariant()));

            var clist = ModerationBlacklistItem.GetChannelItems(channel);
            if (!m_channel_blacklist.ContainsKey(channel))
                m_channel_blacklist.Add(channel, new List<string>());
            else
                m_channel_blacklist[channel].Clear();

            m_channel_blacklist[channel].AddRange(clist.Select(a => a.Text.ToLowerInvariant()));

        }
        private void RehashAll()
        {
            var list = ModerationBlacklistItem.GetAllItems();

            m_all_blacklistitems.Clear();
            m_channel_blacklist.Clear();

            foreach (var item in list)
            {
                if (item.Channel == "all")
                    m_all_blacklistitems.Add(item.Text.ToLowerInvariant());
                else
                {
                    if (!m_channel_blacklist.ContainsKey(item.Channel))
                        m_channel_blacklist.Add(item.Channel, new List<string>());
                    m_channel_blacklist[item.Channel].Add(item.Text.ToLowerInvariant());
                }
            }
        }

        public void DoAddBlacklistItemGlobal(TwitchClient client, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Developer)
            {
                ApiHelper.CallWhisperApi(BotUsername, message.UserId, "Sorry {0}, this command is only for Developer and above.", message.SenderDisplayName);
                return;
            }

            string msg = message.Message.Replace("\"", "").Trim();

            if (msg.IndexOf(' ') == -1)
            {
                ApiHelper.CallWhisperApi(BotUsername, message.UserId, "Please specify what you want to add to the moderation database");
                return;
            }

            string sub = msg.Substring(msg.IndexOf(' ')).Trim();

            ModerationBlacklistItem.AddNewModerationBlacklistItem("all", sub, message.SenderName);


            client.SendMessage(message.Channel, "\"{0}\" have been added to the global moderation database", sub);
            Rehash(message.Channel);
        }

        public void DoAddBlacklistItem(TwitchClient client, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Mod)
            {
                ApiHelper.CallWhisperApi(BotUsername, message.UserId, "Sorry {0}, this command is only for mods and above.", message.SenderDisplayName);
                return;
            }
            if (message.IsWhisper)
            {
                ApiHelper.CallWhisperApi(BotUsername, message.UserId, "Sorry by that command is not supported over whisper");
                return;
            }

            string msg = message.Message.Replace("\"", "").Trim();

            if (msg.IndexOf(' ') == -1)
            {
                ApiHelper.CallWhisperApi(BotUsername, message.UserId, "Please specify what you want to add to the moderation database");
                return;
            }

            string sub = msg.Substring(msg.IndexOf(' ')).Trim();

            ModerationBlacklistItem.AddNewModerationBlacklistItem(message.Channel, sub, message.SenderName);


            client.SendMessage(message.Channel, "\"{0}\" have been added to the moderation database", sub);
            Rehash(message.Channel);
        }
        public void EnableLanguageBan(TwitchClient client, TwitchMessage message)
        {
            if (message.UserType < TwitchUserTypes.Mod)
            {
                ApiHelper.CallWhisperApi(BotUsername, message.UserId, "Sorry {0}, this command is only for mods and above.", message.SenderDisplayName);
                return;
            }
            if (message.IsWhisper)
            {
                ApiHelper.CallWhisperApi(BotUsername, message.UserId, "Sorry by that command is not supported over whisper");
                return;
            }


            string msg = message.Message.Replace("\"", "").Trim();
            switch (msg)
            {
                case "cyrillic":
                    break;
                default: break;
            }

            Rehash(message.Channel);
        }

        static readonly Regex cyrillicRegex = new Regex("[\u0400-\u04FF]{4,}", RegexOptions.Compiled);

        public static bool ContainsCyrillic(string text)
        {
            return cyrillicRegex.IsMatch(text);
        }
    }
}
