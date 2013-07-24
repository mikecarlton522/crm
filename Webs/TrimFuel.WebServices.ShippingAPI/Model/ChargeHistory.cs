using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Model.Views;

namespace TrimFuel.WebServices.ShippingAPI.Model
{
    public class ChargeHistory
    {
        public long? ChargeHistoryID { get; set; }
        public decimal? Amount { get; set; }
        public bool? Success { get; set; }
        public string MID { get; set; }
        public string GatewayResponse { get; set; }
        public DateTime? Date { get; set; }

        public static ChargeHistory FromChargeHistoryView(ChargeHistoryView ch)
        {
            if (ch == null || ch.ChargeHistory == null)
            {
                return null;
            }

            ChargeHistory res = new ChargeHistory();
            res.ChargeHistoryID = ch.ChargeHistory.ChargeHistoryID;
            res.Amount = ch.CurrencyAmount;
            res.Success = ch.ChargeHistory.Success;
            res.MID = ch.MIDName;
            res.Date = ch.ChargeHistory.ChargeDate;
            res.GatewayResponse = ch.ChargeHistory.Response;

            return res;
        }
    }

    public class ChargeHistoryList : List<ChargeHistory>
    {
        public static ChargeHistoryList FromChargeHistoryViewList(IList<ChargeHistoryView> src)
        {
            ChargeHistoryList res = new ChargeHistoryList();
            if (src != null)
            {
                foreach (var item in src)
                {
                    res.Add(ChargeHistory.FromChargeHistoryView(item));
                }
            }
            return res;
        }
    }
}