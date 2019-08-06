﻿using CitiBot.Database;
using CitiBot.Plugins.CookieGiver.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Plugins.Twitch.Models
{

    [DataContract]
    [Table("t_twitch_users")]
    public class TwitchUser : BaseModel<TwitchUser>
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual long? TwitchId { get; set; }
        public virtual string Name { get; set; }
        public virtual string DisplayName { get; set; }
        public virtual long? FFZId { get; set; }
        public virtual long? TwitchIconId { get; set; }

        public virtual ICollection<CookieUser> CookieUsers { get; set; }

        [NotMapped]
        public string BusinessDisplayName { get { return DisplayName ?? Name; } }

        private bool isNew = false;

        private TwitchUser()
        {

        }

        private static TwitchUser New(string name, long? twitchId, string displayName = null)
        {
            return new TwitchUser()
            {
                isNew = true,
                Name = name,
                TwitchId = twitchId,
                DisplayName = displayName
            };
        }

        public static TwitchUser GetOrCreateUser(long? twitchId, string username, string displayName)
        {
            TwitchUser val = null;
            if (twitchId.HasValue)
            {
                val = Registry.Instance.TwitchUsers.Where(t => t.TwitchId == twitchId).FirstOrDefault();
            }
            if (val == null && username != null)
            {
                val = Registry.Instance.TwitchUsers.Where(t => t.Name == username).FirstOrDefault();
            }

            if (val == null && displayName != null)
            {
                val = Registry.Instance.TwitchUsers.Where(t => t.DisplayName == displayName).FirstOrDefault();
            }
            if (val == null && displayName != null)
            {
                string lower = displayName.ToLowerInvariant();
                val = Registry.Instance.TwitchUsers.Where(t => t.Name == lower).FirstOrDefault();
            }

            if (val == null)
            {
                val = New(username, twitchId, displayName);
                val.Save();
            }
            else
            {
                val.CheckAndUpdate(username, displayName, twitchId);
            }
            return val;
        }

        public static TwitchUser GetUser(int id)
        {
            return Registry.Instance.TwitchUsers.Where(c => c.Id == id).FirstOrDefault();
        }

        public static TwitchUser GetUserByTwitchId(int id)
        {
            return Registry.Instance.TwitchUsers.Where(c => c.TwitchId == id).FirstOrDefault();
        }

        private void CheckAndUpdate(string username, string displayName, long? twitchId = null)
        {
            bool doSave = false;
            // Check if another user exists with same username
            if (twitchId.HasValue && twitchId.Value != 0)
            {
                var other = Registry.Instance.TwitchUsers.Where(c => c.Name == username && c.Id != Id).FirstOrDefault();
                if (other != null && !other.TwitchId.HasValue && other.TwitchId != 0)
                {
                    other.Delete();
                }
            }
            if (!string.IsNullOrEmpty(username) && !Name.Equals(username))
            {
                doSave = true;
                Name = username;
            }
            if (!string.IsNullOrEmpty(displayName) && !BusinessDisplayName.Equals(displayName))
            {
                doSave = true;
                DisplayName = displayName;
            }
            if (twitchId != null && TwitchId != twitchId)
            {
                doSave = true;
                TwitchId = twitchId;
            }
            if(doSave)
                Save();
        }

        public override void Delete()
        {
            foreach (var c in CookieUsers)
                c.Delete();
            base.Delete();
        }
        public virtual void Save()
        {
            base.Save(isNew);
            if (isNew)
                isNew = false;
        }
    }

}

