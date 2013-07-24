using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    [XmlType("CustomerInfo")]
    public class CustomerInfo
    {
        //[XmlElement("Company")]
        //public string Company { get; set; }
        [XmlElement("FirstName")]
        public string FirstName { get; set; }
        [XmlElement("LastName")]
        public string LastName { get; set; }
        [XmlElement("Address1")]
        public string Address1 { get; set; }
        [XmlElement("Address2")]
        public string Address2 { get; set; }
        [XmlElement("City")]
        public string City { get; set; }
        [XmlElement("State")]
        public string State { get; set; }
        [XmlElement("Zip")]
        public string Zip { get; set; }
        [XmlElement("Country")]
        public string Country { get; set; }
        [XmlElement("Email")]
        public string Email { get; set; }
        [XmlElement("Phone")]
        public string Phone { get; set; }
        //[XmlElement("Company")]
        //public string Company { get; set; }
        //[XmlElement("Fax")]
        //public string Fax { get; set; }
        //[XmlElement("IsResidential")]
        //public bool IsResidential { get; set; }
    }
}
