using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class BillingAPIRequestDataProvider : EntityDataProvider<BillingAPIRequest>
    {
        private const string INSERT_COMMAND = "INSERT INTO BillingAPIRequest(CreateDT, URL, IP, Method, Request, Response) VALUES(@CreateDT, @URL, @IP, @Method, @Request, @Response); SELECT @@IDENTITY;";

        public override void Save(BillingAPIRequest entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.BillingAPIRequestID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                throw new NotImplementedException();
                //cmd.CommandText = UPDATE_COMMAND;
                //cmd.Parameters.Add("@BillingAPIRequestID", MySqlDbType.Int32).Value = entity.BillingAPIRequestID;
            }

            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
            cmd.Parameters.Add("@URL", MySqlDbType.VarChar).Value = entity.URL;
            cmd.Parameters.Add("@IP", MySqlDbType.VarChar).Value = entity.IP;
            cmd.Parameters.Add("@Method", MySqlDbType.VarChar).Value = entity.Method;
            cmd.Parameters.Add("@Request", MySqlDbType.VarChar).Value = entity.Request;
            cmd.Parameters.Add("@Response", MySqlDbType.VarChar).Value = entity.Response;

            if (entity.BillingAPIRequestID == null)
            {
                entity.BillingAPIRequestID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("BillingAPIRequest({0}) was not found in database.", entity.BillingAPIRequestID));
                }
            }
        }

        public override BillingAPIRequest Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override BillingAPIRequest Load(DataRow row)
        {
            throw new NotImplementedException();
        }
    }
}
