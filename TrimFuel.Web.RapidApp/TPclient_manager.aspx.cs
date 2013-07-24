using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.RapidApp.Logic;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Enums;

namespace TrimFuel.Web.RapidApp
{
    public partial class TPclient_manager : PageX
    {
        DashboardService dashboardService = new DashboardService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if ((AdminMembership.CurrentAdmin == null) || (AdminMembership.CurrentAdminRole.ToUpper() != "ADMIN"))
                Response.Redirect("/login.asp");
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
        }

        public override string HeaderString
        {
            get { return "Client Configuration"; }
        }
    }
}
