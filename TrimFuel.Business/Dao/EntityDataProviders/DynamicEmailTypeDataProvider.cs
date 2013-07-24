using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class DynamicEmailTypeDataProvider : EntityDataProvider<DynamicEmailType>
    {
        private const string SELECT_COMMAND = "SELECT * FROM DynamicEmailType WHERE ID=@DynamicEmailTypeID;";

        public override void Save(DynamicEmailType entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override DynamicEmailType Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@DynamicEmailTypeID", MySqlDbType.Byte).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override DynamicEmailType Load(DataRow row)
        {
            DynamicEmailType res = new DynamicEmailType();

            if (!(row["ID"] is DBNull))
                res.DynamicEmailTypeID = Convert.ToByte(row["ID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["Instant"] is DBNull))
                res.Instant = Convert.ToBoolean(row["Instant"]);
            if (!(row["SortOrder"] is DBNull))
                res.SortOrder = Convert.ToByte(row["SortOrder"]);

            return res;
        }
    }
}
