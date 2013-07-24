using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;
using System.IO;
using System.Net.Mail;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business.Dao;
using TrimFuel.Model.Containers;

namespace TrimFuel.Business
{
    public class ReportService : BaseService
    {
        // Report type
        // 1 = Show chargebacks for users who signed up under this date range
        // 2 = Show chargebacks for sales that occurred under this date range
        // 3 = Show chargebacks whose post date occurred under this date range
        private int _reportType = 0;
        private String _whereClauseChargebackSales = "where sc.CreateDT between @startDate and @endDate ";

        public void SetReportType(int reportType)
        {
            _reportType = reportType;

            switch (_reportType)
            {
                case 1:
                    _whereClauseChargebackSales = "where b.CreateDT between @startDate and @endDate ";
                    break;
                case 2:
                    _whereClauseChargebackSales = "where s.CreateDT between @startDate and @endDate ";
                    break;
                case 3:
                    _whereClauseChargebackSales = "where sc.PostDT between @startDate and @endDate ";
                    break;
                default:
                    _whereClauseChargebackSales = "where sc.CreateDT between @startDate and @endDate ";
                    break;
            }
        }

        public class SalesAgrByReasonShortReportView
        {
            public SalesAgrByReasonShortReportView(IList<SalesAgrReportFullView<SalesAgrByReasonReportView>> list, int? reasonCodeID)
            {
                WonCount = list.Where(u => u.BaseReportView.ChargebackReasonCodeID == reasonCodeID)
                                   .Where(u => u.BaseReportView.ChargebackStatusTID == 3)
                                   .Select(u => u.BaseReportView.SaleCount).Sum() ?? 0;

                LostCount = list.Where(u => u.BaseReportView.ChargebackReasonCodeID == reasonCodeID)
                                    .Where(u => u.BaseReportView.ChargebackStatusTID == 4)
                                    .Select(u => u.BaseReportView.SaleCount).Sum() ?? 0;

                //PendingCount = list.Where(u => u.BaseReportView.ChargebackReasonCodeID == reasonCodeID)
                //    .Where(u => u.BaseReportView.ChargebackStatusTID == 2)
                //    .Select(u => u.BaseReportView.SaleCount).Sum() ?? 0;

                ReasonCode = list.Where(u => u.BaseReportView.ChargebackReasonCodeID == reasonCodeID)
                                        .Select(u => u.BaseReportView.ChargebackReasonCode).Distinct().FirstOrDefault();

                TotalWonLostCount = WonCount + LostCount;
            }

            public string ReasonCode { get; private set; }
            public int WonCount { get; private set; }
            public int LostCount { get; private set; }
            public int TotalWonLostCount { get; private set; }
        }

        public IList<ReturnsReportView> GetReturnsList(DateTime? startDate, DateTime? endDate)
        {
            IList<ReturnsReportView> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(string.Format(
                    "select b.BillingID, b.FirstName, b.LastName, r.CreateDT as RefundCreateDT, r.Reason as RefundReason, brma.RMA as CallRMA, null as DispositionDisplayName from Billing b " +
                    "inner join RefundAuthorizationList r on r.BillingID = b.BillingID " +
                    "left join BillingRMA brma on brma.BillingID = b.BillingID " +
                    "where r.Complete = 0{0}{1} " +
                    "union " +
                    "select b.BillingID, b.FirstName, b.LastName, r.CreateDT as RefundCreateDT, r.Reason as RefundReason, brma.RMA as CallRMA, null as DispositionDisplayName from Billing b " +
                    "inner join RefundDiscretionaryList r on r.BillingID = b.BillingID " +
                    "left join BillingRMA brma on brma.BillingID = b.BillingID " +
                    "where r.Complete = 0{0}{1} ",
                    (startDate != null) ? " and r.CreateDT >= @startDate" : string.Empty,
                    (endDate != null) ? " and r.CreateDT <= @endDate" : string.Empty));

                if (startDate != null)
                {
                    q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value =
                        new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                }
                if (endDate != null)
                {
                    q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value =
                        new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);
                }

                res = dao.Load<ReturnsReportView>(q);

                res = res.OrderByDescending(item => item.RefundCreateDT.Value).ToList();

                ////TODO: load appropriate list from DB Acai2 and join
                //foreach (ReturnsReportView view in res)
                //{
                //    view.CallRMA = GetBillingRMA(view.BillingID.Value);
                //    view.DispositionDisplayName = GetBillingDisposition(view.BillingID.Value);
                //}
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<SalesAgrReportFullView<SalesAgrReportView>> GetAgrSalesReport(DateTime? startDate, DateTime? endDate, string productCode)
        {
            IList<SalesAgrReportFullView<SalesAgrReportView>> res = null;

            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.Now;
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }
                string ProdGroupFilter = "";
                if (productCode != "")
                    ProdGroupFilter = " and p.ProductID = " + productCode + " ";

                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);

                MySqlCommand q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, nmi.CompanyName as NMICompanyName, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join BillingSale bs on bs.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bs.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                        "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                   "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "inner join NMICompany nmi on nmi.NMICompanyID = nmiMid.NMICompanyID " +
                    "where s.CreateDT between @startDate and @endDate " + ProdGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrReportView> billingSales = dao.Load<SalesAgrReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, nmi.CompanyName as NMICompanyName, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join UpsellSale us on us.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                        "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                     "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "inner join NMICompany nmi on nmi.NMICompanyID = nmiMid.NMICompanyID " +
                    "where s.CreateDT between @startDate and @endDate " + ProdGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrReportView> upsellSales = dao.Load<SalesAgrReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, nmi.CompanyName as NMICompanyName, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from OrderSale s " +
                    "inner join Invoice i on i.InvoiceID = s.InvoiceID " +
                    "inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID and ch.Amount > 0 and ch.Success = 1 " +
                    "inner join Orders o on o.OrderID = s.OrderID " +
                    "inner join Billing b on b.BillingID = o.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                        "inner join Product p on p.ProductID = o.ProductID " +
                   "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "inner join NMICompany nmi on nmi.NMICompanyID = nmiMid.NMICompanyID " +
                    "where s.CreateDT between @startDate and @endDate " + ProdGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrReportView> orderSales = dao.Load<SalesAgrReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, nmi.CompanyName as NMICompanyName, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join SaleChargeback sc on sc.SaleID = s.SaleID " +
                    "inner join BillingSale bs on bs.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bs.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                        "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                   "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "inner join NMICompany nmi on nmi.NMICompanyID = nmiMid.NMICompanyID " +
                    _whereClauseChargebackSales + ProdGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrReportView> billingChargebackSales = dao.Load<SalesAgrReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, nmi.CompanyName as NMICompanyName, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join SaleChargeback sc on sc.SaleID = s.SaleID " +
                    "inner join UpsellSale us on us.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                        "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "inner join NMICompany nmi on nmi.NMICompanyID = nmiMid.NMICompanyID " +
                    _whereClauseChargebackSales + ProdGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrReportView> upsellChargebackSales = dao.Load<SalesAgrReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, nmi.CompanyName as NMICompanyName, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from OrderSale s " +
                    "inner join SaleChargeback sc on sc.SaleID = s.SaleID " +
                    "inner join Invoice i on i.InvoiceID = s.InvoiceID " +
                    "inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID and ch.Amount > 0 and ch.Success = 1 " +
                    "inner join Orders o on o.OrderID = s.OrderID " +
                    "inner join Billing b on b.BillingID = o.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                        "inner join Product p on p.ProductID = o.ProductID " +
                    "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "inner join NMICompany nmi on nmi.NMICompanyID = nmiMid.NMICompanyID " +
                    _whereClauseChargebackSales + ProdGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrReportView> orderChargebackSales = dao.Load<SalesAgrReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, nmi.CompanyName as NMICompanyName, lch.PaymentTypeID, count(lch.LastChargeHistoryID) as SaleCount " +
                    "from (select max(ch.ChargeHistoryID) as LastChargeHistoryID, b.PaymentTypeID from BillingChargeback bc " +
                          "inner join BillingSubscription bs on bs.BillingID = bc.BillingID " +
                          "inner join Billing b on b.BillingID = bs.BillingID " +
                              "inner join Subscription sss on bs.SubscriptionID = sss.SubscriptionID " +
                              "inner join Product p on sss.ProductID = p.ProductID " +
                                "inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID and ch.Success = 1 and ch.Amount > 0 " +
                          "where bc.CreateDT between @startDate and @endDate " + ProdGroupFilter +
                          "group by bc.BillingID) lch " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = lch.LastChargeHistoryID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "inner join NMICompany nmi on nmi.NMICompanyID = nmiMid.NMICompanyID " +
                    "group by mid.AssertigyMIDID, lch.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrReportView> chargebackBillings = dao.Load<SalesAgrReportView>(q);

                res = new List<SalesAgrReportFullView<SalesAgrReportView>>();
                foreach (SalesAgrReportView item in billingSales.Concat(upsellSales).Concat(orderSales))
                {
                    SalesAgrReportFullView<SalesAgrReportView> existed = res.Where(i => i.BaseReportView.AssertigyMIDID == item.AssertigyMIDID && i.BaseReportView.PaymentTypeID == item.PaymentTypeID).FirstOrDefault();
                    if (existed == null)
                    {
                        SalesAgrReportFullView<SalesAgrReportView> created = new SalesAgrReportFullView<SalesAgrReportView>();
                        created.BaseReportView = item;
                        created.SaleChargebackCount = 0;
                        created.SaleChargebackPercentage = 0.0M;
                        created.ProjectedSaleCount = 0;
                        created.ProjectedSaleChargebackCount = 0;
                        created.ProjectedChargebackPercentage = 0.0M;
                        res.Add(created);
                    }
                    else
                    {
                        existed.BaseReportView.SaleCount += item.SaleCount;
                    }
                }

