using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.UI.Specialized
{
    public class ProductCodeDDL : DropDownList
    {
        SubscriptionService service = new SubscriptionService();

        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            Items.Add(new ListItem("-- Select --", string.Empty));
            foreach (ProductCode i in service.GetProductCodeList())
            {
                Items.Add(new ListItem(i.ProductCode_ + ((!string.IsNullOrEmpty(i.Name)) ? string.Format(" ({0})", i.Name) : string.Empty), i.ProductCode_.ToString()));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }
    }
}
