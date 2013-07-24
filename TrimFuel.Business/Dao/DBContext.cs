using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Dao
{
    public class DBContext : IDisposable
    {
        private string oldConnectionString;

        private DBContext(string connectionString)
        {
            oldConnectionString = Config.Current.CURRENT_CONNECTION_STRING;
            Config.Current.CURRENT_CONNECTION_STRING = connectionString;
        }
        public static DBContext UseConext(string connectionString)
        {
            return new DBContext(connectionString);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Config.Current.CURRENT_CONNECTION_STRING = oldConnectionString;
        }

        #endregion
    }
}
