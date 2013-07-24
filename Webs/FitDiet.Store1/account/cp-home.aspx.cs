using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace Fitdiet.Store1.account
{
    public partial class cp_home : AccountPageX
    {
        RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public decimal TotalComissionToday { get; set; }
        public decimal TotalComissionLastMonth { get; set; }
        public decimal TotalComissionLast7Days { get; set; }
        public decimal TotalComissionLast30Days { get; set; }
        public decimal TotalComission { get; set; }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            IList<SaleView> primarySaleList = refererService.GetPrimaryReferalsSalesExcludeReturns(Membership.CurrentReferer.RefererID.Value);
            if (primarySaleList == null)
            {
                primarySaleList = new List<SaleView>();
            }
            IList<SaleView> secondarySaleList = refererService.GetSecondaryReferalsSalesExcludeReturns(Membership.CurrentReferer.RefererID.Value);
            if (secondarySaleList == null)
            {
                secondarySaleList = new List<SaleView>();
            }

            DateTime periodStart = DateTime.Today;
            TotalComissionToday = refererService.CalculateRefererPrimaryCommissionInEcigsDollars(
                    primarySaleList.Where(i => i.CreateDT >= periodStart).Sum(i => i.ChargeAmount.Value)) +
                refererService.CalculateRefererSecondaryCommissionInEcigsDollars(
                    secondarySaleList.Where(i => i.CreateDT >= periodStart).Sum(i => i.ChargeAmount.Value));

            periodStart = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            TotalComissionLastMonth = refererService.CalculateRefererPrimaryCommissionInEcigsDollars(
                    primarySaleList.Where(i => i.CreateDT >= periodStart).Sum(i => i.ChargeAmount.Value)) +
                refererService.CalculateRefererSecondaryCommissionInEcigsDollars(
                    secondarySaleList.Where(i => i.CreateDT >= periodStart).Sum(i => i.ChargeAmount.Value));

            periodStart = DateTime.Today.AddDays(-7);
            TotalComissionLast7Days = refererService.CalculateRefererPrimaryCommissionInEcigsDollars(
                    primarySaleList.Where(i => i.CreateDT >= periodStart).Sum(i => i.ChargeAmount.Value)) +
                refererService.CalculateRefererSecondaryCommissionInEcigsDollars(
                    secondarySaleList.Where(i => i.CreateDT >= periodStart).Sum(i => i.ChargeAmount.Value));

            periodStart = DateTime.Today.AddDays(-30);
            TotalComissionLast30Days = refererService.CalculateRefererPrimaryCommissionInEcigsDollars(
                    primarySaleList.Where(i => i.CreateDT >= periodStart).Sum(i => i.ChargeAmount.Value)) +
                refererService.CalculateRefererSecondaryCommissionInEcigsDollars(
                    secondarySaleList.Where(i => i.CreateDT >= periodStart).Sum(i => i.ChargeAmount.Value));


            TotalComission = refererService.CalculateRefererPrimaryCommissionInEcigsDollars(
                    primarySaleList.Sum(i => i.ChargeAmount.Value)) +
                refererService.CalculateRefererSecondaryCommissionInEcigsDollars(
                    secondarySaleList.Sum(i => i.ChargeAmount.Value));
        }
    }
}
