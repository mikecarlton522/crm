using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Upsell : Entity
    {
        public int? UpsellID { get; set; }
        public long? BillingID { get; set; }
        public int? Quantity { get; set; }
        public string ProductCode { get; set; }
        public DateTime? CreateDT { get; set; }
        public bool? Complete { get; set; }
        public int? UpsellTypeID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("ProductCode", ProductCode, 50);
        }
    }
}
