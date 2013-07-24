using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TrimFuel.WebService.SupportAPI.Model.Focus
{
    [Serializable()]
    [XmlType("time")]
    public class Time : XmlElement
    {
        [XmlElement("start")]
        public string StartAsString { get; set; }

        [XmlElement("end")]
        public string EndAsString { get; set; }

        [XmlIgnore()]
        public DateTime? Start 
        {
            get { return ConvertToDate(StartAsString); }
        }

        [XmlIgnore()]
        public DateTime? End
        {
            get { return ConvertToDate(EndAsString); }
        }

        [XmlElement("ani")]
        public string ANI { get; set; }

        [XmlElement("dnis")]
        public string DNIS { get; set; }
    }
}
