using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using System.Net;
using System.IO;

namespace STO_ReAttemptBillingAPIRequests
{
    class Program
    {
        //private static long[] BillingIDs = {253419,253434,253439,253441,253453,253454,253456,253458,253462,253463,
        //                                    253471,253475,253483,253484,253487,253488,253492,253493,253494,253496,
        //                                    253497,253506,253508,253514,253515,253523,253757,253762,253896,254003,
        //                                    254011,254032,254191,254226,254247,254253,254256,254263,254267,254389,
        //                                    254403,254421,254431,254469,254575,254629,254755,255080,255103,255172,
        //                                    255284,255342,255364,255407,255659,255666,255679,255782,255784,256008,
        //                                    256010,256016,256019,256106,256110,256522};
        //private static long[] BillingAPIRequestIDs = {44314,40889,40895,40900,40914,40916,40918,40923,40930,40932,
        //                                              40948,40954,40965,40967,40969,40971,40977,40979,40981,40983,
        //                                              40985,44316,40996,41002,41004,41012,41115,41117,41170,41263,
        //                                              41268,41284,44318,41430,41445,41450,41453,41460,41464,41482,
        //                                              44319,41492,44320,41509,41571,41613,41665,41735,41755,41781,
        //                                              41852,41927,41950,41984,44321,42152,42163,42258,42262,42334,
        //                                              42337,42340,42344,42397,42404,42722};

        private static long[] BillingIDs;
        private static long[] BillingAPIRequestIDs;

        static void Main(string[] args)
        {
            ILog logger = LogManager.GetLogger(typeof(Program));
            log4net.Config.XmlConfigurator.Configure();
            IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select Request as Value from BillingAPIRequest
                    where BillingAPIRequestID in (" + string.Join(",", BillingAPIRequestIDs.Select(i => i.ToString()).ToArray()) + @")
                ");

                IList<StringView> list = dao.Load<StringView>(q);

                foreach (var item in list)
                {
                    ProcessRequest(logger, item.Value);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        static string ProcessRequest(ILog logger, string body)
        {
            string res = null;
            try
            {
                res = HttpSOAPRequest("https://dashboard.trianglecrm.com/api/billing_ws.asmx", "CreateSubscription", body);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        private static string HttpSOAPRequest(string url, string action, string body)
        {
            string res = null;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Headers.Add("SOAPAction", "\"http://trianglecrm.com/" + action + "\"");
            httpRequest.ContentType = "text/xml; charset=utf-8";
            httpRequest.Accept = "text/xml";
            httpRequest.Method = "POST";

            StreamWriter strOut = new StreamWriter(httpRequest.GetRequestStream());
            strOut.Write(body);
            strOut.Close();

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
            res = strIn.ReadToEnd();
            strIn.Close();

            return res;
        }
    }
}
