using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ShipperSettingsDataProvider : EntityDataProvider<TPClientShipperSettings>
    {
        private const string INSERT_COMMAND = @"INSERT INTO ShipperSettings(ShipperID, CompanyName, CompanyPhone, CompanyEmail, ShipmentFee, SetupFee, ReturnsFee, KittingFee, CustomDevelopmentFee, SpecialLaborFee, ShipmentFeeRetail, SetupFeeRetail, ReturnsFeeRetail, KittingFeeRetail, CustomDevelopmentFeeRetail, SpecialLaborFeeRetail, ShipmentSKUFee, ShipmentSKUFeeRetail) 
                                                VALUES(@ShipperID, @CompanyName, @CompanyPhone, @CompanyEmail, @ShipmentFee, @SetupFee, @ReturnsFee, @KittingFee, @CustomDevelopmentFee, @SpecialLaborFee, @ShipmentFeeRetail, @SetupFeeRetail, @ReturnsFeeRetail, @KittingFeeRetail, @CustomDevelopmentFeeRetail, @SpecialLaborFeeRetail, @ShipmentSKUFee, @ShipmentSKUFeeRetail); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = @"UPDATE ShipperSettings SET ShipperID=@ShipperID, CompanyName=@CompanyName, CompanyPhone=@CompanyPhone, CompanyEmail=@CompanyEmail, 
                                                ShipmentFee=@ShipmentFee, SetupFee=@SetupFee, ReturnsFee=@ReturnsFee, KittingFee=@KittingFee, CustomDevelopmentFee=@CustomDevelopmentFee, SpecialLaborFee=@SpecialLaborFee,
                                                ShipmentFeeRetail=@ShipmentFeeRetail, SetupFeeRetail=@SetupFeeRetail, ReturnsFeeRetail=@ReturnsFeeRetail, KittingFeeRetail=@KittingFeeRetail, CustomDevelopmentFeeRetail=@CustomDevelopmentFeeRetail, SpecialLaborFeeRetail=@SpecialLaborFeeRetail, ShipmentSKUFee=@ShipmentSKUFee, ShipmentSKUFeeRetail=@ShipmentSKUFeeRetail WHERE ShipperSettingID=@ShipperSettingID;";
        private const string SELECT_COMMAND = "SELECT * FROM ShipperSettings WHERE ShipperSettingID=@ShipperSettingID;";

        public override void Save(TPClientShipperSettings entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if ((entity.ShipperSettingID == null) || (entity.ShipperSettingID.Value == 0))
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ShipperSettingID", MySqlDbType.Int32).Value = entity.ShipperSettingID;
            }

            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int64).Value = entity.ShipperID;
            cmd.Parameters.Add("@CompanyName", MySqlDbType.VarChar).Value = entity.CompanyName;
            cmd.Parameters.Add("@CompanyPhone", MySqlDbType.VarChar).Value = entity.CompanyPhone;
            cmd.Parameters.Add("@CompanyEmail", MySqlDbType.VarChar).Value = entity.CompanyEmail;
            cmd.Parameters.Add("@SetupFee", MySqlDbType.Double).Value = entity.SetupFee;
            cmd.Parameters.Add("@ShipmentFee", MySqlDbType.Double).Value = entity.ShipmentFee;
            cmd.Parameters.Add("@ReturnsFee", MySqlDbType.Double).Value = entity.ReturnsFee;
            cmd.Parameters.Add("@KittingFee", MySqlDbType.Double).Value = entity.KittingAndAsemblyFee;
            cmd.Parameters.Add("@CustomDevelopmentFee", MySqlDbType.Double).Value = entity.CustomDevelopmentFee;
            cmd.Parameters.Add("@SpecialLaborFee", MySqlDbType.Double).Value = entity.SpecialLaborFee;
            cmd.Parameters.Add("@SetupFeeRetail", MySqlDbType.Double).Value = entity.SetupFeeRetail;
            cmd.Parameters.Add("@ShipmentFeeRetail", MySqlDbType.Double).Value = entity.ShipmentFeeRetail;
            cmd.Parameters.Add("@ReturnsFeeRetail", MySqlDbType.Double).Value = entity.ReturnsFeeRetail;
            cmd.Parameters.Add("@KittingFeeRetail", MySqlDbType.Double).Value = entity.KittingAndAsemblyFeeRetail;
            cmd.Parameters.Add("@CustomDevelopmentFeeRetail", MySqlDbType.Double).Value = entity.CustomDevelopmentFeeRetail;
            cmd.Parameters.Add("@SpecialLaborFeeRetail", MySqlDbType.Double).Value = entity.SpecialLaborFeeRetail;
            cmd.Parameters.Add("@ShipmentSKUFee", MySqlDbType.Double).Value = entity.ShipmentSKUFee;
            cmd.Parameters.Add("@ShipmentSKUFeeRetail", MySqlDbType.Double).Value = entity.ShipmentSKUFeeRetail;
            if ((entity.ShipperSettingID == null) || (entity.ShipperSettingID.Value == 0))
            {
                entity.ShipperSettingID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ShipperSetting({0}) was not found in database.", entity.ShipperSettingID));
                }
            }
        }

        public override TPClientShipperSettings Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ShipperSettingID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override TPClientShipperSettings Load(System.Data.DataRow row)
        {
            TPClientShipperSettings res = new TPClientShipperSettings();

            if (!(row["ShipperSettingID"] is DBNull))
                res.ShipperSettingID = Convert.ToInt32(row["ShipperSettingID"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            if (!(row["CompanyName"] is DBNull))
                res.CompanyName = Convert.ToString(row["CompanyName"]);
            if (!(row["CompanyPhone"] is DBNull))
                res.CompanyPhone = Convert.ToString(row["CompanyPhone"]);
            if (!(row["CompanyEmail"] is DBNull))
                res.CompanyEmail = Convert.ToString(row["CompanyEmail"]);

            if (!(row["ShipmentFee"] is DBNull))
                res.ShipmentFee = Convert.ToDouble(row["ShipmentFee"]);
            if (!(row["SetupFee"] is DBNull))
                res.SetupFee = Convert.ToDouble(row["SetupFee"]);
            if (!(row["ReturnsFee"] is DBNull))
                res.ReturnsFee = Convert.ToDouble(row["ReturnsFee"]);
            if (!(row["KittingFee"] is DBNull))
                res.KittingAndAsemblyFee = Convert.ToDouble(row["KittingFee"]);
            if (!(row["CustomDevelopmentFee"] is DBNull))
                res.CustomDevelopmentFee = Convert.ToDouble(row["CustomDevelopmentFee"]);
            if (!(row["SpecialLaborFee"] is DBNull))
                res.SpecialLaborFee = Convert.ToDouble(row["SpecialLaborFee"]);
            if (!(row["ShipmentSKUFee"] is DBNull))
                res.ShipmentSKUFee = Convert.ToDouble(row["ShipmentSKUFee"]);

            if (!(row["ShipmentFeeRetail"] is DBNull))
                res.ShipmentFeeRetail = Convert.ToDouble(row["ShipmentFeeRetail"]);
            if (!(row["SetupFeeRetail"] is DBNull))
                res.SetupFeeRetail = Convert.ToDouble(row["SetupFeeRetail"]);
            if (!(row["ReturnsFeeRetail"] is DBNull))
                res.ReturnsFeeRetail = Convert.ToDouble(row["ReturnsFeeRetail"]);
            if (!(row["KittingFeeRetail"] is DBNull))
                res.KittingAndAsemblyFeeRetail = Convert.ToDouble(row["KittingFeeRetail"]);
            if (!(row["CustomDevelopmentFeeRetail"] is DBNull))
                res.CustomDevelopmentFeeRetail = Convert.ToDouble(row["CustomDevelopmentFeeRetail"]);
            if (!(row["SpecialLaborFeeRetail"] is DBNull))
                res.SpecialLaborFeeRetail = Convert.ToDouble(row["SpecialLaborFeeRetail"]);
            if (!(row["ShipmentSKUFeeRetail"] is DBNull))
                res.ShipmentSKUFeeRetail = Convert.ToDouble(row["ShipmentSKUFeeRetail"]);

            return res;
        }
    }
}
