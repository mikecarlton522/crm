using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;
using TrimFuel.Model.Views;
using TrimFuel.Business;
using TrimFuel.Model.Utility;

namespace Fitdiet.Store1.account
{
    public partial class cp_referrals : AccountPageX
    {
        RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected IDictionary<string, object> PrimaryTotal { get; set; }
        protected IDictionary<string, object> SecondaryTotal { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            IList<IDictionary<string, object>> ds;
            DateTime last30Days = DateTime.Today.AddDays(-30);

            IList<SaleView> primarySaleList = refererService.GetPrimaryReferalsSalesExcludeReturns(Membership.CurrentReferer.RefererID.Value);
            if (primarySaleList != null && primarySaleList.Count > 0)
            {
                ds = new List<IDictionary<string, object>>();
                foreach (var b in primarySaleList.GroupBy(i => i.Billing.FullName))
                {
                    IDictionary<string, object> agrView = new Dictionary<string, object>();
                    agrView["Name"] = b.Key;
                    agrView["Last30Days"] = b.Where(i => i.CreateDT.Value >= last30Days).Sum(i => i.ChargeAmount);
                    agrView["Lifetime"] = b.Sum(i => i.ChargeAmount);
                    agrView["Commission"] = refererService.CalculateRefererPrimaryCommissionInEcigsDollars(Convert.ToDecimal(agrView["Lifetime"]));

                    ds.Add(agrView);
                }

                rPrimarySales.DataSource = ds;

                PrimaryTotal = new Dictionary<string, object>();
                PrimaryTotal["Last30Days"] = primarySaleList.Where(i => i.CreateDT.Value >= last30Days).Sum(i => i.ChargeAmount);
                PrimaryTotal["Lifetime"] = primarySaleList.Sum(i => i.ChargeAmount);
                PrimaryTotal["Commission"] = refererService.CalculateRefererPrimaryCommissionInEcigsDollars(Convert.ToDecimal(PrimaryTotal["Lifetime"]));
            }
            else
            {
                phNoPrimarySales.Visible = true;
            }

            IList<SaleView> secondarySaleList = refererService.GetSecondaryReferalsSalesExcludeReturns(Membership.CurrentReferer.RefererID.Value);
            if (secondarySaleList != null && secondarySaleList.Count > 0)
            {
                ds = new List<IDictionary<string, object>>();
                foreach (var b in secondarySaleList.GroupBy(i => i.Billing.FullName))
                {
                    IDictionary<string, object> agrView = new Dictionary<string, object>();
                    agrView["Name"] = b.Key;
                    agrView["Last30Days"] = b.Where(i => i.CreateDT.Value >= last30Days).Sum(i => i.ChargeAmount);
                    agrView["Lifetime"] = b.Sum(i => i.ChargeAmount);
                    agrView["Commission"] = refererService.CalculateRefererSecondaryCommissionInEcigsDollars(Convert.ToDecimal(agrView["Lifetime"]));
                    ds.Add(agrView);
                }

                rSecondarySales.DataSource = ds;

                SecondaryTotal = new Dictionary<string, object>();
                SecondaryTotal["Last30Days"] = secondarySaleList.Where(i => i.CreateDT.Value >= last30Days).Sum(i => i.ChargeAmount);
                SecondaryTotal["Lifetime"] = secondarySaleList.Sum(i => i.ChargeAmount);
                SecondaryTotal["Commission"] = refererService.CalculateRefererSecondaryCommissionInEcigsDollars(Convert.ToDecimal(SecondaryTotal["Lifetime"]));
            }
            else
            {
                phNoSecondarySales.Visible = true;
            }
        }
    }
}
