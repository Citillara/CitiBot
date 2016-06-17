using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot
{
    public interface IPlugin
    {
        void Load(CommandsManager commandsManager);
    }
}
