using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Main
{
    public interface IPlugin
    {
        void OnLoad(PluginManager commandsManager);
    }
}
