﻿using CitiBot.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Plugins.Counters
{
    class CountersCommands : Plugin
    {
        public override void OnLoad(PluginManager commandsManager)
        {
            commandsManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoDeathCounter, "!dead", "!rip", "!death") { ChannelCooldown = 5 });
            commandsManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoResetDeath, "!resetdead", "!resetrip", "!resetdeath") { ChannelCooldown = 5 });
            commandsManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoGetDeathCount, "!deadcount", "!ripcount", "!deathcount") { ChannelCooldown = 5 });
            commandsManager.RegisterCommand(
                new PluginManager.OnMessageAction(this, DoSetDeathCounter, "!setdead", "!setrip", "!setdeath") { ChannelCooldown = 5 });
        }

        public void DoDeathCounter(TwitchClient sender, TwitchMessage message)
        {
            if (message.IsWhisper)
                return;
            long cnt = 1;
            if (message.Args.Count() > 1 && !long.TryParse(message.Args[1], out cnt))
                return;
            if (cnt < 1)
                return;
            long c = Counters.Models.Counter.Increment(message.Channel, "deaths", cnt);
            sender.SendMessage(message.Channel, "{0} has died {1} times !", message.Channel.Replace("#", ""), c);
        }


        public void DoResetDeath(TwitchClient sender, TwitchMessage message)
        {
            if (message.IsWhisper)
                return;
            if (message.UserType < TwitchUserTypes.Mod)
                return;
            Counters.Models.Counter.Reset(message.Channel, "deaths");
            sender.SendMessage(message.Channel, "Death counter has been reset");
        }

        public void DoGetDeathCount(TwitchClient sender, TwitchMessage message)
        {
            if (message.IsWhisper)
                return;
            long cnt = Counters.Models.Counter.GetCount(message.Channel, "deaths");
            sender.SendMessage(message.Channel, "{0} has died {1} times !", message.Channel.Replace("#", ""), cnt);
        }


        public void DoSetDeathCounter(TwitchClient sender, TwitchMessage message)
        {
            if (message.IsWhisper)
                return;
            if (message.UserType < TwitchUserTypes.Mod)
                return;
            long cnt = 0;
            if (message.Args.Count() < 1 && !long.TryParse(message.Args[1], out cnt))
                return;
            Counters.Models.Counter.SetCount(message.Channel, "deaths", cnt);
            sender.SendMessage(message.Channel, "{0} has died {1} times !", message.Channel.Replace("#", ""), cnt);
            
        }
    }
}
