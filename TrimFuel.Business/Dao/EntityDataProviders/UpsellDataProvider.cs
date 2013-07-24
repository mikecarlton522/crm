using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class UpsellDataProvider : EntityDataProvider<Upsell>
    {
        private const string INSERT_COMMAND = "INSERT INTO Upsell(BillingID, Quantity, ProductCode, CreateDT, Complete, UpsellTypeID) VALUES(@BillingID, @Quantity, @ProductCode, @CreateDT, @Complete, @UpsellTypeID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Upsell SET BillingID=@BillingID, Quantity=@Quantity, ProductCode=@ProductCode, CreateDT=@CreateDT, Complete=@Complete, UpsellTypeID=@UpsellTypeID WHERE UpsellID=@UpsellID;";
        private const string SELECT_COMMAND = "SELECT * FROM Upsell WHERE UpsellID=@UpsellID;";

        public override void Save(Upsell entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.UpsellID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@UpsellID", MySqlDbType.Int32).Value = entity.UpsellID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;
            cmd.Parameters.Add("@ProductCode", MySqlDbType.VarChar).Value = entity.ProductCode;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@Complete", MySqlDbType.Bit).Value = entity.Complete;
            cmd.Parameters.Add("@UpsellTypeID", MySqlDbType.Int32).Value = entity.UpsellTypeID;

            if (entity.UpsellID == null)
            {
                entity.UpsellID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Upsell({0}) was not found in database.", entity.UpsellID));
                }
            }
        }

        public override Upsell Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@UpsellID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Upsell Load(DataRow row)
        {
            Upsell res = new Upsell();

            if (!(row["UpsellID"] is DBNull))
                res.UpsellID = Convert.ToInt32(row["UpsellID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["Quantity"] is DBNull))
                res.Quantity = Convert.ToInt32(row["Quantity"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["Complete"] is DBNull))
                res.Complete = Convert.ToBoolean(row["Complete"]);
            if (!(row["UpsellTypeID"] is DBNull))
                res.UpsellTypeID = Convert.ToInt32(row["UpsellTypeID"]);

            return res;
        }
    }
}
