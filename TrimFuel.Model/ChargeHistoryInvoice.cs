using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ChargeHistoryInvoice : Entity
    {
        public struct ID
        {
            public long ChargeHistoryID { get; set; }
        }

        public ID? ChargeHistoryInvoiceID { get; set; }
        public long? ChargeHistoryID { get; set; }
        public long? InvoiceID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ChargeHistoryID", ChargeHistoryID);
            v.AssertNotNull("InvoiceID", InvoiceID);
        }
    }
}
