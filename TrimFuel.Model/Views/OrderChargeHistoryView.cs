using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class OrderChargeHistoryView : EntityView
    {
        public long? OrderID { get; set; }
        public IList<ChargeHistoryView> Charges { get; set; }
        public IDictionary<long, IList<ChargeHistoryView>> ChargesByInvoices { get; set; }
        public IDictionary<long, IList<ChargeHistoryView>> RefundsBySales { get; set; }
    }
}
