using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ExtraTrialShip : Entity
    {
        public int? ExtraTrialShipID { get; set; }
        public string ProductCode { get; set; }
        public long? BillingID { get; set; }
        public DateTime? CreateDT { get; set; }
        public int? Quantity { get; set; }
        public bool? Completed { get; set; }
        public int? ExtraTrialShipTypeID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("ProductCode", ProductCode, 50);
        }
    }
}
