using CitiBot.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Main.Models
{
    [Table("t_bots")]
    public class BotSettings : BaseModel<BotSettings>
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        public virtual short Enabled { get; set; }
        public virtual short CallbackPort { get; set; }
        public virtual ICollection<BotPlugin> Plugins { get; set; }
        public virtual ICollection<BotChannel> Channels { get; set; }

        private bool isNew = false;

        public virtual void Save()
        {
            this.Save(isNew);
        }

        public ICollection<BotChannel> GetAutoJoinChannels()
        {
            return Channels.Where(c => c.AutoJoin == BotChannel.AutoJoinSettings.Yes).ToList();
        }

        public BotChannel GetChannel(string channel)
        {
            var chan = Channels.Where(c => c.Channel == channel).FirstOrDefault();
            if (chan != null)
                return chan;

            chan = BotChannel.New(channel, this);
            this.Channels.Add(chan);
            
            return chan;
        }

        public static BotSettings GetById(int id)
        {
            return Registry.Instance.BotSettings.Where(b => b.Id == id).FirstOrDefault();
        }

        public static IEnumerable<BotSettings> GetAllBots()
        {
            return Registry.Instance.BotSettings.Where(b => b.Enabled == 1).ToList();
        }

    }
}
