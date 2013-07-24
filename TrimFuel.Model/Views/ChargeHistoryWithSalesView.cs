using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class ChargeHistoryWithSalesView : EntityView
    {
        public Set<ChargeHistoryEx, FailedChargeHistoryView> ChargeHistoryView { get; set; }
        public IList<Set<UpsellSale, ProductCode, ChargeHistoryExSale>> SaleList { get; set; }
    }
}
