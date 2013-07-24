using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SaleRefund : Entity
    {
        public long? SaleRefundID { get; set; }
        public long? SaleID { get; set; }
        public long? ChargeHistoryID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("ChargeHistoryID", ChargeHistoryID);
        }
    }
}
