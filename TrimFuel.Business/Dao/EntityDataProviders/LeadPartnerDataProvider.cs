using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class LeadPartnerDataProvider : EntityDataProvider<LeadPartner>
    {
        private const string INSERT_COMMAND = @"INSERT INTO LeadPartner(LeadPartnerID, DisplayName, ServiceIsActive) 
                                                VALUES(@LeadPartnerID, @DisplayName, @ServiceIsActive);";
        private const string UPDATE_COMMAND = @"UPDATE LeadPartner SET DisplayName=@DisplayName, ServiceIsActive=@ServiceIsActive where LeadPartnerID=@LeadPartnerID;";
        private const string SELECT_COMMAND = "SELECT * FROM LeadPartner WHERE LeadPartnerID=@LeadPartnerID;";

        public override void Save(LeadPartner entity, IMySqlCommandCreater cmdCreater)
        {
            // try get old record from db
            var dbItem = Load(entity.LeadPartnerID, cmdCreater);
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

            cmd.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = entity.LeadPartnerID;
            cmd.Parameters.Add("@DisplayName", MySqlDbType.VarChar).Value = entity.DisplayName;
            cmd.Parameters.Add("@ServiceIsActive", MySqlDbType.Bit).Value = entity.ServiceisActive;

            if (dbItem == null)
            {
                cmd.ExecuteNonQuery();
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("LeadPartner({0}) was not found in database.", entity.LeadPartnerID));
                }
            }
        }

        public override LeadPartner Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override LeadPartner Load(DataRow row)
        {
            LeadPartner res = new LeadPartner();

            if (!(row["LeadPartnerID"] is DBNull))
                res.LeadPartnerID = Convert.ToInt32(row["LeadPartnerID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["ServiceIsActive"] is DBNull))
                res.ServiceisActive = Convert.ToBoolean(row["ServiceIsActive"]);
            return res;
        }
    }
}
