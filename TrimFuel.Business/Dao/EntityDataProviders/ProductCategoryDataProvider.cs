using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Dao.EntityDataProviders
{
    public class ProductCategoryDataProvider : EntityDataProvider<ProductCategory>
    {
        private const string SELECT_COMMAND = "SELECT * FROM ProductCategory WHERE ProductCategoryID=@ProductCategoryID;";

        public override void Save(ProductCategory entity, IMySqlCommandCreater cmdCreater)
        {
            throw new NotImplementedException();
        }

        public override ProductCategory Load(object key, IMySqlCommandCreater cmdCreater)
        {
            MySqlCommand cmd = cmdCreater.CreateCommand(SELECT_COMMAND);
            cmd.Parameters.Add("@ProductCategoryID", MySqlDbType.Int64).Value = key;

            return Load(cmd).FirstOrDefault();
        }

        public override ProductCategory Load(System.Data.DataRow row)
        {
            ProductCategory res = new ProductCategory();

            if (!(row["ProductCategoryID"] is DBNull))
                res.ProductCategoryID = Convert.ToInt32(row["ProductCategoryID"]);
            if (!(row["CategoryName"] is DBNull))
                res.CategoryName = Convert.ToString(row["CategoryName"]);

             return res;
        }
    }
}
