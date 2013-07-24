using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class OrderSaleView2 : EntityView
    {
        public OrderSaleView SaleView { get; set; }
        public IList<ChargeHistoryView> InvoiceChargeList { get; set; }
        public IList<ChargeHistoryView> SaleRefundList { get; set; }
        public Campaign OrderCampaign { get; set; }
        public Product OrderProduct { get; set; }
        public SaleChargeback Chargeback { get; set; }
    }
}
