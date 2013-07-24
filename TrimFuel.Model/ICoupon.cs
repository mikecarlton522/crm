using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    [Flags()]
    public enum DiscountType
    {
        Discount = 1,
        FixedPrice = 2,
        Any = 3
    }

    public interface ICoupon
    {
        string CouponCode { get; }
        decimal? Discount { get; }
        decimal? FixedPrice { get; }
        decimal ApplyDiscount(decimal amount, DiscountType discountType);
    }
}
