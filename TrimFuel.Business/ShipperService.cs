using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using System.Text.RegularExpressions;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public abstract class ShipperService : BaseService
    {
        public const string DEFAULT_COUNTRY = "United States";

        protected IDictionary<ShipperConfig.ID, ShipperConfig> GetAndCheckConfig()
        {
            ShipperConfigService srv = new ShipperConfigService();
            Shipper sh = EnsureLoad<Shipper>(ShipperID);
            IDictionary<ShipperConfig.ID, ShipperConfig> res = srv.GetShipperConfig(sh.ShipperID);
            srv.CheckShipperConfig(sh, res);
            return res;
        }

        public static IList<ShipperService> ImplementedShippers
        {
            get
            {
                return new List<ShipperService>()
                {
                    new ABFService(),
                    new KeymailService(),
                    new AtLastFulfillmentService(),
                    new GoFulfillmentService(),
                    new NPFService(),
                    new MoldingBoxService(),
                    new TriangleFulfillmentService(),
                    new CustomShipperService()
                    //new TsnService()
                };
            }
        }

        public static ShipperService GetShipperServiceByShipperID(int shipperID)
        {
            ShipperService res = null;
            foreach (ShipperService shipper in ImplementedShippers)
            {
                if (shipper.ShipperID == shipperID)
                {
                    res = shipper;
                    break;
                }
            }
            return res;
        }

        private SaleService saleService { get { return new SaleService(); } }
        private SubscriptionService subscriptionService { get { return new SubscriptionService(); } }
        protected InventoryService inventoryService { get { return new InventoryService(); } }

        protected EmailService emailService { get { return new EmailService(); } }
        public abstract int ShipperID { get; }
        public abstract string TableName { get; }

        public bool SendShippingOrder(Registration r, Billing b, IList<SaleShipperView> saleList)
        {
            try
            {
                //Check inventories and remove sales that don't have shipping inventories
                IList<SaleShipperView> fixedSaleList = new List<SaleShipperView>();

                List<InventoryView> inventoryList = new List<InventoryView>();
                foreach (var item in saleList)
                {
                    var saleInvList = inventoryService.GetInventoryListForShipping(item.ProductCode, ShipperID);
                    if (saleInvList.Count > 0)
                    {
                        fixedSaleList.Add(item);
                        inventoryList.AddRange(saleInvList);
                    }
                }

                if (fixedSaleList.Count > 0)
                {
                    ShipperService routedShipperService = null;
                    if (RouteToDifferentShipper(r, b, fixedSaleList, inventoryList, out routedShipperService))
                    {
                        return routedShipperService.SendShippingOrderInternal(r, b, fixedSaleList);
                    }
                    else
                    {
                        return SendShippingOrderInternal(r, b, fixedSaleList);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return false;
        }

        public abstract bool SendShippingOrderInternal(Registration r, Billing b, IList<SaleShipperView> saleList);

        public DateTime? GetShippedDate(long saleID)
        {
            DateTime? res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(
                    " select ShippedDT as Value from TSNRecord" +
                    " where SaleID = @saleID and Completed = 1" +
                    " union" +
                    " select ShippedDT as Value from ABFRecord" +
                    " where SaleID = @saleID and Completed = 1" +
                    " union" +
                    " select ShippedDT as Value from ALFRecord" +
                    " where SaleID = @saleID and Completed = 1" +
                    " union" +
                    " select ShippedDT as Value from KeymailRecord" +
                    " where SaleID = @saleID and Completed = 1" +
                    " union" +
                    " select ShippedDT as Value from GFRecord" +
                    " where SaleID = @saleID and Completed = 1" +
                    " union" +
                    " select ShippedDT as Value from NPFRecord" +
                    " where SaleID = @saleID and Completed = 1" +
                    " union" +
                    " select ShippedDT as Value from TFRecord" +
                    " where SaleID = @saleID and Completed = 1" +
                    " union" +
                    " select ShippedDT as Value from MBRecord" +
                    " where SaleID = @saleID and Completed = 1" +
                    " union" +
                    " select ShippedDT as Value from CustomShipperRecord" +
                    " where SaleID = @saleID and Completed = 1"
                    );
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                View<DateTime> resView = dao.Load<View<DateTime>>(q).FirstOrDefault();
                if (resView != null)
                {
                    res = resView.Value;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public DateTime? GetReturnDate(long saleID)
        {
            DateTime? res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select ReturnDate as Value from ReturnedSale " +
                    "where SaleID = @saleID");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                View<DateTime> resView = dao.Load<View<DateTime>>(q).FirstOrDefault();
                if (resView != null)
                {
                    res = resView.Value;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public string ExtractTrackingNumber(string src)
        {
            if (string.IsNullOrEmpty(src))
            {
                return null;
            }

            if (src.Length < 20)
            {
                return src;
            }

            string res = null;
            try
            {
                MatchCollection m = Regex.Matches(src, @"(\d{10,})");
                if (m != null && m.Count > 0)
                {
                    res = m[0].Groups[0].Value;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<int> GetShipperListToCheckShipments()
        {
            IList<int> res = new List<int>();
            try
            {
                MySqlCommand q = new MySqlCommand(
                    "select distinct 4 as Value from ABFRecord " +
                    "where Completed = 0 and RegID is not null " +
                    "union " +
                    "select distinct 6 as Value from ALFRecord " +
                    "where Completed = 0 and RegID is not null " +
                    "union " +
                    "select distinct 2 as Value from TSNRecord " +
                    "where Completed = 0 and RegID is not null " +
                    "union " +
                    "select distinct 5 as Value from KeymailRecord " +
                    "where Completed = 0 and RegID is not null " +
                    "union " +
                    "select distinct 7 as Value from GFRecord " +
                    "where Completed = 0 and RegID is not null " +
                    "union " +
                    "select distinct 8 as Value from NPFRecord " +
                    "where Completed = 0 and RegID is not null " +
                    "union " +
                    "select distinct 10 as Value from TFRecord " +
                    "where Completed = 0 and RegID is not null " +
                    "union " +
                    "select distinct 9 as Value from MBRecord " +
                    "where Completed = 0 and RegID is not null " +
                    "union " +
                    "select distinct 11 as Value from CustomShipperRecord " +
                    "where Completed = 0 and RegID is not null ");

                IList<View<int>> resList = dao.Load<View<int>>(q);
                res = resList.Select(i => i.Value.Value).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<int> GetShipperListToCheckReturns()
        {
            IList<int> res = new List<int>();
            try
            {
                MySqlCommand q = new MySqlCommand(
                    "select distinct 4 as Value from ABFRecord " +
                    "where Completed = 1 and RegID is not null " +
                    "union " +
                    "select distinct 6 as Value from ALFRecord " +
                    "where Completed = 1 and RegID is not null " +
                    "union " +
                    "select distinct 2 as Value from TSNRecord " +
                    "where Completed = 1 and RegID is not null " +
                    "union " +
                    "select distinct 5 as Value from KeymailRecord " +
                    "where Completed = 1 and RegID is not null " +
                    "union " +
                    "select distinct 7 as Value from GFRecord " +
                    "where Completed = 1 and RegID is not null " +
                    "union " +
                    "select distinct 8 as Value from NPFRecord " +
                    "where Completed = 1 and RegID is not null " +
                    "union " +
                    "select distinct 10 as Value from TFRecord " +
                    "where Completed = 1 and RegID is not null " +
                    "union " +
                    "select distinct 9 as Value from MBRecord " +
                    "where Completed = 1 and RegID is not null " +
                    "union " +
                    "select distinct 11 as Value from CustomShipperRecord " +
                    "where Completed = 1 and RegID is not null " +
                    "");

                IList<View<int>> resList = dao.Load<View<int>>(q);
                res = resList.Select(i => i.Value.Value).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        //Only for shippers with pending states (Like Keymail or NPF)
        //Keymail is not implemented
        public IList<int> GetShipperListToSendPendingOrders()
        {
            IList<int> res = new List<int>();
            try
            {
                MySqlCommand q = new MySqlCommand(
                    "select distinct 8 as Value from NPFRecordToSend " +
                    "where RegID is not null " +
                    "union " +
                    "select distinct 11 as Value from CustomShipperRecordToSend " +
                    "where RegID is not null ");

                IList<View<int>> resList = dao.Load<View<int>>(q);
                res = resList.Select(i => i.Value.Value).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public virtual void SendPendingOrders()
        {
            //Do nothing by default
        }

        public virtual bool IsPendingStateImplementation
        {
            get { return false; }
        }

        public virtual int RoutingReason { get; set; }

        public bool IsSaleSent(long saleID)
        {
            bool res = true;
            try
            {
                MySqlCommand q = new MySqlCommand(
                    "select 1 as Value from ABFRecord " +
                    "where SaleID = @saleID and RegID is not null " +
                    "union " +
                    "select 1 as Value from ALFRecord " +
                    "where SaleID = @saleID and RegID is not null " +
                    "union " +
                    "select 1 as Value from TSNRecord " +
                    "where SaleID = @saleID and RegID is not null " +
                    "union " +
                    "select 1 as Value from KeymailRecord " +
                    "where SaleID = @saleID and RegID is not null " +
                    "union " +
                    "select 1 as Value from GFRecord " +
                    "where SaleID = @saleID and RegID is not null " +
                    "union " +
                    "select 1 as Value from NPFRecord " +
                    "where SaleID = @saleID and RegID is not null " +
                    "union " +
                    "select 1 as Value from TFRecord " +
                    "where SaleID = @saleID and RegID is not null " +
                    "union " +
                    "select 1 as Value from MBRecord " +
                    "where SaleID = @saleID and RegID is not null " +
                    "union " +
                    "select 1 as Value from CustomShipperRecord " +
                    "where SaleID = @saleID and RegID is not null ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                if (dao.Load<View<int>>(q).Count == 0)
                {
                    res = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public int? DetermineShipper(long billingID)
        {
            int? res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select distinct shp.ShipperID as Value from ShipperProduct shp " +
                    "inner join Subscription s on s.ProductID = shp.ProductID " +
                    "inner join BillingSubscription bs on bs.SubscriptionID = s.SubscriptionID " +
                    "where bs.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;

                View<int> resView = dao.Load<View<int>>(q).FirstOrDefault();
                if (resView != null)
                {
                    res = resView.Value;
                }
                else
                {
                    throw new Exception(string.Format("Can't determine Shipper for Billing({0})", billingID));
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public BusinessError<bool> SendSale(long saleID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, null);
            try
            {
                Sale sale = EnsureLoad<Sale>(saleID);

                if (sale.NotShip != null && sale.NotShip == true)
                {
                    res.ErrorMessage = "Sale is marked as 'Not Ship'.";
                }
                else if (IsSaleSent(saleID))
                {
                    res.ErrorMessage = "Sale was already sent early.";
                }
                else
                {
                    BillingSubscription bs = saleService.GetBillingSubscriptionBySale(saleID);
                    if (bs == null)
                    {
                        throw new Exception(string.Format("Can't determine BillingSubscription for Sale({0})", saleID));
                    }
                    //if (bs.StatusTID != TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Active)
                    //{
                    //    res.ErrorMessage = "Account is not Active.";
                    //}
                    else
                    {
                        if (DetermineShipper(bs.BillingID.Value) != ShipperID)
                        {
                            res.ErrorMessage = "Account is set to another shipper.";
                        }
                        else
                        {
                            Billing b = EnsureLoad<Billing>(bs.BillingID);
                            Registration r = EnsureLoad<Registration>(b.RegistrationID);
                            IList<SaleShipperView> saleList = new List<SaleShipperView>();

                            if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Billing)
                            {
                                BillingSale bsale = EnsureLoad<BillingSale>(saleID);
                                Subscription s = EnsureLoad<Subscription>(bs.SubscriptionID);
                                saleList.Add(new SaleShipperView()
                                {
                                    SaleID = sale.SaleID,
                                    SaleTypeID = TrimFuel.Model.Enums.SaleTypeEnum.Billing,
                                    ProductCode = bsale.ProductCode ?? s.ProductCode,
                                    Quantity = bsale.Quantity ?? s.Quantity
                                });

                                saleList = saleList.
                                    Union(GetUpsellsToShip(b.BillingID.Value)).
                                    Union(GetExtraTrialShipsToShip(b.BillingID.Value)).
                                    ToList();
                            }
                            else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Rebill)
                            {
                                BillingSale bsale = EnsureLoad<BillingSale>(saleID);
                                Subscription s = EnsureLoad<Subscription>(bs.SubscriptionID);
                                saleList.Add(new SaleShipperView()
                                {
                                    SaleID = sale.SaleID,
                                    SaleTypeID = TrimFuel.Model.Enums.SaleTypeEnum.Rebill,
                                    ProductCode = bsale.ProductCode ?? s.SKU2,
                                    Quantity = bsale.Quantity ?? s.QuantitySKU2
                                });
                            }
                            else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Upsell)
                            {
                                UpsellSale us = EnsureLoad<UpsellSale>(saleID);
                                Upsell u = EnsureLoad<Upsell>(us.UpsellID);
                                saleList.Add(new SaleShipperView()
                                {
                                    SaleID = sale.SaleID,
                                    SaleTypeID = TrimFuel.Model.Enums.SaleTypeEnum.Upsell,
                                    ProductCode = u.ProductCode,
                                    Quantity = u.Quantity ?? 1
                                });
                            }
                            else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.ExtraTrialShip)
                            {
                                ExtraTrialShipSale es = EnsureLoad<ExtraTrialShipSale>(saleID);
                                ExtraTrialShip e = EnsureLoad<ExtraTrialShip>(es.ExtraTrialShipID);
                                saleList.Add(new SaleShipperView()
                                {
                                    SaleID = sale.SaleID,
                                    SaleTypeID = TrimFuel.Model.Enums.SaleTypeEnum.ExtraTrialShip,
                                    ProductCode = e.ProductCode,
                                    Quantity = e.Quantity ?? 1
                                });
                            }

                            if (SendShippingOrder(r, b, saleList))
                            {
                                OnSalesSent(saleList);
                                res.ReturnValue = true;
                                res.State = BusinessErrorState.Success;
                            }
                            else
                            {
                                res.ErrorMessage = "Unknown error occurred.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
                if (string.IsNullOrEmpty(res.ErrorMessage))
                {
                    res.ErrorMessage = "Unknown error occurred.";
                }
            }
            return res;
        }

        public BusinessError<bool> SendSale(long saleID, int shipperID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, null);
            try
            {
                Sale sale = EnsureLoad<Sale>(saleID);

                if (sale.NotShip != null && sale.NotShip == true)
                {
                    res.ErrorMessage = "Sale is marked as 'Not Ship'.";
                }
                else if (IsSaleSent(saleID))
                {
                    res.ErrorMessage = "Sale was already sent early.";
                }
                else
                {
                    BillingSubscription bs = saleService.GetBillingSubscriptionBySale(saleID);
                    if (bs == null)
                    {
                        throw new Exception(string.Format("Can't determine BillingSubscription for Sale({0})", saleID));
                    }
                    //if (bs.StatusTID != TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Active)
                    //{
                    //    res.ErrorMessage = "Account is not Active.";
                    //}
                    else
                    {
                        if (shipperID != ShipperID)
                        {
                            res.ErrorMessage = "Account is set to another shipper.";
                        }
                        else
                        {
                            Billing b = EnsureLoad<Billing>(bs.BillingID);
                            Registration r = EnsureLoad<Registration>(b.RegistrationID);
                            IList<SaleShipperView> saleList = new List<SaleShipperView>();

                            if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Billing)
                            {
                                BillingSale bsale = EnsureLoad<BillingSale>(saleID);
                                Subscription s = EnsureLoad<Subscription>(bs.SubscriptionID);
                                saleList.Add(new SaleShipperView()
                                {
                                    SaleID = sale.SaleID,
                                    SaleTypeID = TrimFuel.Model.Enums.SaleTypeEnum.Billing,
                                    ProductCode = bsale.ProductCode ?? s.ProductCode,
                                    Quantity = bsale.Quantity ?? s.Quantity
                                });

                                saleList = saleList.
                                    Union(GetUpsellsToShip(b.BillingID.Value)).
                                    Union(GetExtraTrialShipsToShip(b.BillingID.Value)).
                                    ToList();
                            }
                            else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Rebill)
                            {
                                BillingSale bsale = EnsureLoad<BillingSale>(saleID);
                                Subscription s = EnsureLoad<Subscription>(bs.SubscriptionID);
                                saleList.Add(new SaleShipperView()
                                {
                                    SaleID = sale.SaleID,
                                    SaleTypeID = TrimFuel.Model.Enums.SaleTypeEnum.Rebill,
                                    ProductCode = bsale.ProductCode ?? s.SKU2,
                                    Quantity = bsale.Quantity ?? s.QuantitySKU2
                                });
                            }
                            else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Upsell)
                            {
                                UpsellSale us = EnsureLoad<UpsellSale>(saleID);
                                Upsell u = EnsureLoad<Upsell>(us.UpsellID);
                                saleList.Add(new SaleShipperView()
                                {
                                    SaleID = sale.SaleID,
                                    SaleTypeID = TrimFuel.Model.Enums.SaleTypeEnum.Upsell,
                                    ProductCode = u.ProductCode,
                                    Quantity = u.Quantity ?? 1
                                });
                            }
                            else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.ExtraTrialShip)
                            {
                                ExtraTrialShipSale es = EnsureLoad<ExtraTrialShipSale>(saleID);
                                ExtraTrialShip e = EnsureLoad<ExtraTrialShip>(es.ExtraTrialShipID);
                                saleList.Add(new SaleShipperView()
                                {
                                    SaleID = sale.SaleID,
                                    SaleTypeID = TrimFuel.Model.Enums.SaleTypeEnum.ExtraTrialShip,
                                    ProductCode = e.ProductCode,
                                    Quantity = e.Quantity ?? 1
                                });
                            }

                            if (SendShippingOrder(r, b, saleList))
                            {
                                OnSalesSent(saleList);
                                res.ReturnValue = true;
                                res.State = BusinessErrorState.Success;
                            }
                            else
                            {
                                res.ErrorMessage = "Unknown error occurred.";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
                if (string.IsNullOrEmpty(res.ErrorMessage))
                {
                    res.ErrorMessage = "Unknown error occurred.";
                }
            }
            return res;
        }

        public BusinessError<bool> SendUpsells(long billingID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, null);
            try
            {
                Billing b = EnsureLoad<Billing>(billingID);
                if (DetermineShipper(b.BillingID.Value) != ShipperID)
                {
                    res.ErrorMessage = "Account is set to another shipper.";
                }
                else
                {
                    IList<SaleShipperView> saleList = new List<SaleShipperView>();
                    Registration r = EnsureLoad<Registration>(b.RegistrationID);

                    saleList = saleList.
                        Union(GetUpsellsToShip(b.BillingID.Value)).
                        ToList();

                    if (saleList.Count == 0)
                    {
                        res.ErrorMessage = "No Sales to shpping were found for Account. Sales were sent early or excluded from shipping.";
                    }
                    else
                    {
                        if (SendShippingOrder(r, b, saleList))
                        {
                            OnSalesSent(saleList);
                            res.ReturnValue = true;
                            res.State = BusinessErrorState.Success;
                        }
                        else
                        {
                            res.ErrorMessage = "Unknown error occurred.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
                if (string.IsNullOrEmpty(res.ErrorMessage))
                {
                    res.ErrorMessage = "Unknown error occurred.";
                }
            }
            return res;
        }

        public BusinessError<bool> SendExtraTrialShips(long billingID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, null);
            try
            {
                Billing b = EnsureLoad<Billing>(billingID);
                if (DetermineShipper(b.BillingID.Value) != ShipperID)
                {
                    res.ErrorMessage = "Account is set to another shipper.";
                }
                else
                {
                    IList<SaleShipperView> saleList = new List<SaleShipperView>();
                    Registration r = EnsureLoad<Registration>(b.RegistrationID);

                    saleList = saleList.
                        Union(GetExtraTrialShipsToShip(b.BillingID.Value)).
                        ToList();

                    if (saleList.Count == 0)
                    {
                        res.ErrorMessage = "No Sales to shpping were found for Account. Sales were sent early or excluded from shipping.";
                    }
                    else
                    {
                        if (SendShippingOrder(r, b, saleList))
                        {
                            OnSalesSent(saleList);
                            res.ReturnValue = true;
                            res.State = BusinessErrorState.Success;
                        }
                        else
                        {
                            res.ErrorMessage = "Unknown error occurred.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public BusinessError<bool> SendExtraTrialShips(long billingID, int shipperID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, null);
            try
            {
                Billing b = EnsureLoad<Billing>(billingID);
                if (shipperID != ShipperID)
                {
                    res.ErrorMessage = "Account is set to another shipper.";
                }
                else
                {
                    IList<SaleShipperView> saleList = new List<SaleShipperView>();
                    Registration r = EnsureLoad<Registration>(b.RegistrationID);

                    saleList = saleList.
                        Union(GetExtraTrialShipsToShip(b.BillingID.Value)).
                        ToList();

                    if (saleList.Count == 0)
                    {
                        res.ErrorMessage = "No Sales to shpping were found for Account. Sales were sent early or excluded from shipping.";
                    }
                    else
                    {
                        if (SendShippingOrder(r, b, saleList))
                        {
                            OnSalesSent(saleList);
                            res.ReturnValue = true;
                            res.State = BusinessErrorState.Success;
                        }
                        else
                        {
                            res.ErrorMessage = "Unknown error occurred.";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<SaleShipperView> GetUpsellsToShip(long billingID)
        {
            IList<SaleShipperView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select us.SaleID, s.SaleTypeID, u.ProductCode, u.Quantity from UpsellSale us " +
                    "inner join Sale s on s.SaleID = us.SaleID " +
                    "inner join Upsell u on u.UpsellID = us.UpsellID " +
                    "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID " +
                    "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                    "where bs.BillingID = @billingID and ch.Success = 1 and u.Complete = 0 and coalesce(s.NotShip,0) <> 1");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;

                IList<SaleShipperView> resTemp = dao.Load<SaleShipperView>(q);

                res = new List<SaleShipperView>();
                foreach (SaleShipperView s in resTemp)
                {
                    if (!IsSaleSent(s.SaleID.Value))
                    {
                        res.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<SaleShipperView> GetExtraTrialShipsToShip(long billingID)
        {
            IList<SaleShipperView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select es.SaleID, s.SaleTypeID, e.ProductCode, e.Quantity from ExtraTrialShipSale es " +
                    "inner join Sale s on s.SaleID = es.SaleID " +
                    "inner join ExtraTrialShip e on e.ExtraTrialShipID = es.ExtraTrialShipID " +
                    "where e.BillingID = @billingID and e.Completed = 0 and coalesce(s.NotShip,0) <> 1");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;

                IList<SaleShipperView> resTemp = dao.Load<SaleShipperView>(q);

                res = new List<SaleShipperView>();
                foreach (SaleShipperView s in resTemp)
                {
                    if (!IsSaleSent(s.SaleID.Value))
                    {
                        res.Add(s);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        protected void OnSalesSent(IList<SaleShipperView> saleList)
        {
            try
            {
                dao.BeginTransaction();

                foreach (SaleShipperView sale in saleList)
                {
                    if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Billing)
                    {
                        BillingSale bsale = EnsureLoad<BillingSale>(sale.SaleID);
                        BillingSubscription bs = EnsureLoad<BillingSubscription>(bsale.BillingSubscriptionID);
                        Subscription s = EnsureLoad<Subscription>(bs.SubscriptionID);
                        if (s.Recurring == false)
                        {
                            bs.StatusTID = TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.OrderCompleted;
                            dao.Save<BillingSubscription>(bs);
                        }
                    }
                    else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Rebill)
                    {
                    }
                    else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.Upsell)
                    {
                        UpsellSale us = EnsureLoad<UpsellSale>(sale.SaleID);
                        Upsell u = EnsureLoad<Upsell>(us.UpsellID);
                        u.Complete = true;
                        dao.Save<Upsell>(u);
                    }
                    else if (sale.SaleTypeID == TrimFuel.Model.Enums.SaleTypeEnum.ExtraTrialShip)
                    {
                        ExtraTrialShipSale es = EnsureLoad<ExtraTrialShipSale>(sale.SaleID);
                        ExtraTrialShip e = EnsureLoad<ExtraTrialShip>(es.ExtraTrialShipID);
                        e.Completed = true;
                        dao.Save<ExtraTrialShip>(e);
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public abstract void CheckShipmentsState();

        public abstract void CheckReturns();

        protected void OnSalesShipped(string trackingNumber, DateTime shippedDate, IList<long> saleIDs)
        {
            try
            {
                dao.BeginTransaction();

                foreach (long saleID in saleIDs)
                {
                    Sale s = EnsureLoad<Sale>(saleID);
                    s.TrackingNumber = trackingNumber;
                    dao.Save<Sale>(s);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        protected long? GetRegIDBySale(long saleID, out int? shipperServiceID)
        {
            shipperServiceID = null;
            long? regID = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select RegID 'RegID', 4 'ShipperID' from ABFRecord where (RegID is not null) and (SaleID=@saleID)
                    union
                    select RegID 'RegID', 2 'ShipperID' from TSNRecord where (RegID is not null) and (SaleID=@saleID)
                    union
                    select RegID 'RegID', 6 'ShipperID' from ALFRecord where (RegID is not null) and (SaleID=@saleID)
                    union
                    select RegID 'RegID', 5 'ShipperID' from KeymailRecord where (RegID is not null) and (SaleID=@saleID)
                    union
                    select RegID 'RegID', 7 'ShipperID' from GFRecord where (RegID is not null) and (SaleID=@saleID)
                    union
                    select RegID 'RegID', 8 'ShipperID' from NPFRecord where (RegID is not null) and (SaleID=@saleID)
                    union
                    select RegID 'RegID', 9 'ShipperID' from MBRecord where (RegID is not null) and (SaleID=@saleID)
                    union
                    select RegID 'RegID', 11 'ShipperID' from CustomShipperRecord where (RegID is not null) and (SaleID=@saleID)
                    union
                    select RegID 'RegID', 10 'ShipperID' from TFRecord where (RegID is not null) and (SaleID=@saleID)
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                var item = dao.Load<ShortShipmentView>(q).SingleOrDefault();
                if (item != null)
                {
                    shipperServiceID = item.ShipperID;
                    regID = item.RegID;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return regID;
        }

        public abstract void OnOrderReturned(long regID, string reason, DateTime? returnDate);

        protected void OnOrderReturnedBase<RecordType>(long regID, string reason, DateTime? returnDate) where RecordType : ShipperRecord
        {
            IList<long> saleIDs = new List<long>();
            try
            {
                ReturnSaleService retSaleService = new ReturnSaleService();
                MySqlCommand q = new MySqlCommand("select shipper.* from " +
                    TableName +
                    " shipper " +
                    "where shipper.RegID = @regID");
                q.Parameters.Add("@regID", MySqlDbType.Int64).Value = regID;

                foreach (RecordType shipper in dao.Load<RecordType>(q))
                    retSaleService.ReturnSale(shipper.SaleID.Value, reason, returnDate, 0);

            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public abstract void OnOrderShipped(long regID, string trackingNumber, DateTime shippedDate);

        protected void OnOrderShippedBase<RecordType>(long regID, string trackingNumber, DateTime shippedDate) where RecordType : ShipperRecord
        {
            bool alreadyShipped = false;

            IList<long> saleIDs = new List<long>();
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select shipper.* from " +
                    TableName +
                    " shipper " +
                    "where shipper.RegID = @regID");
                q.Parameters.Add("@regID", MySqlDbType.Int64).Value = regID;

                foreach (RecordType shipper in dao.Load<RecordType>(q))
                {
                    if (!shipper.Completed.Value)
                    {
                        saleIDs.Add(shipper.SaleID.Value);

                        shipper.ShippedDT = shippedDate;
                        shipper.StatusResponse = trackingNumber;
                        shipper.Completed = true;
                        dao.Save<RecordType>(shipper);
                    }
                    else
                    {
                        alreadyShipped = true;
                    }
                }

                OnSalesShipped(trackingNumber, shippedDate, saleIDs);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }

            if (!alreadyShipped)
            {
                try
                {
                    MySqlCommand q = new MySqlCommand("select i.InventoryID, i.SKU, i.Product, " +
                        "IfNull(bsale.Quantity, (case s.SaleTypeID when 1 then subs.Quantity else subs.QuantitySKU2 end)) * pci.Quantity as Quantity " +
                        "from " + TableName + " shipper " +
                        "inner join Sale s on s.SaleID = shipper.SaleID " +
                        "inner join BillingSale bsale on bsale.SaleID = s.SaleID " +
                        "inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID " +
                        "inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID " +
                        "inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID " +
                        "inner join Subscription subs on bs.SubscriptionID = subs.SubscriptionID " +
                        "inner join ProductCode pc on pc.ProductCode = cd.SKU " +
                        "inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID " +
                        "inner join Inventory i on i.InventoryID = pci.InventoryID " +
                        "where shipper.RegID = @regID and i.InventoryType = @InventoryType_Inventory " +
                        "union " +
                        "select i.InventoryID, i.SKU, i.Product, u.Quantity * pci.Quantity as Quantity from " + TableName + " shipper " +
                        "inner join UpsellSale us on us.SaleID = shipper.SaleID " +
                        "inner join Upsell u on u.UpsellID = us.UpsellID " +
                        "inner join ProductCode pc on pc.ProductCode = u.ProductCode " +
                        "inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID " +
                        "inner join Inventory i on i.InventoryID = pci.InventoryID " +
                        "where shipper.RegID = @regID and i.InventoryType = @InventoryType_Inventory " +
                        "union " +
                        "select i.InventoryID, i.SKU, i.Product, ets.Quantity * pci.Quantity as Quantity from " + TableName + " shipper " +
                        "inner join ExtraTrialShipSale etssale on etssale.SaleID = shipper.SaleID " +
                        "inner join ExtraTrialShip ets on ets.ExtraTrialShipID = etssale.ExtraTrialShipID " +
                        "inner join ProductCode pc on pc.ProductCode = ets.ProductCode " +
                        "inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID " +
                        "inner join Inventory i on i.InventoryID = pci.InventoryID " +
                        "where shipper.RegID = @regID and i.InventoryType = @InventoryType_Inventory ");
                    q.Parameters.Add("@regID", MySqlDbType.Int64).Value = regID;
                    q.Parameters.Add("@InventoryType_Inventory", MySqlDbType.Int32).Value = InventoryTypeEnum.Inventory;

                    IList<InventoryView> inventoryList = dao.Load<InventoryView>(q);
                    if (inventoryList.Count > 0)
                    {
                        BillingSubscription bs = saleService.GetBillingSubscriptionBySale(saleIDs[0]);
                        Subscription s = EnsureLoad<Subscription>(bs.SubscriptionID);
                        Billing b = EnsureLoad<Billing>(bs.BillingID);

                        emailService.SendShippingEmail(b, s, inventoryList, trackingNumber);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }
            }
        }

        public abstract void OnTrackingNumberUpdated(long regID, string trackingNumber, DateTime shippedDate);

        protected void OnTrackingNumberUpdatedBase<RecordType>(long regID, string trackingNumber, DateTime shippedDate) where RecordType : ShipperRecord
        {
            IList<long> saleIDs = new List<long>();
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select shipper.* from " +
                    TableName +
                    " shipper " +
                    "where shipper.RegID = @regID");
                q.Parameters.Add("@regID", MySqlDbType.Int64).Value = regID;

                foreach (RecordType shipper in dao.Load<RecordType>(q))
                {
                    saleIDs.Add(shipper.SaleID.Value);
                    if (shipper.ShippedDT == null)
                        shipper.ShippedDT = shippedDate;
                    if (string.IsNullOrEmpty(shipper.StatusResponse))
                        shipper.StatusResponse = trackingNumber;
                    shipper.Completed = true;
                    dao.Save<RecordType>(shipper);
                }

                OnSalesShipped(trackingNumber, shippedDate, saleIDs);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        /// <summary>
        /// Call this method for processing AggUnsentShipments table after shipments was sent
        /// </summary>
        /// <typeparam name="RecordType">Shipper table type</typeparam>
        /// <param name="shipper">Shipper row</param>
        protected void OnOrderSentBase<RecordType>(RecordType shipper) where RecordType : ShipperRecord
        {
            MySqlCommand q = null;
            try
            {
                var existingUnsentRecord = dao.Load<AggUnsentShipments>(shipper.SaleID);
                if (shipper.RegID == null)
                {
                    if (existingUnsentRecord == null)
                    {
                        //add new record to Agg table
                        if (shipper.SaleID != null)
                        {
                            var billing = saleService.GetBillingBySale(shipper.SaleID.Value);
                            if (billing != null)
                            {
                                existingUnsentRecord = new AggUnsentShipments()
                                {
                                    BillingID = billing.BillingID,
                                    ShipperID = ShipperID,
                                    Reason = shipper.Response,
                                    Date = shipper.CreateDT,
                                    SaleID = shipper.SaleID
                                };
                                dao.Save<AggUnsentShipments>(existingUnsentRecord);
                            }
                        }
                    }
                    else
                    {
                        //modify reason
                        existingUnsentRecord.Reason = shipper.Response;
                        existingUnsentRecord.ShipperID = ShipperID;
                        existingUnsentRecord.Date = shipper.CreateDT;
                        dao.Save<AggUnsentShipments>(existingUnsentRecord);
                    }
                }
                else
                {
                    if (existingUnsentRecord != null)
                    {
                        //delete
                        q = new MySqlCommand("Delete From AggUnsentShipments where SaleID=@SaleID");
                        q.Parameters.Add("@SaleID", MySqlDbType.UInt64).Value = shipper.SaleID;
                        dao.ExecuteNonQuery(q);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        private bool RouteToDifferentShipper(Registration r, Billing b, IList<SaleShipperView> saleList, IList<InventoryView> inventoryList, out ShipperService routedShipperService)
        {
            //Hardcoded routings to different shippers

            //Routing ABF -> TF
            if (ShipperID == TrimFuel.Model.Enums.ShipperEnum.ABF)
            {
                if (inventoryList.Where(i => i.SKU == "ABF-HIGH").Count() > 0 ||
                    inventoryList.Where(i => i.SKU == "ABF-LOW").Count() > 0 ||
                    inventoryList.Where(i => i.SKU == "ABF-MED").Count() > 0 ||
                    inventoryList.Where(i => i.SKU == "ABF-MENTH").Count() > 0 ||
                    inventoryList.Where(i => i.SKU == "ABF-STARTBXMINI").Count() > 0)
                {
                    routedShipperService = new TriangleFulfillmentService();
                    return true;
                }
            }

            //Routing to CustomShipper for CoAction order not to Australia.
            //if (Config.Current.APPLICATION_ID == TrimFuel.Model.Enums.ApplicationEnum.CoActionCRM)
            //{
            //    if (b.Country != null)
            //    {
            //        string country = b.Country.ToLower();

            //        if (country != "au" && country != "aus" && country != "australia")
            //        {
            //            routedShipperService = new CustomShipperService();
            //            routedShipperService.RoutingReason = ShipperRoutingReasonEnum.CoActionCanadaShipments;
            //            return true;
            //        }
            //    }
            //}

            //Routing to CustomShipper for OrangeStar. All orders.
            if (Config.Current.APPLICATION_ID == TrimFuel.Model.Enums.ApplicationEnum.OrangeStarCRM)
            {
                routedShipperService = new CustomShipperService();
                routedShipperService.RoutingReason = ShipperRoutingReasonEnum.OrangeStarAllShipments;
                return true;
            }

            if (b.CampaignID != null)
            {
                Campaign campaign = Load<Campaign>(b.CampaignID.Value);
                if (campaign.ShipperID != null)
                {
                    if (campaign.ShipperID.Value != ShipperID)
                    {
                        routedShipperService = GetShipperServiceByShipperID(campaign.ShipperID.Value);
                        if (routedShipperService != null)
                            return true;
                    }
                }
            }

            routedShipperService = null;
            return false;
        }
    }
}
