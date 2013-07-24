using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;
using TrimFuel.Model;
using TrimFuel.Business;
using FitDiet.Store1.Controls;
using TrimFuel.Business.Utils;

namespace Fitdiet.Store1.Controls
{
    public partial class ShoppingCart_ : UserControl, IStepControl
    {
        void IStepControl.ShowError(string errorText)
        {
            lError.Text = errorText;
            phError.Visible = true;
        }

        void IStepControl.HideError()
        {
            phError.Visible = false;
        }

        public event EventHandler<StepChangeEventArgs> StepChanged;
        protected void OnStepChanged(StepChangeEventArgs e)
        {
            if (StepChanged != null)
            {
                StepChanged(this, e);
            }
        }

        protected void ChangeStep_Click(object sender, StepChangeEventArgs e)
        {
            OnStepChanged(e);
        }

        public event EventHandler<EventArgs> EcigBucksRemoved;
        protected void OnEcigBucksRemoved()
        {
            if (EcigBucksRemoved != null)
            {
                EcigBucksRemoved(this, EventArgs.Empty);
            }
        }

        public event EventHandler<GiftCertificateEventArgs> GiftCertificateRemoved;
        protected void OnGiftCertificateRemoved(string giftCertificateNumber)
        {
            if (GiftCertificateRemoved != null)
            {
                GiftCertificateRemoved(this, new GiftCertificateEventArgs(giftCertificateNumber));
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

        public event EventHandler<GiftCertificateEventArgs> GiftCertificatePopulated;
        protected void OnGiftCertificatePopulated(string giftCertificateNumber)
        {
            if (GiftCertificatePopulated != null)
            {
                GiftCertificatePopulated(this, new GiftCertificateEventArgs(giftCertificateNumber));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            HideCouponErrors();
            HideRefererCodeError();

            if (Page.IsPostBack && Products == null)
            {
                LoadProducts();
            }
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
            return price.ToString("c");
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
                tbCoupon.Text = "";
                OnCouponChanged();
            }
            else
            {
                //Check if user tries Gift Certificate here:
                PromoGift g = (new SaleService()).GetGiftCertificateByNumber(tbCoupon.Text.Trim());
                if (g != null && g.RemainingValue != null && g.RemainingValue.Value > 0M)
                {
                    OnGiftCertificatePopulated(g.GiftNumber);
                    tbCoupon.Text = "";
                    ShowCouponError2();
                }
                else
                {
                    ShowCouponError();
                }
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
            rGiftCertificateList.DataSource = ShoppingCart.Current.GetPromoGiftRedeemList();
        }

        protected void lbRemoveEcigBucks_Click(object sender, EventArgs e)
        {
            OnEcigBucksRemoved();
        }

        protected void rGiftCertificateList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                if (e.CommandName == "Remove")
                {
                    OnGiftCertificateRemoved(e.CommandArgument.ToString());
                }
            }
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
                return ShoppingCart.Current.TotalCost;
            }
        }

        protected void Products_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                OnProductsChanged();
            }
        }
    }
}