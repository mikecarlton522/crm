using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model;

namespace TrimFuel.WebServices.BillingAPI_old.Model
{
    public class ChargeHistory
    {
        public long? ChargeHistoryID { get; set; }
        public decimal? Amount { get; set; }
        public bool? Success { get; set; }
        public string MID { get; set; }
        public DateTime? Date { get; set; }

        public static ChargeHistory FromChargeHistoryEx(ChargeHistoryEx ch)
        {
            if (ch == null)
            {
                return null;
            }

            ChargeHistory res = new ChargeHistory();
            res.ChargeHistoryID = ch.ChargeHistoryID;
            res.Amount = ch.Amount;
            res.Success = ch.Success;
            res.MID = ch.ChildMID;
            res.Date = ch.ChargeDate;

            return res;
        }
    }

    public class ChargeHistoryList : List<ChargeHistory>
    {
    }
}
