using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Data;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class SHWProductDataProvider : EntityDataProvider<SHWProduct>
    {
        public override void Save(SHWProduct entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override SHWProduct Load(object key, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override SHWProduct Load(DataRow row)
        {
            SHWProduct res = new SHWProduct();

            if (!(row["SHWProductID"] is DBNull))
                res.SHWProductID = Convert.ToInt32(row["SHWProductID"]);
            if (!(row["ProductID"] is DBNull))
                res.ProductID = Convert.ToInt32(row["ProductID"]);
            if (!(row["IntegrationID"] is DBNull))
                res.IntegrationID = Convert.ToString(row["IntegrationID"]);
            if (!(row["CourseID"] is DBNull))
                res.CourseID = Convert.ToInt32(row["CourseID"]);
            if (!(row["SubscriptionID"] is DBNull))
                res.SubscriptionID = Convert.ToInt32(row["SubscriptionID"]);
            if (!(row["CompanyID"] is DBNull))
                res.CompanyID = Convert.ToInt32(row["CompanyID"]);

            return res;
        }
    }
}
