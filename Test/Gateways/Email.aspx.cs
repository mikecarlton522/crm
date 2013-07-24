using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Hosting;
using System.IO;

namespace Gateways
{
    public partial class Email : System.Web.UI.Page
    {
        private const string EMAIL_FILE_NAME_TEMPLATE = "~/Emails/email_{0}_{1}_{2}_{3}_{4}_{5}_{6}.html";
        private const string EMAIL_FILE_CONTROL_PATH = "~/EmailFile.ascx";

        protected void Page_Load(object sender, EventArgs e)
        {
            EmailFile emailControl = (EmailFile)LoadControl(EMAIL_FILE_CONTROL_PATH);

            using (StreamWriter str = File.CreateText(HostingEnvironment.MapPath(string.Format(EMAIL_FILE_NAME_TEMPLATE, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond))))
            {
                emailControl.FromName = Request.Params["fromName"];
                emailControl.FromAddress = Request.Params["fromAddress"];
                emailControl.ToName = Request.Params["toName"];
                emailControl.ToAddress = Request.Params["toAddress"];
                emailControl.Subject = Request.Params["subject"];
                emailControl.Body = Request.Params["body"];
                
                emailControl.DataBind();
                emailControl.RenderControl(new Html32TextWriter(str));
            }
        }
    }
}
