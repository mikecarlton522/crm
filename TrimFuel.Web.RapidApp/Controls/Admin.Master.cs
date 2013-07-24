using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.RapidApp.Logic;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.RapidApp.Controls
{
    public partial class Admin : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string loginURL = string.Format("https://{0}/login.asp", Config.Current.APPLICATION_ID);

            if (Request.Cookies["admName4Net"] == null)
                Response.Redirect(loginURL);

            else if (string.IsNullOrEmpty(Request.Cookies["admName4Net"].Value))
                Response.Redirect(loginURL);
        }

        protected string Header
        {
            get
            {
                if (Page is PageX)
                {
                    return ((PageX)Page).HeaderString;
                }
                return null;
            }
        }
    }
}
