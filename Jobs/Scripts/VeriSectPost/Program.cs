using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using System.Configuration;
using TrimFuel.Model.Views;
using TrimFuel.Model;
using TrimFuel.Business.Dao.EntityDataProviders.Views;
using TrimFuel.Business.Dao.EntityDataProviders;
using System.Web;
using TrimFuel.Model.Enums;
using System.Net;

namespace VeriSectPost
{
    class Program
    {
        static ILog logger = null;

        private const string query =
                    " select * from (" +
                    " select distinct b.*, c.URL as RefURL, ch.ChargeHistoryID, ch.ChargeDate, ch.Amount, ch.MerchantAccountID, sl.SaleTypeID, sl.TrackingNumber, bsl.RebillCycle as RebillNumber, bsl.ProductCode from Billing b" +
                    " inner join Campaign c on c.CampaignID = b.CampaignID" +
                    " inner join BillingSubscription bs on bs.BillingID = b.BillingID" +
                    " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID" +
                    " inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID" +
                    " inner join BillingSale bsl on bsl.ChargeHistoryID = ch.ChargeHistoryID" +
                    " inner join Sale sl on sl.SaleID = bsl.SaleID" +
                    " where ch.Amount > 0 and ch.Success = 1" +
                    " ##FILTER## " +
                    " union all" +
                    " select distinct b.*, c.URL as RefURL, ch.ChargeHistoryID, ch.ChargeDate, IfNull(chs.Amount, ch.Amount) as Amount, ch.MerchantAccountID, sl.SaleTypeID, sl.TrackingNumber, 0 as RebillNumber, u.ProductCode from Billing b" +
                    " inner join Campaign c on c.CampaignID = b.CampaignID" +
                    " inner join BillingSubscription bs on bs.BillingID = b.BillingID" +
                    " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID" +
                    " inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID" +
                    " left join ChargeHistoryExSale chs on chs.ChargeHistoryID = ch.ChargeHistoryID" +
                    " inner join UpsellSale usl on usl.ChargeHistoryID = ch.ChargeHistoryID" +
                    " inner join Upsell u on u.UpsellID = usl.UpsellID" +
                    " inner join Sale sl on sl.SaleID = usl.SaleID" +
                    " where ch.Amount > 0 and ch.Success = 1" +
                    " ##FILTER## " +
                    " ) t" +
                    " where not exists (select * from VeriSectPost where ChargeHistoryID = t.ChargeHistoryID)" +
                    " order by t.ChargeHistoryID desc" +
                    " limit @limitStart, @limitCount";

        private static MySqlCommand EPEC_36519_to_March_31_3000()
        {
            MySqlCommand q = new MySqlCommand(
                query.Replace("##FILTER##", "and b.Affiliate = 'EPEC' and b.SubAffiliate = '36519' and ch.ChargeDate <= '2011-03-31 23:59:59'")
            );
            q.Parameters.Add("@limitStart", MySqlDbType.Int32).Value = 0;
            q.Parameters.Add("@limitCount", MySqlDbType.Int32).Value = 3000;
            return q;
        }

        private static MySqlCommand EPEC_43310_1000()
        {
            MySqlCommand q = new MySqlCommand(
                query.Replace("##FILTER##", "and b.Affiliate = 'EPEC' and b.SubAffiliate = '43310'")
            );
            q.Parameters.Add("@limitStart", MySqlDbType.Int32).Value = 0;
            q.Parameters.Add("@limitCount", MySqlDbType.Int32).Value = 1000;
            return q;
        }

        private static MySqlCommand CBEC09a_CD113698_500()
        {
            MySqlCommand q = new MySqlCommand(
                query.Replace("##FILTER##", "and b.Affiliate = 'CBEC09a' and b.SubAffiliate = 'CD113698_'")
            );
            q.Parameters.Add("@limitStart", MySqlDbType.Int32).Value = 0;
            q.Parameters.Add("@limitCount", MySqlDbType.Int32).Value = 500;
            return q;
        }

