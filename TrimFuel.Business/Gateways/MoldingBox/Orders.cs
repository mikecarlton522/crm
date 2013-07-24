using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.MoldingBox
{
    [XmlType("Orders")]
    [XmlRoot("Orders")]
    public class Orders
    {
        [XmlAttribute("apiKey")]
        public string apiKey { get; set; }
        [XmlElement("Order")]
        public List<Order> OrderList { get; set; }
    }
}
