using System;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.MoldingBox
{
    [XmlType("ShipMethodEnum")]
    public enum ShipMethodEnum
    {
        DHLGlobalMail = 1,
        DHLGlobalMailTrackTrace = 2,
        FedExGround = 3,
        FedEx2Day = 4,
        FedExStandardOvernight = 5,
        FedExPriorityOvernight = 6,
        FedExInternationalEconomy = 7,
        FedExInternationalPriority = 8,
        FedExCODGround = 9,
        FedExCODExpress = 10,
        USPSFirstClass = 11,
        USPSPriority = 12,
        USPSPriorityFlatRate = 13,
        USPSFirstClassInternational = 14
    }
}
