using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Business.Dao;
using System.Configuration;
using System.Linq;
using System.Data.Linq;

namespace UpdateDBApplication
{
    public class DBRepository
    {
        private static ILog logger = null;
        private static string TRIM_FUEL = ConfigurationManager.ConnectionStrings["TrimFuel"].ConnectionString;

        public DBRepository()
        {
            logger = LogManager.GetLogger(typeof(DBRepository));
            log4net.Config.XmlConfigurator.Configure();
        }

        public void RunScript(string command, string connectionString)
        {
            logger.Info("---------- Start script for " + connectionString + " ----------");
            IDao dao = new MySqlDao(connectionString);
            try
            {
                MySqlCommand q = new MySqlCommand(command);
                dao.ExecuteNonQuery(q);
                logger.Info("Success");
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            logger.Info("---------- End script" + " ----------");
        }

        public List<TPClient> GetClientList()
        {
            List<TPClient> res = null;
            try
            {
                IDao dao = new MySqlDao(TRIM_FUEL);
                MySqlCommand q = new MySqlCommand("SELECT * FROM TPClient");
                res = dao.Load<TPClient>(q).ToList();
            }
            catch (Exception ex)
            {
                res = new List<TPClient>();
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public string GetConnectionStringForClient(int? clientID)
        {
            IDao dao = new MySqlDao(TRIM_FUEL);
            string res = null;
            try
            {
                res = dao.Load<TPClient>(clientID).ConnectionString;
            }
            catch (Exception ex)
            {
                res = null;
                logger.Error(GetType(), ex);
            }

            return res;
        }
    }
}
