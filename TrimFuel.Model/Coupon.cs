using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Coupon : Entity, ICoupon
    {
        public int? CouponID { get; set; }
        public int? ProductID { get; set; }
        public string Code { get; set; }
        public decimal? Discount { get; set; }
        public decimal? NewPrice { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("Code", Code, 255);
        }

        #region Logic

        public bool HasDiscount
        {
            get
            {
                return (Discount != null && Discount.Value > 0M) ||
                    (NewPrice != null && NewPrice.Value > 0M);
            }
        }


        #endregion

        #region ICoupon Members

        public string CouponCode
        {
            get { return Code; }
        }

        public decimal ApplyDiscount(decimal amount, DiscountType discountType)
        {
            decimal res = amount;

            if (Discount != null &&
                (discountType == DiscountType.Discount || discountType == DiscountType.Any))
            {
                decimal normalizeDiscount = (Discount.Value <= 1M) ? Discount.Value : Discount.Value / 100M;
                res = decimal.Round(res - res * normalizeDiscount, 2);                
            }
            else if (NewPrice != null &&
                (discountType == DiscountType.FixedPrice || discountType == DiscountType.Any))
            {
                res = NewPrice.Value;
            }

            return res;
        }

        public decimal? FixedPrice
        {
            get { return NewPrice; }
        }

        #endregion
    }
}
