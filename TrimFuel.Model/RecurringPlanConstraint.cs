using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class RecurringPlanConstraint : Entity
    {
        public struct ID
        {
            public int? RecurringPlanCycleID { get; set; }
        }

        public ID? RecurringPlanConstraintID { get; set; }
        public int? RecurringPlanCycleID { get; set; }
        public int? ChargeTypeID { get; set; }
        public decimal? Amount { get; set; }
        public decimal? ShippingAmount { get; set; }
        public decimal? TaxAmount { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("RecurringPlanCycleID", RecurringPlanCycleID);
            v.AssertNotNull("ChargeTypeID", ChargeTypeID);
            v.AssertNotNull("Amount", Amount);
            v.AssertNotNull("ShippingAmount", ShippingAmount);
            v.AssertNotNull("TaxAmount", TaxAmount);
        }
    }
}