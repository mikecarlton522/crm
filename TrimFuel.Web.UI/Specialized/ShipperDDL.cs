using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;

namespace TrimFuel.Web.UI.Specialized
{
    public class ShipperDDL : DropDownList
    {
        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            var shippers = new ProductService().GetShippers().Where(u => u.ServiceIsActive == true);

            Items.Add(new ListItem("-- Select --", string.Empty));
            foreach (var shipper in shippers)
            {
                Items.Add(new ListItem(shipper.Name, shipper.ShipperID.ToString()));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }

    }
}
