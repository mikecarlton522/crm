using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;

namespace Securetrialoffers.admin
{
    public partial class referer_management : System.Web.UI.Page
    {
        private RefererService refererService = new RefererService();

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

            rReferers.DataSource = refererService.GetRefererList();
        }
    }
}
