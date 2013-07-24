using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class CampaignUpsell : Entity
    {
        public int? CampaignUpsellID { get; set; }
        public int? CampaignID { get ; set; }
        public string ProductCode { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public int? CampaignPageID { get; set; }
        public int? SubscriptionID { get; set; }
        public int? RecurringPlanID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
