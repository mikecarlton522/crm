using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Controls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using Fitdiet.Store1.Logic;
using TrimFuel.Model;
using TrimFuel.Model.Containers;

namespace FitDiet.Store1.Controls
{
    public partial class ShippingDetails : System.Web.UI.UserControl, IStepControl
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                LoadBilling();
            }
        }

        public Referer Referer { get { return Membership.CurrentReferer; } }

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
            }
        }
    }
}