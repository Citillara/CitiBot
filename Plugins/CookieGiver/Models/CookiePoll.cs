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
    [Table("t_cookie_polls")]
    public class CookiePoll : BaseModel<CookiePoll>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual CookieChannel Channel { get; set; }
        public virtual CookiePollState Status { get; set; }
        public virtual int Duration { get; set; }
        public virtual string Title { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual DateTime CreationTime { get; set; }

        public virtual ICollection<CookiePollOption> PollOptions { get; set; }

        private bool isNew = false;

        public static CookiePoll GetLastestPoll(string channel)
        {
            var r = Registry.Instance.CookiePolls
                .Where(d => d.Channel.Channel == channel)
                .OrderByDescending(d => d.CreationTime).FirstOrDefault();
            return r;
        }

        public static CookiePoll CreateNewPoll(string channel)
        {
            // First close all existing polls for that channel
            var running_polls = Registry.Instance.CookiePolls
                .Where(p => p.Status == CookiePollState.Initial || p.Status == CookiePollState.Running);
            foreach (var poll in running_polls)
            {
                poll.Status = CookiePollState.Deleted;
                poll.Save();
            }

            return new CookiePoll()
            {
                Channel = CookieChannel.GetChannel(channel),
                CreationTime = DateTime.Now,
                StartTime = DateTime.Now,
                Duration = 300,
                Status = CookiePollState.Initial,
                Title = "",
                PollOptions = new List<CookiePollOption>(),
                isNew = true
            };
        }

        public void SetTitle(string title)
        {
            this.Title = title;
            Save();
        }

        public bool Start()
        {
            if(this.Status == CookiePollState.Initial && this.PollOptions != null && this.PollOptions.Count > 0)
            {
                this.Status = CookiePollState.Running;
                this.StartTime = DateTime.Now;
                Save();
                return true;
            }
            return false;
        }


        public void AddOption(string text)
        {
            if (this.PollOptions == null)
                this.PollOptions = new List<CookiePollOption>();

            var o = new CookiePollOption() { Poll = this,  Order= this.PollOptions.Count(), Text = text, Votes = 0};
            o.SetIsNew();

            this.PollOptions.Add(o);

            o.Save();
            this.Save();
        }

        public void ClosePoll()
        {
            this.Status = CookiePollState.Finished;
            this.Save();
        }

        public bool AddVotesToOption(int amount, int option)
        {
            // Notes : options are 0 based index
            if (this.Status == CookiePollState.Running  && this.PollOptions.Count >= option)
            {
                var p = this.PollOptions.OrderBy(pp => pp.Order).ElementAt(option - 1);
                p.Votes += amount;
                p.Save();
                return true;
            }
            else
            {
                return false;
            }
        }

        protected virtual void Save()
        {
            this.Save(isNew);
            if (isNew)
                isNew = false;
        }


        public enum CookiePollState
        {
            Initial = 0,
            Running = 1,
            Finished = 2,
            Deleted = 3
        }
    }
}
