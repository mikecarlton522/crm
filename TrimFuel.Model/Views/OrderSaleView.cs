using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class OrderSaleView : EntityView
    {
        public OrderView Order { get; set; }
        public OrderSale OrderSale { get; set; }
        public IList<OrderProductView> ProductList { get; set; }
        public IList<OrderRecurringPlan> PlanList { get; set; }
        public IList<ShipmentView> ShipmentList { get; set; }
        public InvoiceView Invoice { get; set; }
    }
}
