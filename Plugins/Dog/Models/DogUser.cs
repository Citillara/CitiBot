using CitiBot.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Plugins.Dog.Models
{
    [Table("t_dog_users")]
    public class DogUser
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual int BonesReceived { get; set; }
        public virtual int TopBonesCount { get; set; }
        public virtual DateTime? LastReceived { get; set; }
        public virtual string Username { get; set; }
        public virtual string Channel { get; set; }

        public static DogUser GetUser(string channel, string username)
        {
            return Registry.Instance.DogUsers.Where(c => c.Channel == channel && c.Username == username).FirstOrDefault();
        }

        public static DogUser GetUser(int id)
        {
            return Registry.Instance.DogUsers.Where(c => c.Id == id).FirstOrDefault();
        }


        public static int GetUserRankingInChannel(string channel, string username)
        {
            var sorted = from db in Registry.Instance.DogUsers
                         orderby db.BonesReceived descending
                         where db.Channel == channel
                         select new
                         {
                             Username = db.Username,
                             Rank = (from dbb in Registry.Instance.DogUsers
                                     where dbb.BonesReceived > db.BonesReceived
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
            return Registry.Instance.DogUsers.Count(c => c.Channel == channel);
        }
        public static IEnumerable<DogUser> GetChannelTopUsers(string channel, int count)
        {
            return Registry.Instance.DogUsers.Where(c => c.Channel == channel).OrderByDescending(c => c.BonesReceived).Take(count);
        }
        public static IEnumerable<Int32> GetChannelUserIdsWithCookies(string channel)
        {
            return Registry.Instance.DogUsers.Where(c => c.Channel == channel && c.BonesReceived > 0).Select(c => c.Id);
        }

        public virtual void Save()
        {

            var db = Registry.Instance;
            var id = this.Id;

            if (this.LastReceived == DateTime.MinValue)
                this.LastReceived = null;

            if (this.BonesReceived > this.TopBonesCount)
                this.TopBonesCount = this.BonesReceived;


            if (db.DogUsers.Any(e => e.Id == id))
            {
                db.Set<DogUser>().Attach(this);
                db.Entry<DogUser>(this).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.DogUsers.Add(this);
                db.Entry<DogUser>(this).State = System.Data.Entity.EntityState.Added;
            }

            db.SaveChanges();
        }

    }
}
