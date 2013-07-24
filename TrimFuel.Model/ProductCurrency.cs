using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductCurrency: Entity
    {
        public struct ID
        {
            public int? ProductID { get; set; }
            public int? CurrencyID { get; set; }
        }

        public ID? ProductCurrencyID { get; set; }
        public int? ProductID { get; set; }
        public int? CurrencyID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ProductID", ProductID);
            v.AssertNotNull("CurrencyID", CurrencyID);
        }
    }
}
