using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.UI.Specialized
{
    public class AssertigyMidDDL : DropDownList
    {
        MerchantService merchantService = new MerchantService();

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            foreach (ListItem item in Items)
            {
                if (item.Value == "0")
                {
                    item.Attributes.Add("disabled", "disabled");
                }
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            Items.Add(new ListItem("-- Select --", string.Empty));
            foreach (var grp in merchantService.GetMIDs())
            {
                ListItem header = new ListItem(grp.Key.DisplayName.ToUpper(), "0");
                header.Attributes.Add("disabled", "disabled");
                Items.Add(header);
                foreach (var mid in grp)
                {
                    Items.Add(new ListItem(mid.MID + " " + mid.DisplayName, mid.AssertigyMIDID.ToString()));
                }
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }
    }
}
