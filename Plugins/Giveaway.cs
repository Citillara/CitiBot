using Irc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Plugins
{
    class Giveaway
    {
        private bool active = false;
        private bool inProgress = false;
        private string channel;
        private Dictionary<string,List<string>> channelCompetitors;

        private Giveaway() { }

        public Giveaway(string channel)
        {
            this.channel = channel;
        }

        public void Activate()
        {
            active = true;
        }

        public void OnPrivateMessage(IrcClient sender, string channel, string nickname, string message)
        {
            
        }
    }
}
