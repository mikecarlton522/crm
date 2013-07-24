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
    public partial class ClientProductGroups : BaseControlPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        protected int? TPClientID
        {
            get
            {
                return Utility.TryGetInt(Request["ClientId"]);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            rProducts.DataSource = (new TPClientService()).GetProductList(TPClientID.Value);
        }

        protected void rProducts_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "delete")
            {
                (new TPClientService()).DeleteProduct(TPClientID.Value, Convert.ToInt32(e.CommandArgument));
                DataBind();
            }
        }
    }
}
