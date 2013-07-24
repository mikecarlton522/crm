using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingBatchEmailTypeDataProvider : EntityDataProvider<BillingBatchEmailType>
    {
        private const string SELECT_COMMAND = "SELECT * FROM BillingBatchEmailType WHERE BillingBatchEmailTypeID=@BillingBatchEmailTypeID;";

        public override void Save(BillingBatchEmailType entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override BillingBatchEmailType Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingBatchEmailTypeID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingBatchEmailType Load(DataRow row)
        {
            BillingBatchEmailType res = new BillingBatchEmailType();

            if (!(row["BillingBatchEmailTypeID"] is DBNull))
                res.BillingBatchEmailTypeID = Convert.ToInt32(row["BillingBatchEmailTypeID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);
            if (!(row["DynamicEmailID"] is DBNull))
                res.DynamicEmailID = Convert.ToInt32(row["DynamicEmailID"]);

            return res;
        }
    }
}
