using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ChargeHistoryViewDataProvider : EntityViewDataProvider<ChargeHistoryView>
    {
        public override ChargeHistoryView Load(DataRow row)
        {
            ChargeHistoryView res = new ChargeHistoryView();

            int? chargeTypeID = row.Field<int?>("ChargeTypeID");
            if ((chargeTypeID == TrimFuel.Model.Enums.ChargeTypeEnum.AuthOnly || 
                 chargeTypeID == TrimFuel.Model.Enums.ChargeTypeEnum.VoidAuthOnly) &&
                row.Table.Columns.Contains("RequestedAmount"))
            {
                res.ChargeHistory = EntityDataProvider<AuthOnlyChargeDetails>.CreateProvider().Load(row);
            }
            else
            {
                res.ChargeHistory = EntityDataProvider<ChargeHistoryEx>.CreateProvider().Load(row);
            }

            if (row.Table.Columns.Contains("CurrencyAmount") &&
                row.Field<decimal?>("CurrencyAmount") != null)
            {
                res.ChargeHistoryCurrency = EntityDataProvider<ChargeHistoryExCurrency>.CreateProvider().Load(row);
                res.Currency = EntityDataProvider<Currency>.CreateProvider().Load(row);
            }
            res.MIDName = row.Field<string>("DisplayName");

            return res;
        }
    }
}
