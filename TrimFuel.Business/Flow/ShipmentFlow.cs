using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Gateways;

namespace TrimFuel.Business.Flow
{
    public class ShipmentFlow : BaseService
    {
        public void ProcessShipment(ShipmentView shipment)
        {
            //do nothing
        }

        public bool ShipShipments(string shipperRegID, long shipperRequestID, string trackingNumber, DateTime shipDate, IList<ProductSKU> preloadProductSKUList)
        {
            bool res = true;
            IList<Shipment> shipmentList = null;
            try
            {
                dao.BeginTransaction();

                shipmentList = new OrderShipmentService().GetShipmentListByShipperRegID(shipperRegID);
                if (shipmentList.Count > 0)
                {
                    foreach (var shipment in shipmentList)
                    {
                        ShipmentShipperRequest link = new ShipmentShipperRequest();
                        link.ShipmentID = shipment.ShipmentID;
                        link.ShipperRequestID = shipperRequestID;
                        dao.Save(link);
                    }

                    ShipShipments(shipmentList, trackingNumber, shipDate);
                }
                else
                {
                    res = false;
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
                res = false;
            }

            if (res && shipmentList != null)
            {
                OnShipmentsShipped(shipmentList, trackingNumber, shipDate, preloadProductSKUList);
            }

            return res;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shipperRegID"></param>
        /// <param name="note"></param>
        /// <param name="trackingNumber"></param>
        /// <param name="shipDate"></param>
        /// <param name="preloadProductSKUList">Full list of ProductSKU to avoid loading it in each call
        /// if preloadProductSKUList = NULL then preloadProductSKUList will be loaded
        /// </param>
        /// <returns></returns>
        public bool ShipShipments(string shipperRegID, string note, string trackingNumber, DateTime shipDate, IList<ProductSKU> preloadProductSKUList)
        {
            bool res = true;
            IList<Shipment> shipmentList = null;
            try
            {
                dao.BeginTransaction();

                shipmentList = new OrderShipmentService().GetShipmentListByShipperRegID(shipperRegID);
                if (shipmentList.Count > 0)
                {
                    ShippingNote shippingNote = new ShippingNote()
                    {
                        Note = note,
                        NoteShipmentStatus = ShipmentStatusEnum.Shipped,
                        CreateDT = DateTime.Now
                    };
                    dao.Save(shippingNote);

                    foreach (var sh in shipmentList)
                    {
                        ShipmentShippingNote link = new ShipmentShippingNote();
                        link.ShipmentID = sh.ShipmentID;
                        link.ShippingNoteID = shippingNote.ShippingNoteID;
                        dao.Save(link);
                    }

                    ShipShipments(shipmentList, trackingNumber, shipDate);
                }
                else
                {
                    res = false;
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
                res = false;
            }

            if (res && shipmentList != null)
            {
                OnShipmentsShipped(shipmentList, trackingNumber, shipDate, preloadProductSKUList);
            }

            return res;
        }

        protected virtual void OnShipmentsShipped(IList<Shipment> shipmentList, string trackingNumber, DateTime shipDate, IList<ProductSKU> preloadProductSKUList)
        {
            try 
	        {	        
		        OrderSale sl = EnsureLoad<OrderSale>(shipmentList[0].SaleID);
                Order o = EnsureLoad<Order>(sl.OrderID);
                Billing b = EnsureLoad<Billing>(o.BillingID);
                Product p = EnsureLoad<Product>(o.ProductID);

                if (preloadProductSKUList == null)
                {
                    preloadProductSKUList = new SubscriptionNewService().GetProductList();
                }

                new EmailService().SendShippingEmail(
                    b, 
                    p,
                    (from sh in shipmentList
                        join pi in preloadProductSKUList on sh.ProductSKU equals pi.ProductSKU_
                        select new Set<Shipment, ProductSKU>()
                        {
                            Value1 = sh,
                            Value2 = pi
                        }).ToList(),
                        trackingNumber,
                    shipDate);
	        }
	        catch (Exception ex)
	        {
                logger.Error(ex);
	        }
        }

        protected virtual bool IsAvailableStatusChange(int fromStatus, int toStatus)
        {
            bool res = false;
            if (toStatus == ShipmentStatusEnum.Blocked &&
                fromStatus < ShipmentStatusEnum.Shipped)
            {
                res = true;
            }
            else
            {
                res = (toStatus >= fromStatus);
            }
            return res;
        }

        protected virtual bool ChangeShipmentStatus(Shipment shipment, int toStatus)
        {
            bool res = false;
            try
            {
                dao.BeginTransaction();

                if (IsAvailableStatusChange(shipment.ShipmentStatus.Value, toStatus))
                {
                    shipment.ShipmentStatus = toStatus;
                    res = true;
                    dao.Save(shipment);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
                res = false;
            }
            return res;
        }

        protected virtual void BlockShipment(Shipment shipment)
        {
            try
            {
                dao.BeginTransaction();

                ChangeShipmentStatus(shipment, ShipmentStatusEnum.Blocked);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        protected virtual void UnblockShipment(Shipment shipment)
        {
            try
            {
                dao.BeginTransaction();

                ChangeShipmentStatus(shipment, ShipmentStatusEnum.New);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        protected virtual void ReturnShipment(Shipment shipment, DateTime returnDate)
        {
            try
            {
                dao.BeginTransaction();

                if (ChangeShipmentStatus(shipment, ShipmentStatusEnum.Returned))
                {
                    shipment.ReturnDT = returnDate;
                    dao.Save(shipment);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        protected virtual void SubmitErrorShipment(Shipment shipment)
        {
            try
            {
                dao.BeginTransaction();

                ChangeShipmentStatus(shipment, ShipmentStatusEnum.SubmitError);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        protected virtual void SubmitShipment(Shipment shipment, int shipperID, string shipperRegID)
        {
            try
            {
                dao.BeginTransaction();

                if (ChangeShipmentStatus(shipment, ShipmentStatusEnum.Submitted))
                {
                    shipment.ShipperID = (short)shipperID;
                    shipment.ShipperRegID = shipperRegID;
                    shipment.SendDT = DateTime.Now;
                    dao.Save(shipment);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        protected virtual void ShipShipment(Shipment shipment, string trackingNumber, DateTime shipDate)
        {
            try
            {
                dao.BeginTransaction();

                if (ChangeShipmentStatus(shipment, ShipmentStatusEnum.Shipped))
                {
                    shipment.TrackingNumber = trackingNumber;
                    shipment.ShipDT = shipDate;
                    dao.Save(shipment);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        public void BlockShipments(IList<Shipment> shipments, string reason)
        {
            try
            {
                dao.BeginTransaction();

                bool isAvailable = true;
                foreach (var item in shipments)
                {
                    if (!IsAvailableStatusChange(item.ShipmentStatus.Value, ShipmentStatusEnum.Blocked))
                    {
                        isAvailable = false;
                        break;
                    }
                }
                if (isAvailable)
                {
                    ShippingNote note = new ShippingNote();
                    note.Note = reason;
                    note.CreateDT = DateTime.Now;
                    note.NoteShipmentStatus = ShipmentStatusEnum.Blocked;
                    dao.Save(note);

                    foreach (var item in shipments)
                    {
                        BlockShipment(item);
                        ShipmentShippingNote link = new ShipmentShippingNote();
                        link.ShipmentID = item.ShipmentID;
                        link.ShippingNoteID = note.ShippingNoteID;
                        dao.Save(link);
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        public void UnblockShipments(IList<Shipment> shipments, string reason)
        {
            try
            {
                dao.BeginTransaction();

                bool isAvailable = true;
                foreach (var item in shipments)
                {
                    if (!IsAvailableStatusChange(item.ShipmentStatus.Value, ShipmentStatusEnum.New))
                    {
                        isAvailable = false;
                        break;
                    }
                }
                if (isAvailable)
                {
                    ShippingNote note = new ShippingNote();
                    note.Note = reason;
                    note.CreateDT = DateTime.Now;
                    note.NoteShipmentStatus = ShipmentStatusEnum.New;
                    dao.Save(note);

                    foreach (var item in shipments)
                    {
                        UnblockShipment(item);
                        ShipmentShippingNote link = new ShipmentShippingNote();
                        link.ShipmentID = item.ShipmentID;
                        link.ShippingNoteID = note.ShippingNoteID;
                        dao.Save(link);
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        public void ReturnShipments(IList<Shipment> shipments, string reason, DateTime returnDate)
        {
            try
            {
                dao.BeginTransaction();

                bool isAvailable = true;
                foreach (var item in shipments)
                {
                    if (!IsAvailableStatusChange(item.ShipmentStatus.Value, ShipmentStatusEnum.Returned))
                    {
                        isAvailable = false;
                        break;
                    }
                }
                if (isAvailable)
                {
                    ShippingNote note = new ShippingNote();
                    note.Note = reason;
                    note.CreateDT = DateTime.Now;
                    note.NoteShipmentStatus = ShipmentStatusEnum.Returned;
                    dao.Save(note);

                    foreach (var item in shipments)
                    {
                        ReturnShipment(item, returnDate);
                        ShipmentShippingNote link = new ShipmentShippingNote();
                        link.ShipmentID = item.ShipmentID;
                        link.ShippingNoteID = note.ShippingNoteID;
                        dao.Save(link);
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        public void SubmitErrorShipments(IList<Shipment> shipments, string error)
        {
            try
            {
                dao.BeginTransaction();

                bool isAvailable = true;
                foreach (var item in shipments)
                {
                    if (!IsAvailableStatusChange(item.ShipmentStatus.Value, ShipmentStatusEnum.SubmitError))
                    {
                        isAvailable = false;
                        break;
                    }
                }
                if (isAvailable)
                {
                    ShippingNote note = new ShippingNote();
                    note.Note = error;
                    note.CreateDT = DateTime.Now;
                    note.NoteShipmentStatus = ShipmentStatusEnum.SubmitError;
                    dao.Save(note);

                    foreach (var item in shipments)
                    {
                        SubmitErrorShipment(item);
                        ShipmentShippingNote link = new ShipmentShippingNote();
                        link.ShipmentID = item.ShipmentID;
                        link.ShippingNoteID = note.ShippingNoteID;
                        dao.Save(link);
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        public void SubmitShipments(IList<Shipment> shipments, int shipperID, string shipperRegID)
        {
            try
            {
                dao.BeginTransaction();

                int resultState = (!string.IsNullOrEmpty(shipperRegID) ? ShipmentStatusEnum.Submitted : ShipmentStatusEnum.SubmitError);
                bool isAvailable = true;
                foreach (var item in shipments)
                {
                    if (!IsAvailableStatusChange(item.ShipmentStatus.Value, resultState))
                    {
                        isAvailable = false;
                        break;
                    }
                }
                if (isAvailable)
                {
                    foreach (var item in shipments)
                    {
                        if (resultState == ShipmentStatusEnum.Submitted)
                            SubmitShipment(item, shipperID, shipperRegID);
                        else
                            SubmitErrorShipment(item);
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        protected void ShipShipments(IList<Shipment> shipments, string trackingNumber, DateTime shipDate)
        {
            try
            {
                dao.BeginTransaction();

                bool isAvailable = true;
                foreach (var item in shipments)
                {
                    if (!IsAvailableStatusChange(item.ShipmentStatus.Value, ShipmentStatusEnum.Shipped))
                    {
                        isAvailable = false;
                        break;
                    }
                }
                if (isAvailable)
                {
                    foreach (var item in shipments)
                    {
                        ShipShipment(item, trackingNumber, shipDate);
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }
    }
}
