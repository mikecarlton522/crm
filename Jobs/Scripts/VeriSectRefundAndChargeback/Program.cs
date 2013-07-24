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

namespace VeriSectRefund
{
    class Program
    {
        static ILog logger = null;

        private const string query = @"
                    select * from (
                        select distinct ch.ChargeHistoryID, refCh.ChargeHistoryID as RefundChargeHistoryID, refCh.ChargeDate as RefundChargeDate, ch.TransactionNumber, refCh.TransactionNumber as RefundTransactionNumber, ch.Amount, refCh.Amount as RefundAmount from VeriSectPost vsp
                        inner join ChargeHistoryEx ch on ch.ChargeHistoryID = vsp.ChargeHistoryID and ch.Success = 1 and ch.Amount > 0
                        inner join BillingSale bsl on bsl.ChargeHistoryID = ch.ChargeHistoryID
                        inner join SaleRefund sr on sr.SaleID = bsl.SaleID
                        inner join ChargeHistoryEx refCh on refCh.ChargeHistoryID = sr.ChargeHistoryID and refCh.Success = 1 and refCh.Amount < 0 and refCh.ChargeTypeID = 2
                        where vsp.VeriSectPostType = @veriSectPostType_Charge
                        union all
                        select distinct ch.ChargeHistoryID, refCh.ChargeHistoryID as RefundChargeHistoryID, refCh.ChargeDate as RefundChargeDate, ch.TransactionNumber, refCh.TransactionNumber as RefundTransactionNumber, ch.Amount, refCh.Amount as RefundAmount from VeriSectPost vsp
                        inner join ChargeHistoryEx ch on ch.ChargeHistoryID = vsp.ChargeHistoryID and ch.Success = 1 and ch.Amount > 0
                        inner join UpsellSale usl on usl.ChargeHistoryID = ch.ChargeHistoryID
                        inner join SaleRefund sr on sr.SaleID = usl.SaleID
                        inner join ChargeHistoryEx refCh on refCh.ChargeHistoryID = sr.ChargeHistoryID and refCh.Success = 1 and refCh.Amount < 0 and refCh.ChargeTypeID = 2
                        where vsp.VeriSectPostType = @veriSectPostType_Charge
                    ) t
                    where not exists (select * from VeriSectPost where ChargeHistoryID = t.RefundChargeHistoryID and VeriSectPostType = @veriSectPostType_Refund)
                    order by t.ChargeHistoryID desc
                    limit @limitStart, @limitCount";

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
                MySqlCommand q = new MySqlCommand(
                    query
                );
                q.Parameters.Add("@veriSectPostType_Charge", MySqlDbType.Int32).Value = VeriSectPostTypeEnum.Charge;
                q.Parameters.Add("@veriSectPostType_Refund", MySqlDbType.Int32).Value = VeriSectPostTypeEnum.Refund;
                q.Parameters.Add("@limitStart", MySqlDbType.Int32).Value = 0;
                q.Parameters.Add("@limitCount", MySqlDbType.Int32).Value = 1000;

                transactions = dao.Load<VeriSectTransView>(q);
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
                data += "&id=" + trans.ChargeHistoryID;
                data += "&rf=1";
                data += "&rd=" + HttpUtility.UrlEncode(trans.RefundChargeDate.Value.ToString("yyyy-MM-dd"));

                data = "http://upload.verisect.net/updateTrx.php" + data;
                //data = "http://localhost/gateways/msc.aspx" + data;

                WebClient wc = new WebClient();
                string response = wc.DownloadString(data);

                TrimFuel.Model.VeriSectPost post = new TrimFuel.Model.VeriSectPost();
                post.ChargeHistoryID = trans.RefundChargeHistoryID;
                post.VeriSectPostType = VeriSectPostTypeEnum.Refund;
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
        public long? ChargeHistoryID { get; set; }
        public long? RefundChargeHistoryID { get; set; }
        public DateTime? RefundChargeDate { get; set; }
    }

    public class VeriSectTransViewDataProvider : EntityViewDataProvider<VeriSectTransView>
    {
        public override VeriSectTransView Load(System.Data.DataRow row)
        {
            VeriSectTransView res = new VeriSectTransView();

            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);
            if (!(row["RefundChargeHistoryID"] is DBNull))
                res.RefundChargeHistoryID = Convert.ToInt64(row["RefundChargeHistoryID"]);
            if (!(row["RefundChargeDate"] is DBNull))
                res.RefundChargeDate = Convert.ToDateTime(row["RefundChargeDate"]);

            return res;
        }
    }
}
