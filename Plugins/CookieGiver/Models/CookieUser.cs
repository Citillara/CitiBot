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

namespace CitiBot.Plugins.CookieGiver.Models
{
    [DataContract]
    public class CookieUser
    {

        [DataMember]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        [DataMember]
        public virtual int CookieReceived { get; set; }
        [DataMember]
        public virtual DateTime? LastReceived { get; set; }
        [DataMember]
        public virtual DateTime? LastYoshiBribe { get; set; }
        [DataMember]
        public virtual DateTime? LastSend { get; set; }
        [DataMember]
        public virtual string Username { get; set; }
        [DataMember]
        public virtual string Channel { get; set; }

        public static CookieUser GetUser(string channel, string username)
        {
            return Database.Instance.CookieUsers.Where(c => c.Channel == channel && c.Username == username).FirstOrDefault();
        }

        public static CookieUser GetUser(int id)
        {
            return Database.Instance.CookieUsers.Where(c => c.Id == id).FirstOrDefault();
        }


        public static int GetUserRankingInChannel(string channel, string username)
        {
            var sorted = from db in Database.Instance.CookieUsers
                         orderby db.CookieReceived descending
                         where db.Channel == channel
                         select new
                         {
                             Username = db.Username,
                             Rank = (from dbb in Database.Instance.CookieUsers
                                     where dbb.CookieReceived > db.CookieReceived
                                     where dbb.Channel == channel
                                     select dbb).Count() + 1
                         };
            var result = sorted.Where(c => c.Username == username).FirstOrDefault();
            if (result == null)
                return -1;
            return result.Rank;
        }


        public static int GetChannelUserCount(string channel)
        {
            return Database.Instance.CookieUsers.Count(c => c.Channel == channel);
        }
        public static IEnumerable<CookieUser> GetChannelTopUsers(string channel, int count)
        {
            return Database.Instance.CookieUsers.Where(c => c.Channel == channel).OrderByDescending(c => c.CookieReceived).Take(count);
        }
        public static IEnumerable<Int32> GetChannelUserIdsWithCookies(string channel)
        {
            return Database.Instance.CookieUsers.Where(c => c.Channel == channel && c.CookieReceived > 0).Select(c => c.Id);
        }

        public virtual void Save()
        {

            var db = Database.Instance;
            var id = this.Id;

            if (this.LastReceived == DateTime.MinValue)
                this.LastReceived = null;
            if (this.LastSend == DateTime.MinValue)
                this.LastSend = null;
            if (this.LastYoshiBribe == DateTime.MinValue)
                this.LastYoshiBribe = null;


            if (db.CookieUsers.Any(e => e.Id == id))
            {
                db.Set<CookieUser>().Attach(this);
                db.Entry<CookieUser>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.CookieUsers.Add(this);
                db.Entry<CookieUser>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }

    }
}
