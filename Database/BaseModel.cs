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


        protected void Save(bool isNew)
        {
            T t = (T)(object)this;
            if (isNew)
            {
                Database.Registry.Instance.Set<T>().Add(t);
                Database.Registry.Instance.Entry<T>(t).State = System.Data.Entity.EntityState.Added;
            }
            else
            {
                Database.Registry.Instance.Set<T>().Attach(t);
                Database.Registry.Instance.Entry<T>(t).State = System.Data.Entity.EntityState.Modified;
            }

            Database.Registry.Instance.SaveChanges();
        }
    }
}
