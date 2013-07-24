using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls.SubscriptionControl
{
    public partial class Subscription : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public RecurringPlanView RecurringPlan { get; set; }
        public int? SelectedRecurringPlanID 
        {
            get 
            {
                return Utility.TryGetInt(hdnRecurringPlanID.Text);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (RecurringPlan != null)
            {
                SubscriptionProductGroup1.SelectedProductID = RecurringPlan.ProductID;

                string selectedGroupProductSKU = string.Empty;
                foreach (var item in RecurringPlan.CycleList.Last().ShipmentList.OrderBy(i => i.ProductSKU))
                {
                    selectedGroupProductSKU += "_" + item.Quantity.ToString() + "x" + item.ProductSKU;
                }
                selectedGroupProductSKU = selectedGroupProductSKU.TrimStart('_');
                SubscriptionProduct1.SelectedGroupProductSKU = selectedGroupProductSKU;
                SubscriptionProduct1.ProductID = RecurringPlan.ProductID;

                SubscriptionPlanFrequency1.SelectedPlanFrequency = RecurringPlan.CycleList.Last().Cycle.Interim;
                SubscriptionPlanFrequency1.ProductID = RecurringPlan.ProductID;
                SubscriptionPlanFrequency1.GroupProductSKU = selectedGroupProductSKU;

                SubscriptionList1.SelectedRecurringPlanID = RecurringPlan.RecurringPlanID;
                SubscriptionList1.ProductID = RecurringPlan.ProductID;
                SubscriptionList1.GroupProductSKU = selectedGroupProductSKU;
                SubscriptionList1.PlanFrequency = RecurringPlan.CycleList.Last().Cycle.Interim;
            }
        }
    }
}