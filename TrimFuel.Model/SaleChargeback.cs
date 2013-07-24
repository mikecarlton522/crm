using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SaleChargeback : Entity
    {
        public int? SaleChargebackID { get; set; }
        public int? SaleID { get; set; }
        public int? BillingID { get; set; }
        public int? ChargebackStatusTID { get; set; }
        public int? ChargebackReasonCodeID { get; set; }
        public string CaseNumber { get; set; }
        public string ARN { get; set; }
        public DateTime? CreateDT { get; set; }
        public DateTime? PostDT { get; set; }
        public DateTime? DisputeSentDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("CaseNumber", CaseNumber, 45);
        }
    }
}
