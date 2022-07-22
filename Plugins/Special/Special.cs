using CitiBot.Main;
using CitiBot.Main.Models;
using CitiBot.Plugins.Moderation.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Plugins.Special
{
    public class Special : Plugin
    {
        public override void OnLoad(PluginManager pluginManager)
        {

        }

        long lastCitiMessagge = -1;

        public override void OnMessage(TwitchClient sender, TwitchMessage message)
        {
            if (message.Channel != "#kaguyanicky")
                return;
            if (message.SenderName == "citillara")
            {
                lastCitiMessagge = message.TwitchTimestamp;
            }
            else if (message.SenderName == "buttsbot")
            {
                long diff = message.TwitchTimestamp - lastCitiMessagge;
                if( diff > 0 && diff < 2000)
                {
                    sender.SendAction(message.Channel, "boinks buttsbot very hard! citiBoink2 citiBoink2 citiBoink2 citiBoink2");
                }
            }
        }
    }
}
