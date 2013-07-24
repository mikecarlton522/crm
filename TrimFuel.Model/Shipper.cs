using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Shipper : Entity
    {
        public int ShipperID { get; set; }
        public string Name { get; set; }
        public decimal FulfillmentCost { get; set; }
        public bool ServiceIsActive { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ShipperID", ShipperID);
        }
    }
}
