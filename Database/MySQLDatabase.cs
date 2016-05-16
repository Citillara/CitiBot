using CitiBot.Plugins.CookieGiver.Models;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot
{

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class Database : DbContext
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;

        public Database()
            : base(ConnectionString)
        {}


        public Database(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {}

        private static Database m_instance;
        public static Database Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Database();

                return m_instance;
            }
        }

        public DbSet<CookieUser> CookieUsers { get; set; }
        public DbSet<CookieFlavour> CookieFlavours { get; set; }
        public DbSet<CaloriesPerActivity> CaloriesPerActivity { get; set; }
    }
}
