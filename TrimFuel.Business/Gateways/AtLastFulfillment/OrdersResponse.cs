using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    [XmlType("OrdersResponse")]
    [XmlRoot("response")]
    public class OrdersResponse
    {
        [XmlElement("shipments")]
        public ResponseShipments Shipments { get; set; }
    }
}
