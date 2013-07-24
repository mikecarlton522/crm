using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class FakeCoupon : ICoupon
    {
        public FakeCoupon(string couponCode, decimal? discount)
        {
            CouponCode = couponCode;
            Discount = discount;
        }

        public string CouponCode { get; set; }
        public decimal? Discount { get; set; }

        #region ICoupon Members

        public decimal? FixedPrice
        {
            get { return null; }
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

            return res;
        }

        #endregion
    }
}
