using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AssertigyMIDSettingsDataProvider : EntityDataProvider<AssertigyMIDSettings>
    {
        private const string INSERT_COMMAND = "INSERT INTO AssertigyMIDSettings(AssertigyMIDID, ChargebackFee, ChargebackRepresentationFee, ChargebackRepresentationFeeRetail, TransactionFee, DiscountRate, GatewayFeeRetail, GatewayFee) VALUES(@AssertigyMIDID, @ChargebackFee, @ChargebackRepresentationFee, @ChargebackRepresentationFeeRetail, @TransactionFee, @DiscountRate, @GatewayFeeRetail, @GatewayFee)";
        private const string UPDATE_COMMAND = "UPDATE AssertigyMIDSettings SET ChargebackFee=@ChargebackFee, ChargebackRepresentationFee=@ChargebackRepresentationFee, ChargebackRepresentationFeeRetail=@ChargebackRepresentationFeeRetail, TransactionFee=@TransactionFee, DiscountRate=@DiscountRate, GatewayFeeRetail=@GatewayFeeRetail, GatewayFee=@GatewayFee WHERE AssertigyMIDID=@AssertigyMIDID;";
        private const string SELECT_COMMAND = "SELECT * FROM AssertigyMIDSettings WHERE AssertigyMIDID=@AssertigyMIDID;";

        public override void Save(AssertigyMIDSettings entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.AssertigyMIDSettingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            cmd.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = entity.AssertigyMIDID;
            cmd.Parameters.Add("@ChargebackFee", MySqlDbType.Double).Value = entity.ChargebackFee;
            cmd.Parameters.Add("@ChargebackRepresentationFee", MySqlDbType.Double).Value = entity.ChargebackRepresentationFee;
            cmd.Parameters.Add("@ChargebackRepresentationFeeRetail", MySqlDbType.Double).Value = entity.ChargebackRepresentationFeeRetail;
            cmd.Parameters.Add("@TransactionFee", MySqlDbType.Double).Value = entity.TransactionFee;
            cmd.Parameters.Add("@DiscountRate", MySqlDbType.Double).Value = entity.DiscountRate;

            cmd.Parameters.Add("@GatewayFee", MySqlDbType.Double).Value = entity.GatewayFee;
            cmd.Parameters.Add("@GatewayFeeRetail", MySqlDbType.Double).Value = entity.GatewayFeeRetail;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("AssertigyMIDSettings ({0}) was not found in database.", entity.AssertigyMIDID));
            }
            entity.AssertigyMIDSettingID = entity.AssertigyMIDID;

        }

        public override AssertigyMIDSettings Load(object key, IMySqlCommandCreater cmdCreater)
        {
            var cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = key;
            return Load(cmd).FirstOrDefault();
        }

        public override AssertigyMIDSettings Load(System.Data.DataRow row)
        {
            AssertigyMIDSettings res = new AssertigyMIDSettings();

            if (!(row["AssertigyMIDID"] is DBNull))
                res.AssertigyMIDID = Convert.ToInt32(row["AssertigyMIDID"]);
            if (!(row["ChargebackFee"] is DBNull))
                res.ChargebackFee = Convert.ToDouble(row["ChargebackFee"]);
            if (!(row["ChargebackRepresentationFee"] is DBNull))
                res.ChargebackRepresentationFee = Convert.ToDouble(row["ChargebackRepresentationFee"]);
            if (!(row["ChargebackRepresentationFeeRetail"] is DBNull))
                res.ChargebackRepresentationFeeRetail = Convert.ToDouble(row["ChargebackRepresentationFeeRetail"]);
            if (!(row["TransactionFee"] is DBNull))
                res.TransactionFee = Convert.ToDouble(row["TransactionFee"]);
            if (!(row["DiscountRate"] is DBNull))
                res.DiscountRate = Convert.ToDouble(row["DiscountRate"]);

            if (!(row["GatewayFee"] is DBNull))
                res.GatewayFee = Convert.ToDouble(row["GatewayFee"]);
            if (!(row["GatewayFeeRetail"] is DBNull))
                res.GatewayFeeRetail = Convert.ToDouble(row["GatewayFeeRetail"]);

            res.AssertigyMIDSettingID = res.AssertigyMIDID;

            return res;
        }
    }
}
