using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class USStateDataProvider : EntityDataProvider<USState>
    {
        public override void Save(USState entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override USState Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override USState Load(DataRow row)
        {
            USState res = new USState();

            if (!(row["ShortName"] is DBNull))
                res.ShortName = Convert.ToString(row["ShortName"]);
            if (!(row["FullName"] is DBNull))
                res.FullName = Convert.ToString(row["FullName"]);
            if (!(row["ListAtEnd"] is DBNull))
                res.ListAtEnd = Convert.ToBoolean(row["ListAtEnd"]);

            return res;
        }
    }
}
