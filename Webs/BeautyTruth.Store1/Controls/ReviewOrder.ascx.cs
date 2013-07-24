using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business;
using BeautyTruth.Store1.Logic;
using TrimFuel.Business.Utils;

namespace BeautyTruth.Store1.Controls
{
    public partial class ReviewOrder : System.Web.UI.UserControl
    {
        public bool IsGiftCertificateValidate
        {
            get { return ShoppingCart.Current.TotalCost == 0; }
        }

        protected string FormatPrice(decimal price)
        {
            return price.ToString("c");
        }
        protected decimal ShippingValue
        {
            get
            {
                if (Session["ShoppingCart"] == null)
                    return 0;
                return (Session["ShoppingCart"] as ShoppingCart_).ShippingValue;
            }
        }

        protected bool ShippingAsBilling
        {
            get
            {
                if (Session["Checkout"] == null)
                    return false;
                return (Session["Checkout"] as Checkout).ShippingAsBilling;
            }
        }

        public IEnumerable<KeyValuePair<ShoppingCartProduct, int>> Products { get; set; }
        public string CouponCode
        {
            get { return ShoppingCart.Current.CouponCode; }
        }
        public int? CampaignID { get; set; }

        private ICoupon coupon = null;
        protected ICoupon Coupon
        {
            get
            {
                if (coupon == null && CouponCode != null)
                {
                    coupon = (new RegistrationService()).GetCampaignDiscount(CouponCode, GeneralInfo.CampaignID.Value);
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

        #region From Billing Step

        public Billing Billing
        {
            get
            {
                if (Session["Checkout"] == null)
                    return new Billing();
                return ((Checkout)Session["Checkout"]).Billing;
            }
        }

        public Registration Registration
        {
            get
            {
                if (Session["Checkout"] == null)
                    return new Registration();
                return ((Checkout)Session["Checkout"]).Registration;
            }
        }

        public RegistrationInfo RegistrationInfo
        {
            get
            {
                if (Session["Checkout"] == null)
                    return new RegistrationInfo();
                return ((Checkout)Session["Checkout"]).RegistrationInfo;
            }
        }

        #endregion

        protected decimal TotalCost
        {
            get
            {
                return ShoppingCart.Current.TotalCost;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rProducts.DataSource = ShoppingCart.Current.Products;
            LoadBilling();
            LoadShipping();
        }

        private void LoadBilling()
        {
            if (Billing != null)
            {
                lblCreditCard.Text = Billing.PaymentTypeID == 2 ? "Visa " : (Billing.PaymentTypeID == 3 ? "MC " : "Amex ");
                if (Billing.CreditCard != null)
                {
                    lblCreditCard.Text += Billing.CreditCard.Remove(6, 6).Insert(6, "******");
                }
                //tbCreditCardCVV.Text = Billing.CVV;
                if (Billing.ExpMonth != null)
                {
                    lblExpMonth.Text = Billing.ExpMonth.ToString();
                }
                if (Billing.ExpYear != null)
                {
                    lblExpYear.Text = Billing.ExpYear.ToString();
                }

                lblFirstName.Text = Billing.FirstName;
                lblLastName.Text = Billing.LastName;
                lblAddress1.Text = Billing.Address1;
                lblAddress2.Text = Billing.Address2;
                lblCity.Text = Billing.City;
                lblState.Text = Billing.State;
                lblZip.Text = Billing.Zip;
                if (string.IsNullOrEmpty(Billing.Country) || Billing.Country == RegistrationService.DEFAULT_COUNTRY)
                {
                    lblCountry.Text = RegistrationService.DEFAULT_COUNTRY;
                    phPhoneEx.Visible = true;
                    lblPhone1.Text = Billing.PhoneCnt.Code;
                    lblPhone2.Text = Billing.PhoneCnt.Part1;
                    lblPhone3.Text = Billing.PhoneCnt.Part2;
                }
                else
                {
                    lblCountry.Text = RegistrationService.DEFAULT_COUNTRY;
                    phPhoneEx.Visible = false;
                    lblPhone1.Text = Billing.Phone;
                }
                lblEmail.Text = Billing.Email;
            }

            //EcigBucksAvailable = 0M;
            //EcigBucksAvailableToApply = 0M;
            //if (Referer != null)
            //{
            //    EcigBucksAvailable = (new RefererService()).GetAvailableAmountToRedeemInEcigsDollars(Referer.RefererID.Value);
            //    EcigBucksAvailable = EcigBucksAvailable - ShoppingCart.Current.EcigBucksRedeem;
            //    EcigBucksAvailableToApply = (ShoppingCart.Current.TotalCost > EcigBucksAvailable ? EcigBucksAvailable : ShoppingCart.Current.TotalCost);
            //}
        }

        private void LoadShipping()
        {
            if (!ShippingAsBilling)
            {
                lblShippingFirstName.Text = Registration.FirstName;
                lblShippingLastName.Text = Registration.LastName;
                lblShippingAddress1.Text = Registration.Address1;
                lblShippingAddress2.Text = Registration.Address2;
                lblShippingCity.Text = Registration.City;
                lblShippingState.Text = Registration.State;
                lblShippingZip.Text = Registration.Zip;
                lblShippingCountry.Text = RegistrationInfo.Country;
                lblShippingEmail.Text = Registration.Email;

                if (string.IsNullOrEmpty(RegistrationInfo.Country) || RegistrationInfo.Country == RegistrationService.DEFAULT_COUNTRY)
                {
                    phShippingPhoneEx.Visible = true;
                    lblShippingPhone1.Text = Registration.PhoneCnt.Code;
                    lblShippingPhone2.Text = Registration.PhoneCnt.Part1;
                    lblShippingPhone3.Text = Registration.PhoneCnt.Part2;
                }
                else
                {
                    phShippingPhoneEx.Visible = false;
                    lblShippingPhone1.Text = Registration.Phone;
                }

                return;
            }

            if (Billing != null)
            {
                lblShippingFirstName.Text = Billing.FirstName;
                lblShippingLastName.Text = Billing.LastName;
                lblShippingAddress1.Text = Billing.Address1;
                lblShippingAddress2.Text = Billing.Address2;
                lblShippingCity.Text = Billing.City;
                lblShippingState.Text = Billing.State;
                lblShippingEmail.Text = Billing.Email;
                lblShippingZip.Text = Billing.Zip;
                if (string.IsNullOrEmpty(Billing.Country) || Billing.Country == RegistrationService.DEFAULT_COUNTRY)
                {
                    lblShippingCountry.Text = RegistrationService.DEFAULT_COUNTRY;

                    phShippingPhoneEx.Visible = true;
                    lblShippingPhone1.Text = Billing.PhoneCnt.Code;
                    lblShippingPhone2.Text = Billing.PhoneCnt.Part1;
                    lblShippingPhone3.Text = Billing.PhoneCnt.Part2;
                }
                else
                {
                    phShippingPhoneEx.Visible = false;
                    lblShippingPhone1.Text = Billing.Phone;
                    lblShippingCountry.Text = RegistrationService.DEFAULT_COUNTRY;
                }
            }
        }
    }
}