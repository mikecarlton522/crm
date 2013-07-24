using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ChargeHistoryInvoiceDataProvider : EntityDataProvider<ChargeHistoryInvoice>
    {
        private const string INSERT_COMMAND = "INSERT INTO ChargeHistoryInvoice(ChargeHistoryID, InvoiceID) VALUES(@ChargeHistoryID, @InvoiceID);";
        private const string UPDATE_COMMAND = "UPDATE ChargeHistoryInvoice SET ChargeHistoryID=@ChargeHistoryID, InvoiceID=@InvoiceID WHERE ChargeHistoryID=@IDChargeHistoryID;";
        private const string SELECT_COMMAND = "SELECT * FROM ChargeHistoryInvoice WHERE ChargeHistoryID=@IDChargeHistoryID;";

        public override void Save(ChargeHistoryInvoice entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.ChargeHistoryInvoiceID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@IDChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryInvoiceID.Value.ChargeHistoryID;
            }

            cmd.Parameters.Add("@ChargeHistoryID", MySqlDbType.Int64).Value = entity.ChargeHistoryID;
            cmd.Parameters.Add("@InvoiceID", MySqlDbType.Int64).Value = entity.InvoiceID;

            if (entity.ChargeHistoryInvoiceID == null)
            {
                cmd.ExecuteNonQuery();
                entity.ChargeHistoryInvoiceID = new ChargeHistoryInvoice.ID() { ChargeHistoryID = entity.ChargeHistoryID.Value };
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("ChargeHistoryInvoice({0}) was not found in database.", entity.ChargeHistoryInvoiceID.Value.ChargeHistoryID));
                }
                else
                {
                    entity.ChargeHistoryInvoiceID = new ChargeHistoryInvoice.ID() { ChargeHistoryID = entity.ChargeHistoryID.Value };
                }
            }
        }

        public override ChargeHistoryInvoice Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@IDChargeHistoryID", MySqlDbType.Int64).Value = ((ChargeHistoryInvoice.ID?)key).Value.ChargeHistoryID;

            return Load(cmd).FirstOrDefault();
        }

        public override ChargeHistoryInvoice Load(DataRow row)
        {
            ChargeHistoryInvoice res = new ChargeHistoryInvoice();

            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryInvoiceID = new ChargeHistoryInvoice.ID()
                {
                    ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"])
                };
            if (!(row["ChargeHistoryID"] is DBNull))
                res.ChargeHistoryID = Convert.ToInt64(row["ChargeHistoryID"]);
            if (!(row["InvoiceID"] is DBNull))
                res.InvoiceID = Convert.ToInt64(row["InvoiceID"]);

            return res;
        }
    }
}
