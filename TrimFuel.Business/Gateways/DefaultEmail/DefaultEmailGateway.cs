using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Net;
using System.IO;
using TrimFuel.Business.Utils;
using System.Web;

namespace TrimFuel.Business.Gateways.DefaultEmail
{
    public class DefaultEmailGateway : IEmailGateway
    {
        //TODO: config
        private const string REACTIVATION_LINK_TEMPLATE = "http://www.ecigsbrandoffer.com/reactivate/default.asp?b={0}";

        private string GetResponse(string request)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            string url = Config.Current.EMAIL_GATEWAY_URL;
            var smtpSetting = new SMTPSettingService().GetSMTPSetting();
            if (smtpSetting != null)
                url = smtpSetting.URL;

            return wc.UploadString(url, "POST", request);
        }

        #region IEmailGateway Members

        public BusinessError<GatewayResult> SendEmail(long emailID, DynamicEmail dynaminEmail, Billing billing, decimal? shippingAmount, decimal? productAmount, string merchantName, string billingCancelCode, string ownRefererCode, string refererCode, string refererPassword)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult emailResult = new GatewayResult();
            emailResult.Request = ReplaceVariables(dynaminEmail.Content, emailID, dynaminEmail, billing, shippingAmount, productAmount, merchantName, billingCancelCode, ownRefererCode, refererCode, refererPassword, null, null);
            string subject = ReplaceVariables(dynaminEmail.Subject, emailID, dynaminEmail, billing, shippingAmount, productAmount, merchantName, billingCancelCode, ownRefererCode, refererCode, refererPassword, null, null);
            string post = string.Format("fromName={0}&" +
                "fromAddress={1}&" +
                "toName={2}&" +
                "toAddress={3}&" +
                "body={4}&" +
                "subject={5}",
                HttpUtility.UrlEncode(dynaminEmail.FromName),
                HttpUtility.UrlEncode(dynaminEmail.FromAddress),
                HttpUtility.UrlEncode(string.Format("{0} {1}", billing.FirstName, billing.LastName)),
                HttpUtility.UrlEncode(billing.Email),
                HttpUtility.UrlEncode(emailResult.Request),
                HttpUtility.UrlEncode(subject));
            emailResult.Response = GetResponse(post);

            res.ReturnValue = emailResult;
            res.State = BusinessErrorState.Success;
            res.ReturnValue.ResponseParams = new StaticResponseParams(){{"subject", subject}};

