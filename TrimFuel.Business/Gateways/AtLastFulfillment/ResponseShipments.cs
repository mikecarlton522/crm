using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    [XmlType("ResponseShipments")]
    public class ResponseShipments
    {
        [XmlElement("shipment")]
        public List<ResponseShipment> ShipmentList { get; set; }
    }
}
