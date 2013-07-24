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
    public partial class management_emails : PageX
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public override string HeaderString
        {
            get { return "Email Management"; }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
            rProducts.DataSource = (new SubscriptionService()).GetProductList().Where(i => i.ProductID > 0);
        }
    }
}
