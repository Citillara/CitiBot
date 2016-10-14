using CitiBot.Database;
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

namespace CitiBot.Plugins.CookieGiver.Models
{
    [Table("t_cookie_users")]
    public class CookieUser
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual int CookieReceived { get; set; }
        public virtual int TopCookieCount { get; set; }
        public virtual DateTime? LastReceived { get; set; }
        public virtual DateTime? LastYoshiBribe { get; set; }
        public virtual DateTime? LastSend { get; set; }
        public virtual string Username { get; set; }
        public virtual string Channel { get; set; }

        [NotMapped]
        public string DisplayName { get; set; }

        public CookieUser()
        {
        }

        public static CookieUser New(CookieUser user, string displayName = null)
        {
            return new CookieUser()
            {
                Channel = user.Channel,
                CookieReceived = user.CookieReceived,
                DisplayName = displayName,
                Id = user.Id,
                LastReceived = user.LastReceived,
                LastSend = user.LastSend,
                LastYoshiBribe = user.LastYoshiBribe,
                TopCookieCount = user.TopCookieCount,
                Username = user.Username
            };
        }

        public static CookieUser GetUser(string channel, string username)
        {
            return Registry.Instance.CookieUsers.Where(c => c.Channel == channel && c.Username == username).FirstOrDefault();
        }

        public static CookieUser GetUser(int id)
        {
            return Registry.Instance.CookieUsers.Where(c => c.Id == id).FirstOrDefault();
        }


        public static int GetUserRankingInChannel(string channel, string username)
        {
            var sorted = from db in Registry.Instance.CookieUsers
                         orderby db.CookieReceived descending
                         where db.Channel == channel
                         select new
                         {
                             Username = db.Username,
                             Rank = (from dbb in Registry.Instance.CookieUsers
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
            return Registry.Instance.CookieUsers.Count(c => c.Channel == channel);
        }
        public static IEnumerable<CookieUser> GetChannelTopUsers(string channel, int count)
        {

            var result = from db in Registry.Instance.CookieUsers
                         where db.Channel == channel
                         join tu in Registry.Instance.TwitchUsers on db.Username equals tu.Name into joined
                         from subtu in joined.DefaultIfEmpty()
                         orderby db.CookieReceived descending
                         select new { db, subtu };
            var take = result.Take(count);
            var list = new List<CookieUser>(take.Count());
            take.ToList().ForEach(e => list.Add(New(e.db, e.subtu == null ? null : e.subtu.DisplayName)));
            return list;
        }
        public static IEnumerable<Int32> GetChannelUserIdsWithCookies(string channel)
        {
            return Registry.Instance.CookieUsers.Where(c => c.Channel == channel && c.CookieReceived > 0).Select(c => c.Id);
        }

        public virtual void Save()
        {

            var db = Registry.Instance;
            var id = this.Id;

            if (this.LastReceived == DateTime.MinValue)
                this.LastReceived = null;
            if (this.LastSend == DateTime.MinValue)
                this.LastSend = null;
            if (this.LastYoshiBribe == DateTime.MinValue)
                this.LastYoshiBribe = null;

            if (this.CookieReceived > this.TopCookieCount)
                this.TopCookieCount = this.CookieReceived;


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
