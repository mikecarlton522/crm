using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic.BillingAPI;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin
{
    public partial class testing_billing_api : PageX
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        private int? GetSelectedClient()
        {
            int? res = null;
            int temp = 0;
            if (!string.IsNullOrEmpty(ddlClient.SelectedValue) && int.TryParse(ddlClient.SelectedValue, out temp))
            {
                res = temp;
            }
            return res;
        }

        protected void btnCharge_Click(object sender, EventArgs e)
        {
            int? clientID = GetSelectedClient();
            if (clientID == null)
            {
                lChargeResponse.Text = "Please, select Client";
            }
            else
            {
                BillingAPITest t = new BillingAPITest(clientID.Value);
                lChargeResponse.Text = HttpUtility.HtmlEncode(t.Charge(tbAmount.Text, tbShipping.Text,
                    tbFirstName.Text, tbLastName.Text, tbAddress1.Text, tbAddress2.Text, tbCity.Text, tbState.Text, tbZip.Text, tbPhone.Text, tbEmail.Text, tbIP.Text,
                    tbAffiliate.Text, tbSubAffiliate.Text, tbInternalID.Text,
                    ddlPaymentType.SelectedValue, tbCreditCard.Text, tbCVV.Text, ddlExpMonth.SelectedValue, ddlExpYear.SelectedValue)); ;
            }
        }

        protected void btnRefund_Click(object sender, EventArgs e)
        {
            int? clientID = GetSelectedClient();
            if (clientID == null)
            {
                lRefundResponse.Text = "Please, select Client";
            }
            else
            {
                BillingAPITest t = new BillingAPITest(clientID.Value);
                lRefundResponse.Text = HttpUtility.HtmlEncode(t.Refund(tbRefundChargeHistoryID.Text, tbRefundAmount.Text));
            }
        }

        protected void btnVoid_Click(object sender, EventArgs e)
        {
            int? clientID = GetSelectedClient();
            if (clientID == null)
            {
                lVoidResponse.Text = "Please, select Client";
            }
            else
            {
                BillingAPITest t = new BillingAPITest(clientID.Value);
                lVoidResponse.Text = HttpUtility.HtmlEncode(t.Void(tbVoidChargeHistoryID.Text));
            }
        }

        public override string HeaderString
        {
            get { return "Billing API Test"; }
        }

        protected void btnSSPO_click(object sender, EventArgs e)
        {
            GeneralShipperService generalShipperService = new GeneralShipperService();
            generalShipperService.SendPendingOrders();
        }
    }
}
