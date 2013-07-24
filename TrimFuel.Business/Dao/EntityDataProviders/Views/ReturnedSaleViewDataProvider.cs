using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ReturnedSaleViewDataProvider : EntityViewDataProvider<ReturnedSaleView>
    {
        public override ReturnedSaleView Load(DataRow row)
        {
            ReturnedSaleView res = new ReturnedSaleView();
            if (!(row["ReturnProcessingActionID"] is DBNull))
                res.ReturnProcessingActionID = Convert.ToByte(row["ReturnProcessingActionID"]);
            if (!(row["ExtraTrialShipTypeID"] is DBNull))
                res.ExtraTrialShipTypeID = Convert.ToInt32(row["ExtraTrialShipTypeID"]);
            if (!(row["NewSubscriptionID"] is DBNull))
                res.NewSubscriptionID = Convert.ToInt32(row["NewSubscriptionID"]);
            if (!(row["RefundAmount"] is DBNull))
                res.RefundAmount = Convert.ToDecimal(row["RefundAmount"]);
            if (!(row["UpsellTypeID"] is DBNull))
                res.UpsellTypeID = Convert.ToInt32(row["UpsellTypeID"]);

            return res;
        }
    }
}
