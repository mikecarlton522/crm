using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class RecurringPlanShipment : Entity
    {
        public int? RecurringPlanShipmentID { get; set; }
        public int? RecurringPlanCycleID { get; set; }
        public string ProductSKU { get; set; }
        public int? Quantity { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("RecurringPlanCycleID", RecurringPlanCycleID);
            v.AssertNotNull("ProductSKU", ProductSKU);
            v.AssertNotNull("Quantity", Quantity);
            v.AssertString("ProductSKU", ProductSKU, 100);
        }
    }
}
