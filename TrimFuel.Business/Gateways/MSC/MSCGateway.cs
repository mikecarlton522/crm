﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.IO;
using TrimFuel.Business.Utils;
using System.Net;

namespace TrimFuel.Business.Gateways.MSC
{
    public class MSCGateway : IPaymentGateway
    {
        private const string CURRENCY_TEMPLATE = "&currency={0}";

        private string GetResponse(string request)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(Config.Current.MSC_URL + "?" + request);
        }

        private void ProcessResult(BusinessError<GatewayResult> result)
        {
            string success =  ExtractResponseParam(result.ReturnValue.Response, "success");
            if (success != null && success.ToLower() == "yes")
            {
                //Success
                result.State = BusinessErrorState.Success;
            }
            else
            {
                //Error
                result.State = BusinessErrorState.Error;
                result.ErrorMessage = "We're sorry, but your transaction failed.  Please verify your payment information, ensuring your card type, number, and expiration date are in the correct format, and try again.";
            }
        }

        #region IPaymentGateway Members

        public BusinessError<GatewayResult> Sale(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing, Product product)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = PrepareSale(merchantID, merchantLogin, merchantPassword, amount, currency, saleID, billing, product);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new MSCGatewayResponseParams(paymentResult.Response);

            return res;

        }

        public BusinessError<GatewayResult> AuthOnly(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, Billing billing, Product product)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Credit(string merchantID, string merchantLogin, string merchantPassword, decimal amount, TrimFuel.Model.Currency currency, long saleID, TrimFuel.Model.Billing billing)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Refund(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, TrimFuel.Model.Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = PrepareRefund(merchantLogin, merchantPassword, chargeHistory, refundAmount, currency);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new MSCGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Void(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, TrimFuel.Model.Currency currency)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();
            paymentResult.Request = PrepareVoid(merchantLogin, merchantPassword, chargeHistory, refundAmount, currency);
            paymentResult.Response = GetResponse(paymentResult.Request);

            res.ReturnValue = paymentResult;
            ProcessResult(res);

            paymentResult.ResponseParams = new MSCGatewayResponseParams(paymentResult.Response);

            return res;
        }

        private string PrepareSale(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing, Product product)
        {
            string request = Utility.LoadFromEmbeddedResource(typeof(MSCGateway), "msc_sale.txt");

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
            request = request.Replace("##B_COUNTRY##", billing.Country);
            request = request.Replace("##B_ZIP##", billing.Zip);
            request = request.Replace("##B_PHONE##", billing.Phone);
            request = request.Replace("##B_EMAIL##", billing.Email);
            request = request.Replace("##IP##", billing.IP);

            request = request.Replace("##AFF##", billing.Affiliate);
            request = request.Replace("##SUBAFF##", billing.SubAffiliate);

            request = request.Replace("##CAMPAIGN_ID##", ConvertToString(billing.CampaignID));
            request = request.Replace("##PRODUCT_NAME##", product.ProductName);

            request = ApplyCurrency(request, currency);

            return request;
        }

        private string ConvertToString(int? val)
        {
            if (val == null)
            {
                return string.Empty;
            }
            return val.Value.ToString();
        }

        private string PrepareVoid(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(MSCGateway), "msc_void.txt");

            res = res.Replace("##GATEWAY_USERNAME##", merchantLogin);
            res = res.Replace("##GATEWAY_PASSWORD##", merchantPassword);
            res = res.Replace("##TXN_NUMBER##", chargeHistory.TransactionNumber);

            res = ApplyCurrency(res, currency);

            return res;
        }

        private string PrepareRefund(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(MSCGateway), "msc_refund.txt");

            res = res.Replace("##GATEWAY_USERNAME##", merchantLogin);
            res = res.Replace("##GATEWAY_PASSWORD##", merchantPassword);
            res = res.Replace("##TXN_NUMBER##", chargeHistory.TransactionNumber);
            res = res.Replace("##AMOUNT##", Utility.FormatPrice(refundAmount));

            res = ApplyCurrency(res, currency);

            return res;
        }

        public string GetTransactionAuthCode(GatewayResult gatewayResult)
        {
            return ExtractResponseParam(gatewayResult.Response, "authcode");
        }

        public string GetTransactionID(GatewayResult gatewayResult)
        {
            return ExtractResponseParam(gatewayResult.Response, "referenceid");
        }

        #endregion

        private string ApplyCurrency(string request, Currency currency)
        {
            if (currency != null)
            {
                return request + string.Format(CURRENCY_TEMPLATE, currency.CurrencyName);
            }
            return request;
        }

        private string ExtractResponseParam(string response, string paramName)
        {
            MSCGatewayResponseParams p = new MSCGatewayResponseParams(response);
            return p.GetParam(paramName);
        }

        #region IPaymentGateway Members


        public BusinessError<GatewayResult> Capture(string merchantLogin, string merchantPassword, ChargeHistoryEx authChargeHistory, decimal amount, Currency currency)
        {
            throw new NotImplementedException();
        }

        public string GetResultText(string response)
        {
            return ExtractResponseParam(response, "message");
        }

        #endregion
    }
}
