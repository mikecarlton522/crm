using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Configuration;
using TrimFuel.Business.Dao;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;
using TrimFuel.Model.Containers;
using TrimFuel.Business;

namespace CoAction_ImportLimeLight
{
    class Program
    {
        private const string CSV_FILE = "Data.coaction_active_subscriptions.csv";
        private const string CSV_SUBS_FILE = "Data.subscriptions.csv";
        private const int TAG_ID = 22;

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            int limit = int.Parse(ConfigurationSettings.AppSettings["limit"]);

            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                IDictionary<int, CASubscription> subscriptionMap = new Dictionary<int, CASubscription>();
                using (Stream s = typeof(Program).Assembly.GetManifestResourceStream(typeof(Program), CSV_SUBS_FILE))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        using (CsvReader csv = new CsvReader(sr, false))
                        {
                            while (csv.ReadNextRecord())
                            {
                                int? id = Utility.TryGetInt(csv[0]);
                                int? days = Utility.TryGetInt(csv[1]);
                                string sku = Utility.TryGetStr(csv[2]);
                                decimal? price = Utility.TryGetDecimal(csv[3]);

                                if (id != null &&
                                    days != null &&
                                    sku != null &&
                                    price != null)
                                {
                                    subscriptionMap.Add(id.Value, new CASubscription()
                                    {
                                        Days = days.Value,
                                        SKU = sku,
                                        Price = price.Value
                                    });
                                }
                                else
                                {
                                    logger.Info("Invalid subscription info. Line Number: " + csv.CurrentRecordIndex.ToString());
                                }
                            }
                        }
                    }
                }

                IDictionary<string, KeyValuePair<Set<Billing, Registration, RegistrationInfo>, IDictionary<int, CAUserSubscription>>> userList = 
                    new Dictionary<string, KeyValuePair<Set<Billing, Registration, RegistrationInfo>, IDictionary<int, CAUserSubscription>>>();

                using (Stream s = typeof(Program).Assembly.GetManifestResourceStream(typeof(Program), CSV_FILE))
                {
                    using (StreamReader sr = new StreamReader(s))
                    {
                        using (CsvReader csv = new CsvReader(sr, true))
                        {
                            while (csv.ReadNextRecord())
                            {
                                KeyValuePair<string, KeyValuePair<Set<Billing, Registration, RegistrationInfo>, KeyValuePair<int, CAUserSubscription>>>? userInfo = ExtractInfo(logger, csv, subscriptionMap);
                                if (userInfo != null)
                                {
                                    if (userList.ContainsKey(userInfo.Value.Key))
                                    {
                                        //User already exists

                                        //Replace Billing info with most recent
                                        userList[userInfo.Value.Key].Key.Value1 = userInfo.Value.Value.Key.Value1;
                                        //Replace Registration info with most recent
                                        userList[userInfo.Value.Key].Key.Value2 = userInfo.Value.Value.Key.Value2;
                                        if (userList[userInfo.Value.Key].Value.ContainsKey(userInfo.Value.Value.Value.Key))
                                        {
                                            //Subscription already exists
                                            userList[userInfo.Value.Key].Value[userInfo.Value.Value.Value.Key] = userInfo.Value.Value.Value.Value;
                                        }
                                        else
                                        {
                                            //Subscription does not exist
                                            userList[userInfo.Value.Key].Value.Add(userInfo.Value.Value.Value.Key, userInfo.Value.Value.Value.Value);
                                        }
                                    }
                                    else
                                    {
                                        //User does not exist

                                        userList.Add(userInfo.Value.Key,
                                            new KeyValuePair<Set<Billing, Registration, RegistrationInfo>, IDictionary<int, CAUserSubscription>>(
                                                userInfo.Value.Value.Key,
                                                new Dictionary<int, CAUserSubscription>(){
                                                    {userInfo.Value.Value.Value.Key, userInfo.Value.Value.Value.Value}
                                                }
                                            ));
                                    }
                                }
                            }
                        }
                    }
                }

                foreach (KeyValuePair<string, KeyValuePair<Set<Billing, Registration, RegistrationInfo>, IDictionary<int, CAUserSubscription>>> us in userList)
                {
                    if (limit <= 0)
                        break;
                    ProcessUser(dao, logger, us);
                    limit--;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

        }

        static KeyValuePair<string, KeyValuePair<Set<Billing, Registration, RegistrationInfo>, KeyValuePair<int, CAUserSubscription>>>? ExtractInfo(ILog logger, CsvReader csv, IDictionary<int, CASubscription> subscriptionMap)
        {
            KeyValuePair<string, KeyValuePair<Set<Billing, Registration, RegistrationInfo>, KeyValuePair<int, CAUserSubscription>>>? res = null;

            try
            {
                string internalID = Utility.TryGetStr(csv[39]);

                Registration r = new Registration();
                r.CreateDT = Utility.TryGetDate(csv[94]);

                r.FirstName = Utility.TryGetStr(csv[11]);
                r.LastName = Utility.TryGetStr(csv[12]);
                r.Address1 = Utility.TryGetStr(csv[13]);
                r.Address2 = null;
                r.City = Utility.TryGetStr(csv[14]);
                r.State = Utility.TryGetStr(csv[15]);
                r.Zip = Utility.TryGetStr(csv[16]);

                r.Phone = Utility.TryGetStr(csv[18]);
                r.Email = Utility.TryGetStr(csv[19]);
                r.IP = Utility.TryGetStr(csv[46]);

                r.CampaignID = null;
                r.Affiliate = Utility.TryGetStr(csv[74]);
                r.SubAffiliate = Utility.TryGetStr(csv[75]);

                RegistrationInfo rInfo = new RegistrationInfo();
                rInfo.Country = FixCountry(Utility.TryGetStr(csv[17]));

                Billing b = new Billing();
                b.CreateDT = Utility.TryGetDate(csv[94]);

                b.FirstName = Utility.TryGetStr(csv[2]);
                b.LastName = Utility.TryGetStr(csv[3]);
                b.Address1 = Utility.TryGetStr(csv[4]);
                b.Address2 = null;
                b.City = Utility.TryGetStr(csv[5]);
                b.State = Utility.TryGetStr(csv[6]);
                b.Zip = Utility.TryGetStr(csv[7]);
                b.Country = FixCountry(Utility.TryGetStr(csv[8]));

                b.Phone = Utility.TryGetStr(csv[9]);
                b.Email = Utility.TryGetStr(csv[10]);
                b.IP = Utility.TryGetStr(csv[46]);

                b.CampaignID = null;
                b.Affiliate = Utility.TryGetStr(csv[74]);
                b.SubAffiliate = Utility.TryGetStr(csv[75]);

                b.CreditCard = (new CreditCard(Utility.TryGetStr(csv[41]))).EncryptedCreditCard;
                string expDate = Utility.TryGetStr(csv[42]);
                if (expDate.Length != 4)
                {
                    throw new Exception("Invalid exp date");
                }
                b.ExpMonth = Utility.TryGetInt(expDate.Substring(0, 2));
                b.ExpYear = Utility.TryGetInt(expDate.Substring(2, 2));
                //b.CVV = Utility.TryGetStr(csv[0]);
                b.PaymentTypeID = GetPaymentType(Utility.TryGetStr(csv[41]));

                KeyValuePair<int, CAUserSubscription> s = new KeyValuePair<int, CAUserSubscription>(
                    Utility.TryGetInt(csv[38]).Value,
                    new CAUserSubscription() { 
                        Subscription = subscriptionMap[Utility.TryGetInt(csv[38]).Value],
                        NextBillDate = Utility.TryGetDate(csv[60]).Value,
                        LastBillDate = Utility.TryGetDate(csv[34] + " " + csv[35]).Value,
                        RebillCycle = Utility.TryGetInt(csv[84]).Value
                    }
                );

                res = new KeyValuePair<string, KeyValuePair<Set<Billing, Registration, RegistrationInfo>, KeyValuePair<int, CAUserSubscription>>>(
                    internalID,
                    new KeyValuePair<Set<Billing, Registration, RegistrationInfo>, KeyValuePair<int, CAUserSubscription>>(
                        new Set<Billing, Registration, RegistrationInfo>() { 
                            Value1 = b,
                            Value2 = r,
                            Value3 = rInfo
                        },
                        s
                    )
                );
            }
            catch (Exception ex)
            {
                logger.Error("Line: " + csv.CurrentRecordIndex.ToString(), ex);
                res = null;
            }

            return res;
        }

        static int? GetPaymentType(string cc)
        {
            if (cc == null || cc.Length < 1)
                return null;
            if (cc[0] == '3')
                return 1;
            if (cc[0] == '4')
                return 2;
            if (cc[0] == '5')
                return 3;
            return null;
        }

        static string FixCountry(string country)
        {
            if (country == null)
                return null;
            return country.Replace("United States", "US");
        }

        static void ProcessUser(IDao dao, ILog logger, KeyValuePair<string, KeyValuePair<Set<Billing, Registration, RegistrationInfo>, IDictionary<int, CAUserSubscription>>> user)
        {
            try
            {
                dao.BeginTransaction();

                BillingService serv = new BillingService();
                if (!string.IsNullOrEmpty(user.Key) && serv.GetLastBillingByInternalID(user.Key) != null)
                {
                    throw new Exception(string.Format("InternalID({0}) already exists", user.Key));
                }


                int productID = DetermineProductGroup(user.Value.Key.Value1.Country);

                dao.Save<Registration>(user.Value.Key.Value2);

                user.Value.Key.Value3.RegistrationID = user.Value.Key.Value2.RegistrationID;
                dao.Save<RegistrationInfo>(user.Value.Key.Value3);

                user.Value.Key.Value1.RegistrationID = user.Value.Key.Value2.RegistrationID;
                dao.Save<Billing>(user.Value.Key.Value1);

                BillingExternalInfo bei = new BillingExternalInfo();
                bei.BillingID = user.Value.Key.Value1.BillingID;
                bei.InternalID = user.Key;
                dao.Save<BillingExternalInfo>(bei);

                TagBillingLink tag = new TagBillingLink();
                tag.BillingID = user.Value.Key.Value1.BillingID;
                tag.TagID = TAG_ID;
                dao.Save<TagBillingLink>(tag);

                foreach (var item in user.Value.Value)
                {
                    bool shipFirst = (item.Value.RebillCycle > 0);
                    Subscription ss = DetermineOrCreateSubscription(dao, logger, item.Value.Subscription, productID, shipFirst);
                    
                    BillingSubscription bs = new BillingSubscription();
                    bs.BillingID = user.Value.Key.Value1.BillingID;
                    bs.CreateDT = user.Value.Key.Value1.CreateDT;
                    bs.LastBillDate = item.Value.LastBillDate;
                    bs.NextBillDate = item.Value.NextBillDate;
                    bs.StatusTID = 1;
                    bs.SubscriptionID = ss.SubscriptionID;
                    bs.CustomerReferenceNumber = Utility.RandomString(new Random(), 6);

                    dao.Save<BillingSubscription>(bs);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error("User was not added: " + user.Key, ex);
                dao.RollbackTransaction();
            }
        }

        static int DetermineProductGroup(string country)
        {
            if (country == "US" || country == "Canada")
                return 1;
            if (country == "United Kingdom")
                return 2;
            throw new Exception("Can't determine Product Group for Country: " + country);
        }

        static Subscription DetermineOrCreateSubscription(IDao dao, ILog logger, CASubscription s, int productID, bool shipFirstRebill)
        {
            Subscription res = null;
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand(@"
                    select s.* from Subscription s
                    where s.Recurring = 1 
                    and s.ProductID = @productID
                    and s.SecondInterim = @days and s.RegularInterim = @days
                    and s.SKU2 = @sku
                    and s.QuantitySKU2 = 1
                    and s.SecondBillAmount = @price and s.SecondShipping = 0.0 and s.RegularBillAmount = @price and s.RegularShipping = 0.00
                    and s.ShipFirstRebill = @shipFirstRebill
                    ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@days", MySqlDbType.Int32).Value = s.Days;
                q.Parameters.Add("@sku", MySqlDbType.VarChar).Value = s.SKU;
                q.Parameters.Add("@price", MySqlDbType.Decimal).Value = s.Price;
                q.Parameters.Add("@shipFirstRebill", MySqlDbType.Bit).Value = shipFirstRebill;

                Subscription ss = dao.Load<Subscription>(q).FirstOrDefault();
                if (ss == null)
                {
                    ss = new Subscription();
                    ss.ProductID = productID;
                    ss.Recurring = true;
                    ss.ProductCode = s.SKU;
                    ss.Quantity = 1;
                    ss.SKU2 = s.SKU;
                    ss.QuantitySKU2 = 1;
                    ss.InitialBillAmount = 0M;
                    ss.InitialShipping = 0M;
                    ss.SaveBilling = 0M;
                    ss.SaveShipping = 0M;
                    ss.InitialInterim = 14;
                    ss.SecondBillAmount = s.Price;
                    ss.SecondShipping = 0M;
                    ss.SecondInterim = s.Days;
                    ss.RegularBillAmount = s.Price;
                    ss.RegularShipping = 0M;
                    ss.RegularInterim = s.Days;
                    ss.ShipFirstRebill = shipFirstRebill;
                    dao.Save<Subscription>(ss);
                }

                res = ss;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error("Can't determine subscription", ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }
    }

    public class CASubscription
    {
        public int Days { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
    }

    public class CAUserSubscription
    {
        public CASubscription Subscription { get; set; }
        public DateTime NextBillDate { get; set; }
        public DateTime LastBillDate { get; set; }
        public int RebillCycle { get; set; }
    }
}
