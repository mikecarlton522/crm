using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Business.Dao;

namespace TrimFuel.Business
{
    public class TPClientService : BaseService
    {
        public TPClientService()
        {
            dao = null;
        }

        public IList<TPClient> GetClientList()
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            IList<TPClient> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from TPClient");
                res = dao.Load<TPClient>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public TPClient GetClientByName(string clientName)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            TPClient res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from TPClient where Name=@Name");
                q.Parameters.Add("@Name", MySqlDbType.VarChar).Value = clientName;
                res = dao.Load<TPClient>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<TPClientNoteView> GetClientNotes(int clientID)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            IList<TPClientNoteView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from TPClientNote e inner join Admin a on a.AdminID=e.AdminID Where e.TPClientID=@TPClientID order by e.CreateDT desc");
                q.Parameters.Add("TPClientID", MySqlDbType.Int32).Value = clientID;
                res = dao.Load<TPClientNoteView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<TPClientNoteView>();
            }
            return res;
        }

        public TPClientNote GetClientNote(int noteID)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            TPClientNote res = null;
            try
            {
                res = dao.Load<TPClientNote>(noteID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public TPClientEmail GetClientEmail(int emailID)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            TPClientEmail res = null;
            try
            {
                res = dao.Load<TPClientEmail>(emailID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<TPClientEmailView> GetClientEmails(int clientID)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            IList<TPClientEmailView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from TPClientEmail e inner join Admin a on a.AdminID=e.AdminID Where e.TPClientID=@TPClientID  order by e.CreateDT desc");
                q.Parameters.Add("TPClientID", MySqlDbType.Int32).Value = clientID;
                res = dao.Load<TPClientEmailView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<TPClientEmailView>();
            }
            return res;
        }

        public TPClient GetClient(int ClientId)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            TPClient res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from TPClient where TPClientID=@TPClientID");
                q.Parameters.Add("TPClientID", ClientId);
                res = dao.Load<TPClient>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public void SaveClient(TPClient client)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            try
            {
                dao.Save<TPClient>(client);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public TPClientSetting GetClientSetting(int ClientId)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            TPClientSetting res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from TPClientSettings where id_tpclient=@TPClientID");
                q.Parameters.Add("TPClientID", ClientId);
                res = dao.Load<TPClientSetting>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public void SaveClientSetting(TPClientSetting settings)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            try
            {
                dao.Save<TPClientSetting>(settings);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void SaveClientNote(TPClientNote note)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            try
            {
                dao.Save<TPClientNote>(note);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        // Shipper

        public TPClientShipperSettings GetClientShipperSettings(int shipperID, int ClientID)
        {
            dao = GetClientDao(ClientID);
            TPClientShipperSettings res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from ShipperSettings ss inner join Shipper s
                                      on s.ShipperID=ss.ShipperID where s.ShipperID=@ShipperID");
                q.Parameters.Add("ShipperID", shipperID);
                res = dao.Load<TPClientShipperSettings>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public List<ShipperConfig> GetShipperConfig(int shipperID, int clientID)
        {
            dao = GetClientDao(clientID);
            List<ShipperConfig> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from ShipperConfig sc inner join Shipper s
                                      on s.ShipperID=sc.ShipperID where s.ShipperID=@ShipperID");
                q.Parameters.Add("ShipperID", shipperID);
                res = dao.Load<ShipperConfig>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public void SaveShipperSettings(int clientID, TPClientShipperSettings shipperSettings, List<ShipperConfig> configFields, string shipperName, int shipperID)
        {
            dao = GetClientDao(clientID);
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("Select * from Shipper where ShipperID=@ShipperID");
                q.Parameters.Add("ShipperID", shipperID);
                var shipper = dao.Load<Shipper>(q).SingleOrDefault();
                if (shipper == null)
                {
                    shipper = new Shipper();
                    shipper.Name = shipperName;
                    shipper.ShipperID = shipperID;
                }
                shipper.ServiceIsActive = true;
                dao.Save(shipper);

                // save config fields

                foreach (var config in configFields)
                {
                    config.ShipperID = shipper.ShipperID;
                    ShipperConfig.ID shipperConfigID = new ShipperConfig.ID()
                    {
                        Key = config.Key,
                        ShipperID = config.ShipperID.Value
                    };
                    var configFromDB = dao.Load<ShipperConfig>(shipperConfigID);
                    if (configFromDB != null)
                    {
                        config.ShipperConfigID = configFromDB.ShipperConfigID;
                    }
                    dao.Save(config);
                }

                //------------------

                shipperSettings.ShipperID = shipper.ShipperID;
                dao.Save(shipperSettings);
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public IList<Shipper> GetClientShippers(int clientID)
        {
            dao = GetClientDao(clientID);
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

        public void SaveShipperProducts(int clientID, List<ShipperProduct> shipperProductList)
        {
            dao = GetClientDao(clientID);
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

        // ---------------

        // Gateway

        public void SaveGateway(NMICompany company, int clientID)
        {
            dao = GetClientDao(clientID);
            try
            {
                dao.BeginTransaction();
                if (company.NMICompanyID == null)
                {
                    //add midcategory
                    MIDCategory category = new MIDCategory()
                    {
                        DisplayName = company.CompanyName
                    };
                    dao.Save<MIDCategory>(category);
                }
                dao.Save(company);
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public IList<NMICompany> GetClientGatewayServices(int clientID)
        {
            dao = GetClientDao(clientID);
            IList<NMICompany> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from NMICompany");
                res = dao.Load<NMICompany>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<NMICompany>();
            }
            return res;
        }

        public NMICompany GetClientGatewayService(int nmiCompanyID, int clientID)
        {
            dao = GetClientDao(clientID);
            NMICompany res = null;
            try
            {
                res = dao.Load<NMICompany>(nmiCompanyID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<AssertigyMID> GetAssertigyMIDList(int companyID, int clientID)
        {
            dao = GetClientDao(clientID);
            IList<AssertigyMID> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from AssertigyMID aMID
                                                    inner join NMICompanyMID nMID on nMID.AssertigyMIDID=aMID.AssertigyMIDID
                                                    inner join NMICompany n on n.NMICompanyID=nMID.NMICompanyID
                                                    WHERE n.NMICompanyID=@NMICompanyID");
                q.Parameters.Add("@NMICompanyID", MySqlDbType.VarChar).Value = companyID;
                res = dao.Load<AssertigyMID>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<AssertigyMID>();
            }
            return res;
        }

        public IList<AssertigyMID> GetAllAssertigyMIDList(int clientID)
        {
            dao = GetClientDao(clientID);
            IList<AssertigyMID> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from AssertigyMID aMID
                                                    WHERE aMID.Visible=1");
                res = dao.Load<AssertigyMID>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<AssertigyMID>();
            }
            return res;
        }

        public IList<AssertigyMIDSettings> GetAssertigyMIDSettingsList(int companyID, int clientID)
        {
            dao = GetClientDao(clientID);
            IList<AssertigyMIDSettings> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from AssertigyMIDSettings aMID
                                                    inner join NMICompanyMID nMID on nMID.AssertigyMIDID=aMID.AssertigyMIDID
                                                    inner join NMICompany n on n.NMICompanyID=nMID.NMICompanyID
                                                    WHERE n.NMICompanyID=@NMICompanyID");
                q.Parameters.Add("@NMICompanyID", MySqlDbType.VarChar).Value = companyID;
                res = dao.Load<AssertigyMIDSettings>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<AssertigyMIDSettings>();
            }
            return res;
        }


        public AssertigyMID GetAssertigyMID(int mid, int clientID)
        {
            dao = GetClientDao(clientID);
            AssertigyMID res = null;
            try
            {
                res = dao.Load<AssertigyMID>(mid);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public AssertigyMIDSettings GetAssertigyMIDSetting(int mid, int clientID)
        {
            dao = GetClientDao(clientID);
            AssertigyMIDSettings res = null;
            try
            {
                res = dao.Load<AssertigyMIDSettings>(mid);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<MIDCategory> GetMIDCategoryList(int clientID)
        {
            dao = GetClientDao(clientID);
            IList<MIDCategory> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from MIDCategory");
                res = dao.Load<MIDCategory>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<MIDCategory>();
            }
            return res;
        }

        public void SaveAssertigyMID(AssertigyMID assertigy, int clientID, int nmiCompanyID, List<int?> paymentTypesID, List<int?> productsID, AssertigyMIDSettings setting)
        {
            dao = GetClientDao(clientID);
            try
            {
                bool isNew = assertigy.AssertigyMIDID == null ? true : false;
                dao.BeginTransaction();

                //save assertigy
                MySqlCommand q = null;
                dao.Save<AssertigyMID>(assertigy);
                if (isNew)
                {
                    q = new MySqlCommand("INSERT INTO NMICompanyMID (NMICompanyID, AssertigyMIDID) VALUES(@NMICompanyID, @AssertigyMIDID)");
                    q.Parameters.Add("@NMICompanyID", MySqlDbType.Int32).Value = nmiCompanyID;
                    q.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = assertigy.AssertigyMIDID;
                    dao.ExecuteNonQuery(q);
                }

                // save payments types
                q = new MySqlCommand("DELETE FROM AssertigyMIDPaymentType WHERE AssertigyMIDID=@AssertigyMIDID");
                q.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = assertigy.AssertigyMIDID;
                dao.ExecuteNonQuery(q);
                foreach (var typeID in paymentTypesID)
                {
                    q = new MySqlCommand("INSERT INTO AssertigyMIDPaymentType (AssertigyMIDID, PaymentTypeID) VALUES(@AssertigyMIDID, @PaymentTypeID)");
                    q.Parameters.Add("@PaymentTypeID", MySqlDbType.Int32).Value = typeID;
                    q.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = assertigy.AssertigyMIDID;
                    dao.ExecuteNonQuery(q);
                }
                // ------------------------

                // save products
                q = new MySqlCommand("UPDATE NMIMerchantAccountProduct SET UseForTrial=0 WHERE AssertigyMIDID=@AssertigyMIDID");
                q.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = assertigy.AssertigyMIDID;
                dao.ExecuteNonQuery(q);
                foreach (var productID in productsID)
                {
                    q = new MySqlCommand("SELECT * From NMIMerchantAccountProduct WHERE AssertigyMIDID=@AssertigyMIDID AND ProductID=@ProductID");
                    q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                    q.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = assertigy.AssertigyMIDID;

                    var item = dao.Load<NMIMerchantAccountProduct>(q).FirstOrDefault();
                    if (item != null)
                        item.UseForTrial = true;
                    else
                        item = new NMIMerchantAccountProduct()
                            {
                                AssertigyMIDID = assertigy.AssertigyMIDID,
                                UseForTrial = true,
                                ProductID = productID,
                                Percentage = 100,
                                UseForRebill = true,
                                OnlyRefundCredit = false,
                                QueueRebills = false
                            };

                    dao.Save<NMIMerchantAccountProduct>(item);
                }

                var currSetting = dao.Load<AssertigyMIDSettings>(assertigy.AssertigyMIDID);
                if (currSetting == null)
                {
                    currSetting = new AssertigyMIDSettings();
                    currSetting.AssertigyMIDSettingID = null;
                }
                currSetting.AssertigyMIDID = assertigy.AssertigyMIDID;
                currSetting.ChargebackFee = setting.ChargebackFee;
                currSetting.ChargebackRepresentationFee = setting.ChargebackRepresentationFee;
                currSetting.ChargebackRepresentationFeeRetail = setting.ChargebackRepresentationFeeRetail;
                currSetting.TransactionFee = setting.TransactionFee;
                currSetting.DiscountRate = setting.DiscountRate;

                currSetting.GatewayFee = setting.GatewayFee;
                currSetting.GatewayFeeRetail = setting.GatewayFeeRetail;

                dao.Save<AssertigyMIDSettings>(currSetting);

                // ---------------
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public void SaveProductMerchantAccount(int clientID, int? productID, int currencyID, List<int> accounts)
        {
            dao = GetClientDao(clientID);
            try
            {
                dao.BeginTransaction();

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

                // save accounts
                q = new MySqlCommand("UPDATE NMIMerchantAccountProduct SET UseForTrial=0 WHERE ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                dao.ExecuteNonQuery(q);
                foreach (var account in accounts)
                {
                    q = new MySqlCommand("SELECT * From NMIMerchantAccountProduct WHERE AssertigyMIDID=@AssertigyMIDID AND ProductID=@ProductID");
                    q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                    q.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = account;

                    var item = dao.Load<NMIMerchantAccountProduct>(q).FirstOrDefault();
                    if (item != null)
                        item.UseForTrial = true;
                    else
                        item = new NMIMerchantAccountProduct()
                        {
                            AssertigyMIDID = account,
                            UseForTrial = true,
                            ProductID = productID,
                            Percentage = 100,
                            UseForRebill = true,
                            OnlyRefundCredit = false,
                            QueueRebills = false
                        };

                    dao.Save<NMIMerchantAccountProduct>(item);
                }
                // ---------------
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public void DeleteMID(int clientID, int? AssertigyMIDID)
        {
            dao = GetClientDao(clientID);
            try
            {
                dao.BeginTransaction();

                var item = dao.Load<AssertigyMID>(AssertigyMIDID);
                item.Deleted = true;
                item.Visible = false;
                dao.Save<AssertigyMID>(item);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public void DeleteGateway(int clientID, int? nmiCompanyID)
        {
            dao = GetClientDao(clientID);
            try
            {
                dao.BeginTransaction();

                var item = dao.Load<NMICompany>(nmiCompanyID);
                item.Deleted = true;
                dao.Save<NMICompany>(item);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }


        public IList<Product> GetProductList(int clientID)
        {
            IList<Product> res = null;
            dao = GetClientDao(clientID);

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select * from Product
                        where ProductIsActive = 1
                        order by ProductName
                    ");
                    res = dao.Load<Product>(q);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public Currency GetProductCurrency(int clientID, int productID)
        {
            Currency res = null;
            dao = GetClientDao(clientID);

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select * from Currency c
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

        public IList<Currency> GetCurrencyList(int clientID)
        {
            IList<Currency> res = null;
            dao = GetClientDao(clientID);

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

        public IList<Inventory> GetInventoryList(int clientID)
        {
            IList<Inventory> res = null;
            dao = GetClientDao(clientID);

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

        public IList<ProductCode> GetProductCodeList(int clientID)
        {
            dao = GetClientDao(clientID);
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

        public IList<ShipperProductView> GetShipperProductList(int clientID)
        {
            IList<ShipperProductView> res = null;
            dao = GetClientDao(clientID);

            try
            {
                MySqlCommand q = new MySqlCommand(@"select p.ProductName Product, s.Name Shipper, p.ProductID ProductID, sp.ShipperID ShipperID From Product p
					                left outer join ShipperProduct sp on sp.ProductID = p.ProductID
					                left outer join Shipper s on s.ShipperID = sp.ShipperID
                                    where p.ProductIsActive=1");
                res = dao.Load<ShipperProductView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public void DeleteShipperProduct(int clientID, int productID)
        {
            dao = GetClientDao(clientID);

            try
            {
                MySqlCommand q = new MySqlCommand(@"DELETE FROM ShipperProduct Where ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                dao.ExecuteNonQuery(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public IList<ProductCodeInventory> GetProductCodeInventoryList(int clientID, int productCodeID)
        {
            IList<ProductCodeInventory> res = null;
            dao = GetClientDao(clientID);

            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"select * from ProductCodeInventory where ProductCodeID=@ProductCodeID");
                    q.Parameters.Add("@ProductCodeID", MySqlDbType.VarChar).Value = productCodeID;
                    res = dao.Load<ProductCodeInventory>(q);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public ProductCode SaveProductCode(int clientID, List<ProductCodeInventory> items, int? productCodeID, string sku)
        {
            ProductCode productCode = null;
            dao = GetClientDao(clientID);

            if (dao != null)
            {
                try
                {
                    dao.BeginTransaction();
                    if (productCodeID != null)
                    {
                        MySqlCommand q = new MySqlCommand("DELETE FROM ProductCodeInventory WHERE ProductCodeID=@ProductCodeID");
                        q.Parameters.Add("@ProductCodeID", MySqlDbType.Int32).Value = productCodeID;
                        dao.ExecuteNonQuery(q);
                        productCode = dao.Load<ProductCode>(productCodeID);
                    }
                    else
                    {
                        productCode = new ProductCode();
                        productCode.Name = string.Empty;
                    }

                    productCode.ProductCode_ = sku;
                    dao.Save<ProductCode>(productCode);

                    foreach (var item in items)
                    {
                        item.ProductCodeID = productCode.ProductCodeID;
                        dao.Save<ProductCodeInventory>(item);
                    }

                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    productCode = null;
                    dao.RollbackTransaction();
                    logger.Error(GetType(), ex);
                }
            }
            return productCode;
        }

        public ProductCode GetProductCode(int clientID, int productCodeID)
        {
            ProductCode res = null;
            dao = GetClientDao(clientID);

            if (dao != null)
            {
                try
                {
                    res = dao.Load<ProductCode>(productCodeID);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public Product GetProduct(int clientID, int productID)
        {
            Product res = null;
            dao = GetClientDao(clientID);

            if (dao != null)
            {
                try
                {
                    res = dao.Load<Product>(productID);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public Product SaveProduct(int clientID, int? productID, string productName)
        {
            Product res = null;
            dao = GetClientDao(clientID);

            if (dao != null)
            {
                try
                {
                    if (productID == null)
                    {
                        res = new Product();
                        res.ProductIsActive = true;
                    }
                    else
                    {
                        res = dao.Load<Product>(productID);
                    }
                    res.ProductName = productName;
                    dao.Save<Product>(res);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public Inventory GetInventory(int clientID, int inventoryID)
        {
            Inventory res = null;
            dao = GetClientDao(clientID);

            if (dao != null)
            {
                try
                {
                    res = dao.Load<Inventory>(inventoryID);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public Inventory SaveInventory(int clientID, int? inventoryID, string sku, string product)
        {
            Inventory res = null;
            dao = GetClientDao(clientID);

            if (dao != null)
            {
                bool isNew = false;
                try
                {
                    dao.BeginTransaction();                    

                    if (string.IsNullOrEmpty(sku))
                    {
                    }
                    else if (string.IsNullOrEmpty(product))
                    {
                    }
                    else
                    {
                        ProductSKU pSKU = dao.Load<ProductSKU>(sku);
                        if (!(inventoryID == null && pSKU != null))
                        {
                            if (inventoryID == null)
                            {
                                res = new Inventory();
                                res.SKU = sku;
                                res.Costs = 0M;
                                res.InStock = 0;
                                res.RetailPrice = 0M;
                                isNew = true;

                                pSKU = new ProductSKU();
                                pSKU.ProductSKU_ = sku;
                            }
                            else
                            {
                                res = dao.Load<Inventory>(inventoryID);
                            }

                            pSKU.ProductName = product;
                            dao.Save(pSKU);

                            res.Product = product;
                            dao.Save<Inventory>(res);

                            if (isNew)
                            {
                                var newProductCode = new ProductCode { ProductCode_ = sku, Name = string.Empty };
                                dao.Save<ProductCode>(newProductCode);
                                dao.Save<ProductCodeInventory>(new ProductCodeInventory
                                {
                                    InventoryID = res.InventoryID,
                                    ProductCodeID = newProductCode.ProductCodeID,
                                    Quantity = 1
                                });
                            }
                        }
                    }
                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    dao.RollbackTransaction();
                    logger.Error(GetType(), ex);
                }
            }
            return res;
        }

        public void DeleteProduct(int clientID, int productID)
        {
            dao = GetClientDao(clientID);
            if (dao != null)
            {
                try
                {
                    Product res = dao.Load<Product>(productID);
                    res.ProductIsActive = false;
                    dao.Save<Product>(res);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
        }

        public TPClient SaveTPClientBillingAPICredentials(int tpClientID, string billingAPIUsername, string billingAPIPassword)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            TPClient res = null;
            try
            {
                dao.BeginTransaction();

                res = dao.Load<TPClient>(tpClientID);
                res.Username = billingAPIUsername;
                res.Password = billingAPIPassword;
                dao.Save<TPClient>(res);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public IDao GetClientDao(int clientID)
        {
            IDao clientdao = null;
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            if (clientID != -1)
            {
                try
                {
                    TPClient client = dao.Load<TPClient>(clientID);
                    clientdao = new MySqlDao(client.ConnectionString);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
            else
            {
                clientdao = dao;
            }
            return clientdao;
        }

        public IList<PaymentType> GetAssertigyMIDPaymentTypes(int mid, int clientID)
        {
            dao = GetClientDao(clientID);
            IList<PaymentType> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"SELECT * FROM AssertigyMIDPaymentType ap
                                                    inner join PaymentType p on ap.PaymentTypeID=p.PaymentTypeID
                                                    WHERE ap.AssertigyMIDID=@AssertigyMIDID");
                q.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = mid;
                res = dao.Load<PaymentType>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<PaymentType> GetAllPaymentTypes(int clientID)
        {
            IList<PaymentType> res = null;
            dao = GetClientDao(clientID);
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM PaymentType");
                res = dao.Load<PaymentType>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Product> GetAssertigyMIDProducts(int mid, int clientID)
        {
            IList<Product> res = null;
            dao = GetClientDao(clientID);
            try
            {
                MySqlCommand q = new MySqlCommand(@"SELECT * FROM NMIMerchantAccountProduct mp
                                                    inner join Product p on mp.ProductID=p.ProductID
                                                    WHERE mp.AssertigyMIDID=@AssertigyMIDID
                                                    AND mp.UseForTrial=1");
                q.Parameters.Add("@AssertigyMIDID", MySqlDbType.Int32).Value = mid;
                res = dao.Load<Product>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Product> GetAllProducts(int clientID)
        {
            IList<Product> res = null;
            dao = GetClientDao(clientID);
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT * FROM Product");
                res = dao.Load<Product>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<AssertigyMID> GetProductAssertigyMIDs(int productID, int clientID)
        {
            IList<AssertigyMID> res = null;
            dao = GetClientDao(clientID);
            try
            {
                MySqlCommand q = new MySqlCommand(@"SELECT * FROM NMIMerchantAccountProduct mp
                                                    inner join AssertigyMID a on mp.AssertigyMIDID=a.AssertigyMIDID
                                                    WHERE mp.ProductID=@ProductID AND a.Visible=1 AND mp.UseForTrial=1");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<AssertigyMID>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        //---------------

        // Call Center Outbound

        public IList<LeadPartner> GetClientLeadPartnerList(int clientID)
        {
            IList<LeadPartner> res = null;
            dao = GetClientDao(clientID);
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from LeadPartner");
                res = dao.Load<LeadPartner>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<LeadPartner>();
            }
            return res;
        }

        public IList<LeadType> GetLeadTypes(int clientID)
        {
            IList<LeadType> res = null;
            dao = GetClientDao(clientID);
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from LeadType");
                res = dao.Load<LeadType>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<LeadType>();
            }
            return res;
        }

        public LeadPartnerSettings GetClientLeadPartnerSettings(int leadPartnerID, int ClientID)
        {
            LeadPartnerSettings res = null;
            dao = GetClientDao(ClientID);

            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from LeadPartnerSettings ls inner join LeadPartner l
                                      on l.LeadPartnerID=ls.LeadPartnerID where l.LeadPartnerID=@LeadPartnerID");
                q.Parameters.Add("LeadPartnerID", leadPartnerID);
                res = dao.Load<LeadPartnerSettings>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public LeadPartner GetClientLeadPartner(int leadPartnerID, int ClientID)
        {
            LeadPartner res = null;
            dao = GetClientDao(ClientID);

            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from LeadPartner l where l.LeadPartnerID=@LeadPartnerID");
                q.Parameters.Add("LeadPartnerID", leadPartnerID);
                res = dao.Load<LeadPartner>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public List<LeadPartnerConfigValue> GetLeadPartnerConfigValues(int leadPartnerID, int clientID)
        {
            dao = GetClientDao(clientID);
            List<LeadPartnerConfigValue> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from LeadPartnerConfigValue lc inner join LeadPartner l
                                      on l.LeadPartnerID=lc.LeadPartnerID where l.LeadPartnerID=@LeadPartnerID");
                q.Parameters.Add("LeadPartnerID", leadPartnerID);
                res = dao.Load<LeadPartnerConfigValue>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<LeadPartnerConfigValue>();
            }
            return res;
        }

        public void SaveLeadPartnerConfigValue(int clientID, LeadPartnerConfigValue leadPartnerConfigValue)
        {
            dao = GetClientDao(clientID);
            if (dao != null)
            {
                try
                {
                    dao.Save<LeadPartnerConfigValue>(leadPartnerConfigValue);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
        }

        public void DeleteLeadPartnerConfigValue(int clientID, LeadPartnerConfigValue.ID leadPartnerConfigValueID)
        {
            dao = GetClientDao(clientID);
            if (dao != null)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand(@"DELETE FROM LeadPartnerConfigValue 
                                                      WHERE LeadPartnerID=@LeadPartnerID
                                                      AND LeadTypeID=@LeadTypeID
                                                      AND ProductID=@ProductID
                                                      AND `Key`=@Key");
                    q.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = leadPartnerConfigValueID.LeadPartnerID;
                    q.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = leadPartnerConfigValueID.LeadTypeID;
                    q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = leadPartnerConfigValueID.ProductID;
                    q.Parameters.Add("@Key", MySqlDbType.VarChar).Value = leadPartnerConfigValueID.Key;
                    dao.ExecuteNonQuery(q);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
        }

        public void SaveLeadPartnerSettings(int clientID, LeadPartnerSettings leadPartnerSettings, string leadPartnerName, int leadPartnerID)
        {
            dao = GetClientDao(clientID);

            try
            {
                dao.BeginTransaction();

                var leadPartner = dao.Load<LeadPartner>(leadPartnerID);
                if (leadPartner == null)
                {
                    leadPartner = new LeadPartner();
                    leadPartner.DisplayName = leadPartnerName;
                    leadPartner.LeadPartnerID = leadPartnerID;
                }
                leadPartner.ServiceisActive = true;
                dao.Save<LeadPartner>(leadPartner);

                leadPartnerSettings.LeadPartnerID = leadPartner.LeadPartnerID;
                dao.Save<LeadPartnerSettings>(leadPartnerSettings);
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }
        }

        public IList<LeadRouting> GetRoutingRules(int clientID, int productID)
        {
            IList<LeadRouting> res = null;
            dao = GetClientDao(clientID);

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select lr.* from LeadRouting lr
                    where lr.ProductID = @productID
                    ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<LeadRouting>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<LeadPartner> GetPartnerList(int clientID)
        {
            IList<LeadPartner> res = null;
            dao = GetClientDao(clientID);

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from LeadPartner where ServiceIsActive=1
                    order by LeadPartnerID asc
                    ");
                res = dao.Load<LeadPartner>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<LeadRoutingView> GetRoutingRules(int clientID)
        {
            IList<LeadRoutingView> res = null;
            dao = GetClientDao(clientID);

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select p.ProductID, lr.LeadTypeID, lr.LeadPartnerID, lr.Percentage, p.ProductName as ProductName, lp.DisplayName as LeadPartnerName from Product p
                    left join LeadRouting lr on lr.ProductID = p.ProductID
                    left join LeadPartner lp on lp.LeadPartnerID = lr.LeadPartnerID
                    where p.ProductID > 0 and p.ProductIsActive = 1
                    order by p.ProductID asc, lr.Percentage desc
                    ");
                res = dao.Load<LeadRoutingView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public void SetRoutingRules(int clientID, int productID, int leadTypeID, IDictionary<int, int> leadPartnerPercentage)
        {
            dao = GetClientDao(clientID);

            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand(@"
                    delete from LeadRouting
                    where ProductID = @productID and leadTypeID = @leadTypeID
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@leadTypeID", MySqlDbType.Int32).Value = leadTypeID;
                dao.ExecuteNonQuery(q);

                foreach (var item in leadPartnerPercentage)
                {
                    LeadRouting l = new LeadRouting();
                    l.ProductID = productID;
                    l.LeadTypeID = leadTypeID;
                    l.LeadPartnerID = item.Key;
                    l.Percentage = item.Value;
                    dao.Save<LeadRouting>(l);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        //--------------------

    }
}
