using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Enums;

namespace TrimFuel.Model.Views
{
    public class OrderRecurringPlanView : EntityView
    {
        public OrderRecurringPlan OrderRecurringPlan { get; set; }
        public RecurringPlanView Plan { get; set; }
        public IList<RecurringSale> AttemptList { get; set; }
        public IList<RecurringSale> SaleList 
        {
            get 
            {
                if (AttemptList == null)
                    return null;
                return AttemptList.Where(i => i.SaleStatus == SaleStatusEnum.Approved).ToList();
            }
        }

        //!!!Affects rebill script, do not change
        public RecurringPlanCycleView GetNextCycle()
        {
            int nextCycle = 1;
            if (SaleList.Count > 0)
                nextCycle = SaleList.Last().RecurringCycle.Value + 1;

            int notRecurringItems = Plan.CycleList.Where(i => i.Cycle.Recurring == false).Count();
            int recurringItems = Plan.CycleList.Where(i => i.Cycle.Recurring == true).Count();

            int nextRecurringCycle = nextCycle;
            if (nextRecurringCycle > notRecurringItems && recurringItems > 0)
            {
                nextRecurringCycle = notRecurringItems + 1 + (nextRecurringCycle - (notRecurringItems + 1)) % recurringItems;
            }

            return Plan.CycleList.FirstOrDefault(i => i.Cycle.Cycle == nextRecurringCycle);
        }
    }
}
