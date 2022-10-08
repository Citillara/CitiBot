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
    [Table("t_moderation_blacklist_item")]
    public class ModerationBlacklistItem : BaseModel<ModerationBlacklistItem>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual ModerationBlacklistItemState Status { get; set; }
        public virtual string Channel { get; set; }
        public virtual DateTime? AddedDate { get; set; }
        public virtual string AddedBy { get; set; }
        public virtual string Text { get; set; }

        private bool isNew = false;

        private ModerationBlacklistItem()
        {

        }

        public static IEnumerable<ModerationBlacklistItem> GetChannelItems(string channel)
        {
            return Registry.Instance.ModerationBlacklistItems.Where(c => c.Channel == channel && c.Status == ModerationBlacklistItemState.Active).ToList();
        }

        public static IEnumerable<ModerationBlacklistItem> GetCommonItems()
        {
            return Registry.Instance.ModerationBlacklistItems.Where(c => c.Channel == "all" && c.Status == ModerationBlacklistItemState.Active).ToList();
        }

        public static IEnumerable<ModerationBlacklistItem> GetAllItems()
        {
            return Registry.Instance.ModerationBlacklistItems.Where(c => c.Status == ModerationBlacklistItemState.Active).ToList();
        }

        public static ModerationBlacklistItem GetItem(int id)
        {
            return Registry.Instance.ModerationBlacklistItems.Where(c => c.Id == id).FirstOrDefault();
        }
        public static int GetItemCount()
        {
            return Registry.Instance.CookieFlavours.Count();
        }

        public static void AddNewModerationBlacklistItem(string channel, string text, string addedBy)
        {
            var c = new ModerationBlacklistItem()
            {
                Channel = channel,
                Text = text,
                Status = ModerationBlacklistItemState.Active,
                AddedBy = addedBy,
                AddedDate = DateTime.Now,
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
    }
}
