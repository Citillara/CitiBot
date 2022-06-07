using CitiBot.Database;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Main.Models
{
    [Table("t_settings")]
    public class GlobalSetting : BaseModel<GlobalSetting>
    {
        private static Dictionary<string, string> _SettingsCache = new Dictionary<string, string>();
        private static DateTime _NextUpdateTime = DateTime.MinValue;

        public static TimeSpan _CacheDuration = new TimeSpan(0, 5, 0);

        [Key]
        public virtual string Name { get; set; }
        public virtual string Value { get; set; }

        public static string GetSettingValue(string name)
        {
            if(DateTime.Now > _NextUpdateTime)
            {
                RefreshCache();
            }

            return _SettingsCache[name];
        }

        private static void RefreshCache()
        {
            using (Registry registry = new Registry())
            {
                _SettingsCache.Clear();
                registry.GlobalSettings.ToList().ForEach(setting => _SettingsCache.Add(setting.Name, setting.Value));
                _NextUpdateTime = DateTime.Now.Add(_CacheDuration);
            }
        }

        public static void ClearCache()
        {
            _NextUpdateTime = DateTime.MinValue;
        }
    }
}
