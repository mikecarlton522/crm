using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class AssertigyMIDGroupDataProvider : EntityDataProvider<AssertigyMIDGroup>
    {
        private const string SELECT_COMMAND = "SELECT * FROM AssertigyMIDGroup WHERE AssertigyMIDGroupID = @AssertigyMIDGroupID;";

        public override void Save(AssertigyMIDGroup entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override AssertigyMIDGroup Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@AssertigyMIDGroupID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override AssertigyMIDGroup Load(DataRow row)
        {
            AssertigyMIDGroup res = new AssertigyMIDGroup();

            if (!(row["AssertigyMIDGroupID"] is DBNull))
                res.AssertigyMIDGroupID = Convert.ToInt32(row["AssertigyMIDGroupID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);

            return res;
        }
    }
}
