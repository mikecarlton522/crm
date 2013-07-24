using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AssertigyMIDPaymentTypeDataProvider : EntityDataProvider<AssertigyMIDPaymentType>
    {
        private const string INSERT_COMMAND = "INSERT INTO AssertigyMIDPaymentType(AssertigyMIDID, PaymentTypeID) VALUES(@AssertigyMIDID, @PaymentTypeID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE AssertigyMIDPaymentType SET AssertigyMIDID=@AssertigyMIDID, PaymentTypeID=@PaymentTypeID WHERE AssertigyMIDID=@IDAssertigyMIDID AND PaymentTypeID=@IDPaymentTypeID;";
        private const string SELECT_COMMAND = "SELECT * FROM AssertigyMIDPaymentType WHERE AssertigyMIDID=@IDAssertigyMIDID AND PaymentTypeID=@IDPaymentTypeID;";

        public override void Save(AssertigyMIDPaymentType entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.AssertigyMIDPaymentTypeID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDAssertigyMIDID", MySqlDbType.Int32).Value = entity.AssertigyMIDPaymentTypeID.Value.AssertigyMIDID;
                cmd.Parameters.Add("@IDPaymentTypeID", MySqlDbType.Int32).Value = entity.AssertigyMIDPaymentTypeID.Value.PaymentTypeID;
            }

            cmd.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = entity.AssertigyMIDID;
            cmd.Parameters.Add("@PaymentTypeID", MySqlDbType.Int32).Value = entity.PaymentTypeID;


            if (entity.AssertigyMIDPaymentTypeID == null)
            {
                cmd.ExecuteNonQuery();
                entity.AssertigyMIDPaymentTypeID = new AssertigyMIDPaymentType.ID() { AssertigyMIDID = entity.AssertigyMIDID.Value, PaymentTypeID = entity.PaymentTypeID.Value };
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("AssertigyMIDPaymentType({0},{1}) was not found in database.", entity.AssertigyMIDPaymentTypeID.Value.AssertigyMIDID, entity.AssertigyMIDPaymentTypeID.Value.PaymentTypeID));
                }
                else
                {
                    entity.AssertigyMIDPaymentTypeID = new AssertigyMIDPaymentType.ID() { AssertigyMIDID = entity.AssertigyMIDID.Value, PaymentTypeID = entity.PaymentTypeID.Value };
                }
            }
        }

        public override AssertigyMIDPaymentType Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IDAssertigyMIDID", MySqlDbType.Int32).Value = ((AssertigyMIDPaymentType.ID?)key).Value.AssertigyMIDID;
            cmd.Parameters.Add("@IDPaymentTypeID", MySqlDbType.Int32).Value = ((AssertigyMIDPaymentType.ID?)key).Value.PaymentTypeID;

            return Load(cmd).FirstOrDefault();
        }

        public override AssertigyMIDPaymentType Load(DataRow row)
        {
            AssertigyMIDPaymentType res = new AssertigyMIDPaymentType();

            if (!(row["AssertigyMIDID"] is DBNull))
                res.AssertigyMIDID = Convert.ToInt32(row["AssertigyMIDID"]);
            if (!(row["PaymentTypeID"] is DBNull))
                res.PaymentTypeID = Convert.ToInt32(row["PaymentTypeID"]);
            if (res.AssertigyMIDID != null && res.PaymentTypeID != null)
            {
                res.AssertigyMIDPaymentTypeID = new AssertigyMIDPaymentType.ID() { AssertigyMIDID = res.AssertigyMIDID.Value, PaymentTypeID = res.PaymentTypeID.Value };
            }

            return res;
        }
    }
}