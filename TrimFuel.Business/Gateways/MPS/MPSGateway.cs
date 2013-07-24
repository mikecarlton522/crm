using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using System.Net;
using TrimFuel.Business.Dao;

namespace TrimFuel.Business.Gateways.MPS
{
    public class MPSGateway : IPaymentGateway
    {
        IDao dao = null;

        public MPSGateway()
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
        }

        public BusinessError<GatewayResult> AuthOnly(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Model.Currency currency, Model.Billing billing, Model.Product product)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = prepareAuthOnly(merchantLogin,
                                                merchantPassword,
                                                amount,
                                                billing,
                                                product,
                                                currency);

            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new MPSGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Capture(string merchantLogin, string merchantPassword, Model.ChargeHistoryEx authChargeHistory, decimal amount, Model.Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = prepareCapture(merchantLogin, merchantPassword, authChargeHistory, amount, currency);

            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new MPSGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Sale(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Model.Currency currency, long saleID, Model.Billing billing, Model.Product product)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = prepareSale(saleID,
                                                merchantLogin,
                                                merchantPassword,
                                                amount,
                                                billing,
                                                product,
                                                currency);

            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new MPSGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Credit(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Model.Currency currency, long saleID, Model.Billing billing)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Refund(string merchantLogin, string merchantPassword, Model.ChargeHistoryEx chargeHistory, decimal refundAmount, Model.Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = prepareRefund(merchantLogin, merchantPassword, chargeHistory, refundAmount, currency);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new MPSGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Void(string merchantLogin, string merchantPassword, Model.ChargeHistoryEx chargeHistory, decimal refundAmount, Model.Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = prepareVoid(merchantLogin, merchantPassword, chargeHistory, refundAmount, currency);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new MPSGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public string GetTransactionAuthCode(GatewayResult gatewayResult)
        {
            return ExtractResponseParam(gatewayResult.Response, "AuthorizationCode");
        }

        public string GetTransactionID(GatewayResult gatewayResult)
        {
            return ExtractResponseParam(gatewayResult.Response, "TransactionID");
        }

        public string GetResultText(string response)
        {
            return ExtractResponseParam(response, "ResponseMessage");
        }


        private string prepareAuthOnly(string merchantLogin, string merchantPassword, decimal transactionAmount, Billing billing, Product product, Currency currency)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "TransactionType", TransactionType.CreditCardAuthorization);
            addRequestParam(result, "MerchantId", merchantLogin);
            addRequestParam(result, "MerchantKey", merchantPassword);

            addRequestParam(result, "CardNumber", billing.CreditCardCnt.DecryptedCreditCard);
            if (billing.ExpMonth.HasValue && billing.ExpYear.HasValue)
            {
                addRequestParam(result, "ExpirationDateMMYY", addLeadingZeros(billing.ExpMonth.Value, 2) + addLeadingZeros(billing.ExpYear.Value, 2));
            }
            addRequestParam(result, "CVV2", billing.CVV);
            addRequestParam(result, "TransactionAmount", Utility.FormatPrice(transactionAmount)); /*SHOULD BE IN USD*/

            addRequestParam(result, "Currency", currency == null ? "USD" : currency.CurrencyName);

            addRequestParam(result, "BillingNameFirst", billing.FirstName);
            addRequestParam(result, "BillingNameLast", billing.LastName);
            addRequestParam(result, "BillingFullName", billing.FullName);

            addRequestParam(result, "BillingAddress", billing.Address1);
            addRequestParam(result, "BillingZipCode", billing.Zip);
            addRequestParam(result, "BillingCity", billing.City);
            addRequestParam(result, "BillingState", billing.State);
            addRequestParam(result, "BillingCountry", FixCountry(billing.Country));

            addRequestParam(result, "EmailAddress", billing.Email);
            addRequestParam(result, "PhoneNumber", billing.Phone);

            if (billing.RegistrationID != null)
            {
                var registration = dao.Load<Registration>(billing.RegistrationID);                

                addRequestParam(result, "ShippingAddress1", registration.Address1);
                addRequestParam(result, "ShippingAddress2", registration.Address2);
                addRequestParam(result, "ShippingZipCode", registration.Zip);
                addRequestParam(result, "ShippingCity", registration.City);
                addRequestParam(result, "ShippingState", registration.State);
                addRequestParam(result, "ShippingCountry", FixCountry(billing.Country));

            }
            addRequestParam(result, "ClientIPAddress", billing.IP);

            addRequestParam(result, "ReferenceNumber", billing.BillingID);

            //do not sent PostBackURL, it will cause to invalid response
            //addRequestParam(result, "PostBackURL", billing.URL);

            addRequestParam(result, "CustomInfo1", billing.Affiliate);
            addRequestParam(result, "CustomInfo2", billing.SubAffiliate);

            return result.ToString();
        }

        private string prepareCapture(string merchantLogin, string merchantPassword, ChargeHistoryEx authChargeHistory, decimal amount, Currency currency)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "TransactionType", TransactionType.CreditCardSettle);
            addRequestParam(result, "MerchantId", merchantLogin);
            addRequestParam(result, "MerchantKey", merchantPassword);

            addRequestParam(result, "TransactionID", authChargeHistory.TransactionNumber);
            addRequestParam(result, "ReferenceNumber", ExtractResponseParam(authChargeHistory.Response, "ReferenceNumber"));

