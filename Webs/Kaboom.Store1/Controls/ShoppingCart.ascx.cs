using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kaboom.Store1.Logic;
using TrimFuel.Model;
using TrimFuel.Business;

namespace Kaboom.Store1.Controls
{
    public partial class ShoppingCart_ : UserControl
    {
        public event EventHandler<EventArgs> ProductsChanged;
        protected void OnProductsChanged()
        {
            if (ProductsChanged != null)
            {
                ProductsChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> CouponChanged;
        protected void OnCouponChanged()
        {
            if (CouponChanged != null)
            {
                CouponChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> RefererCodeChanged;
        protected void OnRefererCodeChanged()
        {
            if (RefererCodeChanged != null)
            {
                RefererCodeChanged(this, EventArgs.Empty);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            HideCouponError();
            HideRefererCodeError();

            if (Page.IsPostBack && Products == null)
            {
                LoadProducts();
            }
        }

        private void HideCouponError()
        {
            phCouponError.Visible = false;
        }

        private void ShowCouponError()
        {
            phCouponError.Visible = true;
        }

        private void HideRefererCodeError()
        {
            phRefererCodeError.Visible = false;
        }

        private void ShowRefererCodeError()
        {
            phRefererCodeError.Visible = true;
        }

        private void LoadProducts()
        {
            Dictionary<ShoppingCartProduct, int> loadProducts = new Dictionary<ShoppingCartProduct, int>();
            foreach (RepeaterItem rItem in rProducts.Items)
            {
                if (rItem.ItemType == ListItemType.Item || rItem.ItemType == ListItemType.AlternatingItem)
                {
                    HiddenField hdnProductID = (HiddenField)rItem.FindControl("hdnProductID");
                    TextBox tbProductCount = (TextBox)rItem.FindControl("tbProductCount");

                    ShoppingCartProduct product = ShoppingCart.DeserializeProduct(hdnProductID.Value);
                    if (product != null)
                    {
                        int productCount = 1;
                        int.TryParse(tbProductCount.Text, out productCount);

                        if (productCount > 0)
                        {
                            if (!loadProducts.ContainsKey(product))
                            {
                                loadProducts[product] = productCount;
                            }
                            else
                            {
                                loadProducts[product] += productCount;
                            }
                        }
                    }
                }
            }
            Products = loadProducts;
        }

        protected string FormatPrice(decimal price)
        {
            return price.ToString("$0.00");
        }

        protected void bUpdateQuantities_Click(object sender, EventArgs e)
        {
            OnProductsChanged();
        }

        protected void bApplyCoupon_Click(object sender, EventArgs e)
        {
            ICoupon c = (new RegistrationService()).GetCampaignDiscount(tbCoupon.Text.Trim(), CampaignID.Value);
            if (c != null)
            {
                CouponCode = c.CouponCode;
                coupon = c;
                OnCouponChanged();
            }
            else
            {
                ShowCouponError();
            }
        }

        protected void bApplyRefererCode_Click(object sender, EventArgs e)
        {
            Referer c = (new RefererService()).GetByCode(tbRefererCode.Text.Trim());
            if (c != null)
            {
                RefererCode = c.RefererCode;
                referer = c;
                OnRefererCodeChanged();
            }
            else
            {
                ShowRefererCodeError();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rProducts.DataSource = Products;
        }

        public IEnumerable<KeyValuePair<ShoppingCartProduct, int>> Products { get; set; }
        public string CouponCode
        {
            get { return (string)ViewState["CouponCode"]; }
            set { ViewState["CouponCode"] = value; }
        }
        public string RefererCode
        {
            get { return (string)ViewState["RefererCode"]; }
            set { ViewState["RefererCode"] = value; }
        }
        public int? CampaignID { get; set; }

        private ICoupon coupon = null;
        public ICoupon Coupon 
        {
            get
            {
                if (coupon == null && CouponCode != null)
                {
                    coupon = (new RegistrationService()).GetCampaignDiscount(CouponCode, CampaignID.Value);
                }
                return coupon;
            }
        }

        private Referer referer = null;
        public Referer Referer
        {
            get
            {
                if (referer == null && RefererCode != null)
                {
                    referer = (new RefererService()).GetByCode(RefererCode);
                }
                return referer;
            }
        }

        protected decimal TotalCost
        {
            get
            {
                decimal sum = Products.Sum(item => item.Key.Price * (decimal)item.Value);
                if (Coupon != null)
                {
                    sum = Coupon.ApplyDiscount(sum, DiscountType.Discount);
                }
                return sum;
            }
        }
    }
}