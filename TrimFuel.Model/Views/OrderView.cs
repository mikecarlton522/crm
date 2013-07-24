using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class OrderView : EntityView
    {
        public Billing Billing { get; set; }
        public Order Order { get; set; }
        public IList<OrderSaleView> SaleList { get; set; }
        public IList<InvoiceView> InvoiceList { get; set; }

        public IList<OrderRecurringPlan> GetAllPLans()
        {
            return SaleList.SelectMany(sl => sl.PlanList).ToList();
        }
    }
}
