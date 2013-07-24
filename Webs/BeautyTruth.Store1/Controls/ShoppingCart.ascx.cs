using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business;
using BeautyTruth.Store1.Logic;

namespace BeautyTruth.Store1.Controls
{
    public partial class ShoppingCart_ : System.Web.UI.UserControl
    {
        #region Events

        public event EventHandler<EventArgs> GiftCertificateRemoved;
        protected void OnGiftCertificateRemoved()
        {
            if (GiftCertificateRemoved != null)
            {
                GiftCertificateRemoved(this, new EventArgs());
            }
        }

        public event EventHandler<EventArgs> ProductsChanged;
        protected void OnProductsChanged()
        {
            if (ProductsChanged != null)
            {
                ProductsChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> CouponAdded;
        protected void OnCouponAdded()
        {
            if (CouponAdded != null)
            {
                CouponAdded(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> CouponRemoved;
        protected void OnCouponRemoved()
        {
            if (CouponRemoved != null)
            {
                CouponRemoved(this, EventArgs.Empty);
            }
        }

        public event EventHandler<GiftCertificateEventArgs> GiftCertificatePopulated;
        protected void OnGiftCertificatePopulated(string giftCertificateNumber)
        {
            if (GiftCertificatePopulated != null)
            {
                GiftCertificatePopulated(this, new GiftCertificateEventArgs(giftCertificateNumber));
            }
        }

        #endregion

        protected string FormatPrice(decimal price)
        {
            return price.ToString("c");
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
        public string ProductCode
        {
            get
            {
                if (Products.Count() > 0)
                    return Products.FirstOrDefault().Key.ProductSKU;
                else
                    return string.Empty;
            }
        }

        public ICoupon coupon = null;
        protected ICoupon Coupon
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

        public PromoGift Gift
        {
            get
            {
                return ShoppingCart.Current.GetPromoGiftRedeemList().FirstOrDefault();
            }
        }

        public decimal ShippingValue
        {
            get
            {
                return Convert.ToDecimal(ddlShipping.SelectedValue);
            }
        }

        protected decimal TotalCost
        {
            get
            {
                return ShoppingCart.Current.TotalCost;
            }
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
                    CheckBox cbRemoved = (CheckBox)rItem.FindControl("cbRemoved");

                    ShoppingCartProduct product = ShoppingCart.DeserializeProduct(hdnProductID.Value);
                    if (product != null)
                    {
                        if (!cbRemoved.Checked)
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
            }
            Products = loadProducts;
        }

        private void HideCouponErrors()
        {
            phCouponError.Visible = false;
            phCouponError2.Visible = false;
        }

        private void ShowCouponError()
        {
            phCouponError.Visible = true;
        }

        private void ShowCouponError2()
        {
            phCouponError2.Visible = true;
        }

        protected void bApplyCoupon_Click(object sender, EventArgs e)
        {
            ICoupon c = (new RegistrationService()).GetCampaignDiscount(tbCoupon.Text.Trim(), CampaignID.Value);
            if (c != null)
            {
                CouponCode = c.CouponCode;
                coupon = c;
                tbCoupon.Text = "";
                OnCouponAdded();
            }
            else
            {
                //Check if user tries Gift Certificate here:
                PromoGift g = (new SaleService()).GetGiftCertificateByNumber(tbCoupon.Text.Trim());
                if (g != null && g.RemainingValue != null && g.RemainingValue.Value > 0M)
                {
                    OnGiftCertificatePopulated(g.GiftNumber);
                    tbCoupon.Text = "";
                    //ShowCouponError2();
                }
                else
                {
                    ShowCouponError();
                }
            }
        }

        protected void bRemoveCoupon_Click(object sender, EventArgs e)
        {
            OnCouponRemoved();
        }

        protected void bRemoveGift_Click(object sender, EventArgs e)
        {
            OnGiftCertificateRemoved();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            HideCouponErrors();
            //HideRefererCodeError();

            if (Page.IsPostBack && Products == null)
            {
                LoadProducts();
            }
            DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rProducts.DataSource = Products;
        }

        protected void ddlShipping_Changed(object sender, EventArgs e)
        {
            DataBind();
        }
    }
}