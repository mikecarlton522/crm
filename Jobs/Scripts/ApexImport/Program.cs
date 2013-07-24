using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Configuration;
using TrimFuel.Business.Dao;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using System.Text.RegularExpressions;
using TrimFuel.Encrypting;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Model.Containers;

namespace ApexImport
{
    class Program
    {
        private const string FILE_NAME = @"Customers.csv";
        private const int CAMPAIGN_ID = 10000;
        private const int PRODUCT_ID = 1;
        private static IList<Billing> _billingList = null;
        private static Dictionary<long?, string> _decryptedCards = null;

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            int limit = int.Parse(ConfigurationSettings.AppSettings["limit"]);
            int lineNumber = 1;
            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                using (CsvReader csv = new CsvReader(new StreamReader(FILE_NAME), true))
                {
                    while (csv.ReadNextRecord())
                    {
                        if (limit <= 0)
                        {
                            break;
                        }
                        ProcessLine(dao, logger, csv, lineNumber);
                        Console.WriteLine(lineNumber);
                        lineNumber++;
                        limit--;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
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

        private static void ProcessLine(IDao dao, ILog logger, CsvReader csv, int line)
        {
            try
            {
                CC cc = new CC();

                // billing
                string firstName = HasValue(csv[1]) ? csv[1] : null;
                string lastName = HasValue(csv[2]) ? csv[2] : null;
                string address1 = HasValue(csv[3]) ? csv[3] : null;
                string address2 = HasValue(csv[4]) ? csv[4] : null;
                string city = HasValue(csv[5]) ? csv[5] : null;
                string state = HasValue(csv[6]) ? csv[6] : null;
                string zip = HasValue(csv[7]) ? csv[7] : null;
                string country = "US";

                // shipping
                string s_address1 = HasValue(csv[8]) ? csv[8] : null;
                string s_address2 = HasValue(csv[9]) ? csv[9] : null;
                string s_city = HasValue(csv[10]) ? csv[10] : null;
                string s_state = HasValue(csv[11]) ? csv[11] : null;
                string s_postcode = HasValue(csv[12]) ? csv[12] : null;

                string phone = HasValue(csv[13]) ? csv[13] : null;
                string email = HasValue(csv[17]) ? csv[17] : null;

                string url = "Import3";
                string affiliate = string.Empty;

                string creditcard = HasValue(csv[14]) ? csv[14] : null;
                if (creditcard != null)
                {
                    creditcard = Regex.Replace(creditcard, "[^0-9]", "");
                    creditcard = cc.GetEncrypted(creditcard);
                }

                string cvv = HasValue(csv[16]) ? csv[16] : null;

                int expMonthTmp = HasValue(csv[15]) && csv[15].Length >= 4 ? (int.TryParse(csv[15].Substring(0,2), out expMonthTmp) ? expMonthTmp : 0) : 0;
                int? expMonth = expMonthTmp;
                if (expMonthTmp == 0)
                    expMonth = null;

                int expYearTmp = HasValue(csv[15]) && csv[15].Length >= 4 ? (int.TryParse(csv[15].Substring(2,2), out expMonthTmp) ? expMonthTmp : 0) : 0;
                int? expYear = expYearTmp;
                if (expYearTmp == 0)
                    expYear = null;

                DateTime createDateTmp = DateTime.Now;
                DateTime? createDate = createDateTmp;
                if (createDateTmp == DateTime.MinValue)
                    createDate = null;

                InsertCustomerData(dao, logger, creditcard, firstName, lastName, address1, address2, affiliate, city, country,
                        email, phone, state, url, zip, createDate, cvv, expMonth, expYear, line);
            }
            catch
            {
                logger.Info(string.Format("/* CSV error on line {0} */", line));
            }
        }

        private static void InsertCustomerData(IDao dao, ILog logger, string cc, string firstName, string lastName, string address1, string address2,
                            string affiliate, string city, string country, string email, string phone, string state, string url, string zip, DateTime? createDate,
                            string cvv, int? expMonth, int? expYear, int line)
        {
            try
            {
                var isExists = IsExists(logger, dao, cc);
                if (isExists)
                    return;

                logger.Info(string.Format("/* Add line {0} */", line));
                _billingList = null;

                dao.BeginTransaction();
                Registration registration = new Registration()
                {
                    Address1 = address1,
                    Address2 = address2,
                    Affiliate = affiliate,
                    CampaignID = null,
                    City = city,
                    CreateDT = createDate,
                    Email = email,
                    FirstName = firstName,
                    IP = null,
                    LastName = lastName,
                    Phone = phone,
                    State = state,
                    SubAffiliate = null,
                    URL = url,
                    Zip = zip
                };
                dao.Save<Registration>(registration);

                RegistrationInfo regInfo = new RegistrationInfo()
                {
                    RegistrationID = registration.RegistrationID,
                    Country = country
                };
                dao.Save<RegistrationInfo>(regInfo);

                Billing billing = new Billing()
                {
                    Address1 = address1,
                    Address2 = address2,
                    Affiliate = affiliate,
                    CampaignID = null,
                    City = city,
                    CreateDT = createDate,
                    Email = email,
                    FirstName = firstName,
                    IP = null,
                    LastName = lastName,
                    Phone = phone,
                    State = state,
                    SubAffiliate = null,
                    URL = url,
                    Zip = zip,
                    Country = country,
                    CreditCard = cc,
                    CVV = cvv,
                    ExpMonth = expMonth,
                    ExpYear = expYear,
                    RegistrationID = registration.RegistrationID,
                };
                billing.PaymentTypeID = billing.CreditCardCnt.TryGetCardType();
                dao.Save<Billing>(billing);

                dao.CommitTransaction();
            }
            catch
            {
                dao.RollbackTransaction();
                throw;
            }
        }

        static bool IsExists(ILog logger, IDao dao, string creditCard)
        {
            bool res = false;
            string decrCC = new CreditCard(creditCard).DecryptedCreditCard;

            try
            {
                if (_billingList == null)
                {
                    MySqlCommand q = new MySqlCommand(@"SELECT * FROM Billing");
                    _billingList = dao.Load<Billing>(q);
                    _decryptedCards = _billingList.Select(u => new KeyValuePair<long?, string>(u.BillingID, u.CreditCardCnt.DecryptedCreditCard)).ToDictionary(x => x.Key, x => x.Value);
                }

                foreach (Billing match in _billingList)
                {
                    if (creditCard == match.CreditCard)
                    {
                        //Exact match
                        res = true;
                        break;
                    }
                    else
                    {
                        bool matchCC = false;
                        try
                        {
                            matchCC = (decrCC == _decryptedCards[match.BillingID]);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("ERROR: Can't decrypt CC of match.", ex);
                        }
                        if (matchCC)
                        {
                            res = true;
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
