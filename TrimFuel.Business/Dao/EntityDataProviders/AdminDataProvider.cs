using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AdminDataProvider : EntityDataProvider<Admin>
    {
        private const string SELECT_COMMAND = "SELECT * FROM Admin WHERE AdminID=@AdminID;";

        public override void Save(Admin entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override Admin Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@AdminID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Admin Load(DataRow row)
        {
            Admin res = new Admin();

            if (!(row["AdminID"] is DBNull))
                res.AdminID = Convert.ToInt32(row["AdminID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToBoolean(row["Active"]);
            if (!(row["Password"] is DBNull))
                res.Password = Convert.ToString(row["Password"]);
            if (!(row["RestrictLevel"] is DBNull))
                res.RestrictLevel = Convert.ToInt32(row["RestrictLevel"]);

            return res;
        }
    }
}
