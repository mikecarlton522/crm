using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class SaleShipperView : EntityView
    {
        public long? SaleID { get; set; }
        public int? SaleTypeID { get; set; }
        public string ProductCode { get; set; }
        public int? Quantity { get; set; }
    }
}
