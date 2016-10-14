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

    [Table("t_bots_plugins")]
    public class BotPlugin
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string PluginName { get; set; }
        public virtual BotSettings BotSettings { get; set; }


        public virtual void Save()
        {

            var db = Registry.Instance;
            var id = this.Id;

            if (db.CookieUsers.Any(e => e.Id == id))
            {
                db.Set<BotPlugin>().Attach(this);
                db.Entry<BotPlugin>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.BotPlugins.Add(this);
                db.Entry<BotPlugin>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }

    }
}
