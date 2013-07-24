using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Flow
{
    public class OrderFlow : BaseService
    {
        public OrderFlow()
        {
            PaymentFlow = new PaymentFlow();
            SaleFlow = new SaleFlow();
        }

        public OrderFlow(PaymentFlow paymentFlow, SaleFlow saleFlow)
        {
            PaymentFlow = paymentFlow;
            SaleFlow = saleFlow;
        }

        public PaymentFlow PaymentFlow { get; set; }
        public SaleFlow SaleFlow { get; set; }

        public virtual BusinessError<IList<ChargeHistoryView>> UpdateOrder(OrderView order, bool scrubOrder, bool blockShipments, bool voidTransactions, int? adminID)
        {            
            BusinessError<IList<ChargeHistoryView>> res = new BusinessError<IList<ChargeHistoryView>>(new List<ChargeHistoryView>(), BusinessErrorState.Success, "");
            try 
	        {
                Admin admin = null;
                string adminName = "Automatic Process";
                if (adminID != null)
                {
                    admin = Load<Admin>(adminID);
                    if (admin != null)
                    {
                        adminName = admin.DisplayName;
                    }
                }

		        if (order.Order.Scrub != scrubOrder)
                {
                    order.Order.Scrub = scrubOrder;
                    dao.Save(order.Order);
                }
                foreach (var plan in order.GetAllPLans())
                {
                    if (plan.RecurringStatus != RecurringStatusEnum.New)
                    {
                        if (scrubOrder && plan.RecurringStatus != RecurringStatusEnum.Scrubbed)
                        {
                            plan.RecurringStatus = RecurringStatusEnum.Scrubbed;
                            dao.Save(plan);
                        }
                        if (!scrubOrder && plan.RecurringStatus == RecurringStatusEnum.Scrubbed)
                        {
                            plan.RecurringStatus = RecurringStatusEnum.Inactive;
                            dao.Save(plan);
                        }
                    }
                }
                if (voidTransactions)
                {
                    foreach (var invoice in order.InvoiceList)
                    {
                        if (invoice.Invoice.InvoiceStatus == InvoiceStatusEnum.Paid &&
                            invoice.Invoice.Amount > 0M)
                        {
                            var res2 = new PaymentFlow().ProcessFullRefund(invoice);
                            if (res2.ReturnValue != null)
                            {
                                res.ReturnValue.Add(res2.ReturnValue);
                            }
                        }
                    }
                }
                if (blockShipments)
                {
                    foreach (var sale in order.SaleList)
                    {
                        if (sale.OrderSale.SaleStatus == SaleStatusEnum.Approved)
                        {
                            IList<Shipment> shipmentList = new OrderService().GetSaleShipments(sale.OrderSale.SaleID.Value);
                            new ShipmentFlow().BlockShipments(shipmentList, "Blocked by " + adminName);
                        }
                    }
                }
                else
                {
                    foreach (var sale in order.SaleList)
                    {
                        if (sale.OrderSale.SaleStatus == SaleStatusEnum.Approved)
                        {
                            bool fullRefunded = true;
                            if (sale.OrderSale.ChargedAmount > 0M)
                            {
                                var refunds = new SaleService().GetSaleRefunds(sale.OrderSale.SaleID.Value);
                                if (refunds != null && refunds.Count > 0 &&
                                    refunds.Sum(i => i.CurrencyAmount.Value) >= sale.OrderSale.ChargedAmount.Value)
                                {
                                    fullRefunded = true;
                                }
                                else
                                {
                                    fullRefunded = false;
                                }
                            }
                            else
                            {
                                fullRefunded = false;
                            }
                            if (!fullRefunded)
                            {
                                IList<Shipment> shipmentList = new OrderService().GetSaleShipments(sale.OrderSale.SaleID.Value);
                                new ShipmentFlow().UnblockShipments(shipmentList, "Unblocked by " + adminName);
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

        public BusinessError<IList<InvoiceView2>> ProcessOrder(OrderView order)
        {
            BusinessError<IList<InvoiceView2>> res = new BusinessError<IList<InvoiceView2>>(new List<InvoiceView2>(), BusinessErrorState.Error, "Unknown error occurred.");
            try
            {
                if (!ValidateOrder(order))
                {
                    //todo: add full description of error
                    throw new Exception("Invalid Order attempt.");
                }
                bool isTest = IsTestOrder(order);
                if (isTest)
                {
                    UpdateOrder(order, true, false, false, null);
                }
                foreach (InvoiceView invoice in CreateInvoices(order))
                {
                    if (isTest)
                    {
                        var res1 = ProcessInvoiceTest(invoice);
                        InvoiceView2 invRes = new InvoiceView2();
                        invRes.Invoice = invoice;
                        invRes.ChargeResult = res1.ReturnValue;
                        res.ReturnValue.Add(invRes);
                        if (res1.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;
                            res.ErrorMessage = "";
                        }
                        else
                        {
                            res.ErrorMessage = res1.ErrorMessage;
                        }
                    }
                    else
                    {
                        if (SaveInvoice(invoice))
                        {
                            var res1 = ProcessInvoice(invoice);
                            InvoiceView2 invRes = new InvoiceView2();
                            invRes.Invoice = invoice;
                            invRes.ChargeResult = res1.ReturnValue;
                            res.ReturnValue.Add(invRes);
                            if (res1.State == BusinessErrorState.Success)
                            {
                                res.State = BusinessErrorState.Success;
                                res.ErrorMessage = "";
                            }
                            else
                            {
                                res.ErrorMessage = res1.ErrorMessage;
                            }
                        }
                    }
                }
                if (res.State == BusinessErrorState.Success)
                {
                    OnOrderProcessed(order, res.ReturnValue);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public virtual void OnOrderProcessed(OrderView order, IList<InvoiceView2> invoicesProcessed)
        {
            try
            {
                SaleService saleService = new SaleService();
                saleService.ValidateCustomer(order.Billing.BillingID.Value);
                saleService.ValidateFraud(order.Billing.BillingID.Value, order.SaleList.Last().OrderSale.SaleID.Value);
                foreach (var invoice in invoicesProcessed)
                {
                    if (invoice.Invoice.SaleList.Where(i => i.OrderSale.SaleStatus == SaleStatusEnum.Approved && i.OrderSale.SaleType == OrderSaleTypeEnum.Trial).Count() > 0)
                    {                       
                        //new EmailService().SendConfirmationTrialEmail(invoice);
                        foreach(var sale in invoice.Invoice.SaleList)
                            new EmailService().PushConfirmationEmailToQueue(invoice.Invoice.Order.Billing.BillingID, sale.OrderSale == null ? null : sale.OrderSale.SaleID);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        //public IList<InvoiceView> GetInvoiceListToProcess(OrderView order)
        //{
        //    IList<InvoiceView> res = null;
        //    try
        //    {

        //        res = CreateInvoices(order);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //    }
        //    return res;
        //}

        private bool SaveInvoice(InvoiceView invoice)
        {
            bool res = true;
            try
            {
                dao.BeginTransaction();

                invoice.Invoice.InvoiceID = null;
                invoice.Invoice.InvoiceStatus = InvoiceStatusEnum.New;
                invoice.Invoice.CreateDT = DateTime.Now;
                dao.Save(invoice.Invoice);
                foreach (OrderSaleView s in invoice.SaleList)
                {
                    s.OrderSale.InvoiceID = invoice.Invoice.InvoiceID;
                    dao.Save(s.OrderSale);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                res = false;
                logger.Error(ex);
            }
            return res;
        }

        private BusinessError<ChargeHistoryView> ProcessInvoice(InvoiceView invoice)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Success, "Unknown error occurred");
            try
            {
                if (invoice.Invoice.Amount <= 0.0M && invoice.Invoice.AuthAmount <= 0.0M)
                {
                    res.State = BusinessErrorState.Success;
                    res.ErrorMessage = "";
                }
                else if (invoice.Invoice.Amount <= 0.0M)
                {
                    res = PaymentFlow.ProcessAuth(invoice);
                }
                else
                {
                    res = PaymentFlow.ProcessCharge(invoice);
                }

                if (res.State == BusinessErrorState.Success)
                {
                    try
                    {
                        dao.BeginTransaction();

                        invoice.Invoice.InvoiceStatus = InvoiceStatusEnum.Paid;
                        invoice.Invoice.ProcessDT = DateTime.Now;
                        dao.Save(invoice.Invoice);

                        if (invoice.Order.Order.OrderStatus != OrderStatusEnum.Active)
                        {
                            invoice.Order.Order.OrderStatus = OrderStatusEnum.Active;
                            dao.Save(invoice.Order.Order);
                        }

                        foreach (OrderSaleView sale in invoice.SaleList)
                        {
                            sale.OrderSale.SaleStatus = SaleStatusEnum.Approved;
                            sale.OrderSale.ProcessDT = DateTime.Now;
                            dao.Save(sale.OrderSale);
                        }

                        dao.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        dao.RollbackTransaction();
                        logger.Error(ex);
                    }

                    foreach (OrderSaleView sale in invoice.SaleList)
                    {
                        SaleFlow.ProcessSale(sale);
                    }
                }
                else if (res.State == BusinessErrorState.Error && res.ReturnValue == null)
                {
                    //pending sale
                    try
                    {
                        dao.BeginTransaction();

                        //invoice.Invoice.InvoiceStatus = InvoiceStatusEnum.Pending;
                        //dao.Save(invoice.Invoice);

                        dao.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        dao.RollbackTransaction();
                        logger.Error(ex);
                    }
                }
                else if (res.State == BusinessErrorState.Error && res.ReturnValue != null)
                {
                    //pending sale
                    try
                    {
                        dao.BeginTransaction();

                        invoice.Invoice.ProcessDT = DateTime.Now;
                        dao.Save(invoice.Invoice);

                        foreach (OrderSaleView sale in invoice.SaleList)
                        {
                            sale.OrderSale.SaleStatus = SaleStatusEnum.Declined;
                            sale.OrderSale.ProcessDT = DateTime.Now;
                            dao.Save(sale.OrderSale);
                        }

                        dao.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        dao.RollbackTransaction();
                        logger.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        private BusinessError<ChargeHistoryView> ProcessInvoiceTest(InvoiceView invoice)
        {
            BusinessError<ChargeHistoryView> res = new BusinessError<ChargeHistoryView>(null, BusinessErrorState.Success, "Unknown error occurred");
            try
            {
                if (invoice.Invoice.Amount <= 0.0M)
                {
                    res.State = BusinessErrorState.Success;
                    res.ErrorMessage = "";
                }
                else
                {
                    res = PaymentFlow.ProcessTestCharge(invoice);
                }

                if (res.State == BusinessErrorState.Success)
                {
                    //try
                    //{
                    //    dao.BeginTransaction();

                    //    invoice.Invoice.InvoiceStatus = InvoiceStatusEnum.Paid;
                    //    invoice.Invoice.ProcessDT = DateTime.Now;
                    //    dao.Save(invoice.Invoice);

                    //    foreach (OrderSaleView sale in invoice.SaleList)
                    //    {
                    //        sale.OrderSale.SaleStatus = SaleStatusEnum.Approved;
                    //        sale.OrderSale.ProcessDT = DateTime.Now;
                    //        dao.Save(sale.OrderSale);
                    //    }

                    //    dao.CommitTransaction();
                    //}
                    //catch (Exception ex)
                    //{
                    //    dao.RollbackTransaction();
                    //    logger.Error(ex);
                    //}

                    //foreach (OrderSaleView sale in invoice.SaleList)
                    //{
                    //    SaleFlow.ProcessSale(sale);
                    //}
                }
                else if (res.State == BusinessErrorState.Error && res.ReturnValue == null)
                {
                    ////pending sale
                    //try
                    //{
                    //    dao.BeginTransaction();

                    //    //invoice.Invoice.InvoiceStatus = InvoiceStatusEnum.Pending;
                    //    //dao.Save(invoice.Invoice);

                    //    dao.CommitTransaction();
                    //}
                    //catch (Exception ex)
                    //{
                    //    dao.RollbackTransaction();
                    //    logger.Error(ex);
                    //}
                }
                else if (res.State == BusinessErrorState.Error && res.ReturnValue != null)
                {
                    ////pending sale
                    //try
                    //{
                    //    dao.BeginTransaction();

                    //    invoice.Invoice.ProcessDT = DateTime.Now;
                    //    dao.Save(invoice.Invoice);

                    //    foreach (OrderSaleView sale in invoice.SaleList)
                    //    {
                    //        sale.OrderSale.SaleStatus = SaleStatusEnum.Declined;
                    //        sale.OrderSale.ProcessDT = DateTime.Now;
                    //        dao.Save(sale.OrderSale);
                    //    }

                    //    dao.CommitTransaction();
                    //}
                    //catch (Exception ex)
                    //{
                    //    dao.RollbackTransaction();
                    //    logger.Error(ex);
                    //}
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        protected virtual IList<InvoiceView> CreateInvoices(OrderView order)
        {
            IList<InvoiceView> res = new List<InvoiceView>();
            Currency currency = new SaleService().GetCurrencyByProduct(order.Order.ProductID.Value);
            int? currencyID = (currency != null ? currency.CurrencyID : null);


            IEnumerable<OrderSaleView> trialSales = from s in order.SaleList
                                                     where s.OrderSale.SaleType == OrderSaleTypeEnum.Trial && s.OrderSale.InvoiceID == null && s.OrderSale.SaleStatus == SaleStatusEnum.New
                                                     select s;
            
            IEnumerable<OrderSaleView> singleSales = from s in order.SaleList
                                                    where s.OrderSale.SaleType == OrderSaleTypeEnum.Upsell && s.OrderSale.SaleStatus == SaleStatusEnum.New
                                                    select s;

            if (trialSales.Count() > 0)
            {
                res.Add(new InvoiceView()
                {
                    Invoice = new Invoice() { OrderID = order.Order.OrderID, InvoiceStatus = InvoiceStatusEnum.New, Amount = CalculateInvoice(trialSales.ToList()), AuthAmount = 0.0M, CurrencyID = currencyID },
                    Order = order,
                    Currency = currency,
                    SaleList = trialSales.ToList()
                });
            }

            if (singleSales.Count() > 0)
            {
                res.Add(new InvoiceView()
                {
                    Invoice = new Invoice() { OrderID = order.Order.OrderID, InvoiceStatus = InvoiceStatusEnum.New, Amount = CalculateInvoice(singleSales.ToList()), AuthAmount = 0.0M, CurrencyID = currencyID  },
                    Order = order,
                    Currency = currency,
                    SaleList = singleSales.ToList()
                });
            }

            return res;
        }

        protected virtual decimal CalculateInvoice(IList<OrderSaleView> sales)
        {
            decimal amount = 0.0M;

            amount = amount + sales.Sum(i => i.OrderSale.Quantity.Value * i.OrderSale.PurePrice.Value);

            return amount;
        }

        private bool ValidateOrder(OrderView order)
        {
            //Validate IDs and statuses of Sales, Shipments, RecurringPlans, ...
            return true;
        }

        protected virtual bool IsTestOrder(OrderView order)
        {
            if (new SaleService().IsTestCreditCard(order.Billing.CreditCard))
            {
                return true;
            }
            return false;
        }
    }
}

