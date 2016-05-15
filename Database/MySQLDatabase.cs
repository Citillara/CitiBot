using CitiBot.Plugins.CookieGiver.Models;
using MySql.Data.Entity;
using System;
using System.Collections.Generic;
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
        private static readonly string ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Database"].ConnectionString;

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

    }
}
