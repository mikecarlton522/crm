using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin.Controls.SubscriptionControl
{
    public partial class NewSubscriptionControl : System.Web.UI.UserControl
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GenerateID = "SubscriptionControl_" + Utility.RandomString(new Random(), 5);
        }

        public RecurringPlanView RecurringPlan { get; set; }
        public int? SelectedRecurringPlanID 
        {
            get 
            {
                return Subscription1.SelectedRecurringPlanID;
            }
        }
        public string TrialSKU 
        { 
            get
            {
                return ProductSKUDDL1.SelectedValue;
            }
        }
        public decimal? TrialPrice
        {
            get
            {
                return Utility.TryGetDecimal(tbPrice.Text);
            }
        }
        public int? TrialQty
        {
            get
            {
                return Utility.TryGetInt(tbQty.Text);
            }
        }
        public int? TrialPeriod
        {
            get
            {
                return Utility.TryGetInt(tbTrialInterim.Text);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            Subscription1.RecurringPlan = RecurringPlan;

        }

        public string GenerateID
        {
            get;
            private set;
        }
    }
}