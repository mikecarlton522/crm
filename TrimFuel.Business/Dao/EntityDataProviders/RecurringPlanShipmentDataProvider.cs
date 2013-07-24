using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class RecurringPlanShipmentDataProvider : EntityDataProvider<RecurringPlanShipment>
    {
        private const string INSERT_COMMAND = "INSERT INTO RecurringPlanShipment(RecurringPlanCycleID, ProductSKU, Quantity) VALUES(@RecurringPlanCycleID, @ProductSKU, @Quantity); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE RecurringPlanShipment SET RecurringPlanCycleID=@RecurringPlanCycleID, ProductSKU=@ProductSKU, Quantity=@Quantity WHERE RecurringPlanShipmentID=@RecurringPlanShipmentID;";
        private const string SELECT_COMMAND = "SELECT * FROM RecurringPlanShipment WHERE RecurringPlanShipmentID=@RecurringPlanShipmentID;";

        public override void Save(RecurringPlanShipment entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.RecurringPlanShipmentID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@RecurringPlanShipmentID", MySqlDbType.Int32).Value = entity.RecurringPlanShipmentID;
            }

            cmd.Parameters.Add("@RecurringPlanCycleID", MySqlDbType.Int32).Value = entity.RecurringPlanCycleID;
            cmd.Parameters.Add("@ProductSKU", MySqlDbType.VarChar).Value = entity.ProductSKU;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;

            if (entity.RecurringPlanShipmentID == null)
            {
                entity.RecurringPlanShipmentID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("RecurringPlanShipment({0}) was not found in database.", entity.RecurringPlanShipmentID));
                }
            }
        }

        public override RecurringPlanShipment Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@RecurringPlanShipmentID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override RecurringPlanShipment Load(DataRow row)
        {
            RecurringPlanShipment res = new RecurringPlanShipment();

            if (!(row["RecurringPlanShipmentID"] is DBNull))
                res.RecurringPlanShipmentID = Convert.ToInt32(row["RecurringPlanShipmentID"]);
            if (!(row["RecurringPlanCycleID"] is DBNull))
                res.RecurringPlanCycleID = Convert.ToInt32(row["RecurringPlanCycleID"]);
            if (!(row["ProductSKU"] is DBNull))
                res.ProductSKU = Convert.ToString(row["ProductSKU"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);

            return res;
        }
    }
}
