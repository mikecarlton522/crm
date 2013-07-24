using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Web.Admin.Logic;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin
{
    public partial class reports_referers_sales : PageX
    {
        private RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DateFilter1.Date1 = DateTime.Today.AddDays(1 - DateTime.Today.Day);
                DateFilter1.Date2 = DateTime.Today;
            }
        }

        public override string HeaderString
        {
            get { return "Referer Sales Report"; }
        }

        public decimal TotalSalesCommission { get; set; }
        public int TotalPrimarySales { get; set; }
        public int TotalSecondarySales { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            IList<RefererView> primarySaleList = refererService.GetPrimaryReferalsSalesExcludeReturns(DateFilter1.Date1WithTime, DateFilter1.Date2WithTime);
            IList<RefererView> secondarySaleList = refererService.GetSecondaryReferalsSalesExcludeReturns(DateFilter1.Date1WithTime, DateFilter1.Date2WithTime);

            //Calculate commissions
            foreach (var i in primarySaleList)
            {
                i.SalesAmount = refererService.CalculateRefererPrimaryCommissionInEcigsDollars(i.SalesAmount);
            }
            foreach (var i in secondarySaleList)
            {
                i.SalesAmount = refererService.CalculateRefererSecondaryCommissionInEcigsDollars(i.SalesAmount);
            }

            if (primarySaleList.Count > 0 || secondarySaleList.Count > 0)
            {
                IList<Dictionary<string, object>> ds = primarySaleList.Union(secondarySaleList).
                    OrderBy(i => i.Referer.FullName).
                    GroupBy(i => i.Referer.RefererID).
                    Select(i => new Dictionary<string, object>()
                    {
                        {"RefererID", i.Key},
                        {"FullName", i.First().Referer.FullName},
                        {"Company", i.First().Referer.Company},
                        {"RefererCode", i.First().Referer.RefererCode}
                    }).
                    ToList();

                foreach (var r in ds)
                {
                    r["PrimarySalesCount"] = primarySaleList.Where(i => i.Referer.RefererID == (int?)r["RefererID"]).Sum(i => i.SalesCount);
                    r["SecondarySalesCount"] = secondarySaleList.Where(i => i.Referer.RefererID == (int?)r["RefererID"]).Sum(i => i.SalesCount);
                    r["SalesCommission"] = 
                        primarySaleList.Where(i => i.Referer.RefererID == (int?)r["RefererID"]).Sum(i => i.SalesAmount) +
                        secondarySaleList.Where(i => i.Referer.RefererID == (int?)r["RefererID"]).Sum(i => i.SalesAmount);
                }

                rReferersSales.DataSource = ds;

                TotalSalesCommission = primarySaleList.Sum(i => i.SalesAmount) + secondarySaleList.Sum(i => i.SalesAmount);
                TotalPrimarySales = primarySaleList.Sum(i => i.SalesCount);
                TotalSecondarySales = secondarySaleList.Sum(i => i.SalesCount);

                phNoSales.Visible = false;
            }
            else
            {
                phNoSales.Visible = true;
            }
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            DataBind();
        }
    }
}
