using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using System.Data;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class StoreDataProvider : EntityDataProvider<Store>
    {
        private const string SELECT_COMMAND = "SELECT * FROM Store WHERE StoreID=@StoreID;";

        public override void Save(Store entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override Store Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@StoreID", MySqlDbType.Int32).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override Store Load(DataRow row)
        {
            Store res = new Store();

            if (!(row["StoreID"] is DBNull))
                res.StoreID = Convert.ToInt32(row["StoreID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);
            if (!(row["Description"] is DBNull))
                res.Description = Convert.ToString(row["Description"]);

            return res;
        }
    }
}
