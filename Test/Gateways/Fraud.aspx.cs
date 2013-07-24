using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Web.Hosting;

namespace Gateways
{
    public partial class Fraud : System.Web.UI.Page
    {
        private const string FRAUD_FILE_NAME_TEMPLATE = "~/Frauds/fraud_{0}_{1}_{2}_{3}_{4}_{5}_{6}.html";
        private const string FRAUD_FILE_CONTROL_PATH = "~/FraudFile.ascx";

        protected void Page_Load(object sender, EventArgs e)
        {
            FraudFile fraudControl = (FraudFile)LoadControl(FRAUD_FILE_CONTROL_PATH);

            using (StreamWriter str = File.CreateText(HostingEnvironment.MapPath(string.Format(FRAUD_FILE_NAME_TEMPLATE, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond))))
            {
                fraudControl.Fraud = Request.Url.AbsoluteUri;
                fraudControl.DataBind();
                fraudControl.RenderControl(new Html32TextWriter(str));
            }
        }
    }
}
