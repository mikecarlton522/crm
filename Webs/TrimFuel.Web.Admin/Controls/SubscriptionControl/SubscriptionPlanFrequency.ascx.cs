using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin.Controls.SubscriptionControl
{
    public partial class SubscriptionPlanFrequency : System.Web.UI.UserControl
    {
        private SubscriptionNewService service = new SubscriptionNewService();

        public int? ProductID { get; set; }
        public string GroupProductSKU { get; set; }
        public int? SelectedPlanFrequency { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            if (ProductID != null && !string.IsNullOrEmpty(GroupProductSKU))
            {
                rProducts.DataSource = service.GetPlanFrequency(ProductID.Value, GroupProductSKU);
            }
        }
    }
}