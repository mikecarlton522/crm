using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AssertigyMIDDataProvider : EntityDataProvider<AssertigyMID>
    {
        private const string INSERT_COMMAND = @"INSERT INTO AssertigyMID(MID, DisplayName, ParentMID, MonthlyCap, ProcessingRate, ReserveAccountRate, ChargebackFee, TransactionFee, Visible, GatewayName, MIDCategoryID, Deleted) 
                                              VALUES(@MID, @DisplayName, @ParentMID, @MonthlyCap, @ProcessingRate, @ReserveAccountRate, @ChargebackFee, @TransactionFee, @Visible, @GatewayName, @MIDCategoryID, @Deleted); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE AssertigyMID SET MID=@MID, DisplayName=@DisplayName, ParentMID=@ParentMID, MonthlyCap=@MonthlyCap, ProcessingRate=@ProcessingRate, ReserveAccountRate=@ReserveAccountRate, ChargebackFee=@ChargebackFee, TransactionFee=@TransactionFee, Visible=@Visible, GatewayName=@GatewayName, MIDCategoryID=@MIDCategoryID, Deleted=@Deleted WHERE AssertigyMIDID=@AssertigyMIDID;";
        private const string SELECT_COMMAND = "SELECT * FROM AssertigyMID WHERE AssertigyMIDID = @AssertigyMIDID;";

        public override void Save(AssertigyMID entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.AssertigyMIDID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = entity.AssertigyMIDID;
            }

            cmd.Parameters.Add("@MID", MySqlDbType.VarChar).Value = entity.MID;
            cmd.Parameters.Add("@DisplayName", MySqlDbType.VarChar).Value = entity.DisplayName;
            cmd.Parameters.Add("@ParentMID", MySqlDbType.Int32).Value = entity.ParentMID;
            cmd.Parameters.Add("@MonthlyCap", MySqlDbType.Decimal).Value = entity.MonthlyCap;
            cmd.Parameters.Add("@ProcessingRate", MySqlDbType.Double).Value = entity.ProcessingRate;
            cmd.Parameters.Add("@ReserveAccountRate", MySqlDbType.Double).Value = entity.ReserveAccountRate;
            cmd.Parameters.Add("@ChargebackFee", MySqlDbType.Double).Value = entity.ChargebackFee;
            cmd.Parameters.Add("@TransactionFee", MySqlDbType.Double).Value = entity.TransactionFee;
            cmd.Parameters.Add("@Visible", MySqlDbType.Bit).Value = entity.Visible;
            cmd.Parameters.Add("@Deleted", MySqlDbType.Bit).Value = entity.Deleted;
            cmd.Parameters.Add("@GatewayName", MySqlDbType.VarChar).Value = entity.GatewayName;
            cmd.Parameters.Add("@MIDCategoryID", MySqlDbType.Int32).Value = entity.MIDCategoryID;

            if (entity.AssertigyMIDID == null)
            {
                entity.AssertigyMIDID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("AssertigyMID ({0}) was not found in database.", entity.AssertigyMIDID));
                }
            }
        }

        public override AssertigyMID Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override AssertigyMID Load(DataRow row)
        {
            AssertigyMID res = new AssertigyMID();

            if (!(row["AssertigyMIDID"] is DBNull))
                res.AssertigyMIDID = Convert.ToInt32(row["AssertigyMIDID"]);
            if (!(row["MID"] is DBNull))
                res.MID = Convert.ToString(row["MID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["ParentMID"] is DBNull))
                res.ParentMID = Convert.ToInt32(row["ParentMID"]);
            if (!(row["MonthlyCap"] is DBNull))
                res.MonthlyCap = Convert.ToDecimal(row["MonthlyCap"]);
            if (!(row["ProcessingRate"] is DBNull))
                res.ProcessingRate = Convert.ToDouble(row["ProcessingRate"]);
            if (!(row["ReserveAccountRate"] is DBNull))
                res.ReserveAccountRate = Convert.ToDouble(row["ReserveAccountRate"]);
            if (!(row["ChargebackFee"] is DBNull))
                res.ChargebackFee = Convert.ToDouble(row["ChargebackFee"]);
            if (!(row["TransactionFee"] is DBNull))
                res.TransactionFee = Convert.ToDouble(row["TransactionFee"]);
            if (!(row["Visible"] is DBNull))
                res.Visible = Convert.ToBoolean(row["Visible"]);
            if (!(row["GatewayName"] is DBNull))
                res.GatewayName = Convert.ToString(row["GatewayName"]);
            if (!(row["MIDCategoryID"] is DBNull))
                res.MIDCategoryID = Convert.ToInt32(row["MIDCategoryID"]);
            if (!(row["Deleted"] is DBNull))
                res.Deleted = Convert.ToBoolean(row["Deleted"]);

            return res;
        }
    }
}
