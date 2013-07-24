using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class DropDownFreeItemOld : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                ExtraTrialShipType = Utility.TryGetInt(ddlFreeItems.SelectedValue);
            }
        }

        public int? ExtraTrialShipType { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            ddlFreeItems.Items.Clear();
            ddlFreeItems.Items.Add(new ListItem("-- Select --", ""));
            foreach (var item in (new DashboardService()).GetActiveFreeItems())
            {
                ddlFreeItems.Items.Add(new ListItem(item.DisplayName, item.ExtraTrialShipTypeID.ToString()));
            }
            ddlFreeItems.SelectedValue = (ExtraTrialShipType != null ? ExtraTrialShipType.ToString() : "");
        }
    }
}