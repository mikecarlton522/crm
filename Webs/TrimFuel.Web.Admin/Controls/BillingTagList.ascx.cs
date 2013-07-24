using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class BillingTagList : System.Web.UI.UserControl
    {
        private TagService tagService = new TagService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rTag.DataSource = tagService.GetTagListByBilling(BillingID);
        }

        public long BillingID { get; set; }
    }
}