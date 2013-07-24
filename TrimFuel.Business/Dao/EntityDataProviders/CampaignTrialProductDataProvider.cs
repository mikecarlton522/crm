using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CampaignTrialProductDataProvider : EntityDataProvider<CampaignTrialProduct>
    {
        private const string INSERT_COMMAND = "INSERT INTO CampaignTrialProduct(CampaignRecurringPlanID, ProductSKU, Quantity) VALUES(@CampaignRecurringPlanID, @ProductSKU, @Quantity); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE CampaignTrialProduct SET CampaignRecurringPlanID=@CampaignRecurringPlanID, ProductSKU=@ProductSKU, Quantity=@Quantity WHERE CampaignTrialProductID=@CampaignTrialProductID;";
        private const string SELECT_COMMAND = "SELECT * FROM CampaignTrialProduct WHERE CampaignTrialProductID=@CampaignTrialProductID;";

        public override void Save(CampaignTrialProduct entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.CampaignTrialProductID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@CampaignTrialProductID", MySqlDbType.Int32).Value = entity.CampaignTrialProductID;
            }

            cmd.Parameters.Add("@CampaignRecurringPlanID", MySqlDbType.Int32).Value = entity.CampaignRecurringPlanID;
            cmd.Parameters.Add("@ProductSKU", MySqlDbType.VarChar).Value = entity.ProductSKU;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;


            if (entity.CampaignTrialProductID == null)
            {
                entity.CampaignTrialProductID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("CampaignTrialProduct({0}) was not found in database.", entity.CampaignTrialProductID));
                }
            }
        }

        public override CampaignTrialProduct Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@CampaignTrialProductID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override CampaignTrialProduct Load(DataRow row)
        {
            CampaignTrialProduct res = new CampaignTrialProduct();

            if (!(row["CampaignTrialProductID"] is DBNull))
                res.CampaignTrialProductID = Convert.ToInt32(row["CampaignTrialProductID"]);
            if (!(row["CampaignRecurringPlanID"] is DBNull))
                res.CampaignRecurringPlanID = Convert.ToInt32(row["CampaignRecurringPlanID"]);
            if (!(row["ProductSKU"] is DBNull))
                res.ProductSKU = Convert.ToString(row["ProductSKU"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);

            return res;
        }
    }
}
