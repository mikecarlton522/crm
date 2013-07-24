using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BeautyTruth.Store1.Logic;
using BeautyTruth.Store1.Controls;
using TrimFuel.Model;
using System.Text.RegularExpressions;
using TrimFuel.Business;
using TrimFuel.Model.Containers;
using TrimFuel.Model.Views;

namespace BeautyTruth.Store1
{
    public partial class Cart : PageCartEx
    {
        private bool wasComplete = false;
        private bool wasError = false;
        private int prevStepWithError = 0;
        protected string[] StepTitles
        {
            get
            {
                return new string[] { "Step 1: Shopping Cart", "Step 2: Account Information", "Step 3: Review Order", "Step 4: Complete!" };
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Session["Checkout"] = Checkout;
            Session["ShoppingCart"] = ShoppingCart;
            ShoppingCart.CampaignID = base.ShoppingCart.CampaignID;
            HideErrors();
            if (IsPostBack)
            {

            }
        }

        protected void StepChanged(object sender, EventArgs e)
        {
            if (wasError)
                wSteps.ActiveStepIndex = prevStepWithError;
        }

        protected void DeactivateWebStep1(object sender, EventArgs e)
        {
            if (wSteps.ActiveStepIndex >= 0)
                if (!ValidateProducts())
                {
                    wasError = true;
                    prevStepWithError = 0;
                    return;
                }
        }

        protected void DeactivateWebStep2(object sender, EventArgs e)
        {
            if (wSteps.ActiveStepIndex >= 1)
                if (!ValidateBilling() || !ValidateShipping())
                {
                    wasError = true;
                    prevStepWithError = 1;
                    return;
                }
        }

        protected void Step2_Activate(object sender, EventArgs e)
        {
            ShoppingCart_ProductsChanged(null, null);
            DeactivateWebStep1(null, null);

            LinkButton btn = wSteps.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton") as LinkButton;
            btn.Text = "Continue and Review Order »";
            btn.ValidationGroup = "billingGroup";
        }

        protected void Step3_Activate(object sender, EventArgs e)
        {
            DeactivateWebStep1(null, null);
            DeactivateWebStep2(null, null);

            LinkButton btn = wSteps.FindControl("StepNavigationTemplateContainerID").FindControl("StepNextButton") as LinkButton;
            btn.Text = "Complete Order »";
            btn.ValidationGroup = "";
        }

        protected void Step4_Activate(object sender, EventArgs e)
        {
            DeactivateWebStep1(null, null);
            DeactivateWebStep2(null, null);
            if (!wasError)
            {
                if (!wasComplete)
                    wSteps_FinishButtonClick(null, null);

                if (!wasError)
                {
                    DataList SideBarList = wSteps.FindControl("SideBarContainer").FindControl("SideBarList") as DataList;
                    SideBarList.Enabled = false;
                    this.ShoppingCart.Products = new Dictionary<ShoppingCartProduct, int>();
                    base.ShoppingCart.GiftCertificateList.Clear();
                    base.ShoppingCart.CouponCode = null;
                    base.ShoppingCart.RefererCode = null;
                    ShoppingCart_ProductsChanged(null, null);
                }
            }
        }

        protected void ShoppingCart_ProductsChanged(object sender, EventArgs e)
        {
            base.ShoppingCart.Products = this.ShoppingCart.Products;
            base.ShoppingCart.Save();
            DataBind();
        }

        protected void ShoppingCart_CouponAdded(object sender, EventArgs e)
        {
            base.ShoppingCart.Products = ShoppingCart.Products;
            base.ShoppingCart.CouponCode = ShoppingCart.CouponCode;
            base.ShoppingCart.RefererCode = ShoppingCart.RefererCode;
            base.ShoppingCart.Save();
            DataBind();
        }

        protected void ShoppingCart_GiftCertificatePopulated(object sender, GiftCertificateEventArgs e)
        {
            base.ShoppingCart.GiftCertificateList.Clear();
            base.ShoppingCart.GiftCertificateList.Add(e.GiftCertificateNumber);
            base.ShoppingCart.Save();
            DataBind();
        }

        protected void ShoppingCart_CouponRemoved(object sender, EventArgs e)
        {
            base.ShoppingCart.Products = ShoppingCart.Products;
            base.ShoppingCart.CouponCode = null;
            ShoppingCart.coupon = null;
            base.ShoppingCart.RefererCode = ShoppingCart.RefererCode;
            base.ShoppingCart.Save();
            DataBind();
        }

        protected void ShoppingCart_GiftRemoved(object sender, EventArgs e)
        {
            base.ShoppingCart.GiftCertificateList.Clear();
            base.ShoppingCart.Save();
            DataBind();
        }

        protected void bUpdateQuantities_Click(object sender, EventArgs e)
        {
            ShoppingCart_ProductsChanged(null, null);
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            DataList SideBarList = wSteps.FindControl("SideBarContainer").FindControl("SideBarList") as DataList;
            SideBarList.DataSource = StepTitles;

            ShoppingCart.Products = base.ShoppingCart.Products;
            ShoppingCart.CouponCode = base.ShoppingCart.CouponCode;
            ShoppingCart.RefererCode = base.ShoppingCart.RefererCode;
        }

        #region Validation

        private bool ValidateProducts()
        {
            if (base.ShoppingCart.Products.Count() == 0)
            {
                ShowError(
                    "Your shopping cart is empty. Please, choose products you wish in our Beauty Truth Shop.<br/>" +
                    "<a href='category.aspx'>Beauty Truth</a><br/>");
                return false;
            }
            //Only one trial
            int trialCount = 0;
            foreach (KeyValuePair<ShoppingCartProduct, int> p in base.ShoppingCart.Products)
            {
                if (p.Key.ProductType == ShoppingCartProductType.Subscription && p.Value > 1)
                {
                    trialCount = p.Value;
                    break;
                }
            }
            if (trialCount > 1)
            {
                ShowError("Sorry, but only one subscription package is allowed. Please remove extra subscription to continue.");
                return false;
            }

            return true;
        }

        private bool ValidateBilling()
        {
            Billing b = Checkout.Billing;

            if(Checkout.IsGiftCertificateValidate)
            {
                if (b == null || b.FirstName == null || b.LastName == null || b.Address1 == null || b.City == null || b.Email == null
                    || b == null)
                {
                    ShowError("Please verify your billing information and try again.");
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
            }
            else
            {
                if (b == null || b.FirstName == null || b.LastName == null || b.Address1 == null || b.City == null || b.Email == null
                   || b == null || b.CVV == null || !CreditCard.ValidateDecryptedFormat(b.CreditCard))
                {
                    ShowError("Please verify your billing information and try again.");
                    return false;
                }

                if (Regex.IsMatch(b.FirstName, "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                    Regex.IsMatch(b.LastName, "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                    Regex.IsMatch(b.Address1, "^[a-zA-Z_0-9\\#\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                    Regex.IsMatch(b.City, "^[a-zA-Z_0-9\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                    !string.IsNullOrEmpty(b.Country) &&
                    (b.Country != RegistrationService.DEFAULT_COUNTRY || Regex.IsMatch(b.Zip, "^\\d{5}$")) &&
                    (b.Country != RegistrationService.DEFAULT_COUNTRY || Regex.IsMatch(b.Phone, "^\\d{10}$")) &&
                    Regex.IsMatch(b.Email, "^[a-zA-Z_0-9\\.\\-]+@([a-zA-Z_0-9\\-]+\\.)+[a-zA-Z0-9]{2,4}$") &&
                    Regex.IsMatch(b.CVV, "^[\\d-]+$") &&
                    Regex.IsMatch(b.CreditCardCnt.DecryptedCreditCard, "^[\\d-]+$")
                    )
                {
                    return true;
                }   
            }

            ShowError("Please verify your billing information and try again.");
            return false;
        }

        private bool ValidateShipping()
        {
            if (Checkout.cbUseShippingAsBilling.Checked)
                return true;

            Registration r = Checkout.Registration;
            RegistrationInfo ri = Checkout.RegistrationInfo;

            if (r == null || r.FirstName == null || r.LastName == null || r.Address1 == null || r.City == null || r.Email == null
                || r == null)
            {
                ShowError("Please verify your shipping information and try again.");
                return false;
            }

            if (Regex.IsMatch(r.FirstName, "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Regex.IsMatch(r.LastName, "^[a-zA-Z_\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Regex.IsMatch(r.Address1, "^[a-zA-Z_0-9\\#\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                Regex.IsMatch(r.City, "^[a-zA-Z_0-9\\-\\.\\,\\(\\)\\'\\\"\\s]{2,}$") &&
                !string.IsNullOrEmpty(ri.Country) &&
                (ri.Country != RegistrationService.DEFAULT_COUNTRY || Regex.IsMatch(r.Zip, "^\\d{5}$")) &&
                (ri.Country != RegistrationService.DEFAULT_COUNTRY || Regex.IsMatch(r.Phone, "^\\d{10}$")) &&
                Regex.IsMatch(r.Email, "^[a-zA-Z_0-9\\.\\-]+@([a-zA-Z_0-9\\-]+\\.)+[a-zA-Z0-9]{2,4}$")
                )
            {
                return true;
            }

            ShowError("Please verify your shipping information and try again.");
            return false;
        }

        #endregion

        #region Errors Section

        private void HideErrors()
        {
            GetErrorPlaceHolder().Visible = false;
        }

        private void ShowError(string error)
        {
            GetErrorPlaceHolder().Visible = true;
            (GetErrorPlaceHolder().FindControl("lError") as Literal).Text = error;
        }

        private PlaceHolder GetErrorPlaceHolder()
        {
            return wSteps.FindControl("SideBarContainer").FindControl("phError") as PlaceHolder;
        }

        #endregion

        protected void wSteps_FinishButtonClick(object sender, EventArgs e)
        {
            if (wSteps.ActiveStepIndex == 1)
                return;

            if (ValidateProducts() && ValidateBilling())
            {
                Billing b = Checkout.Billing;
                Registration r = Checkout.Registration;
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
                        var sServise = new SaleService();
                        var campaign = sServise.Load<Campaign>(GeneralInfo.CampaignID);
                        if (campaign != null)
                        {
                            var subscription = sServise.Load<Subscription>(campaign.SubscriptionID);
                            if (subscription != null)
                            {
                                var upType = new SaleService().GetOrCreateUpsell(subscription.ProductID, item.Key.ProductSKU, (short)item.Value, item.Key.Price);
                                if (upType != null)
                                    upsells.Add(upType.UpsellTypeID.Value, item.Value);
                            }
                        }
                    }
                }
                IList<PromoGift> promoGiftsUsed =
                    (new SaleService()).GetGiftCertificateByNumber(base.ShoppingCart.GiftCertificateList);

                BusinessError<ComplexSaleView> res = null;
                if (Checkout.ShippingAsBilling)
                {
                    res =
                        (new SaleService()).CreateComplexSale(((Membership.CurrentReferer != null) ? Membership.CurrentReferer.RefererID : null),
                            b.FirstName, b.LastName, b.Address1, b.Address2, b.City, b.State, b.Zip, b.Country, b.Phone,
                            b.Email,
                            this.Checkout.Password,
                            ShoppingCart.CampaignID, base.ShoppingCart.Affiliate, base.ShoppingCart.SubAffiliate, base.ShoppingCart.IP,
                            base.ShoppingCart.Url,
                            b.PaymentTypeID, b.CreditCard, b.CVV, b.ExpMonth, b.ExpYear,
                            base.ShoppingCart.ClickID, false, ShoppingCart.CouponCode, ShoppingCart.RefererCode,
                            base.ShoppingCart.EcigBucksAmount, base.ShoppingCart.GiftCertificateList,
                            subscriptions, upsells, this.ShoppingCart.ShippingValue);
                }
                else
                {
                    res =
                        (new SaleService()).CreateComplexSale(((Membership.CurrentReferer != null) ? Membership.CurrentReferer.RefererID : null),
                            b.FirstName, b.LastName, b.Address1, b.Address2, b.City, b.State, b.Zip, b.Country, b.Phone,
                            b.Email,
                            r.FirstName, r.LastName, r.Address1, r.Address2, r.City, r.State, r.Zip, Checkout.RegistrationInfo.Country,
                            r.Phone, r.Email,
                            this.Checkout.Password,
                            ShoppingCart.CampaignID, base.ShoppingCart.Affiliate, base.ShoppingCart.SubAffiliate, base.ShoppingCart.IP,
                            base.ShoppingCart.Url,
                            b.PaymentTypeID, b.CreditCard, b.CVV, b.ExpMonth, b.ExpYear,
                            base.ShoppingCart.ClickID, false, ShoppingCart.CouponCode, ShoppingCart.RefererCode,
                            base.ShoppingCart.EcigBucksAmount, base.ShoppingCart.GiftCertificateList,
                            subscriptions, upsells, this.ShoppingCart.ShippingValue);
                }

                if (res != null && res.State == BusinessErrorState.Success)
                {
                    res.ReturnValue.PromoGiftList = promoGiftsUsed;
                    res.ReturnValue.RefererCommissionRedeem = base.ShoppingCart.EcigBucksAmount;
                    base.ShoppingCart.SaveOrder(res.ReturnValue);
                    wasComplete = true;
                }
                else if (res != null)
                {
                    //if (res.ReturnValue.ParentBilling != null)
                    //{
                    //base.ShoppingCart.FailedBillingID = res.ReturnValue.ParentBilling.BillingID;
                    //base.ShoppingCart.Save();
                    //}
                    if (res.State == BusinessErrorState.Error && !string.IsNullOrEmpty(res.ErrorMessage))
                    {
                        wasError = true;
                        prevStepWithError = 2;
                        ShowError(res.ErrorMessage);
                    }
                    else
                    {
                        wasError = true;
                        prevStepWithError = 2;
                        ShowError(
                            "We're sorry, but your transaction failed. Unknown error occured while performing transaction. Please, verify your billing information and try again.");
                    }
                }
                else
                {
                    wasError = true;
                    prevStepWithError = 2;
                    ShowError(
                        "We're sorry, but your transaction failed. Unknown error occured while performing transaction. Please, verify your billing information and try again.");
                }
            }
        }
    }
}
