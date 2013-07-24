using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using TrimFuel.Business.Gateways.IPG;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace TrimFuel.Business.Gateways.BPagGateway
{
    public class BPagGateway : BaseService, IPaymentGateway
    {
        private string ConvertResponse(string status, string returnCode, string message, string transactionId, string authorisationNumber, string amount, string orderId)
        {
            return string.Format("status={0}&returnCode={1}&message={2}&transactionId={3}&authorisationNumber={4}&amount={5}&orderId={6}",
                status, returnCode, message, transactionId, authorisationNumber, amount, orderId);
        }

        //protected virtual string ConvertRequest(string method, string merchantPassword, decimal amount, Currency currency, string orderId, Billing billing)
        //{
        //    StringBuilder result = new StringBuilder();

        //    result.AppendFormat("{0}={1}", "method", method);
        //    addRequestParam(result, "merchantId", merchantPassword);
        //    addRequestParam(result, "orderId", orderId);
        //    addRequestParam(result, "customerName", GetCustomerName(billing));
        //    addRequestParam(result, "amount", FormatPrice(amount));
        //    addRequestParam(result, "paymentMethod", billing.CreditCardCnt.TryGetCardType());
        //    addRequestParam(result, "holder", GetCustomerName(billing));
        //    addRequestParam(result, "cardNumber", billing.CreditCardCnt.DecryptedCreditCard);
        //    addRequestParam(result, "expiration", GetExpiration(billing));
        //    addRequestParam(result, "installment", PaymentExVars.Installments.ToString());
        //    addRequestParam(result, "securityCode", billing.CVV);

        //    return result.ToString();
        //}

        //private string ConvertRequest(string method, string merchantPassword, decimal amount, Currency currency, string orderId)
        //{
        //    StringBuilder result = new StringBuilder();

        //    result.AppendFormat("{0}={1}", "method", method);
        //    addRequestParam(result, "merchantId", merchantPassword);
        //    addRequestParam(result, "orderId", orderId);
        //    addRequestParam(result, "amount", FormatPrice(amount));

        //    return result.ToString();
        //}

        private void addRequestParam(StringBuilder resultString, string paramName, object paramValue)
        {
            if (paramValue != null && !String.IsNullOrEmpty(paramValue.ToString()))
            {
                resultString.AppendFormat("&{0}={1}", paramName, paramValue);
            }
        }

        private string FormatPrice(decimal? price)
        {
            if (price == null)
            {
                return string.Empty;
            }
            return Utility.FormatPrice(price).Replace(".", ",");
        }

        private string GetCustomerName(Billing billing)
        {
            return billing.FirstName + " " + billing.LastName;
        }

        private string GetExpiration(Billing billing)
        {
            return billing.ExpMonth.Value.ToString("00") + "/" + billing.ExpYear.Value.ToString("00");
        }

        public BusinessError<GatewayResult> Sale(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing, Product product)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();
            GatewayResult paymentResult = new GatewayResult();

            string OrderId = "";
            if (saleID > 0)
                OrderId = saleID.ToString();
            else
                OrderId = billing.BillingID.ToString();

            paymentResult.Request = PrepareSale(merchantID, merchantLogin, merchantPassword, currency, billing, saleID, amount);
            string bPagResult = GetResponse(merchantID, merchantLogin, merchantPassword, "payOrder", paymentResult.Request);

            paymentResult.Response = bPagResult;
            res.ReturnValue = paymentResult;
            ProcessResult(res, "payOrderReturn");

            //change Response view
            paymentResult.Response = ConvertResponse(
                    ExtractResponseParam(bPagResult, "status", "payOrderReturn"),
                    ExtractResponseParam(bPagResult, "status", "bpag_data"),
                    ExtractResponseParam(bPagResult, "msg", "payOrderReturn"),
                    ExtractResponseParam(bPagResult, "id", "last_attempt"),
                    ExtractResponseParam(bPagResult, "auth_code", "last_attempt"),
                    amount.ToString(),
                    ExtractResponseParam(bPagResult, "id", "bpag_data")
                );

            //paymentResult.Request = ConvertRequest("PayOrder", merchantPassword, amount, currency, OrderId, billing);

            paymentResult.ResponseParams = new BPagGatewayResponseParams(paymentResult.Response);

            //try capture
            //Capture(merchantID, merchantLogin, merchantPassword, saleID, ExtractResponseParam(bPagResult, "id", "bpag_data"));

            return res;
        }

        public BusinessError<GatewayResult> Void(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();
            GatewayResult paymentResult = new GatewayResult();

            paymentResult.Request = PrepareVoid(chargeHistory);
            paymentResult.Response = GetResponse(chargeHistory.ChildMID, merchantLogin, merchantPassword, "cancel", paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res, "cancelReturn");

            paymentResult.Response = ConvertResponse(
                        ExtractResponseParam(paymentResult.Response, "status", "cancelReturn"),
                        ExtractResponseParam(paymentResult.Response, "status", "bpag_data"),
                        ExtractResponseParam(paymentResult.Response, "msg", "cancelReturn"),
                        ExtractResponseParam(paymentResult.Response, "id", "last_attempt"),
                        ExtractResponseParam(paymentResult.Response, "auth_code", "last_attempt"),
                        chargeHistory.Amount.ToString(),
                        ExtractResponseParam(paymentResult.Response, "id", "bpag_data")
                    );

            var sale = new SaleService().GetSaleByChargeHistory(chargeHistory.ChargeHistoryID.Value);
            //paymentResult.Request = ConvertRequest("cancelReturn", merchantPassword, chargeHistory.Amount ?? 0, currency, sale.SaleID.ToString());

            paymentResult.ResponseParams = new BPagGatewayResponseParams(paymentResult.Response);
            return res;
        }

        public BusinessError<GatewayResult> Capture(string merchantLogin, string merchantPassword, ChargeHistoryEx authChargeHistory, decimal amount, Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();
            GatewayResult paymentResult = new GatewayResult();

            var mid = dao.Load<AssertigyMID>(authChargeHistory.MerchantAccountID);
            var nmiCompany = new MerchantService().GetNMICompanyByAssertigyMID(authChargeHistory.MerchantAccountID);

            if (mid != null && nmiCompany != null)
            {
                paymentResult.Request = PrepareCapture(authChargeHistory);
                paymentResult.Response = GetResponse(mid.MID, nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, "capture", paymentResult.Request);
                res.ReturnValue = paymentResult;
                ProcessResult(res, "captureReturn");
                paymentResult.ResponseParams = new BPagGatewayResponseParams(paymentResult.Response);
            }

            return res;
        }

        private string PrepareVoid(ChargeHistoryEx chargeHistory)
        {
            string request = Utility.LoadFromEmbeddedResource(typeof(BPagGateway), "Cancel.xml");

            var sale = new SaleService().GetSaleByChargeHistory(chargeHistory.ChargeHistoryID.Value);
            request = request.Replace("##MERCHANT_REF##", sale.SaleID.ToString());
            request = request.Replace("##ORDER_ID##", chargeHistory.TransactionNumber);

            return request;
        }

        private string PrepareCapture(ChargeHistoryEx authChargeHistory)
        {
            string request = Utility.LoadFromEmbeddedResource(typeof(BPagGateway), "Capture.xml");

            var sale = new SaleService().GetSaleByChargeHistory(authChargeHistory.ChargeHistoryID.Value);
            request = request.Replace("##MERCHANT_REF##", sale.SaleID.ToString());
            request = request.Replace("##ORDER_ID##", authChargeHistory.TransactionNumber);

            return request;
        }

        private void Capture(string merchantId, string merchantLogin, string merchantPassword, long saleID, string orderID)
        {
            string request = Utility.LoadFromEmbeddedResource(typeof(BPagGateway), "Capture.xml");
            request = request.Replace("##MERCHANT_REF##", saleID.ToString());
            request = request.Replace("##ORDER_ID##", orderID);

            var response = GetResponse(merchantId, merchantLogin, merchantPassword, "capture", request);
        }

        protected virtual string ReplacePaymentMethod(string requestString, Billing billing)
        {
            string request = requestString;
            switch (billing.CreditCardCnt.TryGetCardType())
            {
                case PaymentTypeEnum.Visa:
                    request = request.Replace("##PAYMENT_METHOD##", "cielows2p_visa");
                    break;
                case PaymentTypeEnum.Mastercard:
                    request = request.Replace("##PAYMENT_METHOD##", "cielows2p_mastercard");
                    break;
                case PaymentTypeEnum.AmericanExpress:
                    request = request.Replace("##PAYMENT_METHOD##", "amex_webpos2p");
                    break;
                default:
                    request = request.Replace("##PAYMENT_METHOD##", string.Empty);
                    break;
            }
            return request;
        }

        private string PrepareSale(string merchantID, string merchantLogin, string merchantPassword, Currency currency, Billing billing, long saleID, decimal amount)
        {
            string request = Utility.LoadFromEmbeddedResource(typeof(BPagGateway), "PayOrder.xml");

            request = request.Replace("##MERCHANT_REF##", saleID.ToString());
            request = request.Replace("##CURRENCY##", currency == null ? "US" : currency.CurrencyName);
            //request = request.Replace("##CURRENCY##", "BRL");
            request = request.Replace("##AMOUNT##", GetAmount(amount).ToString());

            request = ReplacePaymentMethod(request, billing);

            request = request.Replace("##CC_NUM##", GetEncrypted(billing.CreditCardCnt.DecryptedCreditCard));
            request = request.Replace("##CVD##", GetEncrypted(billing.CVV));
            request = request.Replace("##CC_TYPE##", billing.CreditCardCnt.TryGetCardName());
            string month = billing.ExpMonth.ToString();
            if (month.Length == 1)
                month = "0" + month;
            request = request.Replace("##CC_MONTH##", month);
            string year = billing.ExpYear.ToString();
            if (year.Length == 2)
                year = "20" + year;
            request = request.Replace("##CC_YEAR##", year);
            request = request.Replace("##B_F_NAME##", billing.FirstName);
            request = request.Replace("##B_L_NAME##", billing.LastName);
            request = request.Replace("##BILLING_ID##", billing.BillingID.ToString());
            request = request.Replace("##IP##", billing.IP);
            request = request.Replace("##B_EMAIL##", billing.Email);
            request = request.Replace("##B_PHONE##", FixPhone(billing.Phone));
            request = request.Replace("##B_ADDRESS##", billing.Address1);
            request = request.Replace("##B_CITY##", billing.City);
            request = request.Replace("##B_STATE##", billing.State);
            request = request.Replace("##B_COUNTRY##", FixCountry(billing.Country));
            request = request.Replace("##B_ZIP##", billing.Zip);
            request = request.Replace("##CARD_HOLDER##", GetCardHolder(billing));

            //new value INSTALLMENTS. passed to Sale using static class 
            request = request.Replace("##INSTALLMENTS##", PaymentExVars.Installments.ToString());
            //new value INSTALLMENTS. passed to Sale using static class 

            return request;
        }

        private string FixCountry(string country)
        {
            if (country == "Brasil")
                return "BR";
            return country;
        }

        private string FixPhone(string phone)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in phone)
            {
                if (c >= '0' && c <= '9')
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private int GetAmount(decimal? amount)
        {
            if (amount == null)
                return 0;
            return (int)(amount * 100);
        }

        private string ExtractResponseParam(string response, string paramName, string parentNode)
        {
            BPagGatewayResponseParams p = new BPagGatewayResponseParams(response);
            return p.GetParam(paramName, parentNode);
        }

        private void ProcessResult(BusinessError<GatewayResult> result, string parentNode)
        {
            string success = ExtractResponseParam(result.ReturnValue.Response, "status", parentNode);
            string message = ExtractResponseParam(result.ReturnValue.Response, "msg", parentNode);
            if (success == BPagResponceStatus.Success)
            {
                //Success
                result.State = BusinessErrorState.Success;
            }
            else
            {
                //Error
                result.State = BusinessErrorState.Error;
                if (String.IsNullOrEmpty(message))
                {
                    result.ErrorMessage = "We're sorry, but your transaction failed.  Please verify your payment information, ensuring your card type, number, and expiration date are in the correct format, and try again.";
                }
                else
                {
                    result.ErrorMessage = String.Format("We are sorry, but during the transaction an error occured : {0}.", message);
                }
            }
        }

        private string GetResponse(string merchantID, string merchantLogin, string merchantPassword, string action, string request)
        {
            using (BPag.BPagWSService client = new BPag.BPagWSService())
            {
                return client.doService(BPagMembers.Version, action, merchantID, merchantLogin, merchantPassword, request);
                //for testing
                //return client.doService(BPagMembers.Version, action, "interlogix", "interlogix_ws", "123456", request);
            }
        }

        private string GetEncrypted(string cc)
        {
            string res = string.Empty;
            try
            {
                X509Certificate2 certificate = new X509Certificate2(@"C:/Key/bpag.cer");
                var provider = certificate.PublicKey.Key as RSACryptoServiceProvider;
                var dataBytes = System.Text.Encoding.UTF8.GetBytes(cc);
                var encryptedData = provider.Encrypt(dataBytes, false);
                res = Convert.ToBase64String(encryptedData); ;
            }
            catch (Exception ex)
            {
                res = string.Empty;
                logger.Error(GetType(), ex);
            }
            return res;
        }

        private string GetCardHolder(Billing b)
        {
            string res = b.FullName;

            try
            {
                var billInfo = dao.Load<BillingExternalInfo>(b.BillingID);
                if (billInfo != null)
                    res = billInfo.CustomField4;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public string GetTransactionAuthCode(GatewayResult gatewayResult)
        {
            return gatewayResult.ResponseParams.GetParam("authorisationNumber");
        }

        public string GetTransactionID(GatewayResult gatewayResult)
        {
            return gatewayResult.ResponseParams.GetParam("orderId");
        }

        public string GetResultText(string response)
        {
            BPagGatewayResponseParams p = new BPagGatewayResponseParams(response);
            return p.GetParam("message");
        }

        #region Interface members which not implemented

        public BusinessError<GatewayResult> AuthOnly(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, Billing billing, Product product)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Credit(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Refund(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
