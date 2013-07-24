using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Gateways.BPagGateway
{
    public class BPagRedecardGateway : BPagGateway
    {
        protected override string ReplacePaymentMethod(string requestString, Billing billing)
        {
            string request = requestString;
            switch (billing.CreditCardCnt.TryGetCardType())
            {
                case PaymentTypeEnum.Visa:
                    request = request.Replace("##PAYMENT_METHOD##", "redecard_ws_visa");
                    break;
                case PaymentTypeEnum.Mastercard:
                    request = request.Replace("##PAYMENT_METHOD##", "redecard_ws_mastercard");
                    break;
                case PaymentTypeEnum.DinersClub:
                    request = request.Replace("##PAYMENT_METHOD##", "redecard_ws_diners");
                    break;
                default:
                    request = request.Replace("##PAYMENT_METHOD##", string.Empty);
                    break;
            }
            return request;
        }
    }
}
