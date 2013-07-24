using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Model.Views;

namespace TrimFuel.Business
{
    public class MagentoLocalService : BaseService
    {
        public void SaveMagentoProductCategory(int inventoryID, string categoryName)
        {
            try
            {
                MagentoProductCategory magentoProductCategory = new MagentoProductCategory();
                magentoProductCategory.InventoryID = inventoryID;
                magentoProductCategory.CategoryName = categoryName;
                dao.Save<MagentoProductCategory>(magentoProductCategory);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public MagentoConfig GetMagentoConfig()
        {
            MagentoConfig conf = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from MagentoConfig ");
                conf = dao.Load<MagentoConfig>(q).LastOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                conf = null;
            }
            return conf;
        }

        public IList<MagentoProductCategoryView> GetMagentoProductCategoryViewList()
        {
            IList<MagentoProductCategoryView> res = null;
            try
            {
                MySqlCommand q =
                    new MySqlCommand(
                        "select inv.InventoryID, inv.SKU, inv.Product, mpc.CategoryName from Inventory inv " +
                        "left join MagentoProductCategory mpc on inv.InventoryID=mpc.InventoryID " +
                        "order by inv.InventoryID");

                res = dao.Load<MagentoProductCategoryView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }
    }
}
