using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ShipmentPackageView
    {
        public Registration Registration { get; set; }
        public RegistrationInfo RegistrationInfo { get; set; }
        public Billing Billing { get; set; }
        public Product Product { get; set; }
        public IList<OrderSaleShipmentView> SaleList { get; set; }
        public IList<ShipmentShipperView> GetShipmentList()
        {
            if (SaleList == null)
            {
                return null;
            }
            return SaleList.SelectMany(i => i.ShipmentList).ToList();
        }
    }
}
