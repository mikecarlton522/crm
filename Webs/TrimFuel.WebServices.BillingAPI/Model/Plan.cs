using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class Plan
    {
        public int PlanID { get; set; }
        public DateTime NextBillDate { get; set; }

        public static Plan FromSubscription(BillingSubscription src)
        {
            if (src == null)
            {
                return null;
            }

            Plan res = new Plan();
            if (src.SubscriptionID != null)
            {
                res.PlanID = src.SubscriptionID.Value;
            }
            if (src.NextBillDate != null)
            {
                res.NextBillDate = src.NextBillDate.Value;
            }

            return res;
        }
    }
}
