using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class UnpayedRebillsView : EntityView
    {
        public int? BillingSubscriptionID { get; set; }
        public int? BillingID { get; set; }
        public int? SubscriptionID { get; set; }
        public DateTime? NextBillDate { get; set; }
        public string Affiliate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
