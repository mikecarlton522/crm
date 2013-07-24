using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.Gateways.GoFulfillmentGateway;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business
{
    public class GoFulfillmentService : ShipperService
    {
        public override int ShipperID
        {
            get { return TrimFuel.Model.Enums.ShipperEnum.GoFulfillment; }
        }

        public override string TableName
        {
            get { return "GFRecord"; }
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

                IList<KeyValuePair<InventoryView, long>> inventoryList = new List<KeyValuePair<InventoryView, long>>();
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
                        inventoryList.Add(new KeyValuePair<InventoryView, long>(inv, sale.SaleID.Value));
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
                    GoFulfillmentGateway gateway = new GoFulfillmentGateway(cfg);
                    try
                    {
                        regID = gateway.PostShipment(saleID, r, b, country, inventoryList, out request, out response);
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
                    res = true;
                    Random rnd = new Random();
                    regID = 1000000000 + (long)(10000.0 * rnd.NextDouble());
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
                    GFRecord gfRecord = new GFRecord();

                    gfRecord.SaleID = sale.SaleID;
                    gfRecord.Request = request;
                    gfRecord.Response = response;
                    gfRecord.RegID = regID;
                    gfRecord.Completed = false;
                    gfRecord.CreateDT = DateTime.Now;

                    dao.Save<GFRecord>(gfRecord);

                    OnOrderSentBase(gfRecord);
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
            throw new NotImplementedException();
        }

        public override void CheckReturns()
        {
            throw new NotImplementedException();
        }

        public override void OnOrderShipped(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnOrderShippedBase<GFRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnTrackingNumberUpdated(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnTrackingNumberUpdatedBase<GFRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnOrderReturned(long regID, string reason, DateTime? returnDate)
        {
            throw new NotImplementedException();
        }
    }
}
