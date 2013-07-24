using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;
using TrimFuel.Model.Views;

namespace TrimFuel.WebServices.BillingAPI.Model
{
    public class ChargeHistory
    {
        public long? ChargeHistoryID { get; set; }
        public decimal? Amount { get; set; }
        public bool? Success { get; set; }
        public string MID { get; set; }
        public DateTime? Date { get; set; }
        public long? SaleID { get; set; }

        public static ChargeHistory FromChargeHistoryEx(Set<ChargeHistoryEx, FailedChargeHistoryView> ch)
        {
            if (ch == null || (ch.Value1 == null && ch.Value2 == null))
            {
                return null;
            }

            ChargeHistory res = new ChargeHistory();
            if (ch.Value1 != null)
            {
                res.ChargeHistoryID = ch.Value1.ChargeHistoryID;
                res.Amount = ch.Value1.Amount;
                res.Success = ch.Value1.Success;
                res.MID = ch.Value1.ChildMID;
                res.Date = ch.Value1.ChargeDate;
            }
            else
            {
                res.ChargeHistoryID = 0;
                res.Amount = ch.Value2.Amount;
                res.Success = ch.Value2.Success;
                res.MID = ch.Value2.ChildMID;
                res.Date = ch.Value2.ChargeDate;
            }

            return res;
        }
    }

    public class ChargeHistoryList : List<ChargeHistory>
    {
    }
}
