using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Dao.EntityDataProviders;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using System.Collections;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class InventoryService : BaseService
    {
        public IList<Inventory> GetInventoryList()
        {
            IList<Inventory> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select i.* from Inventory i
                    order by i.SKU");

                res = dao.Load<Inventory>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public Inventory GetInventoryBySKU(string sku)
        {
            Inventory res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select i.* from Inventory i
                    where i.SKU = @sku");
                q.Parameters.Add("@sku", MySqlDbType.VarChar).Value = sku;

                res = dao.Load<Inventory>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<ProductCode> GetProductCodeList()
        {
            IList<ProductCode> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select p.ProductCodeID, p.ProductCode, GROUP_CONCAT(CONCAT(CAST(pi.Quantity as CHAR), 'x ', i.Product) SEPARATOR ' + ') as Name from ProductCode p
					                inner join ProductCodeInventory pi on pi.ProductCodeID = p.ProductCodeID
                                    inner join Inventory i on i.InventoryID = pi.InventoryID
					                group by p.ProductCodeID
					                having sum(pi.Quantity) > 0
					                order by p.ProductCode");
                res = dao.Load<ProductCode>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public InventorySKUMappingView GetInventorySKUMapping(int inventoryID)
        {
            InventorySKUMappingView res = null;
            try
            {
                res = new InventorySKUMappingView();
                res.Inventory = EnsureLoad<Inventory>(inventoryID);

                IList<Shipper> shipperList = new ProductService().GetShippers().Where(u => u.ServiceIsActive == true).ToList();

                MySqlCommand q = new MySqlCommand(@"
                    select i.* from InventorySKU i
                    where i.ProductSKU = @sku");
                q.Parameters.Add("@sku", MySqlDbType.VarChar).Value = res.Inventory.SKU;

                IList<InventorySKU> skuList = dao.Load<InventorySKU>(q);

                res.Mapping = (from sh in shipperList
                               join i in skuList on sh.ShipperID equals i.ShipperID.Value into iGrp
                               from i2 in iGrp.DefaultIfEmpty()
                               select new Set<Shipper, InventorySKU>()
                               {
                                   Value1 = sh,
                                   Value2 = i2
                               }).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<InventoryView> GetInventoryListForShipping(string productCode, int shipperID)
        {
            IList<InventoryView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select i.InventoryID, (case when coalesce(isku.InventorySKU, '') <> '' then isku.InventorySKU else psku.ProductSKU end) as SKU, i.Product, pci.Quantity from Inventory i " +
                    "inner join ProductSKU psku on psku.ProductSKU = i.SKU " +
                    "left join InventorySKU isku on isku.ProductSKU = psku.ProductSKU and isku.ShipperID = @shipperID " +
                    "inner join ProductCodeInventory pci on pci.InventoryID = i.InventoryID " +
                    "inner join ProductCode pc on pc.ProductCodeID = pci.ProductCodeID " +
                    "where pc.ProductCode = @productCode and i.InventoryType=@InventoryType_Inventory");
                q.Parameters.Add("@productCode", MySqlDbType.VarChar).Value = productCode;
                q.Parameters.Add("@shipperID", MySqlDbType.Int16).Value = shipperID;
                q.Parameters.Add("@InventoryType_Inventory", MySqlDbType.Int32).Value = InventoryTypeEnum.Inventory;

                res = dao.Load<InventoryView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }
        /// <summary>
        /// Creates new inventory
        /// </summary>
        /// <param name="sku">sku of the new inventory</param>
        /// <param name="product">name of the new inventory</param>
        /// <param name="inStock">in stock of the new inventory</param>
        /// <param name="costs">costs of the new inventory</param>
        /// <param name="retailPrice">retail price of the new inventory</param>
        /// <returns>BusinessError</returns>
        public Inventory CreateInventory(string sku, string product, int inStock, decimal? costs, decimal? retailPrice, int? inventoryType, IList<KeyValuePair<int, string>> shipperSKUs)
        {
            Inventory res = null;
            try
            {
                dao.BeginTransaction();

                if (string.IsNullOrEmpty(sku))
                {
                    throw new Exception("SKU is not specified");
                }
                else if (string.IsNullOrEmpty(product))
                {
                    throw new Exception("Product not specified");
                }
                else
                {
                    ProductSKU productSKU = new ProductSKU();
                    productSKU.ProductSKU_ = sku;
                    productSKU.ProductName = product;
                    dao.Save(productSKU);


                    Inventory inventory = new Inventory();
                    inventory.SKU = productSKU.ProductSKU_;
                    inventory.Product = productSKU.ProductName;
                    inventory.InStock = inStock;
                    inventory.Costs = costs;
                    inventory.RetailPrice = retailPrice;
                    inventory.InventoryType = inventoryType;
                    dao.Save<Inventory>(inventory);

                    if (shipperSKUs != null)
                    {
                        foreach (var item in shipperSKUs)
                        {
                            if (!string.IsNullOrEmpty(item.Value))
                            {
                                InventorySKU invSKU = new InventorySKU();
                                invSKU.ProductSKU = productSKU.ProductSKU_;
                                invSKU.ShipperID = item.Key;
                                invSKU.InventorySKU_ = item.Value;
                                dao.Save(invSKU);
                            }
                        }
                    }

                    res = inventory;
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }

        public Inventory UpdateInventory(int inventoryID, string product, int inStock, decimal? costs, decimal? retailPrice, int? inventoryType, IList<KeyValuePair<int, string>> shipperSKUs)
        {
            Inventory res = null;
            try
            {
                dao.BeginTransaction();                

                if (string.IsNullOrEmpty(product))
                {
                    throw new Exception("Product not specified");
                }
                else
                {
                    res = EnsureLoad<Inventory>(inventoryID);

                    ProductSKU productSKU = EnsureLoad<ProductSKU>(res.SKU);
                    productSKU.ProductName = product;
                    dao.Save(productSKU);

                    res.Product = productSKU.ProductName;
                    res.InStock = inStock;
                    res.Costs = costs;
                    res.RetailPrice = retailPrice;
                    res.InventoryType = inventoryType;
                    dao.Save<Inventory>(res);

                    if (shipperSKUs != null)
                    {
                        foreach (var item in shipperSKUs)
                        {
                            InventorySKU invSKU = dao.Load<InventorySKU>(new InventorySKU.ID() { ProductSKU = productSKU.ProductSKU_, ShipperID = item.Key });
                            if (invSKU != null || !string.IsNullOrEmpty(item.Value))
                            {
                                if (invSKU == null)
                                {
                                    invSKU = new InventorySKU() { ProductSKU = productSKU.ProductSKU_, ShipperID = item.Key };
                                }

                                invSKU.InventorySKU_ = item.Value;
                                dao.Save(invSKU);
                            }
                        }
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }

        /// <summary>
        /// Creates new inventory with product
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="product"></param>
        /// <param name="inStock"></param>
        /// <param name="costs"></param>
        /// <param name="retailPrice"></param>
        /// <returns></returns>
        public BusinessError<Inventory> CreateInventoryWithProduct(string sku, string product, int inStock, decimal? costs, decimal? retailPrice, int? inventoryType, IList<KeyValuePair<int, string>> shipperSKUs)
        {
            BusinessError<Inventory> res = new BusinessError<Inventory>(null, BusinessErrorState.Error, "Unknown error occurred");
            try
            {
                dao.BeginTransaction();

                if (string.IsNullOrEmpty(sku))
                {
                    res.ErrorMessage = "SKU is not specified";
                }
                else if (string.IsNullOrEmpty(product))
                {
                    res.ErrorMessage = "Product name is not specified";
                }
                else
                {
                    Inventory existing = GetInventoryBySKU(sku);
                    if (existing != null)
                    {
                        res.ErrorMessage = string.Format("Inventory with SKU({0}) already exists", sku);
                    }
                    else
                    {
                        Inventory inventory = CreateInventory(sku, product, inStock, costs, retailPrice, inventoryType, shipperSKUs);

                        res.ReturnValue = inventory;

                        ProductCode productCode = new ProductCode();
                        productCode.ProductCode_ = sku;
                        productCode.Name = string.Empty;
                        dao.Save<ProductCode>(productCode);

                        ProductCodeInventory productCodeInventory = new ProductCodeInventory();
                        productCodeInventory.ProductCodeID = productCode.ProductCodeID;
                        productCodeInventory.InventoryID = inventory.InventoryID;
                        productCodeInventory.Quantity = 1;
                        dao.Save<ProductCodeInventory>(productCodeInventory);

                        res.State = BusinessErrorState.Success;
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public BusinessError<bool> DeleteProductCode(int productCodeID)
        {
            BusinessError<bool> res = new BusinessError<bool>(true, BusinessErrorState.Success, null);
            try
            {
                dao.BeginTransaction();

                ProductCode toDelete = EnsureLoad<ProductCode>(productCodeID);

                //UpsellType validation
                /*
                        union 
                        select concat('UpsellType #', cast(ut.UpsellTypeID as char)) as Value from ProductCode pc
                        inner join UpsellType ut on pc.ProductCode = ut.ProductCode
                        where pc.ProductCodeID = @ProductCodeID
                */
                MySqlCommand cmd = new MySqlCommand(@"
                        select concat('Subscription #', cast(s.SubscriptionID as char)) as Value from ProductCode pc
                        inner join Subscription s on (pc.ProductCode = s.SKU2 OR pc.ProductCode = s.ProductCode)
                        where pc.ProductCodeID = @ProductCodeID
                ");

                cmd.Parameters.Add("@ProductCodeID", MySqlDbType.Int32).Value = productCodeID;
                IList<StringView> alerts = dao.Load<StringView>(cmd);

                //If there is no usings of Product code
                if (alerts.Count == 0)
                {
                    MySqlCommand cmdDel = new MySqlCommand();
                    cmdDel.CommandText =
                        "delete from ProductCodeInventory where ProductCodeID = @ProductCodeID;" +
                        "delete from ProductCodeInfo where ProductCodeID = @ProductCodeID;" +
                        "delete from ProductCode where ProductCodeID = @ProductCodeID;";
                    cmdDel.Parameters.Add("@ProductCodeID", MySqlDbType.String).Value = productCodeID;
                    dao.ExecuteNonQuery(cmdDel);
                }
                else
                {
                    res.State = BusinessErrorState.Error;
                    res.ReturnValue = false;
                    //IList subscriptionIDList = (alerts.Where(i => i.Value.StartsWith("Subscription #")).Select(i => i.Value.Replace("Subscription #")
                    res.ErrorMessage = "There are subscription plans associated with this product.  Please delete these plans and then re-attempt.";
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Error occurred";
            }
            return res;
        }

        /// <summary>
        /// Deletes inventory (with all its' Product codes)
        /// </summary>
        /// <param name="inventoryID">InventoryID of the Inventory, that should be deleted</param>
        /// <returns>BusinessError</returns>
        public BusinessError<bool> DeleteInventory(int inventoryID)
        {
            BusinessError<bool> res = new BusinessError<bool>(true, BusinessErrorState.Success, null);
            try
            {
                dao.BeginTransaction();

                Inventory toDelete = EnsureLoad<Inventory>(inventoryID);

                //UpsellType validation
                /*
                        union 
                        select concat('UpsellType #', cast(ut.UpsellTypeID as char)) as Value from Inventory i
                        inner join ProductCodeInventory pci on i.InventoryID = pci.InventoryID
                        inner join ProductCode pc on pc.ProductCodeID = pci.ProductCodeID 
                        inner join UpsellType ut on pc.ProductCode = ut.ProductCode
                        where i.InventoryID = @InventoryID
                */
                MySqlCommand cmd = new MySqlCommand(@"
                        select concat('Subscription #', cast(s.SubscriptionID as char)) as Value from Inventory i
                        inner join ProductCodeInventory pci on i.InventoryID = pci.InventoryID
                        inner join ProductCode pc on pc.ProductCodeID = pci.ProductCodeID 
                        inner join Subscription s on (pc.ProductCode = s.SKU2 OR pc.ProductCode = s.ProductCode)
                        where i.InventoryID = @InventoryID
                        union 
                        select distinct concat('RecurringPlan #', cast(rp.RecurringPlanID as char)) as Value from Inventory i
                        inner join RecurringPlanShipment rpsh on rpsh.ProductSKU = i.SKU
                        inner join RecurringPlanCycle rpc on rpc.RecurringPlanCycleID = rpsh.RecurringPlanCycleID
                        inner join RecurringPlan rp on rp.RecurringPlanID = rpc.RecurringPlanID
                        where i.InventoryID = @InventoryID
                ");

                cmd.Parameters.Add("@InventoryID", MySqlDbType.Int32).Value = inventoryID;
                IList<StringView> alerts = dao.Load<StringView>(cmd);
                
                //If there is no usings of any Product code of current Inventory in table subscription
                if (alerts.Count == 0)
                {
                    cmd.CommandText =
                        "select * from ProductCodeInventory where " +
                        "InventoryID = @InventoryID";
                    IList<ProductCodeInventory> list = dao.Load<ProductCodeInventory>(cmd);

                    List<int> productCodeIDList = new List<int>();

                    foreach (ProductCodeInventory productCodeInventory in list)
                    {
                        if (!productCodeIDList.Contains(productCodeInventory.ProductCodeID.Value))
                        {
                            productCodeIDList.Add(productCodeInventory.ProductCodeID.Value);
                        }
                    }
                    foreach (int productCodeID in productCodeIDList)
                    {
                        MySqlCommand cmdDel = new MySqlCommand();
                        cmdDel.CommandText =
                            "delete from ProductCodeInventory where (InventoryID = @InventoryID and ProductCodeID = @ProductCodeID);" +
                            "delete from ProductCodeInfo where ProductCodeID = @ProductCodeID;" +
                            "delete from ProductCode where ProductCodeID = @ProductCodeID;";
                        cmdDel.Parameters.Add("@InventoryID", MySqlDbType.String).Value = inventoryID;
                        cmdDel.Parameters.Add("@ProductCodeID", MySqlDbType.String).Value = productCodeID;
                        dao.ExecuteNonQuery(cmdDel);
                    }

                    cmd.CommandText = "delete from Inventory where InventoryID = @InventoryID;";
                    dao.ExecuteNonQuery(cmd);

                    cmd = new MySqlCommand(@"
                        delete from InventoryGeo where ProductSKU = @ProductSKU;
                        delete from InventorySKU where ProductSKU = @ProductSKU;
                        delete from ProductSKU where ProductSKU = @ProductSKU;
                    ");
                    cmd.Parameters.Add("@ProductSKU", MySqlDbType.VarChar).Value = toDelete.SKU;
                    dao.ExecuteNonQuery(cmd);
                }
                else
                {
                    res.State = BusinessErrorState.Error;
                    res.ReturnValue = false;
                    //IList subscriptionIDList = (alerts.Where(i => i.Value.StartsWith("Subscription #")).Select(i => i.Value.Replace("Subscription #")
                    res.ErrorMessage = "There are subscription plans associated with this SKU.  Please delete these plans and then re-attempt.";
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Error occurred";
            }
            return res;
        }

        public IList<InventoryView> GetInventoryListByProductCode(string productCode)
        {
            IList<InventoryView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select i.InventoryID, i.SKU, i.Product, pci.Quantity from Inventory i
                    inner join ProductCodeInventory pci on pci.InventoryID = i.InventoryID
                    inner join ProductCode pc on pc.ProductCodeID = pci.ProductCodeID
                    where pc.ProductCode = @productCode
                    order by i.SKU, pci.Quantity
                ");
                q.Parameters.Add("@productCode", MySqlDbType.VarChar).Value = productCode;
                res = dao.Load<InventoryView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }
            return res;
        }
    }
}
