using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.IO;
using TrimFuel.Business.Utils;
using System.Net;


namespace TrimFuel.Business.Gateways.NetworkMerchants
{
    public class NetworkMerchantsGateway : IPaymentGateway
    {
        private const string CURRENCY_TEMPLATE = "&currency={0}";
        private const string NO_CVV_TEMPLATE = "&billing_method=recurring";

        private string GetResponse(string request)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return wc.UploadString(Config.Current.NETWORKMERCHANTS_URL, "POST", request);
        }

        private void ProcessResult(BusinessError<GatewayResult> result)
        {
            int? responseCode = ExtractResponseParamInt(result.ReturnValue.Response, "response");
            if (responseCode != null && responseCode.Value == 1)
            {
                //Success
                result.State = BusinessErrorState.Success;
            }
            else
            {
                //Error
                result.State = BusinessErrorState.Error;
                int? errorCode = ExtractResponseParamInt(result.ReturnValue.Response, "response_code");
                if (responseCode == null || errorCode == null)
                {
                    result.ErrorMessage = "We're sorry, but your transaction failed. Unknown error occured while performing transaction.";
                }
                else
                {
                    string errorLevel1 = null;
                    string errorLevel2 = null;

                    if (errorCode.Value >= 200 && errorCode.Value < 300)
                    {
                        errorLevel1 = "We're sorry, but your transaction failed.  Please verify your payment information, ensuring your card type, number, and expiration date are in the correct format, and try again.";
                        //errorLevel1 = "Transaction was declined by processor.";
                        if (errorCode.Value == 202)
                        {
                            errorLevel2 = "Insufficient funds.";
                        }
                        else if (errorCode.Value == 203)
                        {
                            errorLevel2 = "Over limit.";
                        }
                        else if (errorCode.Value == 220)
                        {
                            errorLevel2 = "Incorrect payment data.";
                        }
                        else if (errorCode.Value == 223)
                        {
                            errorLevel2 = "Expired card.";
                        }
                        else if (errorCode.Value == 224)
                        {
                            errorLevel2 = "Invalid expiration date.";
                        }
                        else if (errorCode.Value == 225)
                        {
                            errorLevel2 = "Invalid card security code.";
                        }
                        else if (errorCode.Value == 220)
                        {
                            errorLevel2 = "Incorrect payment data.";
                        }
                        else if (errorCode.Value == 240)
                        {
                            errorLevel2 = "Call issuer for futher information.";
                        }
                    }
                    else if (errorCode.Value >= 300 && errorCode.Value < 400)
                    {
                        errorLevel1 = "We're sorry, but your transaction was refused by your credit card company. Please contact your card issuer for information or try again with a different card.";
                        //errorLevel1 = "Transaction was rejected by gateway.";
                    }
                    else
                    {
                        errorLevel1 = "We're sorry, but your transaction failed.  Please verify your payment information, ensuring your card type, number, and expiration date are in the correct format, and try again.";
                    }

                    result.ErrorMessage = errorLevel1;
                    if (errorLevel2 != null)
                    {
                        result.ErrorMessage += string.Format(" Error: {0}", errorLevel2);
                    }
                }
            }
        }

        #region IPaymentGateway Members

