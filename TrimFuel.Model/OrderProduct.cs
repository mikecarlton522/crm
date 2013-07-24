using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class OrderProduct : Entity
    {
        public long? OrderProductID { get; set; }
        public long? SaleID { get; set; }
        public string ProductSKU { get; set; }
        public int? Quantity { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("ProductSKU", ProductSKU);
            v.AssertNotNull("Quantity", Quantity);
            v.AssertString("ProductSKU", ProductSKU, 50);
        }
    }
}
