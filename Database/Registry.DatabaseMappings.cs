using CitiBot.Main.Models;
using CitiBot.Plugins.CookieGiver.Models;
using CitiBot.Plugins.Counters.Models;
using CitiBot.Plugins.Twitch.Models;
using CitiBot.Plugins.Moderation.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Database
{
    public partial class Registry
    {
        public DbSet<CookieUser> CookieUsers { get; set; }
        public DbSet<CookieFlavour> CookieFlavours { get; set; }
        public DbSet<CaloriesPerActivity> CaloriesPerActivity { get; set; }
        public DbSet<CookieChannel> CookieChannels { get; set; }
        public DbSet<CookiePoll> CookiePolls { get; set; }
        public DbSet<CookiePollOption> CookiePollOptions { get; set; }
        public DbSet<Counter> Counters { get; set; }

        public DbSet<TwitchUser> TwitchUsers { get; set; }

        public DbSet<BotSettings> BotSettings { get; set; }
        public DbSet<BotPlugin> BotPlugins { get; set; }
        public DbSet<BotChannel> BotChannels { get; set; }

        public DbSet<Log> Logs { get; set; }
        public DbSet<GlobalSetting> GlobalSettings { get; set; }

        public DbSet<ModerationBlacklistItem> ModerationBlacklistItems { get; set; }
        //public DbSet<ModerationChannelSetting> ModerationChannelSettings { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            modelBuilder.Entity<BotChannel>()
                .HasRequired(p => p.BotSettings)
                .WithMany(s => s.Channels)
                .Map(x => x.MapKey("BotId"));

            modelBuilder.Entity<BotPlugin>()
                .HasRequired(p => p.BotSettings)
                .WithMany(s => s.Plugins)
                .Map(x => x.MapKey("BotId"));

            modelBuilder.Entity<CookieUser>()
                .HasRequired(p => p.TwitchUser)
                .WithMany(s => s.CookieUsers)
                .Map(x => x.MapKey("TwitchUserId"));

            modelBuilder.Entity<CookiePoll>()
                .HasRequired(p => p.Channel)
                .WithMany(s => s.Polls)
                .Map(x => x.MapKey("CookieChannelId"));

            modelBuilder.Entity<CookiePollOption>()
                .HasRequired(p => p.Poll)
                .WithMany(s => s.PollOptions)
                .Map(x => x.MapKey("PollId"));

        }
    }
}
