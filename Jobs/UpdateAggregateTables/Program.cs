using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using log4net;
using TrimFuel.Model;
using TrimFuel.Business.Dao;
using TrimFuel.Business;
using System.Configuration;
using TrimFuel.Model.Views;

namespace UpdateAggregateTables
{
    class Program
    {

        static string _stoConnectionString = ConfigurationManager.ConnectionStrings["TrimFuel"].ConnectionString;
        static DateTime ENDDATE = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).AddDays(0);
        static DateTime STARTDATE = ENDDATE.AddDays(-5);

        static ILog logger = null;
        static IDao dao = null;
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            log4net.ThreadContext.Properties["ApplicationID"] = "All clients";
            logger = LogManager.GetLogger(typeof(Program));

            ProcessSTO();
            ProcessClients();
        }

        static void ProcessSTO()
        {
            // STO
            IDao dao = new MySqlDao(_stoConnectionString);
            PerformUpdate(dao, STARTDATE, ENDDATE, "Triangle (STO)");
        }

        static void ProcessClients()
        {
            IDao dao = new MySqlDao(_stoConnectionString);
            MySqlCommand q = new MySqlCommand("SELECT * FROM TPClient");
            var clients = dao.Load<TPClient>(q);
            foreach (var client in clients)
            {
                // create Client DAO
                dao = new MySqlDao(client.ConnectionString);
                PerformUpdate(dao, STARTDATE, ENDDATE, client.Name);
            }
        }

        static void PerformUpdate(IDao dao, DateTime startDate, DateTime endDate, string clientName)
        {
            logger.Info(string.Format("  Process Client {0}  ", clientName));

            try
            {
                // AggShipperSale
                MySqlCommand q = new MySqlCommand(@"insert into AggShipperSale
                select * from
                (select SaleID, 2, CreateDT from TSNRecord where RegID is not null and Completed = 1
                union 
                select SaleID, 4, CreateDT from ABFRecord where RegID is not null and Completed = 1
                union 
                select SaleID, 10, CreateDT from TFRecord where RegID is not null and Completed = 1 
                union 
                select SaleID, 6, CreateDT from ALFRecord where RegID is not null and Completed = 1 
                union 
                select SaleID, 7, CreateDT from GFRecord where RegID is not null and Completed = 1 
                union 
                select SaleID, 8, CreateDT from NPFRecord where RegID is not null and Completed = 1
                union 
                select SaleID, 5, CreateDT from KeymailRecord where RegID is not null and Completed = 1) t
                where SaleID not in (select SaleID from AggShipperSale)");
                q.Parameters.Add("@StartDate", MySqlDbType.Timestamp).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.Timestamp).Value = endDate;
                dao.ExecuteNonQuery(q);

                // AggBillingSale
                q = new MySqlCommand(@"insert into AggBillingSale
                select * from
                (select etss.SaleID, b.BillingID, bs.BillingSubscriptionID, 5, bs.CreateDT
                from BillingSubscription bs 
                join Billing b on bs.BillingID = b.BillingID 
                join ExtraTrialShip ets on ets.BillingID = b.BillingID 
                join ExtraTrialShipSale etss on etss.ExtraTrialShipID = ets.ExtraTrialShipID 
                union all 
                select sl.SaleID, b.BillingID, bs.BillingSubscriptionID, 3, bs.CreateDT
                from BillingSubscription bs
                join Billing b on bs.BillingID = b.BillingID 
                join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID 
                join UpsellSale sl on sl.ChargeHistoryID = ch.ChargeHistoryID
                union all 
                select sl.SaleID, b.BillingID, bs.BillingSubscriptionID, 1, bs.CreateDT
                from BillingSubscription bs 
                join Billing b on bs.BillingID = b.BillingID 
                join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID
                join BillingSale sl on sl.ChargeHistoryID = ch.ChargeHistoryID) t
                where SaleID not in (select SaleID from AggBillingSale)");
                q.Parameters.Add("@StartDate", MySqlDbType.Timestamp).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.Timestamp).Value = endDate;
                dao.ExecuteNonQuery(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
