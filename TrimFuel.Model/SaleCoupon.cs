using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class SaleCouponCode : Entity
    {
        public int? SaleCouponCodeID { get; set; }
        public int? SaleID { get; set; }
        public string CouponCode { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("CouponCode", CouponCode, 45);
        }
    }
}
