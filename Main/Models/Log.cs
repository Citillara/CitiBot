using CitiBot.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CitiBot.Main.Models
{
    [Table("t_logs")]
    public class Log : BaseModel<Log>
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }
        public virtual DateTime LogDate { get; set; }
        public virtual LogLevel Level { get; set; }
        public virtual LogTypes Type { get; set; }
        public virtual string Criteria1 { get; set; }
        public virtual string Criteria2 { get; set; }
        public virtual string Data { get; set; }

        private bool isNew = false;

        public virtual void Save()
        {
            this.Save(isNew);
        }

        public static void AddTechnicalLog(DateTime date, LogLevel level, string criteria1, string criteria2, string data)
        {
            try
            {
                LogLevel i = (LogLevel)int.Parse(GlobalSetting.GetSettingValue("TechnicalLogLevel"));
                if (level < i)
                    return;
                using (Registry registry = new Registry())
                {
                    var n = new Log();
                    n.LogDate = date;
                    n.Level = level;
                    n.Type = LogTypes.Technical;
                    n.Criteria1 = criteria1;
                    n.Criteria2 = criteria2;
                    n.Data = data;
                    n.isNew = true;
                    n.Save();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void AddBusinessLog(DateTime date, LogLevel level, string criteria1, string criteria2, string data)
        {
            var n = new Log();
            n.LogDate = date;
            n.Level = level;
            n.Type = LogTypes.Business;
            n.Criteria1 = criteria1;
            n.Criteria2 = criteria2;
            n.Data = data;
            n.isNew = true;

            n.Save();
        }

        public static void AddBusinessLog(DateTime date, LogLevel level, string criteria1, string criteria2, string data, params object[] arg)
        {
            AddBusinessLog(date, level, criteria1, criteria2, string.Format(Thread.CurrentThread.CurrentCulture, data, arg));
        }

        public void Say(string destination, string format)
        {
        }

        public enum LogTypes
        {
            Technical = 0,
            Business = 1,
        }

        public enum LogLevel
        {
            Debug = 0,
            Verbose = 1,
            Info = 2,
            Warn = 3,
            Error = 4,
            Fatal = 5
        }
    }
}
