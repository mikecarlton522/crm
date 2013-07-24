using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductCoupon : ICoupon
    {
        public ProductCoupon(string couponCode, string productSKU, decimal? discount, decimal? fixedPrice)
        {
            CouponCode = couponCode;
            ProductSKU = productSKU;
            Discount = discount;
            FixedPrice = fixedPrice;
        }

        public string ProductSKU { get; private set; }

        #region ICoupon Members

        public string CouponCode { get; private set; }
        public decimal? Discount { get; private set; }
        public decimal? FixedPrice { get; private set; }

        public decimal ApplyDiscount(decimal amount, DiscountType discountType)
        {
            return amount;
        }

        #endregion

        public decimal ApplyDiscount(string productSKU, decimal amount, DiscountType discountType)
        {
            decimal res = amount;

            if (IsAppliedTo(productSKU))
            {
                if (Discount != null &&
                    (discountType == DiscountType.Discount || discountType == DiscountType.Any))
                {
                    decimal normalizeDiscount = (Discount.Value <= 1M) ? Discount.Value : Discount.Value / 100M;
                    res = decimal.Round(res - res * normalizeDiscount, 2);
                }
                else if (FixedPrice != null &&
                    (discountType == DiscountType.FixedPrice || discountType == DiscountType.Any))
                {
                    res = FixedPrice.Value;
                }
            }
            return res;
        }

        public bool IsAppliedTo(string productSKU)
        {
            return productSKU == ProductSKU;
        }
    }
}
