using CitiBot.Database;
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

        public virtual ICollection<CookieUser> CookieUsers { get; set; }

        [NotMapped]
        public string BusinessDisplayName { get { return DisplayName ?? Name; } }

        private bool isNew = false;

        private TwitchUser()
        {

        }

        public static TwitchUser New(string name, long? twitchId, string displayName = null)
        {
            return new TwitchUser()
            {
                isNew = true,
                Name = name,
                TwitchId = twitchId,
                DisplayName = displayName
            };
        }

        public static TwitchUser GetUser(string name)
        {
            return Registry.Instance.TwitchUsers.Where(u => u.Name == name).FirstOrDefault();
        }

        public static TwitchUser GetUser(int id)
        {
            return Registry.Instance.TwitchUsers.Where(c => c.Id == id).FirstOrDefault();
        }

        public static TwitchUser GetUserByTwitchId(int id)
        {
            return Registry.Instance.TwitchUsers.Where(c => c.TwitchId == id).FirstOrDefault();
        }

        public void CheckAndUpdate(string displayName, long? twitchId = null)
        {
            bool doSave = false;
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

        public virtual void Save()
        {
            this.Save(isNew);
            if (isNew)
                isNew = false;
        }
    }

}

