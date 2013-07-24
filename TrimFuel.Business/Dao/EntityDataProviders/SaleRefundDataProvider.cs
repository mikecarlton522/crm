using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SaleRefundDataProvider : EntityDataProvider<SaleRefund>
    {
        private const string INSERT_COMMAND = "INSERT INTO SaleRefund(SaleID, ChargeHistoryID) VALUES(@SaleID, @ChargeHistoryID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SaleRefund SET SaleID=@SaleID, ChargeHistoryID=@ChargeHistoryID WHERE SaleRefundID=@SaleRefundID;";
        private const string SELECT_COMMAND = "SELECT * FROM SaleRefund WHERE SaleRefundID = @SaleRefundID;";

        public override void Save(SaleRefund entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SaleRefundID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SaleRefundID", MySqlDbType.Int64).Value = entity.SaleRefundID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;

            if (entity.SaleRefundID == null)
            {
                entity.SaleRefundID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SaleRefund({0}) was not found in database.", entity.SaleRefundID));
                }
            }
        }

        public override SaleRefund Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleRefundID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SaleRefund Load(DataRow row)
        {
            SaleRefund res = new SaleRefund();

            if (!(row["SaleRefundID"] is DBNull))
                res.SaleRefundID = Convert.ToInt64(row["SaleRefundID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);

            return res;
        }
    }
}
