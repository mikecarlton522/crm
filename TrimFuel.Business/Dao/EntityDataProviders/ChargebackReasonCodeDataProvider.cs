using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ChargebackReasonCodeDataProvider : EntityDataProvider<ChargebackReasonCode>
    {
        private const string SELECT_COMMAND = "SELECT * FROM ChargebackReasonCode WHERE ChargebackReasonCodeID = @ChargebackReasonCodeID;";

        public override void Save(ChargebackReasonCode entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override ChargebackReasonCode Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ChargebackReasonCodeID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ChargebackReasonCode Load(DataRow row)
        {
            ChargebackReasonCode res = new ChargebackReasonCode();

            if (!(row["ChargebackReasonCodeID"] is DBNull))
                res.ChargebackReasonCodeID = Convert.ToInt32(row["ChargebackReasonCodeID"]);
            if (!(row["ReasonCode"] is DBNull))
                res.ReasonCode = Convert.ToInt32(row["ReasonCode"]);
            if (!(row["PaymentTypeID"] is DBNull))
                res.PaymentTypeID = Convert.ToInt32(row["PaymentTypeID"]);
            if (!(row["Description"] is DBNull))
                res.Description = Convert.ToString(row["Description"]);

            return res;
        }
    }
}
