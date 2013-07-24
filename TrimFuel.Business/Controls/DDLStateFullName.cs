using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using TrimFuel.Model;

namespace TrimFuel.Business.Controls
{
    public class DDLStateFullName : DropDownList
    {
        protected override void OnDataBinding(EventArgs e)
        {
            int selectedIndex = SelectedIndex;
            Items.Clear();

            IList<USState> states = (new PageService()).GetUSStates();
            foreach (USState state in states)
            {
                Items.Add(new ListItem(state.FullName, state.ShortName));
            }

            SelectedIndex = selectedIndex;

            base.OnDataBinding(e);
        }
    }
}
