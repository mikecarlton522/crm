using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;
namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductDomainRoutingDataProvider : EntityDataProvider<ProductDomainRouting>
    {
        private const string INSERT_COMMAND = "INSERT INTO ProductDomainRouting(ProductDomainID, Percentage, CampaignID, ExtUrl, Affiliate, SubAffiliate) VALUES(@ProductDomainID, @Percentage, @CampaignID, @ExtUrl, @Affiliate, @SubAffiliate); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ProductDomainRouting SET ProductDomainID=@ProductDomainID, Percentage=@Percentage, CampaignID=@CampaignID, ExtUrl=@ExtUrl, Affiliate=@Affiliate, SubAffiliate=@SubAffiliate WHERE ProductDomainRoutingID=@ProductDomainRoutingID;";
        private const string SELECT_COMMAND = "SELECT * FROM ProductDomainRouting WHERE ProductDomainRoutingID=@ProductDomainRoutingID;";

        public override void Save(ProductDomainRouting entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ProductDomainRoutingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ProductDomainRoutingID", MySqlDbType.Int32).Value = entity.ProductDomainRoutingID;
            }

            cmd.Parameters.Add("@ProductDomainID", MySqlDbType.Int32).Value = entity.ProductDomainID;
            cmd.Parameters.Add("@Percentage", MySqlDbType.Int32).Value = entity.Percentage;
            cmd.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = entity.CampaignID;
            cmd.Parameters.Add("@ExtUrl", MySqlDbType.VarChar).Value = entity.ExtUrl;
            cmd.Parameters.Add("@Affiliate", MySqlDbType.VarChar).Value = entity.Affiliate;
            cmd.Parameters.Add("@SubAffiliate", MySqlDbType.VarChar).Value = entity.SubAffiliate;


            if (entity.ProductDomainRoutingID == null)
            {
                entity.ProductDomainRoutingID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ProductDomainRouting({0}) was not found in database.", entity.ProductDomainRoutingID));
                }
            }
        }

        public override ProductDomainRouting Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductDomainRoutingID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductDomainRouting Load(DataRow row)
        {
            ProductDomainRouting res = new ProductDomainRouting();

            if (!(row["ProductDomainRoutingID"] is DBNull))
                res.ProductDomainRoutingID = Convert.ToInt32(row["ProductDomainRoutingID"]);
            if (!(row["ProductDomainID"] is DBNull))
                res.ProductDomainID = Convert.ToInt32(row["ProductDomainID"]);
            if (!(row["Percentage"] is DBNull))
                res.Percentage = Convert.ToInt32(row["Percentage"]);
            if (!(row["CampaignID"] is DBNull))
                res.CampaignID = Convert.ToInt32(row["CampaignID"]);
            if (!(row["ExtUrl"] is DBNull))
                res.ExtUrl = Convert.ToString(row["ExtUrl"]);
            if (!(row["Affiliate"] is DBNull))
                res.Affiliate = Convert.ToString(row["Affiliate"]);
            if (!(row["SubAffiliate"] is DBNull))
                res.SubAffiliate = Convert.ToString(row["SubAffiliate"]);

            return res;
        }
    }
}
