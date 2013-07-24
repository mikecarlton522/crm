//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TrimFuel.Model;
//using TrimFuel.Business.Utils;
//using System.Net;
//using System.IO;

//namespace TrimFuel.Business.Gateways.Iq2solutions
//{
//    public class Iq2solutionsGateway : IFraudGateway
//    {
//        #region IFraudGateway Members

//        public BusinessError<GatewayResult> SendFraudScore(Billing billing, Registration registration, Sale sale)
//        {
//            GatewayResult fraudResult = new GatewayResult();
//            fraudResult.Request = PrepareFraudScore(billing, registration, sale);

//            string url = string.Format("{0}?{1}", GatewayAddress.FRAUD_URL, fraudResult.Request);
//            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
//            httpRequest.Method = "GET";

//            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
//            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
//            fraudResult.Response = strIn.ReadToEnd();
//            strIn.Close();

//            fraudResult.ResponseParams = new Iq2solutionsGatewayResponseParams(fraudResult.Response);
//            return new BusinessError<GatewayResult>(fraudResult, BusinessErrorState.Success, null);
//        }

//        #endregion

//        private string PrepareFraudScore(Billing billing, Registration registration, Sale sale)
//        {
//            string res = Utility.LoadFromEmbeddedResource(typeof(Iq2solutionsGateway), "fraud.tpl");

//            res = res.Replace("##CC_NUM##", billing.CreditCardCnt.DecryptedCreditCard);

//            res = res.Replace("##BILLING_ID##", sale.SaleID.Value.ToString());
//            res = res.Replace("##B_F_NAME##", billing.FirstName);
//            res = res.Replace("##B_L_NAME##", billing.LastName);
//            res = res.Replace("##B_ADDRESS##", billing.Address1);
//            res = res.Replace("##B_CITY##", billing.City);
//            res = res.Replace("##B_STATE##", billing.State);
//            res = res.Replace("##B_COUNTRY##", billing.Country);
//            res = res.Replace("##B_ZIP##", billing.Zip);
//            res = res.Replace("##B_PHONE##", billing.Phone);
//            res = res.Replace("##B_EMAIL##", billing.Email);
//            res = res.Replace("##IP##", billing.IP);

//            res = res.Replace("##S_F_NAME##", registration.FirstName);
//            res = res.Replace("##S_L_NAME##", registration.LastName);
//            res = res.Replace("##S_ADDRESS##", registration.Address1);
//            res = res.Replace("##S_CITY##", registration.City);
//            res = res.Replace("##S_STATE##", registration.State);
//            res = res.Replace("##S_COUNTRY##", billing.Country);
//            res = res.Replace("##S_ZIP##", registration.Zip);
//            res = res.Replace("##S_PHONE##", registration.Phone);
//            res = res.Replace("##S_EMAIL##", registration.Email);

//            if (!string.IsNullOrEmpty(billing.Affiliate))
//            {
//                res = res.Replace("##AFF##", billing.Affiliate);

//                if (!string.IsNullOrEmpty(billing.SubAffiliate))
//                {
//                    res = res.Replace("##SUB##", billing.SubAffiliate);
//                }
//            }

//            //if ##AFF## and ##SUB## are not set, set them to ""
//            res = res.Replace("##AFF##", string.Empty);
//            res = res.Replace("##SUB##", string.Empty);

//            return res;
//        }
//    }
//}
