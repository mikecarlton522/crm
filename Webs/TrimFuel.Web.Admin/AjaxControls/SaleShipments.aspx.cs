using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class SaleShipments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public long? SaleID 
        {
            get { return Utility.TryGetLong(Request["saleID"]); }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            SaleShipments1.SaleID = SaleID;
        }
    }
}