using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Web.RapidApp.Logic;

namespace TrimFuel.Web.RapidApp.AjaxControls
{
    public partial class ClientOutbound : BaseControlPage
    {
        TPClientService service = new TPClientService();

        protected int? TPClientID
        {
            get
            {
                return (Utility.TryGetInt(hdnTPClientID.Value)) ?? Utility.TryGetInt(Request["ClientId"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            var prods = service.GetRoutingRules(TPClientID.Value);
            if (prods != null)
                rProducts.DataSource = prods.GroupBy(i => new KeyValuePair<int, string>(i.LeadRouting.ProductID.Value, i.ProductName));
            else
                rProducts.DataSource = prods;
        }
    }
}
