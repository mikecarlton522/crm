using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class RecurringPlan : Entity
    {
        public int? RecurringPlanID { get; set; }
        public int? ProductID { get; set; }
        public string Name { get; set; }
        //public int? RetryAttempts { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ProductID", ProductID);
            v.AssertNotNull("Name", Name);
            v.AssertString("Name", Name, 100);
        }
    }
}
