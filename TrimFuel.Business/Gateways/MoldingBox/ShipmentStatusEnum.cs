using System;
using System.Xml.Serialization;

namespace TrimFuel.Business.Gateways.MoldingBox
{
    [XmlType("ShipmentStatusEnum")]
    public enum ShipmentStatusEnum
    {
        SHIPPED = 1
    }
}
