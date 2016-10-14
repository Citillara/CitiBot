using CitiBot.Database;
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
    [Table("t_cookie_channels")]
    public class CookieChannel
    {
        [DataMember]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        [DataMember]
        public virtual string Channel { get; set; }
        [DataMember]
        public virtual int Delay { get; set; }

        public static int GetDelay(string channel)
        {
            var entry = Registry.Instance.CookieChannels.Where(d => d.Channel == channel).FirstOrDefault();
            if (entry == null)
                return -1;
            return entry.Delay;
        }

        public static int GetCookieDelay(string channel)
        {
            var r = GetChannelOrNew(channel);
            if (r == null || r.Delay == 0)
                return -1;
            else
                return r.Delay;
        }
        public static void SetCookieDelay(string channel, int delay)
        {
            var r = GetChannelOrNew(channel);
            r.Delay = delay;
            r.Save();

        }

        public static CookieChannel GetChannelOrNew(string channel)
        {
            var r = Registry.Instance.CookieChannels.Where(d => d.Channel == channel).FirstOrDefault();
            if (r == null)
                r = new CookieChannel() { Channel = channel };
            return r;
        }

        public virtual void Save()
        {

            var db = Registry.Instance;
            var id = this.Id;

            if (db.CookieUsers.Any(e => e.Id == id))
            {
                db.Set<CookieChannel>().Attach(this);
                db.Entry<CookieChannel>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.CookieChannels.Add(this);
                db.Entry<CookieChannel>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }
    }
}
