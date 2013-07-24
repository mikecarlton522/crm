using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class SaleReturnAction : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GenerateID = "SubscriptionControl_" + Utility.RandomString(new Random(), 5);
        }

        public string GenerateID
        {
            get;
            private set;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public long? SaleID 
        {
            get { return Utility.TryGetLong(Request["saleId"]); }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            ifSaleExists.Condition = false;
            ifSaleDoesntExist.Condition = true;

            if (SaleID != null)
            {
                ifSaleExists.Condition = true;
                ifSaleDoesntExist.Condition = false;

                SaleReturnProcessing action = (new ReturnSaleService()).GetReturnActionBySaleID(SaleID.Value);
                if (action != null && action.ReturnProcessingActionID != null)
                {
                    chbEnableAction.Checked = true;
                    ddlAction.SelectedValue = (action.ReturnProcessingActionID != null ? action.ReturnProcessingActionID.ToString() : "");
                    tbRefundAmount.Text = (action.RefundAmount != null ? action.RefundAmount.Value.ToString("0.00") : "");
                    ddlFreeItem.ExtraTrialShipType = action.ExtraTrialShipTypeID;
                    ddlUpsellType.UpsellTypeID = action.UpsellTypeID;
                    ddlQuantity.SelectedValue = (action.Quantity != null ? action.Quantity.ToString() : "1");
                    if (action.NewRecurringPlanID != null)
                    {
                        RecurringPlanView rp = (new SubscriptionNewService()).GetPlan(action.NewRecurringPlanID.Value);
                        Subscription1.RecurringPlan = rp;
                    }
                }
                else
                {
                    chbEnableAction.Checked = false;
                }

                //chbEnableAction.Checked = 
            }
        }

        private void DataBindSubscriptionControl(int? recurringPlanID)
        {
            if (recurringPlanID != null)
            {
                RecurringPlanView rp = (new SubscriptionNewService()).GetPlan(recurringPlanID.Value);
                Subscription1.RecurringPlan = rp;
                Subscription1.DataBind();
            }            
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            ReturnSaleService service = new ReturnSaleService();
            var res = service.SetReturnActionForSale(SaleID.Value, 
                chbEnableAction.Checked, Utility.TryGetShort(ddlAction.SelectedValue),
                Utility.TryGetDecimal(tbRefundAmount.Text),
                ddlFreeItem.ExtraTrialShipType, ddlUpsellType.UpsellTypeID, Utility.TryGetInt(ddlQuantity.SelectedValue),
                null, Subscription1.SelectedRecurringPlanID);
            if (res.State == BusinessErrorState.Success)
            {
                Error2.Show();
                DataBind();
            }
            else
            {
                Error1.Show(res.ErrorMessage);
                //needs to be bound every time
                DataBindSubscriptionControl(Subscription1.SelectedRecurringPlanID);
            }            
        }
    }
}