using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class SubscriptionPlanItemControl : ModelDataControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public SubscriptionPlanItem PlanItem { get; set; }
        public SubscriptionPlanItemActionView PlanItemAction { get; set; }

        protected override void SetData()
        {
            If1.Condition = (ViewMode == ViewModeEnum.View || ViewMode == ViewModeEnum.NotEditable);
            If2.Condition = (ViewMode == ViewModeEnum.Edit);

            SubscriptionAction1.PlanItemAction = PlanItemAction;
            SubscriptionAction1.ViewMode = ViewMode;
        }

        protected override void GetData()
        {
            PlanItemAction = SubscriptionAction1.PlanItemAction;
            PlanItem.Interim = Utility.TryGetInt(tbInterim.Text);
        }
    }
}