using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SaleRefererDataProvider : EntityDataProvider<SaleReferer>
    {
        private const string INSERT_COMMAND = "INSERT INTO SaleReferer(SaleID, RefererID, CreateDT) VALUES(@SaleID, @RefererID, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE SaleReferer SET SaleID=@SaleID, RefererID=@RefererID, CreateDT=@CreateDT WHERE SaleRefererID=@SaleRefererID;";
        private const string SELECT_COMMAND = "SELECT * FROM SaleReferer WHERE SaleRefererID = @SaleRefererID;";

        public override void Save(SaleReferer entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SaleRefererID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SaleRefererID", MySqlDbType.Int32).Value = entity.SaleRefererID;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@RefererID", MySqlDbType.Int32).Value = entity.RefererID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.SaleRefererID == null)
            {
                entity.SaleRefererID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("SaleReferer({0}) was not found in database.", entity.SaleRefererID));
                }
            }
        }

        public override SaleReferer Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleRefererID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override SaleReferer Load(DataRow row)
        {
            SaleReferer res = new SaleReferer();

            if (!(row["SaleRefererID"] is DBNull))
                res.SaleRefererID = Convert.ToInt32(row["SaleRefererID"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["RefererID"] is DBNull))
                res.RefererID = Convert.ToInt32(row["RefererID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
