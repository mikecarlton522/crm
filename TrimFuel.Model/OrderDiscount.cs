using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class OrderDiscount
    {
        public long? OrderDiscountID { get; set; }
        public int? DiscountTypeID { get; set; }
        public string Data { get; set; }
    }
}
