using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TrimFuel.WebService.SupportAPI.Model.Focus
{
    [Serializable()]
    [XmlRoot("triangle")]
    public class Triangle : XmlElement
    {
        [XmlElement("csi")]
        public CSI CSI { get; set; }
    }
}
