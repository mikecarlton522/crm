using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    [XmlType("OrderItems")]
    public class OrderItems
    {
        [XmlElement("Item")]
        public List<OrderItem> OrderItemList { get; set; }
    }
}
