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
    public class CookieUser : BaseModel<CookieUser>
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        //public virtual string Username { get; set; }
        public virtual string Channel { get; set; }
        public TwitchUser TwitchUser { get; set; }
        public virtual int CookieReceived { get; set; }
        public virtual int TopCookieCount { get; set; }
        public virtual DateTime LastReceived { get; set; }
        public virtual DateTime LastYoshiBribe { get; set; }
        public virtual DateTime LastSend { get; set; }
        public virtual DateTime LastSteal { get; set; }


        private bool isNew = false;

        private CookieUser()
        {
        }

        private static CookieUser New(string channel, string username, long? twitchId = null, string displayName = null)
        {
            var twitchUser = TwitchUser.GetOrCreateUser(twitchId, username, displayName);
            return new CookieUser()
            {
                isNew = true,
                Channel = channel,
                TwitchUser = twitchUser
            };
        }

        public static CookieUser GetUser(string channel, string username, long? twitchId = null, string displayName = null)
        {
            CookieUser val = null;
            if (twitchId != null)
            {
                val = Registry.Instance.CookieUsers.Include("TwitchUser")
                    .Where(c => c.Channel == channel && c.TwitchUser.TwitchId == twitchId)
                    .FirstOrDefault();
            }
            if (val == null && username != null)
            {
                val = Registry.Instance.CookieUsers.Include("TwitchUser")
                    .Where(c => c.Channel == channel && c.TwitchUser.Name == username)
                    .FirstOrDefault();
            }
            if (val == null && displayName != null)
            {
                val = Registry.Instance.CookieUsers.Include("TwitchUser")
                    .Where(c => c.Channel == channel && c.TwitchUser.DisplayName == displayName)
                    .FirstOrDefault();
            }
            if (val == null && displayName != null)
            {
                string lower = displayName.ToLowerInvariant();
                val = Registry.Instance.CookieUsers.Include("TwitchUser")
                    .Where(c => c.Channel == channel && c.TwitchUser.Name == lower)
                    .FirstOrDefault();
            }
            if (val == null)
            {
                val = New(channel, username, twitchId, displayName);
            }
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
                             Username = db.TwitchUser.Name,
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
        public static IEnumerable<LightCookieUser> GetChannelTopUsers(string channel, int count)
        {

            var result = from db in Registry.Instance.CookieUsers
                         where db.Channel == channel
                         orderby db.CookieReceived descending
                         select new LightCookieUser() { CookieReceived = db.CookieReceived, DisplayName = db.TwitchUser.DisplayName ?? db.TwitchUser.Name };
            var take = result.Take(count);
            return take;
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
            if (this.CookieReceived > this.TopCookieCount)
                this.TopCookieCount = this.CookieReceived;

            this.Save(isNew);
            if (isNew)
                isNew = false;
        }

        public class LightCookieUser
        {
            public int CookieReceived;
            public string DisplayName;
        }

    }
}
