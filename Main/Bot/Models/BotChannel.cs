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

    [Table("t_bots_channels")]
    public class BotChannel : BaseModel<BotChannel>
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Channel { get; set; }
        public virtual BotSettings BotSettings { get; set; }
        public virtual GreetingsTypes Greetings { get; set; }
        public virtual AutoJoinSettings AutoJoin { get; set; }

        private bool isNew = false;

        public static BotChannel New(string channel, BotSettings settings, GreetingsTypes greetings = GreetingsTypes.None)
        {
            var n = new BotChannel();
            n.Channel = channel;
            n.BotSettings = settings;
            n.Greetings = greetings;
            n.AutoJoin = AutoJoinSettings.Yes;
            n.isNew = true;
            return n;
        }

        public virtual void Save()
        {
            this.Save(isNew);
        }

        public enum GreetingsTypes
        {
            None = 0,
            Simple = 1
        }


        public enum AutoJoinSettings
        {
            No = 0,
            Yes = 1
        }
    }
}
