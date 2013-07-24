using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.UI.Specialized
{
    public class AffiliateDDL : DropDownList
    {
        DashboardService service = new DashboardService();

        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            Items.Add(new ListItem("-- Select --", string.Empty));
            foreach (Affiliate i in service.GetAffiliates().Where(i => i.Deleted == false && i.Active == 1).OrderBy(i => i.Code))
            {
                Items.Add(new ListItem(i.Code, i.Code));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }

    }
}
