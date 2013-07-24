using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ecigsbrand.Store2.Logic
{
    public enum ShoppingCartProductType
    {
        Subscription = 1,
        Upsell = 2
    }

    public class ShoppingCartProduct
    {
        public ShoppingCartProductType ProductType { get; set; }
        public int ProductID { get; set; }
        public decimal Price { get; set; }
        public string ProductSKU { get; set; }
    }
}
