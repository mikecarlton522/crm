using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Model.Utility;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Flow;

namespace TrimFuel.Business
{
    public class OrderService : BaseService
    {
        private IList<OrderView>  GenerateOrderList(IEnumerable<Order> orders, IEnumerable<OrderSale> orderSales,
            IEnumerable<OrderProduct> orderProducts, IEnumerable<Shipment> shipments, IEnumerable<ProductSKU> productSKUs,
            IEnumerable<Invoice> invoices, IEnumerable<OrderRecurringPlan> orderRecurringPlans)
        {
            IList<OrderView> ordersView = orders.Select(i => new OrderView() { Order = i }).ToList();
            IList<InvoiceView> invoicesView = invoices.Select(i => new InvoiceView() { Invoice = i }).ToList();
            IList<OrderSaleView> orderSalesView = orderSales.Select(i => new OrderSaleView() { OrderSale = i }).ToList();
            IList<OrderProductView> orderProductsView = orderProducts.Select(i => new OrderProductView() { OrderProduct = i }).ToList();
            IList<ShipmentView> shipmentsView = shipments.Select(i => new ShipmentView() { Shipment = i }).ToList();
            IList<Currency> currencyList = (new DashboardService()).GetCurrencyList();

            foreach (OrderView o in ordersView)
            {
                o.SaleList = orderSalesView.Where(i => i.OrderSale.OrderID == o.Order.OrderID).ToList();
                foreach (OrderSaleView os in o.SaleList)
                    os.Order = o;
                o.InvoiceList = invoicesView.Where(i => i.Invoice.OrderID == o.Order.OrderID).ToList();
                foreach (InvoiceView i in o.InvoiceList)
                    i.Order = o;
            }

            foreach (InvoiceView i in invoicesView)
            {
                i.SaleList = orderSalesView.Where(it => it.OrderSale.InvoiceID == i.Invoice.InvoiceID).ToList();
                foreach (OrderSaleView os in i.SaleList)
                    os.Invoice = i;
            }

            foreach (OrderSaleView os in orderSalesView)
            {
                os.PlanList = orderRecurringPlans.Where(i => i.SaleID == os.OrderSale.SaleID).ToList();
                os.ProductList = orderProductsView.Where(i => i.OrderProduct.SaleID == os.OrderSale.SaleID).ToList();
                foreach (OrderProductView op in os.ProductList)
                    op.Sale = os;
                os.ShipmentList = shipmentsView.Where(i => i.Shipment.SaleID == os.OrderSale.SaleID).ToList();
                foreach (ShipmentView sh in os.ShipmentList)
                    sh.Sale = os;
            }

            foreach (OrderProductView osh in orderProductsView)
            {
                osh.ProductSKU = productSKUs.Where(i => i.ProductSKU_ == osh.OrderProduct.ProductSKU).FirstOrDefault();
            }

            foreach (ShipmentView sh in shipmentsView)
            {
                sh.ProductSKU = productSKUs.Where(i => i.ProductSKU_ == sh.Shipment.ProductSKU).FirstOrDefault();
            }

            foreach (InvoiceView inv in invoicesView)
            {
                if (inv.Invoice.CurrencyID != null)
                {
                    inv.Currency = currencyList.Single(i => i.CurrencyID == inv.Invoice.CurrencyID);
                }
            }

            //return new KeyValuePair<IList<OrderView>,IList<InvoiceView>>(ordersView.ToList(), invoicesView.ToList());
            //return ordersView.ToList();
            return ordersView;
        }

