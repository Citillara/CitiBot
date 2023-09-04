using CitiBot.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Plugins.Moderation.Models
{
    /*[Table("t_moderation_blacklist_item")]
    public class ModerationChannelSetting : BaseModel<ModerationBlacklistItem>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Channel { get; set; }
        public virtual string Setting { get; set; }
        public virtual string Value { get; set; }

        private bool isNew = false;

        private ModerationChannelSetting()
        {

        }

        public static IEnumerable<ModerationChannelSetting> GetChannelItems(string channel)
        {
            return Registry.Instance.ModerationChannelSettings.Where(c => c.Channel == channel).ToList();
        }

        public static ModerationChannelSetting GetItem(int id)
        {
            return Registry.Instance.ModerationChannelSettings.Where(c => c.Id == id).FirstOrDefault();
        }

        public static void AddNewModerationBlacklistItem(string channel, string setting, string value)
        {
            var c = new ModerationChannelSetting()
            {
                Channel = channel,
                Setting = setting,
                Value = value,
                isNew = true
            };

            c.Save();
        }

        private void Save()
        {
            this.Save(isNew);
        }

        public enum ModerationBlacklistItemState
        {
            Active = 0,
            Deleted = 1
        }
    }*/
}
