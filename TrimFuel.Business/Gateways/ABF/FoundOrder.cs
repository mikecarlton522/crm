using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.ABF
{
    [XmlType("order")]
    public class FoundOrder
    {
        [XmlElement("ReferenceNum")]
        public string ReferenceNum { get; set; }
        [XmlIgnore()]
        public long? ReferenceNumLong
        {
            get
            {
                long temp = 0;
                if (long.TryParse(ReferenceNum, out temp))
                {
                    return temp;
                }
                return null;
            }
        }
        [XmlElement("ProcessDate")]
        public DateTime ProcessDate { get; set; }
        [XmlElement("TrackingNumber")]
        public string TrackingNumber { get; set; }
    }
}
