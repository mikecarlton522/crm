using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using TrimFuel.Model;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Flow
{
    public class RebillOrderFlow : OrderFlow
    {
        protected OrderRecurringPlanView OrderPlan { get; set; }

        //TODO: Move validation and other functionallity to RebillService
        //leave only: public BusinessError<InvoiceView2> ProcessRebill(OrderRecurringPlanView orderPlan, Set<NMICompany, AssertigyMID> mid)
        public BusinessError<InvoiceView2> ProcessRebill(OrderRecurringPlanView orderPlan, MIDBalancing midBalancing, IList<AssertigyMIDAmountView> midListWithDailyRebillRevenue)
        {
            var res = new BusinessError<InvoiceView2>(null, BusinessErrorState.Error, "Unknown error occurred");
            decimal amount = 0M;
            int? merchantAccountID = null;
            try
            {                
                //1. Check if Order should continue
                OrderPlan = orderPlan;
                RecurringPlanCycleView cycle = OrderPlan.GetNextCycle();
                if (cycle == null)
                {
                    res.ErrorMessage = "Order Completed";
                }
                else
                {
                    //2. Check MID
                    OrderSale trial = EnsureLoad<OrderSale>(orderPlan.OrderRecurringPlan.SaleID);
                    Order order = EnsureLoad<Order>(trial.OrderID);
                    Billing billing = EnsureLoad<Billing>(order.BillingID);

                    ChargeHistoryView lastCharge = new OrderService().GetLastCharge(billing.BillingID.Value, order.ProductID.Value);                    
                    if (lastCharge != null)
                    {
                        merchantAccountID = lastCharge.ChargeHistory.MerchantAccountID;
                    }
                    int paymentTypeID = billing.CreditCardCnt.TryGetCardType() ?? PaymentTypeEnum.Visa;
                    amount = (cycle.Constraint != null && 
                              cycle.Constraint.ChargeTypeID == ChargeTypeEnum.Charge
                              ? cycle.Constraint.Amount.Value
                              : 0M);
                    amount -= GetSubscriptionDiscount(orderPlan.OrderRecurringPlan, amount);
                    amount = amount < 0 ? 0 : amount;

                    var mid = midBalancing.ChooseForExisting(merchantAccountID, order.ProductID.Value, paymentTypeID, amount);
                    if (mid.ReturnValue != null)
                    {
                        merchantAccountID = mid.ReturnValue.Value2.AssertigyMIDID;
                    }

                    if (mid.State == BusinessErrorState.Error)
                    {
                        res.ErrorMessage = mid.ErrorMessage;
                    }
                    else
                    {
                        //3. Check 10% daily limit
                        var midRevenue = midListWithDailyRebillRevenue.FirstOrDefault(i => i.MID.AssertigyMIDID == mid.ReturnValue.Value2.AssertigyMIDID);
                        if (midRevenue != null && midRevenue.Amount + amount > 0.1M * mid.ReturnValue.Value2.MonthlyCap)
                        {
                            res.ErrorMessage = "10% daily limit exceeded";
                        }
                        else
                        {
                            OrderView orderView = new OrderView()
                            {
                                Order = order,
                                Billing = billing,
                                SaleList = new List<OrderSaleView>()
                            };

                            //4. Process rebill
                            PaymentFlow = new RebillPaymentFlow(mid.ReturnValue, OrderPlan);
                            var res2 = ProcessOrder(orderView);

                            res.State = res2.State;
                            res.ErrorMessage = res2.ErrorMessage;
                            res.ReturnValue = (res2.ReturnValue != null ? res2.ReturnValue.FirstOrDefault() : null);

                            if (res2.State == BusinessErrorState.Success)
                            {
                                OnRebillSucceeded(res.ReturnValue);
                            }
                            else
                            {
                                OnRebillFailed(res.ReturnValue);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res.ErrorMessage = "Script Error: " + ex.ToString();
            }
            
            if (res.ReturnValue == null)
            {
                SaveQueueNote(res.ErrorMessage, merchantAccountID, amount);                
            }


            OrderPlan = null;

            return res;
        }

        protected void SaveQueueNote(string reason, int? merchantAccountID, decimal amount)
        {
            try
            {
                var note = dao.Load<OrderRecurringPlanQueueNote>(new OrderRecurringPlanQueueNote.ID() { OrderRecirringPlanID = OrderPlan.OrderRecurringPlan.OrderRecurringPlanID.Value });
                if (note == null)
                {
                    note = new OrderRecurringPlanQueueNote();
                }
                note.OrderRecurringPlanID = OrderPlan.OrderRecurringPlan.OrderRecurringPlanID;
                note.Reason = reason;
                note.MerchantAccountID = merchantAccountID;
                note.Amount = amount;
                note.CreateDT = DateTime.Now;
                dao.Save(note);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }            
        }

        override protected IList<InvoiceView> CreateInvoices(OrderView order)
        {
            IList<InvoiceView> res = new List<InvoiceView>();
            Currency currency = new SaleService().GetCurrencyByProduct(order.Order.ProductID.Value);
            int? currencyID = (currency != null ? currency.CurrencyID : null);

            RecurringPlanCycleView cycle = OrderPlan.GetNextCycle();

            //TODO: move CreateRebillSale call to ProcessRebill before ProcessOrder call
            var rebillSale = CreateRebillSale(order, cycle);
            order.SaleList.Add(rebillSale);

            decimal amount = (cycle.Constraint != null && cycle.Constraint.ChargeTypeID == ChargeTypeEnum.Charge ? cycle.Constraint.Amount.Value : 0M);
            decimal authAmount = (cycle.Constraint != null && cycle.Constraint.ChargeTypeID == ChargeTypeEnum.AuthOnly ? cycle.Constraint.Amount.Value : 0M);

            amount = amount - GetSubscriptionDiscount(OrderPlan.OrderRecurringPlan, amount);
            amount = amount < 0 ? 0 : amount;
            authAmount = authAmount - GetSubscriptionDiscount(OrderPlan.OrderRecurringPlan, authAmount);
            authAmount = authAmount < 0 ? 0 : authAmount;
            
            res.Add(new InvoiceView()
            {
                Invoice = new Invoice() { OrderID = order.Order.OrderID, InvoiceStatus = InvoiceStatusEnum.New, Amount = amount, AuthAmount = authAmount, CurrencyID = currencyID },
                Order = order,
                Currency = currency,
                SaleList = new List<OrderSaleView>(){ rebillSale }
            });

            return res;
        }

        protected OrderSaleView CreateRebillSale(OrderView order, RecurringPlanCycleView recurringCycleView)
        {
            OrderSaleView res = null;
            try
            {
                dao.BeginTransaction();

                int recurringCycle = 1;
                if (OrderPlan.SaleList.Count > 0)
                    recurringCycle = OrderPlan.SaleList.Last().RecurringCycle.Value + 1;
                bool isReattempt = false;
                var lastAttempt = new OrderService().GetLastRecurringChargeAttempt(OrderPlan.OrderRecurringPlan.OrderRecurringPlanID.Value);
                if (lastAttempt != null && !lastAttempt.ChargeHistory.Success.Value)
                {
                    isReattempt = true;
                }

                OrderSaleView saleView = new OrderSaleView()
                {
                    Order = order,
                    Invoice = null,
                    OrderSale = new RecurringSale()
                    {
                        InvoiceID = null,
                        OrderID = order.Order.OrderID,
                        SaleType = OrderSaleTypeEnum.Rebill,
                        Quantity = 1,
                        PurePrice = (recurringCycleView.Constraint != null &&
                            recurringCycleView.Constraint.ChargeTypeID == ChargeTypeEnum.Charge
                            ? recurringCycleView.Constraint.Amount
                            : 0M),
                        SaleID = null,
                        SaleName = null,
                        SaleStatus = SaleStatusEnum.New,
                        //RecurringSale fields
                        OrderRecurringPlanID = OrderPlan.OrderRecurringPlan.OrderRecurringPlanID,
                        RecurringCycle = recurringCycle,
                        ReAttempt = isReattempt
                    }
                };

                IList<OrderProductView> orderProductList = new List<OrderProductView>();
                if (recurringCycleView.ShipmentList != null)
                {
                    foreach (var item in recurringCycleView.ShipmentList)
                    {
                        orderProductList.Add(new OrderProductView()
                        {
                            ProductSKU = new ProductSKU()
                            {
                                ProductSKU_ = item.ProductSKU
                            },
                            OrderProduct = new OrderProduct()
                            {
                                OrderProductID = null,
                                SaleID = null,
                                ProductSKU = item.ProductSKU,
                                Quantity = item.Quantity
                            },
                            Sale = saleView
                        });
                    }
                }

                saleView.ProductList = orderProductList;

                saleView.PlanList = new List<OrderRecurringPlan>();

                saleView.OrderSale.OrderID = order.Order.OrderID;
                saleView.OrderSale.CreateDT = DateTime.Now;
                dao.Save((RecurringSale)saleView.OrderSale);

                foreach (OrderProductView product in saleView.ProductList)
                {
                    product.OrderProduct.SaleID = saleView.OrderSale.SaleID;
                    dao.Save(product.OrderProduct);
                }

                res = saleView;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
                res = null;
            }

            return res;
        }

        public override void OnOrderProcessed(OrderView order, IList<InvoiceView2> invoicesProcessed)
        {            
        }

        protected virtual void OnRebillSucceeded(InvoiceView2 invoice)
        {
            try
            {
                RecurringPlanCycleView cycle = OrderPlan.GetNextCycle();
                OrderPlan.AttemptList.Add((RecurringSale)invoice.Invoice.SaleList.First().OrderSale);
                if (OrderPlan.GetNextCycle() == null)
                {
                    OrderPlan.OrderRecurringPlan.RecurringStatus = RecurringStatusEnum.Completed;                    
                }
                else
                {
                    OrderPlan.OrderRecurringPlan.NextCycleDT = DateTime.Now.AddDays(cycle.Cycle.Interim.Value);
                    //OrderPlan.OrderRecurringPlan.NextCycleDT = OrderPlan.OrderRecurringPlan.NextCycleDT.Value.AddDays(cycle.Cycle.Interim.Value);
                    //if (OrderPlan.OrderRecurringPlan.NextCycleDT < DateTime.Today.AddDays(1))
                    //{
                    //    OrderPlan.OrderRecurringPlan.NextCycleDT = DateTime.Now.AddDays(cycle.Cycle.Interim.Value);
                    //}
                }
                dao.Save(OrderPlan.OrderRecurringPlan);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected virtual void OnRebillFailed(InvoiceView2 invoice)
        {
            try
            {
                bool declineByResponse = false;
                if (invoice.ChargeResult != null && invoice.ChargeResult.ChargeHistory != null)
                {
                    string chargeBlockReason = null;
                    declineByResponse = PaymentFlow.CheckStopCharge(invoice.ChargeResult.ChargeHistory.Response, out chargeBlockReason);
                }

                //RecurringPlanCycleView cycle = OrderPlan.GetNextCycle();
                //IList<ChargeHistoryView> recurringChargeHistory = new OrderService().GetRecurringChargeHistory(OrderPlan.OrderRecurringPlan.OrderRecurringPlanID.Value);
                OrderPlan.AttemptList.Add((RecurringSale)invoice.Invoice.SaleList.First().OrderSale);
                //set to Inactive:
                //1. pick up card
                //2. three declined attempts
                if ((declineByResponse) ||
                    (OrderPlan.AttemptList != null &&
                    OrderPlan.AttemptList.Count >= 3 &&
                    OrderPlan.AttemptList.OrderByDescending(i => i.SaleID.Value).Take(3).Sum(i => (i.SaleStatus == SaleStatusEnum.Approved ? 1 : 0)) == 0))
                {
                    OrderPlan.OrderRecurringPlan.RecurringStatus = RecurringStatusEnum.Declined;
                }
                else
                {
                    OrderPlan.OrderRecurringPlan.NextCycleDT = DateTime.Now.AddDays(7);
                }
                dao.Save(OrderPlan.OrderRecurringPlan);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public decimal GetSubscriptionDiscount(OrderRecurringPlan recurringPlan, decimal amount)
        {
            decimal res = 0M;
            try
            {
                if (recurringPlan != null && recurringPlan.DiscountValue != null && recurringPlan.DiscountValue > 0)
                {
                    if (recurringPlan.DiscountTypeID == (int)DiscountType.Discount)
                        res = (amount * recurringPlan.DiscountValue.Value) / 100;
                    if (recurringPlan.DiscountTypeID == (int)DiscountType.FixedPrice)                   
                        res = recurringPlan.DiscountValue.Value;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = 0;
            }

            return Math.Round(res, 2);
        }
    }
}
