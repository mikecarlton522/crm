using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;

namespace Securetrialoffers.ec07
{
    public partial class billing_information : System.Web.UI.Page
    {
        public enum Action
        {
            Index,
            Register,
            SetCoupon,
            Order
        }

        protected Action currentAction = Action.Index;

        private const int CAMPAIGN_ID = 229;
        private const int DEFAULT_SUBSCRIPTION_ID = 62;
        private const string PROMO_SPECIALOFFER = "specialoffer";
        private const string EXIT_POPUP_DISABLE_VALUE = "0";
        private const string AFFILATE_CODE_REBILL = "rebill";

        private const string ACTION_SET_COUPON = "coupon";
        private const string ACTION_BILL = "order";

        #region Inout Params

        protected int? campaignID = null;

        protected string affiliateCode = null;
        protected string subAffiliateCode = null;
        protected string test = null;
        protected string clickID = null;
        protected string exitPopup = null;
        protected string promo = null;

        protected string ip = null;
        protected string url = null;

        protected int? subscriptionID = null;
        protected long? registrationID = null;
        protected long? billingID = null;
        protected string error_msg = null;
        protected string coupon = null;
        protected string couponCode = null;
        protected string action = null;

        private void LoadInput()
        {
            campaignID = CAMPAIGN_ID;

            affiliateCode = Utility.TryGetStr(Request.Params["aff"]);
            subAffiliateCode = Utility.TryGetStr(Request.Params["sub"]);
            test = Utility.TryGetStr(Request.Params["test"]);
            clickID = Utility.TryGetStr(Request.Params["cid"]);
            exitPopup = Utility.TryGetStr(Request.Params["exit"]);
            promo = Utility.TryGetStr(Request.Params["promo"]);

            ip = Utility.TryGetStr(Request.Params["REMOTE_ADDR"]);
            url = Utility.TryGetStr(Request.Params["SERVER_NAME"]);

            registrationID = Utility.TryGetInt(Request.Params["id"]);
            billingID = Utility.TryGetInt(Request.Params["bid"]);
            subscriptionID = Utility.TryGetInt(Request.Params["choice"]) ?? DEFAULT_SUBSCRIPTION_ID;
            coupon = Utility.TryGetStr(Request.Params["coupon"]);
            couponCode = Utility.TryGetStr(Request.Params["couponCode"]);
            action = Utility.TryGetStr(Request.Params["_action"]);
        }

        #endregion

        #region Entities

        public Billing Billing { get; set; }
        public Registration Registration { get; set; }
        public Subscription Subscription { get; set; }
        public ICoupon Coupon { get; set; }

        private void LoadEntities()
        {
            if (couponCode != null)
            {
                Coupon = (new RegistrationService()).GetCampaignDiscount(couponCode, campaignID);
            }
            if (subscriptionID != null)
            {
                Subscription = (new BaseService()).Load<Subscription>(subscriptionID);
            }
            if (registrationID != null)
            {
                Registration = (new BaseService()).Load<Registration>(registrationID);
            }
            if (billingID != null)
            {
                Billing = (new BaseService()).Load<Billing>(billingID);
                //Do not allow to show credit card from existed billing
                Billing.CreditCard = null;
                Billing.CVV = null;
            }
            else
            {
                Billing = new Billing();
                Billing.FillFromRegistration(Registration);
            }

            //Fill billing with form fields
            Billing.FirstName = Utility.TryGetStr(Request.Params["First_Name"]) ?? Billing.FirstName;
            Billing.LastName = Utility.TryGetStr(Request.Params["Last_Name"]) ?? Billing.LastName;
            Billing.Address1 = Utility.TryGetStr(Request.Params["Billing_Address_1"]) ?? Billing.Address1;
            Billing.Address2 = Utility.TryGetStr(Request.Params["Billing_Address_2"]) ?? Billing.Address2;
            Billing.City = FormatZip(Utility.TryGetStr(Request.Params["City"])) ?? Billing.City;
            Billing.State = Utility.TryGetStr(Request.Params["state"]) ?? Billing.State;
            Billing.Zip = FormatZip(Utility.TryGetStr(Request.Params["zip"])) ?? Billing.Zip;
            Billing.Phone = FormatPhone(Utility.TryGetStr(Request.Params["Home_Tel"])) ?? Billing.Phone;
            Billing.Email = Utility.TryGetStr(Request.Params["Email"]) ?? Billing.Email;

            Billing.CreditCard = Utility.TryGetStr(Request.Params["CC_Number"]) ?? Billing.CreditCard;
            Billing.CVV = Utility.TryGetStr(Request.Params["CVV"]) ?? Billing.CVV;
            Billing.PaymentTypeID = Utility.TryGetInt(Request.Params["PaymentTID"]) ?? Billing.PaymentTypeID;
            Billing.ExpMonth = Utility.TryGetInt(Request.Params["Month"]) ?? Billing.ExpMonth;
            Billing.ExpYear = Utility.TryGetInt(Request.Params["Year"]) ?? Billing.ExpYear;
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadInput();
            LoadEntities();

            currentAction = DetermineAction();
            ProcessAction(currentAction);
        }

