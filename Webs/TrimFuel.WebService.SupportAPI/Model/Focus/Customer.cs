using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TrimFuel.WebService.SupportAPI.Model.Focus
{
    [Serializable()]
    [XmlType("customer")]
    public class Customer : XmlElement
    {
        [XmlElement("id")]
        public string IDAsString { get; set; }

        [XmlIgnore()]
        public long? ID
        {
            get { return ConvertToLong(IDAsString); }
        }

        [XmlElement("product")]
        public string Product { get; set; }
    }
}
