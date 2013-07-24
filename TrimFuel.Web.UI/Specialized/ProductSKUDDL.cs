using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.UI.Specialized
{
    public class ProductSKUDDL : DropDownList
    {
        SubscriptionNewService service = new SubscriptionNewService();

        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            Items.Add(new ListItem("-- Select --", string.Empty));
            foreach (ProductSKU i in service.GetProductList())
            {
                Items.Add(new ListItem(i.ProductSKU_, i.ProductSKU_));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }
    }
}
