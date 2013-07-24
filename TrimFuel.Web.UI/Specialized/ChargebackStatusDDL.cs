using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.UI.Specialized
{
    public class ChargebackStatusDDL : DropDownList
    {
        DashboardService service = new DashboardService();

        protected override void OnDataBinding(EventArgs e)
        {
            string selectedValue = SelectedValue;
            Items.Clear();

            Items.Add(new ListItem("-- Select --", string.Empty));
            foreach (ChargebackStatusType i in service.GetChargebackStatusTypeList())
            {
                Items.Add(new ListItem(i.DisplayName, i.ChargebackStatusTypeID.ToString()));
            }

            try
            {
                SelectedValue = selectedValue;
            }
            catch { }            

            base.OnDataBinding(e);
        }
    }
}
