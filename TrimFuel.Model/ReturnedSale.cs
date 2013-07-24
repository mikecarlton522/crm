using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ReturnedSale : Entity
    {
        public long? SaleID { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Reason { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("Reason", Reason, 1000);
            v.AssertNotNull("SaleID", SaleID);
        }
    }
}
