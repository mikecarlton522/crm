using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;
using log4net;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;

namespace EmailBodyCompress
{
    class Program
    {
        static string _connectionString = ConfigurationManager.ConnectionStrings["TrimFuel"].ConnectionString;
        static ILog logger = null;

        static void Main(string[] args)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                log4net.ThreadContext.Properties["ApplicationID"] = "Compress";
                logger = LogManager.GetLogger(typeof(Program));

                IDao stoDao = new MySqlDao(_connectionString);
                MySqlCommand q = new MySqlCommand(@"Select * from Email where Body not like 'ZIP+BASE64%' limit 50000");
                var emails = stoDao.Load<Email>(q);
                foreach (var email in emails)
                    stoDao.Save<Email>(email);
            }
            catch (Exception ex)
            {
                logger.Error(typeof(Program), ex);
            }
        }
    }
}
