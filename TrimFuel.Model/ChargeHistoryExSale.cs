using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ChargeHistoryExSale : Entity
    {
        public struct ID
        {
            public long ChargeHistoryID { get; set; }
            public long SaleID { get; set; }
        }

        public ID? ChargeHistoryExSaleID { get; set; }
        public long? ChargeHistoryID { get; set; }
        public long? SaleID { get; set; }
        public decimal? Amount { get; set; }
        public int? CurrencyID { get; set; }
        public decimal? CurrencyAmount { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ChargeHistoryID", ChargeHistoryID);
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("Amount", Amount);
        }
    }
}
