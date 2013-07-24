using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class PromoGiftDataProvider : EntityDataProvider<PromoGift>
    {
        private const string INSERT_COMMAND = "INSERT INTO PromoGift(PromoGiftTypeID, GiftNumber, Details, AssignedSaleID, StoreID, Value, RemainingValue) VALUES(@PromoGiftTypeID, @GiftNumber, @Details, @AssignedSaleID, @StoreID, @Value, @RemainingValue); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE PromoGift SET PromoGiftTypeID=@PromoGiftTypeID, GiftNumber=@GiftNumber, Details=@Details, AssignedSaleID=@AssignedSaleID, StoreID=@StoreID, Value=@Value, RemainingValue=@RemainingValue WHERE RefererCommissionID=@PromoGiftID;";
        private const string SELECT_COMMAND = "SELECT * FROM PromoGift WHERE PromoGiftID=@PromoGiftID;";

        public override void Save(PromoGift entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.PromoGiftID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@PromoGiftID", MySqlDbType.Int64).Value = entity.PromoGiftID;
            }

            cmd.Parameters.Add("@PromoGiftTypeID", MySqlDbType.Int32).Value = entity.PromoGiftTypeID;
            cmd.Parameters.Add("@GiftNumber", MySqlDbType.VarChar).Value = entity.GiftNumber;
            cmd.Parameters.Add("@Details", MySqlDbType.VarChar).Value = entity.Details;
            cmd.Parameters.Add("@AssignedSaleID", MySqlDbType.Int64).Value = entity.AssignedSaleID;
            cmd.Parameters.Add("@StoreID", MySqlDbType.Int32).Value = entity.StoreID;
            cmd.Parameters.Add("@Value", MySqlDbType.Decimal).Value = entity.Value;
            cmd.Parameters.Add("@RemainingValue", MySqlDbType.Decimal).Value = entity.RemainingValue;

            if (entity.PromoGiftID == null)
            {
                entity.PromoGiftID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("PromoGift({0}) was not found in database.", entity.PromoGiftID));
                }
            }
        }

        public override PromoGift Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@PromoGiftID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override PromoGift Load(DataRow row)
        {
            PromoGift res = new PromoGift();

            if (!(row["PromoGiftID"] is DBNull))
                res.PromoGiftID = Convert.ToInt64(row["PromoGiftID"]);
            if (!(row["PromoGiftTypeID"] is DBNull))
                res.PromoGiftTypeID = Convert.ToInt32(row["PromoGiftTypeID"]);
            if (!(row["GiftNumber"] is DBNull))
                res.GiftNumber = Convert.ToString(row["GiftNumber"]);
            if (!(row["Details"] is DBNull))
                res.Details = Convert.ToString(row["Details"]);
            if (!(row["AssignedSaleID"] is DBNull))
                res.AssignedSaleID = Convert.ToInt64(row["AssignedSaleID"]);
            if (!(row["StoreID"] is DBNull))
                res.StoreID = Convert.ToInt32(row["StoreID"]);
            if (!(row["Value"] is DBNull))
                res.Value = Convert.ToDecimal(row["Value"]);
            if (!(row["RemainingValue"] is DBNull))
                res.RemainingValue = Convert.ToDecimal(row["RemainingValue"]);

            return res;
        }
    }
}
