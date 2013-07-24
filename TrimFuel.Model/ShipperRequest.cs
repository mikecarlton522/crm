using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ShipperRequest : Entity
    {
        public long? ShipperRequestID { get; set; }
        public short? ShipperID { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public int? ResponseShipmentStatus { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ShipperID", ShipperID);
            //Allow NULLs
            //v.AssertNotNull("Request", Request);
            //v.AssertNotNull("Response", Response);
            v.AssertNotNull("CreateDT", CreateDT);
            v.AssertNotNull("ResponseShipmentStatus", ResponseShipmentStatus);
        }
    }
}
