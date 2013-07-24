using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Views;
using TrimFuel.Business.Flow;
using System.Web;

namespace TrimFuel.Business
{
    public class SubscriptionPlanService : BaseService
    {
        private SaleService saleService { get { return new SaleService(); } }
        private SubscriptionService subscriptionService { get { return new SubscriptionService(); } }
        private OrderService orderService { get { return new OrderService(); } }

        public SubscriptionPlan UpdateVIPPlan(SubscriptionPlan plan,
            KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemActionView> upsell)
        {
            SubscriptionPlan res = null; ;
            try
            {
                dao.BeginTransaction();

                if (plan == null)
                {
                    throw new ArgumentNullException("plan");
                }

                dao.Save<SubscriptionPlanItem>(upsell.Key);
                SubscriptionPlanItemAction action = new SubscriptionPlanItemAction()
                {
                    SubscriptionPlanItemActionID = upsell.Value.SubscriptionPlanItemActionID,
                    SubscriptionActionTypeID = SubscriptionActionType.Upsell,
                    SubscriptionPlanItemID = upsell.Key.SubscriptionPlanItemID,
                    SubscriptionActionAmount = upsell.Value.SubscriptionActionAmount,
                    SubscriptionActionProductCode = upsell.Value.SubscriptionActionProductCode,
                    SubscriptionActionQuantity = upsell.Value.SubscriptionActionQuantity
                };
                dao.Save<SubscriptionPlanItemAction>(action);
                plan.StartSubscriptionPlanItemID = upsell.Key.SubscriptionPlanItemID;
                dao.Save<SubscriptionPlan>(plan);

                res = plan;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<SubscriptionPlanItem> GetSubscriptionItems(SubscriptionPlan plan)
        {
            IList<SubscriptionPlanItem> res = new List<SubscriptionPlanItem>();
            try
            {

                int? currentItemID = plan.StartSubscriptionPlanItemID;
                while (currentItemID != null && res.Where(i => i.SubscriptionPlanItemID == currentItemID).Count() == 0)
                {
                    SubscriptionPlanItem toAdd = EnsureLoad<SubscriptionPlanItem>(currentItemID);
                    res.Add(toAdd);
                    currentItemID = toAdd.NextSubscriptionPlanItemID;
                }

            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public BillingSubscriptionPlan GetBillingSubscriptionPlanByOrderRecurringPlan(long orderReccuringPlanID)
        {
            BillingSubscriptionPlan res = new BillingSubscriptionPlan();
            try
            {
                MySqlCommand q = new MySqlCommand("Select * from BillingSubscriptionPlan where OrderRecurringPlanID=@OrderRecurringPlanID limit 1");
                q.Parameters.Add("@OrderRecurringPlanID", MySqlDbType.Int64).Value = orderReccuringPlanID;
                res = dao.Load<BillingSubscriptionPlan>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemActionView>? AddRecurringItemVIPPlan(SubscriptionPlan plan,
            KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemActionView> freeProduct)
        {
            KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemActionView>? res = null; ;
            try
            {
                dao.BeginTransaction();

                if (plan == null)
                {
                    throw new ArgumentNullException("plan");
                }

                if (plan.SubscriptionPlanID == null)
                {
                    throw new Exception("SubscriptionPlan has invalid NULL parameter SubscriptionPlanID");
                }

                if (plan.StartSubscriptionPlanItemID == null)
                {
                    throw new Exception(string.Format("SubscriptionPlan({0}) has invalid parameter StartSubscriptionPlanItemID", plan.SubscriptionPlanID));
                }

                if (freeProduct.Key == null)
                {
                    throw new ArgumentNullException("freeProduct.Key");
                }

                if (freeProduct.Value == null)
                {
                    throw new ArgumentNullException("freeProduct.Value");
                }

                if (freeProduct.Key.SubscriptionPlanItemID != null)
                {
                    throw new Exception(string.Format("Can't add existed SubscriptionPlanItem({0})", freeProduct.Key.SubscriptionPlanItemID));
                }

                if (freeProduct.Value.SubscriptionPlanItemActionID != null)
                {
                    throw new Exception(string.Format("Can't add existed SubscriptionPlanItemActionID({0})", freeProduct.Value.SubscriptionPlanItemActionID));
                }

                IList<SubscriptionPlanItem> planItems = GetSubscriptionItems(plan);
                if (planItems.Count == 0)
                {
                    throw new Exception(string.Format("SubscriptionPlan({0}) has no initial SubscriptionPlanItem", plan.SubscriptionPlanID));
                }

                SubscriptionPlanItem lastItem = planItems.Last();
                SubscriptionPlanItem secondItem = (planItems.Count > 1 ? planItems[1] : null);

                if (secondItem != null)
                {
                    freeProduct.Key.NextSubscriptionPlanItemID = secondItem.SubscriptionPlanItemID;
                    dao.Save<SubscriptionPlanItem>(freeProduct.Key);
                }
                else
                {
                    freeProduct.Key.NextSubscriptionPlanItemID = null;
                    dao.Save<SubscriptionPlanItem>(freeProduct.Key);
                    freeProduct.Key.NextSubscriptionPlanItemID = freeProduct.Key.SubscriptionPlanItemID;
                    dao.Save<SubscriptionPlanItem>(freeProduct.Key);
                }
                SubscriptionPlanItemAction action = new SubscriptionPlanItemAction()
                {
                    SubscriptionActionTypeID = SubscriptionActionType.FreeProduct,
                    SubscriptionPlanItemID = freeProduct.Key.SubscriptionPlanItemID,
                    SubscriptionActionAmount = freeProduct.Value.SubscriptionActionAmount,
                    SubscriptionActionProductCode = freeProduct.Value.SubscriptionActionProductCode,
                    SubscriptionActionQuantity = freeProduct.Value.SubscriptionActionQuantity
                };
                dao.Save<SubscriptionPlanItemAction>(action);

                lastItem.NextSubscriptionPlanItemID = freeProduct.Key.SubscriptionPlanItemID;
                dao.Save<SubscriptionPlanItem>(lastItem);

                freeProduct.Value.SubscriptionPlanItemActionID = action.SubscriptionPlanItemActionID;
                res = new KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemActionView>(
                    freeProduct.Key,
                    freeProduct.Value);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemAction>? UpdateRecurringItemVIPPlan(
            KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemActionView> freeProduct)
        {
            KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemAction>? res = null; ;
            try
            {
                dao.BeginTransaction();

                if (freeProduct.Key == null)
                {
                    throw new ArgumentNullException("freeProduct.Key");
                }

                if (freeProduct.Value == null)
                {
                    throw new ArgumentNullException("freeProduct.Value");
                }

                if (freeProduct.Key.SubscriptionPlanItemID == null)
                {
                    throw new ArgumentNullException("freeProduct.Key.SubscriptionPlanItemID");
                }

                if (freeProduct.Value.SubscriptionPlanItemActionID == null)
                {
                    throw new ArgumentNullException("freeProduct.Value.SubscriptionPlanItemActionID");
                }

                SubscriptionPlanItemAction action = dao.Load<SubscriptionPlanItemAction>(freeProduct.Value.SubscriptionPlanItemActionID);
                action.SubscriptionActionAmount = freeProduct.Value.SubscriptionActionAmount;
                action.SubscriptionActionProductCode = freeProduct.Value.SubscriptionActionProductCode;
                action.SubscriptionActionQuantity = freeProduct.Value.SubscriptionActionQuantity;
                dao.Save<SubscriptionPlanItemAction>(action);

                SubscriptionPlanItem item = dao.Load<SubscriptionPlanItem>(freeProduct.Key.SubscriptionPlanItemID);
                item.Interim = freeProduct.Key.Interim;
                dao.Save<SubscriptionPlanItem>(item);

                res = new KeyValuePair<SubscriptionPlanItem, SubscriptionPlanItemAction>(
                    item,
                    action);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<SubscriptionPlan> GetSubscriptionPlans()
        {
            IList<SubscriptionPlan> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select sp.* from SubscriptionPlan sp
                    order by sp.SubscriptionPlanName
                ");
                res = dao.Load<SubscriptionPlan>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<SubscriptionPlanItem> GetSubscriptionPlanItems()
        {
            IList<SubscriptionPlanItem> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select spi.* from SubscriptionPlanItem spi
                ");
                res = dao.Load<SubscriptionPlanItem>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<SubscriptionPlanItemActionView> GetSubscriptionPlanItemActions()
        {
            IList<SubscriptionPlanItemActionView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select a.*, at.SubscriptionActionTypeName, GROUP_CONCAT(concat(convert(pci.Quantity, char), 'x ', i.Product) ORDER BY i.Product ASC SEPARATOR ' + ') as SubscriptionActionProductName from SubscriptionPlanItemAction a
                    inner join SubscriptionActionType at on at.SubscriptionActionTypeID = a.SubscriptionActionTypeID
                    left join ProductCode pc on pc.ProductCode = a.SubscriptionActionProductCode
                    left join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID
                    left join Inventory i on i.InventoryID = pci.InventoryID
                    group by a.SubscriptionPlanItemActionID
                ");
                res = dao.Load<SubscriptionPlanItemActionView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public void ProcessSubscriptionPlans()
        {
            try
            {
                //plans for bs
                MySqlCommand q = new MySqlCommand(@"
                    select bsp.* from BillingSubscriptionPlan bsp
                    inner join BillingSubscription bs on bsp.BillingSubscriptionID = bs.BillingSubscriptionID
                    where bsp.IsActive = 1 and bs.StatusTID = @StatusTID_Active and bsp.NextItemID is not null and Date(bsp.NextItemDate) <= CURDATE()
                ");
                q.Parameters.Add("@StatusTID_Active", MySqlDbType.Int32).Value = TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Active;
                IList<BillingSubscriptionPlan> toProcess = dao.Load<BillingSubscriptionPlan>(q);

                //plans for OrderRecPlan
                q = new MySqlCommand(@"
                    select bsp.* from BillingSubscriptionPlan bsp
                    inner join OrderRecurringPlan orp on orp.OrderRecurringPlanID = bsp.OrderRecurringPlanID
                    where bsp.IsActive = 1 and orp.RecurringStatus = @StatusTID_Active and bsp.NextItemID is not null and Date(bsp.NextItemDate) <= CURDATE()
                ");
                q.Parameters.Add("@StatusTID_Active", MySqlDbType.Int32).Value = TrimFuel.Model.Enums.RecurringStatusEnum.Active;
                IList<BillingSubscriptionPlan> toProcess2 = dao.Load<BillingSubscriptionPlan>(q);

                foreach (var item in toProcess.Union(toProcess2))
                {
                    if (item.IsActive)
                        ProcessSubscriptionPlan(item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public IList<SubscriptionPlanItemAction> GetSubscriptionPlanActions(int subscriptionPlanItemID)
        {
            IList<SubscriptionPlanItemAction> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from SubscriptionPlanItemAction 
                    where SubscriptionPlanItemID = @subscriptionPlanItemID
                ");
                q.Parameters.Add("@subscriptionPlanItemID", MySqlDbType.Int32).Value = subscriptionPlanItemID;

                res = dao.Load<SubscriptionPlanItemAction>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return res;
        }

        private void ProcessSubscriptionPlan(BillingSubscriptionPlan plan)
        {
            try
            {
                //Reload right before processing to avoid double processing
                bool isNewArchitecture = plan.OrderRecurringPlanID != null;
                plan = EnsureLoad<BillingSubscriptionPlan>(plan.BillingSubscriptionPlanID);
                SubscriptionPlanItem nextItem = EnsureLoad<SubscriptionPlanItem>(plan.NextItemID);

                if (plan.NextItemDate.Value.Date > DateTime.Today)
                {
                    throw new Exception(string.Format("Invalid processing Date({0}) for BillingSubscriptionPlan({1})", plan.NextItemDate, plan.BillingSubscriptionPlanID));
                }

                if (isNewArchitecture)
                {
                    //new
                    OrderRecurringPlan orp = EnsureLoad<OrderRecurringPlan>(plan.OrderRecurringPlanID);
                    if (orp.RecurringStatus != TrimFuel.Model.Enums.RecurringStatusEnum.Active)
                    {
                        throw new Exception(string.Format("OrderRecurringPlan is inactive for BillingSubscriptionPlan({0})", plan.BillingSubscriptionPlanID));
                    }
                    foreach (var item in GetSubscriptionPlanActions(plan.NextItemID.Value))
                        ProcessSubscriptionPlanActionNew(orp, item, "Automatic Process", null);
                }
                else
                {
                    //old
                    BillingSubscription bs = EnsureLoad<BillingSubscription>(plan.BillingSubscriptionID);
                    if (bs.StatusTID != TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Active)
                    {
                        throw new Exception(string.Format("BillingSubscription is inactive for BillingSubscriptionPlan({0})", plan.BillingSubscriptionPlanID));
                    }
                    foreach (var item in GetSubscriptionPlanActions(plan.NextItemID.Value))
                        ProcessSubscriptionPlanAction(bs, item);
                }

                FollowNextItem(plan, nextItem);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        #region old architecture

        private BusinessError<SubscriptionPlanItemAction> ProcessSubscriptionPlanAction(BillingSubscription bs, SubscriptionPlanItemAction action)
        {
            BusinessError<SubscriptionPlanItemAction> res =
                new BusinessError<SubscriptionPlanItemAction>(null, BusinessErrorState.Error, string.Empty);

            try
            {
                if (bs == null)
                {
                    throw new ArgumentNullException("plan");
                }
                if (action == null)
                {
                    throw new ArgumentNullException("currentItem");
                }

                if (action.SubscriptionActionTypeID == TrimFuel.Model.Enums.SubscriptionActionType.Upsell)
                {
                    BusinessError<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID>> processRes =
                        ProcessSubscriptionPlanActionUpsell(bs, action);

                    if (processRes.State == BusinessErrorState.Success)
                    {
                        ChargeHistoryEx charge = dao.Load<ChargeHistoryEx>(processRes.ReturnValue.Value1.ChargeHistoryID);

                        res.ErrorMessage = charge.Response;

                        res.State = processRes.State;
                    }
                }
                else if (action.SubscriptionActionTypeID == TrimFuel.Model.Enums.SubscriptionActionType.FreeProduct)
                {
                    if (ProcessSubscriptionPlanActionFreeProduct(bs, action))
                    {
                        res.State = BusinessErrorState.Success;
                    }
                }
                else
                {
                    throw new Exception(string.Format("Undefined SubscriptionAction({0})", action.SubscriptionActionTypeID));
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ErrorMessage = ex.Message;
                res.State = BusinessErrorState.Error;
            }
            return res;
        }

        public BusinessError<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID>> ProcessSubscriptionPlanActionUpsell(BillingSubscription bs, SubscriptionPlanItemAction action)
        {
            BusinessError<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID>> res =
                new BusinessError<Set<UpsellSale, BillingSubscription, Upsell, AssertigyMID>>(null, BusinessErrorState.Error, string.Empty);
            try
            {
                if (action.SubscriptionActionAmount == null || action.SubscriptionActionAmount.Value <= 0)
                {
                    throw new Exception(string.Format("Invalid SubscriptionPlanItemAction Amount({0})", action.SubscriptionActionAmount));
                }
                if (string.IsNullOrEmpty(action.SubscriptionActionProductCode))
                {
                    throw new Exception(string.Format("Invalid SubscriptionPlanItemAction ProductCode({0})", action.SubscriptionActionProductCode));
                }
                if (action.SubscriptionActionQuantity == null || action.SubscriptionActionQuantity.Value <= 0)
                {
                    throw new Exception(string.Format("Invalid SubscriptionPlanItemAction Quantity({0})", action.SubscriptionActionQuantity));
                }

                Billing billing = EnsureLoad<Billing>(bs.BillingID);
                Subscription subscription = EnsureLoad<Subscription>(bs.SubscriptionID);
                UpsellType upType = saleService.GetOrCreateUpsell(subscription.ProductID, action.SubscriptionActionProductCode, (short?)action.SubscriptionActionQuantity.Value, action.SubscriptionActionAmount);
                if (upType == null)
                {
                    throw new Exception(string.Format("Can't create UpsellType for Product({0}), ProductCode({1}), Quantity({2}), Amount({3})", subscription.ProductID, action.SubscriptionActionProductCode, (short?)action.SubscriptionActionQuantity.Value, action.SubscriptionActionAmount));
                }
                else
                {
                    res = saleService.CreateUpsellSale(billing, upType, 1, null, null, null, null, null, null, false);

                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ErrorMessage = ex.Message;
            }
            return res;
        }

        public BusinessError<BillingSubscriptionPlan> SignupUser(long billingID, int subscriptionPlanID)
        {
            BusinessError<BillingSubscriptionPlan> res = new BusinessError<BillingSubscriptionPlan>(null, BusinessErrorState.Error, string.Empty);
            try
            {
                BillingSubscription bs = subscriptionService.GetBillingSubscriptionByBilling(billingID);
                if (bs != null)
                {
                    res = SignupUserAccount(bs.BillingSubscriptionID.Value, subscriptionPlanID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = null;
                res.ErrorMessage = ex.Message;
            }
            return res;
        }

        public BusinessError<BillingSubscriptionPlan> SignupUserAccount(int billingSubscriptionID, int subscriptionPlanID)
        {
            BusinessError<BillingSubscriptionPlan> res = new BusinessError<BillingSubscriptionPlan>(null, BusinessErrorState.Error, string.Empty);
            try
            {
                BillingSubscription bs = EnsureLoad<BillingSubscription>(billingSubscriptionID);
                //Only for Active users
                if (bs.StatusTID == TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Active)
                {
                    SubscriptionPlan plan = EnsureLoad<SubscriptionPlan>(subscriptionPlanID);
                    SubscriptionPlanItem startItem = EnsureLoad<SubscriptionPlanItem>(plan.StartSubscriptionPlanItemID);
                    foreach (var item in GetSubscriptionPlanActions(startItem.SubscriptionPlanItemID.Value))
                    {
                        BusinessError<SubscriptionPlanItemAction> processRes = ProcessSubscriptionPlanAction(bs, item);

                        res.State = processRes.State;
                        res.ErrorMessage = processRes.ErrorMessage;
                    }
                    if (res.State == BusinessErrorState.Success)
                    {
                        BillingSubscriptionPlan newPlan = new BillingSubscriptionPlan();
                        newPlan.CreateDT = DateTime.Now;
                        newPlan.BillingSubscriptionID = bs.BillingSubscriptionID;
                        newPlan.SubscriptionPlanID = subscriptionPlanID;
                        newPlan.IsActive = true;
                        res.ReturnValue = FollowNextItem(newPlan, startItem);
                        res.State = BusinessErrorState.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = null;
                res.ErrorMessage = ex.Message;
                res.State = BusinessErrorState.Error;
            }
            return res;
        }

        private bool ProcessSubscriptionPlanActionFreeProduct(BillingSubscription bs, SubscriptionPlanItemAction action)
        {
            bool res = false;

            try
            {
                if (string.IsNullOrEmpty(action.SubscriptionActionProductCode))
                {
                    throw new Exception(string.Format("Invalid SubscriptionPlanItemAction ProductCode({0})", action.SubscriptionActionProductCode));
                }
                if (action.SubscriptionActionQuantity == null || action.SubscriptionActionQuantity.Value <= 0)
                {
                    throw new Exception(string.Format("Invalid SubscriptionPlanItemAction Quantity({0})", action.SubscriptionActionQuantity));
                }
                Billing billing = EnsureLoad<Billing>(bs.BillingID);
                var res1 = saleService.CreateExtraTrialShipSale(billing, action.SubscriptionActionProductCode, action.SubscriptionActionQuantity.Value);
                if (res1 != null)
                {
                    res = true;
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = false;
            }

            return res;
        }

        #endregion

        #region new architecture

        public BusinessError<BillingSubscriptionPlan> SignupUserNew(long billingID, int subscriptionPlanID, string adminName, int? currentCampaignID)
        {
            BusinessError<BillingSubscriptionPlan> res = new BusinessError<BillingSubscriptionPlan>(null, BusinessErrorState.Error, string.Empty);
            try
            {
                OrderRecurringPlan op = orderService.GetPlanList(billingID).FirstOrDefault(u => u.RecurringStatus == TrimFuel.Model.Enums.RecurringStatusEnum.Active);
                if (op != null)
                {
                    res = SignupUserAccountNew(op, subscriptionPlanID, adminName, currentCampaignID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = null;
                res.ErrorMessage = ex.Message;
            }
            return res;
        }

        public BusinessError<BillingSubscriptionPlan> SignupUserAccountNew(OrderRecurringPlan orderRecurringPlan, int subscriptionPlanID, string adminName, int? currentCampaignID)
        {
            BusinessError<BillingSubscriptionPlan> res = new BusinessError<BillingSubscriptionPlan>(null, BusinessErrorState.Error, string.Empty);
            try
            {
                //Only for Active users
                if (orderRecurringPlan.RecurringStatus == TrimFuel.Model.Enums.RecurringStatusEnum.Active)
                {
                    SubscriptionPlan plan = EnsureLoad<SubscriptionPlan>(subscriptionPlanID);
                    SubscriptionPlanItem startItem = EnsureLoad<SubscriptionPlanItem>(plan.StartSubscriptionPlanItemID);
                    foreach (var item in GetSubscriptionPlanActions(startItem.SubscriptionPlanItemID.Value))
                    {
                        BusinessError<IList<InvoiceView2>> processRes = ProcessSubscriptionPlanActionNew(orderRecurringPlan, item, adminName, currentCampaignID);
                        res.State = processRes.State;
                        res.ErrorMessage = processRes.ErrorMessage;
                    }
                    if (res.State == BusinessErrorState.Success)
                    {
                        BillingSubscriptionPlan newPlan = new BillingSubscriptionPlan();
                        newPlan.CreateDT = DateTime.Now;
                        newPlan.OrderRecurringPlanID = orderRecurringPlan.OrderRecurringPlanID;
                        newPlan.SubscriptionPlanID = subscriptionPlanID;
                        newPlan.IsActive = true;
                        res.ReturnValue = FollowNextItem(newPlan, startItem);
                        res.State = BusinessErrorState.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ReturnValue = null;
                res.ErrorMessage = ex.Message;
                res.State = BusinessErrorState.Error;
            }
            return res;
        }

        private BusinessError<IList<InvoiceView2>> ProcessSubscriptionPlanActionNew(OrderRecurringPlan orderRecurringPlan, SubscriptionPlanItemAction action, string adminName, int? currentCampaignID)
        {
            BusinessError<IList<InvoiceView2>> res = new BusinessError<IList<InvoiceView2>>(null, BusinessErrorState.Error, string.Empty);

            try
            {
                if (orderRecurringPlan == null)
                {
                    throw new ArgumentNullException("plan");
                }
                if (action == null)
                {
                    throw new ArgumentNullException("currentItem");
                }

                if (action.SubscriptionActionTypeID == TrimFuel.Model.Enums.SubscriptionActionType.Upsell ||
                    action.SubscriptionActionTypeID == TrimFuel.Model.Enums.SubscriptionActionType.FreeProduct)
                {
                    var resInvoices = ProcessSubscriptionPlanActionUpsellOrExtraNew(orderRecurringPlan, action, adminName, currentCampaignID);
                    if (resInvoices.State == BusinessErrorState.Success)
                    {
                        res.State = BusinessErrorState.Success;
                        res.ReturnValue = resInvoices.ReturnValue;
                    }
                    else
                    {
                        res.State = resInvoices.State;
                        res.ErrorMessage = resInvoices.ErrorMessage;
                    }
                }
                else
                {
                    throw new Exception(string.Format("Undefined SubscriptionAction({0})", action.SubscriptionActionTypeID));
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ErrorMessage = ex.Message;
                res.State = BusinessErrorState.Error;
            }
            return res;
        }

        public BusinessError<IList<InvoiceView2>> ProcessSubscriptionPlanActionUpsellOrExtraNew(OrderRecurringPlan orderRecurringPlan, SubscriptionPlanItemAction action, string adminName, int? currentCampaignID)
        {
            BusinessError<IList<InvoiceView2>> res =
                new BusinessError<IList<InvoiceView2>>(null, BusinessErrorState.Error, string.Empty);
            try
            {
                if (string.IsNullOrEmpty(action.SubscriptionActionProductCode))
                {
                    throw new Exception(string.Format("Invalid SubscriptionPlanItemAction ProductCode({0})", action.SubscriptionActionProductCode));
                }
                if (action.SubscriptionActionQuantity == null || action.SubscriptionActionQuantity.Value <= 0)
                {
                    throw new Exception(string.Format("Invalid SubscriptionPlanItemAction Quantity({0})", action.SubscriptionActionQuantity));
                }

                var billing = orderService.GetBillingByOrderRecurringPlan(orderRecurringPlan.OrderRecurringPlanID);
                RecurringPlan recuringPlan = EnsureLoad<RecurringPlan>(orderRecurringPlan.RecurringPlanID);

                OrderBuilder orderBuilder = new OrderBuilder();
                orderBuilder.Create(billing.BillingID, billing.CampaignID ?? currentCampaignID, adminName,
                    HttpContext.Current.Request.UserHostAddress, HttpContext.Current.Request.Url.AbsoluteUri, billing.Affiliate, billing.SubAffiliate,
                    recuringPlan.ProductID);
                orderBuilder.AppendProductCode(action.SubscriptionActionProductCode, (short)action.SubscriptionActionQuantity.Value, action.SubscriptionActionAmount ?? 0);

                var isValid = orderBuilder.Validate();
                if (isValid.State == BusinessErrorState.Error)
                {
                    res = new BusinessError<IList<InvoiceView2>>()
                    {
                        ErrorMessage = "Order is not valid",
                        State = BusinessErrorState.Error
                    };
                }
                else
                {
                    var orderRes = orderBuilder.Save();
                    if (orderRes.State == BusinessErrorState.Error)
                        res = new BusinessError<IList<InvoiceView2>>(null, BusinessErrorState.Error, orderRes.ErrorMessage);
                    else
                    {
                        OrderFlow orderFlow = new OrderFlow();
                        res = orderFlow.ProcessOrder(orderRes.ReturnValue);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.ErrorMessage = ex.Message;
            }
            return res;
        }

        #endregion

        private BillingSubscriptionPlan FollowNextItem(BillingSubscriptionPlan plan, SubscriptionPlanItem currentItem)
        {
            BillingSubscriptionPlan res = null;
            try
            {
                dao.BeginTransaction();

                if (plan == null)
                {
                    throw new ArgumentNullException("plan");
                }

                if (currentItem == null)
                {
                    throw new ArgumentNullException("currentItem");
                }

                SubscriptionPlanItem next = null;
                if (currentItem.NextSubscriptionPlanItemID != null)
                {
                    next = EnsureLoad<SubscriptionPlanItem>(currentItem.NextSubscriptionPlanItemID);
                }

                plan.LastItemID = currentItem.SubscriptionPlanItemID;
                plan.LastItemDate = DateTime.Now;
                if (next != null)
                {
                    plan.NextItemID = next.SubscriptionPlanItemID;
                    plan.NextItemDate = DateTime.Now.AddDays(currentItem.Interim.Value);
                }
                else
                {
                    plan.NextItemID = null;
                    plan.NextItemDate = null;
                }

                res = plan;

                dao.Save(plan);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }
    }
}
