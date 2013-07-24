using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls.EditForms
{
    public partial class Subscription_ : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.IsPostBack && Subscription == null)
            {
                Subscription = new Subscription();

                Subscription.ProductID = Utility.TryGetInt(ddlProduct.SelectedValue);
                Subscription.ProductName = Utility.TryGetStr(tbProductName.Text);
                Subscription.ProductCode = Utility.TryGetStr(ddlProductCode.SelectedValue);
                Subscription.SKU2 = Utility.TryGetStr(ddlSKU2.SelectedValue);
                Subscription.DisplayName = Utility.TryGetStr(tbDisplayName.Text);
                Subscription.Quantity = Utility.TryGetInt(tbQuantity.Text);

                Subscription.ParentSubscriptionID = Utility.TryGetInt(ddlParentSubscription.SelectedValue);
                Subscription.Selectable = chbSelectable.Checked;
                Subscription.Recurring = chbRecurring.Checked;
                Subscription.ShipFirstRebill = chbShipFirstRebill.Checked;

                Subscription.InitialInterim = Utility.TryGetInt(tbInitialInterim.Text);
                Subscription.InitialShipping = Utility.TryGetDecimal(tbInitialShipping.Text);
                Subscription.InitialBillAmount = Utility.TryGetDecimal(tbInitialBillAmount.Text);
                Subscription.SaveShipping = Utility.TryGetDecimal(tbSaveShipping.Text);
                Subscription.SaveBilling = Utility.TryGetDecimal(tbSaveBilling.Text);

                Subscription.SecondInterim = Utility.TryGetInt(tbSecondInterim.Text);
                Subscription.SecondShipping = Utility.TryGetDecimal(tbSecondShipping.Text);
                Subscription.SecondBillAmount = Utility.TryGetDecimal(tbSecondBillAmount.Text);

                Subscription.RegularInterim = Utility.TryGetInt(tbRegularInterim.Text);
                Subscription.RegularShipping = Utility.TryGetDecimal(tbRegularShipping.Text);
                Subscription.RegularBillAmount = Utility.TryGetDecimal(tbRegularBillAmount.Text);

                Subscription.Description = Utility.TryGetStr(tbDescription.Text);
            }
        }

        public Subscription Subscription { get; set; }
    }
}