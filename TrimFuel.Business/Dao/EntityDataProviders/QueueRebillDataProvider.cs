using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class QueueRebillDataProvider : EntityDataProvider<QueueRebill>
    {
        private const string SELECT_COMMAND = "SELECT * FROM QueueRebill WHERE QueueRebillID=@QueueRebillID;";

        public override void Save(QueueRebill entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override QueueRebill Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@QueueRebillID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override QueueRebill Load(System.Data.DataRow row)
        {
            QueueRebill res = new QueueRebill();

            if (!(row["QueueRebillID"] is DBNull))
                res.QueueRebillID = Convert.ToInt32(row["QueueRebillID"]);
            if (!(row["BillingSubscriptionID"] is DBNull))
                res.BillingSubscriptionID = Convert.ToInt32(row["BillingSubscriptionID"]);
            if (!(row["Reason"] is DBNull))
                res.Reason = Convert.ToString(row["Reason"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Amount"] is DBNull))
                res.Amount = Convert.ToDecimal(row["Amount"]);

            return res;
        }
    }
}
