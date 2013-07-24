using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin
{
    public partial class lead_routing : PageX
    {


        LeadService service = new LeadService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public override string HeaderString
        {
            get { return "Lead Routing"; }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            rProducts.DataSource = service.GetRoutingRules().GroupBy(i => new KeyValuePair<int, string>(i.LeadRouting.ProductID.Value, i.ProductName));
        }
    }
}
