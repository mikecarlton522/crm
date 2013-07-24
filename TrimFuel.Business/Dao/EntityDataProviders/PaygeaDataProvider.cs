using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class PaygeaDataProvider : EntityDataProvider<Paygea>
    {
        private const string INSERT_COMMAND = "INSERT INTO Paygea(BillingID, Request, Response, CreateDT) VALUES(@BillingID, @Request, @Response, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE Paygea SET BillingID=@BillingID, Request=@Request, Response=@Response, CreateDT=@CreateDT WHERE PaygeaID=@PaygeaID;";
        private const string SELECT_COMMAND = "SELECT * FROM Paygea WHERE PaygeaID=@PaygeaID;";

        public override void Save(Paygea entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.PaygeaID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@PaygeaID", MySqlDbType.Int64).Value = entity.PaygeaID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@Request", MySqlDbType.Text).Value = entity.Request;
            cmd.Parameters.Add("@Response", MySqlDbType.Text).Value = entity.Response;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.PaygeaID == null)
            {
                entity.PaygeaID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("Paygea({0}) was not found in database.", entity.PaygeaID));
                }
            }
        }

        public override Paygea Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@PaygeaID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Paygea Load(DataRow row)
        {
            Paygea res = new Paygea();

            if (!(row["PaygeaID"] is DBNull))
                res.PaygeaID = Convert.ToInt64(row["PaygeaID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["Request"] is DBNull))
                res.Request = Convert.ToString(row["Request"]);
            if (!(row["Response"] is DBNull))
                res.Response = Convert.ToString(row["Response"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
