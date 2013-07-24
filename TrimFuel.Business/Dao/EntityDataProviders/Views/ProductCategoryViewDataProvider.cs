using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    public class ProductCategoryViewDataProvider : EntityViewDataProvider<ProductCategoryView>
    {
        public override ProductCategoryView Load(System.Data.DataRow row)
        {
            ProductCategoryView res = new ProductCategoryView();

            if (!(row["ProductCategoryID"] is DBNull))
                res.ProductCategoryID = Convert.ToInt32(row["ProductCategoryID"]);
            if (!(row["ProductCode"] is DBNull))
                res.ProductCode = Convert.ToString(row["ProductCode"]);
            if (!(row["CategoryName"] is DBNull))
                res.CategoryName = Convert.ToString(row["CategoryName"]);

            return res;
        }
    }
}
