using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ShipperDataProvider : EntityDataProvider<Shipper>
    {
        private const string INSERT_COMMAND = @"INSERT INTO Shipper(ShipperID, Name, FulfillmentCost, ServiceIsActive) 
                                                VALUES(@ShipperID, @Name, @FulfillmentCost, @ServiceIsActive);";
        private const string UPDATE_COMMAND = @"UPDATE Shipper SET Name=@Name, FulfillmentCost=@FulfillmentCost, ServiceIsActive=@ServiceIsActive where ShipperID=@ShipperID;";
        private const string SELECT_COMMAND = "SELECT * FROM Shipper WHERE ShipperID=@ShipperID;";

        public override void Save(Shipper entity, IMySqlCommandCreater cmdCreater)
        {
            // try get old record from db
            Shipper dbItem = Load(entity.ShipperID, cmdCreater);
            //---------------------

            MySqlCommand cmd = cmdCreater.CreateCommand();
            if (dbItem == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
            }

            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int32).Value = entity.ShipperID;
            cmd.Parameters.Add("@Name", MySqlDbType.VarChar).Value = entity.Name;
            cmd.Parameters.Add("@FulfillmentCost", MySqlDbType.Decimal).Value = entity.FulfillmentCost;
            cmd.Parameters.Add("@ServiceIsActive", MySqlDbType.Bit).Value = entity.ServiceIsActive;

            if (dbItem == null)
            {
                cmd.ExecuteNonQuery();
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Shipper({0}) was not found in database.", entity.ShipperID));
                }
            }
        }

        public override Shipper Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ShipperID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Shipper Load(System.Data.DataRow row)
        {
            Shipper res = new Shipper();

            if (!(row["ShipperID"] is DBNull))
                res.ShipperID = Convert.ToInt32(row["ShipperID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);
            if (!(row["ServiceIsActive"] is DBNull))
                res.ServiceIsActive = Convert.ToBoolean(row["ServiceIsActive"]);
            if (!(row["FulfillmentCost"] is DBNull))
                res.FulfillmentCost = Convert.ToDecimal(row["FulfillmentCost"]);

            return res;
        }
    }
}
