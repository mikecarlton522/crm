using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class CampaignRecurringPlan : Entity
    {
        public int? CampaignRecurringPlanID { get; set; }
        public int? CampaignID { get; set; }
        public int? RecurringPlanID { get; set; }
        public decimal? TrialPrice { get; set; }
        public int? TrialInterim { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("CampaignID", CampaignID);
            v.AssertNotNull("RecurringPlanID", RecurringPlanID);
            v.AssertNotNull("TrialPrice", TrialPrice);
            v.AssertNotNull("TrialInterim", TrialInterim);
        }
    }
}
