using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Utils;
using TrimFuel.Model;

namespace TrimFuel.Business.Gateways.CBG
{
    public class CBGGateway : IPaymentGateway
    {
        IDao dao = null;

        public CBGGateway()
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
        }

        public BusinessError<GatewayResult> AuthOnly(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, Billing billing, Product product)
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

            paymentResult.ResponseParams = new CBGGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Sale(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing, Product product)
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

            paymentResult.ResponseParams = new CBGGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Credit(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Refund(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = prepareRefund(merchantLogin, merchantPassword, chargeHistory, refundAmount, currency);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new CBGGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Capture(string merchantLogin, string merchantPassword, ChargeHistoryEx authChargeHistory, decimal amount, Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = prepareCapture(merchantLogin, merchantPassword, authChargeHistory, amount, currency);

            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new CBGGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Void(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = prepareVoid(merchantLogin, merchantPassword, chargeHistory, refundAmount, currency);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new CBGGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public string GetTransactionAuthCode(GatewayResult gatewayResult)
        {
            return ExtractResponseParam(gatewayResult.Response, "authcode");
        }

        public string GetTransactionID(GatewayResult gatewayResult)
        {
            return ExtractResponseParam(gatewayResult.Response, "transaction_id");
        }

        public string GetResultText(string response)
        {
            throw new NotImplementedException();
        }

        public int? GetProductId(int? billingSubscriptionID)
        {
            var billingSubscription = dao.Load<BillingSubscription>(billingSubscriptionID);
            var subscription = dao.Load<Subscription>(billingSubscription.SubscriptionID);
            return subscription.ProductID.Value;
        }


        private string prepareAuthOnly(string merchantLogin, string merchantPassword, decimal transactionAmount, Billing billing, Product product, Currency currency)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "type", TransactionType.Authorization);

            addRequestParam(result, "username", merchantLogin);
            addRequestParam(result, "password", merchantPassword);
            addRequestParam(result, "cc_number", billing.CreditCardCnt.DecryptedCreditCard);
            addRequestParam(result, "cc_exp", addCreditCardExpiration(billing.ExpMonth.Value, billing.ExpYear.Value));
            addRequestParam(result, "cvv", billing.CVV);
            addRequestParam(result, "amount", Utility.FormatPrice(transactionAmount)); /*SHOULD BE IN USD*/
            addRequestParam(result, "currency", currency == null ? "USD" : currency.CurrencyName);
            //addRequestParam(result, "product_id", product.ProductID);
            addRequestParam(result, "product_id", Config.Current.CBG_PRODUCT_ID);
            addRequestParam(result, "ip_address", billing.IP);
            addRequestParam(result, "billing_method", "");
            addRequestParam(result, "processor_id", "");
            addRequestParam(result, "dup_seconds", "");
            addRequestParam(result, "descriptor", "");
            addRequestParam(result, "descriptor_phone", "");
            addRequestParam(result, "product_sku_#", "");
            addRequestParam(result, "order_id", "");
            addRequestParam(result, "order_description", "");
            addRequestParam(result, "tax", "");
            addRequestParam(result, "shipping", "");
            addRequestParam(result, "po_number", "");
            addRequestParam(result, "first_name", billing.FirstName);
            addRequestParam(result, "last_name", billing.LastName);
            addRequestParam(result, "company", "");
            addRequestParam(result, "address1", billing.Address1);
            addRequestParam(result, "address2", billing.Address2);
            addRequestParam(result, "city", billing.City);
            addRequestParam(result, "state", billing.State);
            addRequestParam(result, "zip", billing.Zip);
            addRequestParam(result, "country", FixCountry(billing.Country));
            addRequestParam(result, "phone", billing.Phone);
            addRequestParam(result, "alt_phone", "");
            addRequestParam(result, "fax", "");
            addRequestParam(result, "email", billing.Email);

            if (billing.RegistrationID != null)
            {
                var registration = dao.Load<Registration>(billing.RegistrationID);

                addRequestParam(result, "shipping_firstname", registration.FirstName);
                addRequestParam(result, "shipping_lastname", registration.LastName);
                addRequestParam(result, "shipping_company", "");
                addRequestParam(result, "shipping_address1", registration.Address1);
                addRequestParam(result, "shipping_address2", registration.Address2);
                addRequestParam(result, "shipping_city", registration.City);
                addRequestParam(result, "shipping_state", registration.State);
                addRequestParam(result, "shipping_zip", registration.Zip);
                addRequestParam(result, "shipping_country", FixCountry(billing.Country));
                addRequestParam(result, "shipping_email", registration.Email);
            }

            addRequestParam(result, "merchant_defined_field_12", billing.Affiliate);
            addRequestParam(result, "merchant_defined_field_13", billing.SubAffiliate);
            addRequestParam(result, "merchant_defined_field_14", "");
            addRequestParam(result, "merchant_defined_field_15", "");
            addRequestParam(result, "merchant_defined_field_16", "");
            addRequestParam(result, "merchant_defined_field_17", "");
            addRequestParam(result, "merchant_defined_field_18", "");
            addRequestParam(result, "merchant_defined_field_19", "");
            addRequestParam(result, "merchant_defined_field_20", "");
            addRequestParam(result, "merchant_defined_field_#", "");

            return result.ToString();
        }

        private string prepareCapture(string merchantLogin, string merchantPassword, ChargeHistoryEx authChargeHistory, decimal amount, Currency currency)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "type", TransactionType.Capture);
            addRequestParam(result, "company_id", merchantLogin);
            addRequestParam(result, "company_key", merchantPassword);

            addRequestParam(result, "transaction_id", authChargeHistory.TransactionNumber);
            addRequestParam(result, "amount", Utility.FormatPrice(amount));
            addRequestParam(result, "product_id", Config.Current.CBG_PRODUCT_ID);
            //addRequestParam(result, "product_id", GetProductId(authChargeHistory.BillingSubscriptionID));

            return result.ToString();
        }

