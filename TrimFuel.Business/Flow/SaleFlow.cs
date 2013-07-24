using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Flow
{
    public class SaleFlow : BaseService
    {
        public SaleFlow()
        {
            PlanFlow = new PlanFlow();
            ShipmentFlow = new ShipmentFlow();
            PaymentFlow = new PaymentFlow();
        }

        public PlanFlow PlanFlow { get; set; }
        public ShipmentFlow ShipmentFlow { get; set; }
        public PaymentFlow PaymentFlow { get; set; }

        public void ProcessSale(OrderSaleView sale)
        {
            try
            {
                //process products
                foreach (OrderProductView product in sale.ProductList)
                {
                    ProcessProduct(product);
                }
                //process plans
                foreach (OrderRecurringPlan plan in sale.PlanList)
                {
                    PlanFlow.ProcessPlan(plan);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public string GetTrackingNumberBySale(long? saleID)
        {
            string res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from Shipment where SaleID=@SaleID Order By CreateDT");
                q.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = saleID;
                var shipmentList = dao.Load<Shipment>(q).ToList();
                if (shipmentList.Count > 0)
                    res = shipmentList.FirstOrDefault().TrackingNumber;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public bool IsBlockingAllowed(long? saleID)
        {
            bool res = false;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select * from Shipment
                    where not(TrackingNumber is null and ShipmentStatus > 0 and (ShipperRegID is null or ShipperID = 10)) and SaleID = @saleID
                ");
                q.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = saleID;
                var shipmentList = dao.Load<Shipment>(q).ToList();
                if (shipmentList.Count == 0)
                    res = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = false;
            }
            return res;
        }
        
        protected virtual void ProcessProduct(OrderProductView product)
        {
            //only shipments for now
            Inventory inv = new InventoryService().GetInventoryBySKU(product.OrderProduct.ProductSKU);
            if (inv.InventoryType == InventoryTypeEnum.Inventory)
            {
                for (int i = 0; i < product.OrderProduct.Quantity; i++)
                {
                    Shipment sh = new Shipment()
                    {
                        CreateDT = DateTime.Now,
                        SaleID = product.OrderProduct.SaleID,
                        ProductSKU = product.OrderProduct.ProductSKU,
                        ShipmentStatus = ShipmentStatusEnum.New
                    };
                    dao.Save(sh);
                    ShipmentView shView = new ShipmentView()
                    {
                        Shipment = sh,
                        ProductSKU = product.ProductSKU,
                        Sale = product.Sale
                    };
                    if (product.Sale.ShipmentList == null)
                        product.Sale.ShipmentList = new List<ShipmentView>();
                    product.Sale.ShipmentList.Add(shView);
                    ShipmentFlow.ProcessShipment(shView);
                }
            }
        }

        public bool SetChargeback(long saleID, int? chargebackStatusTypeID, int? chargebackReasonCodeID,
            string caseNumber, string arnNumber,
            DateTime? postDate, DateTime? disputeSentDate)
        {
            bool res = true;
            try
            {
                dao.BeginTransaction();

                OrderSale sale = EnsureLoad<OrderSale>(saleID);
                Order order = EnsureLoad<Order>(sale.OrderID);

                SaleChargeback chargeback = (new SaleService()).GetSaleChargeback(saleID);
                if (chargebackStatusTypeID == null && chargeback != null)
                {
                    MySqlCommand q = new MySqlCommand(@"
                        delete from SaleChargeback where SaleID = @saleID
                    ");
                    q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                    dao.ExecuteNonQuery(q);
                }
                else if (chargebackStatusTypeID != null)
                {
                    if (chargeback == null)
                    {
                        chargeback = new SaleChargeback();
                        chargeback.CreateDT = DateTime.Now;
                        chargeback.SaleID = Convert.ToInt32(saleID);
                    }
                    chargeback.BillingID = (int)order.BillingID.Value;
                    chargeback.ChargebackStatusTID = chargebackStatusTypeID;
                    chargeback.ChargebackReasonCodeID = chargebackReasonCodeID;
                    chargeback.CaseNumber = caseNumber;
                    chargeback.ARN = arnNumber;
                    chargeback.PostDT = postDate;
                    chargeback.DisputeSentDT = disputeSentDate;
                    dao.Save<SaleChargeback>(chargeback);
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

        public BusinessError<ChargeHistoryView> ProcessRefundOrVoid(long saleID, decimal refundAmount)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Error, "Unknown error occurred");

            try
            {
                OrderService orders = new OrderService();
                OrderSale sale = EnsureLoad<OrderSale>(saleID);
                if (refundAmount <= 0M)
                {
                    res.ErrorMessage = "Invalid refund amount";
                }
                else
                {
                    if (sale.InvoiceID == null)
                    {
                        res.ErrorMessage = string.Format("Transaction was not found for Sale #{0}", saleID);
                    }
                    else
                    {
                        ChargeHistoryView charge = orders.GetInvoiceChargeHistory(sale.InvoiceID.Value)
                            .FirstOrDefault(i => i.ChargeHistory.Amount > 0M && i.ChargeHistory.Success == true);
                        if (charge == null)
                        {
                            res.ErrorMessage = string.Format("Transaction was not found for Sale #{0}", saleID);
                        }
                        else
                        {
                            DateTime nowDatePlus1Hour = DateTime.Now.AddHours(1);
                            bool isRefundAvailable = (nowDatePlus1Hour.Date > charge.ChargeHistory.ChargeDate.Value.Date);
                            if (isRefundAvailable)
                            {
                                res = PaymentFlow.ProcessRefund(charge, refundAmount, sale.InvoiceID);
                                if (res.State == BusinessErrorState.Success)
                                {
                                    OnSaleRefunded(sale, charge, res.ReturnValue);
                                }
                            }
                            else if (refundAmount == charge.CurrencyAmount.Value)
                            {
                                res = PaymentFlow.ProcessVoid(charge, sale.InvoiceID);
                                if (res.State == BusinessErrorState.Success)
                                {
                                    OnSaleRefunded(sale, charge, res.ReturnValue);
                                }
                            }
                            else
                            {
                                if (OnPartialVoid(sale, charge, refundAmount))
                                {
                                    res.State = BusinessErrorState.Success;
                                    res.ErrorMessage = "Refund attempt added to queue and will be processed in 24 hours";
                                }
                                else
                                {
                                    res.ErrorMessage = "Refund attempt failed";
                                }
                            }
                            if (res.ReturnValue != null)
                            {
                                SaleRefund saleRef = new SaleRefund();
                                saleRef.SaleID = sale.SaleID;
                                saleRef.ChargeHistoryID = res.ReturnValue.ChargeHistory.ChargeHistoryID;
                                dao.Save(saleRef);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public virtual void OnSaleRefunded(OrderSale sale, ChargeHistoryView originalChargeHistory, ChargeHistoryView refundChargeHistory)
        {
            try
            {
                //Block shipments for full refunds
                if (Math.Abs(refundChargeHistory.CurrencyAmount.Value) >= originalChargeHistory.CurrencyAmount.Value ||
                    Math.Abs(refundChargeHistory.CurrencyAmount.Value) >= sale.ChargedAmount.Value)
                {
                    IList<Shipment> shipmentList = (new OrderService()).GetSaleShipments(sale.SaleID.Value);
                    ShipmentFlow.BlockShipments(shipmentList, "Blocked due to full refund");
                }

                //TODO: trigger refund e-mail
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected virtual bool OnPartialVoid(OrderSale sale, ChargeHistoryView originalChargeHistory, decimal refundAmount)
        {
            bool res = true;
            try
            {
                Invoice invoice = EnsureLoad<Invoice>(sale.InvoiceID);
                Order order = EnsureLoad<Order>(invoice.OrderID);

                VoidQueue voidQueue = new VoidQueue();
                voidQueue.Amount = refundAmount;
                voidQueue.BillingID = order.BillingID;
                voidQueue.Completed = false;
                voidQueue.CreateDT = DateTime.Now;
                voidQueue.SaleChargeDT = originalChargeHistory.ChargeHistory.ChargeDate;
                voidQueue.SaleID = sale.SaleID;
                dao.Save(voidQueue);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = false;
            }
            return res;
        }

        public BusinessError<ReturnedSale> ReturnSale(long saleID, string reason, DateTime? returnDate, int adminID)
        {
            BusinessError<ReturnedSale> res = new BusinessError<ReturnedSale>(null, BusinessErrorState.Error, "Unknown error occurred");

            try
            {
                ReturnSaleService retServ = new ReturnSaleService();
                if (retServ.IsSaleReturned(saleID).Value)
                {
                    res.ErrorMessage = "Sale is alredy returned";
                }
                else
                {
                    OrderSale sale = EnsureLoad<OrderSale>(saleID);
                    Order order = EnsureLoad<Order>(sale.OrderID);
                    Billing billing = EnsureLoad<Billing>(order.BillingID);
                    ReturnedSale retSale = retServ.CreateReturnSale(saleID, billing.BillingID.Value, reason, returnDate, adminID);
                    if (retSale != null)
                    {
                        //Notes from actionRes could be added to billing Notes
                        SaleReturnProcessing processing = retServ.GetReturnActionBySaleID(saleID);
                        if (processing != null && processing.ReturnProcessingActionID == null)
                        {
                            processing = null;
                        }
                        var res2 = ProcessReturnSaleAction(sale, retSale, processing, order, billing);

                        res.ReturnValue = retSale;
                        res.State = BusinessErrorState.Success;
                        res.ErrorMessage = res2.ErrorMessage;

                        IList<Shipment> shipmentList = (new OrderService()).GetSaleShipments(sale.SaleID.Value);
                        ShipmentFlow.ReturnShipments(shipmentList, reason, (returnDate != null ? returnDate.Value : DateTime.Now));
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        protected virtual BusinessError<bool> ProcessReturnSaleAction(OrderSale sale, ReturnedSale retSale, SaleReturnProcessing processing, Order order, Billing billing)
        {
            BusinessError<bool> res = new BusinessError<bool>(false, BusinessErrorState.Error, "Unknow error occurred");
            try
            {
                if (processing == null)
                {
                    res = ProcessDefaultReturnSaleAction(sale, retSale, order, billing);
                }
                else
                {
                    switch (processing.ReturnProcessingActionID.Value)
                    {
                        case ReturnProcessingActionEnum.ISSUE_REFUND:
                            {
                                var res2 = ProcessRefundOrVoid(sale.SaleID.Value, processing.RefundAmount.Value);
                                if (res2.State == BusinessErrorState.Success)
                                {
                                    res.ReturnValue = true;
                                    res.ErrorMessage = "Refund Processed: " + res2.ReturnValue.ChargeHistory.Response;
                                    res.State = BusinessErrorState.Success;
                                }
                                else
                                {
                                    res.ErrorMessage = "Refund Failed: " + (res2.ReturnValue != null ? res2.ReturnValue.ChargeHistory.Response : res2.ErrorMessage);
                                }
                                break;
                            }
                        case ReturnProcessingActionEnum.CANCEL_ACCOUNT:
                            {
                                OrderService orderServ = new OrderService();
                                OrderRecurringPlan plan = orderServ.GetPlanBySale(sale);
                                if (plan != null)
                                {
                                    if (PlanFlow.SetPlanStatus(plan, RecurringStatusEnum.Inactive) != null)
                                    {
                                        res.ErrorMessage = "Subscription has been updated to Inactive state";
                                        res.ReturnValue = true;
                                        res.State = BusinessErrorState.Success;
                                    }
                                }
                                else
                                {
                                    res.ErrorMessage = "Can't change Plan. Subscription Plan was not found";
                                }
                                break;
                            }
                        case ReturnProcessingActionEnum.CHANGE_PLAN:
                            {
                                OrderService orderServ = new OrderService();
                                OrderRecurringPlan plan = orderServ.GetPlanBySale(sale);
                                if (plan != null)
                                {
                                    if (PlanFlow.UpdatePlan(plan, processing.NewRecurringPlanID.Value, plan.NextCycleDT.Value, plan.RecurringStatus.Value) != null)
                                    {
                                        res.ErrorMessage = "Subscription Plan has been changed";
                                        res.ReturnValue = true;
                                        res.State = BusinessErrorState.Success;
                                    }
                                }
                                else
                                {
                                    res.ErrorMessage = "Can't change Plan. Subscription Plan was not found";
                                }
                                break;
                            }
                        case ReturnProcessingActionEnum.SHIP_FREE_ITEM:
                            {
                                OrderBuilder ord = new OrderBuilder();
                                BusinessError<OrderView> newOrder =
                                    ord.Create(billing.BillingID, order.CampaignID, Config.Current.APPLICATION_ID, null, null, order.Affiliate, order.SubAffiliate, order.ProductID)
                                        .AppendExtraTrialShipType(processing.ExtraTrialShipTypeID.Value, processing.Quantity.Value)
                                        .Save();
                                if (newOrder.State == BusinessErrorState.Success)
                                {
                                    OrderFlow orderFlow = new OrderFlow();
                                    var res2 = orderFlow.ProcessOrder(newOrder.ReturnValue);

                                    if (res2.State == BusinessErrorState.Success)
                                    {
                                        res.ReturnValue = true;
                                        res.ErrorMessage = "Free Item has been sent.";
                                        res.State = BusinessErrorState.Success;
                                    }
                                    else
                                    {
                                        res.ErrorMessage = "Can't send Free Item. " + res2.ErrorMessage;
                                    }
                                }
                                else
                                {
                                    res.ErrorMessage = "Can't send Free Item. " + newOrder.ErrorMessage;
                                }
                                break;
                            }
                        case ReturnProcessingActionEnum.BILL_AND_SHIP_ITEM:
                            {
                                OrderBuilder ord = new OrderBuilder();
                                BusinessError<OrderView> newOrder =
                                    ord.Create(billing.BillingID, order.CampaignID, Config.Current.APPLICATION_ID, null, null, order.Affiliate, order.SubAffiliate, order.ProductID)
                                        .AppendUpsellType(processing.UpsellTypeID.Value, processing.Quantity.Value)
                                        .Save();
                                if (newOrder.State == BusinessErrorState.Success)
                                {
                                    OrderFlow orderFlow = new OrderFlow();
                                    var res2 = orderFlow.ProcessOrder(newOrder.ReturnValue);

                                    if (res2.State == BusinessErrorState.Success)
                                    {
                                        res.ReturnValue = true;
                                        res.ErrorMessage = "Item has been billed and sent.";
                                        res.State = BusinessErrorState.Success;
                                    }
                                    else
                                    {
                                        res.ErrorMessage = "Can't bill and send Item. " +
                                            (res2.ReturnValue != null &&
                                            res2.ReturnValue.Count > 0 &&
                                            res2.ReturnValue.Last().ChargeResult != null
                                            ?
                                                res2.ReturnValue.Last().ChargeResult.ChargeHistory.Response
                                            :
                                                "");
                                    }
                                }
                                else
                                {
                                    res.ErrorMessage = "Can't bill and send Item. " + newOrder.ErrorMessage;
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        protected virtual BusinessError<bool> ProcessDefaultReturnSaleAction(OrderSale sale, ReturnedSale retSale, Order order, Billing billing)
        {
            BusinessError<bool> res = new BusinessError<bool>();
            try
            {
                ReturnSaleService retServ = new ReturnSaleService();
                OrderService orderServ = new OrderService();
                OrderRecurringPlan plan = orderServ.GetPlanBySale(sale);
                if (plan != null)
                {
                    bool adminNoteExists = retServ.GetAdminNotesByBillingID(billing.BillingID.Value).Count > 0;
                    if (!adminNoteExists)
                    {
                        if (PlanFlow.SetPlanStatus(plan, RecurringStatusEnum.Inactive) != null)
                        {
                            res.ErrorMessage = "Subscription has been updated to Inactive state";
                            res.ReturnValue = true;
                            res.State = BusinessErrorState.Success;
                        }
                    }
                    else
                    {
                        if (PlanFlow.SetPlanStatus(plan, RecurringStatusEnum.ReturnedNoRMA) != null)
                        {
                            res.ErrorMessage = "Subscription has been updated to Inactive (Returned No RMA) state";
                            res.ReturnValue = true;
                            res.State = BusinessErrorState.Success;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public bool UpdateTrackingNumber(long? saleID, string trackingNumber, int? shipperID, string adminName)
        {
            bool res = false;
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("Select * from Shipment where SaleID=@SaleID Order By CreateDT");
                q.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = saleID;
                var shipmentList = dao.Load<Shipment>(q).ToList();
                var disctinctRegIDList = shipmentList.Select(u => u.ShipperRegID).Distinct().ToList();
                if (shipmentList.Count > 0)
                {
                    var submitedShipments = shipmentList.Where(u => !string.IsNullOrEmpty(u.ShipperRegID)).ToList();
                    var notSubmitedShipments = shipmentList.Where(u => string.IsNullOrEmpty(u.ShipperRegID)).ToList();

                    if (notSubmitedShipments.Count > 0 && shipperID != null)
                    {
                        //submit shipments with RegID = null
                        ShipmentFlow.SubmitShipments(notSubmitedShipments, shipperID.Value, "MAN_" + saleID.ToString());
                        disctinctRegIDList.Add("MAN_" + saleID.ToString());
                    }

                    foreach(var regID in disctinctRegIDList)
                        ShipmentFlow.ShipShipments(regID, "Tracking number updated manually by " + adminName, trackingNumber, DateTime.Now, null);
                    
                    res = true;
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                res = false;
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public bool BlockShipments(long? saleID)
        {
            bool res = false;
            int? shipperID = null;
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("Select * from Shipment where SaleID=@SaleID Order By CreateDT");
                q.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = saleID;
                var shipmentList = dao.Load<Shipment>(q).ToList();

                if (shipmentList.Count > 0)
                {
                    if (shipmentList.Count > 0)
                        shipperID = shipmentList.FirstOrDefault().ShipperID;

                    var submitedShipments = shipmentList.Where(u => !string.IsNullOrEmpty(u.ShipperRegID)).ToList();
                    var notSubmitedShipments = shipmentList.Where(u => string.IsNullOrEmpty(u.ShipperRegID)).ToList();
                    var trackingShipments = shipmentList.Where(u => !string.IsNullOrEmpty(u.TrackingNumber)).ToList();

                    if (trackingShipments.Count > 0)
                        return false;
                    if (submitedShipments.Count > 0 && (shipperID == null || shipperID.Value != 10))
                        return false;

                    ShipmentFlow.BlockShipments(shipmentList, "Cancelled by user");
                    res = true;
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }

            return res;
        }
    }
}
