using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Shipment : Entity
    {
        public long? ShipmentID { get; set; }
        public string SN { get; set; }
        public long? SaleID { get; set; }
        public string ProductSKU { get; set; }
        public int? ShipmentStatus { get; set; }
        public DateTime? CreateDT { get; set; }
        public DateTime? SendDT { get; set; }
        public DateTime? ErrorDT { get; set; }
        public DateTime? ShipDT { get; set; }
        public DateTime? ReturnDT { get; set; }
        public string TrackingNumber { get; set; }
        public short? ShipperID { get; set; }
        public string ShipperRegID { get; set; }


        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("ProductSKU", ProductSKU);
            v.AssertNotNull("ShipmentStatus", ShipmentStatus);
            v.AssertNotNull("CreateDT", CreateDT);
            v.AssertString("SN", SN, 100);
            v.AssertString("ProductSKU", ProductSKU, 50);
            v.AssertString("TrackingNumber", TrackingNumber, 100);
            v.AssertString("ShipperRegID", ShipperRegID, 50);
        }
    }
}
