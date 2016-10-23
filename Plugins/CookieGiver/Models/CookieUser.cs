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
        [Column("LastReceived")]
        protected virtual DateTime? LastReceivedDB { get; set; }
        [Column("LastYoshiBribe")]
        protected virtual DateTime? LastYoshiBribeDB { get; set; }
        [Column("LastSend")]
        protected virtual DateTime? LastSendDB { get; set; }
        [Column("LastSteal")]
        protected virtual DateTime? LastStealDB { get; set; }
        public virtual string Username { get; set; }
        public virtual string Channel { get; set; }

        private string m_displayName;
        [NotMapped]
        public string DisplayName
        {
            get
            {
                if (m_displayName == null)
                {
                    var t = TwitchUser.GetUser(Username);
                    if (t != null)
                        m_displayName = t.DisplayName ?? ToolBox.FirstLetterToUpper(Username);
                    else
                        m_displayName = ToolBox.FirstLetterToUpper(Username);
                }
                return m_displayName;
            }
            set
            {
                m_displayName = value;
            }
        }

        [NotMapped]
        public DateTime LastReceived
        {
            get { return LastReceivedDB.HasValue ? LastReceivedDB.Value : DateTime.MinValue; }
            set { LastReceivedDB = value; }
        }
        [NotMapped]
        public DateTime LastYoshiBribe
        {
            get { return LastYoshiBribeDB.HasValue ? LastYoshiBribeDB.Value : DateTime.MinValue; }
            set { LastYoshiBribeDB = value; }
        }
        [NotMapped]
        public DateTime LastSend
        {
            get { return LastSendDB.HasValue ? LastSendDB.Value : DateTime.MinValue; }
            set { LastSendDB = value; }
        }
        [NotMapped]
        public DateTime LastSteal
        {
            get { return LastStealDB.HasValue ? LastStealDB.Value : DateTime.MinValue; }
            set { LastStealDB = value; }
        }

        protected CookieUser()
        {
        }

        protected static CookieUser New(string channel, string username)
        {
            return new CookieUser()
            {
                Channel = channel,
                Username = username
            };
        }

        public static CookieUser Clone(CookieUser user, string displayName = null)
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
            var val = Registry.Instance.CookieUsers.Where(c => c.Channel == channel && c.Username == username).FirstOrDefault();
            if (val == null)
                val = New(channel, username);
            return val;
        }
        public static CookieUser GetUser(int id)
        {
            var val = Registry.Instance.CookieUsers.Where(c => c.Id == id).FirstOrDefault();
            return val;
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
            take.ToList().ForEach(e => list.Add(Clone(e.db, e.subtu == null ? null : e.subtu.DisplayName)));
            return list;
        }
        public static IEnumerable<Int32> GetChannelUserIdsWithCookies(string channel)
        {
            return Registry.Instance.CookieUsers.Where(c => c.Channel == channel && c.CookieReceived > 0).Select(c => c.Id);
        }

        public static IEnumerable<Int32> GetChannelUserIdsWithExclusion(string channel, IEnumerable<Int32> excludeList)
        {
            return Registry.Instance.CookieUsers.Where(c => c.Channel == channel && !excludeList.Contains(c.Id)).Select(c => c.Id);
        }

        public virtual void Save()
        {

            var db = Registry.Instance;
            var id = this.Id;

            if (this.LastReceivedDB == DateTime.MinValue)
                this.LastReceivedDB = null;
            if (this.LastSendDB == DateTime.MinValue)
                this.LastSendDB = null;
            if (this.LastYoshiBribeDB == DateTime.MinValue)
                this.LastYoshiBribeDB = null;
            if (this.LastStealDB == DateTime.MinValue)
                this.LastStealDB = null;

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
