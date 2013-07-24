using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class SubscriptionActionUpsell : ModelDataControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public SubscriptionPlanItemActionView PlanItemAction { get; set; }

        protected override void SetData()
        {
            If1.Condition = (ViewMode == ViewModeEnum.View || ViewMode == ViewModeEnum.NotEditable);
            If2.Condition = (ViewMode == ViewModeEnum.Edit);
        }

        protected override void GetData()
        {
            if (PlanItemAction != null)
            {
                PlanItemAction.SubscriptionActionAmount = Utility.TryGetDecimal(tbAmount.Text);
                PlanItemAction.SubscriptionActionProductCode = ddlProductCode.SelectedValue;
                PlanItemAction.SubscriptionActionQuantity = Utility.TryGetInt(tbQuantity.Text);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
        }
    }
}