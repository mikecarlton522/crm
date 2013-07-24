using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TrimFuel.Business;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Utils;
using TrimFuel.Encrypting;
using TrimFuel.Model;
using log4net;
using LumenWorks.Framework.IO.Csv;
using MySql.Data.MySqlClient;

namespace ApexRawImport
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

            ImportRawCustomerData(dao, logger, "customers_import_ready.csv");

            ImportRawTransactionData(dao, logger, "transactions_import_ready.csv");
        }

        static bool HasValue(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;

            str = str.Trim();

            if (str.Equals("NULL"))
                return false;

            return true;
        }

        static void ImportRawCustomerData(IDao dao, ILog logger, string filename)
        {
            CC cc = new CC();

            using (StreamReader sr = new StreamReader(filename))
            {
                using (CsvReader csv = new CsvReader(sr, false, ';'))
                {
                    int i = 1;
                    while (csv.ReadNextRecord())
                    {
                        try
                        {
                            int customerIDTmp = HasValue(csv[0]) ? (int.TryParse(csv[0], out customerIDTmp) ? customerIDTmp : 0) : 0;
                            int? customerID = customerIDTmp;
                            if (customerIDTmp == 0)
                                customerID = null;

                            string billingFirstName = HasValue(csv[11]) ? csv[11] : null;
                            string billingLastName = HasValue(csv[12]) ? csv[12] : null;
                            string billingAddress1 = HasValue(csv[13]) ? csv[13] : null;
                            string billingAddress2 = HasValue(csv[14]) ? csv[14] : null;
                            string billingCity = HasValue(csv[15]) ? csv[15] : null;
                            string billingState = HasValue(csv[16]) ? csv[16] : null;
                            string billingPostcode = HasValue(csv[17]) ? csv[17] : null;
                            string billingCountry = HasValue(csv[8]) ? csv[8] : null;
                            string billingPhone = HasValue(csv[18]) ? csv[18] : null;
                            string billingEmail = HasValue(csv[19]) ? csv[19] : null;

                            string shippingFirstName = HasValue(csv[1]) ? csv[1] : billingFirstName;
                            string shippingLastName = HasValue(csv[2]) ? csv[2] : billingLastName;
                            string shippingAddress1 = HasValue(csv[3]) ? csv[3] : billingAddress1;
                            string shippingAddress2 = HasValue(csv[4]) ? csv[4] : billingAddress2;
                            string shippingCity = HasValue(csv[5]) ? csv[5] : billingCity;
                            string shippingState = HasValue(csv[6]) ? csv[6] : billingState;
                            string shippingPostcode = HasValue(csv[7]) ? csv[7] : billingPostcode;
                            string shippingCountry = HasValue(csv[8]) ? csv[8] : null;
                            string shippingPhone = HasValue(csv[9]) ? csv[9] : billingPhone;
                            string shippingEmail = HasValue(csv[10]) ? csv[10] : billingEmail;

                            string creditcard = HasValue(csv[20]) ? csv[20] : null;
                            if (creditcard != null)
                            {
                                creditcard = Regex.Replace(creditcard, "[^0-9]", "");
                                creditcard = cc.GetEncrypted(creditcard);
                            }

                            string cvv = HasValue(csv[21]) ? csv[21] : null;
                                                        
                            int expMonthTmp = HasValue(csv[22]) ? (int.TryParse(csv[22], out expMonthTmp) ? expMonthTmp : 0) : 0;
                            int? expMonth = expMonthTmp;
                            if (expMonthTmp == 0)
                                expMonth = null;

                            int expYearTmp = HasValue(csv[23]) ? (int.TryParse(csv[23], out expMonthTmp) ? expMonthTmp : 0) : 0;
                            int? expYear = expYearTmp;
                            if (expYearTmp == 0)
                                expYear = null;
                            
                            bool active = HasValue(csv[24]) ? csv[24].Equals("1") : false;
                            string orderID = HasValue(csv[25]) ? csv[25] : null;

                            DateTime billDateTmp = HasValue(csv[26]) ? (DateTime.TryParse(csv[26], out billDateTmp) ? billDateTmp : DateTime.MinValue) : DateTime.MinValue;
                            DateTime? billDate = billDateTmp;
                            if (billDateTmp == DateTime.MinValue)
                                billDate = null;

                            string affiliate = HasValue(csv[27]) ? csv[27] : null;
                            if (affiliate == null)
                                affiliate = "Transfers";
                            else if (affiliate.ToLower().Equals("postcard"))
                                affiliate = "Postcard Mailer";
                            else if (affiliate.ToLower().Equals("call transfer"))
                                affiliate = "Transfers";


                            InsertRawCustomerData(dao, logger, i, customerID,
                                shippingFirstName, shippingLastName, shippingAddress1, shippingAddress2, shippingCity, shippingState, shippingPostcode, shippingCountry, shippingPhone, shippingEmail,
                                billingFirstName, billingLastName, billingAddress1, billingAddress2, billingCity, billingState, billingPostcode, shippingCountry, billingPhone, billingEmail,
                                creditcard, cvv, expMonth, expYear, active, orderID, billDate, affiliate);
                        }
                        catch
                        {
                            logger.Info(string.Format("/* CSV error on line {0} */", i));
                        }
                    }
                }
            }
        }

        static void ImportRawTransactionData(IDao dao, ILog logger, string filename)
        {
            using (StreamReader sr = new StreamReader(filename))
            {
                using (CsvReader csv = new CsvReader(sr, false, ';'))
                {
                    int i = 1;
                    while (csv.ReadNextRecord())
                    {
                        try
                        {
                            string orderID = HasValue(csv[0]) ? csv[0] : null;

                            int customerID = int.Parse(csv[1]);

                            DateTime billDate = DateTime.Parse(csv[2]);
                            
                            int subscriptionID = 0;
                            string subscription = HasValue(csv[3]) ? csv[3] : null;
                            if (subscription.Equals("DE"))
                            {
                                subscriptionID = 480;
                            }
                            else if (subscription.Equals("GS"))
                            {
                                subscriptionID = 481;
                            }
                            else
                            {
                                throw new Exception("Can't determined subscription: " + subscription);
                            }

                            decimal amount = Convert.ToDecimal(csv[4].ToString());

                            InsertRawTransactionData(dao, logger, i, orderID, customerID, billDate, subscriptionID, amount);
                        }
                        catch(Exception ex)
                        {
                            logger.Error(ex);
                            logger.Info(string.Format("/* CSV error on line {0} */", i));
                        }
                    }
                }
            }
        }

        static void InsertRawCustomerData(IDao dao, ILog logger, int lineNumber, int? customerID,
            string shippingFirstName, string shippingLastName, string shippingAddress1, string shippingAddress2, string shippingCity, string shippingState, string shippingPostcode, string shippingCountry, string shippingPhone, string shippingEmail,
            string billingFirstName, string billingLastName, string billingAddress1, string billingAddress2, string billingCity, string billingState, string billingPostcode, string billingCountry, string billingPhone, string billingEmail,
            string creditcard, string cvv, int? expMonth, int? expYear, bool active, string orderID, DateTime? billDate, string affiliate)
        {
            string query = @"
                INSERT INTO Import_Customers (
                    CustomerID,
                    ShippingFirstName,ShippingLastName,ShippingAddress1,ShippingAddress2,ShippingCity,ShippingState,ShippingPostcode,ShippingCountry,ShippingPhone,ShippingEmail,
                    BillingFirstName,BillingLastName,BillingAddress1,BillingAddress2,BillingCity,BillingState,BillingPostcode,BillingCountry,BillingPhone,BillingEmail,
                    CreditCard,CVV,ExpMonth,ExpYear,Active,OrderID,BillDate,CustomField)
                VALUES (
                    @CustomerID,
                    @ShippingFirstName,@ShippingLastName,@ShippingAddress1,@ShippingAddress2,@ShippingCity,@ShippingState,@ShippingPostcode,@ShippingCountry,@ShippingPhone,@ShippingEmail,
                    @BillingFirstName,@BillingLastName,@BillingAddress1,@BillingAddress2,@BillingCity,@BillingState,@BillingPostcode,@BillingCountry,@BillingPhone,@BillingEmail,
                    @CreditCard,@CVV,@ExpMonth,@ExpYear,@Active,@OrderID,@BillDate,@CustomField);";

            try
            {
                MySqlCommand command = new MySqlCommand();

                command.CommandText = query;

                command.Parameters.AddWithValue("@CustomerID", customerID);
                command.Parameters.AddWithValue("@ShippingFirstName", shippingFirstName);
                command.Parameters.AddWithValue("@ShippingLastName", shippingLastName);
                command.Parameters.AddWithValue("@ShippingAddress1", shippingAddress1);
                command.Parameters.AddWithValue("@ShippingAddress2", shippingAddress2);
                command.Parameters.AddWithValue("@ShippingCity", shippingCity);
                command.Parameters.AddWithValue("@ShippingState", shippingState);
                command.Parameters.AddWithValue("@ShippingPostcode", shippingPostcode);
                command.Parameters.AddWithValue("@ShippingCountry", shippingCountry);
                command.Parameters.AddWithValue("@ShippingPhone", shippingPhone);
                command.Parameters.AddWithValue("@ShippingEmail", shippingEmail);
                command.Parameters.AddWithValue("@BillingFirstName", billingFirstName);
                command.Parameters.AddWithValue("@BillingLastName", billingLastName);
                command.Parameters.AddWithValue("@BillingAddress1", billingAddress1);
                command.Parameters.AddWithValue("@BillingAddress2", billingAddress2);
                command.Parameters.AddWithValue("@BillingCity", billingCity);
                command.Parameters.AddWithValue("@BillingState", billingState);
                command.Parameters.AddWithValue("@BillingPostcode", billingPostcode);
                command.Parameters.AddWithValue("@BillingCountry", billingCountry);
                command.Parameters.AddWithValue("@BillingPhone", billingPhone);
                command.Parameters.AddWithValue("@BillingEmail", billingEmail);
                command.Parameters.AddWithValue("@CreditCard", creditcard);
                command.Parameters.AddWithValue("@CVV", cvv);
                command.Parameters.AddWithValue("@ExpMonth", expMonth);
                command.Parameters.AddWithValue("@ExpYear", expYear);
                command.Parameters.AddWithValue("@Active", active);
                command.Parameters.AddWithValue("@OrderID", orderID);
                command.Parameters.AddWithValue("@BillDate", billDate);
                command.Parameters.AddWithValue("@CustomField", affiliate);

                //dao.BeginTransaction();

                dao.ExecuteNonQuery(command);

                //dao.CommitTransaction();

                logger.Info(string.Format("/* Line {0} was processed succesfully */", lineNumber));
            }
            catch
            {
                dao.RollbackTransaction();

                logger.Info(string.Format("/* DB error on line {0} */", lineNumber));
            }            
        }

        static void InsertRawTransactionData(IDao dao, ILog logger, int lineNumber, string orderID, int? customerID, DateTime? billDate, int subscriptionID, decimal amount)
        {
            string query = @"
                INSERT INTO Import_Transactions (OrderID,CustomerID,BillDate,SubscriptionID,Amount)
                VALUES (@OrderID,@CustomerID,@BillDate,@SubscriptionID,@Amount);";

            try
            {
                MySqlCommand command = new MySqlCommand();

                command.CommandText = query;
                
                command.Parameters.AddWithValue("@OrderID", orderID);
                command.Parameters.AddWithValue("@CustomerID", customerID);
                command.Parameters.AddWithValue("@BillDate", billDate);
                command.Parameters.AddWithValue("@SubscriptionID", subscriptionID);
                command.Parameters.AddWithValue("@Amount", amount);                

                dao.BeginTransaction();

                dao.ExecuteNonQuery(command);

                dao.CommitTransaction();

                logger.Info(string.Format("/* Line {0} was processed succesfully */", lineNumber));
            }
            catch
            {
                dao.RollbackTransaction();

                logger.Info(string.Format("/* DB error on line {0} */", lineNumber));
            }
        }
    }
}




