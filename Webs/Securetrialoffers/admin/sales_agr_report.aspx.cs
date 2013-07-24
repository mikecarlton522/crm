using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Model.Enums;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using TrimFuel.Model.Utility;

namespace Securetrialoffers.admin
{
    public partial class sales_agr_report : System.Web.UI.Page
    {
        ReportService reportService = new ReportService();

        protected void Page_Load(object sender, EventArgs e)
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            ReportTab = "0";

            if (!string.IsNullOrEmpty(Request["startDate"]))
            {
                DateTime startDate = DateTime.Now;
                if (DateTime.TryParse(Request["startDate"], out startDate))
                {
                    StartDate = startDate;
                }
            }

            if (!string.IsNullOrEmpty(Request["endDate"]))
            {
                DateTime endDate = DateTime.Now;
                if (DateTime.TryParse(Request["endDate"], out endDate))
                {
                    EndDate = endDate;
                }
            }

            if (!string.IsNullOrEmpty(Request["tab"]))
            {
                ReportTab = Request["tab"];
            }

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.Params["csv"]))
                {
                    SendReport();
                }
                else
                {
                    DataBind();
                }
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            rSales.DataSource = reportService.GetAgrSalesReportWithProjected(StartDate, EndDate);
            rSalesByType.DataSource = reportService.GetAgrSalesReportByTypeWithProjected(StartDate, EndDate);

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

            var salesByAffiliateWithProjected = reportService.GetAgrSalesReportByAffiliateWithProjected(StartDate, EndDate).GroupBy(item => new SalesAgrReportFullView<SalesAgrByAffReportView>()
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
            }

            rSalesByAff.DataSource = salesByAffiliateWithProjected;
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
                Response.Write(reportService.GetAgrSalesReportByAffiliateWithProjectedCSV(StartDate, EndDate));
            else if (ReportTab == "2")
                Response.Write(reportService.GetAgrSalesReportByTypeWithProjectedCSV(StartDate, EndDate));
            else
                Response.Write(reportService.GetAgrSalesReportWithProjectedCSV(StartDate, EndDate)); 

            Response.Flush();
            Response.End();
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ReportTab { get; set; }
    }
}
