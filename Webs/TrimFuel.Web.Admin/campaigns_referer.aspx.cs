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
    public partial class campaigns_referer : PageX
    {
        private RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rReferers.DataSource = refererService.GetRefererList();
        }

        public override string HeaderString
        {
            get { return "Referer Management"; }
        }
    }
}
