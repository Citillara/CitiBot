using CitiBot.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Plugins.CookieGiver.Models
{
    [Table("t_cookie_channels")]
    public class CookieChannel : BaseModel<CookieChannel>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string Channel { get; set; }
        public virtual int CookieDelay { get; set; }
        public virtual int BribeDelay { get; set; }
        public virtual int StealDelay { get; set; }
        public virtual int CookieCheers { get; set; }
        public virtual string SubGreetings { get; set; }
        public virtual CookieChannelStates Status { get; set; }

        public virtual ICollection<CookiePoll> Polls { get; set; }

        private bool isNew = false;

        public static CookieChannel GetChannel(string channel)
        {
            var r = Registry.Instance.CookieChannels.Where(d => d.Channel == channel).FirstOrDefault();
            if (r == null)
                r = new CookieChannel()
                {
                    Channel = channel,
                    isNew = true,
                    BribeDelay = 600,
                    CookieDelay = 300,
                    StealDelay = 600,
                    CookieCheers = 0,
                    Status = CookieChannelStates.Enabled
                };
            return r;
        }

        public virtual void Save()
        {
            this.Save(isNew);
            if (isNew)
                isNew = false;
        }

        public enum CookieChannelStates {
            Enabled = 0, Disabled = 1
        }
    }
}
