using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.UI.Specialized
{
    public class ProductDDL : DropDownList
    {
        SubscriptionService service = new SubscriptionService();

        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            Items.Add(new ListItem("-- Select --", string.Empty));
            foreach (Product i in service.GetProductList())
            {
                Items.Add(new ListItem(i.ProductName, i.ProductID.ToString()));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }
    }
}
