using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SalesAgent : Entity
    {
        public int? SalesAgentID { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public decimal? TransactionFeeFixed { get; set; }
        public int? TransactionFeePercentage { get; set; }
        public decimal? ShipmentFee { get; set;  }
        public decimal? ExtraSKUShipmentFee { get; set; }
        public decimal? ChargebackFee { get; set; }
        public decimal? CallCenterFeePerMinute { get; set; }
        public decimal? CallCenterFeePerCall { get; set; }
        public decimal? MonthlyCRMFee { get; set; }
        public int? AdminID { get; set; }
        public int? Commission { get; set; }
        public int? CommissionMerchant { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("Name", Name, 45);
        }        
    }
}
