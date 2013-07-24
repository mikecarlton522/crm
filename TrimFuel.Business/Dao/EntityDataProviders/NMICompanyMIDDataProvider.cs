using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class NMICompanyMIDDataProvider : EntityDataProvider<NMICompanyMID>
    {
        private const string INSERT_COMMAND = "INSERT INTO NMICompanyMID(NMICompanyID, AssertigyMIDID) VALUES(@NMICompanyID, @AssertigyMIDID); SELECT @@IDENTITY;";
        private const string UPDATE_COMMAND = "UPDATE NMICompanyMID SET NMICompanyID=@NMICompanyID, AssertigyMIDID=@AssertigyMIDID WHERE NMICompanyMID=@NMICompanyMID;";
        private const string SELECT_COMMAND = "SELECT * FROM NMICompanyMID WHERE NMICompanyMID=@NMICompanyMID;";

        public override void Save(NMICompanyMID entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();

            if (entity.NMICompanyMID_ == null)
            {
                cmd.CommandText = INSERT_COMMAND;
            }
            else
            {
                cmd.CommandText = UPDATE_COMMAND;
                cmd.Parameters.Add("@NMICompanyMID", MySqlDbType.Int32).Value = entity.NMICompanyMID_;
            }

            cmd.Parameters.Add("@NMICompanyID", MySqlDbType.Int32).Value = entity.NMICompanyID;
            cmd.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = entity.AssertigyMIDID;


            if (entity.NMICompanyMID_ == null)
            {
                entity.NMICompanyMID_ = Convert.ToInt32(cmd.ExecuteScalar());
            }
            else
            {
                if (cmd.ExecuteNonQuery() == 0)
                {
                    throw new Exception(string.Format("NMICompanyMID({0}) was not found in database.", entity.NMICompanyMID_));
                }
            }
        }

        public override NMICompanyMID Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@NMICompanyMID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override NMICompanyMID Load(DataRow row)
        {
            NMICompanyMID res = new NMICompanyMID();

            if (!(row["NMICompanyMID"] is DBNull))
                res.NMICompanyMID_ = Convert.ToInt32(row["NMICompanyMID"]);
            if (!(row["NMICompanyID"] is DBNull))
                res.NMICompanyID = Convert.ToInt32(row["NMICompanyID"]);
            if (!(row["AssertigyMIDID"] is DBNull))
                res.AssertigyMIDID = Convert.ToInt32(row["AssertigyMIDID"]);

            return res;
        }
    }
}
