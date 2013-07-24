using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using TrimFuel.Business;
using TrimFuel.Web.Admin.Logic;

namespace TrimFuel.Web.Admin
{
    public class Global : System.Web.HttpApplication
    {
        //TODO: replace with authorixation in web.config
        private const string AUTH_COOKIE_NAME = "admName4Net";

        protected void Application_Start(object sender, EventArgs e)
        {
            //can't use this because HttpContext is not ready
            //log4net.ThreadContext.Properties["ApplicationID"] = Config.Current.APPLICATION_ID;
            
            log4net.Config.XmlConfigurator.Configure();            
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            log4net.ThreadContext.Properties["ApplicationID"] = Config.Current.APPLICATION_ID;
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            if (Request != null &&
                Request.RawUrl != null &&
                (!Request.RawUrl.Contains("logout.aspx")) &&
                (!Request.RawUrl.Contains(".asmx")) &&
                (Request.RawUrl.Contains(".aspx")) &&
                Response != null &&
                PageX.IsAjaxRequest() &&
                !AdminMembership.IsAuthenticated)
            {
                Response.Redirect("~/ajaxControls/logout.aspx", true);
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}