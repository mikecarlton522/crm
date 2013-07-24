using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TPClientShipperSettings : Entity
    {
        public int? ShipperSettingID { get; set; }
        public int ShipperID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhone { get; set; }
        public string CompanyEmail { get; set; }
        public double? ShipmentFee { get; set; }
        public double? KittingAndAsemblyFee { get; set; }
        public double? SetupFee { get; set; }
        public double? ReturnsFee { get; set; }
        public double? CustomDevelopmentFee { get; set; }
        public double? SpecialLaborFee { get; set; }
        public double? ShipmentFeeRetail { get; set; }
        public double? KittingAndAsemblyFeeRetail { get; set; }
        public double? SetupFeeRetail { get; set; }
        public double? ReturnsFeeRetail { get; set; }
        public double? CustomDevelopmentFeeRetail { get; set; }
        public double? SpecialLaborFeeRetail { get; set; }
        public double? ShipmentSKUFee { get; set; }
        public double? ShipmentSKUFeeRetail { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ShipperSettingsId", ShipperSettingID);
            v.AssertNotNull("ShipperId", ShipperID);
        }
    }
}
