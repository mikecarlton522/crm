using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.MoldingBox
{
    [XmlType("ResponseShipment")]
    public class ResponseShipment
    {
        [XmlAttribute("id")]
        public long id { get; set; }
        [XmlAttribute("orderID")]
        public long orderID { get; set; }
    }
}
