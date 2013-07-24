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
    public class RecurringPlanStatusDDL : DropDownList
    {
        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            foreach (var i in RecurringStatusEnum.Name)
            {
                if (i.Key != RecurringStatusEnum.New)
                {
                    Items.Add(new ListItem(i.Value, i.Key.ToString()));
                }
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }
    }
}

