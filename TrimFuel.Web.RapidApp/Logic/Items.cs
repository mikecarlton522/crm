using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;

namespace TrimFuel.Web.RapidApp.Logic
{
    public class BaseServiceItem
    {
        public int ID { get; set; }
        public string ServiceName { get; set; }
        public string BaseServiceName { get; set; }
        public string DisplayName { get; set; }
    }

    public class GatewayServiceItem : BaseServiceItem
    {
        public double TransactionFee { get; set; }
        public double ChargebackFee { get; set; }
        public double ChargebackRepresentationFee { get; set; }
        public double TransactionFeeRetail { get; set; }
        public double ChargebackFeeRetail { get; set; }
        public double ChargebackRepresentationFeeRetail { get; set; }
        public double DiscountRateRetail { get; set; }
        public double DiscountRate { get; set; }

        public double GatewayFee { get; set; }
        public double GatewayFeeRetail { get; set; }

    }

    public class OutboundServiceItem : BaseServiceItem
    {
        public double SetupFee { get; set; }
        public double SetupFeeRetail { get; set; }

        public double MonthlyFeeRetail { get; set; }
        public double MonthlyFee { get; set; }
        public double PerPourFeeRetail { get; set; }
        public double PerPourFee { get; set; }
    }

    public class FulfillmentServiceItem : BaseServiceItem
    {
        public double ShipmentFee { get; set; }
        public double ShipmentSKUFee { get; set; }
        public double KittingAndAsemblyFee { get; set; }
        public double SetupFee { get; set; }
        public double ReturnsFee { get; set; }
        public double CustomDevelopmentFee { get; set; }
        public double SpecialLaborFee { get; set; }
        public double ShipmentFeeRetail { get; set; }
        public double ShipmentSKUFeeRetail { get; set; }
        public double KittingAndAsemblyFeeRetail { get; set; }
        public double SetupFeeRetail { get; set; }
        public double ReturnsFeeRetail { get; set; }
        public double CustomDevelopmentFeeRetail { get; set; }
        public double SpecialLaborFeeRetail { get; set; }
        public Dictionary<string, string> ConfigFields { get; set; }
    }

    public class ServiceItem
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public List<BaseServiceItem> Services { get; set; }
    }
}
