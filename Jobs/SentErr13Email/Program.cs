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

namespace SentErr13Email
{
    class Program
    {
        private static string FROM_ADDRESS = "donotreply@ecigsbrand.com";
        private static string FROM_NAME = "E-Cigs Electronic Cigarette";
        private static string SUBJECT = "A Message from Customer Care";

        static void Main(string[] args)
        {
            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                IEmailGateway emailGateway = new DefaultEmailGateway();

                MySqlCommand q = new MySqlCommand("select distinct b.* from ABFRecord abf " +
                    "inner join TSNRecord tsn on tsn.SaleID = abf.SaleID " +
                    "inner join BillingSale bsale on bsale.SaleID = abf.SaleID " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = bsale.BillingSubscriptionID " +
                    "inner join Subscription s on s.SubscriptionID = bs.SubscriptionID " +
                    "inner join Billing b on b.BillingID = bs.BillingID " + 
                    "where s.ProductID = 10 " +
                    "limit 100");
                IList<Billing> billingList = dao.Load<Billing>(q);

                string body = Utility.LoadFromEmbeddedResource(typeof(Program), "EmailBody.txt");

                Console.WriteLine("BEGIN PROCESSING");
                foreach (Billing b in billingList)
                {
                    try 
	                {	        
                        Console.WriteLine("BillingID (" + b.BillingID.ToString() + ")");
                		emailGateway.SendEmail(FROM_NAME, FROM_ADDRESS, b.FullName, b.Email, SUBJECT, body.Replace("##FNAME##", b.FirstName));
                        Console.WriteLine("SUCCESS");
	                }
	                catch(Exception ex)
                    { 
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("ERROR");
                    }                    
                }
                Console.WriteLine("END PROCESSING");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
