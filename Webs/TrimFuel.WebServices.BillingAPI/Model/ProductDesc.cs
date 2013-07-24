using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class ProductDesc
    {
        public int ProductID { get; set; }
        public decimal Amount { get; set; }
    }

    public class ProductList : List<ProductDesc>
    {
    }
}