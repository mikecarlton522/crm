using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class CampaignSubscriptionView : EntityView
    {
        public Campaign Campaign { get; set; }
        public Subscription Subscription { get; set; }
        public CampaignRecurringPlan CampaignRecurringPlan { get; set; }
        public RecurringPlanView RecurringPlan { get; set; }
        public IList<CampaignTrialProduct> ProductList { get; set; }
    }
}
