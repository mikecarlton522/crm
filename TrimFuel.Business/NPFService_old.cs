//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TrimFuel.Model;
//using TrimFuel.Model.Views;
//using TrimFuel.Business.Gateways.NPFGateway;
//using MySql.Data.MySqlClient;

//namespace TrimFuel.Business
//{
//    public class NPFService_old : ShipperService
//    {
//        private SaleService saleService { get { return new SaleService(); } }

//        protected override int ShipperID
//        {
//            get { return TrimFuel.Model.Enums.ShipperEnum.NPF; }
//        }

//        protected override bool SendShippingOrder(Registration r, Billing b, IList<SaleShipperView> saleList)
//        {
//            bool res = false;
//            try
//            {
//                long saleID = saleList[0].SaleID.Value;

//                string country = DEFAULT_COUNTRY;
//                RegistrationInfo rInfo = (new RegistrationService()).GetRegistrationInfo(r.RegistrationID.Value);
//                if (rInfo != null && !string.IsNullOrEmpty(rInfo.Country))
//                {
//                    country = rInfo.Country;
//                }

//                IList<InventoryView> inventoryList = new List<InventoryView>();
//                foreach (SaleShipperView sale in saleList)
//                {
//                    IList<InventoryView> invList = inventoryService.GetInventoryByProductCode(sale.ProductCode);
//                    if (invList.Count == 0)
//                    {
//                        throw new Exception(string.Format("Can't find Inventories for ProductCode({0}), Sale({1})", sale.ProductCode, sale.SaleID));
//                    }
//                    foreach (InventoryView inv in invList)
//                    {
//                        inventoryList.Add(inv);
//                    }
//                }

//                long? regID = null;
//                string request = null;
//                string response = null;


//                NPFGateway gateway = new NPFGateway();
//                try
//                {
//                    regID = gateway.PostShipment(saleID, r, country, inventoryList, out request, out response);
//                }
//                catch (Exception ex)
//                {
//                    logger.Error(GetType(), ex);
//                    if (response == null)
//                    {
//                        response = "Error: " + ex.Message;
//                    }
//                }

//                if (regID != null)
//                {
//                    res = true;
//                }
//                OnOrderSent(saleList, request, response, regID);
//            }
//            catch (Exception ex)
//            {
//                logger.Error(GetType(), ex);
//                res = false;
//            }
//            return res;
//        }

//        private void OnOrderSent(IList<SaleShipperView> saleList, string request, string response, long? regID)
//        {
//            try
//            {
//                dao.BeginTransaction();

//                foreach (SaleShipperView sale in saleList)
//                {
//                    NPFRecord npfRecord = new NPFRecord();

//                    npfRecord.SaleID = sale.SaleID;
//                    npfRecord.Request = request;
//                    npfRecord.Response = response;
//                    npfRecord.RegID = regID;
//                    npfRecord.Completed = false;
//                    npfRecord.CreateDT = DateTime.Now;

//                    dao.Save<NPFRecord>(npfRecord);
//                }

//                dao.CommitTransaction();
//            }
//            catch (Exception ex)
//            {
//                logger.Error(GetType(), ex);
//                dao.RollbackTransaction();
//            }
//        }

//        public override void CheckShipmentsState()
//        {
//            throw new NotImplementedException();
//        }

//        public void OnOrderShipped(long regID, string trackingNumber)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
