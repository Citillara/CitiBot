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
    [Table("t_cookie_polls_options")]
    public class CookiePollOption : BaseModel<CookiePollOption>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual CookiePoll Poll { get; set; }
        public virtual string Text { get; set; }
        public virtual int Votes { get; set; }
        public virtual int Order { get; set; }

        private bool isNew = false;

        public void SetIsNew()
        {
            this.isNew = true;
        }

        public virtual void Save()
        {
            this.Save(isNew);
            if (isNew)
                isNew = false;
        }
    }
}
