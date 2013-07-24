using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;
using TrimFuel.Business;
using TrimFuel.Model.Utility;
using MySql.Data.MySqlClient;

namespace TrimFuel.Web.Admin
{
    public partial class reports_chargeback_performance : PageX
    {
        ReportService reportService = new ReportService();

        protected IDictionary<string, object> TotalSales { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
                TheReport.Visible = false;
        }

        protected int TotalChargebackCountByReason { get; private set; }
        public string PrGroup = "";
        public string ReportTab
        {
            get
            {
                if (string.IsNullOrEmpty(hdnSelectedTab.Value))
                {
                    hdnSelectedTab.Value = "1";
                }
                return hdnSelectedTab.Value;
            }
        }

        public DateTime StartDate
        {
            get { return DateFilter1.Date1WithTime; }
        }

        public DateTime EndDate
        {
            get { return DateFilter1.Date2WithTime; }
        }

        //protected void btnCSV_Click(object sender, EventArgs e)
        //{
        //    Response.ClearHeaders();
        //    Response.ClearContent();
        //    Response.Clear();
        //    Response.Buffer = true;
        //    Response.ContentType = "text/csv";
        //    Response.AddHeader("Content-Disposition", string.Format("attachment; filename=ChargebackPerformanceReport({0}_{1}).csv", StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")));

        //    if (ReportTab == "1")
        //        Response.Write(reportService.GetAgrSalesReportByAffiliateWithProjectedCSV(StartDate, EndDate));
        //    else if (ReportTab == "2")
        //        Response.Write(reportService.GetAgrSalesReportByTypeWithProjectedCSV(StartDate, EndDate));
        //    else if (ReportTab == "3")
        //        Response.Write(reportService.GetAgrSalesReportByReasonCSV(StartDate, EndDate, null));
        //    else
        //        Response.Write(reportService.GetAgrSalesReportWithProjectedCSV(StartDate, EndDate));

        //    Response.Flush();
        //    Response.End();
        //}

        protected void btnGo_Click(object sender, EventArgs e)
        {
            TheReport.Visible = true;
            DataBind();
            
        }