            return result.ToString();
        }

        private string prepareSale(long saleID, string merchantLogin, string merchantPassword, decimal transactionAmount, Billing billing, Product product, Currency currency)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "TransactionType", TransactionType.CreditCardCharge);
            result.AppendFormat("&{0}={1}", "MerchantId", merchantLogin);
            addRequestParam(result, "MerchantKey", merchantPassword);

            addRequestParam(result, "CardNumber", billing.CreditCardCnt.DecryptedCreditCard);
            if (billing.ExpMonth.HasValue && billing.ExpYear.HasValue)
            {
                addRequestParam(result, "ExpirationDateMMYY", addLeadingZeros(billing.ExpMonth.Value, 2) + addLeadingZeros(billing.ExpYear.Value, 2));
            }
            addRequestParam(result, "CVV2", billing.CVV);
            addRequestParam(result, "TransactionAmount", Utility.FormatPrice(transactionAmount)); /*SHOULD BE IN USD*/

            addRequestParam(result, "Currency", currency == null ? "USD" : currency.CurrencyName);

            addRequestParam(result, "BillingNameFirst", billing.FirstName);
            addRequestParam(result, "BillingNameLast", billing.LastName);
            addRequestParam(result, "BillingFullName", billing.FullName);

            addRequestParam(result, "BillingAddress", billing.Address1);
            addRequestParam(result, "BillingZipCode", billing.Zip);
            addRequestParam(result, "BillingCity", billing.City);
            addRequestParam(result, "BillingState", billing.State);
            addRequestParam(result, "BillingCountry", FixCountry(billing.Country));

            addRequestParam(result, "EmailAddress", billing.Email);
            addRequestParam(result, "PhoneNumber", billing.Phone);

            if (billing.RegistrationID != null)
            {
                var registration = dao.Load<Registration>(billing.RegistrationID);

                addRequestParam(result, "ShippingAddress1", registration.Address1);
                addRequestParam(result, "ShippingAddress2", registration.Address2);
                addRequestParam(result, "ShippingZipCode", registration.Zip);
                addRequestParam(result, "ShippingCity", registration.City);
                addRequestParam(result, "ShippingState", registration.State);
                addRequestParam(result, "ShippingCountry", FixCountry(billing.Country));

            }
            addRequestParam(result, "ClientIPAddress", billing.IP);

            addRequestParam(result, "ReferenceNumber", billing.BillingID);

            //do not sent PostBackURL, it will cause to invalid response
            //addRequestParam(result, "PostBackURL", billing.URL);

            addRequestParam(result, "CustomInfo1", billing.Affiliate);
            addRequestParam(result, "CustomInfo2", billing.SubAffiliate);


            return result.ToString();
        }

        private string prepareRefund(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "TransactionType", TransactionType.CreditCardCredit);
            addRequestParam(result, "MerchantId", merchantLogin);
            addRequestParam(result, "MerchantKey", merchantPassword);

            addRequestParam(result, "TransactionID", chargeHistory.TransactionNumber);
            addRequestParam(result, "ReferenceNumber", ExtractResponseParam(chargeHistory.Response, "ReferenceNumber"));
            addRequestParam(result, "TransactionAmount", Utility.FormatPrice(refundAmount)); /*SHOULD BE IN USD*/

            addRequestParam(result, "Currency", currency == null ? "USD" : currency.CurrencyName);

            return result.ToString();
        }

        private string prepareVoid(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "TransactionType", TransactionType.CreditCardVoid);
            addRequestParam(result, "MerchantId", merchantLogin);
            addRequestParam(result, "MerchantKey", merchantPassword);

            addRequestParam(result, "TransactionID", chargeHistory.TransactionNumber);
            addRequestParam(result, "ReferenceNumber", ExtractResponseParam(chargeHistory.Response, "ReferenceNumber"));

            return result.ToString();
        }



        private string GetResponse(string request)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return wc.UploadString(Config.Current.MPS_URL, "POST", request);
        }

        private void ProcessResult(BusinessError<GatewayResult> result)
        {
            string success = ExtractResponseParam(result.ReturnValue.Response, "StatusID");
            string code = ExtractResponseParam(result.ReturnValue.Response, "ResponseCode");
            string message = ExtractResponseParam(result.ReturnValue.Response, "ResponseMessage");
            if (success != null && success.ToLower() == "0")
            {
                //Success
                result.State = BusinessErrorState.Success;
            }
            else
            {
                //Error
                result.State = BusinessErrorState.Error;
                if (String.IsNullOrEmpty(code) && String.IsNullOrEmpty(message))
                {
                    result.ErrorMessage = "We're sorry, but your transaction failed.  Please verify your payment information, ensuring your card type, number, and expiration date are in the correct format, and try again.";
                }
                else
                {
                    result.ErrorMessage = String.Format("We are sorry, but during the transaction an error occured {0}: {1}.", code, message);
                }
            }
        }

        private string ExtractResponseParam(string response, string paramName)
        {
            MPSGatewayResponseParams p = new MPSGatewayResponseParams(response);
            return p.GetParam(paramName);
        }

        private void addRequestParam(StringBuilder resultString, string paramName, object paramValue)
        {
            if (paramValue != null && !String.IsNullOrEmpty(paramValue.ToString()))
            {
                resultString.AppendFormat("&{0}={1}", paramName, paramValue);
            }
        }

        private string addLeadingZeros(int value, int digitsCount)
        {
            StringBuilder s = new StringBuilder();

            for (int i = 0; i < digitsCount; i++)
            {
                s.Append("0");
            }
            s.Append(value);

            return s.ToString(s.Length - digitsCount, digitsCount);
        }

        private string FixCountry(string countryName)
        {
            if (string.IsNullOrEmpty(countryName))
                return string.Empty;

            if (countryName.Length > 3)
            {
                Geo country = new GeoService().GetCountryByName(countryName);
                if (country != null && !string.IsNullOrEmpty(country.Code))
                {
                    countryName = country.Code;
                }
            }
            else if (countryName == "USA")
            {
                countryName = "US";
            }

            if (countryName != null && countryName.ToLower() == "uk")
            {
                countryName = "GB";
            }

            return countryName;
        }
    }
}
