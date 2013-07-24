using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ClosedMIDRoutingDataProvider : EntityDataProvider<ClosedMIDRouting>
    {
        private const string INSERT_COMMAND = "INSERT INTO ClosedMIDRouting(ClosedMIDID, AssertigyMIDID, Percentage, PaymentTypeID) VALUES(@ClosedMIDID, @AssertigyMIDID, @Percentage, @PaymentTypeID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE ClosedMIDRouting SET ClosedMIDID=@ClosedMIDID, AssertigyMIDID=@AssertigyMIDID, Percentage=@Percentage, PaymentTypeID=@PaymentTypeID WHERE ClosedMIDRoutingID=@ClosedMIDRoutingID;";
        private const string SELECT_COMMAND = "SELECT * FROM ClosedMIDRouting WHERE ClosedMIDRoutingID=@ClosedMIDRoutingID;";

        public override void Save(ClosedMIDRouting entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ClosedMIDRoutingID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@ClosedMIDRoutingID", MySqlDbType.Int32).Value = entity.ClosedMIDRoutingID;
            }

            cmd.Parameters.Add("@ClosedMIDID", MySqlDbType.Int32).Value = entity.ClosedMIDID;
            cmd.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = entity.AssertigyMIDID;
            cmd.Parameters.Add("@Percentage", MySqlDbType.Int32).Value = entity.Percentage;
            cmd.Parameters.Add("@PaymentTypeID", MySqlDbType.Int32).Value = entity.PaymentTypeID;


            if (entity.ClosedMIDRoutingID == null)
            {
                entity.ClosedMIDRoutingID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ClosedMIDRouting({0}) was not found in database.", entity.ClosedMIDRoutingID));
                }
            }
        }

        public override ClosedMIDRouting Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ClosedMIDRoutingID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ClosedMIDRouting Load(DataRow row)
        {
            ClosedMIDRouting res = new ClosedMIDRouting();

            if (!(row["ClosedMIDRoutingID"] is DBNull))
                res.ClosedMIDRoutingID = Convert.ToInt32(row["ClosedMIDRoutingID"]);
            if (!(row["ClosedMIDID"] is DBNull))
                res.ClosedMIDID = Convert.ToInt32(row["ClosedMIDID"]);
            if (!(row["AssertigyMIDID"] is DBNull))
                res.AssertigyMIDID = Convert.ToInt32(row["AssertigyMIDID"]);
            if (!(row["Percentage"] is DBNull))
                res.Percentage = Convert.ToInt32(row["Percentage"]);
            if (!(row["PaymentTypeID"] is DBNull))
                res.PaymentTypeID = Convert.ToInt32(row["PaymentTypeID"]);

            return res;
        }
    }
}
