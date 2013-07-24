using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using Kaboom.Store1.Logic;

namespace Kaboom.Store1.Controls
{
    public partial class ShoppingCartView : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public ICoupon Coupon { get; set; }
        public Dictionary<ShoppingCartProduct, int> Products { get; set; }

        protected string FormatPrice(decimal price)
        {
            return price.ToString("$0.00");
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rProducts.DataSource = Products;
        }

        protected decimal TotalCost
        {
            get
            {
                decimal sum = Products.Sum(item => item.Key.Price * (decimal)item.Value);
                if (Coupon != null && Coupon.Discount != null)
                {
                    sum = Coupon.ApplyDiscount(sum, DiscountType.Discount);
                }
                return sum;
            }
        }
    }
}