using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Fitdiet.Store1.Logic;
using TrimFuel.Business;
using TrimFuel.Model.Views;
using TrimFuel.Model.Utility;

namespace Fitdiet.Store1.account
{
    public partial class cp_orders : AccountPageX
    {
        RefererService refererService = new RefererService();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);

            IList<SaleView> saleList = refererService.GetOwnSalesExcludeReturns(Membership.CurrentReferer.RefererID.Value);
            if (saleList == null)
            {
                saleList = new List<SaleView>();
            }

            IList<SaleView> shippedSaleList = saleList.Where(i => i.ShippedDT != null).ToList();
            if (shippedSaleList.Count > 0)
            {
                rShippedSales.DataSource = shippedSaleList;
            }
            else
            {
                phNoShippedSales.Visible = true;
            }

            IList<SaleView> pendingSaleList = saleList.Where(i => i.ShippedDT == null).ToList();
            if (pendingSaleList.Count > 0)
            {
                foreach (var i in pendingSaleList)
                {
                    i.ShippedDT = CalculateEstimatedShipmentDate(i.CreateDT.Value);
                }
                rPendingSales.DataSource = pendingSaleList;
            }
            else
            {
                phNoPendingSales.Visible = true;
            }
        }

        private DateTime CalculateEstimatedShipmentDate(DateTime createDT)
        {
            DateTime res = createDT;
            if (createDT.DayOfWeek == DayOfWeek.Sunday ||
                createDT.DayOfWeek == DayOfWeek.Monday ||
                createDT.DayOfWeek == DayOfWeek.Tuesday ||
                createDT.DayOfWeek == DayOfWeek.Wednesday)
            {
                res = res.AddDays(2);
            }
            else if (createDT.DayOfWeek == DayOfWeek.Thursday || 
                createDT.DayOfWeek == DayOfWeek.Friday)
            {
                res = res.AddDays(4);
            }
            else
            {
                res = res.AddDays(3);
            }

            if (res < DateTime.Now)
            {
                //As sale was created yesterday
                res = CalculateEstimatedShipmentDate(DateTime.Now.AddDays(-1));
            }

            return res;
        }
    }
}
