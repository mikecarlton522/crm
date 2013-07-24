using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.MoldingBox
{
    [XmlType("Shipment")]
    public class Shipment
    {
        [XmlElement("ShipmentStatus")]
        public string ShipmentStatus { get; set; }
        [XmlElement("Tracking")]
        public string Tracking { get; set; }
    }
}
