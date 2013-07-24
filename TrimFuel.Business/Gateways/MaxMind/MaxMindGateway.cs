using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;

namespace TrimFuel.Business.Gateways.MaxMind
{
    public class MaxMindGateway : IFraudGateway
    {
        public static IGatewayResponseParams CreateResponseParamsExtractor(string response)
        {
            return new DelimitedResponseParams(response, ';');
        }

        #region IFraudGateway Members

        public BusinessError<GatewayResult> SendFraudScore(Billing billing, Registration registration, RegistrationInfo registrationInfo, Sale sale)
        {
            GatewayResult fraudResult = new GatewayResult();
            fraudResult.Request = PrepareFraudScore(billing, registration, registrationInfo, sale);

            string url = HttpUtility.UrlPathEncode(string.Format("{0}?{1}", Config.Current.MAX_MIND_URL, fraudResult.Request));
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "GET";

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
            fraudResult.Response = strIn.ReadToEnd();
            strIn.Close();

            fraudResult.BinName = GetBINName(fraudResult.Response);
            fraudResult.ResponseParams = MaxMindGateway.CreateResponseParamsExtractor(fraudResult.Response);
            return new BusinessError<GatewayResult>(fraudResult, BusinessErrorState.Success, null);
        }

        #endregion

        private string PrepareFraudScore(Billing billing, Registration registration, RegistrationInfo registrationInfo, Sale sale)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(MaxMindGateway), "fraud.tpl");

            string bCountry = "US";
            string sCountry = "US";
            if (!string.IsNullOrEmpty(billing.Country))
            {
                bCountry = billing.Country;
            }
            if (registrationInfo != null && !string.IsNullOrEmpty(registrationInfo.Country))
            {
                sCountry = registrationInfo.Country;
            }

            res = res.Replace("##IP##", billing.IP);

            res = res.Replace("##B_CITY##", billing.City);
            res = res.Replace("##B_STATE##", billing.State);
            res = res.Replace("##B_ZIP##", billing.Zip);
            res = res.Replace("##B_COUNTRY##", bCountry);
            
            res = res.Replace("##B_PHONE##", billing.Phone);
            res = res.Replace("##B_EMAIL_MD5##", Utility.ComputeMD5(billing.Email));

            res = res.Replace("##CC_NUM_LEFT6##", billing.CreditCardCnt.DecryptedCreditCardLeft6);

            res = res.Replace("##S_ADDRESS##", registration.Address1);
            res = res.Replace("##S_CITY##", registration.City);
            res = res.Replace("##S_STATE##", registration.State);
            res = res.Replace("##S_ZIP##", registration.Zip);
            res = res.Replace("##S_COUNTRY##", sCountry);

            res = res.Replace("##SALE_ID##", sale.SaleID.Value.ToString());

            return res;
        }

        private string GetBINName(string input)
        {
            Regex r = new Regex("binName=(?<name>[^;=]*)", RegexOptions.IgnoreCase);
            return r.Match(input).Groups["name"].Value;
        }
    }
}
