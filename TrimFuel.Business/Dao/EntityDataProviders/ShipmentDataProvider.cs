using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ShipmentDataProvider : EntityDataProvider<Shipment>
    {
        private const string INSERT_COMMAND = "INSERT INTO Shipment(SN, SaleID, ProductSKU, ShipmentStatus, CreateDT, SendDT, ErrorDT, ShipDT, ReturnDT, TrackingNumber, ShipperID, ShipperRegID) VALUES(@SN, @SaleID, @ProductSKU, @ShipmentStatus, @CreateDT, @SendDT, @ErrorDT, @ShipDT, @ReturnDT, @TrackingNumber, @ShipperID, @ShipperRegID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Shipment SET SN=@SN, SaleID=@SaleID, ProductSKU=@ProductSKU, ShipmentStatus=@ShipmentStatus, CreateDT=@CreateDT, SendDT=@SendDT, ErrorDT=@ErrorDT, ShipDT=@ShipDT, ReturnDT=@ReturnDT, TrackingNumber=@TrackingNumber, ShipperID=@ShipperID, ShipperRegID=@ShipperRegID WHERE ShipmentID=@ShipmentID;";
        private const string SELECT_COMMAND = "SELECT * FROM Shipment WHERE ShipmentID=@ShipmentID;";

        public override void Save(Shipment entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ShipmentID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ShipmentID", MySqlDbType.Int64).Value = entity.ShipmentID;
            }

            cmd.Parameters.Add("@SN", MySqlDbType.VarChar).Value = entity.SN;
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@ProductSKU", MySqlDbType.VarChar).Value = entity.ProductSKU;
            cmd.Parameters.Add("@ShipmentStatus", MySqlDbType.Int32).Value = entity.ShipmentStatus;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@SendDT", MySqlDbType.Timestamp).Value = entity.SendDT;
            cmd.Parameters.Add("@ErrorDT", MySqlDbType.Timestamp).Value = entity.ErrorDT;
            cmd.Parameters.Add("@ShipDT", MySqlDbType.Timestamp).Value = entity.ShipDT;
            cmd.Parameters.Add("@ReturnDT", MySqlDbType.Timestamp).Value = entity.ReturnDT;
            cmd.Parameters.Add("@TrackingNumber", MySqlDbType.VarChar).Value = entity.TrackingNumber;
            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int16).Value = entity.ShipperID;
            cmd.Parameters.Add("@ShipperRegID", MySqlDbType.VarChar).Value = entity.ShipperRegID;


            if (entity.ShipmentID == null)
            {
                entity.ShipmentID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Shipment({0}) was not found in database.", entity.ShipmentID));
                }
            }
        }

        public override Shipment Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ShipmentID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Shipment Load(DataRow row)
        {
            Shipment res = new Shipment();

            if (!(row["ShipmentID"] is DBNull))
                res.ShipmentID = Convert.ToInt64(row["ShipmentID"]);
            if (!(row["SN"] is DBNull))
                res.SN = Convert.ToString(row["SN"]);
            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["ProductSKU"] is DBNull))
                res.ProductSKU = Convert.ToString(row["ProductSKU"]);
            if (!(row["ShipmentStatus"] is DBNull))
                res.ShipmentStatus = Convert.ToInt32(row["ShipmentStatus"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["SendDT"] is DBNull))
                res.SendDT = Convert.ToDateTime(row["SendDT"]);
            if (!(row["ErrorDT"] is DBNull))
                res.ErrorDT = Convert.ToDateTime(row["ErrorDT"]);
            if (!(row["ShipDT"] is DBNull))
                res.ShipDT = Convert.ToDateTime(row["ShipDT"]);
            if (!(row["ReturnDT"] is DBNull))
                res.ReturnDT = Convert.ToDateTime(row["ReturnDT"]);
            if (!(row["TrackingNumber"] is DBNull))
                res.TrackingNumber = Convert.ToString(row["TrackingNumber"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt16(row["ShipperID"]);
            if (!(row["ShipperRegID"] is DBNull))
                res.ShipperRegID = Convert.ToString(row["ShipperRegID"]);

            return res;
        }
    }
}
