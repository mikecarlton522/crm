using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class LeadPostDataProvider : EntityDataProvider<LeadPost>
    {
        private const string INSERT_COMMAND = "INSERT INTO LeadPost(RegistrationID, Telephone, CreateDT, RegistrationDT, PostRequest, PostResponse, Completed, ProductID, LeadTypeID, LeadPartnerID) VALUES(@RegistrationID, @Telephone, @CreateDT, @RegistrationDT, @PostRequest, @PostResponse, @Completed, @ProductID, @LeadTypeID, @LeadPartnerID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE LeadPost SET RegistrationID=@RegistrationID, Telephone=@Telephone, CreateDT=@CreateDT, RegistrationDT=@RegistrationDT, PostRequest=@PostRequest, PostResponse=@PostResponse, Completed=@Completed, ProductID=@ProductID, LeadTypeID=@LeadTypeID, LeadPartnerID=@LeadPartnerID WHERE LeadPostID=@LeadPostID;";
        private const string SELECT_COMMAND = "SELECT * FROM LeadPost WHERE LeadPostID=@LeadPostID;";

        public override void Save(LeadPost entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.LeadPostID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@LeadPostID", MySqlDbType.Int64).Value = entity.LeadPostID;
            }

            cmd.Parameters.Add("@RegistrationID", MySqlDbType.Int64).Value = entity.RegistrationID;
            cmd.Parameters.Add("@Telephone", MySqlDbType.VarChar).Value = entity.Telephone;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.DateTime).Value = entity.CreateDT;
            cmd.Parameters.Add("@RegistrationDT", MySqlDbType.DateTime).Value = entity.RegistrationDT;
            cmd.Parameters.Add("@PostRequest", MySqlDbType.Text).Value = entity.PostRequest;
            cmd.Parameters.Add("@PostResponse", MySqlDbType.Text).Value = entity.PostResponse;
            cmd.Parameters.Add("@Completed", MySqlDbType.Bit).Value = entity.Completed;
            cmd.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = entity.ProductID;
            cmd.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = entity.LeadTypeID;
            cmd.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = entity.LeadPartnerID;

            if (entity.LeadPostID == null)
            {
                entity.LeadPostID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("LeadPost({0}) was not found in database.", entity.LeadPostID));
                }
            }
        }

        public override LeadPost Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@LeadPostID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override LeadPost Load(DataRow row)
        {
            LeadPost res = new LeadPost();

            if (!(row["LeadPostID"] is DBNull))
                res.LeadPostID = Convert.ToInt64(row["LeadPostID"]);
            if (!(row["RegistrationID"] is DBNull))
                res.RegistrationID = Convert.ToInt64(row["RegistrationID"]);
            if (!(row["Telephone"] is DBNull))
                res.Telephone = Convert.ToString(row["Telephone"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);
            if (!(row["RegistrationDT"] is DBNull))
                res.RegistrationDT = Convert.ToDateTime(row["RegistrationDT"]);
            if (!(row["PostRequest"] is DBNull))
                res.PostRequest = Convert.ToString(row["PostRequest"]);
            if (!(row["PostResponse"] is DBNull))
                res.PostResponse = Convert.ToString(row["PostResponse"]);
            if (!(row["Completed"] is DBNull))
                res.Completed = Convert.ToBoolean(row["Completed"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["LeadTypeID"] is DBNull))
                res.LeadTypeID = Convert.ToInt32(row["LeadTypeID"]);
            if (!(row["LeadPartnerID"] is DBNull))
                res.LeadPartnerID = Convert.ToInt32(row["LeadPartnerID"]);

            return res;
        }
    }
}
