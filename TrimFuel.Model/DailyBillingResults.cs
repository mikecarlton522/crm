using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class DailyBillingResults : Entity
    {
        public int? DailyBillingResultsID { get; set; }
        public int? SubscriptionID { get; set; }
        public int? ReattemptSuccessCount { get; set; }
        public int? ReattemptFailCount { get; set; }
        public int? RebillSuccessCount { get; set; }
        public int? RebillFailCount { get; set; }
        public decimal? Amount { get; set; }
        public DateTime? ChargeDT { get; set; }
        public string Affiliate { get; set; }
        public string MID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
