using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;
using TrimFuel.Business;

namespace InactiveUserReactivationBatchEmail
{
    class Program
    {
        private const int USER_COUNT = 1400;
        private const string TEST_EMAIL = "dtcoder@gmail.com";
        //private const string TEST_EMAIL = "rob.wertheim@gmail.com";
        private const string FROM = "donotreply@ecigsbrandoffer.com";

        static void Main(string[] args)
        {
            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

                BillingBatchEmailType billingBatchEmailType = dao.Load<BillingBatchEmailType>(BillingBatchEmailTypeEnum.InactiveUserReactivationEmail);
                if (billingBatchEmailType == null)
                {
                    throw new Exception(string.Format("BillingBatchEmailType({0}) was not found in DB", BillingBatchEmailTypeEnum.InactiveUserReactivationEmail));
                }

                DynamicEmail dynamicEmail = dao.Load<DynamicEmail>(billingBatchEmailType.DynamicEmailID);
                if (dynamicEmail == null)
                {
                    throw new Exception(string.Format("DynamicEmail({0}) was not found in DB", billingBatchEmailType.DynamicEmailID));
                }

                //TODO: hack from
                dynamicEmail.FromAddress = FROM;

                MySqlCommand q = new MySqlCommand("select bs.* from Billing b " + 
                    "inner join BillingSubscription bs on bs.BillingID = b.BillingID " +
                    "inner join Subscription s on s.SubscriptionID = bs.SubscriptionID " +
                    "left join BillingBatchEmail bbe on bbe.BillingID = b.BillingID and bbe.BillingBatchEmailTypeID = @InactiveUserReactivationEmail " +
                    "where bs.StatusTID = @Inactive and bbe.BillingBatchEmailID is null and s.ProductID = 10 " +
                    "order by b.BillingID desc " +
                    "limit @UserCount");
                q.Parameters.Add("@InactiveUserReactivationEmail", MySqlDbType.Int32).Value = BillingBatchEmailTypeEnum.InactiveUserReactivationEmail;
                q.Parameters.Add("@Inactive", MySqlDbType.Int32).Value = BillingSubscriptionStatusEnum.Inactive;
                q.Parameters.Add("@UserCount", MySqlDbType.Int32).Value = USER_COUNT;

                IList<BillingSubscription> billingSubscriptionList = dao.Load<BillingSubscription>(q);

                //TODO: hack for 500 oldest from 1900 most recent
                //billingSubscriptionList = billingSubscriptionList.Skip(1400).ToList();

                Console.WriteLine("BEGIN PROCESSING");
                foreach (BillingSubscription billingSubscription in billingSubscriptionList)
                {
                    SendBillingEmail(billingSubscription, dynamicEmail, billingBatchEmailType);
                }
                Console.WriteLine("END PROCESSING");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void SendBillingEmail(BillingSubscription billingSubscription, DynamicEmail dynamicEmail, BillingBatchEmailType billingBatchEmailType)
        {
            Console.WriteLine("--------------------------------------------------------");
            Console.WriteLine(string.Format("BillingID: {0}", billingSubscription.BillingID));

            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                EmailService emailService = new EmailService();

                Billing billing = dao.Load<Billing>(billingSubscription.BillingID);
                if (billing == null)
                {
                    throw new Exception(string.Format("Billing({0}) was not found in DB", billingSubscription.BillingID));
                }

                Subscription subscription = dao.Load<Subscription>(billingSubscription.SubscriptionID);
                if (subscription == null)
                {
                    throw new Exception(string.Format("Subscription({0}) was not found in DB", billingSubscription.SubscriptionID));
                }

                //TODO: hack email to test
                //billing.Email = TEST_EMAIL;

                Email email = emailService.SendEmail(dynamicEmail, billing, subscription.InitialShipping, subscription.InitialBillAmount, null, null, null, DateTime.Now);
                if (email != null)
                {
                    Console.WriteLine(string.Format("Email({0}) Sent", email.EmailID));

                    BillingBatchEmail batchEmail = new BillingBatchEmail();
                    batchEmail.BillingBatchEmailTypeID = billingBatchEmailType.BillingBatchEmailTypeID;
                    batchEmail.BillingID = billing.BillingID;
                    batchEmail.EmailID = email.EmailID;
                    dao.Save<BillingBatchEmail>(batchEmail);
                }               

                Console.WriteLine("SUCCESS");
            }
            catch (Exception ex)
            {
                Console.WriteLine("FAILED");
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
            }

            Console.WriteLine("--------------------------------------------------------");
        }
    }
}
