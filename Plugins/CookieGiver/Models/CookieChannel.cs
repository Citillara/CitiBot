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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Channel { get; set; }
        protected virtual int? CookieDelayDB { get; set; }
        [Column("BribeDelay")]
        protected virtual int? BribeDelayDB { get; set; }
        [Column("StealDelay")]
        protected virtual int? StealDelayDB { get; set; }

        [NotMapped]
        public int CookieDelay
        {
            get { return CookieDelayDB.HasValue ? CookieDelayDB.Value : 300; }
            set { CookieDelayDB = value; }
        }

        [NotMapped]
        public int BribeDelay
        {
            get { return BribeDelayDB.HasValue ? BribeDelayDB.Value : 300; }
            set { BribeDelayDB = value; }
        }

        [NotMapped]
        public int StealDelay
        {
            get { return StealDelayDB.HasValue ? StealDelayDB.Value : 300; }
            set { StealDelayDB = value; }
        }


        public static CookieChannel GetChannel(string channel)
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
