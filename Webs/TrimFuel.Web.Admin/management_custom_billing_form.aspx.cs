using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Business;
using TrimFuel.Business.Flow;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin
{
    public partial class management_custom_billing_form : PageX
    {
        private SubscriptionNewService service = new SubscriptionNewService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override string HeaderString
        {
            get { return "Billing Form"; }
        }

        public string ErrorMessage 
        {
            get { return tbError.Text; }
            set { tbError.Text = value; }
        }

        private const int CAMPAIGN_ID = 1020;

        protected void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            if (NewSubscriptionControl1.SelectedRecurringPlanID == null)
            {
                ErrorMessage = "Please choose subscription to continue.";
                return;
            }

            //check selected subscription
            if (NewSubscriptionControl1.SelectedRecurringPlanID != null)
            {
                if (string.IsNullOrEmpty(NewSubscriptionControl1.TrialSKU) || NewSubscriptionControl1.TrialPeriod == null)
                {
                    ErrorMessage = "Please choose Trial";
                    return;
                }
            }

            //Response.Write("<h1>" + NewSubscriptionControl1.SelectedRecurringPlanID.ToString() + "</h1>");
            NewSubscriptionControl1.RecurringPlan = service.GetPlan(NewSubscriptionControl1.SelectedRecurringPlanID.Value);
            NewSubscriptionControl1.DataBind();

            UserBuilder user = new UserBuilder();
            var userRes = user.Create(CAMPAIGN_ID, ddlAffiliate.SelectedValue, tbSubAffiliate.Text, Request.UserHostAddress, Request.Url.Host)
                .SetShippingAddress(
                    tbFirstName.Text, tbLastName.Text, tbAddress1.Text, tbAddress2.Text, tbCity.Text,
                    (ddlCountry.SelectedValue == "US" ? ddlState.SelectedValue : tbStateEx.Text),
                    (ddlCountry.SelectedValue == "US" ? tbZip.Text : tbZipEx.Text),
                    ddlCountry.SelectedValue, tbEmail.Text,
                    (ddlCountry.SelectedValue == "US" ? tbPhone1.Text + tbPhone2.Text + tbPhone3.Text : tbPhoneEx.Text))
                .SetBillingAddress(
                    tbFirstNameB.Text, tbLastNameB.Text, tbAddress1B.Text, tbAddress2B.Text, tbCityB.Text,
                    (ddlCountryB.SelectedValue == "US" ? ddlStateB.SelectedValue : tbStateExB.Text),
                    (ddlCountryB.SelectedValue == "US" ? tbZipB.Text : tbZipExB.Text),
                    ddlCountryB.SelectedValue, tbEmailB.Text,
                    (ddlCountryB.SelectedValue == "US" ? tbPhone1B.Text + tbPhone2B.Text + tbPhone3B.Text : tbPhoneExB.Text))
                .SetCreditCard(tbCreditCardNumber.Text, tbCreditCardCVV.Text, Utility.TryGetInt(tbExpMonth.Text), Utility.TryGetInt(tbExpYear.Text))
                .Save();

            if (userRes.State != BusinessErrorState.Success)
            {
                ErrorMessage = userRes.ErrorMessage;
                return;
            }

            OrderBuilder order = new OrderBuilder();
            var orderRes = order.Create(user.User.Billing.BillingID, CAMPAIGN_ID, AdminMembership.CurrentAdmin.DisplayName,
                Request.UserHostAddress, Request.Url.Host, ddlAffiliate.SelectedValue, tbSubAffiliate.Text,
                NewSubscriptionControl1.RecurringPlan.ProductID)
                .AppendSale(null, NewSubscriptionControl1.TrialPrice.Value, NewSubscriptionControl1.TrialQty.Value,
                    new OrderBuilder.Product[] { new OrderBuilder.Product() { ProductSKU = new Model.ProductSKU() { ProductSKU_ = NewSubscriptionControl1.TrialSKU }, Quantity = NewSubscriptionControl1.TrialQty.Value } },
                    new OrderBuilder.Plan() { RecurringPlanID = NewSubscriptionControl1.SelectedRecurringPlanID.Value, TrialInterim = NewSubscriptionControl1.TrialPeriod.Value })
                .Save();
            if (orderRes.State != BusinessErrorState.Success)
            {
                ErrorMessage = orderRes.ErrorMessage;
                return;
            }

            OrderFlow orderFlow = new OrderFlow();
            var paymentRes = orderFlow.ProcessOrder(order.Order);
            if (paymentRes.State != BusinessErrorState.Success)
            {
                ErrorMessage = paymentRes.ErrorMessage;
                return;
            }

            ErrorMessage = "Thank you. A new order has been placed. Billing ID: <a href='../billing_edit.asp?id=" + user.User.Billing.BillingID + "'>" + user.User.Billing.BillingID + "</a>";
            tbFirstName.Text = tbLastName.Text = tbAddress1.Text = tbAddress2.Text = tbCity.Text = tbStateEx.Text = tbZip.Text = tbZipEx.Text = tbPhone1.Text = tbPhone2.Text = tbPhone3.Text = tbPhoneEx.Text = tbEmail.Text = "";
            ddlCountry.SelectedValue = "US"; ddlState.SelectedValue = "AL";
            tbFirstNameB.Text = tbLastNameB.Text = tbAddress1B.Text = tbAddress2B.Text = tbCityB.Text = tbStateExB.Text = tbZipB.Text = tbZipExB.Text = tbPhone1B.Text = tbPhone2B.Text = tbPhone3B.Text = tbPhoneExB.Text = tbEmailB.Text = "";
            ddlCountryB.SelectedValue = "US"; ddlStateB .SelectedValue = "AL";
            ddlAffiliate.SelectedValue = tbSubAffiliate.Text = "";
            tbCreditCardNumber.Text = tbCreditCardCVV.Text = tbExpMonth.Text = tbExpYear.Text = "";
            NewSubscriptionControl1.RecurringPlan = null;
            NewSubscriptionControl1.DataBind();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            phError.Visible = ErrorMessage.Length > 0;
        }
    }
}