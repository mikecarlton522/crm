using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Utils;
using TrimFuel.Encrypting;
using TrimFuel.Model;
using log4net;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Containers;
using TrimFuel.Model.Views;

namespace ApexProcessImport
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            int i = 1;

            string query = @"
                select ic.*, IfNull(min(it.BillDate), now()) as CreateDT from Import_Customers ic
                left join Import_Transactions it on it.CustomerID = ic.CustomerID
                group by ic.CustomerID
                order by CreateDT, ic.CustomerID
            ";

            string bsQuery = @"
                select ic.CustomerID, icb.BillingID, min(it.BillDate) as CreateDT, max(it.BillDate) as LastBillDate, ic.BillDate as NextBillDate, ic.Active, it.SubscriptionID from Import_Customers ic
                inner join Import_Transactions it on it.CustomerID = ic.CustomerID
                inner join Import_Customer_Billing icb on icb.CustomerID = ic.CustomerID
                group by ic.CustomerID
                order by ic.CustomerID
            ";

            string saleQuery = @"
                select icbs.BillingSubscriptionID, it.Amount, it.BillDate, s.ProductCode from Import_Transactions it
                inner join Import_Customer_BillingSubscription icbs on it.CustomerID = icbs.CustomerID
                inner join BillingSubscription bs on bs.BillingSubscriptionID = icbs.BillingSubscriptionID
                inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
                order by it.BillDate
            ";

            MySqlConnection connection = new MySqlConnection(Config.Current.CONNECTION_STRINGS["TrimFuel"]);
            MySqlCommand command = new MySqlCommand();

            try
            {
                connection.Open();

                command.Connection = connection;
                MySqlDataReader reader = null;

                ////Create Users
                //logger.Info("-----------------------------------------------");
                //logger.Info("-----------------------------------------------");
                //logger.Info("Users------------------------------------------");
                //logger.Info("-----------------------------------------------");
                //logger.Info("-----------------------------------------------");

                //command.CommandText = query;
                //reader = command.ExecuteReader();
                //while (reader.Read())
                //{
                //    int customerID = Convert.ToInt32(reader["CustomerID"].ToString());
                //    logger.Info("-----------------------------------------------");
                //    logger.Info("CustomerID: " + customerID.ToString());
                //    try
                //    {
                //        int expMonth = Convert.ToInt32(reader["ExpMonth"]);
                //        int expYear = Convert.ToInt32(reader["ExpYear"]);
                //        bool active = Convert.ToBoolean(reader["Active"]);
                //        DateTime createDT = Convert.ToDateTime(reader["CreateDT"]);

                //        CreateRegistrationAndBilling(dao, logger, i, customerID,
                //            reader["ShippingFirstName"].ToString(), reader["ShippingLastName"].ToString(), reader["ShippingAddress1"].ToString(), reader["ShippingAddress2"].ToString(), reader["ShippingCity"].ToString(), reader["ShippingState"].ToString(), reader["ShippingPostcode"].ToString(), reader["ShippingCountry"].ToString(), reader["ShippingPhone"].ToString(), reader["ShippingEmail"].ToString(),
                //            reader["BillingFirstName"].ToString(), reader["BillingLastName"].ToString(), reader["BillingAddress1"].ToString(), reader["BillingAddress2"].ToString(), reader["BillingCity"].ToString(), reader["BillingState"].ToString(), reader["BillingPostcode"].ToString(), reader["BillingCountry"].ToString(), reader["BillingPhone"].ToString(), reader["BillingEmail"].ToString(),
                //            reader["CreditCard"].ToString(), reader["CVV"].ToString(), expMonth, expYear, active, reader["OrderID"].ToString(), createDT, reader["CustomField"].ToString());
                //    }
                //    catch (Exception ex)
                //    {
                //        logger.Error(ex);
                //    }

                //    i++;
                //}

                //logger.Info("-----------------------------------------------");
                //logger.Info("-----------------------------------------------");
                //logger.Info("Subscriptions----------------------------------");
                //logger.Info("-----------------------------------------------");
                //logger.Info("-----------------------------------------------");

                ////Create Subscriptions
                IDictionary<int, int> rebillCycle = new Dictionary<int, int>();
                //command.CommandText = bsQuery;
                //reader = command.ExecuteReader();
                //while (reader.Read())
                //{
                //    int customerID = Convert.ToInt32(reader["CustomerID"].ToString());
                //    logger.Info("-----------------------------------------------");
                //    logger.Info("CustomerID: " + customerID.ToString());
                //    try
                //    {
                //        DateTime createDT = Convert.ToDateTime(reader["CreateDT"]);
                //        DateTime lastBillDate = Convert.ToDateTime(reader["LastBillDate"]);
                //        DateTime nextBillDate = Convert.ToDateTime(reader["NextBillDate"]);
                //        long billingID = Convert.ToInt64(reader["BillingID"]);
                //        bool active = Convert.ToBoolean(reader["Active"]);
                //        int subscriptionID = Convert.ToInt32(reader["SubscriptionID"]);


                //        CreateBillingSubscription(dao, logger,
                //            customerID, billingID, createDT, lastBillDate, nextBillDate, active, subscriptionID, rebillCycle);
                //    }
                //    catch (Exception ex)
                //    {
                //        logger.Error(ex);
                //    }

                //    i++;
                //}


                //Create Sales
                i = 1;
                command.CommandText = saleQuery;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    logger.Info("-----------------------------------------------");
                    logger.Info("Line number: " + i.ToString());
                    try
                    {
                        int billingSubscriptionID = Convert.ToInt32(reader["BillingSubscriptionID"]);
                        DateTime billDate = Convert.ToDateTime(reader["BillDate"]);
                        decimal amount = Convert.ToDecimal(reader["Amount"]);
                        string productCode = Convert.ToString(reader["ProductCode"]);


                        CreateTransaction(dao, logger,
                            billingSubscriptionID, billDate, amount, productCode, rebillCycle);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }

                    i++;
                }
            }
            catch(Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                if(connection != null)
                    connection.Close();

                if(command != null)
                    command.Dispose();
            }

            //TODO: call CreateTransaction for each record in Import_Transaction order by Date

        }

        static void CreateRegistrationAndBilling(IDao dao, ILog logger, int lineNumber, int? customerID,
            string shippingFirstName, string shippingLastName, string shippingAddress1, string shippingAddress2, string shippingCity, string shippingState, string shippingPostcode, string shippingCountry, string shippingPhone, string shippingEmail,
            string billingFirstName, string billingLastName, string billingAddress1, string billingAddress2, string billingCity, string billingState, string billingPostcode, string billingCountry, string billingPhone, string billingEmail,
            string creditcard, string cvv, int? expMonth, int? expYear, bool active, string orderID, DateTime createDT, string affiliate)
        {
            try
            {
                dao.BeginTransaction();

                long billingID = 0;
                Billing b = FindCustomer(logger, dao, billingFirstName, billingLastName, cvv, expMonth, expYear, creditcard, billingAddress1, billingAddress2, billingCity, billingState, billingPostcode, billingCountry, billingEmail, billingPhone);
                if (b == null)
                {
                    long registrationID = CreateRegistration(dao, shippingFirstName, shippingLastName, shippingAddress1, shippingAddress2, shippingCity, shippingState, shippingPostcode, shippingPhone, shippingEmail, createDT, affiliate);
                    long registrationInfoID = CreateRegistrationInfo(dao, registrationID, shippingCountry);
                    billingID = CreateBilling(dao, registrationID, (int)customerID, billingFirstName, billingLastName, creditcard, cvv, expMonth, expYear, billingAddress1, billingAddress2, billingCity, billingState, billingPostcode, billingCountry, billingEmail, billingPhone, createDT, affiliate);
                    CreateBillingExternalInfo(dao, billingID, customerID.ToString(), affiliate);
                }
                else
                {
                    billingID = b.BillingID.Value;
                }
                MySqlCommand q = new MySqlCommand("select count(*) as Value from Import_Customer_Billing where CustomerID = @CustomerID and BillingID = @BillingID");
                q.Parameters.AddWithValue("@CustomerID", customerID);
                q.Parameters.AddWithValue("@BillingID", billingID);
                int processed = dao.Load<View<int>>(q).FirstOrDefault().Value.Value;
                if (processed == 0)
                {
                    q = new MySqlCommand("insert into Import_Customer_Billing (CustomerID, BillingID) values (@CustomerID, @BillingID)");
                    q.Parameters.AddWithValue("@CustomerID", customerID);
                    q.Parameters.AddWithValue("@BillingID", billingID);
                    dao.ExecuteNonQuery(q);
                }

                dao.CommitTransaction();
            }
            catch(Exception ex)
            {
                dao.RollbackTransaction();

                logger.Error(ex);
            }
        }

        static long CreateRegistration(IDao dao, string firstName, string lastName, string address1, string address2, string city, string state, string zip, string phone, string email, DateTime? createDT, string affiliate)
        {
            Registration registration = new Registration();

            registration.FirstName = firstName;
            registration.LastName = lastName;
            registration.Address1 = address1;
            registration.Address2 = address2;
            registration.City = city;
            registration.State = state;
            registration.Zip = zip;
            registration.Phone = phone;
            registration.Email = email;
            registration.CreateDT = createDT;
            registration.Affiliate = affiliate;
            registration.URL = "Import2";

            dao.Save<Registration>(registration);

            return (long)registration.RegistrationID;
        }

        static long CreateRegistrationInfo(IDao dao, long registrationID, string country)
        {
            RegistrationInfo registrationInfo = new RegistrationInfo();

            registrationInfo.RegistrationID = registrationID;
            registrationInfo.Country = country;

            dao.Save<RegistrationInfo>(registrationInfo);

            return (long)registrationInfo.RegistrationInfoID;
        }

        static long CreateBilling(IDao dao, long registrationID, int customerID, string firstName, string lastName, string creditcard, string cvv, int? expMonth, int? expYear, string address1, string address2, string city, string state, string zip, string country, string email, string phone, DateTime? createDT, string affiliate)
        {
            Billing billing = new Billing();                

            billing.RegistrationID = registrationID;
            billing.FirstName = firstName;
            billing.LastName = lastName;
            billing.CreditCard = creditcard;
            billing.PaymentTypeID = billing.CreditCardCnt.TryGetCardType();
            billing.CVV = cvv;
            billing.ExpMonth = expMonth;
            billing.ExpYear = expYear;
            billing.Address1 = address1;
            billing.Address2 = address2;
            billing.City = city;
            billing.State = state;
            billing.Zip = zip;
            billing.Country = country;
            billing.Email = email;
            billing.Phone = phone;
            billing.CreateDT = createDT;
            billing.Affiliate = affiliate;
            billing.URL = "Import2";

            dao.Save<Billing>(billing);

            return (long)billing.BillingID;
        }

        static void CreateBillingExternalInfo(IDao dao, long billingID, string internalID, string customField1)
        {
            BillingExternalInfo billingExternalInfo = new BillingExternalInfo();

            billingExternalInfo.BillingID = billingID;
            billingExternalInfo.InternalID = internalID;
            billingExternalInfo.CustomField1 = customField1;

            dao.Save<BillingExternalInfo>(billingExternalInfo);
        }

        static void CreateBillingSubscription(IDao dao, ILog logger,
            int customerID, long billingID, DateTime createDT, DateTime lastBillDate, DateTime nextBillDate, bool active, int subscriptionID, IDictionary<int, int> rebillCycle)
        {
            try
            {
                dao.BeginTransaction();

                BillingSubscription billingSubscription = new BillingSubscription();

                billingSubscription.BillingID = billingID;
                billingSubscription.SubscriptionID = subscriptionID;
                billingSubscription.CreateDT = createDT;
                billingSubscription.LastBillDate = lastBillDate;
                billingSubscription.NextBillDate = nextBillDate;
                billingSubscription.StatusTID = (active ? 1 : 0);
                billingSubscription.CustomerReferenceNumber = Utility.RandomString(new Random(), 6);

                dao.Save<BillingSubscription>(billingSubscription);

                MySqlCommand q = new MySqlCommand("select count(*) as Value from Import_Customer_BillingSubscription where CustomerID = @CustomerID and BillingSubscriptionID = @BillingSubscriptionID");
                q.Parameters.AddWithValue("@CustomerID", customerID);
                q.Parameters.AddWithValue("@BillingSubscriptionID", billingSubscription.BillingSubscriptionID);
                int processed = dao.Load<View<int>>(q).FirstOrDefault().Value.Value;
                if (processed == 0)
                {
                    q = new MySqlCommand("insert into Import_Customer_BillingSubscription (CustomerID, BillingSubscriptionID) values (@CustomerID, @BillingSubscriptionID)");
                    q.Parameters.AddWithValue("@CustomerID", customerID);
                    q.Parameters.AddWithValue("@BillingSubscriptionID", billingSubscription.BillingSubscriptionID);
                    dao.ExecuteNonQuery(q);
                }
                rebillCycle[billingSubscription.BillingSubscriptionID.Value] = 0;

                dao.CommitTransaction();
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        static void CreateTransaction(IDao dao, ILog logger, int billingSubscriptionID, DateTime billDate, decimal amount, string productCode, IDictionary<int, int> rebillCycle)
        {
            try
            {
                dao.BeginTransaction();
                
                CreateBillingSale(dao, 
                    CreateChargeDetails(dao, amount, billingSubscriptionID, billDate, productCode),
                    rebillCycle);

                dao.CommitTransaction();
            }
            catch(Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        static void CreateBillingSale(IDao dao, ChargeDetails cd, IDictionary<int, int> rebillCycle)
        {
            try
            {
                BillingSale billingSale = new BillingSale();

                billingSale.BillingSubscriptionID = cd.BillingSubscriptionID;
                billingSale.SaleTypeID = (short)cd.SaleTypeID.Value;
                billingSale.NotShip = true;
                if (billingSale.SaleTypeID == 1)
                {
                    billingSale.RebillCycle = 0;
                }
                else
                {
                    if (!rebillCycle.ContainsKey(billingSale.BillingSubscriptionID.Value))
                    {
                        rebillCycle[billingSale.BillingSubscriptionID.Value] = 0;
                    }
                    billingSale.RebillCycle = rebillCycle[billingSale.BillingSubscriptionID.Value] + 1;
                    rebillCycle[billingSale.BillingSubscriptionID.Value] = billingSale.RebillCycle.Value;
                }
                billingSale.ProductCode = cd.SKU;
                billingSale.Quantity = 1;
                billingSale.ChargeHistoryID = cd.ChargeHistoryID;
                billingSale.CreateDT = cd.ChargeDate;

                dao.Save<BillingSale>(billingSale);
            }
            catch
            {
                throw;
            }
        }

        static ChargeDetails CreateChargeDetails(IDao dao, decimal amount, int billingSubscriptionID, DateTime? chargeDate, string sku)
        {
            ChargeDetails chargeDetails = new ChargeDetails();

            chargeDetails.Amount = amount;
            chargeDetails.BillingSubscriptionID = billingSubscriptionID;
            chargeDetails.ChargeDate = chargeDate;
            chargeDetails.ChildMID = "Import";
            chargeDetails.ChargeTypeID = 1;
            chargeDetails.MerchantAccountID = 54;
            chargeDetails.SaleTypeID = amount < 3 ? (short)1 : (short)2;
            chargeDetails.SKU = sku;
            chargeDetails.Success = true;

            dao.Save<ChargeDetails>(chargeDetails);

            return chargeDetails;
        }

        static Billing FindCustomer(ILog logger, IDao dao, 
            string firstName, string lastName,
            string cvv, int? expMonth, int? expYear, string creditCard,
            string address1, string address2, string city, string state, string zip, string country, string email, string phone)
        {
            Billing res = null;

            try
            {
                //First search without CC
                MySqlCommand q = new MySqlCommand(@" 
                    select b.* from Billing b
                    where b.URL = 'Import2'
                    and (b.FirstName = @firstName or (b.FirstName is null and @firstName is null))
                    and (b.LastName = @lastName or (b.LastName is null and @lastName is null))
                    and (b.CVV = @cvv or (b.CVV is null and @cvv is null))
                    and (b.ExpMonth = @expMonth or (b.ExpMonth is null and @expMonth is null))
                    and (b.ExpYear = @expYear or (b.ExpYear is null and @expYear is null))
                    and (b.Address1 = @address1 or (b.Address1 is null and @address1 is null))
                    and (b.Address2 = @address2 or (b.Address2 is null and @address2 is null))
                    and (b.City = @city or (b.City is null and @city is null))
                    and (b.State = @state or (b.State is null and @state is null))
                    and (b.Zip = @Zip or (b.Zip is null and @zip is null))
                    and (b.Country = @country or (b.Country is null and @country is null))
                    and (b.Email = @email or (b.Email is null and @email is null))
                    and (b.Phone = @phone or (b.Phone is null and @phone is null))
                    order by b.BillingID asc"
                );
                q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = firstName;
                q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = lastName;
                q.Parameters.Add("@cvv", MySqlDbType.VarChar).Value = cvv;
                q.Parameters.Add("@expMonth", MySqlDbType.Int32).Value = expMonth;
                q.Parameters.Add("@expYear", MySqlDbType.Int32).Value = expYear;
                q.Parameters.Add("@address1", MySqlDbType.VarChar).Value = address1;
                q.Parameters.Add("@address2", MySqlDbType.VarChar).Value = address2;
                q.Parameters.Add("@city", MySqlDbType.VarChar).Value = city;
                q.Parameters.Add("@state", MySqlDbType.VarChar).Value = state;
                q.Parameters.Add("@zip", MySqlDbType.VarChar).Value = zip;
                q.Parameters.Add("@country", MySqlDbType.VarChar).Value = country;
                q.Parameters.Add("@email", MySqlDbType.VarChar).Value = email;
                q.Parameters.Add("@phone", MySqlDbType.VarChar).Value = phone;

                foreach (Billing match in dao.Load<Billing>(q))
                {
                    if (creditCard == match.CreditCard)
                    {
                        //Exact match
                        res = match;
                        break;
                    }
                    else
                    {
                        bool matchCC = false;
                        try
                        {
                            matchCC = (new CreditCard(creditCard).DecryptedCreditCard == match.CreditCardCnt.DecryptedCreditCard);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("ERROR: Can't decrypt CC of match.", ex);
                        }
                        if (matchCC)
                        {
                            res = match;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }
    }
}
