using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class SubscriptionAction : ModelDataControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public ViewModeEnum ViewMode { get; set; }

        public SubscriptionPlanItemActionView PlanItemAction { get; set; }

        protected override void SetData()
        {
            If1.Condition = (PlanItemAction.SubscriptionActionTypeID == TrimFuel.Model.Enums.SubscriptionActionType.Upsell);
            If2.Condition = (PlanItemAction.SubscriptionActionTypeID == TrimFuel.Model.Enums.SubscriptionActionType.FreeProduct);

            SubscriptionActionFreeProduct1.ViewMode = ViewMode;
            SubscriptionActionUpsell1.ViewMode = ViewMode;
            if (PlanItemAction.SubscriptionActionTypeID == TrimFuel.Model.Enums.SubscriptionActionType.FreeProduct)
            {
                SubscriptionActionFreeProduct1.PlanItemAction = PlanItemAction;
            }
            else if (PlanItemAction.SubscriptionActionTypeID == TrimFuel.Model.Enums.SubscriptionActionType.Upsell)
            {
                SubscriptionActionUpsell1.PlanItemAction = PlanItemAction;
            }
        }

        protected override void GetData()
        {
            if (PlanItemAction.SubscriptionActionTypeID == TrimFuel.Model.Enums.SubscriptionActionType.Upsell)
            {
                PlanItemAction = SubscriptionActionUpsell1.PlanItemAction;
            }
            else if (PlanItemAction.SubscriptionActionTypeID == TrimFuel.Model.Enums.SubscriptionActionType.FreeProduct)
            {
                PlanItemAction = SubscriptionActionFreeProduct1.PlanItemAction;
            }
        }
    }
}