using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using System.Net.Mail;
using TrimFuel.Business.Utils;
using System.IO;
using System.Net;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class GeneralShipperService : ShipperService
    {
        private SaleService saleService { get { return new SaleService(); } }

        public override int ShipperID
        {
            get { throw new NotImplementedException(); }
        }

        public override string TableName
        {
            get { throw new NotImplementedException(); }
        }

        public override bool SendShippingOrderInternal(TrimFuel.Model.Registration r, TrimFuel.Model.Billing b, IList<TrimFuel.Model.Views.SaleShipperView> saleList)
        {
            throw new NotImplementedException();
        }

        public override void CheckShipmentsState()
        {
            try
            {
                IList<int> shipperList = GetShipperListToCheckShipments();
                if (shipperList != null)
                {
                    foreach (int shipperID in shipperList)
                    {
                        ShipperService service = GetShipperServiceByShipperID(shipperID);
                        if (service != null)
                        {
                            try
                            {
                                service.CheckShipmentsState();
                            }
                            catch (Exception ex)
                            {
                                logger.Error(GetType(), ex);
                            }
                        }
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
            try
            {
                IList<int> shipperList = GetShipperListToCheckReturns();
                if (shipperList != null)
                {
                    foreach (int shipperID in shipperList)
                    {
                        ShipperService service = GetShipperServiceByShipperID(shipperID);
                        if (service != null)
                        {
                            try
                            {
                                service.CheckReturns();
                            }
                            catch (Exception ex)
                            {
                                logger.Error(GetType(), ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public override void SendPendingOrders()
        {
            try
            {
                IList<int> shipperList = GetShipperListToSendPendingOrders();
                if (shipperList != null)
                {
                    foreach (int shipperID in shipperList)
                    {
                        ShipperService service = GetShipperServiceByShipperID(shipperID);
                        if (service != null && service.IsPendingStateImplementation)
                        {
                            try
                            {
                                service.SendPendingOrders();
                            }
                            catch (Exception ex)
                            {
                                logger.Error(GetType(), ex);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        new public BusinessError<bool> SendSale(long saleID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                Billing b = saleService.GetBillingBySale(saleID);
                if (b == null || b.BillingID == null)
                {
                    throw new Exception("Can't determine Billing by Sale(" + saleID.ToString() + ")");
                }
                int? shipperID = DetermineShipper(b.BillingID.Value);
                if (shipperID == null)
                {
                    throw new Exception("Can't determine Shipper by Billing(" + b.BillingID.Value.ToString() + ")");
                }
                if (shipperID.Value == (short)ShipperEnum.TSN)
                {
                    res = SendSaleToTSNShipper(saleID);
                }
                else
                {
                    ShipperService shipper = GetShipperServiceByShipperID(shipperID.Value);
                    if (shipper == null)
                    {
                        throw new Exception("Shipper(" + shipperID.Value.ToString() + ") is not implemented");
                    }
                    res = shipper.SendSale(saleID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
            }
            return res;
        }

        new public BusinessError<bool> SendSale(long saleID, int? shipperID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                if (shipperID.Value == (short)ShipperEnum.TSN)
                {
                    res = SendSaleToTSNShipper(saleID);
                }
                else
                {
                    Billing b = saleService.GetBillingBySale(saleID);
                    if (b == null || b.BillingID == null)
                    {
                        throw new Exception("Can't determine Billing by Sale(" + saleID.ToString() + ")");
                    }
                    if (shipperID == null)
                    {
                        throw new Exception("ShipperID can not be null");
                    }
                    ShipperService shipper = GetShipperServiceByShipperID(shipperID.Value);
                    if (shipper == null)
                    {
                        throw new Exception("Shipper(" + shipperID.Value.ToString() + ") is not implemented");
                    }
                    res = shipper.SendSale(saleID, shipperID.Value);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
            }
            return res;
        }

        new public BusinessError<bool> SendUpsells(long billingID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                Billing b = EnsureLoad<Billing>(billingID);
                int? shipperID = DetermineShipper(b.BillingID.Value);
                if (shipperID == null)
                {
                    throw new Exception("Can't determine Shipper by Billing(" + b.BillingID.Value.ToString() + ")");
                }
                ShipperService shipper = GetShipperServiceByShipperID(shipperID.Value);
                if (shipper == null)
                {
                    throw new Exception("Shipper(" + shipperID.Value.ToString() + ") is not implemented");
                }
                res = shipper.SendUpsells(billingID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
            }
            return res;
        }

        new public BusinessError<bool> SendExtraTrialShips(long billingID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                Billing b = EnsureLoad<Billing>(billingID);
                int? shipperID = DetermineShipper(b.BillingID.Value);
                if (shipperID == null)
                {
                    throw new Exception("Can't determine Shipper by Billing(" + b.BillingID.Value.ToString() + ")");
                }
                ShipperService shipper = GetShipperServiceByShipperID(shipperID.Value);
                if (shipper == null)
                {
                    throw new Exception("Shipper(" + shipperID.Value.ToString() + ") is not implemented");
                }
                res = shipper.SendExtraTrialShips(billingID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
            }
            return res;
        }

        new public BusinessError<bool> SendExtraTrialShips(long billingID, int shipperID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                Billing b = EnsureLoad<Billing>(billingID);
                ShipperService shipper = GetShipperServiceByShipperID(shipperID);
                if (shipper == null)
                {
                    throw new Exception("Shipper(" + shipperID.ToString() + ") is not implemented");
                }
                res = shipper.SendExtraTrialShips(billingID, shipperID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
            }
            return res;
        }

        public bool SaleShipped(long saleID, string trackingNumber)
        {
            int? shipperID = null;
            long? regID = GetRegIDBySale(saleID, out shipperID);
            if (regID == null || shipperID == null)
            {
                return false;
            }

            ShipperService shipper = GetShipperServiceByShipperID(shipperID.Value);

            //Invalid saleID
            if (shipper == null)
            {
                return false;
            }

            shipper.OnOrderShipped(regID.Value, trackingNumber, DateTime.Now);

            return true;
        }

        public bool UpdateTrackingNumber(long saleID, string trackingNumber)
        {
            int? shipperID = null;
            long? regID = GetRegIDBySale(saleID, out shipperID);
            if (regID == null || shipperID == null)
            {
                return false;
            }

            ShipperService shipper = GetShipperServiceByShipperID(shipperID.Value);

            //Invalid saleID
            if (shipper == null)
            {
                return false;
            }

            shipper.OnTrackingNumberUpdated(regID.Value, trackingNumber, DateTime.Now);

            return true;
        }

        public override void OnOrderShipped(long regID, string trackingNumber, DateTime shippedDate)
        {
            throw new NotImplementedException();
        }

        public override void OnTrackingNumberUpdated(long regID, string trackingNumber, DateTime shippedDate)
        {
            throw new NotImplementedException();
        }

        public override void OnOrderReturned(long regID, string reason, DateTime? returnDate)
        {
            throw new NotImplementedException();
        }

        private BusinessError<bool> SendSaleToTSNShipper(long saleID)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                string url = string.Format("http://{0}/jobs/tsn.asp?saleID={1}", Config.Current.APPLICATION_ID, saleID);
                using (WebClient wc = new WebClient())
                {
                    var resStr = wc.DownloadString(url);
                    if (resStr.Contains("Success"))
                    {
                        res.State = BusinessErrorState.Success;
                        res.ReturnValue = true;
                        res.ErrorMessage = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = false;
                res.State = BusinessErrorState.Error;
            }
            return res;
        }

        public bool CancelUnsentShipments(long? billingID)
        {
            bool res = true;
            try
            {
                dao.BeginTransaction();
                var sales = saleService.GetSaleListByBillingID(billingID);
                foreach (var sale in sales)
                {
                    if (!IsSaleSent(sale.SaleID.Value) && sale.NotShip == false)
                    {
                        sale.NotShip = true;
                        saleService.Save<Sale>(sale);
                        //dao.ExecuteNonQuery(new MySqlCommand("update TFRecord set Cancelled = 1 where SaleID = " + sale.SaleID));
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
                res = false;
            }
            return res;
        }

        public BusinessError<bool> MarkAsSent<RecordType>(string admin, int saleID) where RecordType : ShipperRecord, new()
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, "Unknown error occurred.");

            try
            {
                dao.BeginTransaction();

                string responseMessage = "Shipment marked as sent by " + admin;

                RecordType shipment = new RecordType()
                  {
                      Completed = true,
                      CreateDT = DateTime.Now,
                      RegID = saleID,
                      SaleID = saleID,
                      Request = "Manually marked as sent.",
                      Response = responseMessage,
                      StatusResponse = null,
                      ShippedDT = DateTime.Now
                  };

                dao.Save<RecordType>(shipment);

                MySqlCommand q = new MySqlCommand(@"Delete from AggUnsentShipments where SaleID = @SaleID");
                q.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = saleID;
                dao.ExecuteNonQuery(q);

                dao.CommitTransaction();

                res.ReturnValue = true;
                res.ErrorMessage = null;
                res.State = BusinessErrorState.Success;
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();

                res.ReturnValue = false;
                res.ErrorMessage = null;
                res.State = BusinessErrorState.Error;

                logger.Error(GetType(), ex);
            }

            return res;
        }

        public int? GetShipperByUnsentSaleID(int saleID)
        {
            try
            {
                MySqlCommand q = new MySqlCommand(@"select ShipperID from AggUnsentShipments where SaleID = @SaleID");
                q.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = saleID;
                return dao.ExecuteScalar<int>(q);
            }
            catch(Exception ex)
            {
                logger.Error(GetType(), ex);
                return null;
            }
        }
    }
}
