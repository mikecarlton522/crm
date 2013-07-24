using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;

namespace STO_UpdateAragonLeads_VerificationDate
{
    class Program
    {
        static ILog logger = null;
        static IDao dao = null;
        static void Main(string[] args)
        {
            logger = LogManager.GetLogger(typeof(Program));
            log4net.Config.XmlConfigurator.Configure();
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            try
            {
                MySqlCommand q = new MySqlCommand("select * from AragonLead");
                IList<AragonLead> leads = dao.Load<AragonLead>(q);
                foreach (AragonLead ld in leads)
                {
                    try
                    {
                        dao.BeginTransaction();

                        string[] fields = ld.Response.Split(new string[] { "\",\"" }, StringSplitOptions.None);
                        DateTime? verificationDate = GetDateTime(fields[4]);

                        ld.VerificationDT = verificationDate;
                        dao.Save<AragonLead>(ld);

                        dao.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        dao.RollbackTransaction();
                        logger.Error("Error", ex); ;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Error", ex); ;
            }
        }

        private static string GetString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value.Trim('"');
        }


        private static DateTime GetDateTime(string value)
        {
            string dateTime = GetString(value);
            DateTime res = DateTime.Today;
            if (!string.IsNullOrEmpty(dateTime))
            {
                DateTime.TryParse(dateTime, out res);
            }
            return res;
        }
    }
}
