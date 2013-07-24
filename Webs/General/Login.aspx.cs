using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace General
{
    public partial class Login : System.Web.UI.Page
    {
        private const string USERNAME = "rob";
        private const string PASSWORD = "eugene";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void bLogin_Click(object sender, EventArgs e)
        {
            if (tbUsername.Text == USERNAME && tbPassword.Text == PASSWORD)
            {
                string returnUrl = Request.QueryString["ReturnUrl"];
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    FormsAuthentication.RedirectFromLoginPage("rob", false);
                }
                else
                {
                    FormsAuthentication.SetAuthCookie("rob", false);
                    Response.Redirect("~/admin/apps/report.aspx?type=report_refund_auth&caption=Returns%20Report");
                }

            }
            else
            {
                vIncorrectLogin.IsValid = false;
            }
        }
    }
}
