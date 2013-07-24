using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;
using TrimFuel.Business;
using Securetrialoffers.admin.Logic.UI;

namespace Securetrialoffers.admin
{
    public partial class report_refund_auth : ReportBasePage
    {
        protected IList<ReturnsReportView> GetReportData()
        {
            return (new ReportService()).GetReturnsList(StartDate, EndDate);
        }

        public override string ReportName
        {
            get { return "Refund Report"; }
        }
    }
}
