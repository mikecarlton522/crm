using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ShipmentShipperView : EntityView
    {
        public Shipment Shipment { get; set; }
        public string InventorySKU { get; set; }
        public string InventoryName { get; set; }
    }
}
