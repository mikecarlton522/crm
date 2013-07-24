using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class RefererCommission : Entity
    {
        public int? RefererCommissionID { get; set; }
        public int? RefererID { get; set; }
        public int? RefererCommissionTID { get; set; }
        public decimal? Amount { get; set; }
        public bool? Completed { get; set; }
        public DateTime? CreateDT { get; set; }
        public decimal? RemainingAmount { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("RefererID", RefererID);
            v.AssertNotNull("RefererCommissionTID", RefererCommissionTID);
            v.AssertNotNull("Amount", Amount);
            v.AssertNotNull("Completed", Completed);
            v.AssertNotNull("RemainingAmount", RemainingAmount);
        }
    }
}
