using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class InvoiceView : EntityView
    {
        public Invoice Invoice { get; set; }
        public Currency Currency { get; set; }
        public OrderView Order { get; set; }
        public IList<OrderSaleView> SaleList { get; set; }
        //public IList<ChargeHistoryEx> TransactionList { get; set; }
    }
}
