using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ExtraTrialShipDataProvider : EntityDataProvider<ExtraTrialShip>
    {
        private const string INSERT_COMMAND = "INSERT INTO ExtraTrialShip(ProductCode, BillingID, CreateDT, Quantity, Completed, ExtraTrialShipTypeID) VALUES(@ProductCode, @BillingID, @CreateDT, @Quantity, @Completed, @ExtraTrialShipTypeID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ExtraTrialShip SET ProductCode=@ProductCode, BillingID=@BillingID, CreateDT=@CreateDT, Quantity=@Quantity, Completed=@Completed, ExtraTrialShipTypeID=@ExtraTrialShipTypeID WHERE ExtraTrialShipID=@ExtraTrialShipID;";
        private const string SELECT_COMMAND = "SELECT * FROM ExtraTrialShip WHERE ExtraTrialShipID=@ExtraTrialShipID;";

        public override void Save(ExtraTrialShip entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ExtraTrialShipID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ExtraTrialShipID", MySqlDbType.Int32).Value = entity.ExtraTrialShipID;
            }

            cmd.Parameters.Add("@ProductCode", MySqlDbType.VarChar).Value = entity.ProductCode;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;
            cmd.Parameters.Add("@Completed", MySqlDbType.Bit).Value = entity.Completed;
            cmd.Parameters.Add("@ExtraTrialShipTypeID", MySqlDbType.Int32).Value = entity.ExtraTrialShipTypeID;

            if (entity.ExtraTrialShipID == null)
            {
                entity.ExtraTrialShipID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ExtraTrialShip({0}) was not found in database.", entity.ExtraTrialShipID));
                }
            }
        }

        public override ExtraTrialShip Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ExtraTrialShipID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ExtraTrialShip Load(DataRow row)
        {
            ExtraTrialShip res = new ExtraTrialShip();

            if (!(row["ExtraTrialShipID"] is DBNull))
                res.ExtraTrialShipID = Convert.ToInt32(row["ExtraTrialShipID"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);
            if (!(row["Completed"] is DBNull))
                res.Completed = Convert.ToBoolean(row["Completed"]);
            if (!(row["ExtraTrialShipTypeID"] is DBNull))
                res.ExtraTrialShipTypeID = Convert.ToInt32(row["ExtraTrialShipTypeID"]);

            return res;
        }
    }
}
