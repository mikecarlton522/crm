using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TrimFuel.Business.Dao.EntityDataProviders;
using TrimFuel.Model;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class TPClientNewsReadDataProvider : EntityDataProvider<TPClientNewsRead>
    {
        private const string INSERT_COMMAND = "INSERT INTO `TPClientNewsRead`(TPClientNewsID, AdminID) VALUES(@TPClientNewsID, @AdminID);";
        private const string SELECT_COMMAND = "SELECT * FROM `TPClientNews` WHERE TPClientNewsID=@TPClientNewsID;";

        public override void Save(TPClientNewsRead entity, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand();
            cmd.CommandText = INSERT_COMMAND;
            cmd.Parameters.Add("@TPClientNewsID", MySqlDbType.Int32).Value = entity.TPClientNewsID;
            cmd.Parameters.Add("@AdminID", MySqlDbType.Int32).Value = entity.AdminID;

            if (cmd.ExecuteNonQuery() == 0)
            {
                throw new Exception(string.Format("TPClientNews({0}) was not found in database.",
                                                  entity.TPClientNewsID));
            }
        }

        public override TPClientNewsRead Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@TPClientNewsID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }
        public override TPClientNewsRead Load(DataRow row)
        {
            TPClientNewsRead res = new TPClientNewsRead();

            if (!(row["TPClientNewsID"] is DBNull))
                res.TPClientNewsID = Convert.ToInt32(row["TPClientNewsID"]);
            if (!(row["AdminID"] is DBNull))
                res.AdminID = Convert.ToInt32(row["AdminID"]);

            return res;
        }
    }
}
