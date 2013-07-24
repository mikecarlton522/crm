using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class Item
    {
        public decimal Amount { get; set; }
        public decimal? Shipping { get; set; }
        public int? ProductID { get; set; }
        public int? ProductTypeID { get; set; }
    }

    public class ItemList : List<Item>
    {
    }
}