        private string prepareSale(long saleID, string merchantLogin, string merchantPassword, decimal transactionAmount, Billing billing, Product product, Currency currency)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "type", TransactionType.Sale);

            addRequestParam(result, "company_id", merchantLogin);
            addRequestParam(result, "company_key", merchantPassword);
            addRequestParam(result, "cc_number", billing.CreditCardCnt.DecryptedCreditCard);
            addRequestParam(result, "cc_exp", addCreditCardExpiration(billing.ExpMonth.Value, billing.ExpYear.Value));
            addRequestParam(result, "cvv", billing.CVV);
            addRequestParam(result, "amount", Utility.FormatPrice(transactionAmount)); /*SHOULD BE IN USD*/
            addRequestParam(result, "currency", currency == null ? "USD" : currency.CurrencyName);
            //addRequestParam(result, "product_id", product.ProductID);
            addRequestParam(result, "product_id", Config.Current.CBG_PRODUCT_ID);
            addRequestParam(result, "ip_address", billing.IP);
            addRequestParam(result, "billing_method", "");
            addRequestParam(result, "processor_id", "");
            addRequestParam(result, "dup_seconds", "");
            addRequestParam(result, "descriptor", "");
            addRequestParam(result, "descriptor_phone", "");
            addRequestParam(result, "product_sku_#", "");
            if (saleID > 0)
                addRequestParam(result, "order_id", saleID.ToString());
            else
                addRequestParam(result, "order_id", "");
            addRequestParam(result, "order_description", "");
            addRequestParam(result, "tax", "");
            addRequestParam(result, "shipping", "");
            addRequestParam(result, "po_number", "");
            addRequestParam(result, "first_name", billing.FirstName);
            addRequestParam(result, "last_name", billing.LastName);
            addRequestParam(result, "company", "");
            addRequestParam(result, "address1", billing.Address1);
            addRequestParam(result, "address2", billing.Address2);
            addRequestParam(result, "city", billing.City);
            addRequestParam(result, "state", billing.State);
            addRequestParam(result, "zip", billing.Zip);
            addRequestParam(result, "country", FixCountry(billing.Country));
            addRequestParam(result, "phone", billing.Phone);
            addRequestParam(result, "alt_phone", "");
            addRequestParam(result, "fax", "");
            addRequestParam(result, "email", billing.Email);

            if (billing.RegistrationID != null)
            {
                var registration = dao.Load<Registration>(billing.RegistrationID);

                addRequestParam(result, "shipping_firstname", registration.FirstName);
                addRequestParam(result, "shipping_lastname", registration.LastName);
                addRequestParam(result, "shipping_company", "");
                addRequestParam(result, "shipping_address1", registration.Address1);
                addRequestParam(result, "shipping_address2", registration.Address2);
                addRequestParam(result, "shipping_city", registration.City);
                addRequestParam(result, "shipping_state", registration.State);
                addRequestParam(result, "shipping_zip", registration.Zip);
                addRequestParam(result, "shipping_country", FixCountry(billing.Country));
                addRequestParam(result, "shipping_email", registration.Email);
            }

            addRequestParam(result, "merchant_defined_field_12", billing.Affiliate);
            addRequestParam(result, "merchant_defined_field_13", billing.SubAffiliate);
            addRequestParam(result, "merchant_defined_field_14", "");
            addRequestParam(result, "merchant_defined_field_15", "");
            addRequestParam(result, "merchant_defined_field_16", "");
            addRequestParam(result, "merchant_defined_field_17", "");
            addRequestParam(result, "merchant_defined_field_18", "");
            addRequestParam(result, "merchant_defined_field_19", "");
            addRequestParam(result, "merchant_defined_field_20", "");
            addRequestParam(result, "merchant_defined_field_#", "");

            return result.ToString();
        }

        private string prepareRefund(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "type", TransactionType.Refund);
            addRequestParam(result, "company_id", merchantLogin);
            addRequestParam(result, "company_key", merchantPassword);

            addRequestParam(result, "transaction_id", chargeHistory.TransactionNumber);
            //addRequestParam(result, "product_id", GetProductId(chargeHistory.BillingSubscriptionID));
            addRequestParam(result, "product_id", Config.Current.CBG_PRODUCT_ID);

            return result.ToString();
        }

        private string prepareVoid(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "type", TransactionType.Void);
            addRequestParam(result, "company_id", merchantLogin);
            addRequestParam(result, "company_key", merchantPassword);
            addRequestParam(result, "transaction_id", chargeHistory.TransactionNumber);
            //addRequestParam(result, "product_id", GetProductId(chargeHistory.BillingSubscriptionID));
            addRequestParam(result, "product_id", Config.Current.CBG_PRODUCT_ID);

            return result.ToString();
        }

        private string GetResponse(string request)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return wc.UploadString(Config.Current.CBG_URL, "POST", request);
        }

        private void ProcessResult(BusinessError<GatewayResult> result)
        {
            string responseText = ExtractResponseParam(result.ReturnValue.Response, "responsetext");
            string responseCode = ExtractResponseParam(result.ReturnValue.Response, "response_code");
            string cbg_id = ExtractResponseParam(result.ReturnValue.Response, "cbg_id");
            if (responseCode == "100")
            {
                //Success
                result.State = BusinessErrorState.Success;
            }
            else
            {
                //Error
                result.State = BusinessErrorState.Error;
                if (String.IsNullOrEmpty(responseCode) && String.IsNullOrEmpty(responseText))
                {
                    result.ErrorMessage = "We're sorry, but your transaction failed.  Please verify your payment information, ensuring your card type, number, and expiration date are in the correct format, and try again.";
                }
                else
                {
                    result.ErrorMessage = String.Format("We are sorry, but during the transaction an error occured {0}: {1}.", responseCode, responseText);
                }
            }
        }

        private string ExtractResponseParam(string response, string paramName)
        {
            CBGGatewayResponseParams p = new CBGGatewayResponseParams(response);
            return p.GetParam(paramName);
        }

        private void addRequestParam(StringBuilder resultString, string paramName, object paramValue)
        {
            if (paramValue != null && !String.IsNullOrEmpty(paramValue.ToString()))
            {
                resultString.AppendFormat("&{0}={1}", paramName, paramValue);
            }
        }

        private string addCreditCardExpiration(int month, int year)
        {
            var _month = month.ToString().Length == 2 ? month.ToString() : "0" + month.ToString();
            var _year = year.ToString().Substring(2);

            return _month + _year;
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
                    return country.Code;
                }
            }
            else if (countryName == "USA")
            {
                return "US";
            }
            return countryName;
        }
    }
}
