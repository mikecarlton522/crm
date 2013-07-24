using System;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    [XmlType("ShipmentStatusEnum")]
    public enum ShipmentStatusEnum
    {
        SHIPPED = 1
    }
}
