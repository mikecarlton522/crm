using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;

namespace Fitdiet.Store1
{
    public partial class cp_logout : PageX
    {
        private const string LOGOUT_URL = "~/default.aspx";
        protected void Page_Load(object sender, EventArgs e)
        {
            Membership.LogoutReferer();
            Response.Redirect(LOGOUT_URL);
        }
    }
}
