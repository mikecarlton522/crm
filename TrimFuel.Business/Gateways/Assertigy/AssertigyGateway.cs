//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TrimFuel.Model;
//using System.IO;
//using TrimFuel.Business.Utils;
//using System.Net;
//using System.Xml;
//using System.Xml.XPath;
//using System.Globalization;

//namespace TrimFuel.Business.Gateways.Assertigy
//{
//    public class AssertigyGateway : IPaymentGateway
//    {
//        private const string ADDENDUM_DATA_LINE_TEMPLATE = "<addendumData><tag>{0}</tag><value>{1}</value></addendumData>";

//        #region IPaymentGateway Members

//        public BusinessError<GatewayResult> ProcessPayment(MerchantAccount merchantAccount, Billing billing, Registration registration, BillingSale billingSale, decimal amount, bool isUpsellPlanChange)
//        {
//            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

//            GatewayResult paymentResult = new GatewayResult();
//            paymentResult.Request = PrepareRequest(merchantAccount, billing, registration, billingSale, amount, isUpsellPlanChange);

//            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(GatewayAddress.ASSERTIGY_URL);
//            httpRequest.Method = "POST";
//            httpRequest.ContentType = "application/x-www-form-urlencoded";

//            StreamWriter strOut = new StreamWriter(httpRequest.GetRequestStream());
//            strOut.Write(string.Format("txnRequest={0}&txnMode=ccPurchase", paymentResult.Request));
//            strOut.Close();

//            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
//            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
//            paymentResult.Response = strIn.ReadToEnd();
//            strIn.Close();

//            //httpResponse.StatusCode == HttpStatusCode.OK
//            res.ReturnValue = paymentResult;
//            int? responseCode = ExtractResponseCode(paymentResult.Response);
//            if (responseCode == null)
//            {
//                //Unknown error
//                throw new Exception("Can't extract code from response");
//            }
//            else if (responseCode == 0)
//            {
//                //Successful Payment
//                res.State = BusinessErrorState.Success;
//            }
//            else
//            {
//                //Failed Payment
//                res.State = BusinessErrorState.Error;
//                res.ErrorMessage = ExtractResponseErrorMessage(responseCode.Value, paymentResult.Response);
//            }

//            paymentResult.ResponseParams = new AssertigyGatewayResponseParams(paymentResult.Response);

//            return res;
//        }

//        public BusinessError<GatewayResult> DoCancel(MerchantAccount merchantAccount, ChargeHistoryEx chargeHistory, long saleOrBillingID)
//        {
//            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

//            GatewayResult paymentResult = new GatewayResult();
//            paymentResult.Request = PrepareCancel(merchantAccount, chargeHistory, saleOrBillingID);

//            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(GatewayAddress.ASSERTIGY_URL);
//            httpRequest.Method = "POST";
//            httpRequest.ContentType = "application/x-www-form-urlencoded";

//            StreamWriter strOut = new StreamWriter(httpRequest.GetRequestStream());
//            strOut.Write(string.Format("txnRequest={0}&txnMode=ccCancelSettle", paymentResult.Request));
//            strOut.Close();

//            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
//            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
//            paymentResult.Response = strIn.ReadToEnd();
//            strIn.Close();

//            res.State = BusinessErrorState.Success;
//            res.ReturnValue = paymentResult;

//            return res;
//        }

//        public BusinessError<GatewayResult> DoRefund(MerchantAccount merchantAccount, ChargeHistoryEx chargeHistory, long saleOrBillingID)
//        {
//            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

//            GatewayResult paymentResult = new GatewayResult();
//            paymentResult.Request = PrepareRefund(merchantAccount, chargeHistory, saleOrBillingID);

//            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(GatewayAddress.ASSERTIGY_URL);
//            httpRequest.Method = "POST";
//            httpRequest.ContentType = "application/x-www-form-urlencoded";

//            StreamWriter strOut = new StreamWriter(httpRequest.GetRequestStream());
//            strOut.Write(string.Format("txnRequest={0}&txnMode=ccCredit", paymentResult.Request));
//            strOut.Close();

