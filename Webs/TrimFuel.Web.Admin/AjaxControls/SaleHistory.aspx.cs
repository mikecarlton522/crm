using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class SaleHistory : System.Web.UI.Page
    {
        private OrderService orderService = new OrderService();

        protected void Page_Load(object sender, EventArgs e)
        {
            BillingID = Utility.TryGetLong(Request["billingID"]);
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public long? BillingID { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (BillingID != null)
            {
                SaleHistory1.Orders = orderService.LoadOrders(BillingID);
                SaleHistory1.Charges = orderService.GetChargeHistory(BillingID.Value);
            }
        }
    }
}