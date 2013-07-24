using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ShipmentShipperRequest : Entity
    {
        public struct ID
        {
            public long ShipperRequestID { get; set; }
            public long ShipmentID { get; set; }
        }

        public ID? ShipmentShipperRequestID { get; set; }
        public long? ShipperRequestID { get; set; }
        public long? ShipmentID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ShipperRequestID", ShipperRequestID);
            v.AssertNotNull("ShipmentID", ShipmentID);
        }
    }
}
