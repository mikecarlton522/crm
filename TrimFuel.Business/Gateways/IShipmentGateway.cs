using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Gateways
{
    interface IShipmentGateway
    {
        IList<ShipmentGatewayResult<ShipmentPackageSubmitResult>> SubmitShipments(IList<ShipmentPackageView> packageList, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallbackWithCount canContinue);
        IList<ShipmentGatewayResult<ShipmentPackageShipResult>> CheckShipped(IList<string> shipperRegIDList, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallback canContinue);
        bool IsCheckShippedImplemented { get; }
        IList<ShipmentGatewayResult<ShipmentPackageReturnResult>> CheckReturned(IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallback canContinue);
        bool IsCheckReturnedImplemented { get; }
    }

    public delegate bool CanContinueCallback(decimal progressPercents);
    public delegate bool CanContinueCallbackWithCount(decimal progressPercents, int processedCount);
}
