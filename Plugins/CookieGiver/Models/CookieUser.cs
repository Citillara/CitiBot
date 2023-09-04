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
        public virtual int CookiesSent { get; set; }
        public virtual int CookiesLostToYoshi { get; set; }
        public virtual int CookiesReceivedByOthers { get; set; }
        public virtual int CookiesGenerated { get; set; }
        public virtual int CookiesGivenToYoshi { get; set; }
        public virtual int CookiesDestroyedByYoshi { get; set; }


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
                TwitchUser = twitchUser,
                LastReceived = new DateTime(2000, 1, 1, 1, 1, 1),
                LastYoshiBribe = new DateTime(2000, 1, 1, 1, 1, 1),
                LastSend = new DateTime(2000, 1, 1, 1, 1, 1),
                LastSteal = new DateTime(2000, 1, 1, 1, 1, 1),
            };
        }


        public static CookieUser GetUserByDisplayName(string channel, string name)
        {
            CookieUser val = null;
            if (val == null && name != null)
            {
                val = FirstOrDefaultLocal("TwitchUser", c => c.Channel == channel && c.TwitchUser.DisplayName == name);
            }
            if (val == null && name != null)
            {
                string lower = name.ToLowerInvariant();
                val = FirstOrDefaultLocal("TwitchUser", c => c.Channel == channel && c.TwitchUser.Name == lower);
            }
            return val;
        }

        public static CookieUser GetUser(string channel, string username, long? twitchId = null, string displayName = null)
        {
            CookieUser val = null;

            string cleanedDisplayName = displayName?.Replace("@", "");
            if (twitchId != null)
            {
                val = FirstOrDefaultLocal("TwitchUser", c => c.Channel == channel && c.TwitchUser.TwitchId == twitchId);
            }
            if (val == null && username != null)
            {
                val = FirstOrDefaultLocal("TwitchUser", c => c.Channel == channel && c.TwitchUser.Name == username);
            }
            if (val == null && cleanedDisplayName != null)
            {
                val = FirstOrDefaultLocal("TwitchUser", c => c.Channel == channel && c.TwitchUser.DisplayName == cleanedDisplayName);
            }
            if (val == null && cleanedDisplayName != null)
            {
                string lower = cleanedDisplayName.ToLowerInvariant();
                val = FirstOrDefaultLocal("TwitchUser", c => c.Channel == channel && c.TwitchUser.Name == lower);
            }
            if (val == null)
            {
                val = New(channel, username, twitchId, cleanedDisplayName);
            }
            return val;
        }
        public static CookieUser GetUser(int id)
        {
            var val = Registry.Instance.CookieUsers.Find(id);
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

        private static readonly DateTime MIN_DT = new DateTime(2000, 01, 01);

        public virtual void Save()
        {
            if (this.CookieReceived > this.TopCookieCount)
                this.TopCookieCount = this.CookieReceived;

            if (this.LastReceived < MIN_DT)
                this.LastReceived = MIN_DT;
            
            if (this.LastYoshiBribe < MIN_DT)
                this.LastYoshiBribe = MIN_DT;

            if (this.LastSend < MIN_DT)
                this.LastSend = MIN_DT;

            if (this.LastSteal < MIN_DT)
                this.LastSteal = MIN_DT;

            this.Save(isNew);
        }

        public override void Delete()
        {
            base.Delete();
        }

        public class LightCookieUser
        {
            public int CookieReceived { get; set; }
            public string DisplayName { get; set; }
        }

    }
}
