using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls
{
    public partial class RefererSales : System.Web.UI.UserControl
    {
        RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public Referer Referer { get; set; }

        public decimal AvailableCommission { get; set; }
        public decimal TotalSalesCommission { get; set; }
        public int TotalPrimarySales { get; set; }
        public int TotalSecondarySales { get; set; }

        private IList<SaleView> primarySaleList = null;
        private IList<SaleView> secondarySaleList = null;

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            //In USD
            decimal availableCommission = refererService.GetAvailableCommission(Referer.RefererID.Value);
            //if (availableCommission > 0M)
            //{
            //    bUseInStore.Enabled = true;
            //    if (availableCommission >= RefererService.REFERER_COMMISSION_MIN_FOR_CASH)
            //    {
            //        bConvertToCash.Text = string.Format("Convert to Real Cash ({0} USD) &amp; Send Me A Cheque", availableCommission.ToString("c"));
            //        bConvertToCash.Enabled = true;
            //    }
            //    else
            //    {
            //        bConvertToCash.Enabled = false;
            //    }
            //}
            //else
            //{
            //    bUseInStore.Enabled = false;
            //    bConvertToCash.Enabled = false;
            //}

            //In Ecigs dollars
            AvailableCommission = ConvertToEcigsDollars(availableCommission);

            IList<RefererCommission> refCommissions = refererService.GetCommissions(Referer.RefererID.Value);
            if (refCommissions != null && refCommissions.Count > 0)
            {
                rCommissions.DataSource = refCommissions;
                phNoCommissions.Visible = false;
            }
            else
            {
                phNoCommissions.Visible = true;
            }

            primarySaleList = refererService.GetPrimaryReferalsSalesExcludeReturns(Referer.RefererID.Value);
            if (primarySaleList == null)
            {
                primarySaleList = new List<SaleView>();
            }
            secondarySaleList = refererService.GetSecondaryReferalsSalesExcludeReturns(Referer.RefererID.Value);
            if (secondarySaleList == null)
            {
                secondarySaleList = new List<SaleView>();
            }

            //Calculate commissions
            foreach (var i in primarySaleList)
            {
                i.ChargeAmount = refererService.CalculateRefererPrimaryCommissionInEcigsDollars(i.ChargeAmount.Value);
            }
            foreach (var i in secondarySaleList)
            {
                i.ChargeAmount = refererService.CalculateRefererSecondaryCommissionInEcigsDollars(i.ChargeAmount.Value);
            }

            IList<SaleView> allSales = primarySaleList.Union(secondarySaleList).ToList();
            if (allSales.Count > 0)
            {
                IList<IDictionary<string, object>> ds = new List<IDictionary<string, object>>();
                foreach (var p in allSales.GroupBy(i => new DateTime(i.CreateDT.Value.Year, i.CreateDT.Value.Month, 1, 0, 0, 0, 0)))
                {
                    IDictionary<string, object> agrView = new Dictionary<string, object>();
                    agrView["Period"] = string.Format("{0} - {1}{2}",
                        p.Key.ToString("MMM d, yyyy"),
                        p.Key.AddMonths(1).ToString("MMM d, yyyy"),
                        (p.Key.Year == DateTime.Today.Year && p.Key.Month == DateTime.Today.Month) ? " (Current)" : "");
                    DateTime releaseDate = p.Key.AddMonths(1).AddDays(RefererService.REFERER_COMMISSION_INCUBATION_PERIOD - 1);
                    agrView["Released"] = (releaseDate < DateTime.Today) ?
                        releaseDate.ToString("MMM d, yyyy") :
                        string.Format("Payable on {0} ({1} Days Hold)", releaseDate.ToString("MMM d, yyyy"), RefererService.REFERER_COMMISSION_INCUBATION_PERIOD);
                    agrView["Commission"] = (releaseDate < DateTime.Today) ?
                        string.Format("{0} E-Cigs Dollars", p.Sum(i => i.ChargeAmount.Value).ToString("c")) :
                        string.Format("{0} E-Cigs Dollars (Estimated)", p.Sum(i => i.ChargeAmount.Value).ToString("c"));
                    agrView["PrimarySales"] = primarySaleList.Where(i => i.CreateDT.Value.Year == p.Key.Year && i.CreateDT.Value.Month == p.Key.Month).ToList();
                    agrView["SecondarySales"] = secondarySaleList.Where(i => i.CreateDT.Value.Year == p.Key.Year && i.CreateDT.Value.Month == p.Key.Month).ToList();

                    ds.Add(agrView);
                }

                rSalesCommissions.DataSource = ds;

                TotalSalesCommission = allSales.Sum(i => i.ChargeAmount.Value);
                TotalPrimarySales = primarySaleList.Count;
                TotalSecondarySales = secondarySaleList.Count;
                phNoSalesCommissions.Visible = false;
            }
            else
            {
                phNoSalesCommissions.Visible = true;
            }
        }

        public decimal ConvertToEcigsDollars(decimal amount)
        {
            return refererService.ConvertToEcigsDollars(amount);
        }

        public decimal ConvertToUSD(decimal amount)
        {
            return refererService.ConvertToUSD(amount);
        }
    }
}