using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    [XmlType("OrderItem")]
    public class OrderItem
    {
        [XmlElement("SKU")]
        public string SKU { get; set; }
        [XmlElement("Qty")]
        public int Qty { get; set; }
    }
}
