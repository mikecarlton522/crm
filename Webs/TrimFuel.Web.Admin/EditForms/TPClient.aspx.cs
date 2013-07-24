using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.EditForms
{
    public partial class TPClient_ : System.Web.UI.Page
    {
        private TPClientService tpClientService = new TPClientService();

        protected void Page_Load(object sender, EventArgs e)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(Request["id"]) && int.TryParse(Request["id"], out id))
            {
                TPClient = tpClientService.GetClient(id);

                if (!string.IsNullOrEmpty(Request["action"]) && TPClient != null)
                {
                    Save();
                }
            }
        }

        public TPClient TPClient { get; set; }

        private void Save()
        {
            TPClient.TriangleFulfillment = string.IsNullOrEmpty(Utility.TryGetStr(Request["triangleFulfillment"])) ? false : true;
            TPClient.TriangleCRM = string.IsNullOrEmpty(Utility.TryGetStr(Request["triangleCRM"])) ? false : true;
            TPClient.TelephonyServices = string.IsNullOrEmpty(Utility.TryGetStr(Request["telephonyServices"])) ? false : true;
            TPClient.CallCenterServices = string.IsNullOrEmpty(Utility.TryGetStr(Request["callCenterServices"])) ? false : true;
            TPClient.TechnologyServices = string.IsNullOrEmpty(Utility.TryGetStr(Request["technologyServices"])) ? false : true;
            TPClient.MediaServices = string.IsNullOrEmpty(Utility.TryGetStr(Request["mediaServices"])) ? false : true;
            tpClientService.SaveClient(TPClient);
        }
    }
}
