using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.UI.Specialized
{
    public class TPClientDDL : DropDownList
    {
        DashboardService service = new DashboardService();

        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            Items.Add(new ListItem("-- Select --", string.Empty));
            foreach (TPClient i in service.GetTPClientList())
            {
                Items.Add(new ListItem(i.Name, i.TPClientID.ToString()));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }
    }
}
