using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class PlanResult
    {
        public Plan Plan { get; set; }
        public ChargeHistory TrialCharge { get; set; }

        public static PlanResult FromBillingSubscriptionAndChargeHistoryEx(Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView> src)
        {
            if (src == null)
            {
                return null;
            }

            PlanResult res = new PlanResult();
            if (src.Value1 != null && src.Value1.StatusTID != null && src.Value1.StatusTID.Value == BillingSubscriptionStatusEnum.Active)
            {
                res.Plan = Plan.FromSubscription(src.Value1);
            }
            res.TrialCharge = ChargeHistory.FromChargeHistoryEx(new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                {
                    Value1 = src.Value2,
                    Value2 = src.Value3
                }
            );

            return res;
        }
    }
}
