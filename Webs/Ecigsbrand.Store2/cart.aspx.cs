using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Ecigsbrand.Store2.Logic;
using TrimFuel.Model;
using System.Text.RegularExpressions;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using System.Web.SessionState;
using Ecigsbrand.Store2.Controls;

namespace Ecigsbrand.Store2
{
    public partial class cart : PageX
    {
        public enum CartState
        {
            Cart = 1,
            Checkout = 2
        }

        public CartState CurrentState
        {
            get 
            {
                if (ViewState["CurrentState"] == null)
                {
                    ViewState["CurrentState"] = CartState.Cart;
                }
                return (CartState)ViewState["CurrentState"]; 
            }
            set { ViewState["CurrentState"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ShoppingCart1.CampaignID = ShoppingCart.CampaignID;
            HideErrors();
        }

        protected void lbCheckout_Click(object sender, EventArgs e)
        {
            ShoppingCart1_ProductsChanged(this, EventArgs.Empty);
            if (ValidateProducts())
            {
                CurrentState = CartState.Checkout;
            }
            Page.DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            ShoppingCart1.Products = ShoppingCart.Products;
            ShoppingCart1.CouponCode = ShoppingCart.CouponCode;
            ShoppingCart1.RefererCode = ShoppingCart.RefererCode;


            //BillingInfo1.BillingID = ShoppingCart.FailedBillingID;
        }

        protected void ShoppingCart1_ProductsChanged(object sender, EventArgs e)
        {
            ShoppingCart.Products = ShoppingCart1.Products;
            ShoppingCart.Save();
            DataBind();
        }

        protected void ShoppingCart1_CouponChanged(object sender, EventArgs e)
        {
            ShoppingCart.Products = ShoppingCart1.Products;
            ShoppingCart.CouponCode = ShoppingCart1.CouponCode;
            ShoppingCart.RefererCode = ShoppingCart1.RefererCode;
            ShoppingCart.Save();
            DataBind();
        }

        protected void ShoppingCart1_RefererCodeChanged(object sender, EventArgs e)
        {
            ShoppingCart.Products = ShoppingCart1.Products;
            ShoppingCart.CouponCode = ShoppingCart1.CouponCode;
            ShoppingCart.RefererCode = ShoppingCart1.RefererCode;
            ShoppingCart.Save();
            DataBind();
        }

        protected void ShoppingCart1_GiftCertificateAdded(object sender, GiftCertificateEventArgs e)
        {
            if (!ShoppingCart.GiftCertificateList.Contains(e.GiftCertificateNumber))
            {
                ShoppingCart.GiftCertificateList.Add(e.GiftCertificateNumber);
                ShoppingCart.Save();
                DataBind();
            }
        }

        protected void ShoppingCart1_GiftCertificatePopulated(object sender, GiftCertificateEventArgs e)
        {
            BillingInfo1.PopulateGiftCertificate(e.GiftCertificateNumber);
        }

        protected void BillingInfo1_GiftCertificateRemoved(object sender, GiftCertificateEventArgs e)
        {
            if (ShoppingCart.GiftCertificateList.Contains(e.GiftCertificateNumber))
            {
                ShoppingCart.GiftCertificateList.Remove(e.GiftCertificateNumber);
                ShoppingCart.Save();
                DataBind();
            }
        }

        protected void ShoppingCart1_EcigBucksRemoved(object sender, EventArgs e)
        {
            ShoppingCart.EcigBucksAmount = null;
            ShoppingCart.Save();
            DataBind();
        }

        protected void BillingInfo1_EcigBucksApplied(object sender, EcigBucksEventArgs e)
        {
            ShoppingCart.EcigBucksAmount = e.EcigBucksAmount;
            ShoppingCart.Save();
            DataBind();
        }

        protected void lbCompleteOrder_Click(object sender, EventArgs e)
        {
            ShoppingCart.Products = ShoppingCart1.Products;
            ShoppingCart.CouponCode = ShoppingCart1.CouponCode;
            ShoppingCart.RefererCode = ShoppingCart1.RefererCode;
            ShoppingCart.Save();

            if (ValidateProducts() && ValidateBilling())
            {
                Billing b = BillingInfo1.Billing;
                Dictionary<int, int> subscriptions = new Dictionary<int,int>();
                Dictionary<int, int> upsells = new Dictionary<int,int>();
                foreach (KeyValuePair<ShoppingCartProduct, int> item in ShoppingCart.Products)
	            {
                    if (item.Key.ProductType == ShoppingCartProductType.Subscription)
                    {
                        subscriptions.Add(item.Key.ProductID, item.Value);
                    }
                    else if (item.Key.ProductType == ShoppingCartProductType.Upsell)
                    {
                        upsells.Add(item.Key.ProductID, item.Value);
                    }
	            }
                IList<PromoGift> promoGiftsUsed = (new SaleService()).GetGiftCertificateByNumber(ShoppingCart.GiftCertificateList);
                BusinessError<ComplexSaleView> res = (new SaleService()).CreateComplexSale(((Membership.CurrentReferer != null) ? Membership.CurrentReferer.RefererID : null),
                    b.FirstName, b.LastName, b.Address1, b.Address2, b.City, b.State, b.Zip, b.Country, b.Phone, b.Email,
                    ShoppingCart.CampaignID, ShoppingCart.Affiliate, ShoppingCart.SubAffiliate, ShoppingCart.IP, ShoppingCart.Url,
                    b.PaymentTypeID, b.CreditCard, b.CVV, b.ExpMonth, b.ExpYear,
                    ShoppingCart.ClickID, false, ShoppingCart.CouponCode,ShoppingCart.RefererCode, ShoppingCart.EcigBucksAmount, ShoppingCart.GiftCertificateList, 
                    subscriptions, upsells);

                if (res != null && res.State == BusinessErrorState.Success)
                {
                    res.ReturnValue.PromoGiftList = promoGiftsUsed;
                    res.ReturnValue.RefererCommissionRedeem = ShoppingCart.EcigBucksAmount;
                    ShoppingCart.SaveOrder(res.ReturnValue);
                    Response.Redirect("order-confirmation.aspx");
                }
                else if (res != null)
                {
                    //if (res.ReturnValue.ParentBilling != null)
                    //{
                    //    ShoppingCart.FailedBillingID = res.ReturnValue.ParentBilling.BillingID;
                    //    ShoppingCart.Save();
                    //}
                    if (res.State == BusinessErrorState.Error && !string.IsNullOrEmpty(res.ErrorMessage))
                    {
                        ShowBillingError(res.ErrorMessage);
                    }
                    else
                    {
                        ShowBillingError("We're sorry, but your transaction failed. Unknown error occured while performing transaction. Please, verify your billing information and try again.");
                    }
                }
                else
                {
                    ShowBillingError("We're sorry, but your transaction failed. Unknown error occured while performing transaction. Please, verify your billing information and try again.");
                }
            }            
        }

        #region Validation

        private bool ValidateProducts()
        {
            if (ShoppingCart1.Products.Count() == 0)
            {
                ShowProductsError("Your shopping cart is empty. Please, choose products you wish in our E-Cigarettes Shop.<br/>" +
                    "<a href='e-cigarette-starter-kits.aspx'>E-Cigarettes Starter Kits</a><br/>" +
                    "<a href='e-cigarette-refill-cartridges.aspx'>E-Cigarette Refill Nicotine Cartridges</a><br/>" +
                    "<a href='e-cigarette-accessories.aspx'>E-Cigarette Accessories</a>");
                return false;
            }
            //Only one trial
            int trialCount = 0;
            foreach (KeyValuePair<ShoppingCartProduct, int> p in ShoppingCart1.Products)
            {
                KnownProduct product = ShoppingCart.GetProductNumber(p.Key);
                if (product == KnownProduct.StarterKit_TrialOrder_MentholFlavor ||
                    product == KnownProduct.StarterKit_TrialOrder_OriginalFlavor)
                {
                    trialCount = trialCount + p.Value;
                }
            }
            if (trialCount > 1)
            {
                ShowProductsError("Sorry, but only one trial package per order is allowed. Please remove extra trial kits to continue.");
                return false;
            }

            return true;
        }

        private bool ValidateBilling()
        {
            Billing b = BillingInfo1.Billing;
            if (Regex.IsMatch(b.FirstName, "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") && 
                Regex.IsMatch(b.LastName, "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") && 
                Regex.IsMatch(b.Address1, "^[a-zA-Z_0-9\\#\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Regex.IsMatch(b.City, "^[a-zA-Z_0-9\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                !string.IsNullOrEmpty(b.Country) &&
                (b.Country != RegistrationService.DEFAULT_COUNTRY || Regex.IsMatch(b.Zip, "^\\d{5}$")) &&
                (b.Country != RegistrationService.DEFAULT_COUNTRY || Regex.IsMatch(b.Phone, "^\\d{10}$")) &&
                Regex.IsMatch(b.Email, "^[a-zA-Z_0-9\\.\\-]+@([a-zA-Z_0-9\\-]+\\.)+[a-zA-Z0-9]{2,4}$") &&
                Regex.IsMatch(b.CVV, "^[\\d-]+$") &&
                Regex.IsMatch(b.CreditCardCnt.DecryptedCreditCard, "^[\\d-]+$"))
            {
                return true;
            }
            ShowBillingError("Please verify your billing information and try again.");
            return false;
        }

        private void HideErrors()
        {
            phProductsError.Visible = false;
            phBillingError.Visible = false;
        }

        private void ShowProductsError(string error)
        {
            lProductsError.Text = error;
            phProductsError.Visible = true;
        }

        private void ShowBillingError(string error)
        {
            lBillingError.Text = error;
            phBillingError.Visible = true;
        }

        #endregion

        protected string KeepShoppingUrl
        {
            get
            {
                string urlReferrer = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : string.Empty;
                if (!urlReferrer.Contains("e-cigarette-starter-kits.aspx") &&
                    !urlReferrer.Contains("e-cigarette-accessories.aspx") &&
                    !urlReferrer.Contains("e-cigarette-accessories.aspx"))
                {
                    return "e-cigarette-starter-kits.aspx";
                }
                return urlReferrer;
            }
        }
    }
}