//            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
//            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
//            paymentResult.Response = strIn.ReadToEnd();
//            strIn.Close();

//            res.State = BusinessErrorState.Success;
//            res.ReturnValue = paymentResult;

//            return res;
//        }

//        #endregion

//        private string PrepareCancel(MerchantAccount merchantAccount, ChargeHistoryEx chargeHistory, long saleOrBillingID)
//        {
//            string res = Utility.LoadFromEmbeddedResource(typeof(AssertigyGateway), "assertigy_void.xml");

//            res = res.Replace("##ACCOUNT_NUMBER##", merchantAccount.AccountNumber);
//            res = res.Replace("##STORE_ID##", merchantAccount.MerchantID);
//            res = res.Replace("##STORE_PWD##", merchantAccount.MerchantPassword);
//            res = res.Replace("##TXN_NUMBER##", chargeHistory.TransactionNumber);
//            res = res.Replace("##BILLING_ID##", saleOrBillingID.ToString());
//            res = res.Replace("##AMOUNT##", Utility.FormatPrice(chargeHistory.Amount));

//            return res;
//        }

//        private string PrepareRefund(MerchantAccount merchantAccount, ChargeHistoryEx chargeHistory, long saleOrBillingID)
//        {
//            string res = Utility.LoadFromEmbeddedResource(typeof(AssertigyGateway), "assertigy_refund.xml");

//            res = res.Replace("##ACCOUNT_NUMBER##", merchantAccount.AccountNumber);
//            res = res.Replace("##STORE_ID##", merchantAccount.MerchantID);
//            res = res.Replace("##STORE_PWD##", merchantAccount.MerchantPassword);
//            res = res.Replace("##TXN_NUMBER##", chargeHistory.TransactionNumber);
//            res = res.Replace("##BILLING_ID##", saleOrBillingID.ToString());
//            res = res.Replace("##AMOUNT##", Utility.FormatPrice(chargeHistory.Amount));

//            return res;
//        }

//        private string PrepareRequest(MerchantAccount merchantAccount, Billing billing, Registration registration, BillingSale billingSale, decimal amount, bool isUpsellPlanChange)
//        {
//            string request = Utility.LoadFromEmbeddedResource(typeof(AssertigyGateway), "assertigy_purchase_eu.xml");

//            request = request.Replace("##ACCOUNT_NUMBER##", merchantAccount.AccountNumber);
//            request = request.Replace("##STORE_ID##", merchantAccount.MerchantID);
//            request = request.Replace("##STORE_PWD##", merchantAccount.MerchantPassword);

//            request = request.Replace("##AMOUNT##", Utility.FormatPrice(amount));

//            request = request.Replace("##CC_NUM##", billing.CreditCardCnt.DecryptedCreditCard);
//            request = request.Replace("##CC_MONTH##", billing.ExpMonth.ToString());
//            request = request.Replace("##CC_YEAR##", billing.ExpYear.ToString());
//            request = request.Replace("##CVD_INDICATOR##", "1");
//            request = request.Replace("##CVD##", billing.CVV);

//            request = request.Replace("##BILLING_ID##", (billingSale != null) ? billingSale.SaleID.ToString() : billing.BillingID.ToString());

//            request = request.Replace("##B_F_NAME##", billing.FirstName);
//            request = request.Replace("##B_L_NAME##", billing.LastName);
//            request = request.Replace("##B_ADDRESS##", billing.Address1);
//            request = request.Replace("##B_CITY##", billing.City);
//            request = request.Replace("##B_STATE##", billing.State);
//            request = request.Replace("##B_COUNTRY##", billing.Country);
//            request = request.Replace("##B_ZIP##", billing.Zip);
//            request = request.Replace("##B_PHONE##", billing.Phone);
//            request = request.Replace("##B_EMAIL##", billing.Email);
//            request = request.Replace("##IP##", billing.IP);

