using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class RecurringPlanDelete : System.Web.UI.Page
    {
        SubscriptionNewService service = new SubscriptionNewService();

        protected void Page_Load(object sender, EventArgs e)
        {
            phError.Visible = false;
            phSuccess.Visible = false;

            DeleteSubscription();
            DataBind();
        }

        public string ErrorMessage { get; set; }

        private void DeleteSubscription()
        {
            int? recurringPlanID = Utility.TryGetInt(Request["recurringPlanID"]);
            if (recurringPlanID == null)
            {
                ErrorMessage = "Plan is not specified";
                phError.Visible = true;
            }
            else
            {
                var res = service.DeleteRecurringPlan(recurringPlanID.Value);
                if (res.State == BusinessErrorState.Error)
                {
                    ErrorMessage = res.ErrorMessage;
                    phError.Visible = true;
                }
                else
                {
                    phSuccess.Visible = true;
                }
            }
        }
    }
}