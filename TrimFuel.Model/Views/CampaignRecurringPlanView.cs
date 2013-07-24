using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class CampaignRecurringPlanView : EntityView
    {
        public CampaignRecurringPlan CampaignRecurringPlan { get; set; }
        public RecurringPlanView RecurringPlan { get; set; }
        public IList<CampaignTrialProduct> ProductList { get; set; }
    }
}
