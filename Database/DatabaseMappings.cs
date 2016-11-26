using CitiBot.Main.Models;
using CitiBot.Plugins.CookieGiver.Models;
using CitiBot.Plugins.Dog.Models;
using CitiBot.Plugins.Twitch.Models;
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

        public DbSet<DogUser> DogUsers { get; set; }

        public DbSet<TwitchUser> TwitchUsers { get; set; }

        public DbSet<BotSettings> BotSettings { get; set; }
        public DbSet<BotPlugin> BotPlugins { get; set; }
        public DbSet<BotChannel> BotChannels { get; set; }

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

        }
    }
}
