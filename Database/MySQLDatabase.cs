using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Database
{

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public partial class Registry : DbContext
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;

        public Registry()
            : base(ConnectionString)
        {
            //this.Database.Log = Console.WriteLine;
        }


        public Registry(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            
        }

        private static Registry m_instance;
        public static Registry Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new Registry();
                return m_instance;
            }
        }

    }
}
