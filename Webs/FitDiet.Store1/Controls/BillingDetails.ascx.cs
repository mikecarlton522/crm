using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Controls;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using Fitdiet.Store1.Logic;
using TrimFuel.Model.Containers;

namespace FitDiet.Store1.Controls
{
    public partial class BillingDetails : System.Web.UI.UserControl, IStepControl
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
            LoadBilling();
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
            Billing.PaymentTypeID = Utility.TryGetInt(ddlPaymentType.SelectedValue);
            if (CreditCard.ValidateDecryptedFormat(tbCreditCardNumber.Text.Trim()))
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