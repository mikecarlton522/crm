using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Model.Containers;
using TrimFuel.Business.Utils;
using Kaboom.Store1.Logic;
using TrimFuel.Business;

namespace Kaboom.Store1.Controls
{
    public partial class BillingInfo : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack)
            {
                LoadBilling();
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
                    billing.CreditCard = null;
                    billing.CVV = null;
                    billing.ExpMonth = null;
                    billing.ExpYear = null;
                }
                if (billing == null)
                {
                    billing = new Billing();
                }
                return billing;
            }
        }

        //<Referer>
        //public Referer Referer { get { return Membership.CurrentReferer; } }
        public Referer Referer { get { return null; } }

        private void LoadBilling()
        {
            Billing.FirstName = Utility.TryGetStr(tbFirstName.Text.Trim());
            Billing.LastName = Utility.TryGetStr(tbLastName.Text.Trim());
            Billing.Address1 = Utility.TryGetStr(tbAddress1.Text.Trim());
            Billing.Address2 = Utility.TryGetStr(tbAddress2.Text.Trim());
            Billing.City = Utility.TryGetStr(tbCity.Text.Trim());
            Billing.State = Utility.TryGetStr(ddlState.SelectedValue);
            Billing.Zip = Utility.TryGetStr(tbZip.Text.Trim());
            Billing.Phone = Utility.TryGetStr((new Phone(tbPhone1.Text.Trim(), tbPhone2.Text.Trim(), tbPhone3.Text.Trim()).ToString()));
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
                if (!string.IsNullOrEmpty(Billing.State))
                {
                    ddlState.SelectedValue = Billing.State;
                }
                tbZip.Text = Billing.Zip;
                tbPhone1.Text = Billing.PhoneCnt.Code;
                tbPhone2.Text = Billing.PhoneCnt.Part1;
                tbPhone3.Text = Billing.PhoneCnt.Part2;
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
        }
    }
}