using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class LeadTypeDataProvider : EntityDataProvider<LeadType>
    {
        private const string SELECT_COMMAND = "SELECT * FROM LeadType WHERE LeadTypeID=@LeadTypeID;";

        public override void Save(LeadType entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override LeadType Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override LeadType Load(System.Data.DataRow row)
        {
            LeadType res = new LeadType();

            if (!(row["LeadTypeID"] is DBNull))
                res.LeadTypeID = Convert.ToInt32(row["LeadTypeID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);

            return res;
        }
    }
}