                foreach (SalesAgrReportView item in billingChargebackSales.Concat(upsellChargebackSales).Concat(orderChargebackSales).Concat(chargebackBillings))
                {
                    SalesAgrReportFullView<SalesAgrReportView> existed = res.Where(i => i.BaseReportView.AssertigyMIDID == item.AssertigyMIDID && i.BaseReportView.PaymentTypeID == item.PaymentTypeID).FirstOrDefault();
                    if (existed == null)
                    {
                        SalesAgrReportFullView<SalesAgrReportView> created = new SalesAgrReportFullView<SalesAgrReportView>();
                        created.BaseReportView = item;
                        created.SaleChargebackCount = item.SaleCount;
                        item.SaleCount = 0;
                        created.SaleChargebackPercentage = 0.0M;
                        created.ProjectedSaleCount = 0;
                        created.ProjectedSaleChargebackCount = 0;
                        created.ProjectedChargebackPercentage = 0.0M;
                        res.Add(created);
                    }
                    else
                    {
                        existed.SaleChargebackCount += item.SaleCount;
                    }
                }

                //decimal monthStartTicks = (new DateTime(endDate.Value.Year, endDate.Value.Month, 1, 0, 0, 0)).Ticks;
                //decimal monthEndTicks = (new DateTime(endDate.Value.Year, endDate.Value.Month, 1, 0, 0, 0)).AddMonths(1).Ticks;
                //decimal endDateTicks = endDate.Value.Ticks;
                //decimal monthDatePercentage = (endDateTicks - monthStartTicks) / (monthEndTicks - monthStartTicks);
                ////Just to make sure no division by zero
                //if (monthDatePercentage == 0.0M)
                //{
                //    monthDatePercentage = 0.01M;
                //}
                //foreach (SalesAgrReportFullView<SalesAgrReportView> item in res)
                //{
                //    if (item.BaseReportView.SaleCount > 0)
                //    {
                //        item.SaleChargebackPercentage = (decimal)item.SaleChargebackCount.Value / (decimal)item.BaseReportView.SaleCount.Value;
                //        item.ProjectedSaleCount = (int)(((decimal)item.BaseReportView.SaleCount.Value) / monthDatePercentage);
                //    }
                //    if (item.SaleChargebackCount > 0)
                //    {
                //        item.ProjectedSaleChargebackCount = (int)(((decimal)item.SaleChargebackCount.Value) / monthDatePercentage);
                //    }
                //}
                foreach (SalesAgrReportFullView<SalesAgrReportView> item in res)
                {
                    if (item.BaseReportView.SaleCount.Value > 0)
                    {
                        item.SaleChargebackPercentage = (decimal)item.SaleChargebackCount.Value / (decimal)item.BaseReportView.SaleCount.Value;
                    }
                    else
                    {
                        item.SaleChargebackPercentage = 1M;
                    }
                }


