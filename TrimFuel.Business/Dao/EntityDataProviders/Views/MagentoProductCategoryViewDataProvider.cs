using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Dao.EntityDataProviders.Views
{
    class MagentoProductCategoryViewDataProvider : EntityViewDataProvider<MagentoProductCategoryView>
    {
        public override MagentoProductCategoryView Load(DataRow row)
        {
            MagentoProductCategoryView res = new MagentoProductCategoryView();

            if (!(row["InventoryID"] is DBNull))
                res.InventoryID = Convert.ToInt32(row["InventoryID"]);
            if (!(row["SKU"] is DBNull))
                res.SKU = Convert.ToString(row["SKU"]);
            if (!(row["Product"] is DBNull))
                res.Product = Convert.ToString(row["Product"]);
            if (!(row["CategoryName"] is DBNull))
                res.CategoryName = Convert.ToString(row["CategoryName"]);

            return res;
        }
    }
}
