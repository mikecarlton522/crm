using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class LeadPartnerSettings : Entity
    {
        public int? LeadPartnerSettingID { get; set; }
        public int? LeadPartnerID { get; set; }
        public double? SetupFee { get; set; }
        public double? SetupFeeRetail { get; set; }
        public double? MonthlyFeeRetail { get; set; }
        public double? MonthlyFee { get; set; }
        public double? PerPourFeeRetail { get; set; }
        public double? PerPourFee { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("LeadPartnerID", LeadPartnerID);
        }
    }
}