//            request = request.Replace("##S_F_NAME##", registration.FirstName);
//            request = request.Replace("##S_L_NAME##", registration.LastName);
//            request = request.Replace("##S_ADDRESS##", registration.Address1);
//            request = request.Replace("##S_CITY##", registration.City);
//            request = request.Replace("##S_STATE##", registration.State);
//            request = request.Replace("##S_COUNTRY##", billing.Country);
//            request = request.Replace("##S_ZIP##", registration.Zip);
//            request = request.Replace("##S_PHONE##", registration.Phone);
//            request = request.Replace("##S_EMAIL##", registration.Email);

//            request = request.Replace("##PREV##", isUpsellPlanChange.ToString().ToLower());

//            string addendumData = string.Empty;
//            if (!string.IsNullOrEmpty(billing.Affiliate))
//            {
//                addendumData += string.Format(ADDENDUM_DATA_LINE_TEMPLATE, "Affiliate", billing.Affiliate);

//                if (!string.IsNullOrEmpty(billing.SubAffiliate))
//                {
//                    addendumData += string.Format(ADDENDUM_DATA_LINE_TEMPLATE, "SubAffiliate", billing.SubAffiliate);
//                }

//                addendumData += string.Format(ADDENDUM_DATA_LINE_TEMPLATE, "USER_DATA_05", string.Format("{0}-{1}", billing.Affiliate, billing.SubAffiliate));
//            }

//            request = request.Replace("##ADDENDUM##", addendumData);

//            return request;
//        }

//        private int? ExtractResponseCode(string response)
//        {
//            int? res = null;
//            if (!string.IsNullOrEmpty(response))
//            {
//                string strCode = ExtractResponseParam(response, "code");
//                if (!string.IsNullOrEmpty(response))
//                {
//                    try
//                    {
//                        res = int.Parse(strCode);
//                    }
//                    catch { }
//                }
//            }
//            return res;
//        }

//        private string ExtractResponseErrorMessage(int code, string response)
//        {
//            if (code == 0)
//            {
//                return null;
//            }

//            string res = "We're sorry, but your transaction failed.  Please verify your payment information and try again.";
//            string errorDescription = ExtractResponseParam(response, "description");

//            switch (code)
//            {
//                case 3009:
//                    res = "We're sorry, but your transaction was refused by your credit card company.  Please contact your card issuer for information or try again with a different card.";
//                    break;
//                case 3012:
//                    res = "We're sorry, but your transaction was refused by your credit card company.  Please contact your card issuer for information or try again with a different card.";
//                    break;
//                case 3013:
//                    res = "We're sorry, but your transaction was refused by your credit card company.  Please contact your card issuer for information or try again with a different card.";
//                    break;
//                case 3014:
//                    res = "We're sorry, but your transaction was refused by your credit card company.  Please contact your card issuer for information or try again with a different card.";
//                    break;
//                case 3005:
//                    res = "We're sorry, the CVV2 card code you submitted appears to be invalid.  Please verify and try again.";
//                    break;
//                case 3019:
//                    res = "We're sorry, the CVV2 card code you submitted appears to be invalid.  Please verify and try again.";
//                    break;
//                case 3002:
//                    res = "We're sorry, but your transaction failed.  Please verify your payment information, ensuring your card type, number, and expiration date are in the correct format, and try again.";
//                    break;
//                case 3006:
//                    res = "We're sorry, but your transaction failed.  Please verify your payment information, ensuring your card type, number, and expiration date are in the correct format, and try again.";
//                    break;
//                case 5034:
//                    res = "We're sorry, the Email address you submitted appears to be in an invalid format.  Please verify and try again.";
//                    break;
//                case 5039:
//                    res = "We're sorry, the Billing address you supplied was not accepted.  Please verify that the Billing address appears exactly as it does on your credit card statement and try again.";
//                    break;
//                default:
//                    break;
//            }

//            if (!string.IsNullOrEmpty(errorDescription))
//            {
//                res += string.Format(" (Error: {0})", errorDescription);
//            }

//            return res;
//        }

//        private string ExtractResponseParam(string response, string paramName)
//        {
//            AssertigyGatewayResponseParams p = new AssertigyGatewayResponseParams(response);
//            return p.GetParam(paramName);
//        }
//    }
//}
