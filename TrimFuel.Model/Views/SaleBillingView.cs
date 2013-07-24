using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class SaleBillingView : EntityView
    {
        public long? SaleID { get; set; }
        public DateTime? SaleDT { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? ShippingAmount { get; set; }
        public Billing Billing { get; set; }
    }
}
