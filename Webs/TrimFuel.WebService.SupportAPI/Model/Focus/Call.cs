using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TrimFuel.WebService.SupportAPI.Model.Focus
{
    [Serializable()]
    [XmlType("call")]
    public class Call : XmlElement
    {
        [XmlAttribute("id")]
        public string IDAsString { get; set; }

        [XmlIgnore()]
        public long? ID
        {
            get { return ConvertToLong(IDAsString); }
        }

        [XmlElement("time")]
        public Time Time { get; set; }

        [XmlElement("agent")]
        public Agent Agent { get; set; }

        [XmlElement("disposition")]
        public Disposition Disposition { get; set; }

        [XmlElement("customer")]
        public Customer Customer { get; set; }

        [XmlElement("holdTime")]
        public string HoldTimeAsString { get; set; }

        [XmlIgnore()]
        public int? HoldTime
        {
            get { return ConvertToInt(HoldTimeAsString); }
        }
    }
}
