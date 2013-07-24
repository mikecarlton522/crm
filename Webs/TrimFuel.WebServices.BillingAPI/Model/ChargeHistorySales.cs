using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model.Views;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class ChargeHistorySales
    {
        public long? ChargeHistoryID { get; set; }
        public decimal? Amount { get; set; }
        public bool? Success { get; set; }
        public string MID { get; set; }
        public DateTime? Date { get; set; }
        public SaleList SaleList { get; set; }

        public static ChargeHistorySales FromChargeWithSales(ChargeHistoryWithSalesView ch)
        {
            if (ch == null || ch.ChargeHistoryView == null || (ch.ChargeHistoryView.Value1 == null && ch.ChargeHistoryView.Value2 == null))
            {
                return null;
            }

            ChargeHistorySales res = new ChargeHistorySales();
            if (ch.ChargeHistoryView.Value1 != null)
            {
                res.ChargeHistoryID = ch.ChargeHistoryView.Value1.ChargeHistoryID;
                res.Amount = ch.ChargeHistoryView.Value1.Amount;
                res.Success = ch.ChargeHistoryView.Value1.Success;
                res.MID = ch.ChargeHistoryView.Value1.ChildMID;
                res.Date = ch.ChargeHistoryView.Value1.ChargeDate;
            }
            else
            {
                res.ChargeHistoryID = 0;
                res.Amount = ch.ChargeHistoryView.Value2.Amount;
                res.Success = ch.ChargeHistoryView.Value2.Success;
                res.MID = ch.ChargeHistoryView.Value2.ChildMID;
                res.Date = ch.ChargeHistoryView.Value2.ChargeDate;
            }
            if (ch.SaleList != null && ch.SaleList.Count > 0)
            {
                res.SaleList = new SaleList();
                foreach (var item in ch.SaleList)
                {
                    if (item.Value1 != null && item.Value2 != null && 
                        item.Value1.SaleID != null && item.Value2.ProductCodeID != null &&
                        ch.ChargeHistoryView.Value1 != null &&
                        ch.ChargeHistoryView.Value1.Amount != null)
                    {
                        SaleDesc sl = new SaleDesc();
                        sl.ProductID = item.Value2.ProductCodeID.Value;
                        sl.SaleID = item.Value1.SaleID.Value;
                        sl.Amount = ch.ChargeHistoryView.Value1.Amount.Value;
                        if (item.Value3 != null && item.Value3.Amount != null)
                        {
                            sl.Amount = item.Value3.Amount.Value;
                        }
                        res.SaleList.Add(sl);
                    }
                }
            }

            return res;
        }
    }
}