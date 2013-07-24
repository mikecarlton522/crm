using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using System.Data;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class AssertigyMIDAmountViewDataProvider : EntityViewDataProvider<AssertigyMIDAmountView>
    {
        public override AssertigyMIDAmountView Load(DataRow row)
        {
            AssertigyMIDAmountView res = new AssertigyMIDAmountView();

            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);
            res.MID = EntityDataProvider<AssertigyMIDBriefView>.CreateProvider().Load(row);

            return res;
        }
    }
}
