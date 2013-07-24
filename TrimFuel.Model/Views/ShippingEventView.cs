using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ShippingEventView : EntityView
    {
        public DateTime? CreateDT { get; set; }
        public short? ShipperID { get; set; }
        public string EventText { get; set; }
        public int? ResultShipmentStatus { get; set; }

        public Shipper Shipper { get; set; }
    }
}
