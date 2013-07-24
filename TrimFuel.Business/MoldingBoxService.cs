using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Gateways.MoldingBox;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using API = TrimFuel.Business.MBApi;

namespace TrimFuel.Business
{
    public class MoldingBoxService : ShipperService
    {
        private SaleService saleService { get { return new SaleService(); } }

        public override int ShipperID
        {
            get { return TrimFuel.Model.Enums.ShipperEnum.MB; }
        }

        public override string TableName
        {
            get { return "MBRecord"; }
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

                    MoldingBoxGateway gateway = new MoldingBoxGateway(cfg);
                    try
                    {
                        //logger.Error("Checking of molding box key: " + Config.Current.MOLDING_BOX_API_KEY);
                        regID = gateway.PostShipment(saleID, r, country, inventoryList, out request, out response);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(GetType(), ex);
                        regID = null;
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
                    MBRecord mbRecord = new MBRecord();

                    mbRecord.SaleID = sale.SaleID;
                    mbRecord.Request = request;
                    mbRecord.Response = response;
                    mbRecord.RegID = regID;
                    mbRecord.Completed = false;
                    mbRecord.CreateDT = DateTime.Now;

                    dao.Save<MBRecord>(mbRecord);

                    OnOrderSentBase(mbRecord);
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

                MoldingBoxGateway gateway = new MoldingBoxGateway(cfg);

                MySqlCommand q = new MySqlCommand(
                    " select distinct mb.RegID as Value from MBRecord mb" +
                    " where mb.Completed = 0 and mb.RegID is not null" +
                    " order by mb.CreateDT");
                IList<View<long>> list = dao.Load<View<long>>(q);
                foreach (View<long> item in list)
                {
                    string trackNum = null;
                    try
                    {
                        trackNum = gateway.CheckTrackingNumber(Convert.ToInt32(item.Value.Value));
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
            throw new NotImplementedException();
        }

        public void OnOrderShipped(long regID, string trackingNumber)
        {
            OnOrderShippedBase<MBRecord>(regID, trackingNumber, DateTime.Now);
        }

        public override void OnOrderShipped(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnOrderShippedBase<MBRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnTrackingNumberUpdated(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnTrackingNumberUpdatedBase<MBRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnOrderReturned(long regID, string reason, DateTime? returnDate)
        {
            throw new NotImplementedException();
        }
    }
}
