using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class SubscriptionRecurringPlanView : EntityView
    {
        public int? SubscriptionID { get; set; }
        public int? RecurringPlanID { get; set; }
    }
}
