using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Invoice : Entity
    {
        public long? InvoiceID { get; set; }
        public decimal? Amount { get; set; }
        public decimal? AuthAmount { get; set; }
        public long? OrderID { get; set; }
        public int? CurrencyID { get; set; }
        public int? InvoiceStatus { get; set; }
        public DateTime? CreateDT { get; set; }
        public DateTime? ProcessDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Amount", Amount);
            v.AssertNotNull("InvoiceStatus", InvoiceStatus);
            v.AssertNotNull("CreateDT", CreateDT);
        }
    }
}
