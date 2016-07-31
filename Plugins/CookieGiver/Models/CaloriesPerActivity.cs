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
    [Table("caloriesperactivities")]
    public class CaloriesPerActivity
    {
        [DataMember]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        [DataMember]
        public virtual string Text { get; set; }
        [DataMember]
        public virtual int Calories { get; set; }


        public static IEnumerable<Int32> GetListOfActivities()
        {
            return Database.Instance.CaloriesPerActivity.Select(c => c.Id);
        }

        public static CaloriesPerActivity GetById(int id)
        {
            return Database.Instance.CaloriesPerActivity.Where(c => c.Id == id).FirstOrDefault();
        }


        public virtual void Save()
        {

            var db = Database.Instance;
            var id = this.Id;

            if (db.CookieUsers.Any(e => e.Id == id))
            {
                db.Set<CaloriesPerActivity>().Attach(this);
                db.Entry<CaloriesPerActivity>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.CaloriesPerActivity.Add(this);
                db.Entry<CaloriesPerActivity>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }
    }
}