            return res;
        }

        public BusinessError<GatewayResult> SendEmail(long emailID, DynamicEmail dynaminEmail, Billing billing, decimal? shippingAmount, decimal? productAmount, string merchantName, string billingCancelCode, string ownRefererCode, string refererCode, string refererPassword, DateTime createDT, Product product)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult emailResult = new GatewayResult();
            emailResult.Request = ReplaceVariables(dynaminEmail.Content, emailID, dynaminEmail, billing, shippingAmount, productAmount, merchantName, billingCancelCode, ownRefererCode, refererCode, refererPassword, createDT, product);
            string subject = ReplaceVariables(dynaminEmail.Subject, emailID, dynaminEmail, billing, shippingAmount, productAmount, merchantName, billingCancelCode, ownRefererCode, refererCode, refererPassword, createDT, product);
            string post = string.Format("fromName={0}&" +
                "fromAddress={1}&" +
                "toName={2}&" +
                "toAddress={3}&" +
                "body={4}&" +
                "subject={5}",
                HttpUtility.UrlEncode(dynaminEmail.FromName),
                HttpUtility.UrlEncode(dynaminEmail.FromAddress),
                HttpUtility.UrlEncode(string.Format("{0} {1}", billing.FirstName, billing.LastName)),
                HttpUtility.UrlEncode(billing.Email),
                HttpUtility.UrlEncode(emailResult.Request),
                HttpUtility.UrlEncode(subject));
            emailResult.Response = GetResponse(post);

            res.ReturnValue = emailResult;
            res.State = BusinessErrorState.Success;
            res.ReturnValue.ResponseParams = new StaticResponseParams() { { "subject", subject } };

            return res;
        }

        public BusinessError<GatewayResult> SendEmail(string fromName, string fromAddress, string toName, string toAddress, string subject, string body)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult emailResult = new GatewayResult();
            emailResult.Request = body;
            string post = string.Format("fromName={0}&" +
                "fromAddress={1}&" +
                "toName={2}&" +
                "toAddress={3}&" +
                "body={4}&" +
                "subject={5}",
                HttpUtility.UrlEncode(fromName),
                HttpUtility.UrlEncode(fromAddress),
                HttpUtility.UrlEncode(toName),
                HttpUtility.UrlEncode(toAddress),
                HttpUtility.UrlEncode(emailResult.Request),
                HttpUtility.UrlEncode(subject));
            emailResult.Response = GetResponse(post);

            res.ReturnValue = emailResult;
            res.State = BusinessErrorState.Success;
            res.ReturnValue.ResponseParams = new StaticResponseParams() { { "subject", subject } };

            return res;
        }

        #endregion

        private string ReplaceVariables(string str, long emailID, DynamicEmail dynaminEmail, Billing billing, decimal? shippingAmount, decimal? productAmount, string merchantName, string billingCancelCode, string ownRefererCode, string refererCode, string refererPassword, DateTime? createDT, Product product)
        {
            str = str.Replace("##FNAME##", billing.FirstName);
            str = str.Replace("##LNAME##", billing.LastName);
            str = str.Replace("##ADD1##", billing.Address1);
            str = str.Replace("##ADD2##", billing.Address2);
            str = str.Replace("##CITY##", billing.City);
            str = str.Replace("##STATE##", billing.State);
            str = str.Replace("##ZIP##", billing.Zip);
            str = str.Replace("##PHONE##", billing.Phone);
            str = str.Replace("##EMAIL##", billing.Email);
            str = str.Replace("##PASSWORD##", refererPassword);

            var registration = new BaseService().Load<Registration>(billing.RegistrationID);

            str = str.Replace("##SHIPPING_FNAME##", registration.FirstName);
            str = str.Replace("##SHIPPING_LNAME##", registration.LastName);
            str = str.Replace("##SHIPPING_ADD1##", registration.Address1);
            str = str.Replace("##SHIPPING_ADD2##", registration.Address2);
            str = str.Replace("##SHIPPING_CITY##", registration.City);
            str = str.Replace("##SHIPPING_STATE##", registration.State);
            str = str.Replace("##SHIPPING_ZIP##", registration.Zip);
            str = str.Replace("##SHIPPING_PHONE##", registration.Phone);
            str = str.Replace("##SHIPPING_EMAIL##", registration.Email);

            string creditCardType = null;
            string creditCard = billing.CreditCardCnt.DecryptedCreditCard;
            if (creditCard.StartsWith("4"))
            {
                creditCardType = "Visa";
            }
            else if (creditCard.StartsWith("5"))
            {
                creditCardType = "MasterCard";
            }
            else
            {
                creditCardType = "card";
            }
            creditCard = creditCard.Substring(creditCard.Length - 4, 4);

            str = str.Replace("##CARDTYPE##", creditCardType);
            str = str.Replace("##LAST4##", creditCard);
            str = str.Replace("##MID##", merchantName ?? "**TEST ORDER**");
            str = str.Replace("##SH_AMOUNT##", Utility.FormatPrice(shippingAmount));
            str = str.Replace("##PRODUCT_AMOUNT##", Utility.FormatPrice(productAmount));
            str = str.Replace("##TOTAL_AMOUNT##", Utility.FormatPrice(Utility.Add(shippingAmount, productAmount)));
            str = str.Replace("##BILLINGID##", billing.BillingID.Value.ToString());
            str = str.Replace("##ID##", emailID.ToString());

            str = str.Replace("##REACTIVATION_LINK##", string.Format(REACTIVATION_LINK_TEMPLATE, billing.BillingID));
            str = str.Replace("##CANCELCODE##", billingCancelCode);

            str = str.Replace("##OWN_REFERER_CODE##", ownRefererCode);
            str = str.Replace("##REFERER_CODE##", refererCode);

            str = str.Replace("##PRODUCT_NAME##", product == null ? string.Empty : product.ProductName);
            str = str.Replace("##ORDER_DATE##", createDT == null ? string.Empty : createDT.Value.ToShortDateString());

            return str;
        }
    }
}
