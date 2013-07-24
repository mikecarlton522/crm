using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;
using TrimFuel.Model;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class QueuedTransactions : System.Web.UI.Page
    {
        ReportService reportService = new ReportService();
        Dictionary<int?, List<UnsentUnpayedView>> mids = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            phMessage2.Visible = false;
            phMessage3.Visible = false;

            if (!IsPostBack)
                GenerateTransactionReport();
        }

        private void GenerateTransactionReport()
        {
            var bills = reportService.GetUnpayedBills().Union(reportService.GetUnpayedRebills()).ToList().OrderByDescending(u => u.CreateDT).ToList();

            mids = new Dictionary<int?, List<UnsentUnpayedView>>();
            foreach (var bill in bills)
            {
                if (mids.ContainsKey(bill.GroupID))
                    mids[bill.GroupID].Add(bill);
                else
                {
                    mids.Add(bill.GroupID, new List<UnsentUnpayedView>());
                    mids[bill.GroupID].Add(bill);
                }
            }

            rBills.DataSource = mids;
            DataBind();
        }

        protected void btnResendBills_Click(object sender, EventArgs e)
        {
            SaleService ser = new SaleService();
            var billList = string.IsNullOrEmpty(Request["billToSend"]) ? new string[0] : Request["billToSend"].Split(',');
            foreach (var strQueueID in billList)
            {
                ser.ProcessEmergencyQueue(Utility.TryGetInt(strQueueID), null);
            }
            var rebillList = string.IsNullOrEmpty(Request["rebillToSend"]) ? new string[0] : Request["rebillToSend"].Split(',');
            foreach (var bsID in rebillList)
            {
                ser.ProcessRebill(Utility.TryGetInt(bsID));
            }
            GenerateTransactionReport();
            phMessage2.Visible = true;
        }

        protected string GetMIDNameById(string midID)
        {
            if (midID == "0")
                return string.Format("Unallocated ({0})", mids[0].Count);

            string res = string.Empty;
            var mid = reportService.Load<AssertigyMID>(Utility.TryGetInt(midID));
            if (mid != null)
                res = mid.DisplayName;

            res += string.Format(" ({0})", mids[Utility.TryGetInt(midID)].Count);
            return res;
        }

        protected void btnSendBillsToDifferentMID_Click(object sender, EventArgs e)
        {
            SaleService ser = new SaleService();
            if (Utility.TryGetInt(AssertigyMidDDL.SelectedValue) == null)
                return;

            var rebillList = string.IsNullOrEmpty(Request["rebillToSend"]) ? new string[0] : Request["rebillToSend"].Split(',');
            foreach (var bsID in rebillList)
            {
                ser.ProcessRebill(Utility.TryGetInt(bsID), Utility.TryGetInt(AssertigyMidDDL.SelectedValue));
            }
            var billList = string.IsNullOrEmpty(Request["billToSend"]) ? new string[0] : Request["billToSend"].Split(',');
            foreach (var strQueueID in billList)
            {
                ser.ProcessEmergencyQueue(Utility.TryGetInt(strQueueID), Utility.TryGetInt(AssertigyMidDDL.SelectedValue));
            }
            GenerateTransactionReport();
            phMessage2.Visible = true;
        }
        
        protected void btnRemoveFromList_Click(object sender, EventArgs e)
        {
            SaleService ser = new SaleService();

            var rebillList = string.IsNullOrEmpty(Request["rebillToSend"]) ? new string[0] : Request["rebillToSend"].Split(',');
            foreach (var bsID in rebillList)
            {
                ser.AddRebillToIgnoreUnbilledTransaction(Utility.TryGetInt(bsID));
            }
            var billList = string.IsNullOrEmpty(Request["billToSend"]) ? new string[0] : Request["billToSend"].Split(',');
            foreach (var strQueueID in billList)
            {
                ser.AddQueuedSaleToIgnoreUnbilledTransaction(Utility.TryGetLong(strQueueID));
            }
            GenerateTransactionReport();
            phMessage3.Visible = true;
        }
    }
}