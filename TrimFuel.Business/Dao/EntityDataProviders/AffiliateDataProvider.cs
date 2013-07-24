using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AffiliateDataProvider : EntityDataProvider<Affiliate>
    {
        private const string INSERT_COMMAND = "INSERT INTO Affiliate(AffiliateMasterID, Code, Password, Active, CostPerSale, Deleted, AffiliateFriendlyName) VALUES(@AffiliateMasterID, @Code, @Password, @Active, @CostPerSale, @Deleted, @AffiliateFriendlyName); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Affiliate SET AffiliateMasterID=@AffiliateMasterID, Code=@Code, Password=@Password, Active=@Active, CostPerSale=@CostPerSale, Deleted=@Deleted, AffiliateFriendlyName=@AffiliateFriendlyName WHERE AffiliateID=@AffiliateID;";
        private const string SELECT_COMMAND = "SELECT * FROM Affiliate WHERE AffiliateID=@AffiliateID;";

        public override void Save(Affiliate entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.AffiliateID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@AffiliateID", MySqlDbType.Int32).Value = entity.AffiliateID;
            }

            cmd.Parameters.Add("@AffiliateMasterID", MySqlDbType.Int32).Value = entity.AffiliateMasterID;
            cmd.Parameters.Add("@Code", MySqlDbType.VarChar).Value = entity.Code;
            cmd.Parameters.Add("@Password", MySqlDbType.VarChar).Value = entity.Password;
            cmd.Parameters.Add("@Active", MySqlDbType.Int32).Value = entity.Active;
            cmd.Parameters.Add("@CostPerSale", MySqlDbType.Decimal).Value = entity.CostPerSale;
            cmd.Parameters.Add("@Deleted", MySqlDbType.Bit).Value = entity.Deleted;
            cmd.Parameters.Add("@AffiliateFriendlyName", MySqlDbType.VarChar).Value = entity.AffiliateFriendlyName;


            if (entity.AffiliateID == null)
            {
                entity.AffiliateID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Affiliate({0}) was not found in database.", entity.AffiliateID));
                }
            }
        }

        public override Affiliate Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@AffiliateID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Affiliate Load(DataRow row)
        {
            Affiliate res = new Affiliate();

            if (!(row["AffiliateID"] is DBNull))
                res.AffiliateID = Convert.ToInt32(row["AffiliateID"]);
            if (!(row["AffiliateMasterID"] is DBNull))
                res.AffiliateMasterID = Convert.ToInt32(row["AffiliateMasterID"]);
            if (!(row["Code"] is DBNull))
                res.Code = Convert.ToString(row["Code"]);
            if (!(row["Password"] is DBNull))
                res.Password = Convert.ToString(row["Password"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToInt32(row["Active"]);
            if (!(row["CostPerSale"] is DBNull))
                res.CostPerSale = Convert.ToDecimal(row["CostPerSale"]);
            if (!(row["Deleted"] is DBNull))
                res.Deleted = Convert.ToBoolean(row["Deleted"]);
            if (!(row["AffiliateFriendlyName"] is DBNull))
                res.AffiliateFriendlyName = Convert.ToString(row["AffiliateFriendlyName"]);

            return res;
        }
    }
}
