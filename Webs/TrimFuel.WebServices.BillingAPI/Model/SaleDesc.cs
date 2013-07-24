using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class SaleDesc
    {
        public long SaleID { get; set; }
        public int ProductID { get; set; }
        public decimal Amount { get; set; }
    }

    public class SaleList : List<SaleDesc>
    {
    }
}