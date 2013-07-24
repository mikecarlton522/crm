using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin
{
    public partial class unsent_unpayed_shippments : PageX
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public override string HeaderString
        {
            get { return "Unsent Shipments and Queued Transactions"; }
        }
    }
}
