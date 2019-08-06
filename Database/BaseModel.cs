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
        private bool hasBeenDeleted = false;
        private bool doOnce = false;

        public virtual void Delete()
        {
            hasBeenDeleted = true;
            T t = (T)(object)this;
            Database.Registry.Instance.Entry<T>(t).State = System.Data.Entity.EntityState.Deleted;
            Database.Registry.Instance.Set<T>().Remove(t);

            //Database.Registry.Instance.SaveChanges();
        }

        protected void Save(bool isNew)
        {
            if (hasBeenDeleted)
                return;

            if (doOnce)
                return;
            doOnce = true;

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

            //Database.Registry.Instance.SaveChanges();
        }
    }
}
