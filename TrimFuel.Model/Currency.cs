using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Currency : Entity
    {
        public int? CurrencyID { get; set; }
        public decimal? Rate { get; set; }
        public string CurrencyName { get; set; }
        public string HtmlSymbol { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Rate", Rate);
            v.AssertNotNull("CurrencyName", CurrencyName);
            v.AssertNotNull("CurrencySymbol", HtmlSymbol);
            v.AssertString("CurrencyName", CurrencyName, 50);
            v.AssertString("HtmlSymbol", HtmlSymbol, 50);
        }

        #region Logic

        public decimal ConvertToUSD(decimal amount)
        {
            if (Rate == null)
            {
                return amount;
            }
            return Math.Round(amount * Rate.Value, 2);
        }

        #endregion
    }
}
