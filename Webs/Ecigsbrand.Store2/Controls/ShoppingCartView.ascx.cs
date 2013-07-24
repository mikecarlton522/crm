using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using Ecigsbrand.Store2.Logic;

namespace Ecigsbrand.Store2.Controls
{
    public partial class ShoppingCartView : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public ICoupon Coupon { get; set; }
        public Dictionary<ShoppingCartProduct, int> Products { get; set; }
        public IList<PromoGift> GiftCertificateList { get; set; }
        public decimal? EcigBucksAmount { get; set; }

        public IList<PromoGift> GiftCertificateListApplied { get; set; }
        public decimal EcigBucksAmountApplied { get; set; }

        protected string FormatPrice(decimal price)
        {
            return price.ToString("c");
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rProducts.DataSource = Products;

            CalclulateRedeems();
            rGiftCertificateList.DataSource = GiftCertificateListApplied;
        }

        protected decimal TotalCost
        {
            get
            {
                return CalculateTotalCost();
            }
        }

        private void CalclulateRedeems()
        {
            decimal sum = CalculatePoorTotalCost();

            EcigBucksAmountApplied = 0M;
            if (EcigBucksAmount != null)
            {
                EcigBucksAmountApplied = (sum > EcigBucksAmount.Value ? EcigBucksAmount.Value : sum);
            }

            sum -= EcigBucksAmountApplied;

            GiftCertificateListApplied = new List<PromoGift>();
            if (GiftCertificateList != null)
            {
                foreach (PromoGift item in GiftCertificateList)
                {
                    if (sum == 0M)
                    {
                        break;
                    }
                    if (item.RemainingValue != null)
                    {
                        decimal redeemAmount = (sum > item.RemainingValue.Value ? item.RemainingValue.Value : sum);
                        sum -= redeemAmount;
                        GiftCertificateListApplied.Add(new PromoGift()
                        {
                            GiftNumber = item.GiftNumber,
                            Value = redeemAmount
                        });
                    }
                }
            }
        }

        protected decimal CalculatePoorTotalCost()
        {
            decimal sum = 0M;
            foreach (var item in Products)
            {
                decimal itemAmount = item.Key.Price;
                if (Coupon != null)
                {
                    if (Coupon is ProductCoupon)
                    {
                        itemAmount = ((ProductCoupon)Coupon).ApplyDiscount(item.Key.ProductSKU, itemAmount, DiscountType.Any);
                    }
                    else
                    {
                        itemAmount = Coupon.ApplyDiscount(itemAmount, DiscountType.Discount);
                    }
                }
                itemAmount = itemAmount * (decimal)item.Value;
                sum += itemAmount;
            }
            return sum;
        }

        protected decimal CalculateTotalCost()
        {
            decimal sum = CalculatePoorTotalCost();
            sum -= EcigBucksAmountApplied;
            foreach (PromoGift item in GiftCertificateListApplied)
            {
                sum -= item.Value.Value;
            }
            return sum;
        }
    }
}