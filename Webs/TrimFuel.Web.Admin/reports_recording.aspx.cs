using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin
{
    public partial class reports_recording : PageX
    {
        ReportService reportService = new ReportService();

        #region Paging

        int totalNumberOfRecords = -1;
        protected int TotalNumberOfRecords
        {
            get
            {
                if (totalNumberOfRecords == -1)
                    totalNumberOfRecords = reportService.GetRecordingReportTotalCount(DateFilter1.Date1WithTime, DateFilter1.Date2WithTime);
                return totalNumberOfRecords;
            }
        }

        protected int CountOnPage
        {
            get
            {
                var ddlValue = Utility.TryGetInt(ddlPageRecords.Value) ?? 0;
                if (ddlValue == 0)
                    return int.MaxValue;
                else
                    return ddlValue;
            }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            data.Visible = false;
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            data.Visible = true;
            Paging.CurrentPage = 1;
            var recordingList = reportService.GetRecordingReport(DateFilter1.Date1WithTime, DateFilter1.Date2WithTime, 0, Paging.CountOnPage);
            BindToRepeater(recordingList);
        }

        protected void btnGoToPage_Click(object sender, EventArgs e)
        {
            data.Visible = true;
            var recordingList = reportService.GetRecordingReport(DateFilter1.Date1WithTime, DateFilter1.Date2WithTime, (Paging.CurrentPage - 1) * Paging.CountOnPage, Paging.CountOnPage);
            BindToRepeater(recordingList);
        }

        private void BindToRepeater(List<BillingCallView> recordingList)
        {
            int rowNumber = (Paging.CurrentPage - 1) * Paging.CountOnPage + 1;
            rRecording.DataSource = recordingList.Select(u => new
            {
                BillingID = u.Billing.BillingID,
                FirstName = u.Billing.FirstName,
                LastName = u.Billing.LastName,
                CallDate = u.LastCall.CreateDT.Value.ToShortDateString(),
                NumberOfCalls = u.NumberOfCalls,
                CustomerProduct = u.LastCall.CustomerProduct,
                Phone = u.Billing.Phone,
                CallID = u.LastCall.CallID,
                ExternalCallID = u.LastCall.ExternalCallID,
                RowNumber = rowNumber++
            });
            DataBind();
        }

        public override string HeaderString
        {
            get { return "Call Center Recording Report"; }
        }
    }
}