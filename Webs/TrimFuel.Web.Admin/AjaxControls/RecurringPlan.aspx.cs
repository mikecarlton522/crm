using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class RecurringPlan_ : System.Web.UI.Page
    {
        SubscriptionNewService service = new SubscriptionNewService();

        protected void Page_Load(object sender, EventArgs e)
        {
            phError.Visible = false;
            phSuccess.Visible = false;
            phSuccess.Visible = false;

            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public int? SelectedRecurringPlanID 
        {
            get { return Utility.TryGetInt(hdnRecurringPlanID.Value) ?? Utility.TryGetInt(Request["recurringPlanID"]); }
            set { hdnRecurringPlanID.Value = value.ToString(); }
        }

        public int? ProposedProductID
        {
            get { return Utility.TryGetInt(Request["proposedProductID"]); }
        }

        private RecurringPlan recurringPlan = null;
        public RecurringPlan RecurringPlan 
        {
            get 
            {
                if (recurringPlan == null)
                {
                    if (SelectedRecurringPlanID != null)
                    {
                        recurringPlan = service.Load<RecurringPlan>(SelectedRecurringPlanID);
                    }
                    else
                    {
                        recurringPlan = new RecurringPlan()
                        {
                            ProductID = ProposedProductID                            
                        };
                    }                    
                }
                return recurringPlan;
            }
            set
            {
                recurringPlan = value;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            RecurringPlan res = service.InsertOrUpdatePlan(SelectedRecurringPlanID, Utility.TryGetInt(ProductDDL1.SelectedValue), Utility.TryGetStr(tbName.Text));
            if (res != null)
            {
                RecurringPlan = res;
                SelectedRecurringPlanID = res.RecurringPlanID;
                phSuccess.Visible = true;
            }
            else
            {
                RecurringPlan.ProductID = Utility.TryGetInt(ProductDDL1.SelectedValue);
                RecurringPlan.Name = tbName.Text;
                phError.Visible = true;
            }
            DataBind();
        }
    }
}
