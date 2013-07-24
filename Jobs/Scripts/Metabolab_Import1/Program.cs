using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using System.IO;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Containers;
using TrimFuel.Business.Gateways.NetworkMerchants;
using TrimFuel.Business.Gateways;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace Metabolab_Import1
{
    class Program
    {
        private const string FILE_NAME = @"FitDiet_all.csv";
        private const int CAMPAIGN_ID = 10000;
        private const int FAKE_MID_ID = 36;

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));

            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                string[] lines = File.ReadAllLines(FILE_NAME);
                foreach (string line in lines)
                {
                    ProcessLine(dao, logger, line);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        static string[] DELIMITERS = { "," };
        private static void ProcessLine(IDao dao, ILog logger, string line)
        {
            string internalID = null;
            string createDT = null;
            try
            {
                dao.BeginTransaction();

                string[] fields = line.Split(DELIMITERS, StringSplitOptions.None);
                if (fields.Length < 104)
                {
                    throw new Exception(string.Format("Can't process row({0}). Insufficient fields number({1})", line, fields.Length));
                }

                internalID = GetString(fields[2]);
                createDT = fields[0] + " " + fields[1];

                AssertigyMID importMID = dao.Load<AssertigyMID>(FAKE_MID_ID);
                if (importMID == null)
                {
                    throw new Exception("Can't find MID(" + FAKE_MID_ID.ToString() + ") for import.");
                }

                Registration r = new Registration();
                r.CampaignID = CAMPAIGN_ID;
                r.FirstName = GetString(fields[19]);
                r.LastName = GetString(fields[20]);
                r.Address1 = GetString(fields[21]);
                r.Address2 = GetString(fields[22]);
                r.City = GetString(fields[23]);
                r.State = GetString(fields[24]);
                r.Zip = GetString(fields[25]);
                r.Phone = GetString(fields[17]);
                r.Email = GetString(fields[18]);

                r.CreateDT = GetDateTime(createDT);
                r.IP = null;
                r.Affiliate = null;
                r.SubAffiliate = null;

                Billing b = new Billing();
                b.CampaignID = CAMPAIGN_ID;
                b.FirstName = GetString(fields[8]);
                b.LastName = GetString(fields[9]);
                b.Address1 = GetString(fields[11]);
                b.Address2 = GetString(fields[12]);
                b.City = GetString(fields[13]);
                b.State = GetString(fields[14]);
                b.Zip = GetString(fields[15]);
                b.Country = "US";
                b.Phone = GetString(fields[17]);
                b.Email = GetString(fields[18]);
                switch (GetString(fields[29]))
	            {
                    case "A":
                        b.PaymentTypeID = PaymentTypeEnum.AmericanExpress;
                        break;
                    case "V":
                        b.PaymentTypeID = PaymentTypeEnum.Visa;
                        break;
                    case "M":
                        b.PaymentTypeID = PaymentTypeEnum.Mastercard;
                        break;
                    case "D":
                        b.PaymentTypeID = PaymentTypeEnum.Discover;
                        break;
	            }
                b.CreditCard = (new CreditCard(GetString(fields[30]))).EncryptedCreditCard;
                b.CVV = GetString(fields[32]);
                b.ExpMonth = int.Parse(fields[31].Substring(0, 2));
                b.ExpYear = 2000 + int.Parse(fields[31].Substring(2, 2));

                b.CreateDT = GetDateTime(createDT);
                b.IP = null;
                b.Affiliate = null;
                b.SubAffiliate = null;

                BillingExternalInfo bInfo = new BillingExternalInfo();
                bInfo.InternalID = internalID;

                dao.Save<Registration>(r);
                b.RegistrationID = r.RegistrationID;
                dao.Save<Billing>(b);
                bInfo.BillingID = b.BillingID;
                dao.Save<BillingExternalInfo>(bInfo);

                const int salesStartIndex = 44;
                const int salesFieldsCount = 6;
                int salesCount = 0;
                for (int sIndex = 0; sIndex < 10; sIndex++)
                {
                    string sName = GetString(fields[salesStartIndex + 0 + sIndex * salesFieldsCount]);
                    decimal? sAmount = GetDecimal(fields[salesStartIndex + 3 + sIndex * salesFieldsCount]);
                    decimal? sShippingAmount = GetDecimal(fields[salesStartIndex + 5 + sIndex * salesFieldsCount]);
                    if (!string.IsNullOrEmpty(sName))
                    {
                        if (sAmount == null)
                        {
                            throw new Exception("Can't determine Amount for Sale #" + (sIndex + 1).ToString() + "");
                        }

                        if (sShippingAmount == null)
                        {
                            throw new Exception("Can't determine Shipping Amount for Sale #" + (sIndex + 1).ToString() + "");
                        }

                        if (sAmount.Value <= 0M)
                        {
                            throw new Exception("Invalid Amount(" + sAmount.Value.ToString() + ") for Sale #" + (sIndex + 1).ToString() + "");
                        }

                        if (sShippingAmount.Value < 0M)
                        {
                            throw new Exception("Invalid Shipping Amount(" + sShippingAmount.Value.ToString() + ") for Sale #" + (sIndex + 1).ToString() + "");
                        }

                        MySqlCommand q = new MySqlCommand("select * from Subscription where DisplayName=@name");
                        q.Parameters.Add("@name", MySqlDbType.VarChar).Value = sName;

                        Subscription s = dao.Load<Subscription>(q).FirstOrDefault();
                        if (s == null)
                        {
                            throw new Exception("Can't determine Subscription(" + sName + ")");
                        }

                        //Create BillingSubscription, BillingSale, Paygea, ChargeHistoryEx                        
                        BillingSubscription bs;
                        bs = new BillingSubscription();
                        bs.CreateDT = b.CreateDT;
                        bs.LastBillDate = b.CreateDT;
                        //bs.NextBillDate = (s.InitialInterim != null && s.Recurring != null && s.Recurring.Value) ?
                        //    b.CreateDT.Value.AddDays(s.InitialInterim.Value) :
                        //    new DateTime(2020, 12, 31);
                        bs.NextBillDate = new DateTime(2011, 11, 23);
                        bs.StatusTID = BillingSubscriptionStatusEnum.Active;
                        //bs.SKU = s.SKU2;
                        bs.SubscriptionID = s.SubscriptionID;
                        bs.BillingID = b.BillingID;
                        dao.Save<BillingSubscription>(bs);

                        BusinessError<GatewayResult> result = new BusinessError<GatewayResult>();
                        result.State = BusinessErrorState.Success;
                        result.ReturnValue = new GatewayResult();
                        result.ReturnValue.ResponseParams = new NetworkMerchantsGatewayResponseParams("authcode=&transactionid=");
                        result.ReturnValue.Request = "Data import.";
                        result.ReturnValue.Response = "Data import.";

                        Set<Paygea, ChargeHistoryEx> log = new SaleService().ChargeLogging(result, b, bs, s.ProductCode, SaleTypeEnum.Billing, importMID, ChargeTypeEnum.Charge, sAmount.Value + sShippingAmount.Value, null);
                        log.Value1.CreateDT = b.CreateDT;
                        dao.Save<Paygea>(log.Value1);
                        log.Value2.ChargeDate = b.CreateDT;
                        dao.Save<ChargeHistoryEx>(log.Value2);


                        BillingSale sale = new BillingSale();
                        sale.SaleTypeID = SaleTypeEnum.Billing;
                        sale.TrackingNumber = null;
                        sale.CreateDT = b.CreateDT;
                        sale.NotShip = false;
                        sale.BillingSubscriptionID = bs.BillingSubscriptionID;
                        sale.RebillCycle = 0; //TODO ?
                        sale.ProductCode = s.ProductCode;
                        sale.Quantity = s.Quantity;
                        sale.PaygeaID = log.Value1.PaygeaID;
                        sale.ChargeHistoryID = log.Value2.ChargeHistoryID;
                        dao.Save<BillingSale>(sale);

                        salesCount++;
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(ex);
                if (!string.IsNullOrEmpty(internalID))
                {
                    logger.Error("InternalID(" + internalID + ") CreateDT(" + createDT + ")");
                }
                logger.Error(ex);
            }
        }

        private static string GetString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }
            return value.Trim('"').Replace(@"\N", "");
        }

        private static DateTime GetDateTime(string value)
        {
            string dateTime = GetString(value);
            DateTime res = DateTime.Today;
            if (!string.IsNullOrEmpty(dateTime))
            {
                DateTime.TryParse(dateTime, out res);
            }
            return res;
        }

        private static Decimal? GetDecimal(string value)
        {
            string decimalValue = GetString(value);
            Decimal temp = 0M;
            if (!string.IsNullOrEmpty(decimalValue) && Decimal.TryParse(decimalValue, out temp))
            {
                return temp;
            }
            return null;
        }
    }
}
