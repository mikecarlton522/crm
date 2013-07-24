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
    public partial class ClientServiceItem : System.Web.UI.UserControl
    {
        TPClientService ser = new TPClientService();

        public string ServiceType { get; set; }
        public string ServiceTypeDisplayName { get; set; }

        public int TPClientID { get; set; }

        public List<BaseServiceItem> Services 
        { 
            get 
            {
                List<BaseServiceItem> services = new List<BaseServiceItem>();
                switch (ServiceType.ToLower())
                {
                    case "fulfillment":
                        {
                            services = ser.GetClientShippers(TPClientID).Where(u => u.ServiceIsActive)
                                .Select(t => new BaseServiceItem() { ID = t.ShipperID, ServiceName = t.Name, DisplayName = t.Name, BaseServiceName = string.Empty }).ToList();
                            break;
                        }
                    case "gateway":
                        {
                            services = ser.GetClientGatewayServices(TPClientID).Where(u => u.Deleted == false)
                                .Select(t => new BaseServiceItem() { ID = t.NMICompanyID.Value, ServiceName = t.CompanyName, DisplayName = GetDisplayNameForGateway(t), BaseServiceName = string.IsNullOrEmpty(t.GatewayIntegrated) ? string.Empty : t.GatewayIntegrated
                                }).ToList();
                            break;
                        }
                    case "outbound":
                        {
                            services = ser.GetClientLeadPartnerList(TPClientID).Where(u => u.ServiceisActive.Value)
                                .Select(t => new BaseServiceItem() { ID = t.LeadPartnerID.Value, ServiceName = t.DisplayName, DisplayName = t.DisplayName, BaseServiceName = string.Empty }).ToList();
                            break;
                        }
                }
                return services;
            } 
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        private string GetDisplayNameForGateway(NMICompany gateway)
        {
            string res = gateway.GatewayIntegrated + " - " + gateway.CompanyName;
            res += gateway.Active.Value ? " (Active)" : " (Inactive)";
            return res;
        }
    }
}