using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AggUnsentShipmentsDataProvider : EntityDataProvider<AggUnsentShipments>
    {
        private const string INSERT_COMMAND = "INSERT INTO AggUnsentShipments(SaleID, BillingID, ShipperID, `Date`, Reason) VALUES(@SaleID, @BillingID, @ShipperID, @Date, @Reason);";
        private const string UPDATE_COMMAND = "UPDATE AggUnsentShipments SET BillingID=@BillingID, ShipperID=@ShipperID, `Date`=@Date, Reason=@Reason WHERE SaleID=@SaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM AggUnsentShipments WHERE SaleID=@SaleID;";

        public override void Save(AggUnsentShipments entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            var existedItem = Load(entity.SaleID, cmdCreater);

            if (existedItem == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int32).Value = entity.ShipperID;
            cmd.Parameters.Add("@Date", MySqlDbType.Timestamp).Value = entity.Date;
            cmd.Parameters.Add("@Reason", MySqlDbType.VarChar).Value = entity.Reason;


            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("AggUnsentShipments({0}) was not found in database.", entity.SaleID));
            }
        }

        public override AggUnsentShipments Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override AggUnsentShipments Load(DataRow row)
        {
            AggUnsentShipments res = new AggUnsentShipments();

            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            if (!(row["Date"] is DBNull))
                res.Date = Convert.ToDateTime(row["Date"]);
            if (!(row["Reason"] is DBNull))
                res.Reason = Convert.ToString(row["Reason"]);

            return res;
        }
    }
}