        protected void ddlType_DataBound(object sender, EventArgs e)
        {
            if (ddlType.Items.Count == 0)
            {
                ddlType.Items.Insert(0, new ListItem("Chargebacks posted in date range", "3"));
                ddlType.Items.Insert(0, new ListItem("Transactions that occurred in date range", "2"));
                ddlType.Items.Insert(0, new ListItem("Customers who signed up in date range", "1"));
                ddlType.SelectedIndex = 0;
            }
        }
        protected void PrdGroup_OnDataBound(object sender, EventArgs e)
        {
            if (PrdGroup.Items.Count == 0)
            {
                IList<Product> res = null;

                PrdGroup.Items.Add(new ListItem("All", ""));
                try
                {
                    ProductService ps = new ProductService();
                    res = ps.GetProductList();
                    for (int i = 0; i < res.Count; i++)
                    {
                        PrdGroup.Items.Add(new ListItem(res[i].ProductName, res[i].ProductID.ToString()));
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            TotalSales = new Dictionary<string, object>();
            int totalCmpSaleCount = 0;
            int totalCmpSaleChargebackCount = 0;
            int totalCmpSaleProjCount = 0;
            int totalCmpSaleProjChargebackCount = 0;
            int totalSaleCount = 0;
            int totalSaleChargebackCount = 0;
            int totalSaleProjectedCount = 0;
            int totalSaleProjectedChargebackCount = 0;

            if (ddlType.SelectedIndex >= 0)  
            {
                // set report type
                reportService.SetReportType(ddlType.SelectedIndex + 1);

                var salesReportWithProjected = reportService.GetAgrSalesReportWithProjected(StartDate, EndDate, PrdGroup.SelectedValue);
                foreach (var item in salesReportWithProjected)
                {
                    totalCmpSaleCount += item.BaseReportView.SaleCount.Value;
                    totalCmpSaleChargebackCount += item.SaleChargebackCount.Value;
                    totalCmpSaleProjCount += item.ProjectedSaleCount.Value;
                    totalCmpSaleProjChargebackCount += item.ProjectedSaleChargebackCount.Value;
                }
                rSales.DataSource = salesReportWithProjected;

                rSalesByType.DataSource = reportService.GetAgrSalesReportByTypeWithProjected(StartDate, EndDate, PrdGroup.SelectedValue);

                KeyEqualityComparer<SalesAgrReportFullView<SalesAgrByAffReportView>> comparer = new KeyEqualityComparer<SalesAgrReportFullView<SalesAgrByAffReportView>>(
                    item => string.Format("BaseReportView.Affiliate={0}" + "BaseReportView.SubAffiliate={1}" +
                        "BaseReportView.SaleCount={2}" + "BaseReportView.PaymentTypeID={3}" +
                        "BaseReportView.AssertigyMIDID={4}" + "BaseReportView.AssertigyDisplayName={5}" +
                        "BaseReportView.AssertigyMID={6}" + "SaleChargebackCount={7}" +
                        "ProjectedSaleCount={8}" + "ProjectedSaleChargebackCount={9}" +
                        "SaleChargebackPercentage={10}" + "ProjectedChargebackPercentage={11}",
                        item.BaseReportView.Affiliate, item.BaseReportView.SubAffiliate,
                        item.BaseReportView.SaleCount, item.BaseReportView.PaymentTypeID,
                        item.BaseReportView.AssertigyMIDID, item.BaseReportView.AssertigyDisplayName,
                        item.BaseReportView.AssertigyMID, item.SaleChargebackCount,
                        item.ProjectedSaleCount, item.ProjectedSaleChargebackCount,
                        item.SaleChargebackPercentage, item.ProjectedChargebackPercentage));

                var salesByAffiliateWithProjected = reportService.GetAgrSalesReportByAffiliateWithProjected(StartDate, EndDate, PrdGroup.SelectedValue).GroupBy(item => new SalesAgrReportFullView<SalesAgrByAffReportView>()
                {
                    BaseReportView = new SalesAgrByAffReportView()
                    {
                        Affiliate = item.BaseReportView.Affiliate,
                        SubAffiliate = item.BaseReportView.SubAffiliate,
                        PaymentTypeID = item.BaseReportView.PaymentTypeID,
                        AssertigyMIDID = null,
                        AssertigyDisplayName = string.Empty,
                        AssertigyMID = string.Empty,
                        SaleCount = null
                    },
                    SaleChargebackCount = null,
                    ProjectedSaleCount = null,
                    ProjectedSaleChargebackCount = null,
                    SaleChargebackPercentage = null,
                    ProjectedChargebackPercentage = null
                }, comparer).ToList().GroupBy(item => new SalesAgrReportFullView<SalesAgrByAffReportView>()
                {
                    BaseReportView = new SalesAgrByAffReportView()
                    {
                        Affiliate = item.Key.BaseReportView.Affiliate,
                        SubAffiliate = item.Key.BaseReportView.SubAffiliate,
                        PaymentTypeID = null,
                        AssertigyMIDID = null,
                        AssertigyDisplayName = string.Empty,
                        AssertigyMID = string.Empty,
                        SaleCount = null
                    },
                    SaleChargebackCount = null,
                    ProjectedSaleCount = null,
                    ProjectedSaleChargebackCount = null,
                    SaleChargebackPercentage = null,
                    ProjectedChargebackPercentage = null
                }, comparer).ToList().GroupBy(item => new SalesAgrReportFullView<SalesAgrByAffReportView>()
                {
                    BaseReportView = new SalesAgrByAffReportView()
                    {
                        Affiliate = item.Key.BaseReportView.Affiliate,
                        SubAffiliate = string.Empty,
                        PaymentTypeID = null,
                        AssertigyMIDID = null,
                        AssertigyDisplayName = string.Empty,
                        AssertigyMID = string.Empty,
                        SaleCount = null
                    },
                    SaleChargebackCount = null,
                    ProjectedSaleCount = null,
                    ProjectedSaleChargebackCount = null,
                    SaleChargebackPercentage = null,
                    ProjectedChargebackPercentage = null
                }, comparer).ToList();

                foreach (var item in salesByAffiliateWithProjected)
                {
                    foreach (var item1 in item)
                    {
                        foreach (var item2 in item1)
                        {
                            item2.Key.BaseReportView.SaleCount = item2.Sum(i => i.BaseReportView.SaleCount);
                            item2.Key.SaleChargebackCount = item2.Sum(i => i.SaleChargebackCount);
                            item2.Key.SaleChargebackPercentage = (item2.Key.BaseReportView.SaleCount != 0) ? (decimal)item2.Key.SaleChargebackCount.Value / (decimal)item2.Key.BaseReportView.SaleCount.Value : 0;

                            item2.Key.ProjectedSaleCount = item2.Sum(i => i.ProjectedSaleCount);
                            item2.Key.ProjectedSaleChargebackCount = item2.Sum(i => i.ProjectedSaleChargebackCount);
                            item2.Key.ProjectedChargebackPercentage = (item2.Key.ProjectedSaleCount != 0) ? (decimal)item2.Key.ProjectedSaleChargebackCount.Value / (decimal)item2.Key.ProjectedSaleCount.Value : 0;
                        }

                        item1.Key.BaseReportView.SaleCount = item1.Sum(i => i.Key.BaseReportView.SaleCount);
                        item1.Key.SaleChargebackCount = item1.Sum(i => i.Key.SaleChargebackCount);
                        item1.Key.SaleChargebackPercentage = (item1.Key.BaseReportView.SaleCount != 0) ? (decimal)item1.Key.SaleChargebackCount.Value / (decimal)item1.Key.BaseReportView.SaleCount.Value : 0;

                        item1.Key.ProjectedSaleCount = item1.Sum(i => i.Key.ProjectedSaleCount);
                        item1.Key.ProjectedSaleChargebackCount = item1.Sum(i => i.Key.ProjectedSaleChargebackCount);
                        item1.Key.ProjectedChargebackPercentage = (item1.Key.ProjectedSaleCount != 0) ? (decimal)item1.Key.ProjectedSaleChargebackCount.Value / (decimal)item1.Key.ProjectedSaleCount.Value : 0;
                    }

                    item.Key.BaseReportView.SaleCount = item.Sum(i => i.Key.BaseReportView.SaleCount);
                    item.Key.SaleChargebackCount = item.Sum(i => i.Key.SaleChargebackCount);
                    item.Key.SaleChargebackPercentage = (item.Key.BaseReportView.SaleCount != 0) ? (decimal)item.Key.SaleChargebackCount.Value / (decimal)item.Key.BaseReportView.SaleCount.Value : 0;

                    item.Key.ProjectedSaleCount = item.Sum(i => i.Key.ProjectedSaleCount);
                    item.Key.ProjectedSaleChargebackCount = item.Sum(i => i.Key.ProjectedSaleChargebackCount);
                    item.Key.ProjectedChargebackPercentage = (item.Key.ProjectedSaleCount != 0) ? (decimal)item.Key.ProjectedSaleChargebackCount.Value / (decimal)item.Key.ProjectedSaleCount.Value : 0;

                    totalSaleCount += item.Key.BaseReportView.SaleCount.Value;
                    totalSaleChargebackCount += item.Key.SaleChargebackCount.Value;
                    totalSaleProjectedCount += item.Key.ProjectedSaleCount.Value;
                    totalSaleProjectedChargebackCount += item.Key.ProjectedSaleChargebackCount.Value;
                }

                rSalesByAff.DataSource = salesByAffiliateWithProjected;

                #region byReason

                //main table
                var shortReasonList = reportService.GetAgrSalesReportByReasonWithProjected(StartDate, EndDate, null, PrdGroup.SelectedValue); ;

                lblTotalWonCount.Text = shortReasonList.Select(u => u.WonCount).Sum().ToString();
                lblTotalLostCount.Text = shortReasonList.Select(u => u.LostCount).Sum().ToString();
                lblTotalWonLostCount.Text = shortReasonList.Select(u => u.TotalWonLostCount).Sum().ToString();
                rSalesReasons.DataSource = shortReasonList;

                //summary table
                var fullReasonList = reportService.GetAgrSalesReportByReason(StartDate, EndDate, null, PrdGroup.SelectedValue);
                Dictionary<string, int> summaryResonDict = new Dictionary<string, int>();
                foreach (var item in fullReasonList)
                {
                    if (summaryResonDict.ContainsKey(item.BaseReportView.ChargebackStatus))
                    {
                        summaryResonDict[item.BaseReportView.ChargebackStatus] += item.BaseReportView.SaleCount ?? 0;
                    }
                    else
                    {
                        if (item.BaseReportView.SaleCount > 0)
                            summaryResonDict.Add(item.BaseReportView.ChargebackStatus, item.BaseReportView.SaleCount ?? 0);
                    }
                }
                TotalChargebackCountByReason = summaryResonDict.Select(u => u.Value).Sum();

                rReasonSummary.DataSource = summaryResonDict;

                #endregion

            }

            TotalSales["CmpSaleCount"] = totalCmpSaleCount;
            TotalSales["CmpSaleChargebackCount"] = totalCmpSaleChargebackCount;
            TotalSales["CmpProjCount"] = totalCmpSaleProjCount;
            TotalSales["CmpProjChargebackCount"] = totalCmpSaleProjChargebackCount;
            TotalSales["SaleCount"] = totalSaleCount;
            TotalSales["SaleChargebackCount"] = totalSaleChargebackCount;
            TotalSales["ProjectedCount"] = totalSaleProjectedCount;
            TotalSales["ProjectedChargebackCount"] = totalSaleProjectedChargebackCount;

        }

        protected string SaleTypeName(int? saleTypeID)
        {
            if (saleTypeID == SaleTypeEnum.Billing)
            {
                return "Trial";
            }
            if (saleTypeID == SaleTypeEnum.Rebill)
            {
                return "Rebill";
            }
            if (saleTypeID == SaleTypeEnum.Upsell)
            {
                return "Upsell";
            }
            return "Unknown";
        }

        protected string PaymentTypeName(int? paymentTypeID)
        {
            if (paymentTypeID == PaymentTypeEnum.Mastercard)
            {
                return "MC";
            }
            if (paymentTypeID == PaymentTypeEnum.Visa)
            {
                return "VI";
            }
            if (paymentTypeID == PaymentTypeEnum.AmericanExpress)
            {
                return "AE";
            }
            if (paymentTypeID == PaymentTypeEnum.Discover)
            {
                return "DV";
            }
            return "Unknown";
        }

        private void SendReport()
        {
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", string.Format("attachment; filename=ChargebackPerformanceReport({0}_{1}).csv", StartDate.ToString("yyyy-MM-dd"), EndDate.ToString("yyyy-MM-dd")));

            if (ReportTab == "1")
                Response.Write(reportService.GetAgrSalesReportByAffiliateWithProjectedCSV(StartDate, EndDate, PrdGroup.SelectedValue));
            else if (ReportTab == "2")
                Response.Write(reportService.GetAgrSalesReportByTypeWithProjectedCSV(StartDate, EndDate, PrdGroup.SelectedValue));
            else if (ReportTab == "3")
                Response.Write(reportService.GetAgrSalesReportByReasonCSV(StartDate, EndDate, null, PrdGroup.SelectedValue));
            else
                Response.Write(reportService.GetAgrSalesReportWithProjectedCSV(StartDate, EndDate, PrdGroup.SelectedValue));

            Response.Flush();
            Response.End();
        }
        public override string HeaderString
        {
            get { return "Chargeback Performance Report"; }
        }
    }
}
