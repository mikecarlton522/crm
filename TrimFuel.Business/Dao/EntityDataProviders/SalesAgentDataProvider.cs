using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SalesAgentProvider : EntityDataProvider<SalesAgent>
    {
        private const string INSERT_COMMAND = "INSERT INTO SalesAgent (Name, IsActive, TransactionFeeFixed, TransactionFeePercentage, ShipmentFee, ExtraSKUShipmentFee, ChargebackFee, CallCenterFeePerMinute, CallCenterFeePerCall, MonthlyCRMFee, AdminID, Commission, CommissionMerchant) VALUES (@Name, @IsActive, @TransactionFeeFixed, @TransactionFeePercentage, @ShipmentFee, @ExtraSKUShipmentFee, @ChargebackFee, @CallCenterFeePerMinute, @CallCenterFeePerCall, @MonthlyCRMFee, @AdminID, @Commission, @CommissionMerchant); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SalesAgent SET Name = @Name, IsActive = @IsActive, TransactionFeeFixed = @TransactionFeeFixed ,TransactionFeePercentage = @TransactionFeePercentage, ShipmentFee = @ShipmentFee, ExtraSKUShipmentFee = @ExtraSKUShipmentFee, ChargebackFee = @ChargebackFee, CallCenterFeePerMinute = @CallCenterFeePerMinute, CallCenterFeePerCall = @CallCenterFeePerCall, MonthlyCRMFee = @MonthlyCRMFee, AdminID = @AdminID, Commission=@Commission, CommissionMerchant=@CommissionMerchant WHERE SalesAgentID = @SalesAgentID;";
        private const string SELECT_COMMAND = "SELECT * FROM SalesAgent WHERE SalesAgentID = @SalesAgentID;";

        public override void Save(SalesAgent entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SalesAgentID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SalesAgentID", MySqlDbType.Int32).Value = entity.SalesAgentID;
            }

            cmd.Parameters.Add("@Name", MySqlDbType.String).Value = entity.Name;
            cmd.Parameters.Add("@IsActive", MySqlDbType.Bit).Value = entity.IsActive;
            cmd.Parameters.Add("@TransactionFeeFixed", MySqlDbType.Decimal).Value = entity.TransactionFeeFixed;
            cmd.Parameters.Add("@TransactionFeePercentage", MySqlDbType.Int32).Value = entity.TransactionFeePercentage;
            cmd.Parameters.Add("@ShipmentFee", MySqlDbType.Decimal).Value = entity.ShipmentFee;
            cmd.Parameters.Add("@ExtraSKUShipmentFee", MySqlDbType.Decimal).Value = entity.ExtraSKUShipmentFee;
            cmd.Parameters.Add("@ChargebackFee", MySqlDbType.Decimal).Value = entity.ChargebackFee;
            cmd.Parameters.Add("@CallCenterFeePerMinute", MySqlDbType.Decimal).Value = entity.CallCenterFeePerMinute;
            cmd.Parameters.Add("@CallCenterFeePerCall", MySqlDbType.Decimal).Value = entity.CallCenterFeePerCall;
            cmd.Parameters.Add("@MonthlyCRMFee", MySqlDbType.Decimal).Value = entity.MonthlyCRMFee;
            cmd.Parameters.Add("@Commission", MySqlDbType.Int32).Value = entity.Commission;
            cmd.Parameters.Add("@CommissionMerchant", MySqlDbType.Int32).Value = entity.CommissionMerchant;
            cmd.Parameters.Add("@AdminID", MySqlDbType.Int32).Value = entity.AdminID;

            if (entity.SalesAgentID == null)
            {
                entity.SalesAgentID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SalesAgent({0}) was not found in database.", entity.SalesAgentID));
                }
            }
        }

        public override SalesAgent Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SalesAgentID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SalesAgent Load(DataRow row)
        {
            SalesAgent res = new SalesAgent();

            if (!(row["SalesAgentID"] is DBNull))
                res.SalesAgentID = Convert.ToInt32(row["SalesAgentID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);
            if (!(row["IsActive"] is DBNull))
                res.IsActive = Convert.ToBoolean(row["IsActive"]);
            if (!(row["TransactionFeeFixed"] is DBNull))
                res.TransactionFeeFixed = Convert.ToDecimal(row["TransactionFeeFixed"]);
            if (!(row["TransactionFeePercentage"] is DBNull))
                res.TransactionFeePercentage = Convert.ToInt32(row["TransactionFeePercentage"]);
            if (!(row["ShipmentFee"] is DBNull))
                res.ShipmentFee = Convert.ToDecimal(row["ShipmentFee"]);
            if (!(row["ExtraSKUShipmentFee"] is DBNull))
                res.ExtraSKUShipmentFee = Convert.ToDecimal(row["ExtraSKUShipmentFee"]);
            if (!(row["ChargebackFee"] is DBNull))
                res.ChargebackFee = Convert.ToDecimal(row["ChargebackFee"]);
            if (!(row["CallCenterFeePerMinute"] is DBNull))
                res.CallCenterFeePerMinute = Convert.ToDecimal(row["CallCenterFeePerMinute"]);
            if (!(row["CallCenterFeePerCall"] is DBNull))
                res.CallCenterFeePerCall = Convert.ToDecimal(row["CallCenterFeePerCall"]);
            if (!(row["MonthlyCRMFee"] is DBNull))
                res.MonthlyCRMFee = Convert.ToDecimal(row["MonthlyCRMFee"]);
            if (!(row["Commission"] is DBNull))
                res.Commission = Convert.ToInt32(row["Commission"]);
            if (!(row["CommissionMerchant"] is DBNull))
                res.CommissionMerchant = Convert.ToInt32(row["CommissionMerchant"]);
            if (!(row["AdminID"] is DBNull))
                res.AdminID = Convert.ToInt32(row["AdminID"]);
            return res;
        }
    }
}
