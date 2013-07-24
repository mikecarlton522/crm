using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ShippmentsToSendView : EntityView
    {
        public int? Count { get; set; }
        public int? ShipperID { get; set; }
    }
}
