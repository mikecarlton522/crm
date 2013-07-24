using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;
using TrimFuel.Business.Flow;
using TrimFuel.Model;
using TrimFuel.Web.Admin.Logic;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class OrderFreeItem : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public long? BillingID
        {
            get
            {
                return Utility.TryGetLong(Request["BillingID"]);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            ifCustomerExists.Condition = false;
            ifCustomerDoesntExist.Condition = true;
            if (BillingID != null)
            {
                ifCustomerExists.Condition = true;
                ifCustomerDoesntExist.Condition = false;

                tbQuantity.Text = "1";
            }
        }

        protected void btnSendShipment_Click(object sender, EventArgs e)
        {
            DashboardService srv = new DashboardService();
            //Check selected ExtraTriapShipType
            if (ddlFreeItem.ExtraTrialShipType == null)
            {
                Error1.Show("Please choose product", Logic.Error.TypeEnum.Error);
                return;
            }

            //Check quantity
            int? quantity = Utility.TryGetInt(tbQuantity.Text);
            if (quantity == null)
            {
                Error1.Show("Please enter a valid Quantity", Logic.Error.TypeEnum.Error);
                return;
            }

            //Check current campaign
            int? campaignID = Logic.AdminMembership.CurrentCampaignID;
            if (campaignID == null)
            {
                Error1.Show("Error: Can't determine Campaign", Logic.Error.TypeEnum.Error);
                return;
            }

            //Check user default ProductID
            var product = new OrderService().GetDefaultProduct(BillingID.Value);
            if (product == null)
            {
                Error1.Show("Error: Can't determine customer Product Group", Logic.Error.TypeEnum.Error);
                return;
            }

            //Check input customer data
            UserBuilder user = new UserBuilder().LoadByBillingID(BillingID);
            if (user.User == null)
            {
                Error1.Show("Error: Can't find Customer", Logic.Error.TypeEnum.Error);
                return;
            }

            //Check order data
            OrderBuilder orderBuilder = new OrderBuilder();
            orderBuilder.Create(user.User.Billing.BillingID, campaignID, Logic.AdminMembership.CurrentAdmin.DisplayName,
                Request.UserHostAddress, Request.Url.AbsoluteUri, user.User.Billing.Affiliate, user.User.Billing.SubAffiliate,
                product.ProductID);
            var extraTrialShipType = new OrderService().Load<ExtraTrialShipType>(ddlFreeItem.ExtraTrialShipType);
            orderBuilder.AppendProductCode(extraTrialShipType.ProductCode, quantity.Value, 0);
            var isValid = orderBuilder.Validate();
            if (isValid.State == BusinessErrorState.Error)
            {
                Error1.Show(isValid.ErrorMessage + "<br/>" + string.Join("<br/>", isValid.ReturnValue.ToArray()), Logic.Error.TypeEnum.Error);
                return;
            }

            //Save Order
            var orderRes = orderBuilder.Save();
            if (orderRes.State == BusinessErrorState.Error)
            {
                Error1.Show("Error: " + orderRes.ErrorMessage, Logic.Error.TypeEnum.Error);
                return;
            }

            //Process Order
            OrderFlow orderFlow = new OrderFlow();
            var res = orderFlow.ProcessOrder(orderRes.ReturnValue);
            if (res.State == BusinessErrorState.Success)
            {
                srv.AddBillingNote(BillingID.Value, AdminMembership.CurrentAdmin.AdminID, string.Format("Free item added: {0} / {1}", product.Code, quantity.Value));
                Error1.Show("Thank you. A Free Product has been added." + (res.ReturnValue != null && res.ReturnValue.Count > 0 && res.ReturnValue.Last().ChargeResult != null ? "<br/>Gateway Response: " + res.ReturnValue.Last().ChargeResult.ChargeHistory.Response : ""), res.State);
            }
            else
            {
                Error1.Show("Error. " + res.ErrorMessage + "<br/>" + (res.ReturnValue != null && res.ReturnValue.Count > 0 && res.ReturnValue.Last().ChargeResult != null ? "<br/>Gateway Response: " + res.ReturnValue.Last().ChargeResult.ChargeHistory.Response : ""), res.State);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (Error1.Visible == false)
                Error1.Show("", BusinessErrorState.Success);
        }

    }
}