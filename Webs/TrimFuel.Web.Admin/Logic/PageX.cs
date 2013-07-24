using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace TrimFuel.Web.Admin.Logic
{
    public abstract class PageX : Page
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public abstract string HeaderString { get; }

        public static bool IsAjaxRequest()
        {
            if (HttpContext.Current == null || HttpContext.Current.Request == null)
                return false;
            return ((HttpContext.Current.Request["X-Requested-With"] == "XMLHttpRequest") ||
                ((HttpContext.Current.Request.Headers != null) && (HttpContext.Current.Request.Headers["X-Requested-With"] == "XMLHttpRequest")));
        }
    }
}
