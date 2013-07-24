using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.Controls.SubscriptionControl
{
    public partial class SubscriptionProductGroup : System.Web.UI.UserControl
    {
        private ProductService service = new ProductService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public int? SelectedProductID { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rProducts.DataSource = service.GetProductList().Where(i => i.ProductIsActive == true).ToList();
        }
    }
}