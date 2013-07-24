using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.UI.Specialized
{
    public class SubscriptionDDL : DropDownList
    {
        SubscriptionService service = new SubscriptionService();

        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            Items.Add(new ListItem("-- Select --", string.Empty));
            foreach (Subscription i in service.GetSubscriptionList())
            {
                Items.Add(new ListItem(string.Format("#{0} {1} {2}/{3}", i.SubscriptionID, i.DisplayName, i.ProductCode, i.SKU2), i.SubscriptionID.ToString()));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }
    }
}
