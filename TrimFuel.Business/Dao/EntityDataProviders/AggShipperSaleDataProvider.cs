using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AggShipperSaleDataProvider : EntityDataProvider<AggShipperSale>
    {
        private const string INSERT_COMMAND = "INSERT INTO AggShipperSale(SaleID, ShipperID, CreateDT) VALUES(@SaleID, @ShipperID, @CreateDT);";
        private const string UPDATE_COMMAND = "UPDATE AggShipperSale SET ShipperID=@ShipperID, CreateDT=@CreateDT WHERE SaleID=@SaleID;";
        private const string SELECT_COMMAND = "SELECT * FROM AggShipperSale WHERE SaleID=@SaleID;";

        public override void Save(AggShipperSale entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            var existedEntity = Load(entity.SaleID, cmdCreater);
            
            if (entity.SaleID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int32).Value = entity.ShipperID;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;


            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("AggShipperSale({0}) was not found in database.", entity.SaleID));
            }
        }

        public override AggShipperSale Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override AggShipperSale Load(DataRow row)
        {
            AggShipperSale res = new AggShipperSale();

            if (!(row["SaleID"] is DBNull))
                res.SaleID = Convert.ToInt64(row["SaleID"]);
            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
