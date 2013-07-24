using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace TrimFuel.WebService.SupportAPI.Model.Focus
{
    [Serializable()]
    [XmlType("disposition")]
    public class Disposition : XmlElement
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

        [XmlElement("agent_notes")]
        public string AgentNotes { get; set; }
    }
}
