using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;
using TrimFuel.Model;
using TrimFuel.Model.Containers;
using TrimFuel.Business.Utils;
using TrimFuel.Business;

namespace Fitdiet.Store1.Controls
{
    public partial class BillingInfo : UserControl
    {
        public event EventHandler<GiftCertificateEventArgs> GiftCertificateAdded;
        protected void OnGiftCertificateAdded(string giftCertificateNumber)
        {
            if (GiftCertificateAdded != null)
            {
                GiftCertificateAdded(this, new GiftCertificateEventArgs(giftCertificateNumber));
            }
        }

        public event EventHandler<EcigBucksEventArgs> EcigBucksApplied;
        protected void OnEcigBucksApplied(decimal ecigBucksAmount)
        {
            if (EcigBucksApplied != null)
            {
                EcigBucksApplied(this, new EcigBucksEventArgs(ecigBucksAmount));
            }
        }

        public void PopulateGiftCertificate(string giftNumber)
        {
            tbGiftCertificateNumber.Text = giftNumber;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            HideGiftCertificateError();
            HideEcigBucksError();

            if (Page.IsPostBack)
            {
                LoadBilling();
            }
        }

        private void HideGiftCertificateError()
        {
            phGiftCertificateError.Visible = false;
        }

        private void ShowGiftCertificateError()
        {
            phGiftCertificateError.Visible = true;
        }

        private void HideEcigBucksError()
        {
            phEcigBucksError.Visible = false;
        }

        private void ShowEcigBucksError()
        {
            phEcigBucksError.Visible = true;
        }

        protected void btnApplyGiftCertificate_Click(object sender, EventArgs e)
        {
            PromoGift g = (new SaleService()).GetGiftCertificateByNumber(tbGiftCertificateNumber.Text.Trim());
            if (g != null && g.RemainingValue != null && g.RemainingValue.Value > 0M)
            {
                OnGiftCertificateAdded(g.GiftNumber);
            }
            else
            {
                ShowGiftCertificateError();
            }
        }

        protected void btnApplyEcigBucks_Click(object sender, EventArgs e)
        {
            decimal? amount = Utility.TryGetDecimal(tbEcigBucksAmount.Text);
            if (amount != null && amount.Value > 0M)
            {
                EcigBucksAvailable = 0M;
                EcigBucksAvailableToApply = 0M;
                if (Referer != null)
                {
                    EcigBucksAvailable = (new RefererService()).GetAvailableAmountToRedeemInEcigsDollars(Referer.RefererID.Value);
                    EcigBucksAvailable = EcigBucksAvailable - ShoppingCart.Current.EcigBucksRedeem;
                    EcigBucksAvailableToApply = (ShoppingCart.Current.TotalCost > EcigBucksAvailable ? EcigBucksAvailable : ShoppingCart.Current.TotalCost);
                }
                if (EcigBucksAvailableToApply > 0M)
                {
                    OnEcigBucksApplied(amount.Value > EcigBucksAvailableToApply ? EcigBucksAvailableToApply : amount.Value);
                }
            }
            else
            {
                ShowEcigBucksError();
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
        }

        public Referer Referer { get { return Membership.CurrentReferer; } }
        public decimal EcigBucksAvailable { get; set; }
        public decimal EcigBucksAvailableToApply { get; set; }

        private void LoadBilling()
        {
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
                Billing.State = Utility.TryGetStr(tbStateEx.Text);
                Billing.Zip = Utility.TryGetStr(tbZipEx.Text);
                Billing.Phone = Utility.TryGetStr(tbPhoneEx.Text);
            }
            Billing.Email = Utility.TryGetStr(tbEmail.Text.Trim());

            Billing.PaymentTypeID = Utility.TryGetInt(ddlPaymentType.SelectedValue);
            Billing.CreditCard = Utility.TryGetStr(tbCreditCardNumber.Text.Trim());
            Billing.CVV = Utility.TryGetStr(tbCreditCardCVV.Text.Trim());
            Billing.ExpMonth = Utility.TryGetInt(ddlExpireMonth.SelectedValue);
            Billing.ExpYear = Utility.TryGetInt(ddlExpireYear.SelectedValue);
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (Billing != null)
            {
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
                    tbStateEx.Text = Billing.State;
                    tbZipEx.Text = Billing.Zip;
                    tbPhoneEx.Text = Billing.Phone;
                }
                tbEmail.Text = Billing.Email;

                //tbCreditCardNumber.Text = Billing.CreditCardCnt.DecryptedCreditCard;
                //tbCreditCardCVV.Text = Billing.CVV;
                //if (Billing.ExpMonth != null)
                //{
                //    ddlExpireMonth.SelectedValue = Billing.ExpMonth.ToString();
                //}
                //if (Billing.ExpYear != null)
                //{
                //    ddlExpireYear.SelectedValue = Billing.ExpYear.ToString();
                //}
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