using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class BillingCancelCode : Entity
    {
        public int? BillingCancelCodeID { get; set; }
        public int? BillingID { get; set; }
        public string CancelCode { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("CancelCode", CancelCode, 45);
        }
    }
}
