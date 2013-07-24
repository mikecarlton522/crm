using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    [XmlType("Shipments")]
    [XmlRoot("Shipments")]
    public class Shipments
    {
        [XmlElement("Shipment")]
        public List<Shipment> ShipmentList { get; set; }
    }
}