        private static MySqlCommand CBEC_CD8417_1000()
        {
            MySqlCommand q = new MySqlCommand(
                query.Replace("##FILTER##", "and b.Affiliate = 'CBEC' and b.SubAffiliate = 'CD8417'")
            );
            q.Parameters.Add("@limitStart", MySqlDbType.Int32).Value = 0;
            q.Parameters.Add("@limitCount", MySqlDbType.Int32).Value = 1000;
            return q;
        }

        private static MySqlCommand REC_2000()
        {
            MySqlCommand q = new MySqlCommand(
                query.Replace("##FILTER##", "and b.Affiliate = 'REC'")
            );
            q.Parameters.Add("@limitStart", MySqlDbType.Int32).Value = 0;
            q.Parameters.Add("@limitCount", MySqlDbType.Int32).Value = 2000;
            return q;
        }

        private static MySqlCommand CBAFA_CD51188_all()
        {
            MySqlCommand q = new MySqlCommand(
                query.Replace("##FILTER##", "and b.Affiliate = 'CBAFA' and b.SubAffiliate = 'CD51188'")
            );
            q.Parameters.Add("@limitStart", MySqlDbType.Int32).Value = 0;
            q.Parameters.Add("@limitCount", MySqlDbType.Int32).Value = 100000;
            return q;
        }

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            logger = LogManager.GetLogger(typeof(Program));