        public IList<OrderView> LoadOrders(long? billingID)
        {
            IList<OrderView> res = new List<OrderView>();
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select o.* from Orders o
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<Order> orders = dao.Load<Order>(q);

                q = new MySqlCommand(@"
                    select os.* from OrderSale os
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<OrderSale> orderSales = dao.Load<OrderSale>(q);

                q = new MySqlCommand(@"
                    select os.*, rs.* from RecurringSale rs
                    inner join OrderSale os on os.SaleID = rs.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<RecurringSale> recurringSales = dao.Load<RecurringSale>(q);
                orderSales = orderSales.Except(recurringSales.Cast<OrderSale>(), new KeyEqualityComparer<OrderSale>(s => s.SaleID.Value)).Union(recurringSales.Cast<OrderSale>()).ToList();
                
                q = new MySqlCommand(@"
                    select op.* from OrderProduct op
                    inner join OrderSale os on os.SaleID = op.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<OrderProduct> orderProducts = dao.Load<OrderProduct>(q);

                q = new MySqlCommand(@"
                    select sh.* from Shipment sh
                    inner join OrderSale os on os.SaleID = sh.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<Shipment> shipments = dao.Load<Shipment>(q);

                q = new MySqlCommand(@"
                    select distinct i.* from Invoice i
                    inner join OrderSale os on os.InvoiceID = i.InvoiceID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<Invoice> invoices = dao.Load<Invoice>(q);

                q = new MySqlCommand(@"
                    select orp.* from OrderRecurringPlan orp
                    inner join OrderSale os on os.SaleID = orp.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<OrderRecurringPlan> orderRecurringPlans = dao.Load<OrderRecurringPlan>(q);

                q = new MySqlCommand(@"
                    select distinct sku.* from ProductSKU sku
                    inner join OrderProduct op on op.ProductSKU = sku.ProductSKU
                    inner join OrderSale os on os.SaleID = op.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<ProductSKU> productSKUs = dao.Load<ProductSKU>(q);

                res = GenerateOrderList(orders, orderSales, orderProducts, shipments, productSKUs, invoices, orderRecurringPlans);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public OrderView LoadOrder(long? orderID)
        {
            OrderView res = null;
            try
            {
                Order order = EnsureLoad<Order>(orderID);                

                MySqlCommand q = new MySqlCommand(@"
                    select os.* from OrderSale os
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.OrderID = @orderID
                ");
                q.Parameters.Add("@orderID", MySqlDbType.Int64).Value = orderID;
                IList<OrderSale> orderSales = dao.Load<OrderSale>(q);

                q = new MySqlCommand(@"
                    select os.*, rs.* from RecurringSale rs
                    inner join OrderSale os on os.SaleID = rs.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.OrderID = @orderID
                ");
                q.Parameters.Add("@orderID", MySqlDbType.Int64).Value = orderID;
                IList<RecurringSale> recurringSales = dao.Load<RecurringSale>(q);
                orderSales = orderSales.Except(recurringSales.Cast<OrderSale>(), new KeyEqualityComparer<OrderSale>(s => s.SaleID.Value)).Union(recurringSales.Cast<OrderSale>()).ToList();

                q = new MySqlCommand(@"
                    select op.* from OrderProduct op
                    inner join OrderSale os on os.SaleID = op.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.OrderID = @orderID
                ");
                q.Parameters.Add("@orderID", MySqlDbType.Int64).Value = orderID;
                IList<OrderProduct> orderProducts = dao.Load<OrderProduct>(q);

                q = new MySqlCommand(@"
                    select sh.* from Shipment sh
                    inner join OrderSale os on os.SaleID = sh.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.OrderID = @orderID
                ");
                q.Parameters.Add("@orderID", MySqlDbType.Int64).Value = orderID;
                IList<Shipment> shipments = dao.Load<Shipment>(q);

                q = new MySqlCommand(@"
                    select distinct i.* from Invoice i
                    inner join OrderSale os on os.InvoiceID = i.InvoiceID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.OrderID = @orderID
                ");
                q.Parameters.Add("@orderID", MySqlDbType.Int64).Value = orderID;
                IList<Invoice> invoices = dao.Load<Invoice>(q);

                q = new MySqlCommand(@"
                    select orp.* from OrderRecurringPlan orp
                    inner join OrderSale os on os.SaleID = orp.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.OrderID = @orderID
                ");
                q.Parameters.Add("@orderID", MySqlDbType.Int64).Value = orderID;
                IList<OrderRecurringPlan> orderRecurringPlans = dao.Load<OrderRecurringPlan>(q);

                q = new MySqlCommand(@"
                    select distinct sku.* from ProductSKU sku
                    inner join OrderProduct op on op.ProductSKU = sku.ProductSKU
                    inner join OrderSale os on os.SaleID = op.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.OrderID = @orderID
                ");
                q.Parameters.Add("@orderID", MySqlDbType.Int64).Value = orderID;
                IList<ProductSKU> productSKUs = dao.Load<ProductSKU>(q);

                res = GenerateOrderList(new List<Order>(){ order }, orderSales, orderProducts, shipments, productSKUs, invoices, orderRecurringPlans)[0];

                res.Billing = EnsureLoad<Billing>(order.BillingID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }
            return res;
        }

        public Billing GetBillingByOrderRecurringPlan(long? orderRecurringPlanID)
        {
            Billing res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select b.* from Billing b
                    inner join Orders o on o.BillingID = b.BillingID
                    inner join OrderSale os on os.OrderID = o.OrderID
                    inner join OrderRecurringPlan orp on orp.SaleID = os.SaleID
                    where orp.OrderRecurringPlanID = @OrderRecurringPlanID
                ");
                q.Parameters.Add("@OrderRecurringPlanID", MySqlDbType.Int64).Value = orderRecurringPlanID;
                res = dao.Load<Billing>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public IList<Shipment> GetSaleShipments(long saleID)
        {
            IList<Shipment> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select sh.* from Shipment sh
                    inner join OrderSale os on os.SaleID = sh.SaleID
                    where os.SaleID = @saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                res = dao.Load<Shipment>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public IList<OrderRecurringPlan> GetPlanListForTrialSale(long saleID)
        {
            IList<OrderRecurringPlan> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select orp.* from OrderRecurringPlan orp
                    inner join OrderSale os on os.SaleID = orp.SaleID
                    where os.SaleID = @saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                res = dao.Load<OrderRecurringPlan>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public OrderSaleView2 LoadSale(long saleID)
        {
            OrderSaleView2 res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select o.* from Orders o
                    inner join OrderSale os on os.OrderID = o.OrderID
                    where os.SaleID = @saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                IList<Order> orders = dao.Load<Order>(q);

                q = new MySqlCommand(@"
                    select os.* from OrderSale os
                    where os.SaleID = @saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                IList<OrderSale> orderSales = dao.Load<OrderSale>(q);

                q = new MySqlCommand(@"
                    select os.*, rs.* from RecurringSale rs
                    inner join OrderSale os on os.SaleID = rs.SaleID
                    where os.SaleID = @saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                IList<RecurringSale> recurringSales = dao.Load<RecurringSale>(q);
                orderSales = orderSales.Except(recurringSales.Cast<OrderSale>(), new KeyEqualityComparer<OrderSale>(s => s.SaleID.Value)).Union(recurringSales.Cast<OrderSale>()).ToList();

                q = new MySqlCommand(@"
                    select op.* from OrderProduct op
                    inner join OrderSale os on os.SaleID = op.SaleID
                    where os.SaleID = @saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                IList<OrderProduct> orderProducts = dao.Load<OrderProduct>(q);

                IList<Shipment> shipments = GetSaleShipments(saleID);

                q = new MySqlCommand(@"
                    select distinct i.* from Invoice i
                    inner join OrderSale os on os.InvoiceID = i.InvoiceID
                    where os.SaleID = @saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                IList<Invoice> invoices = dao.Load<Invoice>(q);

                IList<OrderRecurringPlan> orderRecurringPlans = GetPlanListForTrialSale(saleID);

                q = new MySqlCommand(@"
                    select distinct sku.* from ProductSKU sku
                    inner join OrderProduct op on op.ProductSKU = sku.ProductSKU
                    inner join OrderSale os on os.SaleID = op.SaleID
                    where os.SaleID = @saleID
                ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;
                IList<ProductSKU> productSKUs = dao.Load<ProductSKU>(q);

                var res2 = GenerateOrderList(orders, orderSales, orderProducts, shipments, productSKUs, invoices, orderRecurringPlans);
                res = new OrderSaleView2();
                res.SaleView = res2.SelectMany(i => i.SaleList).Single(i => i.OrderSale.SaleID == saleID);
                if (res.SaleView.OrderSale.InvoiceID != null)
                {
                    res.InvoiceChargeList = GetInvoiceChargeHistory(res.SaleView.OrderSale.InvoiceID.Value);
                    res.SaleRefundList = (new SaleService()).GetSaleRefunds(res.SaleView.OrderSale.SaleID.Value);
                }
                else
                {
                    res.InvoiceChargeList = new List<ChargeHistoryView>();
                    res.SaleRefundList = new List<ChargeHistoryView>();
                }
                if (res.SaleView.Order.Order.CampaignID != null)
                {
                    res.OrderCampaign = EnsureLoad<Campaign>(res.SaleView.Order.Order.CampaignID);
                }
                res.OrderProduct = EnsureLoad<Product>(res.SaleView.Order.Order.ProductID);
                res.Chargeback = (new SaleService()).GetSaleChargeback(saleID);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }
            return res;
        }

        public IList<OrderRecurringPlan> GetPlanList(long billingID)
        {
            IList<OrderRecurringPlan> res = new List<OrderRecurringPlan>();
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select orp.* from OrderRecurringPlan orp
                    inner join OrderSale os on os.SaleID = orp.SaleID
                    inner join Orders o on o.OrderID = os.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                res = dao.Load<OrderRecurringPlan>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public OrderRecurringPlanView GetPlan(long orderRecurringPlanID)
        {
            OrderRecurringPlanView res = null;
            try
            {
                OrderRecurringPlan orderPlan = EnsureLoad<OrderRecurringPlan>(orderRecurringPlanID);
                RecurringPlanView plan = (new SubscriptionNewService()).GetPlan(orderPlan.RecurringPlanID.Value);
                MySqlCommand q = new MySqlCommand(@"
                    select os.*, rs.* from RecurringSale rs
                    inner join OrderSale os on os.SaleID = rs.SaleID
                    where rs.OrderRecurringPlanID = @orderRecurringPlanID
                ");
                q.Parameters.Add("@orderRecurringPlanID", MySqlDbType.Int64).Value = orderRecurringPlanID;
                IList<RecurringSale> recurringSales = dao.Load<RecurringSale>(q);
                res = new OrderRecurringPlanView() { 
                    OrderRecurringPlan = orderPlan,
                    Plan = plan,
                    AttemptList = recurringSales
                };
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public ChargeHistoryView GetLastCharge(long billingID, int productID)
        {
            ChargeHistoryView res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select ch.*, chCur.*, cur.*, authCh.*, am.DisplayName from ChargeHistoryEx ch
                    inner join ChargeHistoryInvoice chInv on chInv.ChargeHistoryID = ch.ChargeHistoryID
                    inner join Invoice inv on inv.InvoiceID = chInv.InvoiceID
                    inner join Orders o on o.OrderID = inv.OrderID
                    left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                    left join Currency cur on cur.CurrencyID = chCur.CurrencyID
                    left join AuthOnlyChargeDetails authCh on authCh.ChargeHistoryID = ch.ChargeHistoryID
                    left join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                    where o.BillingID = @billingID and o.ProductID = @productID and ch.Success = 1 
                    and (ch.ChargeTypeID = @chargeType_Charge or ch.ChargeTypeID = @chargeType_Auth)
                    order by ch.ChargeHistoryID desc
                    limit 1
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@chargeType_Charge", MySqlDbType.Int32).Value = ChargeTypeEnum.Charge;
                q.Parameters.Add("@chargeType_Auth", MySqlDbType.Int32).Value = ChargeTypeEnum.AuthOnly;
                res = dao.Load<ChargeHistoryView>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public ChargeHistoryView GetLastRecurringCharge(long orderRecurringPlanID)
        {
            ChargeHistoryView res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select ch.*, chCur.*, cur.*, authCh.*, am.DisplayName from ChargeHistoryEx ch
                    inner join ChargeHistoryInvoice chInv on chInv.ChargeHistoryID = ch.ChargeHistoryID
                    inner join Invoice inv on inv.InvoiceID = chInv.InvoiceID
                    inner join OrderSale osl on osl.InvoiceID = inv.InvoiceID
                    inner join RecurringSale rsl on rsl.SaleID = osl.SaleID
                    left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                    left join Currency cur on cur.CurrencyID = chCur.CurrencyID
                    left join AuthOnlyChargeDetails authCh on authCh.ChargeHistoryID = ch.ChargeHistoryID
                    left join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                    where rsl.OrderRecurringPlanID = @orderRecurringPlanID and ch.Success = 1 
                    and (ch.ChargeTypeID = @chargeType_Charge or ch.ChargeTypeID = @chargeType_Auth)
                    order by ch.ChargeHistoryID desc
                    limit 1
                ");
                q.Parameters.Add("@orderRecurringPlanID", MySqlDbType.Int64).Value = orderRecurringPlanID;
                q.Parameters.Add("@chargeType_Charge", MySqlDbType.Int32).Value = ChargeTypeEnum.Charge;
                q.Parameters.Add("@chargeType_Auth", MySqlDbType.Int32).Value = ChargeTypeEnum.AuthOnly;
                res = dao.Load<ChargeHistoryView>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public ChargeHistoryView GetLastRecurringChargeAttempt(long orderRecurringPlanID)
        {
            ChargeHistoryView res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select ch.*, chCur.*, cur.*, authCh.*, am.DisplayName from ChargeHistoryEx ch
                    inner join ChargeHistoryInvoice chInv on chInv.ChargeHistoryID = ch.ChargeHistoryID
                    inner join Invoice inv on inv.InvoiceID = chInv.InvoiceID
                    inner join OrderSale osl on osl.InvoiceID = inv.InvoiceID
                    inner join RecurringSale rsl on rsl.SaleID = osl.SaleID
                    left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                    left join Currency cur on cur.CurrencyID = chCur.CurrencyID
                    left join AuthOnlyChargeDetails authCh on authCh.ChargeHistoryID = ch.ChargeHistoryID
                    left join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                    where rsl.OrderRecurringPlanID = @orderRecurringPlanID
                    and (ch.ChargeTypeID = @chargeType_Charge or ch.ChargeTypeID = @chargeType_Auth)
                    order by ch.ChargeHistoryID desc
                    limit 1
                ");
                q.Parameters.Add("@orderRecurringPlanID", MySqlDbType.Int64).Value = orderRecurringPlanID;
                q.Parameters.Add("@chargeType_Charge", MySqlDbType.Int32).Value = ChargeTypeEnum.Charge;
                q.Parameters.Add("@chargeType_Auth", MySqlDbType.Int32).Value = ChargeTypeEnum.AuthOnly;
                res = dao.Load<ChargeHistoryView>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public IList<ChargeHistoryView> GetRecurringChargeHistory(long orderRecurringPlanID)
        {
            IList<ChargeHistoryView> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select ch.*, chCur.*, cur.*, authCh.*, am.DisplayName from ChargeHistoryEx ch
                    inner join ChargeHistoryInvoice chInv on chInv.ChargeHistoryID = ch.ChargeHistoryID
                    inner join Invoice inv on inv.InvoiceID = chInv.InvoiceID
                    inner join OrderSale osl on osl.InvoiceID = inv.InvoiceID
                    inner join RecurringSale rsl on rsl.SaleID = osl.SaleID
                    left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                    left join Currency cur on cur.CurrencyID = chCur.CurrencyID
                    left join AuthOnlyChargeDetails authCh on authCh.ChargeHistoryID = ch.ChargeHistoryID
                    left join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                    where rsl.OrderRecurringPlanID = @orderRecurringPlanID and ch.Success = 1 
                    and (ch.ChargeTypeID = @chargeType_Charge or ch.ChargeTypeID = @chargeType_Auth)
                    order by ch.ChargeHistoryID desc
                ");
                q.Parameters.Add("@orderRecurringPlanID", MySqlDbType.Int64).Value = orderRecurringPlanID;
                q.Parameters.Add("@chargeType_Charge", MySqlDbType.Int32).Value = ChargeTypeEnum.Charge;
                q.Parameters.Add("@chargeType_Auth", MySqlDbType.Int32).Value = ChargeTypeEnum.AuthOnly;
                res = dao.Load<ChargeHistoryView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public ChargeHistoryView GetChargeHistoryView(long chargeHistoryID)
        {
            ChargeHistoryView res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select ch.*, chCur.*, cur.*, authCh.*, am.DisplayName from ChargeHistoryEx ch
                    inner join ChargeHistoryInvoice chInv on chInv.ChargeHistoryID = ch.ChargeHistoryID
                    left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                    left join Currency cur on cur.CurrencyID = chCur.CurrencyID
                    left join AuthOnlyChargeDetails authCh on authCh.ChargeHistoryID = ch.ChargeHistoryID
                    left join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                    where chInv.ChargeHistoryID = @chargeHistoryID
                ");
                q.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = chargeHistoryID;
                res = dao.Load<ChargeHistoryView>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public Invoice GetInvoiceByChargeHistory(long chargeHistoryID)
        {
            Invoice res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select i.* from ChargeHistoryInvoice chi
                    inner join Invoice i on i.InvoiceID = chi.InvoiceID
                    where chi.ChargeHistoryID = @chargeHistoryID
                ");
                q.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = chargeHistoryID;
                res = dao.Load<Invoice>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }
        
        public IList<ChargeHistoryView> GetInvoiceChargeHistory(long invoiceID)
        {
            IList<ChargeHistoryView> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select ch.*, chCur.*, cur.*, authCh.*, am.DisplayName from ChargeHistoryEx ch
                    inner join ChargeHistoryInvoice chInv on chInv.ChargeHistoryID = ch.ChargeHistoryID
                    left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                    left join Currency cur on cur.CurrencyID = chCur.CurrencyID
                    left join AuthOnlyChargeDetails authCh on authCh.ChargeHistoryID = ch.ChargeHistoryID
                    left join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                    where chInv.InvoiceID = @invoiceID
                ");
                q.Parameters.Add("@invoiceID", MySqlDbType.Int64).Value = invoiceID;
                res = dao.Load<ChargeHistoryView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public IList<OrderChargeHistoryView> GetChargeHistory(long billingID)
        {
            IList<OrderChargeHistoryView> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select ch.*, chCur.*, cur.*, authCh.*, am.DisplayName from ChargeHistoryEx ch
                    inner join ChargeHistoryInvoice chInv on chInv.ChargeHistoryID = ch.ChargeHistoryID
                    inner join Invoice i on i.InvoiceID = chInv.InvoiceID
                    inner join Orders o on o.OrderID = i.OrderID
                    left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                    left join Currency cur on cur.CurrencyID = chCur.CurrencyID
                    left join AuthOnlyChargeDetails authCh on authCh.ChargeHistoryID = ch.ChargeHistoryID
                    left join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<ChargeHistoryView> res1 = dao.Load<ChargeHistoryView>(q);

                q = new MySqlCommand(@"
                    select chInv.* from ChargeHistoryInvoice chInv
                    inner join Invoice i on i.InvoiceID = chInv.InvoiceID
                    inner join Orders o on o.OrderID = i.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<ChargeHistoryInvoice> res2 = dao.Load<ChargeHistoryInvoice>(q);

                q = new MySqlCommand(@"
                    select distinct i.* from ChargeHistoryInvoice chInv
                    inner join Invoice i on i.InvoiceID = chInv.InvoiceID
                    inner join Orders o on o.OrderID = i.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<Invoice> res3 = dao.Load<Invoice>(q);

                q = new MySqlCommand(@"
                    select sr.* from SaleRefund sr
                    inner join OrderSale osl on osl.SaleID = sr.SaleID
                    inner join Orders o on o.OrderID = osl.OrderID
                    where o.BillingID = @billingID
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<SaleRefund> res4 = dao.Load<SaleRefund>(q);

                res = res3.Select(i => i.OrderID).Distinct().OrderBy(i => i.Value).Select(i => new OrderChargeHistoryView(){OrderID = i}).ToList();
                foreach (var item in res)
                {
                    item.Charges = (from inv in res3
                                    join chInv in res2 on inv.InvoiceID.Value equals chInv.InvoiceID.Value
                                    join ch in res1 on chInv.ChargeHistoryID.Value equals ch.ChargeHistory.ChargeHistoryID.Value
                                    where inv.OrderID.Value == item.OrderID.Value
                                    orderby ch.ChargeHistory.ChargeHistoryID
                                    select ch).ToList();
                    item.ChargesByInvoices = (from inv in res3
                                              join chInv in res2 on inv.InvoiceID.Value equals chInv.InvoiceID.Value
                                              join ch in item.Charges on chInv.ChargeHistoryID.Value equals ch.ChargeHistory.ChargeHistoryID.Value
                                              orderby inv.InvoiceID
                                              group ch by inv.InvoiceID into chs                                              
                                              select new KeyValuePair<long, IList<ChargeHistoryView>>(chs.Key.Value, chs.ToList())
                                             ).ToDictionary(i => i.Key, i => i.Value);
                    item.RefundsBySales = (from sr in res4
                                           join ch in item.Charges.Where(i => i.ChargeHistory.Success.Value) on sr.ChargeHistoryID.Value equals ch.ChargeHistory.ChargeHistoryID.Value
                                           orderby sr.SaleID
                                           group ch by sr.SaleID into chs                                              
                                           select new KeyValuePair<long, IList<ChargeHistoryView>>(chs.Key.Value, chs.ToList())
                                          ).ToDictionary(i => i.Key, i => i.Value);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public OrderRecurringPlan GetPlanBySale(OrderSale sale)
        {
            OrderRecurringPlan res = null;
            try
            {
                if (sale.SaleType == OrderSaleTypeEnum.Trial)
                {
                    res = GetPlanListForTrialSale(sale.SaleID.Value).FirstOrDefault();
                }
                else if (sale.SaleType == OrderSaleTypeEnum.Rebill)
                {
                    RecurringSale rSale = EnsureLoad<RecurringSale>(sale.SaleID);
                    res = EnsureLoad<OrderRecurringPlan>(rSale.OrderRecurringPlanID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public Product GetDefaultProduct(long billingID)
        {
            Product res = null;

            try
            {
                //try get by active subscription
                MySqlCommand q = new MySqlCommand(@"
                    select p.* from Product p
                    inner join RecurringPlan rp on rp.ProductID = p.ProductID
                    inner join OrderRecurringPlan orp on orp.RecurringPlanID = rp.RecurringPlanID
                    inner join OrderSale sl on sl.SaleID = orp.SaleID
                    inner join Orders o on o.OrderID = sl.OrderID
                    where orp.RecurringStatus = @recurringStatusActive and o.BillingID = @billingID
                    order by orp.StartDT desc
                    limit 1
                ");
                q.Parameters.Add("@recurringStatusActive", MySqlDbType.Int32).Value = RecurringStatusEnum.Active;
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                res = dao.Load<Product>(q).FirstOrDefault();

                if (res == null)
                {
                    //try get by last order
                    q = new MySqlCommand(@"
                        select p.* from Product p
                        inner join Orders o on o.ProductID = p.ProductID
                        where o.BillingID = @billingID
                        order by o.CreateDT desc
                        limit 1
                    ");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                    res = dao.Load<Product>(q).FirstOrDefault();
                }

                if (res == null)
                {
                    //try get by Campaign Recurring Plan
                    q = new MySqlCommand(@"
                        select p.* from Product p
                        inner join RecurringPlan rp on rp.ProductID = p.ProductID
                        inner join CampaignRecurringPlan crp on crp.RecurringPlanID = rp.RecurringPlanID
                        inner join Billing b on b.CampaignID = crp.CampaignID
                        where b.BillingID = @billingID
                        limit 1
                    ");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                    res = dao.Load<Product>(q).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        public void AutoProcessOrders()
        {
            try
            {
                //Just in case 
                //process only orders not older than 2 hours before
                MySqlCommand q = new MySqlCommand(@"
                    select * from OrderAutoProcessing
                    where Completed = 0 and ScheduleDT <= @now and ScheduleDT > @nowMinus2Hours
                ");
                q.Parameters.Add("@now", MySqlDbType.Timestamp).Value = DateTime.Now;
                q.Parameters.Add("@nowMinus2Hours", MySqlDbType.Timestamp).Value = DateTime.Now.AddHours(-2);
                IList<OrderAutoProcessing> orderList = dao.Load<OrderAutoProcessing>(q);
                foreach (var item in orderList)
                {
                    AutoProcessOrder(item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void AutoProcessOrder(OrderAutoProcessing orderToProcess)
        {
            try
            {
                if (orderToProcess.Completed == false)
                {
                    orderToProcess.Completed = true;
                    orderToProcess.CompleteDT = DateTime.Now;
                    dao.Save(orderToProcess);

                    OrderView order = LoadOrder(orderToProcess.OrderID);
                    new OrderFlow().ProcessOrder(order);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public BusinessError<IList<ChargeHistoryView>> UpdateAccount(long billingID, bool scrubOrders, bool blockShipments, bool voidTransactions, int? adminID)
        {
            BusinessError<IList<ChargeHistoryView>> res = new BusinessError<IList<ChargeHistoryView>>(new List<ChargeHistoryView>(), BusinessErrorState.Success, "");
            try
            {
                foreach (var order in LoadOrders(billingID))
                {
                    var res2 = new OrderFlow().UpdateOrder(order, scrubOrders, blockShipments, voidTransactions, adminID);
                    if (res2.ReturnValue != null)
                    {
                        res.ReturnValue = res.ReturnValue.Union(res2.ReturnValue).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public Currency GetCurrencyByOrderReccuringPlan(long orderReccuringPlanID)
        {
            Currency res = null;

            try
            {
                //try get by active subscription
                MySqlCommand q = new MySqlCommand(@"
                    select c.* from Currency c
                    inner join ProductCurrency pc on pc.CurrencyID=c.CurrencyID
                    inner join Product p on p.ProductID=pc.ProductID
                    inner join RecurringPlan rp on rp.ProductID = p.ProductID
                    inner join OrderRecurringPlan orp on orp.RecurringPlanID = rp.RecurringPlanID
                    where orp.OrderRecurringPlanID = @orderReccuringPlanID
                    limit 1
                ");
                q.Parameters.Add("@orderReccuringPlanID", MySqlDbType.Int64).Value = orderReccuringPlanID;
                res = dao.Load<Currency>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }
    }
}
