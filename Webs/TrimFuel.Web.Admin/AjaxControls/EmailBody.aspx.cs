using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class EmailBody : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var email = new EmailService().Load<Email>(Request["emailID"]);
            if (email != null)
                Response.Write(email.Body);
            Response.End();
        }
    }
}