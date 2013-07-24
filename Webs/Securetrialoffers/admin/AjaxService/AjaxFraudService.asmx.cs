using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.IO;
using Securetrialoffers.admin.Controls;

namespace Securetrialoffers.admin.AjaxService
{
    /// <summary>
    /// Summary description for AjaxFraudService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class AjaxFraudService : System.Web.Services.WebService
    {
        private TControl LoadControl<TControl>(string path) where TControl : UserControl
        {
            UserControl u = new UserControl();
            return (TControl)u.LoadControl(path);
        }

        [WebMethod]
        public string LoadBillingFraudDescription(long billingID)
        {
            FraudDescription control = LoadControl<FraudDescription>("Admin/Controls/FraudDescription.ascx");
            control.BillingID = billingID;
            control.DataBind();

            StringWriter writer = new StringWriter();
            control.RenderControl(new Html32TextWriter(writer));

            return writer.ToString();
        }
    }
}
