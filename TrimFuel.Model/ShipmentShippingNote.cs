using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ShipmentShippingNote : Entity
    {
        public struct ID
        {
            public long ShippingNoteID { get; set; }
            public long ShipmentID { get; set; }
        }

        public ID? ShipmentShippingNoteID { get; set; }
        public long? ShippingNoteID { get; set; }
        public long? ShipmentID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ShippingNoteID", ShippingNoteID);
            v.AssertNotNull("ShipmentID", ShipmentID);
        }
    }
}
