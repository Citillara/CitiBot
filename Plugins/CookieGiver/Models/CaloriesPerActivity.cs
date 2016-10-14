﻿using CitiBot.Database;
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
    [Table("t_cookie_calories")]
    public class CaloriesPerActivity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Text { get; set; }
        public virtual int Calories { get; set; }


        public static IEnumerable<Int32> GetListOfActivities()
        {
            return Registry.Instance.CaloriesPerActivity.Select(c => c.Id);
        }

        public static CaloriesPerActivity GetById(int id)
        {
            return Registry.Instance.CaloriesPerActivity.Where(c => c.Id == id).FirstOrDefault();
        }


        public virtual void Save()
        {

            var db = Registry.Instance;
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
