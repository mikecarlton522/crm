using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;
using TrimFuel.Model;
using System.Text.RegularExpressions;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using System.Web.SessionState;
using Fitdiet.Store1.Controls;
using FitDiet.Store1.Logic;
using FitDiet.Store1.Controls;
using TrimFuel.Model.Containers;

namespace Fitdiet.Store1
{
    public partial class cart : PageCartExt
    {
        #region StepControls

        private Dictionary<CartState, IStepControl> StepControls = new Dictionary<CartState, IStepControl>();

        private void InitializeStepControls()
        {
            StepControls.Add(CartState.Cart, ShoppingCart1);
            StepControls.Add(CartState.ShippingDetails, ShippingDetails);
            StepControls.Add(CartState.BillingDetails, BillingDetails);
            StepControls.Add(CartState.PlaceMyOrder, PlaceMyOrder);
        }

        #endregion

        protected Billing Billing
        {
            get
            {
                Billing b = ShippingDetails.Billing;
                b.PaymentTypeID = BillingDetails.Billing.PaymentTypeID;
                b.ExpYear = BillingDetails.Billing.ExpYear;
                b.ExpMonth = BillingDetails.Billing.ExpMonth;
                b.CVV = BillingDetails.Billing.CVV;
                b.CreditCard = BillingDetails.Billing.CreditCard;
                return b;
            }
        }

        public string CurrImage
        {
            get
            {
                string img = "images/next_out_btn.gif";
                if (CurrentState == CartState.Cart)
                    img = "images/chk_out_btn.gif";
                return img;
            }
        }

        protected string ValidateMethod
        {
            get
            {
                string method = "return validateBilling();";
                if (CurrentState == CartState.Cart)
                    method = "return validateProducts();";
                return method;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            InitializeStepControls();
            ShoppingCart1.CampaignID = ShoppingCart.CampaignID;
            PlaceMyOrder.CampaignID = ShoppingCart.CampaignID;
            HideErrors();
        }

        protected void lbCheckout_Click(object sender, EventArgs e)
        {
            ShoppingCart1_ProductsChanged(this, null);
            if (ValidateProducts())
            {
                CurrentState = (CartState)((int)CurrentState + 1);
            }
            Page.DataBind();
        }

        protected void ChangeStep_Click(object sender, StepChangeEventArgs e)
        {
            ShoppingCart1_ProductsChanged(null, null);
            int state = (int)CurrentState;
            bool isValid = true;
            if (state <= (int)e.ToState)
            {
                if ((int)e.ToState > 3)
                    if (!ValidateBilling())
                    {
                        isValid = false;
                        CurrentState = CartState.BillingDetails;
                    }
                if ((int)e.ToState > 2)
                    if (!ValidateShipping())
                    {
                        CurrentState = CartState.ShippingDetails;
                        isValid = false;
                    }
                if ((int)e.ToState > 1)
                    if (!ValidateProducts())
                    {
                        CurrentState = CartState.Cart;
                        isValid = false;
                    }
            }
            if (isValid)
                CurrentState = e.ToState;

            Page.DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            imbCheckout.ImageUrl = CurrImage;
            ShoppingCart1.Products = ShoppingCart.Products;
            ShoppingCart1.CouponCode = ShoppingCart.CouponCode;
            ShoppingCart1.RefererCode = ShoppingCart.RefererCode;
            PlaceMyOrder.Products = ShoppingCart.Products;
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
            BillingDetails.PopulateGiftCertificate(e.GiftCertificateNumber);
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
                Billing b = Billing;
                Dictionary<int, int> subscriptions = new Dictionary<int, int>();
                Dictionary<int, int> upsells = new Dictionary<int, int>();
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
                IList<PromoGift> promoGiftsUsed =
                    (new SaleService()).GetGiftCertificateByNumber(ShoppingCart.GiftCertificateList);
                BusinessError<ComplexSaleView> res =
                    (new SaleService()).CreateComplexSale(
                        ((Membership.CurrentReferer != null) ? Membership.CurrentReferer.RefererID : null),
                        b.FirstName, b.LastName, b.Address1, b.Address2, b.City, b.State, b.Zip, b.Country, b.Phone,
                        b.Email,
                        ShoppingCart.CampaignID, ShoppingCart.Affiliate, ShoppingCart.SubAffiliate, ShoppingCart.IP,
                        ShoppingCart.Url,
                        b.PaymentTypeID, b.CreditCard, b.CVV, b.ExpMonth, b.ExpYear,
                        ShoppingCart.ClickID, false, ShoppingCart.CouponCode, ShoppingCart.RefererCode,
                        ShoppingCart.EcigBucksAmount, ShoppingCart.GiftCertificateList,
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
                        ShowError(res.ErrorMessage, CartState.PlaceMyOrder);
                    }
                    else
                    {
                        ShowError(
                            "We're sorry, but your transaction failed. Unknown error occured while performing transaction. Please, verify your billing information and try again.", CartState.PlaceMyOrder);
                    }
                }
                else
                {
                    ShowError(
                        "We're sorry, but your transaction failed. Unknown error occured while performing transaction. Please, verify your billing information and try again.", CartState.PlaceMyOrder);
                }
            }
        }

