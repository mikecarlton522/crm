using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class OrderSaleBillingView : EntityView
    {
        public OrderSale Sale { get; set; }
        public long? BillingID { get; set; }
    }
}
