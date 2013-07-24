using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class OrderSaleShipmentView : EntityView
    {
        public long? BillingID { get; set; }
        public OrderSale Sale { get; set; }
        public SaleShippingOption ShippingOption { get; set; }
        public IList<ShipmentShipperView> ShipmentList { get; set; }
    }
}
