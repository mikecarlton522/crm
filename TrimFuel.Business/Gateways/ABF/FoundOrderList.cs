using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Collections.ObjectModel;

namespace TrimFuel.Business.Gateways.ABF
{
    [XmlType("orders")]
    public class FindOrderResult
    {
        [XmlElement("order")]
        public Collection<FoundOrder> Orders { get; set; }
    }
}
