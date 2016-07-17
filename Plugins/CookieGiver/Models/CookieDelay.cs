using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Plugins.CookieGiver.Models
{
    [DataContract]
    public class CookieDelay
    {
        [DataMember]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        [DataMember]
        public virtual string Channel { get; set; }
        [DataMember]
        public virtual int Delay { get; set; }
        [DataMember]
        public virtual DateTime? ChangedLast { get; set; }
        [DataMember]
        public virtual string ChangedBy { get; set; }

        public static int GetDelay(string channel)
        {
            var entry = Database.Instance.CookieDelays.Where(d => d.Channel == channel).FirstOrDefault();
            if (entry == null)
                return -1;
            return entry.Delay;
        }

        public static CookieDelay GetCookieDelay(string channel)
        {
            return Database.Instance.CookieDelays.Where(d => d.Channel == channel).FirstOrDefault();
        }

        public virtual void Save()
        {

            var db = Database.Instance;
            var id = this.Id;

            if (db.CookieUsers.Any(e => e.Id == id))
            {
                db.Set<CookieDelay>().Attach(this);
                db.Entry<CookieDelay>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.CookieDelays.Add(this);
                db.Entry<CookieDelay>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }
    }
}
