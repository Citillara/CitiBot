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
    public class BotChannel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Channel { get; set; }
        public virtual BotSettings BotSettings { get; set; }


        public virtual void Save()
        {

            var db = Registry.Instance;
            var id = this.Id;

            if (db.CookieUsers.Any(e => e.Id == id))
            {
                db.Set<BotChannel>().Attach(this);
                db.Entry<BotChannel>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.BotChannels.Add(this);
                db.Entry<BotChannel>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }

    }
}
