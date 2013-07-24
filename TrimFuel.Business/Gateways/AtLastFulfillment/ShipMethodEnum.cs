using System;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    [XmlType("ShipMethodEnum")]
    public enum ShipMethodEnum
    {
        POM1C = 1,
        XPOCPF = 2
    }
}
