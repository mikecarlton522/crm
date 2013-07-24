using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ShipperRequestView : EntityView
    {
        public ShipperRequest Request { get; set; }
        public Shipper Shipper { get; set; }
    }
}
