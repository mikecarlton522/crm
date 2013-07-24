using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.UI.Specialized
{
    public class CurrencyDDL : DropDownList
    {
        protected override void OnDataBinding(EventArgs e)
        {
            var service = new ProductService();

            int selectedIndex = SelectedIndex;
            Items.Clear();

            Items.Add(new ListItem("-- Select --", string.Empty));
            Items.Add(new ListItem("USD", "0"));
            foreach (var i in service.GetCurrencyList().OrderBy(i => i.CurrencyName))
            {
                Items.Add(new ListItem(i.CurrencyName, i.CurrencyID.ToString()));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }

    }
}
