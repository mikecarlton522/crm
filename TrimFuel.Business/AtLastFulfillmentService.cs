using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.Gateways.AtLastFulfillment;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business
{
    public class AtLastFulfillmentService : ShipperService
    {
        private SaleService saleService { get { return new SaleService(); } }

        public override int ShipperID
        {
            get { return TrimFuel.Model.Enums.ShipperEnum.AtLastFulfillment; }
        }

        public override string TableName
        {
            get { return "ALFRecord"; }
        }

        public override bool SendShippingOrderInternal(Registration r, Billing b, IList<SaleShipperView> saleList)
        {
            bool res = false;
            try
            {
                var cfg = GetAndCheckConfig();

                long saleID = saleList[0].SaleID.Value;

                string country = DEFAULT_COUNTRY;
                RegistrationInfo rInfo = (new RegistrationService()).GetRegistrationInfo(r.RegistrationID.Value);
                if (rInfo != null && !string.IsNullOrEmpty(rInfo.Country))
                {
                    country = rInfo.Country;
                }

                IList<InventoryView> inventoryList = new List<InventoryView>();
                foreach (SaleShipperView sale in saleList)
                {
                    IList<InventoryView> invList = inventoryService.GetInventoryListForShipping(sale.ProductCode, ShipperID);
                    //remove validation, sales can contain Inventories with InventoryType = Service
                    //if (invList.Count == 0)
                    //{
                    //    throw new Exception(string.Format("Can't find Inventories for ProductCode({0}), Sale({1})", sale.ProductCode, sale.SaleID));
                    //}
                    foreach (InventoryView inv in invList)
                    {
                        inv.Quantity = sale.Quantity * inv.Quantity;
                        inventoryList.Add(inv);
                    }
                }

                if (inventoryList.Count == 0)
                {
                    throw new Exception(string.Format("Can't find Inventories for ProductCodes({0}), Sales({1})", string.Join(",", saleList.Select(i => i.ProductCode).ToArray()), string.Join(",", saleList.Select(i => i.SaleID.ToString()).ToArray())));
                }

                long? regID = null;
                string request = null;
                string response = null;
                if (!Config.Current.SHIPPING_TEST_MODE)
                {

                    AtLastFulfillmentGateway gateway = new AtLastFulfillmentGateway(cfg);
                    try
                    {
                        string company = string.Empty;
                        if (Config.Current.APPLICATION_ID == "coaction.trianglecrm.com")
                        {
                            //BillingSubscription bs = saleService.GetBillingSubscriptionBySale(saleID);
                            //Subscription s = Load<Subscription>(bs.SubscriptionID);
                            //Product product = new ProductService().GetProduct(s.ProductID.Value);
                            //if (product.ProductName == "Face Serum US/CA")
                            //    company = "PureCollagenSkin.com";
                            //if (product.ProductName == "Slim Wrap US/CA")
                            //    company = "ZenBodyWrap.com";
                            company = "PureCollagenSkin.com";
                        }
                        regID = gateway.PostShipment(saleID, r, country, inventoryList, company, out request, out response);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(GetType(), ex);
                        if (response == null)
                        {
                            response = "Error: " + ex.Message;
                        }
                    }
                }
                else
                {
                    response = "Test";
                    regID = 1;
                }
                if (regID != null)
                {
                    res = true;
                }
                OnOrderSent(saleList, request, response, regID);

            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = false;
            }
            return res;
        }

        private void OnOrderSent(IList<SaleShipperView> saleList, string request, string response, long? regID)
        {
            try
            {
                dao.BeginTransaction();

                foreach (SaleShipperView sale in saleList)
                {
                    ALFRecord alfRecord = new ALFRecord();

                    alfRecord.SaleID = sale.SaleID;
                    alfRecord.Request = request;
                    alfRecord.Response = response;
                    alfRecord.RegID = regID;
                    alfRecord.Completed = false;
                    alfRecord.CreateDT = DateTime.Now;

                    dao.Save<ALFRecord>(alfRecord);

                    OnOrderSentBase(alfRecord);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public override void CheckShipmentsState()
        {
            if (Config.Current.SHIPPING_TEST_MODE)
                return;

            try
            {
                var cfg = GetAndCheckConfig();

                AtLastFulfillmentGateway gateway = new AtLastFulfillmentGateway(cfg);

                MySqlCommand q = new MySqlCommand(
                    " select distinct alf.RegID as Value from ALFRecord alf" +
                    " where alf.Completed = 0 and alf.RegID is not null" +
                    " order by alf.CreateDT");
                IList<View<long>> list = dao.Load<View<long>>(q);
                foreach (View<long> item in list)
                {
                    string trackNum = null;
                    try
                    {
                        trackNum = gateway.CheckTrackingNumber(item.Value.Value);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(GetType(), ex);
                        trackNum = null;
                    }
                    if (trackNum != null)
                    {
                        OnOrderShipped(item.Value.Value, trackNum);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public override void CheckReturns()
        {
            if (Config.Current.SHIPPING_TEST_MODE)
                return;

            try
            {
                var cfg = GetAndCheckConfig();

                AtLastFulfillmentGateway gateway = new AtLastFulfillmentGateway(cfg);

                var list = gateway.GetReturns();
                foreach (var item in list)
                {
                    OnOrderReturned(item.Key, item.Value.Key, item.Value.Value);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void OnOrderShipped(long regID, string trackingNumber)
        {
            OnOrderShippedBase<ALFRecord>(regID, trackingNumber, DateTime.Now);
        }

        public override void OnOrderShipped(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnOrderShippedBase<ALFRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnTrackingNumberUpdated(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnTrackingNumberUpdatedBase<ALFRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnOrderReturned(long regID, string reason, DateTime? returnDate)
        {
            OnOrderReturnedBase<ALFRecord>(regID, reason, returnDate);
        }
    }
}
