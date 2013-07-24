using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using log4net;

namespace EmailRestore
{
    class Program
    {
        static string _stoConnectionString = ConfigurationManager.ConnectionStrings["TrimFuel"].ConnectionString;
        static string _restoreConnectionString = ConfigurationManager.ConnectionStrings["Restore"].ConnectionString;
        static ILog logger = null;
        private const string INSERT_COMMAND = "INSERT INTO Email(EmailID, DynamicEmailID, Email, Subject, Body, Response, CreateDT) VALUES(@EmailID, @DynamicEmailID, @Email, @Subject, @Body, @Response, @CreateDT);";


        static void Main(string[] args)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                log4net.ThreadContext.Properties["ApplicationID"] = "Restore";
                logger = LogManager.GetLogger(typeof(Program));

                IDao stoDao = new MySqlDao(_stoConnectionString);
                IDao restoreDao = new MySqlDao(_restoreConnectionString);
                MySqlCommand q = new MySqlCommand(@"Select * from Email where EmailID >= 1090000");
                var emails = restoreDao.Load<Email>(q);
                foreach (var email in emails)
                {
                    q = new MySqlCommand(INSERT_COMMAND);
                    q.Parameters.Add("@DynamicEmailID", MySqlDbType.Int32).Value = email.DynamicEmailID;
                    q.Parameters.Add("@EmailID", MySqlDbType.Int32).Value = email.EmailID;
                    q.Parameters.Add("@Email", MySqlDbType.VarChar).Value = email.Email_;
                    q.Parameters.Add("@Subject", MySqlDbType.VarChar).Value = email.Subject;
                    q.Parameters.Add("@Body", MySqlDbType.Text).Value = email.Body;
                    q.Parameters.Add("@Response", MySqlDbType.Text).Value = email.Response;
                    q.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = email.CreateDT;
                    stoDao.ExecuteNonQuery(q);
                }
            }
            catch (Exception ex)
            {
                logger.Error(typeof(Program), ex);
            }
        }
    }
}
