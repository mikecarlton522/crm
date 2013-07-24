using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ChargeHistoryView : EntityView
    {
        public ChargeHistoryEx ChargeHistory { get; set; }
        public ChargeHistoryExCurrency ChargeHistoryCurrency { get; set; }
        public Currency Currency { get; set; }
        public string MIDName { get; set; }

        public decimal? CurrencyAmount
        {
            get
            {
                if (ChargeHistoryCurrency != null && Currency != null)
                    return ChargeHistoryCurrency.CurrencyAmount;
                return ChargeHistory.Amount;
            }
        }

        public decimal? CurrencyAuthAmount
        {
            get
            {
                if (!(ChargeHistory is AuthOnlyChargeDetails))
                    return null;
                AuthOnlyChargeDetails auth = (AuthOnlyChargeDetails)ChargeHistory;
                if (auth.RequestedCurrencyID != null)
                    return auth.RequestedCurrencyAmount;
                return auth.Amount;
            }
        }

        public string CurrencyHtmlSymbol
        {
            get
            {
                if (ChargeHistoryCurrency != null && Currency != null)
                    return Currency.HtmlSymbol;
                return "$";
            }
        }
    }
}
