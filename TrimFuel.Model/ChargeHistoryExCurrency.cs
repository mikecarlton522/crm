using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ChargeHistoryExCurrency : Entity
    {
        public long? ChargeHistoryID { get; set; }
        public int? CurrencyID { get; set; }
        public decimal? CurrencyAmount { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("CurrencyID", CurrencyID);
            v.AssertNotNull("CurrencyAmount", CurrencyAmount);
        }
    }
}
