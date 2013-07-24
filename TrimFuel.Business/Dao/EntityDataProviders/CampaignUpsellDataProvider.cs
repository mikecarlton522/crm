using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CampaignUpsellDataProvider : EntityDataProvider<CampaignUpsell>
    {
        private const string INSERT_COMMAND = "INSERT INTO CampaignUpsell(CampaingID, CampaignPageID, Price, ProductCode, Quantity, SubscriptionID, RecurringPlanID) VALUES(@CampaignID, @CampaignPageID, @Price, @ProductCode, @Quantity, @SubscriptionID, @RecurringPlanID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE CampaignUpsell SET CampaingID=@CampaignID, CampaignPageID=@CampaignPageID, Price=@Price, ProductCode=@ProductCode, Quantity=@Quantity, SubscriptionID=@SubscriptionID, RecurringPlanID=@RecurringPlanID WHERE CampaignUpsellID=@CampaignUpsellID;";
        private const string SELECT_COMMAND = "SELECT * FROM CampaignUpsell WHERE CampaignUpsellID=@CampaignUpsellID;";

        public override void Save(CampaignUpsell entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.CampaignUpsellID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@CampaignUpsellID", MySqlDbType.Int32).Value = entity.CampaignUpsellID;
            }

            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int64).Value = entity.CampaignID;
            cmd.Parameters.Add("@CampaignPageID", MySqlDbType.Int64).Value = entity.CampaignPageID;
            cmd.Parameters.Add("@Price", MySqlDbType.Decimal).Value = entity.Price;
            cmd.Parameters.Add("@ProductCode", MySqlDbType.VarChar).Value = entity.ProductCode;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;
            cmd.Parameters.Add("@SubscriptionID", MySqlDbType.Int32).Value = entity.SubscriptionID;
            cmd.Parameters.Add("@RecurringPlanID", MySqlDbType.Int32).Value = entity.RecurringPlanID;

            if (entity.CampaignUpsellID == null)
            {
                entity.CampaignUpsellID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("CampaignUpsell ({0}) was not found in database.", entity.CampaignUpsellID));
                }
            }
        }

        public override CampaignUpsell Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CampaignUpsellID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override CampaignUpsell Load(System.Data.DataRow row)
        {
            CampaignUpsell res = new CampaignUpsell();

            if (!(row["CampaignUpsellID"] is DBNull))
                res.CampaignUpsellID = Convert.ToInt32(row["CampaignUpsellID"]);
            if (!(row["CampaingID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaingID"]);
            if (!(row["CampaignPageID"] is DBNull))
                res.CampaignPageID = Convert.ToInt32(row["CampaignPageID"]);
            if (!(row["Price"] is DBNull))
                res.Price = Convert.ToDecimal(row["Price"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);
            if (!(row["SubscriptionID"] is DBNull))
                res.SubscriptionID = Convert.ToInt32(row["SubscriptionID"]);
            if (!(row["RecurringPlanID"] is DBNull))
                res.RecurringPlanID = Convert.ToInt32(row["RecurringPlanID"]);

            return res;
        }
    }
}
