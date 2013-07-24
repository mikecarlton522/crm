using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.IO;
using TrimFuel.Business.Utils;
using System.Net;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Gateways.Pagador
{
    public class PagadorGateway : IPaymentGateway
    {
        private void ProcessResult(BusinessError<GatewayResult> res, PagadorWService.PagadorReturn result)
        {
            if (result.status <= 1)
            {
                //Success
                res.State = BusinessErrorState.Success;
            }
            else
            {
                //Error
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = String.Format("We are sorry, but during the transaction an error occured {0}: {1}.", result.returnCode, result.message);
            }
        }

        #region IPaymentGateway Members

        public BusinessError<GatewayResult> Sale(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, long saleID, Billing billing, Product product)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();
            GatewayResult paymentResult = new GatewayResult();

            string OrderId = "";
            if (saleID > 0)
                OrderId = saleID.ToString();
            else
                OrderId = billing.BillingID.ToString();
            string Price = FormatPrice(amount);
            paymentResult.Request = ConvertRequest("Charge", merchantPassword, amount, currency, OrderId, billing);

            using (PagadorWService.Pagador service = new PagadorWService.Pagador())
            {
                PagadorWService.PagadorReturn result = service.Authorize(merchantPassword, OrderId, GetCustomerName(billing), Price,
                    GetPaymentMethod(billing.CreditCardCnt.TryGetCardType()).ToString(), GetCustomerName(billing), billing.CreditCardCnt.DecryptedCreditCard,
                    GetExpiration(billing), billing.CVV, PaymentExVars.Installments.ToString(), PagadorPaymentEnum.One_Off.ToString());

                paymentResult.Response = ConvertResponse(result, OrderId);
                res.ReturnValue = paymentResult;
                ProcessResult(res, result);

                if (result.status == 1)
                {
                    PagadorWService.PagadorReturn capture_result = service.Capture(merchantPassword, OrderId);

                    paymentResult.Response = string.Format("status={0}&returnCode={1}&message={2}&orderId={3}&transactionId={4}&authorisationNumber={5}&amount={6}",
                        capture_result.status, capture_result.returnCode, capture_result.message, OrderId, result.transactionId, result.authorisationNumber, result.amount);
                    res.ReturnValue = paymentResult;
                    ProcessResult(res, capture_result);
                }

                paymentResult.ResponseParams = new PagadorGatewayResponseParams(paymentResult.Response);
            }

            return res;
        }

        public BusinessError<GatewayResult> AuthOnly(string merchantID, string merchantLogin, string merchantPassword, decimal amount, Currency currency, Billing billing, Product product)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>();

            GatewayResult paymentResult = new GatewayResult();

            string OrderId = billing.BillingID.ToString();
            paymentResult.Request = ConvertRequest("Auth", merchantPassword, amount, currency, OrderId, billing);

            using (PagadorWService.Pagador service = new PagadorWService.Pagador())
            {
                PagadorWService.PagadorReturn result = service.Authorize(merchantPassword, OrderId, GetCustomerName(billing), FormatPrice(amount),
                    GetPaymentMethod(billing.CreditCardCnt.TryGetCardType()).ToString(), GetCustomerName(billing), billing.CreditCardCnt.DecryptedCreditCard,
                    GetExpiration(billing), billing.CVV, PaymentExVars.Installments.ToString(), PagadorPaymentEnum.One_Off.ToString());

                paymentResult.Response = ConvertResponse(result, OrderId);
                res.ReturnValue = paymentResult;
                ProcessResult(res, result);
            }

            paymentResult.ResponseParams = new PagadorGatewayResponseParams(paymentResult.Response);

            return res;
        }

        public BusinessError<GatewayResult> Credit(string merchantID, string merchantLogin, string merchantPassword, decimal amount, TrimFuel.Model.Currency currency, long saleID, TrimFuel.Model.Billing billing)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Capture(string merchantLogin, string merchantPassword, ChargeHistoryEx authChargeHistory, decimal amount, Currency currency)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Void(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            throw new NotImplementedException();
        }

        public BusinessError<GatewayResult> Refund(string merchantLogin, string merchantPassword, ChargeHistoryEx chargeHistory, decimal refundAmount, Currency currency)
        {
            throw new NotImplementedException();
        }

        #endregion

        private string ConvertRequest(string method, string merchantPassword, decimal amount, Currency currency, string orderId, Billing billing)
        {
            StringBuilder result = new StringBuilder();

            result.AppendFormat("{0}={1}", "method", method);
            addRequestParam(result, "merchantId", merchantPassword);
            addRequestParam(result, "orderId", orderId);
            addRequestParam(result, "customerName", GetCustomerName(billing));
            addRequestParam(result, "amount", FormatPrice(amount));
            addRequestParam(result, "paymentMethod", GetPaymentMethod(billing.CreditCardCnt.TryGetCardType()).ToString());
            addRequestParam(result, "holder", GetCustomerName(billing));
            addRequestParam(result, "cardNumber", billing.CreditCardCnt.DecryptedCreditCard);
            addRequestParam(result, "expiration", GetExpiration(billing));
            addRequestParam(result, "securityCode", billing.CVV);
            addRequestParam(result, "numberPayments", PaymentExVars.Installments.ToString());
            addRequestParam(result, "typePayment", PagadorPaymentEnum.One_Off.ToString());

            return result.ToString();
        }

        private string ConvertResponse(PagadorWService.PagadorReturn result, string OrderId)
        {
            return string.Format("status={0}&returnCode={1}&message={2}&orderId={3}&transactionId={4}&authorisationNumber={5}&amount={6}",
                result.status, result.returnCode, result.message, OrderId, result.transactionId, result.authorisationNumber, result.amount);
        }

        public string GetTransactionAuthCode(GatewayResult gatewayResult)
        {
            return ExtractResponseParam(gatewayResult.Response, "authorisationNumber");
        }

        public string GetTransactionID(GatewayResult gatewayResult)
        {
            return ExtractResponseParam(gatewayResult.Response, "orderId");
        }

        private string ExtractResponseParam(string response, string paramName)
        {
            PagadorGatewayResponseParams p = new PagadorGatewayResponseParams(response);
            return p.GetParam(paramName);
        }

        public int GetPaymentMethod(int? paymentMethod)
        {
            if (paymentMethod == null)
                return 0;

            switch (paymentMethod.Value)
            {
                case PaymentTypeEnum.Visa:
                    return PaymentMethodEnum.Cielo_Visa_Captura;
                case PaymentTypeEnum.Mastercard:
                    return PaymentMethodEnum.Cielo_Mastercard_Captura;
                case PaymentTypeEnum.AmericanExpress:
                    return PaymentMethodEnum.Cielo_Elo;
                case PaymentTypeEnum.Discover:
                    return PaymentMethodEnum.Cielo_Diners;
                default:
                    return 0;
            }
        }

        private void addRequestParam(StringBuilder resultString, string paramName, object paramValue)
        {
            if (paramValue != null && !String.IsNullOrEmpty(paramValue.ToString()))
            {
                resultString.AppendFormat("&{0}={1}", paramName, paramValue);
            }
        }

        public string GetResultText(string response)
        {
            return ExtractResponseParam(response, "message");
        }

        public string FormatPrice(decimal? price)
        {
            if (price == null)
            {
                return string.Empty;
            }
            return Utility.FormatPrice(price).Replace(".", ",");
        }

        public string GetCustomerName(Billing billing)
        {
            return billing.FirstName + " " + billing.LastName;
        }

        public string GetExpiration(Billing billing)
        {
            return billing.ExpMonth.Value.ToString("00") + "/" + billing.ExpYear.Value.ToString("00");
        }
    }
}
