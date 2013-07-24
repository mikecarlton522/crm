using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class PixelDataProvider : EntityDataProvider<Pixel>
    {
        public override void Save(Pixel entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override Pixel Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override Pixel Load(DataRow row)
        {
            Pixel res = new Pixel();

            if (!(row["PixelID"] is DBNull))
                res.PixelID = Convert.ToInt32(row["PixelID"]);
            if (!(row["AffiliateID"] is DBNull))
                res.AffiliateID = Convert.ToInt32(row["AffiliateID"]);
            if (!(row["DisplayName"] is DBNull))
                res.DisplayName = Convert.ToString(row["DisplayName"]);
            if (!(row["Code"] is DBNull))
                res.Code = Convert.ToString(row["Code"]);
            if (!(row["Active"] is DBNull))
                res.Active = Convert.ToInt32(row["Active"]);

            return res;
        }
    }
}
