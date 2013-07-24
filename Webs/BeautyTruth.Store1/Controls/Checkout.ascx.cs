using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BeautyTruth.Store1.Logic;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Containers;

namespace BeautyTruth.Store1.Controls
{
    public partial class Checkout : System.Web.UI.UserControl
    {
        public bool IsGiftCertificateValidate
        {
            get { return ShoppingCart.Current.TotalCost == 0; }
        }

        TrimFuel.Business.Controls.DDLPaymentType.RenderPaymentTypes PaymentTypesToRender
        {
            get
            {
                return TrimFuel.Business.Controls.DDLPaymentType.RenderPaymentTypes.Amex
                    | TrimFuel.Business.Controls.DDLPaymentType.RenderPaymentTypes.Visa
                    | TrimFuel.Business.Controls.DDLPaymentType.RenderPaymentTypes.MasterCard;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ddlPaymentType.PaymentTypes = PaymentTypesToRender;
            ddlPaymentType.DataBind();
            if (Page.IsPostBack)
            {
                LoadBilling();
                LoadRegistration();
            }
        }

        public string Password
        {
            get
            {
                if (tbPassword.Text == tbPasswordAgain.Text)
                    return tbPassword.Text;
                else
                    return null;
            }
        }
        
        public bool ShippingAsBilling
        {
            get
            {
                return cbUseShippingAsBilling.Checked;
            }
        }

        private Billing billing = null;
        public Billing Billing
        {
            get
            {
                if (billing == null && Referer != null && Referer.RefererID != null)
                {
                    billing = (new RefererService()).GetLastBillingByReferer(Referer.RefererID.Value);
                    if (billing != null)
                    {
                        billing.CreditCard = null;
                        billing.CVV = null;
                        billing.ExpMonth = null;
                        billing.ExpYear = null;
                    }
                    else
                    {
                        billing = new Billing();
                    }
                }
                else if (billing == null)
                {
                    billing = new Billing();
                }
                return billing;
            }
            set
            {
                billing = value;
                DataBind();
            }
        }
        private Registration registration = null;
        public Registration Registration
        {
            get
            {
                if (registration == null)
                    if (Billing != null && Billing.RegistrationID != null)
                        registration = new RegistrationService().Load<Registration>(Billing.RegistrationID);
                if (registration == null)
                    registration = new Registration();
                return registration;
            }
            set
            {
                registration = value;
                DataBind();
            }
        }
        private RegistrationInfo registrationInfo = null;
        public RegistrationInfo RegistrationInfo
        {
            get
            {
                if (registrationInfo == null)
                    if (Billing != null && Billing.RegistrationID != null)
                        registrationInfo = new RegistrationService().GetRegistrationInfo(Billing.RegistrationID.Value);
                if (registrationInfo == null)
                    registrationInfo = new RegistrationInfo();
                return registrationInfo;
            }
            set
            {
                registrationInfo = value;
                DataBind();
            }
        }
        public Referer Referer { get { return Membership.CurrentReferer; } }
        public decimal EcigBucksAvailable { get; set; }
        public decimal EcigBucksAvailableToApply { get; set; }

        private void LoadBilling()
        {
            Billing.PaymentTypeID = Utility.TryGetInt(ddlPaymentType.SelectedValue);
            if (CreditCard.ValidateDecryptedFormat(tbCreditCardNumber.Text.Trim()))
                Billing.CreditCard = Utility.TryGetStr(tbCreditCardNumber.Text.Trim());
            Billing.CVV = Utility.TryGetStr(tbCreditCardCVV.Text.Trim());
            Billing.ExpMonth = Utility.TryGetInt(ddlExpireMonth.SelectedValue);
            Billing.ExpYear = Utility.TryGetInt(ddlExpireYear.SelectedValue);

            Billing.FirstName = Utility.TryGetStr(tbFirstName.Text.Trim());
            Billing.LastName = Utility.TryGetStr(tbLastName.Text.Trim());
            Billing.Address1 = Utility.TryGetStr(tbAddress1.Text.Trim());
            Billing.Address2 = Utility.TryGetStr(tbAddress2.Text.Trim());
            Billing.City = Utility.TryGetStr(tbCity.Text.Trim());
            Billing.Country = Utility.TryGetStr(ddlCountry.SelectedValue);
            if (Billing.Country == RegistrationService.DEFAULT_COUNTRY)
            {
                Billing.State = Utility.TryGetStr(ddlState.SelectedValue);
                Billing.Zip = Utility.TryGetStr(tbZip.Text.Trim());
                Billing.Phone = Utility.TryGetStr((new Phone(tbPhone1.Text.Trim(), tbPhone2.Text.Trim(), tbPhone3.Text.Trim()).ToString()));
            }
            else
            {
                Billing.State = Utility.TryGetStr(tbStateEx.Text.Trim());
                Billing.Zip = Utility.TryGetStr(tbZipEx.Text.Trim());
                Billing.Phone = Utility.TryGetStr(tbPhoneEx.Text.Trim());
            }
            Billing.Email = Utility.TryGetStr(tbEmail.Text.Trim());
        }

        private void LoadRegistration()
        {
            if (cbUseShippingAsBilling.Checked == true)
            {
                Registration = null;
                RegistrationInfo = null;
                return;
            }
            Registration.FirstName = Utility.TryGetStr(tbShippingFirstName.Text.Trim());
            Registration.LastName = Utility.TryGetStr(tbShippingLastName.Text.Trim());
            Registration.Address1 = Utility.TryGetStr(tbShippingAddress1.Text.Trim());
            Registration.Address2 = Utility.TryGetStr(tbShippingAddress2.Text.Trim());
            Registration.City = Utility.TryGetStr(tbShippingCity.Text.Trim());
            RegistrationInfo.Country = Utility.TryGetStr(ddlShippingCountry.SelectedValue);
            if (RegistrationInfo.Country == RegistrationService.DEFAULT_COUNTRY)
            {
                Registration.State = Utility.TryGetStr(shippingState.SelectedValue);
                Registration.Zip = Utility.TryGetStr(tbShippingZip.Text.Trim());
                Registration.Phone = Utility.TryGetStr((new Phone(tbShippingPhone1.Text.Trim(), tbShippingPhone2.Text.Trim(), tbShippingPhone3.Text.Trim()).ToString()));
            }
            else
            {
                Registration.State = Utility.TryGetStr(tbShippingStateEx.Text.Trim());
                Registration.Zip = Utility.TryGetStr(tbShippingZipEx.Text.Trim());
                Registration.Phone = Utility.TryGetStr(tbShippingPhoneEx.Text.Trim());
            }
            Registration.Email = Utility.TryGetStr(tbShippingEmail.Text.Trim());
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (Billing != null)
            {
                tbCreditCardNumber.Text = Billing.CreditCard;
                tbCreditCardCVV.Text = Billing.CVV;
                if (Billing.ExpMonth != null)
                {
                    ddlExpireMonth.SelectedValue = Billing.ExpMonth.ToString();
                }
                if (Billing.ExpYear != null)
                {
                    ddlExpireYear.SelectedValue = Billing.ExpYear.ToString();
                }

                tbFirstName.Text = Billing.FirstName;
                tbLastName.Text = Billing.LastName;
                tbAddress1.Text = Billing.Address1;
                tbAddress2.Text = Billing.Address2;
                tbCity.Text = Billing.City;
                if (string.IsNullOrEmpty(Billing.Country) || Billing.Country == RegistrationService.DEFAULT_COUNTRY)
                {
                    ddlCountry.SelectedValue = RegistrationService.DEFAULT_COUNTRY;
                    if (!string.IsNullOrEmpty(Billing.State))
                    {
                        ddlState.SelectedValue = Billing.State;
                    }
                    tbZip.Text = Billing.Zip;
                    tbPhone1.Text = Billing.PhoneCnt.Code;
                    tbPhone2.Text = Billing.PhoneCnt.Part1;
                    tbPhone3.Text = Billing.PhoneCnt.Part2;
                }
                else
                {
                    ddlCountry.SelectedValue = Billing.Country;
                    tbPhoneEx.Text = Billing.Phone;
                    tbZipEx.Text = Billing.Zip;
                    tbStateEx.Text = Billing.State;
                }
                tbEmail.Text = Billing.Email;
            }

            if (Registration != null)
            {
                tbShippingFirstName.Text = Registration.FirstName;
                tbShippingLastName.Text = Registration.LastName;
                tbShippingAddress1.Text = Registration.Address1;
                tbShippingAddress2.Text = Registration.Address2;
                tbShippingCity.Text = Registration.City;
                if (RegistrationInfo == null || string.IsNullOrEmpty(RegistrationInfo.Country) || RegistrationInfo.Country == RegistrationService.DEFAULT_COUNTRY)
                {
                    ddlShippingCountry.SelectedValue = RegistrationService.DEFAULT_COUNTRY;
                    if (!string.IsNullOrEmpty(Registration.State))
                    {
                        shippingState.SelectedValue = Registration.State;
                    }
                    tbShippingZip.Text = Registration.Zip;
                    tbShippingPhone1.Text = Registration.PhoneCnt.Code;
                    tbShippingPhone2.Text = Registration.PhoneCnt.Part1;
                    tbShippingPhone3.Text = Registration.PhoneCnt.Part2;
                }
                else
                {
                    ddlShippingCountry.SelectedValue = RegistrationInfo.Country;
                    tbShippingPhoneEx.Text = Registration.Phone;
                    tbShippingZipEx.Text = Registration.Zip;
                    tbShippingStateEx.Text = Registration.State;
                }
                tbShippingEmail.Text = Registration.Email;
            }

            EcigBucksAvailable = 0M;
            EcigBucksAvailableToApply = 0M;
            if (Referer != null)
            {
                EcigBucksAvailable = (new RefererService()).GetAvailableAmountToRedeemInEcigsDollars(Referer.RefererID.Value);
                EcigBucksAvailable = EcigBucksAvailable - ShoppingCart.Current.EcigBucksRedeem;
                EcigBucksAvailableToApply = (ShoppingCart.Current.TotalCost > EcigBucksAvailable ? EcigBucksAvailable : ShoppingCart.Current.TotalCost);
            }
        }
    }
}