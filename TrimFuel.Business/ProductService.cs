using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;

namespace TrimFuel.Business
{
    public class ProductService : BaseService
    {
        public List<Product> GetProductList()
        {
            List<Product> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM Product WHERE ProductIsActive=1 Order By ProductName");
                res = dao.Load<Product>(q).ToList();
            }
            catch (Exception ex)
            {
                res = new List<Product>();
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public Product GetProduct(int? productId)
        {
            Product res = null;
            try
            {
                res = dao.Load<Product>(productId);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public ProductRouting GetProductRouting(int productID)
        {
            ProductRouting res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM ProductRouting WHERE ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<ProductRouting>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<ProductDomain> GetProductDomain(int productID)
        {
            IList<ProductDomain> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM ProductDomain WHERE ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<ProductDomain>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<ProductDomainRouting> GetProductDomainRouting(int productDomainID)
        {
            IList<ProductDomainRouting> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM ProductDomainRouting WHERE ProductDomainID=@ProductDomainID");
                q.Parameters.Add("@ProductDomainID", MySqlDbType.Int32).Value = productDomainID;
                res = dao.Load<ProductDomainRouting>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<ShipperProductView> GetShipperProductList(int productID)
        {
            IList<ShipperProductView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select p.ProductName Product, s.Name Shipper, p.ProductID ProductID, sp.ShipperID ShipperID From Product p
					                left outer join ShipperProduct sp on sp.ProductID = p.ProductID
					                left outer join Shipper s on s.ShipperID = sp.ShipperID
                                    where p.ProductIsActive=1 and p.ProductID = @ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<ShipperProductView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public ShipperProduct GetShipperProduct(int productID)
        {
            ShipperProduct res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select * FROM ShipperProduct WHERE ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<ShipperProduct>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public void SaveShipperProducts(List<ShipperProduct> shipperProductList)
        {
            try
            {
                dao.BeginTransaction();
                foreach (var shipperProduct in shipperProductList)
                    if (shipperProduct.ShipperID <= 0)
                    {
                        //delete
                        var q = new MySqlCommand("DELETE FROM ShipperProduct WHERE ProductID=@ProductID");
                        q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = shipperProduct.ProductID;
                        dao.ExecuteNonQuery(q);
                    }
                    else
                        dao.Save<ShipperProduct>(shipperProduct);
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public IList<Shipper> GetShippers()
        {
            IList<Shipper> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from Shipper");
                res = dao.Load<Shipper>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<Shipper>();
            }
            return res;
        }

        public IList<NMIMerchantAccountProduct> GetMIDProductList(int productID)
        {
            IList<NMIMerchantAccountProduct> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select * From Product p
					                inner join NMIMerchantAccountProduct a on p.ProductID = a.ProductID
                                    inner join AssertigyMID am on am.AssertigyMIDID = a.AssertigyMIDID
                                    where p.ProductIsActive=1 and p.ProductID = @ProductID and am.Visible=1");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<NMIMerchantAccountProduct>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public NMIMerchantAccountProduct GetMIDProduct(int ID)
        {
            NMIMerchantAccountProduct res = null;
            try
            {
                res = dao.Load<NMIMerchantAccountProduct>(ID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public void SaveMIDProduct(NMIMerchantAccountProduct midProduct)
        {
            try
            {
                dao.Save<NMIMerchantAccountProduct>(midProduct);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public IList<AssertigyMID> GetMIDs()
        {
            IList<AssertigyMID> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from AssertigyMID");
                res = dao.Load<AssertigyMID>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<AssertigyMID>();
            }
            return res;
        }

        public void DeleteMIDProduct(int id)
        {
            try
            {
                MySqlCommand q = new MySqlCommand(@"DELETE FROM NMIMerchantAccountProduct Where MerchantAccountProductID=@MerchantAccountProductID");
                q.Parameters.Add("@MerchantAccountProductID", MySqlDbType.Int32).Value = id;
                dao.ExecuteNonQuery(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public IList<Inventory> GetInventoryList()
        {
            IList<Inventory> res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select * from Inventory
                        order by SKU
                    ");
                    res = dao.Load<Inventory>(q);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public IList<Inventory> GetInventoryListByProductCodeID(int? productCodeID)
        {
            IList<Inventory> res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select i.* from Inventory i
                        inner join ProductCodeInventory pci on pci.InventoryID=i.InventoryID
                        where pci.ProductCodeID=@ProductCodeID
                        order by SKU
                    ");
                    q.Parameters.Add("@ProductCodeID", MySqlDbType.Int32).Value = productCodeID;
                    res = dao.Load<Inventory>(q);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public IList<ProductInventory> GetProductInventoryList()
        {
            IList<ProductInventory> res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select * from ProductInventory
                    ");
                    res = dao.Load<ProductInventory>(q);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public IList<ProductCodeInventory> GetProductCodeInventoryList()
        {
            IList<ProductCodeInventory> res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select * from ProductCodeInventory
                    ");
                    res = dao.Load<ProductCodeInventory>(q);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public void SaveProductInventoryList(List<int?> inventoryList, int? productID)
        {
            if (dao != null)
            {
                try
                {
                    dao.BeginTransaction();
                    MySqlCommand q = new MySqlCommand(@"
                        Delete from ProductInventory where ProductID=@ProductID
                    ");
                    q.Parameters.AddWithValue("@ProductID", productID);
                    dao.ExecuteNonQuery(q);

                    foreach (var invID in inventoryList)
                    {
                        var obj = new ProductInventory()
                        {
                            ProductID = productID,
                            InventoryID = invID
                        };
                        dao.Save<ProductInventory>(obj);
                    }

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dao.RollbackTransaction();
                    logger.Error(GetType(), ex);
                }
            }
        }

        public void SaveProductProductCodeList(List<int?> prodCodeList, int? productID)
        {
            if (dao != null)
            {
                try
                {
                    dao.BeginTransaction();
                    MySqlCommand q = new MySqlCommand(@"
                        Delete from ProductProductCode where ProductID=@ProductID
                    ");
                    q.Parameters.AddWithValue("@ProductID", productID);
                    dao.ExecuteNonQuery(q);

                    foreach (var prodCodeID in prodCodeList)
                    {
                        var obj = new ProductProductCode()
                        {
                            ProductID = productID,
                            ProductCodeID = prodCodeID
                        };
                        dao.Save<ProductProductCode>(obj);
                    }

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dao.RollbackTransaction();
                    logger.Error(GetType(), ex);
                }
            }
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

        public IList<ProductCodeInfo> GetProductCodeInfoList()
        {
            IList<ProductCodeInfo> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from ProductCodeInfo inner join ProductCode on ProductCode.ProductCodeID=ProductCodeInfo.ProductCodeID");
                res = dao.Load<ProductCodeInfo>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<ProductProductCode> GetProductProductCodeList()
        {
            IList<ProductProductCode> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from ProductProductCode");
                res = dao.Load<ProductProductCode>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<ProductProductCode> GetProductProductCodeListByProductID(int productID)
        {
            IList<ProductProductCode> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from ProductProductCode WHERE ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<ProductProductCode>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }
    
        public IList<Subscription> GetProductSubscriptions(int productID)
        {
            IList<Subscription> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from Subscription WHERE ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<Subscription>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<Subscription>();
            }
            return res;
        }

        public IList<Campaign> GetProductCampaigns(int productID)
        {
            IList<Campaign> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select c.* from Campaign c
                                                   inner join Subscription s on s.SubscriptionID=c.SubscriptionID
                                                   WHERE s.ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<Campaign>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<Campaign>();
            }
            return res;
        }

        public IList<Campaign> GetProductCampaignsBySubscriptionID(int SubscriptionID)
        {
            IList<Campaign> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select c.* from Campaign c
                                                   WHERE c.SubscriptionID=@SubscriptionID");
                q.Parameters.Add("@SubscriptionID", MySqlDbType.Int32).Value = SubscriptionID;
                res = dao.Load<Campaign>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<Campaign>();
            }
            return res;
        }

        public IList<DynamicEmail> GetProductEmails(int productID)
        {
            IList<DynamicEmail> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select de.* from DynamicEmail de
                                                   WHERE de.ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<DynamicEmail>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<DynamicEmail>();
            }
            return res;
        }

        public Campaign GetProductCampaign(int? campaignID)
        {
            Campaign res = null;
            try
            {
                res = dao.Load<Campaign>(campaignID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public Subscription GetOrCreteDefaultProductSubscription(int productID)
        {
            Subscription res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from Subscription " +
                    "where productID = @productID and Recurring = 0 and ProductCode is null");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                res = dao.Load<Subscription>(q).LastOrDefault();
                if (res == null)
                {
                    Product p = EnsureLoad<Product>(productID);

                    res = new Subscription();
                    res.DisplayName = string.Format("One-time sale subscription for {0}", p.ProductName);
                    res.Selectable = false;
                    res.ProductID = productID;
                    res.Recurring = false;
                    res.ShipFirstRebill = false;
                    dao.Save<Subscription>(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }
            return res;
        }

        public Subscription GetProductSubscription(int? subscriptionID)
        {
            Subscription res = null;
            try
            {
                res = dao.Load<Subscription>(subscriptionID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public void SaveCampaign(Campaign campaign)
        {
            try
            {
                dao.Save<Campaign>(campaign);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void HideCampaign(int id)
        {
            try
            {
                MySqlCommand q = new MySqlCommand(@"UPDATE Campaign SET Active=0 Where CampaignID=@CampaignID");
                q.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = id;
                dao.ExecuteNonQuery(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void ShowCampaign(int id)
        {
            try
            {
                MySqlCommand q = new MySqlCommand(@"UPDATE Campaign SET Active=1 Where CampaignID=@CampaignID");
                q.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = id;
                dao.ExecuteNonQuery(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void SaveProductCurrency(int productID, int currencyID)
        {
            try
            {
                MySqlCommand q = null;
                if (currencyID == 0)
                {
                    //delete currency for this product
                    q = new MySqlCommand("DELETE FROM ProductCurrency WHERE ProductID=@ProductID");
                    q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                    dao.ExecuteNonQuery(q);
                }
                else
                {
                    ProductCurrency pc = new ProductCurrency();
                    pc.ProductID = productID;
                    pc.CurrencyID = currencyID;
                    q = new MySqlCommand("SELECT * FROM ProductCurrency WHERE ProductID=@ProductID");
                    q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                    var itemFromDB = dao.Load<ProductCurrency>(q).FirstOrDefault();

                    if (itemFromDB != null)
                        pc.ProductCurrencyID = itemFromDB.ProductCurrencyID;

                    dao.Save<ProductCurrency>(pc);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public Currency GetProductCurrency(int productID)
        {
            Currency res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select c.* from Currency c
                        inner join ProductCurrency pc on pc.CurrencyID=c.CurrencyID
                        where pc.ProductID=@ProductID");
                    q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                    res = dao.Load<Currency>(q).SingleOrDefault();
                }
                catch (Exception ex)
                {
                    res = null;
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public IList<Currency> GetCurrencyList()
        {
            IList<Currency> res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select * from Currency ");
                    res = dao.Load<Currency>(q);
                }
                catch (Exception ex)
                {
                    res = null;
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }
        
        public IList<ProductEvent> GetProductEvents(int? productID)
        {
            IList<ProductEvent> res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select * from ProductEvents WHERE ProductID=@ProductID ");
                    q.Parameters.AddWithValue("@ProductID", productID);
                    res = dao.Load<ProductEvent>(q);
                }
                catch (Exception ex)
                {
                    res = new List<ProductEvent>();
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public ProductEvent GetProductEvent(int? productEventID)
        {
            ProductEvent res = null;

            if (dao != null)
            {
                try
                {
                    res = dao.Load<ProductEvent>(productEventID);
                }
                catch (Exception ex)
                {
                    res = null;
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public void DeleteProductEvent(int? productEventID)
        {
            try
            {
                MySqlCommand q = new MySqlCommand(@"DELETE FROM ProductEvents Where ProductEventID=@ProductEventID");
                q.Parameters.Add("@ProductEventID", MySqlDbType.Int32).Value = productEventID;
                dao.ExecuteNonQuery(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void SaveProductEvent(ProductEvent prEvent)
        {
            try
            {
                dao.Save<ProductEvent>(prEvent);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public ProductCodeInfo GetProductCodeInfo(int? productCodeID)
        {
            ProductCodeInfo res = null;

            if (dao != null)
            {
                try
                {
                    res = dao.Load<ProductCodeInfo>(productCodeID);
                }
                catch (Exception ex)
                {
                    res = null;
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public ProductCodeInfo GetProductCodeInfo(string productCode)
        {
            ProductCodeInfo res = null;

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"SELECT * FROM ProductCodeInfo pi 
                                    inner join ProductCode p on pi.ProductCodeID=p.ProductCodeID
                                    and p.ProductCode=@ProductCode");
                    q.Parameters.AddWithValue("ProductCode", productCode);
                    res = dao.Load<ProductCodeInfo>(q).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    res = null;
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public ProductWiki GetDocLink(int? productID)
        {
            ProductWiki res = null;

            if (dao != null)
            {
                try
                {
                    res = dao.Load<ProductWiki>(productID);
                }
                catch (Exception ex)
                {
                    res = null;
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public void DeleteDocLink(int? productID)
        {
            try
            {
                MySqlCommand q = new MySqlCommand(@"DELETE FROM ProductWiki Where ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                dao.ExecuteNonQuery(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void SaveProductWiki(ProductWiki prWiki)
        {
            try
            {
                dao.Save<ProductWiki>(prWiki);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public bool DuplicateProduct(Product existProduct, string newProductName)
        {
            try
            {
                dao.BeginTransaction();
                int newProductID = CopyAndCreateProducts(existProduct, newProductName);
                CopyMIDSettigs(existProduct.ProductID.Value, newProductID);
                CopyShipperSettigs(existProduct.ProductID.Value, newProductID);
                CopySubscriptionSettigs(existProduct.ProductID.Value, newProductID);
                CopyTrafficRouting(existProduct.ProductID.Value, newProductID);
                CopyDomainRouting(existProduct.ProductID.Value, newProductID);
                CopyEmailSettings(existProduct.ProductID.Value, newProductID);
                CopyLeadSettings(existProduct.ProductID.Value, newProductID);
                CopyEventsSettings(existProduct.ProductID.Value, newProductID);
                CopyProductList(existProduct.ProductID.Value, newProductID);
                CopyDocumentationLink(existProduct.ProductID.Value, newProductID);
                dao.CommitTransaction();

                return true;
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
                return false;
            }
        }

        private int CopyAndCreateProducts(Product existProduct, string newProductName)
        {
            Product newProduct = new Product();
            newProduct.Code = existProduct.Code;
            newProduct.ProductIsActive = existProduct.ProductIsActive;
            newProduct.ProductName = newProductName;
            new BaseService().Save<Product>(newProduct);

            return newProduct.ProductID.Value;
        }

        private void CopyMIDSettigs(int existProductID, int newProductID)
        {
            IList<NMIMerchantAccountProduct> existProdMIDList = GetMIDProductList(existProductID);
            foreach (NMIMerchantAccountProduct prodMID in existProdMIDList)
            {
                NMIMerchantAccountProduct newMIDProduct = new NMIMerchantAccountProduct()
                {
                    ProductID = newProductID,
                    AssertigyMIDID = prodMID.AssertigyMIDID,
                    Percentage = prodMID.Percentage,
                    OnlyRefundCredit = prodMID.OnlyRefundCredit,
                    UseForRebill = prodMID.UseForRebill,
                    UseForTrial = prodMID.UseForTrial,
                    QueueRebills = prodMID.QueueRebills,
                    RolloverAssertigyMIDID = prodMID.RolloverAssertigyMIDID,
                    MerchantAccountProductID = null
                };
                dao.Save<NMIMerchantAccountProduct>(newMIDProduct);
            }
            SaveProductCurrency(newProductID, GetExistingProductCurrency(existProductID));
        }

        private void CopyShipperSettigs(int existProductID, int newProductID)
        {
            ShipperProduct shipperProduct = GetShipperProduct(existProductID);
            if (shipperProduct != null)
            {
                ShipperProduct newShipperProduct = new ShipperProduct()
                {
                    NeedConfirm = shipperProduct.NeedConfirm,
                    ProductID = newProductID,
                    ShipperID = shipperProduct.ShipperID
                };
                dao.Save<ShipperProduct>(newShipperProduct);
            }

        }

        private void CopySubscriptionSettigs(int existProductID, int newProductID)
        {
            IList<Subscription> existProdSubscriptionList = GetProductSubscriptions(existProductID);
            foreach (Subscription prodSubscription in existProdSubscriptionList)
            {
                Subscription newSubscriptionProduct = new Subscription()
                {
                    ProductID = newProductID,
                    Description = prodSubscription.Description,
                    DisplayName = prodSubscription.DisplayName,
                    InitialBillAmount = prodSubscription.InitialBillAmount,
                    InitialInterim = prodSubscription.InitialInterim,
                    InitialShipping = prodSubscription.InitialShipping,
                    ParentSubscriptionID = prodSubscription.ParentSubscriptionID,
                    ProductCode = prodSubscription.ProductCode,
                    ProductName = prodSubscription.ProductName,
                    Quantity = prodSubscription.Quantity,
                    QuantitySKU2 = prodSubscription.QuantitySKU2,
                    Recurring = prodSubscription.Recurring,
                    RecurText = prodSubscription.RecurText,
                    RegularBillAmount = prodSubscription.RegularBillAmount,
                    RegularInterim = prodSubscription.RegularInterim,
                    RegularShipping = prodSubscription.RegularShipping,
                    ReplacementText = prodSubscription.ReplacementText,
                    ReplacementTrialText = prodSubscription.ReplacementTrialText,
                    SaveBilling = prodSubscription.SaveBilling,
                    SaveShipping = prodSubscription.SaveShipping,
                    SaveText = prodSubscription.SaveText,
                    SecondBillAmount = prodSubscription.SecondBillAmount,
                    SecondInterim = prodSubscription.SecondInterim,
                    SecondShipping = prodSubscription.SecondShipping,
                    Selectable = prodSubscription.Selectable,
                    ShipFirstRebill = prodSubscription.ShipFirstRebill,
                    SKU2 = prodSubscription.SKU2,
                    TrialText = prodSubscription.TrialText,
                    UpsellText = prodSubscription.UpsellText,
                    SubscriptionID = null
                };
                dao.Save<Subscription>(newSubscriptionProduct);
                //CopyFlowSettigs(prodSubscription.SubscriptionID.Value, newSubscriptionProduct.SubscriptionID.Value);
            }
        }

        private void CopyTrafficRouting(int existProductID, int newProductID)
        {
            ProductRouting productRouting = GetProductRouting(existProductID);
            if (productRouting != null)
            {
                ProductRouting newProductRouting = new ProductRouting()
                {
                    ProductRoutingID = null,
                    ProductID = newProductID,
                    RoutingURL = productRouting.RoutingURL
                };
                dao.Save<ProductRouting>(newProductRouting);
            }
        }

        private void CopyDomainRouting(int existProductID, int newProductID)
        {
            IList<ProductDomain> existProductDomainList = GetProductDomain(existProductID);
            foreach (ProductDomain existProductDomain in existProductDomainList)
            {
                ProductDomain newProductDomain = new ProductDomain()
                {
                    ProductDomainID = null,
                    ProductID = newProductID,
                    DomainName = existProductDomain.DomainName
                };

                dao.Save<ProductDomain>(newProductDomain);

                IList<ProductDomainRouting> existProductDomainRoutingList = GetProductDomainRouting(existProductDomain.ProductDomainID.Value);
                foreach (ProductDomainRouting existProductDomainRouting in existProductDomainRoutingList)
                {
                    ProductDomainRouting newProductDomainRouting = new ProductDomainRouting()
                    {
                        ProductDomainRoutingID = null,
                        ProductDomainID = newProductDomain.ProductDomainID,
                        Percentage = existProductDomainRouting.Percentage,
                        CampaignID = existProductDomainRouting.CampaignID,
                        ExtUrl = existProductDomainRouting.ExtUrl,
                        Affiliate = existProductDomainRouting.Affiliate,
                        SubAffiliate = existProductDomainRouting.SubAffiliate
                    };

                    dao.Save<ProductDomainRouting>(newProductDomainRouting);
                }
            }
        }

        private void CopyFlowSettigs(int exisSubscriptionID, int newSubscriptionID)
        {
            IList<Campaign> existProdCampaignList = GetProductCampaignsBySubscriptionID(exisSubscriptionID);
            foreach (Campaign prodCampaign in existProdCampaignList)
            {
                Campaign newCampaignProduct = new Campaign()
                {
                    Active = prodCampaign.Active,
                    CreateDT = prodCampaign.CreateDT,
                    DisplayName = prodCampaign.DisplayName,
                    EnableFitFactory = prodCampaign.EnableFitFactory,
                    IsDupeChecking = prodCampaign.IsDupeChecking,
                    IsExternal = prodCampaign.IsExternal,
                    IsMerchant = prodCampaign.IsMerchant,
                    IsRiskScoring = prodCampaign.IsRiskScoring,
                    IsSave = prodCampaign.IsSave,
                    IsSTO = prodCampaign.IsSTO,
                    ParentCampaignID = prodCampaign.ParentCampaignID,
                    Percentage = prodCampaign.Percentage,
                    Redirect = prodCampaign.Redirect,
                    RedirectURL = prodCampaign.RedirectURL,
                    SendUserEmail = prodCampaign.SendUserEmail,
                    ShipperID = prodCampaign.ShipperID,
                    SubscriptionID = newSubscriptionID,
                    URL = prodCampaign.URL,
                    CampaignID = null
                };
                dao.Save<Campaign>(newCampaignProduct);
            }
        }

        private void CopyEmailSettings(int existProductID, int newProductID)
        {
            IList<DynamicEmail> prodEmail = GetProductEmails(existProductID);
            foreach (DynamicEmail email in prodEmail)
            {
                DynamicEmail newProductEmail = new DynamicEmail()
                {
                    Active = email.Active,
                    Content = email.Content,
                    Days = email.Days,
                    DynamicEmailID = null,
                    DynamicEmailTypeID = email.DynamicEmailTypeID,
                    FromAddress = email.FromAddress,
                    FromName = email.FromName,
                    Landing = email.Landing,
                    LandingLink = email.LandingLink,
                    ProductID = newProductID,
                    Subject = email.Subject
                };
                dao.Save<DynamicEmail>(newProductEmail);
            }
        }

        private void CopyLeadSettings(int existProductID, int newProductID)
        {
            LeadService service = new LeadService();
            IList<LeadRouting> leadRoutingList = service.GetRoutingRules(existProductID);
            IList<LeadPartnerConfigValue> leadPartnerConfigList = service.GetLeadPartnerConfigValues(existProductID);
            foreach (LeadRouting leadRouting in leadRoutingList)
            {
                LeadRouting newLeadRouting = new LeadRouting()
                {
                    LeadPartnerID = leadRouting.LeadPartnerID,
                    LeadRoutingID = null,
                    LeadTypeID = leadRouting.LeadTypeID,
                    Percentage = leadRouting.Percentage,
                    ProductID = newProductID
                };
                dao.Save<LeadRouting>(newLeadRouting);
            }

            foreach (LeadPartnerConfigValue leadPartnerConfig in leadPartnerConfigList)
            {
                LeadPartnerConfigValue newLeadPartnerConfig = new LeadPartnerConfigValue()
                {
                    Key = leadPartnerConfig.Key,
                    LeadPartnerConfigValueID = null,
                    LeadPartnerID = leadPartnerConfig.LeadPartnerID,
                    LeadTypeID = leadPartnerConfig.LeadTypeID,
                    ProductID = newProductID,
                    Value = leadPartnerConfig.Value
                };
                dao.Save<LeadPartnerConfigValue>(newLeadPartnerConfig);
            }
        }

        private void CopyEventsSettings(int existProductID, int newProductID)
        {
            IList<ProductEvent> productEventsList = GetProductEvents(existProductID);
            foreach (ProductEvent productEvents in productEventsList)
            {
                ProductEvent newProductEvents = new ProductEvent()
                {
                    EventTypeID = productEvents.EventTypeID,
                    ProductEventID = null,
                    ProductID = newProductID,
                    URl = productEvents.URl
                };
                dao.Save<ProductEvent>(newProductEvents);
            }
        }

        private void CopyProductList(int existProductID, int newProductID)
        {
            IList<ProductProductCode> productProductCodeList = GetProductProductCodeListByProductID(existProductID);
            foreach (ProductProductCode productProductCode in productProductCodeList)
            {
                ProductProductCode newProductProductCode = new TrimFuel.Model.ProductProductCode()
                {
                    ProductProductCodeID = null,
                    ProductID = newProductID,
                    ProductCodeID = productProductCode.ProductCodeID
                };
                dao.Save<ProductProductCode>(newProductProductCode);
            }
        }

        private void CopyDocumentationLink(int existProductID, int newProductID)
        {
            ProductWiki productWikiDoc = GetDocLink(existProductID);
            if (productWikiDoc != null)
            {
                ProductWiki newProductWikiDoc = new ProductWiki()
                {
                    Path = productWikiDoc.Path,
                    ProductID = newProductID
                };
                dao.Save<ProductWiki>(newProductWikiDoc);
            }
        }

        private int GetExistingProductCurrency(int existProductID)
        {
            var item = GetProductCurrency(existProductID);
            return item == null ? 0 : item.CurrencyID.Value;
        }
    }
}
