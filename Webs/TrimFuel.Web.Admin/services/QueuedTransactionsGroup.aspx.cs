using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.services
{
    public partial class QueuedTransactionsGroup : System.Web.UI.Page
    {
        protected int? MidID
        {
            get
            {
                return Utility.TryGetInt(Request["midID"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (MidID == null)
                return;

            DataBind();
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            var reportService = new ReportService();
            rBills.DataSource = reportService.GetUnpayedBills().Union(reportService.GetUnpayedRebills()).Where(u => u.GroupID == MidID);
        }
    }
}