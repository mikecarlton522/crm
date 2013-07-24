using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Gateways
{
    public class ShipmentGatewayResult<T> where T : ShipmentPackageResult
    {
        public string Request { get; set; }
        public string Response { get; set; }
        public IList<T> PackageList { get; set; }
    }

    public abstract class ShipmentPackageResult
    {
        public string ShipperRegID { get; set; }
    }

    public class ShipmentPackageSubmitResult : ShipmentPackageResult
    {
        public IList<long> ShipmentIDList { get; set; }
    }

    public class ShipmentPackageShipResult : ShipmentPackageResult
    {
        public string TrackingNumber { get; set; }
        public DateTime? ShipDT { get; set; }
    }

    public class ShipmentPackageReturnResult : ShipmentPackageResult
    {
        public string Reason { get; set; }
        public DateTime? ReturnDT { get; set; }
    }
}
