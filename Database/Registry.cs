using MySql.Data.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiBot.Database
{

    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public partial class Registry : DbContext
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
        private bool m_disposed = false;

        public Registry()
            : base(ConnectionString)
        {
            this.Database.Log = Log;
            this.Configuration.UseDatabaseNullSemantics = true;
        }

        private void Log(string text)
        {
            Console.WriteLine(text);
            File.AppendAllText(@"C:\log.txt", text + Environment.NewLine);
        }

        public Registry(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            
        }

        public void Close()
        {
            m_disposed = true;
            this.Dispose();
        }
        
        [ThreadStatic]
        private static Registry m_instance;
        public static Registry Instance
        {
            get
            {
                if (m_instance == null || m_instance.m_disposed)
                {
                    m_instance = new Registry();
                }
                return m_instance;
            }
        }

    }
}
