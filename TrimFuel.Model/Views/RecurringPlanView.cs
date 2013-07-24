using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class RecurringPlanView : EntityView
    {
        public int? RecurringPlanID { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public string Name { get; set; }
        public IList<RecurringPlanCycleView> CycleList { get; set; }
    }
}
