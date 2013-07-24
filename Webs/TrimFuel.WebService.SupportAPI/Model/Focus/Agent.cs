using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TrimFuel.WebService.SupportAPI.Model.Focus
{
    [Serializable()]
    [XmlType("agent")]
    public class Agent : XmlElement
    {
        [XmlElement("id")]
        public string IDAsString { get; set; }

        [XmlIgnore()]
        public int? ID
        {
            get { return ConvertToInt(IDAsString); }
        }

        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("location")]
        public string Location { get; set; }
    }
}
