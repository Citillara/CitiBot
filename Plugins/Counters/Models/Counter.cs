using CitiBot.Database;
using CitiBot.Main;
using CitiBot.Plugins.Twitch.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Plugins.Counters.Models
{
    [DataContract]
    [Table("t_counters")]
    public class Counter : BaseModel<Counter>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Channel { get; set; }
        public virtual string Name { get; set; }
        public virtual long Count { get; set; }

        private bool isNew = false;

        private static Counter GetOrCreate(string channel, string name)
        {
            var cnt = Registry.Instance.Counters.Where(c => c.Channel == channel && c.Name == name).FirstOrDefault();
            if (cnt != null)
                return cnt;
            return new Counter()
            {
                Channel = channel,
                Name = name,
                Count = 0,
                isNew = true
            };
        }

        public static long GetCount(string channel, string name)
        {
            var cnt = GetOrCreate(channel, name);
            return cnt.Count;
        }

        public static void SetCount(string channel, string name, long count)
        {
            var cnt = GetOrCreate(channel, name);
            cnt.Count = count;
            cnt.Save();
        }

        public static void Reset(string channel, string name)
        {
            SetCount(channel, name, 0);
        }
        public static long Increment(string channel, string name, long count = 1)
        {
            var cnt = GetOrCreate(channel, name);
            cnt.Count = cnt.Count + count;
            cnt.Save();
            return cnt.Count;
        }


        public virtual void Save()
        {
            this.Save(isNew);
            if (isNew)
                isNew = false;
        }

    }
}
