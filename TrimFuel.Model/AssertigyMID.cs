using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class AssertigyMID : Entity
    {
        public int? AssertigyMIDID { get; set; }
        public string MID { get; set; }
        public string DisplayName { get; set; }
        public int? ParentMID { get; set; }
        public decimal? MonthlyCap { get; set; }
        public double? ProcessingRate { get; set; }
        public double? ReserveAccountRate { get; set; }
        public double? ChargebackFee { get; set; }
        public double? TransactionFee { get; set; }
        public bool? Visible { get; set; }
        public bool? Deleted { get; set; }
        public string GatewayName { get; set; }
        public int? MIDCategoryID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("GatewayName", GatewayName);
            v.AssertString("GatewayName", GatewayName, 50);
            v.AssertString("MID", MID, 50);
            v.AssertString("DisplayName", DisplayName, 50);
        }
    }
}
