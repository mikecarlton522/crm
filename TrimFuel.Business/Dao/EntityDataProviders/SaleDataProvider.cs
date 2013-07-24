using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SaleDataProvider : EntityDataProvider<Sale>
    {
        private const string INSERT_COMMAND = "INSERT INTO Sale(SaleTypeID, TrackingNumber, CreateDT, NotShip) VALUES(@SaleTypeID, @TrackingNumber, @CreateDT, @NotShip); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Sale SET SaleTypeID=@SaleTypeID, TrackingNumber=@TrackingNumber, CreateDT=@CreateDT, NotShip=@NotShip WHERE SaleID=@SaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM Sale WHERE SaleID = @SaleID;";

        public override void Save(Sale entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.SaleID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            }

            cmd.Parameters.Add("@SaleTypeID", MySqlDbType.Int16).Value = entity.SaleTypeID;
            cmd.Parameters.Add("@TrackingNumber", MySqlDbType.VarChar).Value = entity.TrackingNumber;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@NotShip", MySqlDbType.Bit).Value = entity.NotShip;

            if (entity.SaleID == null)
            {
                entity.SaleID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Sale({0}) was not found in database.", entity.SaleID));
                }
            }
        }

        public override Sale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Sale Load(DataRow row)
        {
            Sale res = new Sale();

            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["SaleTypeID"] is DBNull))
                res.SaleTypeID = Convert.ToInt16(row["SaleTypeID"]);
            if (!(row["TrackingNumber"] is DBNull))
                res.TrackingNumber = Convert.ToString(row["TrackingNumber"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["NotShip"] is DBNull))
                res.NotShip = Convert.ToBoolean(row["NotShip"]);

            return res;
        }
    }
}
