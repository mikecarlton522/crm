using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class CountryDataProvider : EntityDataProvider<Country>
    {
        public override void Save(Country entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override Country Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override Country Load(DataRow row)
        {
            Country res = new Country();

            if (!(row["CountryID"] is DBNull))
                res.CountryID = Convert.ToInt32(row["CountryID"]);
            if (!(row["Name"] is DBNull))
                res.Name = Convert.ToString(row["Name"]);
            if (!(row["Code"] is DBNull))
                res.Code = Convert.ToString(row["Code"]);
            if (!(row["Area"] is DBNull))
                res.Area = Convert.ToString(row["Area"]);

            return res;
        }
    }
}
