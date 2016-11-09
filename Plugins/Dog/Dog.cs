using CitiBot.Main;
using CitiBot.Plugins.Dog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Twitch;
using Twitch.Models;

namespace CitiBot.Plugins.Dog
{
    public class Dog : IPlugin
    {
        Random m_Random = new Random();

        public void OnLoad(PluginManager pluginManager)
        {
            pluginManager.RegisterCommand("!dig2", Dig);
        }

        private void Dig(TwitchClient client, TwitchMessage message)
        {
            if (message.IsWhisper)
            {
                client.SendWhisper(message.Channel, "Sorry by that command is not supported over whisper");
                return;
            }

            var user = DogUser.GetUser(message.Channel, message.SenderName);

            if (user != null && user.LastReceived.HasValue)
            {
                if (message.UserType < TwitchUserTypes.Broadcaster && user.LastReceived.Value.AddMinutes(5) > DateTime.Now)
                {
                    client.SendWhisper(message.SenderName,
                        "Sorry {0}, but you can use that command only every 5 minutes.",
                        message.SenderDisplayName);
                    return;
                }
            }

            int found_bones = m_Random.Next(0, 100);
            if (found_bones > 50)
            {
                client.SendMessage(message.Channel, "{0} digs but founds nothing.", message.SenderDisplayName);
                return;
            }

            if (user == null)
            {
                user = new DogUser()
                {
                    BonesReceived = 0,
                    Channel = message.Channel,
                    LastReceived = DateTime.Now,
                    TopBonesCount = 0,
                    Username = message.SenderName
                };
            }


            int bones_found = m_Random.Next (1, 21);

            user.BonesReceived += bones_found;

            client.SendMessage(message.Channel, "{0} digs and finds {1} bones ! (Total : {2})",
                message.SenderDisplayName, bones_found, user.BonesReceived);

            user.Save();
        }
    }
}
