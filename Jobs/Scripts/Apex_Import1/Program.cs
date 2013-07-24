using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Configuration;
using TrimFuel.Business.Dao;
using LumenWorks.Framework.IO.Csv;
using System.IO;
using TrimFuel.Encrypting;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Containers;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;

namespace Apex_Import1
{
    class Program
    {
        private const string FILE_NAME = @"Transactions.csv";
        private const int CAMPAIGN_ID = 10000;
        private const int PRODUCT_ID = 1;
        private static IList<Billing> _billingList = null;
        static Dictionary<string, string> MIDs = new Dictionary<string, string>();
        private static Dictionary<long?, string> _decryptedCards = null;

        static void fillDictionary()
        {
            MIDs.Add("108901", "GS Saver");
            MIDs.Add("109060", "DE Saver");
            MIDs.Add("109061", "GS Saver");
        }

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            int limit = int.Parse(ConfigurationSettings.AppSettings["limit"]);
            int lineNumber = 1;
            fillDictionary();
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

        private static void ProcessLine(IDao dao, ILog logger, CsvReader csv, int lineNumber)
        {
            try
            {

                string type = HasValue(csv[6]) ? csv[6] : null;

                if (!(type == "Credit Card Sale" || type == "Credit Card Refund" || type == "CCConfitmation"))
                    return;

                dao.BeginTransaction();
                MySqlCommand q = null;
                string transactionID = HasValue(csv[0]) ? csv[0] : null;
                string mid = HasValue(csv[1]) ? csv[1] : null;

                DateTime transactionDateTmp = HasValue(csv[2]) ? (DateTime.TryParse(csv[2], out transactionDateTmp) ? transactionDateTmp : DateTime.MinValue) : DateTime.MinValue;
                DateTime? transactionDate = transactionDateTmp;
                if (transactionDateTmp == DateTime.MinValue)
                    transactionDate = null;

                string creditCard = HasValue(csv[3]) ? csv[3] : null;
                string authID = HasValue(csv[4]) ? csv[4] : null;
                decimal? amount = Utility.TryGetDecimal(HasValue(csv[5]) ? csv[5].Replace("$", "").Replace("(", "").Replace(")", "") : null);
                string status = HasValue(csv[7]) ? csv[7] : null;
                string fullName = HasValue(csv[8]) ? csv[8] : null;

                amount = type == "Credit Card Refund" ? -amount : amount;

                var customer = FindCustomer(logger, dao, creditCard);

                if (customer == null)
                {
                    logger.Info(string.Format("/* Can't find customer line {0} */", lineNumber));
                    dao.RollbackTransaction();
                    return;
                }

                var subscription = FindSubscription(logger, dao, MIDs[mid]);

                if (amount > 0 && amount < 2 && status != "Approved" && type != "Credit Card Refund")
                {
                    int? count = 0;
                    q = new MySqlCommand(@"SELECT Count(*) as Value FROM FailedChargeHistory ch 
                                           inner join FailedChargeHistoryDetails chDet on ch.FailedChargeHistoryID=chDet.FailedChargeHistoryID
                                           WHERE ch.BillingID=@BillingID  and ch.Amount=@Amount
                                           and DATE_FORMAT(@ChargeDate, '%Y-%m-%d %h:%i') BETWEEN DATE_FORMAT(ch.ChargeDate, '%Y-%m-%d %h:%i') AND DATE_FORMAT(ch.ChargeDate + INTERVAL 1 MINUTE, '%Y-%m-%d %h:%i')
                                           and chDet.SubscriptionID=@SubscriptionID");
                    q.Parameters.Add("@BillingID", MySqlDbType.Int32).Value = customer.BillingID;
                    q.Parameters.Add("@SubscriptionID", MySqlDbType.Int32).Value = subscription.SubscriptionID;
                    q.Parameters.Add("@ChargeDate", MySqlDbType.DateTime).Value = transactionDate;
                    q.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = amount;
                    count = dao.Load<View<int>>(q).FirstOrDefault().Value;

                    if (count == null || count == 0)
                    {
                        logger.Info(string.Format("/* Add FCH line {0} */", lineNumber));

                        var fch = new FailedChargeHistoryDetails()
                        {
                            Amount = amount,
                            BillingID = customer.BillingID,
                            MerchantAccountID = 54,
                            SaleTypeID = 1,
                            Success = false,
                            ChargeDate = transactionDate,
                            Response = string.Empty,
                            SKU = subscription.SKU2,
                            SubscriptionID = subscription.SubscriptionID
                        };
                        dao.Save<FailedChargeHistoryDetails>(fch);
                    }
                }
                else
                {

                    var bs = FindBillingSubscription(logger, dao, subscription, customer.BillingID);

                    ChargeHistoryEx ch = null;
                    if (bs == null)
                    {
                        bs = CreateBillingSubscription(logger, dao, subscription, customer.BillingID, transactionDate);
                    }
                    else
                    {
                        if (bs.CreateDT > transactionDate)
                            bs.CreateDT = transactionDate;
                        if (bs.LastBillDate < transactionDate)
                            bs.LastBillDate = transactionDate;
                        dao.Save<BillingSubscription>(bs);

                        q = new MySqlCommand(@"SELECT * FROM ChargeHistoryEx ch WHERE ch.BillingSubscriptionID=@BillingSubscriptionID and ch.Success=@Success and ch.Amount=@Amount
                                and DATE_FORMAT(@ChargeDate, '%Y-%m-%d %h:%i') BETWEEN DATE_FORMAT(ch.ChargeDate, '%Y-%m-%d %h:%i') AND DATE_FORMAT(ch.ChargeDate + INTERVAL 1 MINUTE, '%Y-%m-%d %h:%i')");
                        q.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = bs.BillingSubscriptionID;
                        q.Parameters.Add("@ChargeDate", MySqlDbType.DateTime).Value = transactionDate;
                        q.Parameters.Add("@Amount", MySqlDbType.Decimal).Value = amount;
                        q.Parameters.Add("@Success", MySqlDbType.Bit).Value = status == "Approved" ? true : false;
                        ch = dao.Load<ChargeHistoryEx>(q).FirstOrDefault();
                    }

                    if (ch == null)
                    {
                        var chDet = CreateChargeDetails(dao, amount, bs.BillingSubscriptionID, transactionDate, subscription.SKU2, type, authID, status);
                        logger.Info(string.Format("/* Add CHEX line {0} */", lineNumber));
                        if (type != "Credit Card Refund" && chDet.Success == true)
                            CreateBillingSale(dao, logger, chDet);
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Info(string.Format("/* CSV error on line {0} */", lineNumber));
                //logger.Error(typeof(Program), ex);
                dao.RollbackTransaction();
            }
        }

        static Billing FindCustomer(ILog logger, IDao dao, string creditCard)
        {
            Billing res = null;
            int countOfStars = creditCard.LastIndexOf('*') - 5;

            if (_billingList == null)
            {
                MySqlCommand q = new MySqlCommand(@" 
                                    select b.* from Billing b"
                                );
                _billingList = dao.Load<Billing>(q);
                _decryptedCards = _billingList.Select(u => new KeyValuePair<long?, string>(u.BillingID, u.CreditCardCnt.DecryptedCreditCard)).ToDictionary(x => x.Key, x => x.Value);
            }

            foreach (Billing match in _billingList)
            {
                string ccToCompare = match.CreditCard;
                if (ccToCompare.Length >= 12)
                    ccToCompare = ccToCompare.Remove(6, countOfStars).Insert(6, GetStarsString(countOfStars));

                if (creditCard == ccToCompare)
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
                        ccToCompare = _decryptedCards[match.BillingID];
                        if (ccToCompare.Length >= 12)
                            ccToCompare = ccToCompare.Remove(6, countOfStars).Insert(6, GetStarsString(countOfStars));

                        matchCC = creditCard == ccToCompare;
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

            return res;
        }

        static Subscription FindSubscription(ILog logger, IDao dao, string productName)
        {
            Subscription res = null;

            //First search without CC
            MySqlCommand q = new MySqlCommand(@" 
                    select s.* from Subscription s
                    Where s.ProductName = @ProductName"
            );
            q.Parameters.Add("@ProductName", MySqlDbType.VarChar).Value = productName;
            res = dao.Load<Subscription>(q).FirstOrDefault();

            return res;
        }

        static BillingSubscription FindBillingSubscription(ILog logger, IDao dao, Subscription subscription, long? billingID)
        {
            MySqlCommand q = new MySqlCommand("select * from BillingSubscription where BillingID = @BillingID and SubscriptionID = @SubscriptionID");
            q.Parameters.AddWithValue("@BillingID", billingID);
            q.Parameters.AddWithValue("@SubscriptionID", subscription.SubscriptionID);
            return dao.Load<BillingSubscription>(q).FirstOrDefault();
        }

        static BillingSubscription CreateBillingSubscription(ILog logger, IDao dao, Subscription subscription, long? billingID, DateTime? transactionDT)
        {
            BillingSubscription bs = new BillingSubscription();
            bs.BillingID = billingID;
            bs.SubscriptionID = subscription.SubscriptionID;
            bs.CreateDT = transactionDT;
            bs.LastBillDate = transactionDT;
            bs.StatusTID = 0;
            bs.CustomerReferenceNumber = Utility.RandomString(new Random(), 6);
            dao.Save<BillingSubscription>(bs);

            return bs;
        }

        static void CreateBillingSale(IDao dao, ILog logger, ChargeDetails cd)
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
                billingSale.RebillCycle = CalculateRebillCycle(logger, dao, cd.BillingSubscriptionID, cd.ChargeDate);
            }
            billingSale.ProductCode = cd.SKU;
            billingSale.Quantity = 1;
            billingSale.ChargeHistoryID = cd.ChargeHistoryID;
            billingSale.CreateDT = cd.ChargeDate;

            dao.Save<BillingSale>(billingSale);

            if (billingSale.SaleTypeID == 2)
            {
                MySqlCommand q = new MySqlCommand(@"
                                                    Update BillingSale bs  
                                                    inner join Sale s on s.SaleID=bs.SaleID
                                                    SET bs.RebillCycle = bs.RebillCycle + 1
                                                    WHERE s.CreateDT > @CreadeDT
                                                    and bs.BillingSubscriptionID=@BillingSubscriptionID
                                                    and s.SaleTypeID=2;
                                                  ");
                q.Parameters.Add("@CreadeDT", MySqlDbType.DateTime).Value = cd.ChargeDate;
                q.Parameters.Add("@BillingSubscriptionID", MySqlDbType.Int32).Value = cd.BillingSubscriptionID;
                dao.ExecuteNonQuery(q);
            }
        }

        static ChargeDetails CreateChargeDetails(IDao dao, decimal? amount, long? billingSubscriptionID, DateTime? chargeDate, string sku, string type, string authID, string status)
        {
            ChargeDetails chargeDetails = new ChargeDetails();

            chargeDetails.Amount = amount;
            chargeDetails.BillingSubscriptionID = (int)billingSubscriptionID;
            chargeDetails.ChargeDate = chargeDate;
            chargeDetails.ChildMID = "Import";
            chargeDetails.ChargeTypeID = type == "Credit Card Refund" ? 2 : 1;
            chargeDetails.MerchantAccountID = 54;
            chargeDetails.SaleTypeID = amount < 3 ? (short)1 : (short)2;
            chargeDetails.SKU = sku;
            chargeDetails.Success = status == "Approved" ? true : false;
            chargeDetails.TransactionNumber = string.Empty;
            chargeDetails.AuthorizationCode = authID;

            dao.Save<ChargeDetails>(chargeDetails);

            return chargeDetails;
        }

        static int? CalculateRebillCycle(ILog logger, IDao dao, int? bsID, DateTime? createDate)
        {
            int? res = null;
            MySqlCommand q = new MySqlCommand(@"
                    select count(*) as Value from BillingSale bsl
                    inner join Sale sl on sl.SaleID = bsl.SaleID
                    where sl.SaleTypeID = 2 and bsl.BillingSubscriptionID = @billingSubscriptionID and sl.CreateDT<@CreateDT
                ");
            q.Parameters.Add("@billingSubscriptionID", MySqlDbType.Int32).Value = bsID;
            q.Parameters.Add("@CreateDT", MySqlDbType.DateTime).Value = createDate;

            res = dao.Load<View<int>>(q).FirstOrDefault().Value;
            res += 1;
            return res;
        }

        static string GetStarsString(int length)
        {
            string res = string.Empty;
            while (res.Length != length)
                res += "*";
            return res;
        }
    }
}
