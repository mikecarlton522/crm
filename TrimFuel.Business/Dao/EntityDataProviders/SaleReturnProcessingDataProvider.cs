using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SaleReturnProcessingDataProvider : EntityDataProvider<SaleReturnProcessing>
    {
        private const string INSERT_COMMAND = "INSERT INTO SaleReturnProcessing(SaleID, ReturnProcessingActionID, RefundAmount, NewSubscriptionID, ExtraTrialShipTypeID, UpsellTypeID, Quantity, NewRecurringPlanID) VALUES(@SaleID, @ReturnProcessingActionID, @RefundAmount, @NewSubscriptionID, @ExtraTrialShipTypeID, @UpsellTypeID, @Quantity, @NewRecurringPlanID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SaleReturnProcessing SET SaleID=@SaleID, ReturnProcessingActionID=@ReturnProcessingActionID, RefundAmount=@RefundAmount, NewSubscriptionID=@NewSubscriptionID, ExtraTrialShipTypeID=@ExtraTrialShipTypeID, UpsellTypeID=@UpsellTypeID, Quantity=@Quantity, NewRecurringPlanID=@NewRecurringPlanID WHERE SaleReturnProcessingID=@SaleReturnProcessingID;";
        private const string SELECT_COMMAND = "SELECT * FROM SaleReturnProcessing WHERE SaleReturnProcessingID=@SaleReturnProcessingID;";

        public override void Save(SaleReturnProcessing entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SaleReturnProcessingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SaleReturnProcessingID", MySqlDbType.Int32).Value = entity.SaleReturnProcessingID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@ReturnProcessingActionID", MySqlDbType.Int16).Value = entity.ReturnProcessingActionID;
            cmd.Parameters.Add("@RefundAmount", MySqlDbType.Decimal).Value = entity.RefundAmount;
            cmd.Parameters.Add("@NewSubscriptionID", MySqlDbType.Int32).Value = entity.NewSubscriptionID;
            cmd.Parameters.Add("@ExtraTrialShipTypeID", MySqlDbType.Int32).Value = entity.ExtraTrialShipTypeID;
            cmd.Parameters.Add("@UpsellTypeID", MySqlDbType.Int32).Value = entity.UpsellTypeID;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;
            cmd.Parameters.Add("@NewRecurringPlanID", MySqlDbType.Int32).Value = entity.NewRecurringPlanID;


            if (entity.SaleReturnProcessingID == null)
            {
                entity.SaleReturnProcessingID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SaleReturnProcessing({0}) was not found in database.", entity.SaleReturnProcessingID));
                }
            }
        }

        public override SaleReturnProcessing Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleReturnProcessingID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SaleReturnProcessing Load(DataRow row)
        {
            SaleReturnProcessing res = new SaleReturnProcessing();

            if (!(row["SaleReturnProcessingID"] is DBNull))
                res.SaleReturnProcessingID = Convert.ToInt32(row["SaleReturnProcessingID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["ReturnProcessingActionID"] is DBNull))
                res.ReturnProcessingActionID = Convert.ToInt16(row["ReturnProcessingActionID"]);
            if (!(row["RefundAmount"] is DBNull))
                res.RefundAmount = Convert.ToDecimal(row["RefundAmount"]);
            if (!(row["NewSubscriptionID"] is DBNull))
                res.NewSubscriptionID = Convert.ToInt32(row["NewSubscriptionID"]);
            if (!(row["ExtraTrialShipTypeID"] is DBNull))
                res.ExtraTrialShipTypeID = Convert.ToInt32(row["ExtraTrialShipTypeID"]);
            if (!(row["UpsellTypeID"] is DBNull))
                res.UpsellTypeID = Convert.ToInt32(row["UpsellTypeID"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);
            if (!(row["NewRecurringPlanID"] is DBNull))
                res.NewRecurringPlanID = Convert.ToInt32(row["NewRecurringPlanID"]);

            return res;
        }
    }
}
