using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.Controls
{
    public partial class ClientsMenuItem : System.Web.UI.UserControl
    {
        public string Class { get; set; }

        public string FolderName { get; set; }

        public Dictionary<int, string> Data { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            DataBind();
        }
    }
}