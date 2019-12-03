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

namespace CitiBot.Plugins.CookieGiver.Models
{
    [Table("t_cookie_flavours")]
    public class CookieFlavour : BaseModel<CookieFlavour>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual CookieFlavourState Status { get; set; }
        public virtual string Channel { get; set; }
        public virtual DateTime? AddedDate { get; set; }
        public virtual string AddedBy { get; set; }
        public virtual string Text { get; set; }

        private bool isNew = false;

        private CookieFlavour()
        {

        }

        public static IEnumerable<Int32> GetChannelCookies(string channel)
        {
            return Registry.Instance.CookieFlavours.Where(c => c.Channel == channel && c.Status == CookieFlavourState.Active).Select(c => c.Id);
        }

        public static IEnumerable<Int32> GetCommonCookies()
        {
            return Registry.Instance.CookieFlavours.Where(c => c.Channel == "all" && c.Status == CookieFlavourState.Active).Select(c => c.Id);
        }

        public static CookieFlavour GetCookie(int id)
        {
            return Registry.Instance.CookieFlavours.Where(c => c.Id == id).FirstOrDefault();
        }
        public static int GetCookieCount()
        {
            return Registry.Instance.CookieFlavours.Count();
        }

        public static void AddNewCookieFlavor(string channel, string text, string addedBy)
        {
            var c = new CookieFlavour()
            {
                Channel = channel,
                Text = text,
                Status = CookieFlavourState.Active,
                AddedBy = addedBy,
                AddedDate = DateTime.Now,
                isNew = true
            };

            c.Save();
        }

        public static void AddModifyNewCookieFlavor(string channel, string text, string addedBy)
        {
            var cookie = Registry.Instance.CookieFlavours.Where(c => c.Channel == channel 
            && c.Status == CookieFlavourState.Active).First();
            if (cookie != null)
            {
                cookie.Text = text;
                cookie.Save();
            }
            else
            {
                AddNewCookieFlavor(channel, text, addedBy);
            }
        }

        private void Save()
        {
            this.Save(isNew);
        }

        public enum CookieFlavourState
        {
            Active = 0,
            Deleted = 1
        }
    }
}