                res = res.
                    OrderBy(i => i.BaseReportView.PaymentTypeID).
                    OrderBy(i => i.BaseReportView.AssertigyDisplayName).
                    ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<SalesAgrReportFullView<SalesAgrByAffReportView>> GetAgrSalesReportByAffiliate(DateTime? startDate, DateTime? endDate, string productcode)
        {
            IList<SalesAgrReportFullView<SalesAgrByAffReportView>> res = null;

            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.Now;
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }
                string ProductGroupFilter = "";
                if (productcode != "")
                {
                    ProductGroupFilter = " and p.ProductID = " + productcode + " ";
                }
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);

                MySqlCommand q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, ifnull(b.Affiliate, '') as Affiliate, ifnull(b.SubAffiliate, '') as SubAffiliate, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join BillingSale bs on bs.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bs.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                    "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "left join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "where s.CreateDT between @startDate and @endDate " + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID, ifnull(b.Affiliate, ''), ifnull(b.SubAffiliate, '')");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByAffReportView> billingSales = dao.Load<SalesAgrByAffReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, ifnull(b.Affiliate, '') as Affiliate, ifnull(b.SubAffiliate, '') as SubAffiliate, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join UpsellSale us on us.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                     "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
               "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "left join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "where s.CreateDT between @startDate and @endDate " + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID, ifnull(b.Affiliate, ''), ifnull(b.SubAffiliate, '')");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByAffReportView> upsellSales = dao.Load<SalesAgrByAffReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, ifnull(o.Affiliate, '') as Affiliate, ifnull(o.SubAffiliate, '') as SubAffiliate, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from OrderSale s " +
                    "inner join Invoice i on i.InvoiceID = s.InvoiceID " +
                    "inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID and ch.Amount > 0 and ch.Success = 1 " +
                    "inner join Orders o on o.OrderID = s.OrderID " +
                    "inner join Billing b on b.BillingID = o.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                        "inner join Product p on p.ProductID = o.ProductID " +
                   "left join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "where s.CreateDT between @startDate and @endDate " + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID, ifnull(o.Affiliate, ''), ifnull(o.SubAffiliate, '')");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByAffReportView> orderSales = dao.Load<SalesAgrByAffReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, ifnull(b.Affiliate, '') as Affiliate, ifnull(b.SubAffiliate, '') as SubAffiliate, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join SaleChargeback sc on sc.SaleID = s.SaleID " +
                    "inner join BillingSale bs on bs.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bs.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                      "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
              "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "left join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    _whereClauseChargebackSales + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID, ifnull(b.Affiliate, ''), ifnull(b.SubAffiliate, '')");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByAffReportView> billingChargebackSales = dao.Load<SalesAgrByAffReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, ifnull(b.Affiliate, '') as Affiliate, ifnull(b.SubAffiliate, '') as SubAffiliate, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join SaleChargeback sc on sc.SaleID = s.SaleID " +
                    "inner join UpsellSale us on us.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                   "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                 "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "left join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    _whereClauseChargebackSales + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID, ifnull(b.Affiliate, ''), ifnull(b.SubAffiliate, '')");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByAffReportView> upsellChargebackSales = dao.Load<SalesAgrByAffReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, ifnull(o.Affiliate, '') as Affiliate, ifnull(o.SubAffiliate, '') as SubAffiliate, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from OrderSale s " +
                    "inner join SaleChargeback sc on sc.SaleID = s.SaleID " +
                    "inner join Invoice i on i.InvoiceID = s.InvoiceID " +
                    "inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID and ch.Amount > 0 and ch.Success = 1 " +
                    "inner join Orders o on o.OrderID = s.OrderID " +
                    "inner join Billing b on b.BillingID = o.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                        "inner join Product p on p.ProductID = o.ProductID " +
                   "left join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    _whereClauseChargebackSales + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID, ifnull(o.Affiliate, ''), ifnull(o.SubAffiliate, '')");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByAffReportView> orderChargebackSales = dao.Load<SalesAgrByAffReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, ifnull(lch.Affiliate, '') as Affiliate, ifnull(lch.SubAffiliate, '') as SubAffiliate, lch.PaymentTypeID, count(lch.LastChargeHistoryID) as SaleCount " +
                    "from (select max(ch.ChargeHistoryID) as LastChargeHistoryID, b.PaymentTypeID, ifnull(b.Affiliate, '') as Affiliate, ifnull(b.SubAffiliate, '') as SubAffiliate from BillingChargeback bc " +
                          "inner join BillingSubscription bs on bs.BillingID = bc.BillingID " +
                          "inner join Billing b on b.BillingID = bs.BillingID " +
                           "inner join Subscription sss on bs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
               "inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID and ch.Success = 1 and ch.Amount > 0 " +
                          "where bc.CreateDT between @startDate and @endDate " + ProductGroupFilter +
                          "group by bc.BillingID) lch " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = lch.LastChargeHistoryID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "left join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "group by mid.AssertigyMIDID, lch.PaymentTypeID, ifnull(lch.Affiliate, ''), ifnull(lch.SubAffiliate, '')");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByAffReportView> chargebackBillings = dao.Load<SalesAgrByAffReportView>(q);

                res = new List<SalesAgrReportFullView<SalesAgrByAffReportView>>();
                foreach (SalesAgrByAffReportView item in billingSales.Concat(upsellSales).Concat(orderSales))
                {
                    SalesAgrReportFullView<SalesAgrByAffReportView> existed = res.Where(i => i.BaseReportView.AssertigyMIDID == item.AssertigyMIDID && i.BaseReportView.PaymentTypeID == item.PaymentTypeID && i.BaseReportView.Affiliate == item.Affiliate && i.BaseReportView.SubAffiliate == item.SubAffiliate).FirstOrDefault();
                    if (existed == null)
                    {
                        SalesAgrReportFullView<SalesAgrByAffReportView> created = new SalesAgrReportFullView<SalesAgrByAffReportView>();
                        created.BaseReportView = item;
                        created.SaleChargebackCount = 0;
                        created.SaleChargebackPercentage = 0.0M;
                        created.ProjectedSaleCount = 0;
                        created.ProjectedSaleChargebackCount = 0;
                        created.ProjectedChargebackPercentage = 0.0M;
                        res.Add(created);
                    }
                    else
                    {
                        existed.BaseReportView.SaleCount += item.SaleCount;
                    }
                }

                foreach (SalesAgrByAffReportView item in billingChargebackSales.Concat(upsellChargebackSales).Concat(orderChargebackSales).Concat(chargebackBillings))
                {
                    SalesAgrReportFullView<SalesAgrByAffReportView> existed = res.Where(i => i.BaseReportView.AssertigyMIDID == item.AssertigyMIDID && i.BaseReportView.PaymentTypeID == item.PaymentTypeID && i.BaseReportView.Affiliate == item.Affiliate && i.BaseReportView.SubAffiliate == item.SubAffiliate).FirstOrDefault();
                    if (existed == null)
                    {
                        SalesAgrReportFullView<SalesAgrByAffReportView> created = new SalesAgrReportFullView<SalesAgrByAffReportView>();
                        created.BaseReportView = item;
                        created.SaleChargebackCount = item.SaleCount;
                        item.SaleCount = 0;
                        created.SaleChargebackPercentage = 0.0M;
                        created.ProjectedSaleCount = 0;
                        created.ProjectedSaleChargebackCount = 0;
                        created.ProjectedChargebackPercentage = 0.0M;
                        res.Add(created);
                    }
                    else
                    {
                        existed.SaleChargebackCount += item.SaleCount;
                    }
                }

                foreach (SalesAgrReportFullView<SalesAgrByAffReportView> item in res)
                {
                    if (item.BaseReportView.SaleCount.Value > 0)
                    {
                        item.SaleChargebackPercentage = (decimal)item.SaleChargebackCount.Value / (decimal)item.BaseReportView.SaleCount.Value;
                    }
                    else
                    {
                        item.SaleChargebackPercentage = 1M;
                    }
                }

                res = res.
                    OrderBy(i => i.BaseReportView.PaymentTypeID).
                    OrderBy(i => i.BaseReportView.AssertigyDisplayName).
                    OrderBy(i => i.BaseReportView.SubAffiliate).
                    OrderBy(i => i.BaseReportView.Affiliate).
                    ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<SalesAgrReportFullView<SalesAgrByTypeReportView>> GetAgrSalesReportByType(DateTime? startDate, DateTime? endDate, string productcode)
        {
            IList<SalesAgrReportFullView<SalesAgrByTypeReportView>> res = null;

            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.Now;
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }
                string ProductGroupFilter = "";
                if (productcode != "")
                {
                    ProductGroupFilter = " and p.ProductID = " + productcode + " ";
                }
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);

                MySqlCommand q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, s.SaleTypeID, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join BillingSale bs on bs.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bs.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                      "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                  "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "where s.CreateDT between @startDate and @endDate " + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID, s.SaleTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByTypeReportView> billingSales = dao.Load<SalesAgrByTypeReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, 3 as SaleTypeID, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join UpsellSale us on us.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                     "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                   "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "where s.CreateDT between @startDate and @endDate " + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByTypeReportView> upsellSales = dao.Load<SalesAgrByTypeReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, s.SaleType as SaleTypeID, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from OrderSale s " +
                    "inner join Invoice i on i.InvoiceID = s.InvoiceID " +
                    "inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID and ch.Amount > 0 and ch.Success = 1 " +
                    "inner join Orders o on o.OrderID = s.OrderID " +
                    "inner join Billing b on b.BillingID = o.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                        "inner join Product p on p.ProductID = o.ProductID " +
                   "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    "where s.CreateDT between @startDate and @endDate " + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, s.SaleType, b.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByTypeReportView> orderSales = dao.Load<SalesAgrByTypeReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, s.SaleTypeID, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join SaleChargeback sc on sc.SaleID = s.SaleID " +
                    "inner join BillingSale bs on bs.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bs.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                     "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                   "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    _whereClauseChargebackSales + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID, s.SaleTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByTypeReportView> billingChargebackSales = dao.Load<SalesAgrByTypeReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, 3 as SaleTypeID, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from Sale s " +
                    "inner join SaleChargeback sc on sc.SaleID = s.SaleID " +
                    "inner join UpsellSale us on us.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                     "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                   "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    _whereClauseChargebackSales + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, b.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByTypeReportView> upsellChargebackSales = dao.Load<SalesAgrByTypeReportView>(q);

                q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, s.SaleType as SaleTypeID, b.PaymentTypeID, count(s.SaleID) as SaleCount " +
                    "from OrderSale s " +
                    "inner join SaleChargeback sc on sc.SaleID = s.SaleID " +
                    "inner join Invoice i on i.InvoiceID = s.InvoiceID " +
                    "inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID and ch.Amount > 0 and ch.Success = 1 " +
                    "inner join Orders o on o.OrderID = s.OrderID " +
                    "inner join Billing b on b.BillingID = o.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                        "inner join Product p on p.ProductID = o.ProductID " +
                   "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    _whereClauseChargebackSales + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, s.SaleType, b.PaymentTypeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByTypeReportView> orderChargebackSales = dao.Load<SalesAgrByTypeReportView>(q);

                //q = new MySqlCommand("select mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, lch.Affiliate, lch.SubAffiliate, lch.PaymentTypeID, count(lch.LastChargeHistoryID) as SaleCount " +
                //    "from (select max(ch.ChargeHistoryID) as LastChargeHistoryID, b.PaymentTypeID, b.Affiliate, b.SubAffiliate from BillingChargeback bc " +
                //          "inner join BillingSubscription bs on bs.BillingID = bc.BillingID " +
                //          "inner join Billing b on b.BillingID = bs.BillingID " +
                //          "inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID and ch.Success = 1 and ch.Amount > 0 " +
                //          "where bc.CreateDT between @startDate and @endDate " +
                //          "group by bc.BillingID) lch " +
                //    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = lch.LastChargeHistoryID " +
                //    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                //    "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                //    "group by mid.AssertigyMIDID, lch.PaymentTypeID, lch.Affiliate, lch.SubAffiliate");
                //q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                //q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;
                //
                //IList<SalesAgrByTypeReportView> chargebackBillings = dao.Load<SalesAgrByTypeReportView>(q);
                //TODO: workaround for BillingChargebacks, can't determine SaleTypeID
                IList<SalesAgrByTypeReportView> chargebackBillings = new List<SalesAgrByTypeReportView>();

                res = new List<SalesAgrReportFullView<SalesAgrByTypeReportView>>();
                foreach (SalesAgrByTypeReportView item in billingSales.Concat(upsellSales).Concat(orderSales))
                {
                    SalesAgrReportFullView<SalesAgrByTypeReportView> existed = res.Where(i => i.BaseReportView.AssertigyMIDID == item.AssertigyMIDID && i.BaseReportView.PaymentTypeID == item.PaymentTypeID && i.BaseReportView.SaleTypeID == item.SaleTypeID).FirstOrDefault();
                    if (existed == null)
                    {
                        SalesAgrReportFullView<SalesAgrByTypeReportView> created = new SalesAgrReportFullView<SalesAgrByTypeReportView>();
                        created.BaseReportView = item;
                        created.SaleChargebackCount = 0;
                        created.SaleChargebackPercentage = 0.0M;
                        created.ProjectedSaleCount = 0;
                        created.ProjectedSaleChargebackCount = 0;
                        created.ProjectedChargebackPercentage = 0.0M;
                        res.Add(created);
                    }
                    else
                    {
                        existed.BaseReportView.SaleCount += item.SaleCount;
                    }
                }

                foreach (SalesAgrByTypeReportView item in billingChargebackSales.Concat(upsellChargebackSales).Concat(orderChargebackSales).Concat(chargebackBillings))
                {
                    SalesAgrReportFullView<SalesAgrByTypeReportView> existed = res.Where(i => i.BaseReportView.AssertigyMIDID == item.AssertigyMIDID && i.BaseReportView.PaymentTypeID == item.PaymentTypeID && i.BaseReportView.SaleTypeID == item.SaleTypeID).FirstOrDefault();
                    if (existed == null)
                    {
                        SalesAgrReportFullView<SalesAgrByTypeReportView> created = new SalesAgrReportFullView<SalesAgrByTypeReportView>();
                        created.BaseReportView = item;
                        created.SaleChargebackCount = item.SaleCount;
                        item.SaleCount = 0;
                        created.SaleChargebackPercentage = 0.0M;
                        created.ProjectedSaleCount = 0;
                        created.ProjectedSaleChargebackCount = 0;
                        created.ProjectedChargebackPercentage = 0.0M;
                        res.Add(created);
                    }
                    else
                    {
                        existed.SaleChargebackCount += item.SaleCount;
                    }
                }

                foreach (SalesAgrReportFullView<SalesAgrByTypeReportView> item in res)
                {
                    if (item.BaseReportView.SaleCount.Value > 0)
                    {
                        item.SaleChargebackPercentage = (decimal)item.SaleChargebackCount.Value / (decimal)item.BaseReportView.SaleCount.Value;
                    }
                    else
                    {
                        item.SaleChargebackPercentage = 1M;
                    }
                }

                res = res.
                    OrderBy(i => i.BaseReportView.PaymentTypeID).
                    OrderBy(i => i.BaseReportView.AssertigyDisplayName).
                    OrderBy(i => i.BaseReportView.SaleTypeID).
                    OrderBy(i => i.BaseReportView.SaleCount).
                    ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<SalesAgrReportFullView<SalesAgrByReasonReportView>> GetAgrSalesReportByReason(DateTime? startDate, DateTime? endDate, int? midID, string prdgroup)
        {
            IList<SalesAgrReportFullView<SalesAgrByReasonReportView>> res = null;

            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.Now;
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }
                string ProductGroupFilter = "";
                if (prdgroup != "")
                {
                    ProductGroupFilter = " and p.ProductID = " + prdgroup + " ";
                }
                startDate = new DateTime(startDate.Value.Year, startDate.Value.Month, startDate.Value.Day, 0, 0, 0);
                endDate = new DateTime(endDate.Value.Year, endDate.Value.Month, endDate.Value.Day, 23, 59, 59);

                MySqlCommand q = new MySqlCommand("SELECT mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, s.SaleTypeID, b.PaymentTypeID, count(s.SaleID) as SaleCount, " +
                    "crc.Description as ChargebackReasonCode, cbt.DisplayName as ChargebackStatus, sc.ChargebackStatusTID, sc.ChargebackReasonCodeID FROM SaleChargeback sc " +
                    "inner join Sale s on s.SaleID=sc.SaleID " +
                    "inner join BillingSale bs on bs.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bs.ChargeHistoryID " +
                    "inner join ChargebackStatusType cbt on cbt.ChargebackStatusTypeID=sc.ChargebackStatusTID " +
                    "inner join ChargebackReasonCode crc on crc.ChargebackReasonCodeID=sc.ChargebackReasonCodeID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                      "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
               "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    _whereClauseChargebackSales + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, sc.ChargebackStatusTID, sc.ChargebackReasonCodeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByReasonReportView> billingSales = dao.Load<SalesAgrByReasonReportView>(q);

                if (midID != null)
                    billingSales = billingSales.Where(u => u.AssertigyMIDID == midID).ToList();

                q = new MySqlCommand("SELECT mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, s.SaleTypeID, b.PaymentTypeID, count(s.SaleID) as SaleCount, " +
                    "crc.Description as ChargebackReasonCode, cbt.DisplayName as ChargebackStatus, sc.ChargebackStatusTID, sc.ChargebackReasonCodeID FROM SaleChargeback sc " +
                    "inner join Sale s on s.SaleID=sc.SaleID " +
                    "inner join UpsellSale bs on bs.SaleID = s.SaleID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bs.ChargeHistoryID " +
                    "inner join ChargebackStatusType cbt on cbt.ChargebackStatusTypeID=sc.ChargebackStatusTID " +
                    "inner join ChargebackReasonCode crc on crc.ChargebackReasonCodeID=sc.ChargebackReasonCodeID " +
                    "inner join BillingSubscription bsubs on bsubs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "inner join Billing b on b.BillingID = bsubs.BillingID " +
                     "inner join Subscription sss on bsubs.SubscriptionID = sss.SubscriptionID " +
                        "inner join Product p on sss.ProductID = p.ProductID " +
                "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                    "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    _whereClauseChargebackSales + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, sc.ChargebackStatusTID, sc.ChargebackReasonCodeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByReasonReportView> upsellSales = dao.Load<SalesAgrByReasonReportView>(q);

                if (midID != null)
                    upsellSales = upsellSales.Where(u => u.AssertigyMIDID == midID).ToList();


                q = new MySqlCommand("SELECT mid.AssertigyMIDID, mid.DisplayName as AssertigyDisplayName, mid.MID as AssertigyMID, s.SaleType as SaleTypeID, b.PaymentTypeID, count(s.SaleID) as SaleCount, " +
                    "crc.Description as ChargebackReasonCode, cbt.DisplayName as ChargebackStatus, sc.ChargebackStatusTID, sc.ChargebackReasonCodeID FROM SaleChargeback sc " +
                    "inner join OrderSale s on s.SaleID=sc.SaleID " +
                    "inner join Invoice i on i.InvoiceID = s.InvoiceID " +
                    "inner join ChargeHistoryInvoice chi on chi.InvoiceID = i.InvoiceID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = chi.ChargeHistoryID and ch.Amount > 0 and ch.Success = 1 " +
                    "inner join ChargebackStatusType cbt on cbt.ChargebackStatusTypeID=sc.ChargebackStatusTID " +
                    "inner join ChargebackReasonCode crc on crc.ChargebackReasonCodeID=sc.ChargebackReasonCodeID " +
                    "inner join Orders o on o.OrderID = s.OrderID " +
                    "inner join Billing b on b.BillingID = o.BillingID " +
                    "inner join AssertigyMID mid on mid.MID = ch.ChildMID " +
                        "inner join Product p on p.ProductID = o.ProductID " +
                   "inner join NMICompanyMID nmiMid on nmiMid.AssertigyMIDID = mid.AssertigyMIDID " +
                    _whereClauseChargebackSales + ProductGroupFilter +
                    "group by mid.AssertigyMIDID, sc.ChargebackStatusTID, sc.ChargebackReasonCodeID");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate.Value;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate.Value;

                IList<SalesAgrByReasonReportView> orderSales = dao.Load<SalesAgrByReasonReportView>(q);

                if (midID != null)
                    upsellSales = orderSales.Where(u => u.AssertigyMIDID == midID).ToList();

                res = new List<SalesAgrReportFullView<SalesAgrByReasonReportView>>();
                foreach (SalesAgrByReasonReportView item in billingSales.Concat(upsellSales).Concat(orderSales))
                {
                    SalesAgrReportFullView<SalesAgrByReasonReportView> existed = res.Where(i => i.BaseReportView.ChargebackReasonCodeID == item.ChargebackReasonCodeID && i.BaseReportView.ChargebackStatusTID == item.ChargebackStatusTID).FirstOrDefault();
                    if (existed == null)
                    {
                        SalesAgrReportFullView<SalesAgrByReasonReportView> created = new SalesAgrReportFullView<SalesAgrByReasonReportView>();
                        created.BaseReportView = item;
                        created.SaleChargebackCount = 0;
                        created.SaleChargebackPercentage = 0.0M;
                        created.ProjectedSaleCount = 0;
                        created.ProjectedSaleChargebackCount = 0;
                        created.ProjectedChargebackPercentage = 0.0M;
                        res.Add(created);
                    }
                    else
                    {
                        existed.BaseReportView.SaleCount += item.SaleCount;
                    }
                }

                foreach (SalesAgrReportFullView<SalesAgrByReasonReportView> item in res)
                {
                    if (item.BaseReportView.SaleCount.Value > 0)
                    {
                        item.SaleChargebackPercentage = (decimal)item.SaleChargebackCount.Value / (decimal)item.BaseReportView.SaleCount.Value;
                    }
                    else
                    {
                        item.SaleChargebackPercentage = 1M;
                    }
                }

                res = res.
                    OrderBy(i => i.BaseReportView.PaymentTypeID).
                    OrderBy(i => i.BaseReportView.AssertigyDisplayName).
                    OrderBy(i => i.BaseReportView.ChargebackReasonCode).
                    OrderBy(i => i.BaseReportView.SaleCount).
                    ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        private string PaymentTypeName(int? paymentTypeID)
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

        private string SaleTypeName(int? saleTypeID)
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

        public IList<SalesAgrReportFullView<SalesAgrReportView>> GetAgrSalesReportWithProjected(DateTime? startDate, DateTime? endDate, string prodgroup)
        {
            IList<SalesAgrReportFullView<SalesAgrReportView>> res = null;

            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.Now;
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }

                DateTime endProjectedDate = startDate.Value.AddDays(-1);
                DateTime startProjectedDate = endProjectedDate.AddDays(-30);

                res = GetAgrSalesReport(startDate, endDate, prodgroup);
                IList<SalesAgrReportFullView<SalesAgrReportView>> resProjectedSales = GetAgrSalesReport(startProjectedDate, endProjectedDate, prodgroup);

                foreach (SalesAgrReportFullView<SalesAgrReportView> item in resProjectedSales)
                {
                    SalesAgrReportFullView<SalesAgrReportView> existed = res.Where(i => i.BaseReportView.AssertigyMIDID == item.BaseReportView.AssertigyMIDID && i.BaseReportView.PaymentTypeID == item.BaseReportView.PaymentTypeID).FirstOrDefault();
                    if (existed == null)
                    {
                        SalesAgrReportFullView<SalesAgrReportView> created = new SalesAgrReportFullView<SalesAgrReportView>();

                        created.ProjectedSaleCount = item.BaseReportView.SaleCount;
                        created.ProjectedSaleChargebackCount = item.SaleChargebackCount;
                        created.ProjectedChargebackPercentage = item.SaleChargebackPercentage;

                        created.BaseReportView = item.BaseReportView;
                        created.BaseReportView.SaleCount = 0;
                        created.SaleChargebackCount = 0;
                        created.SaleChargebackPercentage = 0.0M;

                        res.Add(created);
                    }
                    else
                    {
                        existed.ProjectedSaleCount = item.BaseReportView.SaleCount; ;
                        existed.ProjectedSaleChargebackCount = item.SaleChargebackCount;
                        existed.ProjectedChargebackPercentage = item.SaleChargebackPercentage;
                    }
                }

                res = res.
                    OrderBy(i => i.BaseReportView.PaymentTypeID).
                    OrderBy(i => i.BaseReportView.AssertigyDisplayName).
                    ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<SalesAgrReportFullView<SalesAgrByAffReportView>> GetAgrSalesReportByAffiliateWithProjected(DateTime? startDate, DateTime? endDate, string prodgroup)
        {
            IList<SalesAgrReportFullView<SalesAgrByAffReportView>> res = null;

            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.Now;
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }

                DateTime endProjectedDate = startDate.Value.AddDays(-1);
                DateTime startProjectedDate = endProjectedDate.AddDays(-30);

                res = GetAgrSalesReportByAffiliate(startDate, endDate, prodgroup);
                IList<SalesAgrReportFullView<SalesAgrByAffReportView>> resProjectedSales = GetAgrSalesReportByAffiliate(startProjectedDate, endProjectedDate, prodgroup);

                foreach (SalesAgrReportFullView<SalesAgrByAffReportView> item in resProjectedSales)
                {
                    SalesAgrReportFullView<SalesAgrByAffReportView> existed = res.Where(i => i.BaseReportView.AssertigyMIDID == item.BaseReportView.AssertigyMIDID && i.BaseReportView.PaymentTypeID == item.BaseReportView.PaymentTypeID && i.BaseReportView.Affiliate == item.BaseReportView.Affiliate && i.BaseReportView.SubAffiliate == item.BaseReportView.SubAffiliate).FirstOrDefault();
                    if (existed == null)
                    {
                        SalesAgrReportFullView<SalesAgrByAffReportView> created = new SalesAgrReportFullView<SalesAgrByAffReportView>();

                        created.ProjectedSaleCount = item.BaseReportView.SaleCount; ;
                        created.ProjectedSaleChargebackCount = item.SaleChargebackCount;
                        created.ProjectedChargebackPercentage = item.SaleChargebackPercentage;

                        created.BaseReportView = item.BaseReportView;
                        created.BaseReportView.SaleCount = 0;
                        created.SaleChargebackCount = 0;
                        created.SaleChargebackPercentage = 0.0M;

                        res.Add(created);
                    }
                    else
                    {
                        existed.ProjectedSaleCount = item.BaseReportView.SaleCount; ;
                        existed.ProjectedSaleChargebackCount = item.SaleChargebackCount;
                        existed.ProjectedChargebackPercentage = item.SaleChargebackPercentage;
                    }
                }

                res = res.
                    OrderBy(i => i.BaseReportView.PaymentTypeID).
                    OrderBy(i => i.BaseReportView.AssertigyDisplayName).
                    OrderBy(i => i.BaseReportView.SubAffiliate).
                    OrderBy(i => i.BaseReportView.Affiliate).
                    ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<SalesAgrReportFullView<SalesAgrByTypeReportView>> GetAgrSalesReportByTypeWithProjected(DateTime? startDate, DateTime? endDate, string prdgroup)
        {
            IList<SalesAgrReportFullView<SalesAgrByTypeReportView>> res = null;

            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.Now;
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }

                DateTime endProjectedDate = startDate.Value.AddDays(-1);
                DateTime startProjectedDate = endProjectedDate.AddDays(-30);

                res = GetAgrSalesReportByType(startDate, endDate, prdgroup);
                IList<SalesAgrReportFullView<SalesAgrByTypeReportView>> resProjectedSales = GetAgrSalesReportByType(startProjectedDate, endProjectedDate, prdgroup);

                foreach (SalesAgrReportFullView<SalesAgrByTypeReportView> item in resProjectedSales)
                {
                    SalesAgrReportFullView<SalesAgrByTypeReportView> existed = res.Where(i => i.BaseReportView.AssertigyMIDID == item.BaseReportView.AssertigyMIDID && i.BaseReportView.PaymentTypeID == item.BaseReportView.PaymentTypeID && i.BaseReportView.SaleTypeID == item.BaseReportView.SaleTypeID).FirstOrDefault();
                    if (existed == null)
                    {
                        SalesAgrReportFullView<SalesAgrByTypeReportView> created = new SalesAgrReportFullView<SalesAgrByTypeReportView>();

                        created.ProjectedSaleCount = item.BaseReportView.SaleCount; ;
                        created.ProjectedSaleChargebackCount = item.SaleChargebackCount;
                        created.ProjectedChargebackPercentage = item.SaleChargebackPercentage;

                        created.BaseReportView = item.BaseReportView;
                        created.BaseReportView.SaleCount = 0;
                        created.SaleChargebackCount = 0;
                        created.SaleChargebackPercentage = 0.0M;

                        res.Add(created);
                    }
                    else
                    {
                        existed.ProjectedSaleCount = item.BaseReportView.SaleCount; ;
                        existed.ProjectedSaleChargebackCount = item.SaleChargebackCount;
                        existed.ProjectedChargebackPercentage = item.SaleChargebackPercentage;
                    }
                }

                res = res.
                    OrderBy(i => i.BaseReportView.PaymentTypeID).
                    OrderBy(i => i.BaseReportView.AssertigyDisplayName).
                    OrderBy(i => i.BaseReportView.SaleTypeID).
                    OrderBy(i => i.BaseReportView.SaleCount).
                    ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<SalesAgrByReasonShortReportView> GetAgrSalesReportByReasonWithProjected(DateTime? startDate, DateTime? endDate, int? midID, string productgroup)
        {
            IList<SalesAgrByReasonShortReportView> res = new List<SalesAgrByReasonShortReportView>();

            try
            {
                if (startDate == null)
                {
                    startDate = DateTime.Now;
                }
                if (endDate == null)
                {
                    endDate = DateTime.Now;
                }

                var list = GetAgrSalesReportByReason(startDate, endDate, midID, productgroup);
                foreach (var reasonCodeID in list.Select(u => u.BaseReportView.ChargebackReasonCodeID).Distinct())
                    res.Add(new SalesAgrByReasonShortReportView(list, reasonCodeID));

                res = res.Where(u => u.TotalWonLostCount > 0).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public string GetAgrSalesReportWithProjectedCSV(DateTime? startDate, DateTime? endDate, string prodgroup)
        {
            string res = null;

            try
            {
                IList<SalesAgrReportFullView<SalesAgrReportView>> list = GetAgrSalesReportWithProjected(startDate, endDate, prodgroup);
                if (list != null)
                {
                    StringBuilder csv = new StringBuilder();
                    csv.AppendLine("DBA,FMA Number,CHID Value,TYP,MTD SETTCNT,MTD CBCNT,MTD CBPCT,PROJ SETTCNT,PROJ CBCNT,PROJ CBPCT");

                    foreach (SalesAgrReportFullView<SalesAgrReportView> item in list)
                    {
                        csv.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                            item.BaseReportView.AssertigyDisplayName,
                            item.BaseReportView.AssertigyMID,
                            item.BaseReportView.NMICompanyName,
                            PaymentTypeName(item.BaseReportView.PaymentTypeID),
                            item.BaseReportView.SaleCount,
                            item.SaleChargebackCount,
                            (item.SaleChargebackPercentage.Value).ToString("0.00%"),
                            item.ProjectedSaleCount,
                            item.ProjectedSaleChargebackCount,
                            (item.ProjectedChargebackPercentage.Value).ToString("0.00%")
                            ));
                    }

                    res = csv.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public string GetAgrSalesReportByAffiliateWithProjectedCSV(DateTime? startDate, DateTime? endDate, string prdgroup)
        {
            string res = null;

            try
            {
                IList<SalesAgrReportFullView<SalesAgrByAffReportView>> list = GetAgrSalesReportByAffiliateWithProjected(startDate, endDate, prdgroup);
                if (list != null)
                {
                    StringBuilder csv = new StringBuilder();
                    csv.AppendLine("Affiliate,SubAffiliate,DBA,FMA Number,TYP,MTD SETTCNT,MTD CBCNT,MTD CBPCT,PROJ SETTCNT,PROJ CBCNT,PROJ CBPCT");

                    foreach (SalesAgrReportFullView<SalesAgrByAffReportView> item in list)
                    {
                        csv.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                            item.BaseReportView.Affiliate,
                            item.BaseReportView.SubAffiliate,
                            item.BaseReportView.AssertigyDisplayName,
                            item.BaseReportView.AssertigyMID,
                            PaymentTypeName(item.BaseReportView.PaymentTypeID),
                            item.BaseReportView.SaleCount,
                            item.SaleChargebackCount,
                            (item.SaleChargebackPercentage.Value).ToString("0.00%"),
                            item.ProjectedSaleCount,
                            item.ProjectedSaleChargebackCount,
                            (item.ProjectedChargebackPercentage.Value).ToString("0.00%")
                            ));
                    }

                    res = csv.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public string GetAgrSalesReportByTypeWithProjectedCSV(DateTime? startDate, DateTime? endDate, string prodgroup)
        {
            string res = null;

            try
            {
                IList<SalesAgrReportFullView<SalesAgrByTypeReportView>> list = GetAgrSalesReportByTypeWithProjected(startDate, endDate, prodgroup);
                if (list != null)
                {
                    StringBuilder csv = new StringBuilder();
                    csv.AppendLine("Charge Type,DBA,FMA Number,TYP,MTD SETTCNT,MTD CBCNT,MTD CBPCT,PROJ SETTCNT,PROJ CBCNT,PROJ CBPCT");

                    foreach (SalesAgrReportFullView<SalesAgrByTypeReportView> item in list)
                    {
                        csv.AppendLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}",
                            SaleTypeName(item.BaseReportView.SaleTypeID),
                            item.BaseReportView.AssertigyDisplayName,
                            item.BaseReportView.AssertigyMID,
                            PaymentTypeName(item.BaseReportView.PaymentTypeID),
                            item.BaseReportView.SaleCount,
                            item.SaleChargebackCount,
                            (item.SaleChargebackPercentage.Value).ToString("0.00%"),
                            item.ProjectedSaleCount,
                            item.ProjectedSaleChargebackCount,
                            (item.ProjectedChargebackPercentage.Value).ToString("0.00%")
                            ));
                    }

                    res = csv.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public string GetAgrSalesReportByReasonCSV(DateTime? startDate, DateTime? endDate, int? midID, string prodgroup)
        {
            string res = null;

            try
            {
                IList<SalesAgrReportFullView<SalesAgrByReasonReportView>> list = GetAgrSalesReportByReason(startDate, endDate, midID, prodgroup);
                if (list != null)
                {
                    StringBuilder csv = new StringBuilder();
                    csv.AppendLine("Chargeback Reason,Won,Lost,Total,Won %,Lost %");

                    foreach (var reasonCodeID in list.Select(u => u.BaseReportView.ChargebackReasonCodeID).Distinct())
                    {
                        var item = new SalesAgrByReasonShortReportView(list, reasonCodeID);
                        if (item.TotalWonLostCount > 0)
                            csv.AppendLine(string.Format("{0},{1},{2},{3},{4},{5}",
                                    item.ReasonCode,
                                    item.WonCount.ToString(),
                                    item.LostCount.ToString(),
                                    item.TotalWonLostCount.ToString(),
                                    ((float)item.WonCount / item.TotalWonLostCount).ToString("0.00%"),
                                    ((float)item.LostCount / item.TotalWonLostCount).ToString("0.00%"))
                                );
                    }

                    res = csv.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<SaleBillingView> CreateDailySaleReport(DateTime date)
        {
            IList<SaleBillingView> res = null;

            try
            {
                DateTime startDate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                DateTime endDate = startDate.AddDays(1);

                MySqlCommand q = new MySqlCommand(
                    " select sale.SaleID, sale.CreateDT as SaleDT, ch.Amount as TotalAmount, " +
                    " case IfNull(bsale.RebillCycle, 0) " +
                        " when 0 then " +
                            " case " +
                                " when IfNull(bsd.IsSavePrice, 0) = 0 and bsd.Discount is null and bsd.NewShippingAmount is null then round(IfNull(s.InitialShipping, 0) * (case IfNull(chCur.CurrencyAmount, 0) when 0 then 1 else ch.Amount / chCur.CurrencyAmount end), 2) " +
                                " when bsd.IsSavePrice = 1 then round(IfNull(s.SaveShipping, 0) * (case IfNull(chCur.CurrencyAmount, 0) when 0 then 1 else ch.Amount / chCur.CurrencyAmount end), 2) " +
                                " when bsd.Discount is not null then round((IfNull(s.InitialShipping, 0) - IfNull(s.InitialShipping, 0) * bsd.Discount) * (case IfNull(chCur.CurrencyAmount, 0) when 0 then 1 else ch.Amount / chCur.CurrencyAmount end), 2) " +
                                " when bsd.NewShippingAmount is not null then round(bsd.NewShippingAmount * (case IfNull(chCur.CurrencyAmount, 0) when 0 then 1 else ch.Amount / chCur.CurrencyAmount end), 2) " +
                                " else 0 " +
                            " end " +
                        " when 1 then round(IfNull(s.SecondShipping, 0) * (case IfNull(chCur.CurrencyAmount, 0) when 0 then 1 else ch.Amount / chCur.CurrencyAmount end), 2)" +
                        " else round(IfNull(s.RegularShipping, 0) * (case IfNull(chCur.CurrencyAmount, 0) when 0 then 1 else ch.Amount / chCur.CurrencyAmount end), 2)" +
                    " end as ShippingAmount," +
                    " b.* " +
                    " from Sale sale " +
                    " inner join BillingSale bsale on bsale.SaleID = sale.SaleID" +
                    " inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID" +
                    " left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID" +
                    " inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID" +
                    " inner join Billing b on b.BillingID = bs.BillingID" +
                    " inner join Subscription s on s.SubscriptionID = bs.SubscriptionID" +
                    " left join BillingSubscriptionDiscount bsd on bsd.BillingSubscriptionID = bs.BillingSubscriptionID" +
                    " where sale.CreateDT >= @date1 and sale.CreateDT < @date2" +
                    " and ch.Success = 1 and ch.Amount > 0" +
                    " union all " +
                    " select sale.SaleID, sale.CreateDT as SaleDT, ch.Amount as TotalAmount, " +
                    " 0 as ShippingAmount," +
                    " b.* " +
                    " from Sale sale " +
                    " inner join UpsellSale bsale on bsale.SaleID = sale.SaleID" +
                    " inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID" +
                    " inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID" +
                    " inner join Billing b on b.BillingID = bs.BillingID" +
                    " where sale.CreateDT >= @date1 and sale.CreateDT < @date2" +
                    " and ch.Success = 1 and ch.Amount > 0" +
                    " ");
                q.Parameters.Add("@date1", MySqlDbType.Timestamp).Value = startDate;
                q.Parameters.Add("@date2", MySqlDbType.Timestamp).Value = endDate;

                res = dao.Load<SaleBillingView>(q).OrderBy(i => i.SaleDT).ToList();
                foreach (var item in res)
                {
                    if (item.ShippingAmount > item.TotalAmount)
                    {
                        //Just in case
                        item.ShippingAmount = item.TotalAmount;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public void DailyShipmentReport()
        {
            var saleService = new SaleService();
            List<string> recordTables = new List<string>();
            var shippers = dao.Load<Shipper>(new MySqlCommand("SELECT * From Shipper"));
            for (int i = 0; i < shippers.Count; i++)
            {
                var shipperService = ShipperService.GetShipperServiceByShipperID(shippers[i].ShipperID);
                if (shipperService != null)
                    recordTables.Add(shipperService.TableName);
                else
                {
                    shippers.RemoveAt(i);
                    i--;
                }
            }
            List<string> shipmentMethods = shippers.Select(u => u.Name).ToList();

            List<SaleRecordView> saleList = new List<SaleRecordView>();
            StringBuilder csv = new StringBuilder();
            csv.Append("Order Number, Customer's Full name, Shipment Method, Shipment Date, Tracking Number" + Environment.NewLine);
            try
            {
                for (int i = 0; i < shippers.Count; i++)
                {
                    MySqlCommand q = new MySqlCommand("SELECT s.SaleID, r.ShippedDT CreateDT, s.TrackingNumber TrackingNumber, '"
                        + shipmentMethods[i] + "' ShipmentMethod FROM Sale s inner join " + recordTables[i] + " r on r.SaleID=s.SaleID"
                        + " Where r.ShippedDT >= DATE_FORMAT( DATE_SUB(CURDATE(),INTERVAL 1 DAY) , '%Y-%m-%d 00:00:00' ) and r.ShippedDT  < DATE_FORMAT( CURDATE() , '%Y-%m-%d 00:00:00' ) AND r.Completed=1");
                    saleList.AddRange(dao.Load<SaleRecordView>(q));
                }
                foreach (var sale in saleList)
                {
                    var billing = saleService.GetBillingBySale(sale.SaleID.Value);
                    if (billing != null)
                    {
                        csv.Append(string.Format("\"{0}\",", sale.SaleID));
                        csv.Append(string.Format("\"{0}\",", billing.FullName));
                        csv.Append(string.Format("\"{0}\",", sale.ShipmentMethod));
                        csv.Append(string.Format("\"{0}\",", sale.CreateDT));
                        csv.Append(string.Format("\"{0}\"", sale.TrackingNumber));
                        csv.Append(Environment.NewLine);
                    }
                }
                SendDailyShipmentEmail(csv.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public List<AssertigyMID> GetAllMIDs()
        {
            List<AssertigyMID> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from AssertigyMID where Visible=1");
                res = dao.Load<AssertigyMID>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        private void SendDailyShipmentEmail(string attachContent)
        {
            using (Stream s = Utility.OpenStringAsStreamUTF8(attachContent))
            {
                Attachment data = new Attachment(s, "text/csv");
                data.ContentDisposition.CreationDate = DateTime.Now;
                data.ContentDisposition.ModificationDate = DateTime.Now;
                data.ContentDisposition.FileName = "DailyReport.csv";

                MailMessage message = new MailMessage();
                message.From = new MailAddress("donotreply@trianglecrm.com", "TriangleCRM Automated Reporting");
                message.Subject = "Daily Shipment Report";
                foreach (string to in Config.Current.DAILY_SHIPMENTS_EMAILS)
                {
                    message.To.Add(new MailAddress(to));
                }
                message.Attachments.Add(data);

                SmtpClient client = new SmtpClient();
                client.Host = "relay.jangosmtp.net";
                client.Send(message);
            }
        }

        public List<UnsentUnpayedView> GetUnsentShippments(IDao customDao)
        {
            return GetUnsentShippments(DateTime.Now.AddDays(-60), DateTime.Now, customDao);
        }

        public List<UnsentUnpayedView> GetUnsentShippments(DateTime? startDate, DateTime? endDate, IDao customDao)
        {
            List<UnsentUnpayedView> res1 = null;
            List<UnsentUnpayedView> res2 = null;
            List<UnsentUnpayedView> res3 = null;
            List<UnsentUnpayedView> res = null;
            try
            {
                MySqlCommand q = null;
                q = new MySqlCommand("");
                string sqlQuery = string.Empty;
                for (int i = 0; i < ShipperService.ImplementedShippers.Count; i++)
                {
                    var shipper = ShipperService.ImplementedShippers[i];
                    if (i != 0)
                        sqlQuery += " union all ";
                    sqlQuery += @" 
                                Select ID, FirstName, LastName, Reason, BillingID, max(RegID) as MaxRegID, CreateDT, 'none' as BillType, 0 as Amount, " + shipper.ShipperID + @" as GroupID, '-' as SKU FROM
                                (
                                Select r.SaleID as ID, b.FirstName, b.LastName, r.Response as Reason, b.BillingID, r.CreateDT, r.RegID From " + shipper.TableName + " r " +
                                @"inner join BillingSale bSale on bSale.SaleID=r.SaleID 
                                 inner join BillingSubscription bs on bs.BillingSubscriptionID=bSale.BillingSubscriptionID
                                 inner join Billing b on bs.BillingID=b.BillingID
                                 inner join Sale s on s.SaleID=r.SaleID" + GetShipperSaleInnerFilter(shipper.ShipperID) + @"
                                 left join KeymailRecordToSend kmP on kmP.SaleID = s.SaleID and kmP.RegID is not null
                                 where (s.CreateDT between @StartDate and @EndDate)
                                 and coalesce(s.NotShip,0) <> 1
                                 and kmP.KeymailRecordToSendID is null
                                 and not exists (select * from SaleRefund inner join ChargeHistoryEx chrefund on chrefund.ChargeHistoryID = SaleRefund.ChargeHistoryID where SaleRefund.SaleID = s.SaleID and chrefund.Success = 1) 
                                 and b.BillingID not in (Select BillingID From ShippingBlocked)
                                 " + GetShipperSaleWhereFilter(shipper.ShipperID) + @"
                                 order by r.CreateDT desc
                                 ) t
                                 group by ID
                                 having MaxRegID is null
                                ";
                }
                q.CommandText = sqlQuery;
                q.CommandTimeout = 9999;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res1 = customDao.Load<UnsentUnpayedView>(q).ToList();

                q = new MySqlCommand("");
                sqlQuery = string.Empty;
                for (int i = 0; i < ShipperService.ImplementedShippers.Count; i++)
                {
                    var shipper = ShipperService.ImplementedShippers[i];
                    if (i != 0)
                        sqlQuery += " union all ";
                    sqlQuery += @" 
                                 Select ID, FirstName, LastName, Reason, BillingID, max(RegID) as MaxRegID, CreateDT, 'none' as BillType, 0 as Amount, " + shipper.ShipperID + @" as GroupID, '-' as SKU FROM
                                (
                                 Select r.SaleID as ID, b.FirstName, b.LastName, r.Response as Reason, b.BillingID, r.CreateDT, r.RegID From " + shipper.TableName + " r " +
                                @"inner join ExtraTrialShipSale eSale on eSale.SaleID=r.SaleID 
                                 inner join Billing b on eSale.BillingID=b.BillingID
                                 inner join Sale s on s.SaleID=r.SaleID" + GetShipperSaleInnerFilter(shipper.ShipperID) + @"
                                 left join KeymailRecordToSend kmP on kmP.SaleID = s.SaleID and kmP.RegID is not null
                                 where (s.CreateDT between @StartDate and @EndDate)
                                 and coalesce(s.NotShip,0) <> 1
                                 and kmP.KeymailRecordToSendID is null
                                 and b.BillingID not in (Select BillingID From ShippingBlocked)
                                 " + GetShipperSaleWhereFilter(shipper.ShipperID) + @"
                                 order by r.CreateDT desc
                                 ) t
                                 group by ID
                                 having MaxRegID is null
                                ";
                }
                q.CommandText = sqlQuery;
                q.CommandTimeout = 9999;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res2 = customDao.Load<UnsentUnpayedView>(q).ToList();

                q = new MySqlCommand("");
                sqlQuery = string.Empty;
                for (int i = 0; i < ShipperService.ImplementedShippers.Count; i++)
                {
                    var shipper = ShipperService.ImplementedShippers[i];
                    if (i != 0)
                        sqlQuery += " union all ";
                    sqlQuery += @" 
                                 Select ID, FirstName, LastName, Reason, BillingID, max(RegID) as MaxRegID, CreateDT, 'none' as BillType, 0 as Amount, " + shipper.ShipperID + @" as GroupID, '-' as SKU FROM
                                (
                                 Select r.SaleID as ID, b.FirstName, b.LastName, r.Response as Reason, b.BillingID, r.CreateDT, r.RegID  From " + shipper.TableName + " r " +
                                @"inner join UpsellSale uSale on uSale.SaleID=r.SaleID 
                                 inner join Upsell u on uSale.UpsellID=u.UpsellID
                                 inner join Billing b on u.BillingID=b.BillingID
                                 inner join Sale s on s.SaleID=r.SaleID" + GetShipperSaleInnerFilter(shipper.ShipperID) + @"
                                 left join KeymailRecordToSend kmP on kmP.SaleID = s.SaleID and kmP.RegID is not null
                                 where (s.CreateDT between @StartDate and @EndDate)
                                 and coalesce(s.NotShip,0) <> 1
                                 and not exists (select * from SaleRefund inner join ChargeHistoryEx chrefund on chrefund.ChargeHistoryID = SaleRefund.ChargeHistoryID where SaleRefund.SaleID = s.SaleID and chrefund.Success = 1) 
                                 and b.BillingID not in (Select BillingID From ShippingBlocked)
                                 and kmP.KeymailRecordToSendID is null
                                 " + GetShipperSaleWhereFilter(shipper.ShipperID) + @"
                                 order by r.CreateDT desc
                                 ) t
                                 group by ID
                                 having MaxRegID is null
                                ";
                }
                q.CommandText = sqlQuery;
                q.CommandTimeout = 9999;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res3 = customDao.Load<UnsentUnpayedView>(q).ToList();
                res = res1.Union(res2).Union(res3).Where(u => u.Reason.ToLower().Contains("error")).OrderByDescending(u => u.CreateDT).GroupBy(u => u.ID).Select(u => u.FirstOrDefault()).ToList();
                //res = res1.Union(res2).Union(res3).GroupBy(u => u.ID).Select(u => u.FirstOrDefault()).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<UnsentUnpayedView>();
            }

            return res;
        }

        public List<UnsentUnpayedView> GetUnsentShippmentsFromAggTable()
        {
            var q = new MySqlCommand(
                    @"
                        Select a.SaleID as ID, a.ShipperID as GroupID, a.Date as CreateDT, a.Reason, a.BillingID, b.FirstName, b.LastName, 0 as Amount, 'none' as BillType, t.SKU From AggUnsentShipments a 
                        inner join Billing b on a.BillingID=b.BillingID
                        inner join
                        (
	                        Select bSale.SaleID, (case when sal.SaleTypeID=1 then coalesce(bSale.ProductCode, s.ProductCode) else coalesce(bSale.ProductCode, s.SKU2) end) as SKU 
	                        from BillingSale bSale
	                        inner join AggUnsentShipments a on a.SaleID=bSale.SaleID
	                        inner join Sale sal on sal.SaleID=bSale.SaleID
	                        inner join BillingSubscription bs on bs.BillingSubscriptionID=bSale.BillingSubscriptionID
	                        inner join Subscription s on s.SubscriptionID=bs.SubscriptionID
	                        union all
	                        Select uSale.SaleID, u.ProductCode as SKU 
	                        from UpsellSale uSale
	                        inner join Upsell u on u.UpsellID = uSale.UpsellID
	                        inner join AggUnsentShipments a on a.SaleID=uSale.SaleID
	                        union all
	                        Select eSale.SaleID, e.ProductCode as SKU 
	                        from ExtraTrialShipSale eSale
	                        inner join ExtraTrialShip e on e.ExtraTrialShipID=eSale.ExtraTrialShipID
	                        inner join AggUnsentShipments a on a.SaleID=eSale.SaleID
                        ) t on t.SaleID=a.SaleID
                        left join SaleRefund sRefund on sRefund.SaleID=a.SaleID
                        inner join Sale s on s.SaleID=a.SaleID
                        where sRefund.SaleRefundID is null 
                        and (a.Date between @StartDate and @EndDate)
                        and s.NotShip != 1
                    "
                    );
            q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = DateTime.Now.AddDays(-60);
            q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = DateTime.Now;
            return dao.Load<UnsentUnpayedView>(q).ToList();
            
        }

        public List<UnsentUnpayedView> GetUnsentShippmentsFromAggTable(int? shipperID)
        {
            var q = new MySqlCommand(
                    @"
                        Select a.SaleID as ID, a.ShipperID as GroupID, a.Date as CreateDT, a.Reason, a.BillingID, b.FirstName, b.LastName, 0 as Amount, 'none' as BillType, t.SKU From AggUnsentShipments a 
                        inner join Billing b on a.BillingID=b.BillingID
                        inner join
                        (
	                        Select bSale.SaleID, (case when sal.SaleTypeID=1 then coalesce(bSale.ProductCode, s.ProductCode) else coalesce(bSale.ProductCode, s.SKU2) end) as SKU 
	                        from BillingSale bSale
	                        inner join AggUnsentShipments a on a.SaleID=bSale.SaleID
	                        inner join Sale sal on sal.SaleID=bSale.SaleID
	                        inner join BillingSubscription bs on bs.BillingSubscriptionID=bSale.BillingSubscriptionID
	                        inner join Subscription s on s.SubscriptionID=bs.SubscriptionID
	                        union all
	                        Select uSale.SaleID, u.ProductCode as SKU 
	                        from UpsellSale uSale
	                        inner join Upsell u on u.UpsellID = uSale.UpsellID
	                        inner join AggUnsentShipments a on a.SaleID=uSale.SaleID
	                        union all
	                        Select eSale.SaleID, e.ProductCode as SKU 
	                        from ExtraTrialShipSale eSale
	                        inner join ExtraTrialShip e on e.ExtraTrialShipID=eSale.ExtraTrialShipID
	                        inner join AggUnsentShipments a on a.SaleID=eSale.SaleID
                        ) t on t.SaleID=a.SaleID
                        left join SaleRefund sRefund on sRefund.SaleID=a.SaleID
                        inner join Sale s on s.SaleID=a.SaleID
                        where 
                        a.ShipperID=@ShipperID
                        and sRefund.SaleRefundID is null
                        and s.NotShip != 1
                        and (a.Date between @StartDate and @EndDate)
                    "
                    );
            q.Parameters.Add("@ShipperID", MySqlDbType.Int32).Value = shipperID;
            q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = DateTime.Now.AddDays(-60);
            q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = DateTime.Now;
            return dao.Load<UnsentUnpayedView>(q).ToList();
        }

        private string GetShipperSaleInnerFilter(int? shipperID)
        {
            StringBuilder res = new StringBuilder();

            for (int i = 0; i < ShipperService.ImplementedShippers.Count; i++)
            {
                var shipper = ShipperService.ImplementedShippers[i];
                if (shipper.ShipperID != shipperID)
                    res.AppendFormat(" left join {0} on {1}.SaleID=s.SaleID and {2}.RegID is not null ", shipper.TableName, shipper.TableName, shipper.TableName);
            }

            return res.ToString();
        }

        private string GetShipperSaleWhereFilter(int? shipperID)
        {
            StringBuilder res = new StringBuilder();

            for (int i = 0; i < ShipperService.ImplementedShippers.Count; i++)
            {
                var shipper = ShipperService.ImplementedShippers[i];
                if (shipper.ShipperID != shipperID)
                    res.AppendFormat(" and {0}.SaleID is null ", shipper.TableName);
            }

            return res.ToString();
        }

        public List<UnsentUnpayedView> GetUnpayedBills()
        {
            return GetUnpayedBills(DateTime.Now.AddDays(-30), DateTime.Now.AddYears(5));
        }

        public List<UnsentUnpayedView> GetUnpayedBills(DateTime? startDate, DateTime? endDate)
        {
            List<UnsentUnpayedView> res = null;
            List<UnsentUnpayedExView> resEx = null;
            try
            {
                MySqlCommand q = new MySqlCommand("");
                string sqlQuery = string.Empty;

                sqlQuery = @" 
                        Select ID, t.FirstName, t.LastName, t.BillingID, t.BillType,
                            'Can not choose MID' as Reason, t.CreditCard, t.CreateDT as CreateDT, t.Amount
                            from
                            (
	                            Select b.BillingID as ID, b.FirstName, b.LastName, 
	                            b.CreditCard as CreditCard, b.BillingID, e.CreateDT,
	                            'Trial/Upsell' as BillType, e.Amount, tpg.ProductName as TrialProductGroup, upg.ProductName as UpsellProductGroup
	                            From EmergencyQueue e
                                left join 
                                (
		                            select p.ProductName, bs.BillingID from BillingSale bsale
		                            inner join BillingSubscription bs on bs.BillingSubscriptionID = bsale.BillingSubscriptionID
		                            inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
		                            inner join Product p on p.ProductID=s.ProductID
                                    inner join Sale sale on sale.SaleID=bsale.SaleID
		                            where bs.StatusTID = 1
		                            and bsale.ChargeHistoryID = 0
	                            ) as tpg on tpg.BillingID=e.BillingID
                                left join 
	                            (
		                            select p.ProductName, u.BillingID from Upsell u
		                            inner join Billing b on b.BillingID = u.BillingID
		                            inner join UpsellType ut on ut.UpsellTypeID=u.UpsellTypeID
		                            inner join Product p on p.ProductID=ut.ProductID
		                            inner join BillingSubscription bs on bs.BillingID = b.BillingID
		                            left join UpsellSale us on us.UpsellID = u.UpsellID
		                            where bs.StatusTID = 1
		                            and us.SaleID is null
	                            ) upg on upg.BillingID = e.BillingID
	                            inner join Billing b on e.BillingID=b.BillingID
	                            inner join PaymentType pt on pt.PaymentTypeID=b.PaymentTypeID
	                            Where (e.CreateDT between @StartDate and @EndDate)
	                            order by e.CreateDT desc
                            ) t
                            left join IgnoreUnbilledTransaction ign on ign.BillingID = t.BillingID
                            where ign.IgnoreUnbilledTransactionID is null
                            and (t.UpsellProductGroup is not null or t.TrialProductGroup is not null)
                            group by t.BillingID
                        ";

                q.CommandText = sqlQuery;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;

                resEx = dao.Load<UnsentUnpayedExView>(q).ToList();

                res = resEx.Select(u => new UnsentUnpayedView()
                {
                    Reason = string.Format(u.Reason, new CreditCard(u.CreditCard).TryGetCardName()),
                    Amount = u.Amount,
                    BillingID = u.BillingID,
                    CreateDT = u.CreateDT,
                    BillType = u.BillType,
                    FirstName = u.FirstName,
                    ID = u.ID,
                    LastName = u.LastName,
                    GroupID = 0
                }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<UnsentUnpayedView>();
            }

            return res;
        }

        public List<UnsentUnpayedView> GetUnpayedRebills(DateTime? startDate, DateTime? endDate)
        {
            List<UnsentUnpayedView> res = new List<UnsentUnpayedView>();
            try
            {
                MySqlCommand q = new MySqlCommand("");
                string sqlQuery = string.Empty;

                sqlQuery = @" 
                            select t.*, IfNull(ch.MerchantAccountID, 0) as GroupID, '-' as SKU from
                            (
                            select bs.BillingSubscriptionID as ID, bs.BillingID, bs.nextbilldate as CreateDT,b.FirstName, b.LastName,
                            case when k.RebillCycle = 0 then (s.SecondBillAmount + s.SecondShipping) else 
		                            (s.RegularShipping + s.RegularBillAmount) end as Amount, 
                            IfNull(q.Reason, '-') as Reason, 'Rebill' as BillType, k.LastChargeHistoryID
                            from BillingSubscription bs
                            inner join Billing b on b.BillingID = bs.BillingID
                            inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
                            left join 
                            (
	                            Select IfNull(max(bSale.RebillCycle), 0) as RebillCycle, bSale.BillingSubscriptionID, Max(ch.ChargeHistoryID) as LastChargeHistoryID From BillingSale bSale
	                            inner join BillingSubscription bs on bSale.BillingSubscriptionID=bs.BillingSubscriptionID
	                            left join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID
	                            where bs.NextBillDate between @StartDate and @EndDate
	                            group by bs.BillingSubscriptionID
                            ) k on k.BillingSubscriptionID=bs.BillingSubscriptionID
                            left join QueueRebill q on q.BillingSubscriptionID=bs.BillingSubscriptionID
                            where s.ProductID > 0 and s.Recurring = 1
                            and (bs.NextBillDate between @StartDate and @EndDate)
                            and bs.StatusTID=1
                            order by q.CreateDT desc
                            ) t
                            left join ChargeHistoryEx ch on ch.ChargeHistoryID = t.LastChargeHistoryID
                            left join IgnoreUnbilledTransaction ign on ign.BillingSubscriptionID = t.ID
                            where ign.IgnoreUnbilledTransactionID is null
                            group by t.ID";

                q.CommandText = sqlQuery;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res = dao.Load<UnsentUnpayedView>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<UnsentUnpayedView>();
            }

            return res;
        }

        public List<UnsentUnpayedView> GetUnpayedRebills()
        {
            return GetUnpayedRebills(DateTime.Now.AddDays(-30), DateTime.Now);
        }

        public List<BillingCallView> GetRecordingReport(DateTime? startDate, DateTime? endDate, int skip, int take)
        {
            List<BillingCallView> res = null;
            try
            {
                string clientFilter = " where ";
                //if (Config.Current.APPLICATION_ID == ApplicationEnum.LocalhostTriangleCRM)
                if (Config.Current.APPLICATION_ID == ApplicationEnum.TriangleCRM)
                    clientFilter = " inner join TPClientFocusCustomerProduct tpp on tpp.CustomerProduct=c.CustomerProduct where tpp.TPClientID is NULL and ";

                MySqlCommand q = new MySqlCommand(
                    @"
                        select c.*, b.*, t.NumberOfCalls, c.CreateDT as LastCallDate, b.CreateDT as BillingCreateDT from Billing b
                        inner join (select c.CustomerID, max(c.CallID) as CallID, count(*) as NumberOfCalls from `Call` c
                            " + clientFilter +
                            @" c.CreateDT between @StartDate and @EndDate
                            group by CustomerID) t on t.CustomerID = b.BillingID
                        inner join `Call` c on c.CallID = t.CallID
                        order by c.CreateDT desc
                        limit @Skip, @Take
                     "
                    );
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                q.Parameters.Add("@Skip", MySqlDbType.Int32).Value = skip;
                q.Parameters.Add("@Take", MySqlDbType.Int32).Value = take;
                res = dao.Load<BillingCallView>(q).ToList();
            }
            catch (Exception ex)
            {
                res = new List<BillingCallView>();
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public int GetRecordingReportTotalCount(DateTime? startDate, DateTime? endDate)
        {
            int res = 0;
            try
            {
                string clientFilter = " where ";
                //if (Config.Current.APPLICATION_ID == ApplicationEnum.LocalhostTriangleCRM)
                if (Config.Current.APPLICATION_ID == ApplicationEnum.TriangleCRM)
                    clientFilter = " inner join TPClientFocusCustomerProduct tpp on tpp.CustomerProduct=c.CustomerProduct where tpp.TPClientID is NULL and ";

                MySqlCommand q = new MySqlCommand(
                    @"
                        select count(distinct b.BillingID) from `Call` c
                        inner join Billing b on b.BillingID = c.CustomerID
                    " + clientFilter +
                    @"
                        c.CreateDT between @StartDate and @EndDate
                        order by c.CreateDT desc
                     "
                    );
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res = dao.ExecuteScalar<int>(q) ?? 0;
            }
            catch (Exception ex)
            {
                res = 0;
                logger.Error(GetType(), ex);
            }

            return res;
        }
    }
}
