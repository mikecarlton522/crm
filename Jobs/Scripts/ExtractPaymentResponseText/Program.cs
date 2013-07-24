using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using TrimFuel.Business;
using System.Configuration;
using MySql.Data.MySqlClient;
using TrimFuel.Business.Gateways;

namespace ExtractPaymentResponseText
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            int limit = int.Parse(ConfigurationSettings.AppSettings["limit"]);

            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                IDictionary<string, int> res = new Dictionary<string, int>();

                MySqlCommand q = new MySqlCommand(
                    "select * from ChargeHistoryEx order by ChargeHistoryID desc limit @limit"
                );
                q.Parameters.Add("@limit", MySqlDbType.Int32).Value = limit;

                foreach (ChargeHistoryEx ch in dao.Load<ChargeHistoryEx>(q))
                {
                    string responseText = ExtractResponse(logger, dao, ch);
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        if (res.ContainsKey(responseText))
                        {
                            res[responseText] += 1;
                        }
                        else
                        {
                            res[responseText] = 1;
                        }
                    }
                }

                foreach (var item in res)
                {
                    logger.Info(item.Key + ": " + item.Value);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static string ExtractResponse(ILog logger, IDao dao, ChargeHistoryEx ch)
        {
            string res = null;
            try
            {
                SaleService ss = new SaleService();
                AssertigyMID am = dao.Load<AssertigyMID>(ch.MerchantAccountID);
                IPaymentGateway gtw = ss.GetGatewayByMID(am);
                res = gtw.GetResultText(ch.Response);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }
    }
}
