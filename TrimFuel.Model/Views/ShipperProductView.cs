using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ShipperProductView : EntityView
    {
        public int ProductID { get; set; }
        public string Shipper { get; set; }
        public string Product { get; set; }
        public int ShipperID { get; set; }
    }
}