        public BusinessError<GatewayResult> Sale(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing, Product product)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = PrepareSale(merchantID, merchantLogin, merchantPassword, amount, currency, saleID, billing);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new NetworkMerchantsGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> AuthOnly(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, Billing billing, Product product)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = PrepareAuthOnly(merchantID, merchantLogin, merchantPassword, amount, currency, billing);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new NetworkMerchantsGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Capture(string merchantLogin, string merchantPassword, ChargeHistoryEx authChargeHistory, decimal amount, Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = PrepareCapture(merchantLogin, merchantPassword, authChargeHistory, amount, currency);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new NetworkMerchantsGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Credit(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = PrepareCredit(merchantID, merchantLogin, merchantPassword, amount, currency, saleID, billing);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new NetworkMerchantsGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Void(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = PrepareVoid(merchantLogin, merchantPassword, chargeHistory, refundAmount, currency);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new NetworkMerchantsGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Refund(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = PrepareRefund(merchantLogin, merchantPassword, chargeHistory, refundAmount, currency);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new NetworkMerchantsGatewayResponseParams(paymentResult.Response);

            return res;
        }

        #endregion

        private string PrepareSale(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing)
        {
            string request = Utility.LoadFromEmbeddedResource(typeof(NetworkMerchantsGateway), "nmi_purchase_eu.xml");

            request = request.Replace("##MID##", merchantID);
            request = request.Replace("##GATEWAY_USERNAME##", merchantLogin);
            request = request.Replace("##GATEWAY_PASSWORD##", merchantPassword);

            request = request.Replace("##AMOUNT##", Utility.FormatPrice(amount));

            request = request.Replace("##CC_NUM##", billing.CreditCardCnt.DecryptedCreditCard);
            request = request.Replace("##CC_MONTH##", billing.ExpMonth.Value.ToString("00"));
            request = request.Replace("##CC_YEAR##", billing.ExpYear.Value.ToString("00"));
            request = request.Replace("##CVD##", billing.CVV);

            if (saleID > 0)
            {
                request = request.Replace("##BILLING_ID##", saleID.ToString());
            }
            else
            {
                request = request.Replace("##BILLING_ID##", "");
            }
            request = request.Replace("##B_F_NAME##", billing.FirstName);
            request = request.Replace("##B_L_NAME##", billing.LastName);
            request = request.Replace("##B_ADDRESS##", billing.Address1);
            request = request.Replace("##B_ADDRESS2##", billing.Address2);
            request = request.Replace("##B_CITY##", billing.City);
            request = request.Replace("##B_STATE##", billing.State);
            request = request.Replace("##B_COUNTRY##", FixCountry(billing.Country));
            request = request.Replace("##B_ZIP##", billing.Zip);
            request = request.Replace("##B_PHONE##", billing.Phone);
            request = request.Replace("##B_EMAIL##", billing.Email);
            request = request.Replace("##IP##", billing.IP);

            request = request.Replace("##AFF##", billing.Affiliate);
            request = request.Replace("##SUBAFF##", billing.SubAffiliate);

            request = ApplyCurrency(request, currency);
            request = ApplyNoCVV(request, billing.CVV);

            return request;
        }

        private string PrepareAuthOnly(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, Billing billing)
        {
            string request = Utility.LoadFromEmbeddedResource(typeof(NetworkMerchantsGateway), "nmi_auth_only.xml");

            request = request.Replace("##MID##", merchantID);
            request = request.Replace("##GATEWAY_USERNAME##", merchantLogin);
            request = request.Replace("##GATEWAY_PASSWORD##", merchantPassword);

            request = request.Replace("##AMOUNT##", Utility.FormatPrice(amount));

            request = request.Replace("##CC_NUM##", billing.CreditCardCnt.DecryptedCreditCard);
            request = request.Replace("##CC_MONTH##", billing.ExpMonth.Value.ToString("00"));
            request = request.Replace("##CC_YEAR##", billing.ExpYear.Value.ToString("00"));
            request = request.Replace("##CVD##", billing.CVV);

            request = request.Replace("##B_F_NAME##", billing.FirstName);
            request = request.Replace("##B_L_NAME##", billing.LastName);
            request = request.Replace("##B_ADDRESS##", billing.Address1);
            request = request.Replace("##B_ADDRESS2##", billing.Address2);
            request = request.Replace("##B_CITY##", billing.City);
            request = request.Replace("##B_STATE##", billing.State);
            request = request.Replace("##B_COUNTRY##", FixCountry(billing.Country));
            request = request.Replace("##B_ZIP##", billing.Zip);
            request = request.Replace("##B_PHONE##", billing.Phone);
            request = request.Replace("##B_EMAIL##", billing.Email);
            request = request.Replace("##IP##", billing.IP);

            request = request.Replace("##AFF##", billing.Affiliate);
            request = request.Replace("##SUBAFF##", billing.SubAffiliate);

            request = ApplyCurrency(request, currency);
            request = ApplyNoCVV(request, billing.CVV);

            return request;
        }

        private string PrepareCredit(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing)
        {
            string request = Utility.LoadFromEmbeddedResource(typeof(NetworkMerchantsGateway), "nmi_credit_eu.xml");

            request = request.Replace("##MID##", merchantID);
            request = request.Replace("##GATEWAY_USERNAME##", merchantLogin);
            request = request.Replace("##GATEWAY_PASSWORD##", merchantPassword);

            request = request.Replace("##AMOUNT##", Utility.FormatPrice(amount));

            request = request.Replace("##CC_NUM##", billing.CreditCardCnt.DecryptedCreditCard);
            request = request.Replace("##CC_MONTH##", billing.ExpMonth.Value.ToString("00"));
            request = request.Replace("##CC_YEAR##", billing.ExpYear.Value.ToString("00"));
            request = request.Replace("##CVD##", billing.CVV);

            request = request.Replace("##BILLING_ID##", saleID.ToString());
            request = request.Replace("##B_F_NAME##", billing.FirstName);
            request = request.Replace("##B_L_NAME##", billing.LastName);
            request = request.Replace("##B_ADDRESS##", billing.Address1);
            request = request.Replace("##B_ADDRESS2##", billing.Address2);
            request = request.Replace("##B_CITY##", billing.City);
            request = request.Replace("##B_STATE##", billing.State);
            request = request.Replace("##B_COUNTRY##", FixCountry(billing.Country));
            request = request.Replace("##B_ZIP##", billing.Zip);
            request = request.Replace("##B_PHONE##", billing.Phone);
            request = request.Replace("##B_EMAIL##", billing.Email);
            request = request.Replace("##IP##", billing.IP);

            request = ApplyCurrency(request, currency);

            return request;
        }

        private string PrepareVoid(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(NetworkMerchantsGateway), "nmi_void.xml");

            res = res.Replace("##GATEWAY_USERNAME##", merchantLogin);
            res = res.Replace("##GATEWAY_PASSWORD##", merchantPassword);
            res = res.Replace("##TXN_NUMBER##", chargeHistory.TransactionNumber);
            res = res.Replace("##AMOUNT##", Utility.FormatPrice(refundAmount));
            
            res = ApplyCurrency(res, currency);

            return res;
        }

        private string PrepareRefund(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(NetworkMerchantsGateway), "nmi_refund.xml");

            res = res.Replace("##GATEWAY_USERNAME##", merchantLogin);
            res = res.Replace("##GATEWAY_PASSWORD##", merchantPassword);
            res = res.Replace("##TXN_NUMBER##", chargeHistory.TransactionNumber);
            res = res.Replace("##AMOUNT##", Utility.FormatPrice(refundAmount));
            
            res = ApplyCurrency(res, currency);

            return res;
        }

        private string PrepareCapture(string merchantLogin, string merchantPassword, ChargeHistoryEx authChargeHistory, decimal amount, Currency currency)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(NetworkMerchantsGateway), "nmi_capture.xml");

