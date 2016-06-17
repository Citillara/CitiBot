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
    public class CookieBox
    {
        [DataMember]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        [DataMember]
        public virtual string Channel { get; set; }
        [DataMember]
        public virtual int Amount { get; set; }
        [DataMember]
        public virtual DateTime? LastAddedDate { get; set; }
        [DataMember]
        public virtual string LastAddedBy { get; set; }

        public static int GetAmount(string channel)
        {
            var entry = Database.Instance.CookieBoxes.Where(d => d.Channel == channel).First();
            if (entry == null)
                return -1;
            return entry.Amount;
        }

        public static CookieBox GetCookieBox(string channel)
        {
            return Database.Instance.CookieBoxes.Where(d => d.Channel == channel).FirstOrDefault();
        }

        public virtual void Save()
        {

            var db = Database.Instance;
            var id = this.Id;

            if (db.CookieUsers.Any(e => e.Id == id))
            {
                db.Set<CookieBox>().Attach(this);
                db.Entry<CookieBox>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.CookieBoxes.Add(this);
                db.Entry<CookieBox>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }
    }
}
