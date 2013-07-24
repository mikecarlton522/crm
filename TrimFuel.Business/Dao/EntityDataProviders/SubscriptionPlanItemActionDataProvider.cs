using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SubscriptionPlanItemActionDataProvider : EntityDataProvider<SubscriptionPlanItemAction>
    {
        private const string INSERT_COMMAND = "INSERT INTO SubscriptionPlanItemAction(SubscriptionPlanItemID, SubscriptionActionTypeID, SubscriptionActionAmount, SubscriptionActionProductCode, SubscriptionActionQuantity) VALUES(@SubscriptionPlanItemID, @SubscriptionActionTypeID, @SubscriptionActionAmount, @SubscriptionActionProductCode, @SubscriptionActionQuantity); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SubscriptionPlanItemAction SET SubscriptionPlanItemID=@SubscriptionPlanItemID, SubscriptionActionTypeID=@SubscriptionActionTypeID, SubscriptionActionAmount=@SubscriptionActionAmount, SubscriptionActionProductCode=@SubscriptionActionProductCode, SubscriptionActionQuantity=@SubscriptionActionQuantity WHERE SubscriptionPlanItemActionID=@SubscriptionPlanItemActionID;";
        private const string SELECT_COMMAND = "SELECT * FROM SubscriptionPlanItemAction WHERE SubscriptionPlanItemActionID=@SubscriptionPlanItemActionID;";

        public override void Save(SubscriptionPlanItemAction entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SubscriptionPlanItemActionID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SubscriptionPlanItemActionID", MySqlDbType.Int32).Value = entity.SubscriptionPlanItemActionID;
            }

            cmd.Parameters.Add("@SubscriptionPlanItemID", MySqlDbType.Int32).Value = entity.SubscriptionPlanItemID;
            cmd.Parameters.Add("@SubscriptionActionTypeID", MySqlDbType.Int32).Value = entity.SubscriptionActionTypeID;
            cmd.Parameters.Add("@SubscriptionActionAmount", MySqlDbType.Decimal).Value = entity.SubscriptionActionAmount;
            cmd.Parameters.Add("@SubscriptionActionProductCode", MySqlDbType.VarChar).Value = entity.SubscriptionActionProductCode;
            cmd.Parameters.Add("@SubscriptionActionQuantity", MySqlDbType.Int32).Value = entity.SubscriptionActionQuantity;

            if (entity.SubscriptionPlanItemActionID == null)
            {
                entity.SubscriptionPlanItemActionID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SubscriptionPlanItemAction({0}) was not found in database.", entity.SubscriptionPlanItemActionID));
                }
            }
        }

        public override SubscriptionPlanItemAction Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SubscriptionPlanItemActionID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SubscriptionPlanItemAction Load(DataRow row)
        {
            SubscriptionPlanItemAction res = new SubscriptionPlanItemAction();

            if (!(row["SubscriptionPlanItemActionID"] is DBNull))
                res.SubscriptionPlanItemActionID = Convert.ToInt32(row["SubscriptionPlanItemActionID"]);
            if (!(row["SubscriptionPlanItemID"] is DBNull))
                res.SubscriptionPlanItemID = Convert.ToInt32(row["SubscriptionPlanItemID"]);
            if (!(row["SubscriptionActionTypeID"] is DBNull))
                res.SubscriptionActionTypeID = Convert.ToInt32(row["SubscriptionActionTypeID"]);
            if (!(row["SubscriptionActionAmount"] is DBNull))
                res.SubscriptionActionAmount = Convert.ToDecimal(row["SubscriptionActionAmount"]);
            if (!(row["SubscriptionActionProductCode"] is DBNull))
                res.SubscriptionActionProductCode = Convert.ToString(row["SubscriptionActionProductCode"]);
            if (!(row["SubscriptionActionQuantity"] is DBNull))
                res.SubscriptionActionQuantity = Convert.ToInt32(row["SubscriptionActionQuantity"]);

            return res;
        }
    }
}
