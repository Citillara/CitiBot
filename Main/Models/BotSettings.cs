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
    public class BotSettings
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Password { get; set; }
        public virtual short Enabled { get; set; }
        public virtual ICollection<BotPlugin> Plugins { get; set; }
        public virtual ICollection<BotChannel> Channels { get; set; }


        public virtual void Save()
        {

            var db = Registry.Instance;
            var id = this.Id;

            if (db.CookieUsers.Any(e => e.Id == id))
            {
                db.Set<BotSettings>().Attach(this);
                db.Entry<BotSettings>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.BotSettings.Add(this);
                db.Entry<BotSettings>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }

        public static IEnumerable<BotSettings> GetAllBots()
        {
            return Registry.Instance.BotSettings.Where(b => b.Enabled == 1).ToList();
        }

    }
}
