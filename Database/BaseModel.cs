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
            if (hasBeenDeleted)
                return;
            hasBeenDeleted = true;
            T t = (T)(object)this;
            DoDelete(t);
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
                DoAdd(t);
            }
            else
            {
                DoModify(t);
            }
        }

        private static void DoAdd(T t)
        {
            Database.Registry.Instance.Set<T>().Add(t);
            Database.Registry.Instance.Entry<T>(t).State = System.Data.Entity.EntityState.Added;
        }
        private static void DoModify(T t)
        {
            Database.Registry.Instance.Set<T>().Attach(t);
            Database.Registry.Instance.Entry<T>(t).State = System.Data.Entity.EntityState.Modified;
        }
        private static void DoDelete(T t)
        {
            Database.Registry.Instance.Entry<T>(t).State = System.Data.Entity.EntityState.Deleted;
            Database.Registry.Instance.Set<T>().Remove(t);
        }

        protected static T FirstOrDefaultLocal(Func<T, bool> condition)
        {
            return Database.Registry.Instance.Set<T>().Local.Where(condition).FirstOrDefault() 
                ?? Database.Registry.Instance.Set<T>().Where(condition).FirstOrDefault();
        }

        protected static T FirstOrDefaultLocal(string include, Func<T, bool> condition)
        {
            return Database.Registry.Instance.Set<T>().Local.Where(condition).FirstOrDefault()
                ?? Database.Registry.Instance.Set<T>().Include(include).Where(condition).FirstOrDefault();
        }
    }
}
