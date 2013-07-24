using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.RapidApp.Logic;
using TrimFuel.Business;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class LeftMenu : BaseControlPage
    {
        Dictionary<int, string> _clients = null;
        Dictionary<int, string> Clients
        {
            get
            {
                if (_clients != null)
                    return _clients;
                _clients = new Dictionary<int, string>();
                var ser = new TPClientService();
                var allClients = ser.GetClientList().OrderBy(u => u.Name);
                foreach (var client in allClients)
                    _clients.Add(client.TPClientID.Value, client.Name);
                _clients.Add(-1, "Triangle");

                return _clients;
            }
        }

        public Dictionary<int, string> Clients1
        {
            get
            {
                return Clients.Where(u => u.Value.ToUpper()[0] <= 'H').ToDictionary(v => v.Key, v => v.Value);
            }
        }

        public Dictionary<int, string> Clients2
        {
            get
            {
                return Clients.Where(u => (u.Value.ToUpper()[0] > 'H' && u.Value.ToUpper()[0] <= 'P')).ToDictionary(v => v.Key, v => v.Value);
            }
        }

        public Dictionary<int, string> Clients3
        {
            get
            {
                return Clients.Where(u => u.Value.ToUpper()[0] > 'Q').ToDictionary(v => v.Key, v => v.Value);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            rServicesTypes.DataSource = new ServicesManager().GetServiceList().OrderBy(u => u.Name);
            DataBind();
        }
    }
}