            res = res.Replace("##GATEWAY_USERNAME##", merchantLogin);
            res = res.Replace("##GATEWAY_PASSWORD##", merchantPassword);
            res = res.Replace("##TXN_NUMBER##", authChargeHistory.TransactionNumber);
            res = res.Replace("##AMOUNT##", Utility.FormatPrice(amount));

            res = ApplyCurrency(res, currency);

            return res;
        }

        public string GetTransactionAuthCode(GatewayResult gatewayResult)
        {
            return ExtractResponseParam(gatewayResult.Response, "authcode");
        }

        public string GetTransactionID(GatewayResult gatewayResult)
        {
            return ExtractResponseParam(gatewayResult.Response, "transactionid");
        }

        private string ApplyCurrency(string request, Currency currency)
        {
            if (currency != null)
            {
                return request + string.Format(CURRENCY_TEMPLATE, currency.CurrencyName);
            }
            return request;
        }

        private string ApplyNoCVV(string request, string cvv)
        {
            if (string.IsNullOrEmpty(cvv))
            {
                return request += NO_CVV_TEMPLATE;
            }
            return request;
        }

        private int? ExtractResponseParamInt(string response, string paramName)
        {
            int res = 0;
            if (int.TryParse(ExtractResponseParam(response, paramName), out res))
            {
                return res;
            }
            return null;
        }

        private string ExtractResponseParam(string response, string paramName)
        {
            NetworkMerchantsGatewayResponseParams p = new NetworkMerchantsGatewayResponseParams(response);
            return p.GetParam(paramName);
        }

        public string GetResultText(string response)
        {
            return ExtractResponseParam(response, "responsetext");
        }

        private string FixCountry(string country)
        {
            if (country == null)
            {
                return null;
            }
            if (country.ToLower() == "uk" ||
                country.ToLower() == "united kingdom")
            {
                return "GB";
            }
            return country;
        }
    }
}