        #region Validation

        private bool ValidateProducts()
        {
            if (ShoppingCart1.Products.Count() == 0)
            {
                ShowError(
                    "Your shopping cart is empty.",
                    CartState.Cart);
                return false;
            }
            //Only one trial
            int trialCount = 0;
            foreach (KeyValuePair<ShoppingCartProduct, int> p in ShoppingCart1.Products)
            {
                KnownProduct product = ShoppingCart.GetProductNumber(p.Key);
                if (product == KnownProduct.A30DayTrialKit)
                {
                    trialCount = trialCount + p.Value;
                }
            }
            if (trialCount > 1)
            {
                ShowError("Sorry, but only one trial package per order is allowed. Please remove extra trial kits to continue.", CartState.Cart);
                return false;
            }

            return true;
        }

        private bool ValidateShipping()
        {
            Billing b = ShippingDetails.Billing;

            if (b == null || b.FirstName == null || b.LastName == null || b.Address1 == null || b.City == null || b.Email == null)
            {
                ShowError("Please verify your shipping information and try again.", CartState.ShippingDetails);
                return false;
            }

            if (Regex.IsMatch(b.FirstName, "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Regex.IsMatch(b.LastName, "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Regex.IsMatch(b.Address1, "^[a-zA-Z_0-9\\#\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Regex.IsMatch(b.City, "^[a-zA-Z_0-9\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                !string.IsNullOrEmpty(b.Country) &&
                (b.Country != RegistrationService.DEFAULT_COUNTRY || Regex.IsMatch(b.Zip, "^\\d{5}$")) &&
                (b.Country != RegistrationService.DEFAULT_COUNTRY || Regex.IsMatch(b.Phone, "^\\d{10}$")) &&
                Regex.IsMatch(b.Email, "^[a-zA-Z_0-9\\.\\-]+@([a-zA-Z_0-9\\-]+\\.)+[a-zA-Z0-9]{2,4}$")
                )
            {
                return true;
            }
            ShowError("Please verify your shipping information and try again.", CartState.ShippingDetails);
            return false;
        }

        private bool ValidateBilling()
        {
            Billing b = BillingDetails.Billing;

            if (b == null || b.CVV == null || !CreditCard.ValidateDecryptedFormat(b.CreditCard))
            {
                ShowError("Please verify your billing information and try again.", CartState.BillingDetails);
                return false;
            }

            if (Regex.IsMatch(b.CVV, "^[\\d-]+$") &&
                Regex.IsMatch(b.CreditCardCnt.DecryptedCreditCard, "^[\\d-]+$"))
            {
                return true;
            }
            ShowError("Please verify your billing information and try again.", CartState.BillingDetails);
            return false;
        }

        private void HideErrors()
        {
            foreach (var control in StepControls)
            {
                control.Value.HideError();
            }
            //phError.Visible = false;
        }

        private void ShowError(string error, CartState state)
        {
            StepControls[state].ShowError(error);
            //lError.Text = error;
            //phError.Visible = true;
        }

        #endregion

        protected string KeepShoppingUrl
        {
            get
            {
                string urlReferrer = (Request.UrlReferrer != null) ? Request.UrlReferrer.ToString() : string.Empty;
                if (!urlReferrer.Contains("fitdiet-shop.aspx"))
                {
                    return "fitdiet-shop.aspx";
                }
                return urlReferrer;
            }
        }
    }
}