            try
            {
                logger.Info("Script Start-------------------------------------------------------------------------");
                (new VeriSectTransViewDataProvider()).Register();
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

                int limitStart = int.Parse(ConfigurationSettings.AppSettings["start"]);
                int limitCount = int.Parse(ConfigurationSettings.AppSettings["count"]);        

                IList<VeriSectTransView> transactions = new List<VeriSectTransView>();
                //transactions = transactions.Concat(dao.Load<VeriSectTransView>(EPEC_36519_to_March_31_3000())).ToList();
                //transactions = transactions.Concat(dao.Load<VeriSectTransView>(EPEC_43310_1000())).ToList();
                transactions = transactions.Concat(dao.Load<VeriSectTransView>(CBEC09a_CD113698_500())).ToList();
                //transactions = transactions.Concat(dao.Load<VeriSectTransView>(CBEC_CD8417_1000())).ToList();
                //transactions = transactions.Concat(dao.Load<VeriSectTransView>(REC_2000())).ToList();
                transactions = transactions.Concat(dao.Load<VeriSectTransView>(CBAFA_CD51188_all())).ToList();

                foreach (VeriSectTransView trans in transactions)
                {
                    if (limitCount == 0)
                    {
                        break;
                    }
                    SendTransaction(trans, dao);
                    limitCount--;
                }

                logger.Info("Script End-------------------------------------------------------------------------");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        static void SendTransaction(VeriSectTransView trans, IDao dao)
        {
            try
            {
                logger.Info(trans.ChargeHistoryID.ToString());

                string data = "";
                data += "?un=" + "tri";
                data += "&pw=" + "Vo8CdJ198sJBHJH";
                data += "&cid=" + "Triangle";
                data += "&cc=" + trans.Billing.CreditCardCnt.DecryptedCreditCardLeft6;
                data += "&id=" + trans.ChargeHistoryID;
                data += "&fn=" + HttpUtility.UrlEncode(trans.Billing.FirstName);
                data += "&ln=" + HttpUtility.UrlEncode(trans.Billing.LastName);
                data += "&addr=" + HttpUtility.UrlEncode(trans.Billing.Address1 ?? "" + (!string.IsNullOrEmpty(trans.Billing.Address2) ? trans.Billing.Address2 : ""));
                data += "&city=" + HttpUtility.UrlEncode(trans.Billing.City);
                data += "&state=" + HttpUtility.UrlEncode(trans.Billing.State);
                data += "&zip=" + HttpUtility.UrlEncode(trans.Billing.Zip);
                data += "&ph=" + HttpUtility.UrlEncode(trans.Billing.Phone);
                data += "&email=" + HttpUtility.UrlEncode(trans.Billing.Email);
                data += "&ip=" + HttpUtility.UrlEncode(trans.Billing.IP);
                data += "&ot=" + HttpUtility.UrlEncode(trans.ChargeDate.Value.ToString("HH:mm:ss"));
                data += "&od=" + HttpUtility.UrlEncode(trans.ChargeDate.Value.ToString("yyyy-MM-dd"));
                data += "&recuring=" + (trans.SaleTypeID == SaleTypeEnum.Upsell ? "0" : "1");
                data += "&rebill=" + (trans.RebillNumber != null ? trans.RebillNumber.Value.ToString() : "0");
                data += "&expm=" + (trans.Billing.ExpMonth != null ? trans.Billing.ExpMonth.Value.ToString() : "");
                data += "&expy=" + ExpYear4Digits(trans.Billing.ExpYear);
                data += "&amount=" + HttpUtility.UrlEncode(trans.Amount.Value.ToString());
                data += "&pid=" + HttpUtility.UrlEncode(trans.ProductCode);
                data += "&tid=" + HttpUtility.UrlEncode(trans.TrackingNumber);
                data += "&aid=" + HttpUtility.UrlEncode(trans.Billing.Affiliate);
                data += "&ref=" + HttpUtility.UrlEncode(trans.RefURL);

                data += "&Opt_MerchantAccountID=" + (trans.MerchantAccountID != null ? trans.MerchantAccountID.ToString() : "");
                data += "&Opt_SubAffiliate=" + HttpUtility.UrlEncode(trans.Billing.SubAffiliate);
                //data += "&Opt_CVV=" + HttpUtility.UrlEncode(trans.Billing.CVV);

                data = "http://upload.verisect.net/preauth.php" + data;
                //data = "http://localhost/gateways/msc.aspx" + data;

                WebClient wc = new WebClient();
                string response = wc.DownloadString(data);

                TrimFuel.Model.VeriSectPost post = new TrimFuel.Model.VeriSectPost();
                post.ChargeHistoryID = trans.ChargeHistoryID;
                post.VeriSectPostType = VeriSectPostTypeEnum.Charge;
                post.Request = data;
                post.Response = response;
                dao.Save<TrimFuel.Model.VeriSectPost>(post);

                //logger.Info("\n" + response);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        static string ExpYear4Digits(int? year)
        {
            if (year == null)
            {
                return string.Empty;
            }
            if (year.Value < 1000)
            {
                year += 2000;
            }
            return year.Value.ToString();
        }
    }

    public class VeriSectTransView : EntityView
    {
        public Billing Billing { get; set; }

        public long? ChargeHistoryID { get; set; }
        public DateTime? ChargeDate { get; set; }
        public decimal? Amount { get; set; }
        public int? MerchantAccountID { get; set; }
        
        public short? SaleTypeID { get; set; }
        public string TrackingNumber { get; set; }

        public int? RebillNumber { get; set; }

        public string RefURL { get; set; }

        public string ProductCode { get; set; }

    }

    public class VeriSectTransViewDataProvider : EntityViewDataProvider<VeriSectTransView>
    {
        public override VeriSectTransView Load(System.Data.DataRow row)
        {
            VeriSectTransView res = new VeriSectTransView();

            res.Billing = EntityDataProvider<Billing>.CreateProvider().Load(row);

            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);
            if (!(row["ChargeDate"] is DBNull))
                res.ChargeDate = Convert.ToDateTime(row["ChargeDate"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            if (!(row["MerchantAccountID"] is DBNull))
                res.MerchantAccountID = Convert.ToInt32(row["MerchantAccountID"]);

            if (!(row["SaleTypeID"] is DBNull))
                res.SaleTypeID = Convert.ToInt16(row["SaleTypeID"]);
            if (!(row["TrackingNumber"] is DBNull))
                res.TrackingNumber = Convert.ToString(row["TrackingNumber"]);

            if (!(row["RebillNumber"] is DBNull))
                res.RebillNumber = Convert.ToInt32(row["RebillNumber"]);

            if (!(row["RefURL"] is DBNull))
                res.RefURL = Convert.ToString(row["RefURL"]);

            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);

            return res;
        }
    }
}
