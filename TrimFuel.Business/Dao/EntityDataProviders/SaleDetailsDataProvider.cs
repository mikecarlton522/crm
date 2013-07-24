using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SaleDetailsDataProvider : EntityDataProvider<SaleDetails>
    {
        private const string INSERT_COMMAND = "INSERT INTO SaleDetails(SaleID, Description) VALUES(@SaleID, @Description);";
        private const string UPDATE_COMMAND = "UPDATE SaleDetails SET Description=@Description WHERE SaleID=@SaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM SaleDetails WHERE SaleID=@SaleID;";

        public override void Save(SaleDetails entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            //Billing 1 <-> 0..1 BillingExternalInfo association
            //Try to update first
            cmd.CommandText = UPDATE_COMMAND;

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@Description", MySqlDbType.VarChar).Value = entity.Description;

            //Sale 1 <-> 0..1 SaleDetails association
            if (cmd.ExecuteNonQuery() == 0)
            {
                //Sale 1 <-> 0..1 SaleDetails association
                //If update failed try to insert
                cmd.CommandText = INSERT_COMMAND;
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Foreign Key Sale({0}) was not found in database.", entity.SaleID));
                }
            }
        }

        public override SaleDetails Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SaleDetails Load(DataRow row)
        {
            SaleDetails res = new SaleDetails();

            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["Description"] is DBNull))
                res.Description = Convert.ToString(row["Description"]);

            return res;
        }
    }
}
