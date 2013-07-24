using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using System.IO;
using TrimFuel.Model;
using TrimFuel.Business;
using System.Configuration;
using LumenWorks.Framework.IO.Csv;
using MySql.Data.MySqlClient;

namespace CoAction_UpdateCountryAndPlans
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            string filename = ConfigurationSettings.AppSettings["filename"].ToString();

            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                IDictionary<int, string> billingCountryMap = new Dictionary<int, string>();

                using (StreamReader sr = new StreamReader(filename))
                {
                    using (CsvReader csv = new CsvReader(sr, false))
                    {
                        while (csv.ReadNextRecord())
                        {
                            try
                            {
                                billingCountryMap.Add(Convert.ToInt32(csv[0]), csv[1]);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }
                    }
                }

                foreach (KeyValuePair<int, string> kvp in billingCountryMap)
                {
                    UpdateCountry(dao, logger, kvp.Key, kvp.Value);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static bool UpdateCountry(IDao dao, ILog logger, int billingID, string country)
        {
            bool res = false;

            try
            {
                Billing b = dao.Load<Billing>(billingID);

                dao.BeginTransaction();
                logger.Info("Update country: " + country);

                b.Country = country;

                dao.Save<Billing>(b);

                ProcessPlans(dao, logger, b, country);

                dao.CommitTransaction();

            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(ex);
            }

            logger.Info("");

            return res;
        }

        private static void ProcessPlans(IDao dao, ILog logger, Billing b, string country)
        {
            IDictionary<string, int> countryProductMap = new Dictionary<string, int>()
            {
                {"US", 1},
                {"UK", 2},
                {"AU", 3},
                {"CAN", 1 }
            };

            try
            {
                dao.BeginTransaction();

                int productID = countryProductMap[country];
                MySqlCommand q = new MySqlCommand("select * from BillingSubscription where BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;

                IList<BillingSubscription> bsList = dao.Load<BillingSubscription>(q);

                foreach (BillingSubscription bs in bsList)
                {
                    Subscription s = dao.Load<Subscription>(bs.SubscriptionID);
                    if (s.ProductID.Value != productID)
                    {
                        bs.SubscriptionID = FindPlan(dao, logger, s, productID).SubscriptionID;
                        dao.Save<BillingSubscription>(bs);
                        logger.Info("Update plan: " + s.SubscriptionID.Value.ToString() + " -> " + bs.SubscriptionID.Value.ToString());
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(ex);
            }
        }

        private static Subscription FindPlan(IDao dao, ILog logger, Subscription s, int productID)
        {
            Subscription res = null;
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand(
                    " select s.* from Subscription s" +
                    " inner join Subscription s2" +
                    " on (s2.InitialInterim = s.InitialInterim or s2.InitialInterim is null and s.InitialInterim is null)" +
                    " and (s2.InitialShipping = s.InitialShipping or s2.InitialShipping is null and s.InitialShipping is null)" +
                    " and (s2.SaveShipping = s.SaveShipping or s2.SaveShipping is null and s.SaveShipping is null)" +
                    " and (s2.SaveBilling = s.SaveBilling or s2.SaveBilling is null and s.SaveBilling is null)" +
                    " and (s2.InitialBillAmount = s.InitialBillAmount or s2.InitialBillAmount is null and s.InitialBillAmount is null)" +
                    " and (s2.SecondInterim = s.SecondInterim or s2.SecondInterim is null and s.SecondInterim is null)" +
                    " and (s2.SecondShipping = s.SecondShipping or s2.SecondShipping is null and s.SecondShipping is null)" +
                    " and (s2.SecondBillAmount = s.SecondBillAmount or s2.SecondBillAmount is null and s.SecondBillAmount is null)" +
                    " and (s2.RegularInterim = s.RegularInterim or s2.RegularInterim is null and s.RegularInterim is null)" +
                    " and (s2.RegularShipping = s.RegularShipping or s2.RegularShipping is null and s.RegularShipping is null)" +
                    " and (s2.RegularBillAmount = s.RegularBillAmount or s2.RegularBillAmount is null and s.RegularBillAmount is null)" +
                    " and (s2.ProductCode = s.ProductCode or s2.ProductCode is null and s.ProductCode is null)" +
                    " and (s2.Quantity = s.Quantity or s2.Quantity is null and s.Quantity is null)" +
                    " and (s2.Recurring = s.Recurring or s2.Recurring is null and s.Recurring is null)" +
                    " and (s2.ShipFirstRebill = s.ShipFirstRebill or s2.ShipFirstRebill is null and s.ShipFirstRebill is null)" +
                    " and (s2.SKU2 = s.SKU2 or s2.SKU2 is null and s.SKU2 is null)" +
                    " and (s2.QuantitySKU2 = s.QuantitySKU2 or s2.QuantitySKU2 is null and s.QuantitySKU2 is null)" +
                    " where s2.SubscriptionID = @subscriptionID and s.ProductID = @productID" +
                    " order by s.SubscriptionID limit 1"
                );
                q.Parameters.Add("@subscriptionID", MySqlDbType.Int32).Value = s.SubscriptionID;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                res = dao.Load<Subscription>(q).FirstOrDefault();
                if (res == null)
                {
                    throw new Exception(string.Format("Can't find Subscription for Product({0}) similar to Subscription({1})", productID, s.SubscriptionID));
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(ex);
            }

            return res;
        }
    }
}
