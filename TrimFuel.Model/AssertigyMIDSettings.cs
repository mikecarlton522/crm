using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class AssertigyMIDSettings : Entity
    {
        public int? AssertigyMIDSettingID { get; set; }
        public int? AssertigyMIDID { get; set; }
        public double? ChargebackRepresentationFee { get; set; }
        public double? ChargebackRepresentationFeeRetail { get; set; }
        public double? TransactionFee { get; set; }
        public double? ChargebackFee { get; set; }
        public double? DiscountRate { get; set; }

        public double? GatewayFee { get; set; }
        public double? GatewayFeeRetail { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
