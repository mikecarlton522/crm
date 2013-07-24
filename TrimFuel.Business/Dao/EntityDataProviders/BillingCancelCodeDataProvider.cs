using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingCancelCodeDataProvider : EntityDataProvider<BillingCancelCode>
    {
        private const string SELECT_COMMAND = "SELECT * FROM BillingCancelCode WHERE BillingCancelCodeID=@BillingCancelCodeID;";

        public override void Save(BillingCancelCode entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override BillingCancelCode Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingCancelCodeID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingCancelCode Load(DataRow row)
        {
            BillingCancelCode res = new BillingCancelCode();

            if (!(row["BillingCancelCodeID"] is DBNull))
                res.BillingCancelCodeID = Convert.ToInt32(row["BillingCancelCodeID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt32(row["BillingID"]);
            if (!(row["CancelCode"] is DBNull))
                res.CancelCode = Convert.ToString(row["CancelCode"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
