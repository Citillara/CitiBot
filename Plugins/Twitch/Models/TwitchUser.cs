using CitiBot.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Plugins.Twitch.Models
{

    [DataContract]
    [Table("t_twitch_users")]
    public class TwitchUser
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }

        public static TwitchUser GetUser(string name)
        {
            return Registry.Instance.TwitchUsers.Where(u => u.Name == name).FirstOrDefault();
        }

        public static TwitchUser GetUser(int id)
        {
            return Registry.Instance.TwitchUsers.Where(c => c.Id == id).FirstOrDefault();
        }
        
        public virtual void Save()
        {

            var db = Registry.Instance;
            var id = this.Id;
            
            if (db.CookieUsers.Any(e => e.Id == id))
            {
                db.Set<TwitchUser>().Attach(this);
                db.Entry<TwitchUser>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.TwitchUsers.Add(this);
                db.Entry<TwitchUser>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }

    }
}

