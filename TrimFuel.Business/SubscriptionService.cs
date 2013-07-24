using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Views;

namespace TrimFuel.Business
{
    public class SubscriptionService : BaseService
    {
        public BillingSubscription AssignPlan(int? billingSubscriptionID, long billingID, int subscriptionID)
        {
            BillingSubscription res = null;
            try
            {
                dao.BeginTransaction();

                if (billingSubscriptionID != null)
                {
                    res = EnsureLoad<BillingSubscription>(billingSubscriptionID);
                }
                else
                {
                    res = new BillingSubscription();
                }

                Subscription s = EnsureLoad<Subscription>(subscriptionID);

                res.BillingID = billingID;
                res.SubscriptionID = subscriptionID;
                res.CreateDT = DateTime.Now;
                res.StatusTID = BillingSubscriptionStatusEnum.Active;
                res.LastBillDate = DateTime.Now;
                if (s.InitialInterim != null)
                {
                    res.NextBillDate = DateTime.Now.AddDays(s.InitialInterim.Value);
                }
                else
                {
                    res.NextBillDate = null;
                }
                //res.SKU = s.SKU2;
                res.CustomerReferenceNumber = Utility.RandomString(new Random(), 6);

                dao.Save<BillingSubscription>(res);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }

        //cancelDateTime - with precision up to hour
        public IList<Set<Billing, Subscription, BillingCancelCode>> GetCancelledSubscriptions(int productID, DateTime cancelDateTime)
        {
            IList<Set<Billing, Subscription, BillingCancelCode>> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select distinct bs.* from BillingSubscription bs " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "inner join BillingCancelCode bcc on bcc.BillingID = b.BillingID " +
                    "inner join Subscription s on s.SubscriptionID = bs.SubscriptionID " +
                    "where bs.StatusTID = @inactiveSubscriptionStatus and s.ProductID = @productID and DATE(bcc.CreateDT) = DATE(@cancelDateTime) and HOUR(bcc.CreateDT) = HOUR(@cancelDateTime)");
                q.Parameters.Add("@cancelDateTime", MySqlDbType.Timestamp).Value = cancelDateTime;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@inactiveSubscriptionStatus", MySqlDbType.Int32).Value = BillingSubscriptionStatusEnum.Inactive;

                IList<BillingSubscription> bsList = dao.Load<BillingSubscription>(q);

                res = new List<Set<Billing, Subscription, BillingCancelCode>>();
                foreach (BillingSubscription bs in bsList)
                {
                    Billing billing = EnsureLoad<Billing>(bs.BillingID);
                    Subscription subscription = EnsureLoad<Subscription>(bs.SubscriptionID);

                    q = new MySqlCommand("select bcc.* from BillingCancelCode bcc " +
                        "where bcc.BillingID = @billingID");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billing.BillingID;

                    BillingCancelCode billingCancelCode = dao.Load<BillingCancelCode>(q).First();

                    res.Add(new Set<Billing, Subscription, BillingCancelCode>()
                    {
                        Value1 = billing,
                        Value2 = subscription,
                        Value3 = billingCancelCode
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Set<Billing, Subscription>> GetSubscriptionsToReactivate(int productID, DateTime cancelDateTime)
        {
            IList<Set<Billing, Subscription>> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select distinct bs.* from BillingSubscription bs " +
                    "inner join Billing b on b.BillingID = bs.BillingID " +
                    "inner join Subscription s on s.SubscriptionID = bs.SubscriptionID " +
                    "inner join " +
                        "(" +
                        "select bssh.BillingSubscriptionID, max(bssh.CreateDT) as CreateDT from BillingSubscriptionStatusHistory bssh " +
                        "where bssh.StatusTID = @inactiveSubscriptionStatus " +
                        "group by bssh.BillingSubscriptionID" +
                        ") bsc on bsc.BillingSubscriptionID = bs.BillingSubscriptionID " +
                    "where bs.StatusTID = @inactiveSubscriptionStatus and s.ProductID = @productID and DATE(bsc.CreateDT) = DATE(@cancelDateTime) and HOUR(bsc.CreateDT) = HOUR(@cancelDateTime) " +
                    "and b.BillingID not in " +
                        "(" +
                        "select affs.BillingID from AffiliateScrub affs " +
                        "union " +
                        "select bc.BillingID from BillingChargeback bc " +
                        ")" +
                    "and b.Email not in " +
                        "(" +
                        "select u.Email from Unsub u " +
                        ")");
                q.Parameters.Add("@cancelDateTime", MySqlDbType.Timestamp).Value = cancelDateTime;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@inactiveSubscriptionStatus", MySqlDbType.Int32).Value = BillingSubscriptionStatusEnum.Inactive;

                IList<BillingSubscription> bsList = dao.Load<BillingSubscription>(q);

                res = new List<Set<Billing, Subscription>>();
                foreach (BillingSubscription bs in bsList)
                {
                    Billing billing = EnsureLoad<Billing>(bs.BillingID);
                    Subscription subscription = EnsureLoad<Subscription>(bs.SubscriptionID);

                    res.Add(new Set<Billing, Subscription>()
                    {
                        Value1 = billing,
                        Value2 = subscription
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public BillingSubscription GetBillingSubscriptionByBilling(long? billingID)
        {
            BillingSubscription res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select bs.* from BillingSubscription bs " +
                    "where bs.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;

                res = dao.Load<BillingSubscription>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public BillingSubscription GetBillingSubscriptionByBillingAndProduct(long? billingID, int? productID)
        {
            BillingSubscription res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select bs.* from BillingSubscription bs " +
                    "inner join Subscription s on s.SubscriptionID = bs.SubscriptionID " +
                    "where bs.BillingID = @billingID and s.ProductID = @productID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                res = dao.Load<BillingSubscription>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<BillingSubscription> GetBillingSubscriptionsByBilling(long? billingID)
        {
            IList<BillingSubscription> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select bs.* from BillingSubscription bs " +
                    "where bs.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;

                res = dao.Load<BillingSubscription>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Product> GetProductList()
        {
            IList<Product> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select p.* from Product p " +
                    "where p.ProductIsActive = 1 " +
                    "order by p.ProductName");

                res = dao.Load<Product>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<ProductCode> GetProductCodeList()
        {
            IList<ProductCode> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select p.* from ProductCode p " +
                    "order by p.ProductCode");

                res = dao.Load<ProductCode>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Subscription> GetSubscriptionList()
        {
            IList<Subscription> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select s.* from Subscription s " +
                    "order by s.SubscriptionID");

                res = dao.Load<Subscription>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public void CancelSubscription(BillingSubscription billingSubscription)
        {
            try
            {
                dao.BeginTransaction();

                billingSubscription.StatusTID = TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Inactive;
                dao.Save<BillingSubscription>(billingSubscription);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public void ReturnNoRMASubscription(BillingSubscription billingSubscription)
        {
            try
            {
                dao.BeginTransaction();

                billingSubscription.StatusTID = TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.ReturnedNoRMA;
                dao.Save<BillingSubscription>(billingSubscription);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public void ChangePlan(BillingSubscription billingSubscription, int subscriptionID)
        {
            try
            {
                dao.BeginTransaction();

                billingSubscription.SubscriptionID = subscriptionID;
                dao.Save<BillingSubscription>(billingSubscription);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public IList<Registration> GetEmailAbandons(DateTime startDate, DateTime endDate, int productID)
        {
            IList<Registration> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select r.* from Registration r
                    inner join Campaign c on c.CampaignID = r.CampaignID
                    inner join Subscription s on s.SubscriptionID = c.SubscriptionID
                    where r.Email is not null and r.Email <> '' 
                    and s.ProductID = @productID
                    and r.CreateDT between @startDate and @endDate
                    and not exists (select b.Email from Billing b inner join BillingSubscription bs on b.BillingID = bs.BillingID where b.Email = r.Email)
                ");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                res = dao.Load<Registration>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<Registration> GetEmailNewsletters(DateTime startDate, DateTime endDate, int productID)
        {
            IList<Registration> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select r.* from Registration r
                    inner join Campaign c on c.CampaignID = r.CampaignID
                    inner join Subscription s on s.SubscriptionID = c.SubscriptionID
                    inner join Billing b on  b.RegistrationID = r.RegistrationID
                    inner join BillingSubscription bs on b.BillingID = bs.BillingID 
                    left join SaleChargeback sc on sc.BillingID=b.BillingID
                    left join AffiliateScrub afS on afS.BillingID=b.BillingID
                    left join BillingDoNotCall bnc on bnc.BillingID=b.BillingID
					where r.Email is not null and r.Email <> ''
                    and s.ProductID = @productID
                    and r.CreateDT between @startDate and @endDate
                    and afS.BillingID is NULL
                    and bnc.BillingID is NULL
					and sc.BillingID is NULL
                    and bs.StatusTID <> 8
                ");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                res = dao.Load<Registration>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public Subscription GetOrCreateOneTimeSaleSubscription(int productID)
        {
            Subscription subscription = null;

            try
            {
                dao.BeginTransaction();

                Product p = EnsureLoad<Product>(productID);

                MySqlCommand q = new MySqlCommand("select * from Subscription " +
                    "where productID = @productID and Recurring = 0 and ProductCode is null");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                subscription = dao.Load<Subscription>(q).LastOrDefault();
                if (subscription == null)
                {
                    subscription = new Subscription();
                    subscription.DisplayName = string.Format("One-time sale subscription for {0}", p.ProductName);
                    subscription.Selectable = false;
                    subscription.ProductID = productID;
                    subscription.Recurring = false;
                    subscription.ShipFirstRebill = false;
                    dao.Save<Subscription>(subscription);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }

            return subscription;
        }

        public IList<Inventory> GetInventoryList()
        {
            IList<Inventory> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select inv.* from Inventory inv " +
                    "order by inv.InventoryID");

                res = dao.Load<Inventory>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        
    }
}
