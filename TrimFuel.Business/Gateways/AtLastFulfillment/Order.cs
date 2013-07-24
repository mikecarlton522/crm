using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    [XmlType("Order")]
    public class Order
    {
        [XmlAttribute("orderID")]
        public long orderID { get; set; }
        [XmlElement("CustomerInfo")]
        public CustomerInfo CustomerInfo { get; set; }
        [XmlElement("DropShipInfo")]
        public DropShipInfo DropShipInfo { get; set; }
        //[XmlElement("OrderDate")]
        //public DateTime OrderDate { get; set; }
        [XmlElement("ShipMethod")]
        public ShipMethodEnum ShipMethod { get; set; }
        [XmlElement("AllowDuplicate")]
        public bool AllowDuplicate { get; set; }
        [XmlElement("Items")]
        public OrderItems OrderItems { get; set; }
    }
}
