using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class RecurringPlanCycleView : EntityView
    {
        public RecurringPlanCycle Cycle { get; set; }
        public RecurringPlanConstraint Constraint { get; set; }
        public IList<RecurringPlanShipment> ShipmentList { get; set; }
    }
}
