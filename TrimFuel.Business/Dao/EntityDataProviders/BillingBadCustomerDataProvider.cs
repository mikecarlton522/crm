using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingBadCustomerDataProvider : EntityDataProvider<BillingBadCustomer>
    {
        private const string SELECT_COMMAND = "SELECT * FROM BillingBadCustomer WHERE BillingBadCustomerID=@BillingBadCustomerID;";
        private const string INSERT_COMMAND = "INSERT INTO BillingBadCustomer(BillingID, `TransactionId`, `Error`, `Found`, `Result`, Request, Response, CreateDT) VALUES(@BillingID, @TransactionId, @Error, @Found, @Result, @Request, @Response, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE BillingBadCustomer SET BillingID=@BillingID, `TransactionId`=@TransactionId, `Error`=@Error, `Found`=@Found, `Result`=@Result, Request=@Request, Response=@Response, CreateDT=@CreateDT WHERE BillingBadCustomerID=@BillingBadCustomerID;";

        public override void Save(BillingBadCustomer entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BillingBadCustomerID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@BillingBadCustomerID", MySqlDbType.Int32).Value = entity.BillingBadCustomerID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@TransactionId", MySqlDbType.VarChar).Value = entity.TransactionId;
            cmd.Parameters.Add("@Error", MySqlDbType.Byte).Value = entity.Error;
            cmd.Parameters.Add("@Found", MySqlDbType.Byte).Value = entity.Found;
            cmd.Parameters.Add("@Result", MySqlDbType.String).Value = entity.Result;
            cmd.Parameters.Add("@Request", MySqlDbType.String).Value = entity.Request;
            cmd.Parameters.Add("@Response", MySqlDbType.String).Value = entity.Response;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.BillingBadCustomerID == null)
            {
                entity.BillingBadCustomerID = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("BillingBadCustomer({0}) was not found in database.", entity.BillingBadCustomerID));
                }
            }
        }

        public override BillingBadCustomer Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@BillingBadCustomerID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override BillingBadCustomer Load(DataRow row)
        {
            BillingBadCustomer res = new BillingBadCustomer();

            if (!(row["BillingBadCustomerID"] is DBNull))
                res.BillingBadCustomerID = Convert.ToInt32(row["BillingBadCustomerID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["TransactionId"] is DBNull))
                res.TransactionId = Convert.ToString(row["TransactionId"]);
            if (!(row["Error"] is DBNull))
                res.Error = Convert.ToByte(row["Error"]);
            if (!(row["Found"] is DBNull))
                res.Found = Convert.ToByte(row["Found"]);
            if (!(row["Result"] is DBNull))
                res.Result = Convert.ToString(row["Result"]);
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
