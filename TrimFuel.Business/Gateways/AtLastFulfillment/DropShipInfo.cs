using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    [XmlType("DropShipInfo")]
    public class DropShipInfo
    {
        [XmlElement("CompanyName")]
        public string CompanyName { get; set; }
        //[XmlElement("Address")]
        //public string Address { get; set; }
        //[XmlElement("City")]
        //public string City { get; set; }
        //[XmlElement("State")]
        //public string State { get; set; }
        //[XmlElement("Zip")]
        //public string Zip { get; set; }
    }
}
