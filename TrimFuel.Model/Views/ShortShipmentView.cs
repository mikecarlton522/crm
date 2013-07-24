using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ShortShipmentView : EntityView
    {
        public long? RegID { get; set; }
        public int? ShipperID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ShipperID", ShipperID);
        }
    }
}
