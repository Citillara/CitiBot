using CitiBot.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Main.Models
{

    [Table("t_bots_plugins")]
    public class BotPlugin : BaseModel<BotPlugin>
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual string PluginName { get; set; }
        public virtual BotSettings BotSettings { get; set; }

        private bool isNew = false;

        public virtual void Save()
        {
            this.Save(isNew);
        }

    }
}
