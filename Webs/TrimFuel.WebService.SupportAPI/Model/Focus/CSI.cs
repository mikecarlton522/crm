using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace TrimFuel.WebService.SupportAPI.Model.Focus
{
    [Serializable()]
    [XmlType("csi")]
    public class CSI : XmlElement
    {
        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlAttribute("partner")]
        public string Partner { get; set; }

        [XmlElement("call", typeof(Call))]
        public Collection<Call> CallList { get; set; }
    }
}
