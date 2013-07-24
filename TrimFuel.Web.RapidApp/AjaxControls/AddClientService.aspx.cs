using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.RapidApp.Logic;
using TrimFuel.Business.Utils;
using TrimFuel.Business;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class AddClientService : BaseControlPage
    {
        ServicesManager ser = new ServicesManager();
        public int? TPClientID
        {
            get
            {
                return Utility.TryGetInt(Request["ClientId"]);
            }
        }

        public string ServiceType
        {
            get
            {
                return Request["serviceType"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (TPClientID == null)
                return;
            tbTPClientID.Text = TPClientID.Value.ToString();
            List<BaseServiceItem> services = null;
            tbServiceType.Text = ServiceType;
            switch (ServiceType.ToLower())
            {
                case "fulfillment":
                    var currentFulfillmentServices = new TPClientService().GetClientShippers(TPClientID.Value).Where(u => u.ServiceIsActive).Select(u => u.ShipperID);
                    services = ser.GetAllFulfillmentServices().Where(u => !currentFulfillmentServices.Contains(u.ID)).Select(u => new BaseServiceItem { ID = u.ID, ServiceName = u.ServiceName }).ToList();
                    break;
                case "gateway":
                    services = ser.GetAllGatewayServices().Select(u => new BaseServiceItem { ID = u.ID, ServiceName = u.ServiceName }).ToList();
                    break;
                case "outbound":
                    var currentOutboundServices = new TPClientService().GetClientLeadPartnerList(TPClientID.Value).Where(u => u.ServiceisActive.Value).Select(u => u.LeadPartnerID);
                    services = ser.GetAllOutboundServices().Where(u => !currentOutboundServices.Contains(u.ID)).Select(u => new BaseServiceItem { ID = u.ID, ServiceName = u.ServiceName }).ToList();
                    break;
            }
            dpdServices.DataSource = services;
            dpdServices.DataBind();
        }
    }
}
