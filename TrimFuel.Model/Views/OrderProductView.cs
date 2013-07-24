using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class OrderProductView : EntityView
    {
        public OrderProduct OrderProduct { get; set; }
        public OrderSaleView Sale { get; set; }
        public ProductSKU ProductSKU { get; set; }
    }
}
