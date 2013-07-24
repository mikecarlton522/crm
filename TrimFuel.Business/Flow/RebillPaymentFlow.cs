using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using TrimFuel.Model;
using TrimFuel.Business.Gateways;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Flow
{
    public class RebillPaymentFlow : PaymentFlow
    {
        protected Set<NMICompany, AssertigyMID> MID { get; set; }
        protected OrderRecurringPlanView OrderPlan { get; set; }

        public RebillPaymentFlow(Set<NMICompany, AssertigyMID> mid, OrderRecurringPlanView orderPlan)
        {
            MID = mid;
            OrderPlan = orderPlan;
        }

        protected override BusinessError<ChargeHistoryView> ProcessCharge(Set<NMICompany, AssertigyMID> mid, Billing billing, Product product, Currency currency, decimal amount, long? invoiceID)
        {
            string chargeBlockReason = null;
            if (IsChargesBlocked(billing, out chargeBlockReason))
            {
                return new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, chargeBlockReason);
            }

            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                bool captureOnly = false;
                ChargeHistoryView lastRecurringCharge = new OrderService().GetLastRecurringCharge(OrderPlan.OrderRecurringPlan.OrderRecurringPlanID.Value);
                if (lastRecurringCharge != null && 
                    lastRecurringCharge.ChargeHistory.ChargeTypeID == ChargeTypeEnum.AuthOnly &&
                    lastRecurringCharge.ChargeHistory.MerchantAccountID == mid.Value2.AssertigyMIDID)
                {
                    decimal? authAmount = ((AuthOnlyChargeDetails)lastRecurringCharge.ChargeHistory).RequestedCurrencyAmount;
                    if (authAmount == null)
                    {
                        authAmount = ((AuthOnlyChargeDetails)lastRecurringCharge.ChargeHistory).RequestedAmount;
                    }
                    if (authAmount >= amount && lastRecurringCharge.ChargeHistory.ChargeDate.Value.AddDays(30) >= DateTime.Now)
                    {
                        captureOnly = true;
                    }
                }

                IPaymentGateway paymentGateway = (new SaleService()).GetGatewayByMID(mid.Value2);

                BusinessError<GatewayResult> paymentResult = null;
                if (captureOnly)
                {
                    paymentResult = paymentGateway.Capture(
                        mid.Value1.GatewayUsername, mid.Value1.GatewayPassword, 
                        lastRecurringCharge.ChargeHistory, amount, currency);
                }
                else
                {
                    paymentResult = paymentGateway.Sale(mid.Value2.MID,
                        mid.Value1.GatewayUsername, mid.Value1.GatewayPassword, amount, currency,
                        invoiceID.Value, billing, product);
                }
                var chargeRes = ChargeLogging(paymentResult, billing,
                    mid.Value2, ChargeTypeEnum.Charge, amount, currency, invoiceID);

                res.State = paymentResult.State;
                res.ReturnValue = chargeRes.Value2;
                res.ErrorMessage = paymentResult.ErrorMessage;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        protected override BusinessError<Set<NMICompany, AssertigyMID>> ChooseMID(Billing billing, Product product, Currency currency, decimal amount)
        {
            return new BusinessError<Set<NMICompany,AssertigyMID>>(MID, BusinessErrorState.Success, null);
        }
    }
}
