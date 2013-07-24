using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace TrimFuel.Web.RapidApp.Logic
{
    public class BaseControlPage : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if ((AdminMembership.CurrentAdmin == null) || (AdminMembership.CurrentAdminRole.ToUpper() != "ADMIN"))
                Response.Redirect("Empty.aspx");
        }
    }
}
