using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AragonLeadDataProvider : EntityDataProvider<AragonLead>
    {
        private const string INSERT_COMMAND = "INSERT INTO AragonLead(BillingID, AragonCampaignID, VerificationDT, UpsellStatus, StatusText, Response, LeadSent, CreateDT) VALUES(@BillingID, @AragonCampaignID, @VerificationDT, @UpsellStatus, @StatusText, @Response, @LeadSent, @CreateDT); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE AragonLead SET BillingID=@BillingID, AragonCampaignID=@AragonCampaignID, VerificationDT=@VerificationDT, UpsellStatus=@UpsellStatus, StatusText=@StatusText, Response=@Response, LeadSent=@LeadSent, CreateDT=@CreateDT WHERE AragonLeadID=@AragonLeadID;";
        private const string SELECT_COMMAND = "SELECT * FROM AragonLead WHERE AragonLeadID=@AragonLeadID;";

        public override void Save(AragonLead entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.AragonLeadID == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@AragonLeadID", MySqlDbType.Int64).Value = entity.AragonLeadID;
            }

            cmd.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = entity.BillingID;
            cmd.Parameters.Add("@AragonCampaignID", MySqlDbType.Int32).Value = entity.AragonCampaignID;
            cmd.Parameters.Add("@VerificationDT", MySqlDbType.Timestamp).Value = entity.VerificationDT;
            cmd.Parameters.Add("@UpsellStatus", MySqlDbType.Int32).Value = entity.UpsellStatus;
            cmd.Parameters.Add("@StatusText", MySqlDbType.VarChar).Value = entity.StatusText;
            cmd.Parameters.Add("@Response", MySqlDbType.VarChar).Value = entity.Response;
            cmd.Parameters.Add("@LeadSent", MySqlDbType.Bit).Value = entity.LeadSent;
            cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;

            if (entity.AragonLeadID == null)
            {
                entity.AragonLeadID = Convert.ToInt64(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("AragonLead({0}) was not found in database.", entity.AragonLeadID));
                }
            }
        }

        public override AragonLead Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@AragonLeadID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override AragonLead Load(DataRow row)
        {
            AragonLead res = new AragonLead();

            if (!(row["AragonLeadID"] is DBNull))
                res.AragonLeadID = Convert.ToInt64(row["AragonLeadID"]);
            if (!(row["BillingID"] is DBNull))
                res.BillingID = Convert.ToInt64(row["BillingID"]);
            if (!(row["AragonCampaignID"] is DBNull))
                res.AragonCampaignID = Convert.ToInt32(row["AragonCampaignID"]);
            if (!(row["VerificationDT"] is DBNull))
                res.VerificationDT = Convert.ToDateTime(row["VerificationDT"]);
            if (!(row["UpsellStatus"] is DBNull))
                res.UpsellStatus = Convert.ToInt32(row["UpsellStatus"]);
            if (!(row["StatusText"] is DBNull))
                res.StatusText = Convert.ToString(row["StatusText"]);
            if (!(row["Response"] is DBNull))
                res.Response = Convert.ToString(row["Response"]);
            if (!(row["LeadSent"] is DBNull))
                res.LeadSent = Convert.ToBoolean(row["LeadSent"]);
            if (!(row["CreateDT"] is DBNull))
                res.CreateDT = Convert.ToDateTime(row["CreateDT"]);

            return res;
        }
    }
}
