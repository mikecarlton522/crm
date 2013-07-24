using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using log4net;
using LumenWorks.Framework.IO.Csv;

using TrimFuel.Business;
using TrimFuel.Business.Dao;
using TrimFuel.Model;

namespace CoAction_ImportAndCheckPlan
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
                List<Customer> customers = new List<Customer>();


                using (StreamReader sr = new StreamReader(filename))
                {
                    using (CsvReader csv = new CsvReader(sr, false, ';'))
                    {
                        while (csv.ReadNextRecord())
                        {
                            try
                            {
                                Customer customer = new Customer();

                                customer.FirstName= csv[0];
                                customer.LastName= csv[1];
                                customer.Address1= csv[2];
                                customer.Address2= csv[3];
                                customer.City= csv[4];
                                customer.State= csv[5];
                                customer.Zip= csv[6];

                                if (customer.State.Length == 2)
                                {
                                    if (customer.Zip.Length > 5)
                                        customer.Country = "CAN";
                                    else
                                        customer.Country = "US";
                                }
                                else if (customer.State.Length == 3)
                                    customer.Country = "AU";

                                else
                                    customer.Country = "UK";

                                customer.Phone = null;
                                customer.CreateDT= DateTime.Parse(csv[7], new CultureInfo("en-US"));
                                customer.Email= csv[8];
                                customer.CampaignID= null;
                                customer.Affiliate= null;
                                customer.SubAffiliate= null;
                                customer.Ip= null;
                                customer.Url= null;
                                customer.PaymentTypeID= null;
                                customer.CreditCard= null;
                                customer.Cvv= null;
                                customer.ExpMonth= null;
                                customer.ExpYear= null;
                                customer.InternalID= csv[9];
                                customer.SubscriptionID = Convert.ToInt32(csv[10]);
                                customer.NextBillDate = DateTime.Parse(csv[11], new CultureInfo("en-US"));
                                
                                customers.Add(customer);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex.Message);
                            }
                        }
                    }
                }

                foreach (Customer customer in customers)
                {
                    CreateRegistrationAndBilling(dao, logger, customer.FirstName, customer.LastName, customer.Address1, customer.Address2, customer.City, customer.State, customer.Zip, customer.Country, customer.Phone, customer.CreateDT, customer.Email, customer.CampaignID, customer.Affiliate, customer.SubAffiliate, customer.Ip, customer.Url, customer.PaymentTypeID, customer.CreditCard, customer.Cvv, customer.ExpMonth, customer.ExpYear, customer.InternalID, customer.SubscriptionID, customer.NextBillDate);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        static void CreateRegistrationAndBilling(IDao dao, ILog logger, string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country, string phone, DateTime createDT, string email, int? campaignID, string affiliate, string subAffiliate, string ip, string url, int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear, string internalID, int subscriptionID, DateTime nextBillDate)
        {
            RegistrationService rs = new RegistrationService();

            long registrationID = 0;

            long billingID = 0;

            IDictionary<string, string> countryProductMap = new Dictionary<string, string>()
            {
                {"US", "US/CA"},
                {"UK", "GBP"},
                {"AU", "AU"},
                {"CAN", "US/CA" }
            };

            //try
            //{
            //    Registration r = rs.CreateRegistration(null, firstName, lastName, address1, address2, city, state, zip, country, email, phone, createDT, affiliate, subAffiliate, ip, url);

            //    registrationID = (long)r.RegistrationID;

            //    logger.Info("Registration added");
            //}
            //catch (Exception ex)
            //{
            //    logger.Error("Couldn't add registration: " + ex.Message);
            //    logger.Info("");
            //    return;
            //}

            //try
            //{
            //    Billing b = rs.CreateBilling(null, registrationID, firstName, lastName, creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city, state, zip, country, email, phone, createDT, affiliate, subAffiliate, ip, url);

            //    logger.Info("Billing added");

            //    billingID = (long)b.BillingID;
            //}
            //catch (Exception ex)
            //{
            //    logger.Error("Couldn't add billing: " + ex.Message);
            //    logger.Info("");
            //    return;
            //}

            //try
            //{
            //    BillingExternalInfo bei = new BillingExternalInfo();

            //    dao.BeginTransaction();

            //    bei.BillingID = billingID;
            //    bei.InternalID = internalID;

            //    dao.Save<BillingExternalInfo>(bei);

            //    dao.CommitTransaction();

            //    logger.Info("External info added");
            //}
            //catch (Exception ex)
            //{
            //    logger.Error("Couldn't add external info: " + ex.Message);
            //    logger.Info("");
            //    return;
            //}

            try
            {
                Subscription s = dao.Load<Subscription>(subscriptionID);

                if (s == null)
                {
                    logger.Error("Subscription not found: " + subscriptionID);
                    logger.Info("");
                    return;
                }
                else
                {
                    

                    Product p = dao.Load<Product>(s.ProductID);

                    

                    if (!p.ProductName.EndsWith(countryProductMap[country]))
                    {
                        
                        logger.InfoFormat("{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", internalID, firstName, lastName, zip, state, country, p.ProductName, s.SubscriptionID);
                        
                    }

                    //else
                        //logger.Info("Check: OK");

                    //BillingSubscription bs = new BillingSubscription();

                    //dao.BeginTransaction();

                    //bs.SubscriptionID = subscriptionID;
                    //bs.NextBillDate = nextBillDate;
                    //bs.CreateDT = createDT;
                    //bs.LastBillDate = createDT;

                    //dao.Save<BillingSubscription>(bs);

                    //dao.CommitTransaction();

                    //logger.Info("BillingSubscription added");
                }

            }
            catch (Exception ex)
            {
                logger.Error("Couldn't determine product-country match: " + ex.Message);
                logger.Info("");
                return;
            }

            //logger.Info("");

        }
    }

    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public DateTime CreateDT { get; set; }
        public string Email { get; set; }
        public int? CampaignID { get; set; }
        public string Affiliate { get; set; }
        public string SubAffiliate { get; set; }
        public string Ip { get; set; }
        public string Url { get; set; }
        public int? PaymentTypeID { get; set; }
        public string CreditCard { get; set; }
        public string Cvv { get; set; }
        public int? ExpMonth { get; set; }
        public int? ExpYear { get; set; }
        public string InternalID { get; set; }
        public int SubscriptionID { get; set; }
        public DateTime NextBillDate { get; set; }
    }
}
