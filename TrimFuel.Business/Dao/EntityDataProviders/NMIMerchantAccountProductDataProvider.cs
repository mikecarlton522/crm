using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class NMIMerchantAccountProductDataProvider : EntityDataProvider<NMIMerchantAccountProduct>
    {
        string SELECT_COMMAND = "SELECT * From NMIMerchantAccountProduct WHERE MerchantAccountProductID=@MerchantAccountProductID";
        string UPDATE_COMMAND = "UPDATE NMIMerchantAccountProduct SET UseForTrial=@UseForTrial, AssertigyMIDID=@AssertigyMIDID, ProductID=@ProductID, Percentage=@Percentage, UseForRebill=@UseForRebill, OnlyRefundCredit=@OnlyRefundCredit, QueueRebills=@QueueRebills, RolloverAssertigyMIDID=@RolloverAssertigyMIDID WHERE MerchantAccountProductID=@MerchantAccountProductID";
        string INSERT_COMMAND = "INSERT INTO NMIMerchantAccountProduct (AssertigyMIDID, ProductID, Percentage, UseForRebill, OnlyRefundCredit, UseForTrial, QueueRebills, RolloverAssertigyMIDID) VALUES(@AssertigyMIDID, @ProductID, @Percentage, @UseForRebill, @OnlyRefundCredit, @UseForTrial, @QueueRebills, @RolloverAssertigyMIDID); SELECT @@IDENTITY;";

        public override void Save(NMIMerchantAccountProduct entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.MerchantAccountProductID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@MerchantAccountProductID", MySqlDbType.Int32).Value = entity.MerchantAccountProductID;
            }

            cmd.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = entity.AssertigyMIDID;
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@Percentage", MySqlDbType.Int32).Value = entity.Percentage;
            cmd.Parameters.Add("@UseForRebill", MySqlDbType.Bit).Value = entity.UseForRebill;
            cmd.Parameters.Add("@OnlyRefundCredit", MySqlDbType.Bit).Value = entity.OnlyRefundCredit;
            cmd.Parameters.Add("@UseForTrial", MySqlDbType.Bit).Value = entity.UseForTrial;
            cmd.Parameters.Add("@QueueRebills", MySqlDbType.Bit).Value = entity.QueueRebills;
            cmd.Parameters.Add("@RolloverAssertigyMIDID", MySqlDbType.Int32).Value = entity.RolloverAssertigyMIDID;

            if (entity.MerchantAccountProductID == null)
            {
                entity.MerchantAccountProductID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("MerchantAccountProduct ({0}) was not found in database.", entity.MerchantAccountProductID));
                }
            }
        }

        public override NMIMerchantAccountProduct Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@MerchantAccountProductID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override NMIMerchantAccountProduct Load(System.Data.DataRow row)
        {
            NMIMerchantAccountProduct res = new NMIMerchantAccountProduct();

            if (!(row["MerchantAccountProductID"] is DBNull))
                res.MerchantAccountProductID = Convert.ToInt32(row["MerchantAccountProductID"]);
            if (!(row["AssertigyMIDID"] is DBNull))
                res.AssertigyMIDID = Convert.ToInt32(row["AssertigyMIDID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["Percentage"] is DBNull))
                res.Percentage = Convert.ToInt32(row["Percentage"]);
            if (!(row["UseForRebill"] is DBNull))
                res.UseForRebill = Convert.ToBoolean(row["UseForRebill"]);
            if (!(row["OnlyRefundCredit"] is DBNull))
                res.OnlyRefundCredit = Convert.ToBoolean(row["OnlyRefundCredit"]);
            if (!(row["UseForTrial"] is DBNull))
                res.UseForTrial = Convert.ToBoolean(row["UseForTrial"]);
            if (!(row["QueueRebills"] is DBNull))
                res.QueueRebills = Convert.ToBoolean(row["QueueRebills"]);
            if (!(row["RolloverAssertigyMIDID"] is DBNull))
                res.RolloverAssertigyMIDID = Convert.ToInt32(row["RolloverAssertigyMIDID"]);

            return res;
        }
    }
}
