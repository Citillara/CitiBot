using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Plugins.CookieGiver.Models
{
    [DataContract]
    public class CookieFlavour
    {
        [DataMember]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        [DataMember]
        public virtual string Channel { get; set; }
        [DataMember]
        public virtual DateTime? AddedDate { get; set; }
        [DataMember]
        public virtual string AddedBy { get; set; }
        [DataMember]
        public virtual string Text { get; set; }

        public static IEnumerable<Int32> GetChannelCookies(string channel)
        {
            return Database.Instance.CookieFlavours.Where(c => c.Channel == channel).Select(c => c.Id);
        }

        public static IEnumerable<Int32> GetCommonCookies()
        {
            return Database.Instance.CookieFlavours.Where(c => c.Channel == "all").Select(c => c.Id);
        }

        public static CookieFlavour GetCookie(int id)
        {
            return Database.Instance.CookieFlavours.Where(c => c.Id == id).FirstOrDefault();
        }
        public static int GetCookieCount()
        {
            return Database.Instance.CookieFlavours.Count();
        }

        public virtual void Save()
        {

            var db = Database.Instance;
            var id = this.Id;
            if (db.CookieUsers.Any(e => e.Id == id))
            {
                db.Set<CookieFlavour>().Attach(this);
                db.Entry<CookieFlavour>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.CookieFlavours.Add(this);
                db.Entry<CookieFlavour>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }
    }
}
