using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Gateways.DefaultEmail;
using TrimFuel.Business.Utils;
using log4net;

namespace STO_InvalidCallNumberEmail
{
    class Program
    {
        private static string FROM_ADDRESS = "do_not_reply@ecigsbrand.com";
        private static string FROM_NAME = "E-Cigs Electronic Cigarette";
        private static string SUBJECT = "Regarding your recent Electronic Cigarette purchase";

        private static int INVALID_CALL_NUMBER_MID_ID = 38;

        static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger(typeof(Program));
            log4net.Config.XmlConfigurator.Configure();
            IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

            try
            {
                IEmailGateway emailGateway = new DefaultEmailGateway();

                //TODO
                MySqlCommand q = new MySqlCommand(
                    "select distinct b.* from Billing b " +
                    "inner join BillingSubscription bs on bs.BillingID = b.BillingID " +
                    "inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID " +
                    "where ch.MerchantAccountID = " + INVALID_CALL_NUMBER_MID_ID + " and ch.Success = 1 and ch.Amount > 0 " +
                    "limit 1000"
                    );
                IList<Billing> billingList = dao.Load<Billing>(q);

                string body = Utility.LoadFromEmbeddedResource(typeof(Program), "EmailBody.txt");

                foreach (Billing b in billingList)
                {
                    try
                    {
                        //TODO
                        logger.Info("BillingID (" + b.BillingID.ToString() + ")");
                        emailGateway.SendEmail(FROM_NAME, FROM_ADDRESS, b.FullName, b.Email, SUBJECT, body.Replace("##FNAME##", b.FirstName));
                        logger.Info("SUCCESS");
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Fatal Error", ex);
            }
        }
    }
}
