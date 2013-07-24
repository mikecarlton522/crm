using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SaleChargeDetails : Entity
    {
        public long? SaleChargeDetailsID { get; set; }
        public long? SaleID { get; set; }
        public int? SaleChargeTypeID { get; set; }
        public decimal? Amount { get; set; }
        public int? CurrencyID { get; set; }
        public decimal? CurrencyAmount { get; set; }
        public string Description { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("SaleChargeTypeID", SaleChargeTypeID);
            v.AssertNotNull("Amount", Amount);
            v.AssertString("Description", Description, 255);
        }
    }
}