        #region Actions

        private Action DetermineAction()
        {
            if (registrationID == null && !string.IsNullOrEmpty(Request.Params["firstname"]))
            {
                return Action.Register;
            }
            else if (action == ACTION_SET_COUPON)
            {
                return Action.SetCoupon;
            }
            else if (action == ACTION_BILL && !string.IsNullOrEmpty(Request.Params["CC_Number"]))
            {
                return Action.Order;
            }
            return Action.Index;
        }

        private void ProcessAction(Action action)
        {
            switch (action)
            {
                case Action.Register:
                    Register();
                    break;
                case Action.SetCoupon:
                    SetCoupon();
                    break;
                case Action.Order:
                    Order();
                    break;
                default:
                    Index();
                    break;
            }            
        }

        private void Index()
        {
            if (Registration == null)
            {
                Registration = new Registration();
            }            
        }

        private void Register()
        {
            //form fields
            string firstname = Utility.TryGetStr(Request.Params["firstname"]);
            string lastname = Utility.TryGetStr(Request.Params["lastname"]);
            string address1 = Utility.TryGetStr(Request.Params["address1"]);
            string address2 = Utility.TryGetStr(Request.Params["address2"]);
            string city = Utility.TryGetStr(Request.Params["city"]);
            string state = Utility.TryGetStr(Request.Params["state"]);
            string zip = FormatZip(Utility.TryGetStr(Request.Params["zip"]));
            string phone = FormatPhone(Utility.TryGetStr(Request.Params["phone"]));
            string email = Utility.TryGetStr(Request.Params["email"]);

            //Create registration
            Registration = (new RegistrationService()).CreateRegistration(campaignID, firstname, lastname, address1, address2,
                city, state, zip, null, email, phone, DateTime.Now, affiliateCode, subAffiliateCode, ip, url);

            if (Registration != null)
            {
                registrationID = Registration.RegistrationID;
            }
        }

        private void SetCoupon()
        {
            if (campaignID != null)
            {
                Coupon = (new RegistrationService()).GetCampaignDiscount(coupon, campaignID);
                if (Coupon != null)
                {
                    couponCode = Coupon.CouponCode;
                }
                else
                {
                    Coupon = null;
                }
            }            
        }

        private void Order()
        {
            BusinessError<Set<Registration, Billing>> res = (new SaleService()).CreateBillingSale(billingID, registrationID,
                Billing.FirstName, Billing.LastName, Billing.Address1, Billing.Address2, Billing.City, Billing.State, Billing.Zip, Billing.Country,
                Billing.Phone, Billing.Email,
                Billing.PaymentTypeID, Billing.CreditCard, Billing.CVV, Billing.ExpMonth, Billing.ExpYear,
                campaignID, affiliateCode, subAffiliateCode, ip, url, 
                clickID, IsSpecialOffer, couponCode, null,
                subscriptionID.Value);

            if (res != null && res.State == BusinessErrorState.Success)
            {
                Response.Redirect("Redirect success");
            }
            else if (res != null)
            {
                error_msg = res.ErrorMessage;

                Registration = res.ReturnValue.Value1;
                Billing = res.ReturnValue.Value2;

                registrationID = Registration.RegistrationID;
                billingID = Billing.BillingID;

                if (error_msg == "Redirect to Google")
                {
                    Response.Redirect("http://www.google.com");
                }
            }
            else
            {
                error_msg = "We're sorry, but your transaction failed. Unknown error occured while performing transaction. Please, verify your billing information and try again.";
            }
        }

        #endregion

        #region Helpers

        protected bool IsExitPopup
        {
            get { return (!IsSpecialOffer && Coupon == null && exitPopup != EXIT_POPUP_DISABLE_VALUE); }
        }

        protected bool IsSpecialOffer
        {
            get { return (promo == PROMO_SPECIALOFFER); }
        }

        public bool IsRebill 
        {
            get { return affiliateCode == AFFILATE_CODE_REBILL; }
        }

        public decimal? TotalPrice 
        { 
            get 
            {
                decimal? res = null;
                if (Subscription != null)
                {
                    res = (!IsSpecialOffer) ? Subscription.InitialShipping : Subscription.SaveShipping;
                    if (Coupon != null && res != null)
                    {
                        res = Coupon.ApplyDiscount(res.Value, DiscountType.Any);
                    }
                }
                return res;
            } 
        }

        #endregion

        #region Utility

        private string FormatPhone(string phone)
        {
            if (!string.IsNullOrEmpty(phone))
            {
                return phone.Replace(",", "");
            }
            return null;
        }

        private string FormatZip(string zip)
        {
            if (!string.IsNullOrEmpty(zip))
            {
                return zip.Replace(",", "");
            }
            return null;
        }

        #endregion
    }
}
