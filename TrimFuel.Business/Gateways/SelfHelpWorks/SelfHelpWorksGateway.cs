using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using System.Net;
using System.IO;

namespace TrimFuel.Business.Gateways.SelfHelpWorks
{
    public class SelfHelpWorksGateway : ISHWGateway
    {
        #region ISHWGateway Members

        public BusinessError<GatewayResult> SendSHW(SHWProduct shwProduct, Billing billing)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult shwResult = new GatewayResult();
            shwResult.Request = PrepareSHW(shwProduct, billing);

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(Config.Current.SHW_GATEWAY_URL);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            StreamWriter strOut = new StreamWriter(httpRequest.GetRequestStream());
            strOut.Write(shwResult.Request);
            strOut.Close();

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
            shwResult.Response = strIn.ReadToEnd();
            strIn.Close();

            //httpResponse.StatusCode == HttpStatusCode.OK
            res.ReturnValue = shwResult;
            shwResult.ResponseParams = new SelfHelpWorksGatewayResponseParams(shwResult.Response);

            return res;
        }

        #endregion

        private string PrepareSHW(SHWProduct shwProduct, Billing billing)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(SelfHelpWorksGateway), "shw.tpl");

            string expDate = string.Format("{0}/{1}",
                (billing.ExpMonth > 9) ? billing.ExpMonth.ToString() : "0" + billing.ExpMonth.ToString(),
                (billing.ExpYear > 1000) ? billing.ExpYear.ToString() : (2000 + billing.ExpYear).ToString());

            res = res.Replace("##FNAME##", billing.FirstName);
            res = res.Replace("##LNAME##", billing.LastName);
            res = res.Replace("##EMAIL##", billing.Email);
            res = res.Replace("##CC_NUMBER##", billing.CreditCardCnt.DecryptedCreditCard);
            res = res.Replace("##CC_EXPDATE##", expDate);
            res = res.Replace("##CC_NAME##", string.Format("{0} {1}", billing.FirstName, billing.LastName));
            res = res.Replace("##CC_IP##", billing.IP);
            res = res.Replace("##CC_CID##", billing.CVV);

            res = res.Replace("##ADDRESS1##", billing.Address1);
            res = res.Replace("##ADDRESS2##", billing.Address2);
            res = res.Replace("##CITY##", billing.City);
            res = res.Replace("##STATE##", billing.State);
            res = res.Replace("##ZIP##", billing.Zip);
            res = res.Replace("##COUNTRY##", billing.Country);

            res = res.Replace("##COMPANY_ID##", shwProduct.CompanyID.Value.ToString());
            res = res.Replace("##SUBSCRIPTION_ID##", shwProduct.SubscriptionID.Value.ToString());
            res = res.Replace("##COURSE_ID##", shwProduct.CourseID.Value.ToString());
            res = res.Replace("##INTEGRATION_ID##", shwProduct.IntegrationID);
            res = res.Replace(System.Environment.NewLine, string.Empty);

            return res;
        }
    }
}
