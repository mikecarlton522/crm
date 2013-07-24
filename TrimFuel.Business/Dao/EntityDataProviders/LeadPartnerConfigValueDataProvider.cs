using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class LeadPartnerConfigValueDataProvider : EntityDataProvider<LeadPartnerConfigValue>
    {
        string INSERT_COMMAND = "INSERT INTO LeadPartnerConfigValue (LeadPartnerID, LeadTypeID, ProductID, `Key`, `Value`) VALUES(@LeadPartnerID, @LeadTypeID, @ProductID, @Key, @Value)";
        string UPDATE_COMMAND = @"UPDATE LeadPartnerConfigValue SET `Value`=@Value, `Key`=@Key, LeadPartnerID=@LeadPartnerID, LeadTypeID=@LeadTypeID, ProductID=@ProductID
                                WHERE LeadPartnerID=@ID_LeadPartnerID AND `Key`=@ID_Key AND LeadTypeID=@ID_LeadTypeID AND ProductID=@ID_ProductID;";
        string SELECT_COMMAND = "SELECT * FROM LeadPartnerConfigValue WHERE LeadPartnerID=@LeadPartnerID AND `Key`=@Key AND LeadTypeID=@LeadTypeID AND ProductID=@ProductID;";

        public override void Save(LeadPartnerConfigValue entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.LeadPartnerConfigValueID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ID_LeadPartnerID", MySqlDbType.Int32).Value = entity.LeadPartnerConfigValueID.Value.LeadPartnerID;
                cmd.Parameters.Add("@ID_Key", MySqlDbType.VarChar).Value = entity.LeadPartnerConfigValueID.Value.Key;
                cmd.Parameters.Add("@ID_LeadTypeID", MySqlDbType.Int32).Value = entity.LeadPartnerConfigValueID.Value.LeadTypeID;
                cmd.Parameters.Add("@ID_ProductID", MySqlDbType.Int32).Value = entity.LeadPartnerConfigValueID.Value.ProductID;
            }

            cmd.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = entity.LeadPartnerID;
            cmd.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = entity.LeadTypeID;
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@Key", MySqlDbType.VarChar).Value = entity.Key;
            cmd.Parameters.Add("@Value", MySqlDbType.VarChar).Value = entity.Value;


            if (entity.LeadPartnerConfigValueID == null)
            {
                cmd.ExecuteNonQuery();
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("LeadPartnerConfigValue ({0}) was not found in database.", entity.Key));
                }
            }
            entity.LeadPartnerConfigValueID = new LeadPartnerConfigValue.ID()
            {
                Key = entity.Key,
                ProductID = entity.ProductID,
                LeadTypeID = entity.LeadTypeID,
                LeadPartnerID = entity.LeadPartnerID
            };
        }

        public override LeadPartnerConfigValue Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = ((LeadPartnerConfigValue.ID)key).LeadPartnerID;
            cmd.Parameters.Add("@Key", MySqlDbType.VarChar).Value = ((LeadPartnerConfigValue.ID)key).Key;
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = ((LeadPartnerConfigValue.ID)key).ProductID;
            cmd.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = ((LeadPartnerConfigValue.ID)key).LeadTypeID;

            return Load(cmd).FirstOrDefault();
        }

        public override LeadPartnerConfigValue Load(System.Data.DataRow row)
        {
            LeadPartnerConfigValue res = new LeadPartnerConfigValue();

            if (!(row["LeadPartnerID"] is DBNull))
                res.LeadPartnerID = Convert.ToInt32(row["LeadPartnerID"]);
            if (!(row["Key"] is DBNull))
                res.Key = Convert.ToString(row["Key"]);
            if (!(row["Value"] is DBNull))
                res.Value = Convert.ToString(row["Value"]);
            if (!(row["LeadTypeID"] is DBNull))
                res.LeadTypeID = Convert.ToInt32(row["LeadTypeID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);


            res.LeadPartnerConfigValueID = new LeadPartnerConfigValue.ID() { LeadPartnerID = res.LeadPartnerID.Value, Key = res.Key, LeadTypeID = res.LeadTypeID, ProductID = res.ProductID };

            return res;
        }
    }
}
