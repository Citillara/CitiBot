using CitiBot.Database;
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
    [Table("t_cookie_flavours")]
    public class CookieFlavour
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Channel { get; set; }
        public virtual DateTime? AddedDate { get; set; }
        public virtual string AddedBy { get; set; }
        public virtual string Text { get; set; }

        public static IEnumerable<Int32> GetChannelCookies(string channel)
        {
            return Registry.Instance.CookieFlavours.Where(c => c.Channel == channel).Select(c => c.Id);
        }

        public static IEnumerable<Int32> GetCommonCookies()
        {
            return Registry.Instance.CookieFlavours.Where(c => c.Channel == "all").Select(c => c.Id);
        }

        public static CookieFlavour GetCookie(int id)
        {
            return Registry.Instance.CookieFlavours.Where(c => c.Id == id).FirstOrDefault();
        }
        public static int GetCookieCount()
        {
            return Registry.Instance.CookieFlavours.Count();
        }

        public virtual void Save()
        {

            var db = Registry.Instance;
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
