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
        protected internal PluginManager Manager { get; set; }

        public abstract void OnLoad(PluginManager commandsManager);

        /// <summary>
        /// Is executed before any registered command. Return false if you want to cancel the command
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public virtual bool BeforeCommand(TwitchClient sender, TwitchMessage message)
        {
            return true;
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
        public virtual void OnMessage(TwitchClient sender, TwitchMessage message)
        {

        }
    }
}
