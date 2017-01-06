using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Database
{
    public abstract class BaseModel<T> where T : class
    {
        protected Registry DB { get { return Database.Registry.Instance; } }


        protected void Save(bool isNew)
        {
            T t = (T)(object)this;
            if (isNew)
            {
                DB.Set<T>().Add(t);
                DB.Entry<T>(t).State = System.Data.Entity.EntityState.Added;
            }
            else
            {
                DB.Set<T>().Attach(t);
                DB.Entry<T>(t).State = System.Data.Entity.EntityState.Modified;
            }

            DB.SaveChanges();
        }
    }
}
