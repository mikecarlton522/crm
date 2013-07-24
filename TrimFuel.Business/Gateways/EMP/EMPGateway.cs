using System;
using System.Net;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model;

namespace TrimFuel.Business.Gateways.EMP
{
    public class EMPGateway : IPaymentGateway
    {
        private string GetResponse(string request)
        {
            WebClient wc = new WebClient();
            
            return wc.UploadString(Config.Current.EMP_URL, "POST", request);
        }

        private void ProcessResult(BusinessError<GatewayResult> result)
        {
            string success = ExtractResponseParam(result.ReturnValue.Response, "result");

            
            if (success != null && success.ToLower() == "a")
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

            paymentResult.ResponseParams = new EMPGatewayResponseParams(paymentResult.Response);

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

            paymentResult.ResponseParams = new EMPGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Credit(string merchantID, string merchantLogin, string merchantPassword, decimal amount, TrimFuel.Model.Currency currency, long saleID, TrimFuel.Model.Billing billing)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Refund(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, TrimFuel.Model.Currency currency)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Void(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, TrimFuel.Model.Currency currency)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Capture((string merchantLogin, string merchantPassword, ChargeHistoryEx authChargeHistory, decimal amount, Currency currency)
        {
            throw new NotImplementedException();
        }

        public string GetTransactionAuthCode(GatewayResult gatewayResult)
        {
            throw new NotImplementedException();
        }

        public string GetTransactionID(GatewayResult gatewayResult)
        {
            throw new NotImplementedException();
        }

        private string PrepareSale(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing, Product product)
        {
            string request = Utility.LoadFromEmbeddedResource(typeof(EMPGateway), "emp_sale.txt");

            request = request.Replace("##GATEWAY_REFERENCE##", merchantID);
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
            request = request.Replace("##B_CITY##", billing.City);
            request = request.Replace("##B_STATE##", billing.State);
            request = request.Replace("##B_COUNTRY##", billing.Country);
            request = request.Replace("##B_ZIP##", billing.Zip);
            
            request = request.Replace("##B_EMAIL##", billing.Email);
            request = request.Replace("##IP##", billing.IP);

            return request;
        }

        private string PrepareAuthOnly(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, Billing billing)
        {
            string request = Utility.LoadFromEmbeddedResource(typeof(EMPGateway), "emp_auth_only.xml");

            request = request.Replace("##GATEWAY_REFERENCE##", merchantID);
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
            
            request = request.Replace("##B_CITY##", billing.City);
            request = request.Replace("##B_STATE##", billing.State);
            request = request.Replace("##B_COUNTRY##", billing.Country);
            request = request.Replace("##B_ZIP##", billing.Zip);
            
            request = request.Replace("##B_EMAIL##", billing.Email);
            request = request.Replace("##IP##", billing.IP);

            return request;
        }

        #endregion

        private string ExtractResponseParam(string response, string paramName)
        {
            EMPGatewayResponseParams p = new EMPGatewayResponseParams(response);
            return p.GetParam(paramName);
        }
    }
}
