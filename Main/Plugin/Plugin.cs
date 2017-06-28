using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Main
{
    public abstract class Plugin
    {
        public abstract void OnLoad(PluginManager commandsManager);


        public virtual void BeforeCommand(TwitchClient sender, TwitchMessage message)
        {

        }

        public virtual void AfterCommand(TwitchClient sender, TwitchMessage message)
        {

        }

        public virtual void OnBitsSent(TwitchClient sender, TwitchMessage message)
        {

        }

        public virtual void OnNotice(TwitchClient sender, TwitchNotice notice)
        {

        }
    }
}
