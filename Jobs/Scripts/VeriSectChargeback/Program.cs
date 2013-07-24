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

namespace VeriSectChargeback
{
    class Program
    {
        static ILog logger = null;

        private const string query = @"
                    select * from (
                        select distinct ch.ChargeHistoryID, scb.PostDT, cbr.ReasonCode from VeriSectPost vsp
                        inner join ChargeHistoryEx ch on ch.ChargeHistoryID = vsp.ChargeHistoryID and ch.Success = 1 and ch.Amount > 0
                        inner join BillingSale bsl on bsl.ChargeHistoryID = ch.ChargeHistoryID
                        inner join SaleChargeback scb on scb.SaleID = bsl.SaleID
                        inner join ChargebackReasonCode cbr on cbr.ChargebackReasonCodeID = scb.ChargebackReasonCodeID
                        where vsp.VeriSectPostType = @veriSectPostType_Charge
                        union all
                        select distinct ch.ChargeHistoryID, scb.PostDT, cbr.ReasonCode from VeriSectPost vsp
                        inner join ChargeHistoryEx ch on ch.ChargeHistoryID = vsp.ChargeHistoryID and ch.Success = 1 and ch.Amount > 0
                        inner join UpsellSale usl on usl.ChargeHistoryID = ch.ChargeHistoryID
                        inner join SaleChargeback scb on scb.SaleID = usl.SaleID
                        inner join ChargebackReasonCode cbr on cbr.ChargebackReasonCodeID = scb.ChargebackReasonCodeID
                        where vsp.VeriSectPostType = @veriSectPostType_Charge
                    ) t
                    where not exists (select * from VeriSectPost where ChargeHistoryID = t.ChargeHistoryID and VeriSectPostType = @veriSectPostType_Chargeback)
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
                q.Parameters.Add("@veriSectPostType_Chargeback", MySqlDbType.Int32).Value = VeriSectPostTypeEnum.Chargeback;
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
                data += "&cb=1";
                data += "&cbd=" + HttpUtility.UrlEncode(trans.PostDT.Value.ToString("yyyy-MM-dd"));
                data += "&cbr=" + trans.ReasonCode;

                data = "http://upload.verisect.net/updateTrx.php" + data;
                //data = "http://localhost/gateways/msc.aspx" + data;

                WebClient wc = new WebClient();
                string response = wc.DownloadString(data);

                TrimFuel.Model.VeriSectPost post = new TrimFuel.Model.VeriSectPost();
                post.ChargeHistoryID = trans.ChargeHistoryID;
                post.VeriSectPostType = VeriSectPostTypeEnum.Chargeback;
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
        public DateTime? PostDT { get; set; }
        public int? ReasonCode { get; set; }
    }

    public class VeriSectTransViewDataProvider : EntityViewDataProvider<VeriSectTransView>
    {
        public override VeriSectTransView Load(System.Data.DataRow row)
        {
            VeriSectTransView res = new VeriSectTransView();

            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);
            if (!(row["PostDT"] is DBNull))
                res.PostDT = Convert.ToDateTime(row["PostDT"]);
            if (!(row["ReasonCode"] is DBNull))
                res.ReasonCode = Convert.ToInt32(row["ReasonCode"]);

            return res;
        }
    }
}
