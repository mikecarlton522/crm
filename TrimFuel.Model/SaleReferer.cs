using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SaleReferer : Entity
    {
        public int? SaleRefererID { get; set; }
        public long? SaleID { get; set; }
        public int? RefererID { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("SaleID", SaleID);
            v.AssertNotNull("RefererID", RefererID);
        }
    }
}
