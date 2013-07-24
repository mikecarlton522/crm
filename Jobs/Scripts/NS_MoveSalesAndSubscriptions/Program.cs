using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Dao.EntityDataProviders;

namespace NS_MoveSalesAndSubscriptions
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));

            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                DateTime? startBillingCreateDate = Utility.TryGetDate(System.Configuration.ConfigurationManager.AppSettings["startBillingCreateDate"]);
                DateTime? endBillingCreateDate = Utility.TryGetDate(System.Configuration.ConfigurationManager.AppSettings["endBillingCreateDate"]);
                int? noCampaignCampaignID = Utility.TryGetInt(System.Configuration.ConfigurationManager.AppSettings["noCampaignCampaignID"]);
                string customBillingList = System.Configuration.ConfigurationManager.AppSettings["customBillingList"];
                string processLastDays = System.Configuration.ConfigurationManager.AppSettings["processLastDays"];
                if (startBillingCreateDate == null || endBillingCreateDate == null)
                {
                    throw new Exception("Invalid startBillingCreateDate or endBillingCreateDate config settings");
                }

                IList<View<long>> billingIDList = null;
                if (!string.IsNullOrEmpty(customBillingList) &&
                    customBillingList.Split(',').Length > 0)
                {
                    billingIDList = new List<View<long>>();
                    foreach (var item in customBillingList.Split(','))
                    {
                        billingIDList.Add(new View<long>() { 
                            Value = Utility.TryGetLong(item)
                        });
                    }
                }

                if (billingIDList == null)
                {
                    if (!string.IsNullOrEmpty(processLastDays) && Utility.TryGetInt(processLastDays) != null)
                    {
                        endBillingCreateDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0);
                        startBillingCreateDate = endBillingCreateDate.Value.AddDays(-Utility.TryGetInt(processLastDays).Value);

                        MySqlCommand q = new MySqlCommand(@"
                            select b.BillingID as Value from Billing b
                            where b.CreateDT >= @startDT and b.CreateDT < @endDT
                            union
                            select bs.BillingID as Value from BillingSubscription bs
                            where bs.CreateDT >= @startDT and bs.CreateDT < @endDT
                            union
                            select u.BillingID as Value from Upsell u
                            where u.CreateDT >= @startDT and u.CreateDT < @endDT
                            union
                            select e.BillingID as Value from ExtraTrialShip e
                            where e.CreateDT >= @startDT and e.CreateDT < @endDT
                            order by `Value`
                        ");
                        q.Parameters.Add("@startDT", MySqlDbType.Timestamp).Value = startBillingCreateDate;
                        q.Parameters.Add("@endDT", MySqlDbType.Timestamp).Value = endBillingCreateDate;
                        billingIDList = dao.Load<View<long>>(q);
                    }
                    else
                    {
                        MySqlCommand q = new MySqlCommand(@"
                            select distinct b.BillingID as Value from Billing b
                            where b.CreateDT >= @startDT and b.CreateDT < @endDT
                            order by b.BillingID
                        ");
                        q.Parameters.Add("@startDT", MySqlDbType.Timestamp).Value = startBillingCreateDate;
                        q.Parameters.Add("@endDT", MySqlDbType.Timestamp).Value = endBillingCreateDate;
                        billingIDList = dao.Load<View<long>>(q);
                    }
                }

                IDictionary<int, int> subscriptionMap = LoadSubscriptionMap(dao, logger);
                if (subscriptionMap == null || subscriptionMap.Count == 0)
                {
                    throw new Exception("Can't load Subscription -> RecurringPlan map");
                }

                IDictionary<string, IDictionary<Inventory, int>> productCodeMap = LoadProductCodeMap(dao, logger);
                if (productCodeMap == null || productCodeMap.Count == 0)
                {
                    throw new Exception("Can't load ProductCode -> Inventory map");
                }

                //Register customer data providers for sales
                new TrimFuel.Business.Dao.EntityDataProviders.OrderSaleDataProvider().UnRegister();
                new TrimFuel.Business.Dao.EntityDataProviders.RecurringSaleDataProvider().UnRegister();
                new RecurringSaleDataProvider().Register();
                new OrderSaleDataProvider().Register();

                logger.Info(string.Format("--------Start processing {0} - {1} accounts: {2} accounts----------------------------------------------", startBillingCreateDate, endBillingCreateDate, billingIDList.Count));
                int processedCount = 0;
                foreach (var billingID in billingIDList)
                {
                    if (ProcessBilling(dao, logger, subscriptionMap, productCodeMap, billingID.Value.Value, noCampaignCampaignID))
                    {
                        processedCount++;
                    }
                }
                logger.Info(string.Format("--------End processing: {0} accounts processed---------------------------------------------------------", processedCount));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        static IDictionary<int, int> LoadSubscriptionMap(IDao dao, ILog logger)
        {
            IDictionary<int, int> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select t1.SubscriptionID, min(t2.RecurringPlanID) as RecurringPlanID from
                    (
                    select 
                        s.SubscriptionID,
                        s.ProductID,
                        (case when s.SecondInterim = s.RegularInterim and s.ShipFirstRebill = 1 and (s.SecondShipping + s.SecondBillAmount = s.RegularShipping + s.RegularBillAmount) then
                            concat('#1r Charge=', 
                                cast(round(s.RegularShipping + s.RegularBillAmount, 2) as char), ' Ship=', 
                                group_concat(concat(cast(pci.Quantity*s.QuantitySKU2 as char), 'x', i.SKU) order by i.SKU, pci.Quantity*s.QuantitySKU2 separator '+'),
                                ' ', cast(s.RegularInterim as char), ' days')
                        else
                            concat(
                                '#1 Charge=', 
                                cast(round(s.SecondShipping + s.SecondBillAmount, 2) as char), ' Ship=', 
			                    (case when s.ShipFirstRebill = 1 then
				                    group_concat(concat(cast(pci.Quantity*s.QuantitySKU2 as char), 'x', i.SKU) order by i.SKU, pci.Quantity*s.QuantitySKU2 separator '+')
			                     else
				                    ''
			                     end),
                                ' ', cast(s.SecondInterim as char), ' days',
                                ' -> #2r Charge=', 
                                cast(round(s.RegularShipping + s.RegularBillAmount, 2) as char), ' Ship=', 
                                group_concat(concat(cast(pci.Quantity*s.QuantitySKU2 as char), 'x', i.SKU) order by i.SKU, pci.Quantity*s.QuantitySKU2 separator '+'),
                                ' ', cast(s.RegularInterim as char), ' days')
                        end) as Cycles
                    from Subscription s
                    inner join ProductCode pc on pc.ProductCode = s.SKU2
                    inner join ProductCodeInventory pci on pci.ProductCodeID = pc.ProductCodeID
                    inner join Inventory i on i.InventoryID = pci.InventoryID
                    where s.Recurring = 1
                    group by s.SubscriptionID
                    ) t1
                    inner join 
                    (
                    select rp.RecurringPlanID, rp.ProductID, 
                    group_concat(concat(
                        '#', cast(c.Cycle as char), 
                        (case when c.Recurring = 1 then 'r ' else ' ' end), 
                        (case when c.ChargeAmount > 0.0 then concat('Charge=', cast(c.ChargeAmount as char)) when c.AuthAmount > 0.0 then concat('Auth=', cast(c.AuthAmount as char)) else 'Charge=0.00' end),
                        ' Ship=', Shipments, ' ',
                        cast(c.Interim as char), ' days'    
                    ) order by c.Cycle separator ' -> ') as Cycles
                    from RecurringPlan rp
                    inner join (
                    select 
                        rpc.RecurringPlanID, rpc.RecurringPlanCycleID, rpc.Interim, rpc.Cycle, rpc.Recurring,
                        (case when coalesce(rpp.ChargeTypeID, 1) = 1 then coalesce(rpp.Amount, 0.0) else 0.0 end) as ChargeAmount,
                        (case when coalesce(rpp.ChargeTypeID, 1) = 4 then coalesce(rpp.Amount, 0.0) else 0.0 end) as AuthAmount,
                        coalesce(group_concat(concat(cast(rpsh.Quantity as char), 'x', rpsh.ProductSKU) order by rpsh.ProductSKU, rpsh.Quantity separator '+'), '') as Shipments
                    from RecurringPlanCycle rpc
                    left join RecurringPlanConstraint rpp on rpp.RecurringPlanCycleID = rpc.RecurringPlanCycleID
                    left join RecurringPlanShipment rpsh on rpsh.RecurringPlanCycleID = rpc.RecurringPlanCycleID
                    group by rpp.RecurringPlanCycleID) c on c.RecurringPlanID = rp.RecurringPlanID
                    group by rp.RecurringPlanID
                    ) t2 on t1.ProductID = t2.ProductID and t1.Cycles = t2.Cycles
                    group by t1.SubscriptionID
                ");
                IList<SubscriptionRecurringPlanView> map = dao.Load<SubscriptionRecurringPlanView>(q);
                res = map.ToDictionary(i => i.SubscriptionID.Value, i => i.RecurringPlanID.Value);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        static IDictionary<string, IDictionary<Inventory, int>> LoadProductCodeMap(IDao dao, ILog logger)
        {
            IDictionary<string, IDictionary<Inventory, int>> res = null;
            try
            {
                var pcList = new InventoryService().GetProductCodeList();
                var invList = new InventoryService().GetInventoryList();
                var pcInvList = new ProductService().GetProductCodeInventoryList();

                res = (from pc in pcList
                       join inv in
                           (
                               from pcInv in pcInvList
                               join inv in invList on pcInv.InventoryID equals inv.InventoryID
                               select new
                               {
                                   Inventory = inv,
                                   ProductCodeInventory = pcInv
                               }) on pc.ProductCodeID equals inv.ProductCodeInventory.ProductCodeID into invGrouping
                       select new
                       {
                           ProductCode = pc,
                           InventoryList = invGrouping.ToDictionary(i => i.Inventory, i => i.ProductCodeInventory.Quantity.Value) as IDictionary<Inventory, int>
                       }).ToDictionary(i => i.ProductCode.ProductCode_, i => i.InventoryList);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        static bool ProcessBilling(IDao dao, ILog logger,
            IDictionary<int, int> subscriptionMap, IDictionary<string, IDictionary<Inventory, int>> productCodeMap,
            long billingID, int? noCampaignCampaignID)
        {
            bool success = false;
            logger.Info(string.Format("BillingID={0}", billingID));

            try
            {
                dao.BeginTransaction();

                Billing billing = dao.Load<Billing>(billingID);
                if (billing.CampaignID == null)
                {
                    billing.CampaignID = noCampaignCampaignID;
                }

                bool isTestCase = new SaleService().IsTestCreditCard(billing.CreditCard);

                List<long> saleIDList = new List<long>();
                List<Sale> fullSaleList = new List<Sale>();

                IList<BillingSubscription> bsList = new SubscriptionService().GetBillingSubscriptionsByBilling(billingID);

                //bsWithSList RecurringPlan = null - not recurring subscription, should be considered as single sale
                IList<Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount>> bsWithSList = new List<Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount>>();                
                foreach (var bs in bsList)
                {
                    Subscription s = dao.Load<Subscription>(bs.SubscriptionID);
                    if (s == null)
                        throw new Exception("Can't find Subscription for BillingSubscription");
                    if (s.Recurring == true && !subscriptionMap.ContainsKey(s.SubscriptionID.Value))
                        throw new Exception("Can't find RecurringPlan for Subscription");
                    if (s.Recurring == true)
                    {
                        RecurringPlan rp = dao.Load<RecurringPlan>(subscriptionMap[s.SubscriptionID.Value]);
                        bsWithSList.Add(new Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount>() { 
                            Value1 = bs,
                            Value2 = s,
                            Value3 = rp,
                            Value4 = null
                        });
                    }
                    else
                    {
                        bsWithSList.Add(new Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount>()
                        {
                            Value1 = bs,
                            Value2 = s,
                            Value3 = null,
                            Value4 = null
                        });
                    }                                        
                }

                MySqlCommand q = q = new MySqlCommand("select bsl.*, sl.* from BillingSale bsl inner join Sale sl on sl.SaleID = bsl.SaleID inner join BillingSubscription bs on bs.BillingSubscriptionID = bsl.BillingSubscriptionID where bs.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<BillingSale> bslList = dao.Load<BillingSale>(q);
                saleIDList.AddRange(bslList.Select(i => i.SaleID.Value));
                fullSaleList.AddRange(bslList.Cast<Sale>());
                IList<BillingSale> noMidBslList = bslList.Where(i => i.ChargeHistoryID == 0).ToList();
                bslList = bslList.Except(noMidBslList).ToList();

                q = new MySqlCommand("select usl.*, sl.* from UpsellSale usl inner join Sale sl on sl.SaleID = usl.SaleID inner join Upsell u on u.UpsellID = usl.UpsellID where u.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<UpsellSale> uslList = dao.Load<UpsellSale>(q);
                saleIDList.AddRange(uslList.Select(i => i.SaleID.Value));
                fullSaleList.AddRange(uslList.Cast<Sale>());

                q = new MySqlCommand("select u.* from Upsell u where u.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<Upsell> upsellList = dao.Load<Upsell>(q);
                IList<Upsell> noMidUpsellList = upsellList.Where(i => uslList.FirstOrDefault(j => j.UpsellID == i.UpsellID) == null).ToList();
                upsellList = upsellList.Except(noMidUpsellList).ToList();

                q = new MySqlCommand("select e.* from ExtraTrialShip e where e.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<ExtraTrialShip> extraTrialShipList = dao.Load<ExtraTrialShip>(q);

                q = new MySqlCommand("select esl.*, sl.* from ExtraTrialShipSale esl inner join Sale sl on sl.SaleID = esl.SaleID inner join ExtraTrialShip e on e.ExtraTrialShipID = esl.ExtraTrialShipID where e.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<ExtraTrialShipSale> eslList = dao.Load<ExtraTrialShipSale>(q);
                saleIDList.AddRange(eslList.Select(i => i.SaleID.Value));
                fullSaleList.AddRange(eslList.Cast<Sale>());

                q = new MySqlCommand("select bsp.* from BillingSubscriptionPlan bsp inner join BillingSubscription bs on bs.BillingSubscriptionID = bsp.BillingSubscriptionID where bs.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<BillingSubscriptionPlan> bsPlanList = dao.Load<BillingSubscriptionPlan>(q);

                q = new MySqlCommand("select bsrb.* from BillingSubscriptionRebillDiscount bsrb inner join BillingSubscription bs on bs.BillingSubscriptionID = bsrb.BillingSubscriptionID where bs.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<BillingSubscriptionRebillDiscount> bsDiscountList = dao.Load<BillingSubscriptionRebillDiscount>(q);
                foreach (var bsDiscount in bsDiscountList)
	            {
		            bsWithSList.First(i => i.Value1.BillingSubscriptionID == bsDiscount.BillingSubscriptionID).Value4 = bsDiscount;
	            }

                q = new MySqlCommand("select bssh.* from BillingSubscriptionStatusHistory bssh inner join BillingSubscription bs on bs.BillingSubscriptionID = bssh.BillingSubscriptionID where bs.BillingID = @billingID");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                IList<BillingSubscriptionStatusHistory> bsStatusHistoryList = dao.Load<BillingSubscriptionStatusHistory>(q);

                q = new MySqlCommand("Select * from EmergencyQueue where BillingID=@BillingID and Completed=0 and CreateDT >= '2012-01-01 00:00:00'");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<EmergencyQueue> emergencyQueueList = dao.Load<EmergencyQueue>(q);

                q = new MySqlCommand("Select i.* from IgnoreUnbilledTransaction i inner join BillingSubscription bs on bs.BillingSubscriptionID = i.BillingSubscriptionID where bs.BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<IgnoreUnbilledTransaction> ignoreBsQueueList = dao.Load<IgnoreUnbilledTransaction>(q);

                q = new MySqlCommand("Select count(*) as Value from AffiliateScrub where BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                bool billingScrub = (dao.Load<View<int>>(q).First().Value.Value > 0) ;

                q = new MySqlCommand("Select max(CreateDT) as Value from ShippingBlocked where BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                var shippingBlockedDateView = dao.Load<View<DateTime>>(q).FirstOrDefault();
                DateTime? shippingBlockedDate = (shippingBlockedDateView != null ? shippingBlockedDateView.Value : null);

                string saleIDs = "(" + string.Join(",", saleIDList.Select(i => i.ToString()).ToArray()) + ")";

                //ABFRecordID = ShipperID to avoid creating of additional view
                IList<ABFRecord> shipperRequestList = new List<ABFRecord>();
                if (saleIDs.Length > 2)
                {
                    q = new MySqlCommand(@"
                    select 2 as ABFRecordID, SaleID, RegID, Request, Response, StatusResponse, Completed, CreateDT, ShippedDT from TSNRecord where SaleID in @saleIDList
                    union all
                    select 4 as ABFRecordID, SaleID, RegID, Request, Response, StatusResponse, Completed, CreateDT, ShippedDT from ABFRecord where SaleID in @saleIDList
                    union all
                    select 5 as ABFRecordID, SaleID, RegID, Request, Response, StatusResponse, Completed, CreateDT, ShippedDT from KeymailRecord where SaleID in @saleIDList
                    union all
                    select 6 as ABFRecordID, SaleID, RegID, Request, Response, StatusResponse, Completed, CreateDT, ShippedDT from ALFRecord where SaleID in @saleIDList
                    union all
                    select 7 as ABFRecordID, SaleID, RegID, Request, Response, StatusResponse, Completed, CreateDT, ShippedDT from GFRecord where SaleID in @saleIDList
                    union all
                    select 8 as ABFRecordID, SaleID, RegID, Request, Response, StatusResponse, Completed, CreateDT, ShippedDT from NPFRecord where SaleID in @saleIDList
                    union all
                    select 9 as ABFRecordID, SaleID, RegID, Request, Response, StatusResponse, Completed, CreateDT, ShippedDT from MBRecord where SaleID in @saleIDList
                    union all
                    select 10 as ABFRecordID, SaleID, RegID, Request, Response, StatusResponse, Completed, CreateDT, ShippedDT from TFRecord where SaleID in @saleIDList
                    union all
                    select 11 as ABFRecordID, SaleID, RegID, Request, Response, StatusResponse, Completed, CreateDT, ShippedDT from CustomShipperRecord where SaleID in @saleIDList
                    ".Replace("@saleIDList", saleIDs));
                    shipperRequestList = dao.Load<ABFRecord>(q);
                }

                IList<SaleRefund> slRefundList = new List<SaleRefund>();
                if (saleIDs.Length > 2)
                {
                    q = new MySqlCommand(@"
                        select * from SaleRefund where SaleID in @saleIDList
                        ".Replace("@saleIDList", saleIDs));
                    slRefundList = dao.Load<SaleRefund>(q);
                }

                IList<ReturnedSale> slReturnList = new List<ReturnedSale>();
                if (saleIDs.Length > 2)
                {
                    q = new MySqlCommand(@"
                        select * from ReturnedSale where SaleID in @saleIDList
                        ".Replace("@saleIDList", saleIDs));
                    slReturnList = dao.Load<ReturnedSale>(q);
                }

                IList<SaleReferer> slRefererList = new List<SaleReferer>();
                if (saleIDs.Length > 2)
                {
                    q = new MySqlCommand(@"
                        select * from SaleReferer where SaleID in @saleIDList
                        ".Replace("@saleIDList", saleIDs));
                    slRefererList = dao.Load<SaleReferer>(q);
                }

                //Charge history
                q = new MySqlCommand("Select cd.*, ch.* from ChargeDetails cd inner join ChargeHistoryEx ch on ch.ChargeHistoryID = cd.ChargeHistoryID inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID where bs.BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<ChargeDetails> chCdList = dao.Load<ChargeDetails>(q);

                q = new MySqlCommand("Select cd.*, ch.* from AuthOnlyChargeDetails cd inner join ChargeHistoryEx ch on ch.ChargeHistoryID = cd.ChargeHistoryID inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID where bs.BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<AuthOnlyChargeDetails> chAauthList = dao.Load<AuthOnlyChargeDetails>(q);

                q = new MySqlCommand("Select cd.* from ChargeHistoryExCurrency cd inner join ChargeHistoryEx ch on ch.ChargeHistoryID = cd.ChargeHistoryID inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID where bs.BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<ChargeHistoryExCurrency> chCurList = dao.Load<ChargeHistoryExCurrency>(q);

                q = new MySqlCommand("Select cd.* from ChargeHistoryExSale cd inner join ChargeHistoryEx ch on ch.ChargeHistoryID = cd.ChargeHistoryID inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID where bs.BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<ChargeHistoryExSale> chSaleList = dao.Load<ChargeHistoryExSale>(q);

                q = new MySqlCommand("Select ch.* from ChargeHistoryEx ch inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID where bs.BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<ChargeHistoryEx> chList = dao.Load<ChargeHistoryEx>(q);

                //Failed charge history
                q = new MySqlCommand("Select cd.*, ch.* from FailedChargeHistoryDetails cd inner join FailedChargeHistory ch on ch.FailedChargeHistoryID = cd.FailedChargeHistoryID where ch.BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<FailedChargeHistoryDetails> fchCdList = dao.Load<FailedChargeHistoryDetails>(q);

                q = new MySqlCommand("Select cd.*, ch.* from AuthOnlyFailedChargeDetails cd inner join FailedChargeHistory ch on ch.FailedChargeHistoryID = cd.FailedChargeHistoryID where ch.BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<AuthOnlyFailedChargeDetails> fchAuthList = dao.Load<AuthOnlyFailedChargeDetails>(q);

                q = new MySqlCommand("Select cd.* from FailedChargeHistoryCurrency cd inner join FailedChargeHistory ch on ch.FailedChargeHistoryID = cd.FailedChargeHistoryID where ch.BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<FailedChargeHistoryCurrency> fchCurList = dao.Load<FailedChargeHistoryCurrency>(q);

                q = new MySqlCommand("Select ch.* from FailedChargeHistory ch where ch.BillingID=@BillingID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                IList<FailedChargeHistory> fchList = dao.Load<FailedChargeHistory>(q);

                IList<OrderView> orderList = new List<OrderView>();
                IList<OrderView> orderQueueList = new List<OrderView>();

                //Convert Emergency queue
                if (emergencyQueueList.Count > 0 && bsWithSList.Count > 0)
                {
                    var bs = bsWithSList.FirstOrDefault(i => !string.IsNullOrEmpty(i.Value2.ProductCode));
                    if (bs == null)
                        bs = bsWithSList.FirstOrDefault();
                    if (bs == null)
                        throw new Exception("BillingSubscription doesn't exist for EmergencyQueue");

                    //Create order
                    var orderDate = emergencyQueueList.First().CreateDT;
                    OrderView queuedOrder = BuildOrder(billing, bs.Value2.ProductID.Value, bs.Value1.StatusTID == BillingSubscriptionStatusEnum.Scrubbed, orderDate.Value, OrderStatusEnum.New);

                    if (chList.Where(i => i.BillingSubscriptionID == bs.Value1.BillingSubscriptionID).Count() > 0)
                    {
                        bs = null; // subscription will be created later
                    }
                    else
                    {
                        //bsList.Remove(bs.Value1); 
                        bsWithSList.Remove(bs); // should not be processed later
                    }

                    var amount = 0M;
                    if (noMidBslList.Count > 0)
                    {
                        amount = emergencyQueueList.Select(u => u.Amount.Value).Min();
                        string productCode = noMidBslList[0].ProductCode;
                        int qty = noMidBslList[0].Quantity ?? 1;
                        if (string.IsNullOrEmpty(productCode) && bs != null)
                        {
                            productCode = bs.Value2.ProductCode;
                            qty = bs.Value2.Quantity.Value;
                        }
                        if (!string.IsNullOrEmpty(productCode))
                        {
                            var queuedSale = BuildSale(null, null, bs, new List<KeyValuePair<string, int>>() { new KeyValuePair<string, int>(productCode, qty) }, amount, orderDate.Value, SaleStatusEnum.New, productCodeMap);
                            if (queuedOrder.SaleList == null)
                                queuedOrder.SaleList = new List<OrderSaleView>();
                            queuedOrder.SaleList.Add(queuedSale);
                        }
                    }
                    amount = emergencyQueueList.Select(u => u.Amount.Value).Sum() - amount;
                    if (amount > 0M && noMidUpsellList.Count > 0)
                    {
                        //Create upsells as one OrderSale
                        var productList = GetUpsellListProducts(noMidUpsellList);
                        var queuedSale = BuildSale(null, null, null, productList, amount, orderDate.Value, SaleStatusEnum.New, productCodeMap);
                        if (queuedOrder.SaleList == null)
                            queuedOrder.SaleList = new List<OrderSaleView>();
                        queuedOrder.SaleList.Add(queuedSale);
                    }
                    if (queuedOrder.SaleList != null && queuedOrder.SaleList.Count > 0)
                    {
                        //Add order to queued orders
                        orderQueueList.Add(queuedOrder);
                    }
                }

                //Create OrderRecurringPlans with Sale and Invoice and find trials for recurring BillingSubscriptions
                //only 1 trial for BillingSubscription (can be BillingSale(trial), UpsellSale or ExtraTrialShipSale)
                //rebill can't be trial
                //if trial not found empty Sale is set as trial (free sign up)
                IDictionary<int, InvoiceView> trials = new Dictionary<int, InvoiceView>();
                IDictionary<InvoiceView, IList<ChargeHistoryEx>> invoiceList = new Dictionary<InvoiceView, IList<ChargeHistoryEx>>();
                IDictionary<InvoiceView, int> invoiceProductList = new Dictionary<InvoiceView, int>();

                foreach (var bs in bsWithSList.Where(i => i.Value3 != null))
                {
                    //find 1st charge:
                    var trialCh = chList.Where(i => i.BillingSubscriptionID == bs.Value1.BillingSubscriptionID && 
                        i.Success == true && i.Amount >= 0M && 
                        (i.ChargeTypeID == ChargeTypeEnum.Charge || i.ChargeTypeID == ChargeTypeEnum.AuthOnly))
                        .OrderBy(i => i.ChargeDate.Value).FirstOrDefault();

                    if (trialCh != null)
                    {
                        IList<BillingSale> chBslList = bslList.Where(i => i.ChargeHistoryID == trialCh.ChargeHistoryID).OrderBy(i => i.CreateDT.Value).ToList();
                        IList<UpsellSale> chUslList = uslList.Where(i => i.ChargeHistoryID == trialCh.ChargeHistoryID).OrderBy(i => i.CreateDT.Value).ToList();
                        //if (chBslList.Count + chUslList.Count == 0)
                        //    throw new Exception(string.Format("Can't find Sales for ChargeHistory({0})", trialCh.ChargeHistoryID));
                        if (chBslList.Count > 0 && chUslList.Count > 0)
                            throw new Exception(string.Format("BillingSale and UpsellSale found for ChargeHistory({0})", trialCh.ChargeHistoryID));
                        if (chBslList.Count > 1)
                            throw new Exception(string.Format("More than 1 BillingSale found for ChargeHistory({0})", trialCh.ChargeHistoryID));

                        var trialChBsl = chBslList.FirstOrDefault();
                        if (trialChBsl != null)
                        {
                            if (trialChBsl.SaleTypeID == SaleTypeEnum.Rebill)
                            {
                                //rebill can't be trial
                                trialCh = null;
                            }
                            else
                            {
                                //general case: 1st charge is trial
                                var invoice = BuildInvoice(trialCh, chCurList, chAauthList, bs.Value2.ProductID.Value,
                                    invoiceList, invoiceProductList);
                                KeyValuePair<string, int> product = GetBillingSaleProduct(trialChBsl, bs, chCdList);
                                BuildSale(trialChBsl.SaleID, invoice, bs, new List<KeyValuePair<string, int>>() { product },
                                    invoice.Invoice.Amount.Value, trialChBsl.CreateDT.Value, SaleStatusEnum.Approved, productCodeMap);
                                trials.Add(bs.Value1.BillingSubscriptionID.Value, invoice);
                                //mark as processed 
                                bslList.Remove(trialChBsl);
                                logger.Info("trial: normal");
                            }
                        }
                        else if (chUslList.Count > 0)
                        {
                            var trialChUpsell = chUslList.First();
                            var firstUpsellDate = trialChUpsell.CreateDT.Value;
                            var bsDate = bs.Value1.CreateDT.Value;
                            //if free sale exists with date between bsDate and firstUpsellDate 
                            //and (free sale date - bsDate < 1 minute) then free sale is trial, upsell is not trial
                            var freeSaleTrial = eslList.Where(i => i.CreateDT.Value >= bsDate && i.CreateDT.Value.AddMinutes(-1) < bsDate && i.CreateDT.Value < firstUpsellDate).FirstOrDefault();
                            if (freeSaleTrial != null)
                            {
                                trialCh = null;
                            }
                            else
                            {
                                //1st UpsellSale of 1st Charge - trial
                                var invoice = BuildInvoice(trialCh, chCurList, chAauthList, bs.Value2.ProductID.Value,
                                    invoiceList, invoiceProductList);
                                BuildSale(trialChUpsell.SaleID, invoice, bs,
                                    GetUpsellSaleListProducts(new List<UpsellSale>() { trialChUpsell }, upsellList),
                                    GetUpsellAmount(trialChUpsell.SaleID.Value, chSaleList, invoice),
                                    trialChUpsell.CreateDT.Value, SaleStatusEnum.Approved, productCodeMap);
                                trials.Add(bs.Value1.BillingSubscriptionID.Value, invoice);
                                //mark as processed 
                                chUslList.Remove(trialChUpsell);
                                uslList.Remove(trialChUpsell);

                                logger.Info("trial: upsell");

                                if (uslList.Count > 0)
                                    logger.Info("WARNING: trial and upsells in one Invoice");

                                foreach (var usl in chUslList)
                                {
                                    BuildSale(usl.SaleID, invoice, null,
                                        GetUpsellSaleListProducts(new List<UpsellSale>() { usl }, upsellList),
                                        GetUpsellAmount(usl.SaleID.Value, chSaleList, invoice),
                                        usl.CreateDT.Value, SaleStatusEnum.Approved, productCodeMap);
                                    uslList.Remove(usl);
                                }
                            }
                        }
                        else
                        {
                            //no sale, trial - charge
                            //if free sale exists with date between bsDate and firstUpsellDate 
                            //and (free sale date - bsDate < 1 minute) then free sale is trial, charge is not trial
                            var bsDate = bs.Value1.CreateDT.Value;
                            var freeSaleTrial = eslList.Where(i => i.CreateDT.Value >= bsDate && i.CreateDT.Value.AddMinutes(-1) < bsDate && i.CreateDT.Value < trialCh.ChargeDate.Value).FirstOrDefault();
                            if (freeSaleTrial != null)
                            {
                                trialCh = null;
                            }
                            else
                            {
                                var invoice = BuildInvoice(trialCh, chCurList, chAauthList, bs.Value2.ProductID.Value,
                                    invoiceList, invoiceProductList);
                                //build empty sale
                                BuildSale(null, invoice, bs, null,
                                    invoice.Invoice.Amount.Value, trialCh.ChargeDate.Value, SaleStatusEnum.Approved, productCodeMap);
                                trials.Add(bs.Value1.BillingSubscriptionID.Value, invoice);
                                logger.Info("trial: pure charge/auth");
                            }
                        }
                    }

                    //if trial was not found
                    if (trialCh == null)
                    {
                        //Try to find trial as free item
                        //if free sale exists with (free sale date - bsDate < 1 minute) then free sale is trial
                        var bsDate = bs.Value1.CreateDT.Value;
                        var freeSaleTrial = eslList.Where(i => i.CreateDT.Value >= bsDate && i.CreateDT.Value.AddMinutes(-1) < bsDate).FirstOrDefault();
                        if (freeSaleTrial != null)
                        {
                            var invoice = BuildFreeInvoice(freeSaleTrial.CreateDT.Value, bs.Value2.ProductID.Value,
                                    invoiceList, invoiceProductList);
                            BuildSale(freeSaleTrial.SaleID, invoice, bs,
                                null, 0M, freeSaleTrial.CreateDT.Value, SaleStatusEnum.Approved, productCodeMap);
                            trials.Add(bs.Value1.BillingSubscriptionID.Value, invoice);
                            //mark as processed 
                            eslList.Remove(freeSaleTrial);
                            logger.Info("trial: free product");
                        }
                        else
                        {
                            //set trial = empty sale
                            var invoice = BuildFreeInvoice(bs.Value1.CreateDT.Value, bs.Value2.ProductID.Value,
                                    invoiceList, invoiceProductList);
                            BuildSale(null, invoice, bs,
                                null, 0M, bs.Value1.CreateDT.Value, SaleStatusEnum.Approved, productCodeMap);
                            trials.Add(bs.Value1.BillingSubscriptionID.Value, invoice);
                            logger.Info("trial: not found, free sign up");
                        }
                    }
                    else
                    {
                        chList.Remove(trialCh);
                    }
                }

                //Convert other Sales, no trials here
                //Process ChargeHistory in order by date
                //1 successful ChargeHistory (Charge or Auth) = 1 Invoice
                //2 failed ChargeHistory (Charge or Auth) during 5 minutes with same amount, same currency, same BS and same SKU go to 1 Invoice

                IDictionary<OrderSaleView, OrderRecurringPlan> rebillPlanMap = new Dictionary<OrderSaleView, OrderRecurringPlan>();

                foreach (var ch in chList.Where(i => i.Amount >= 0M && (i.ChargeTypeID == ChargeTypeEnum.Charge || i.ChargeTypeID == ChargeTypeEnum.AuthOnly)).OrderBy(i => i.ChargeDate.Value).ToList())
                {
                    var bs = bsWithSList.Where(i => i.Value1.BillingSubscriptionID == ch.BillingSubscriptionID).FirstOrDefault();
                    if (ch.Success == true)
                    {
                        //Find sales
                        IList<BillingSale> chBslList = bslList.Where(i => i.ChargeHistoryID == ch.ChargeHistoryID).OrderBy(i => i.CreateDT.Value).ToList();
                        IList<UpsellSale> chUslList = uslList.Where(i => i.ChargeHistoryID == ch.ChargeHistoryID).OrderBy(i => i.CreateDT.Value).ToList();
                        //if (chBslList.Count + chUslList.Count == 0)
                        //    throw new Exception(string.Format("Can't find Sales for ChargeHistory({0})", ch.ChargeHistoryID));
                        if (chBslList.Count > 0 && chUslList.Count > 0)
                            throw new Exception(string.Format("BillingSale and UpsellSale found for ChargeHistory({0})", ch.ChargeHistoryID));
                        if (chBslList.Count > 1)
                            throw new Exception(string.Format("More than 1 BillingSale found for ChargeHistory({0})", ch.ChargeHistoryID));

                        var chBsl = chBslList.FirstOrDefault();
                        if (chBsl != null)
                        {
                            //trial or rebill
                            if (chBsl.SaleTypeID == SaleTypeEnum.Billing)
                            {
                                //just add as single sale
                                var invoice = BuildInvoice(ch, chCurList, chAauthList, bs.Value2.ProductID.Value,
                                    invoiceList, invoiceProductList);
                                KeyValuePair<string, int> product = GetBillingSaleProduct(chBsl, bs, chCdList);
                                BuildSale(chBsl.SaleID, invoice, null, new List<KeyValuePair<string, int>>() { product },
                                    invoice.Invoice.Amount.Value, chBsl.CreateDT.Value, SaleStatusEnum.Approved, productCodeMap);
                                //mark as processed 
                                bslList.Remove(chBsl);

                                //invoiceList.Add(new KeyValuePair<InvoiceView, IList<ChargeHistoryEx>>(invoice, new List<ChargeHistoryEx>() { ch }));
                            }
                            else if (chBsl.SaleTypeID == SaleTypeEnum.Rebill)
                            {
                                //find subscription:
                                var rp = trials[chBsl.BillingSubscriptionID.Value].SaleList[0].PlanList[0];
                                //create invoice for rebill
                                //create sale for rebill
                                var invoice = BuildInvoice(ch, chCurList, chAauthList, bs.Value2.ProductID.Value,
                                    invoiceList, invoiceProductList);
                                KeyValuePair<string, int> product = GetBillingSaleProduct(chBsl, bs, chCdList);
                                BuildRebillSale(chBsl.SaleID, invoice, rp, rebillPlanMap, new List<KeyValuePair<string, int>>() { product },
                                    invoice.Invoice.Amount.Value, chBsl.CreateDT.Value, SaleStatusEnum.Approved, productCodeMap);                                
                                //mark as processed 
                                bslList.Remove(chBsl);

                                //invoiceList.Add(new KeyValuePair<InvoiceView, IList<ChargeHistoryEx>>(invoice, new List<ChargeHistoryEx>() { ch }));
                            }
                        }
                        else if (chUslList.Count > 0)
                        {
                            //upsells
                            var invoice = BuildInvoice(ch, chCurList, chAauthList, bs.Value2.ProductID.Value,
                                    invoiceList, invoiceProductList);
                            foreach (var usl in chUslList)
                            {
                                BuildSale(usl.SaleID, invoice, null,
                                    GetUpsellSaleListProducts(new List<UpsellSale>() { usl }, upsellList),
                                    GetUpsellAmount(usl.SaleID.Value, chSaleList, invoice),
                                    usl.CreateDT.Value, SaleStatusEnum.Approved, productCodeMap);
                                uslList.Remove(usl);
                            }

                            //invoiceList.Add(new KeyValuePair<InvoiceView, IList<ChargeHistoryEx>>(invoice, new List<ChargeHistoryEx>() { ch }));
                        }
                        else
                        {
                            var invoice = BuildInvoice(ch, chCurList, chAauthList, bs.Value2.ProductID.Value,
                                    invoiceList, invoiceProductList);
                            //build empty sale
                            BuildSale(null, invoice, null, null,
                                invoice.Invoice.Amount.Value, ch.ChargeDate.Value, SaleStatusEnum.Approved, productCodeMap);
                        }
                    }
                    else
                    {
                        //Create failed invoice for failed charge
                        var invoice = BuildInvoice(ch, chCurList, chAauthList, bs.Value2.ProductID.Value,
                                    invoiceList, invoiceProductList);
                        //try to determine SKU for it
                        var cd = chCdList.Where(i => i.ChargeHistoryID == ch.ChargeHistoryID).FirstOrDefault();
                        if (cd != null)
                        {
                            IList<KeyValuePair<string, int>> products = new List<KeyValuePair<string, int>>();
                            if (!string.IsNullOrEmpty(cd.SKU))
                                products.Add(new KeyValuePair<string, int>(cd.SKU, 1));
                            if (cd.SaleTypeID == SaleTypeEnum.Billing || cd.SaleTypeID == SaleTypeEnum.Upsell)
                            {
                                BuildSale(null, invoice, null, products, invoice.Invoice.Amount.Value,
                                    ch.ChargeDate.Value, SaleStatusEnum.Declined, productCodeMap);
                            }
                            else if (cd.SaleTypeID == SaleTypeEnum.Rebill)
                            {
                                var rp = trials[ch.BillingSubscriptionID.Value].SaleList[0].PlanList[0];
                                BuildRebillSale(null, invoice, rp, rebillPlanMap, products, invoice.Invoice.Amount.Value,
                                    ch.ChargeDate.Value, SaleStatusEnum.Declined, productCodeMap);
                            }
                            else
                            {
                                cd = null;
                            }
                        }
                        
                        if (cd == null)
                        {
                            //no info about sale, create empty sale
                            BuildSale(null, invoice, null, null, invoice.Invoice.Amount.Value,
                                ch.ChargeDate.Value, SaleStatusEnum.Declined, productCodeMap);
                        }

                        //invoiceList.Add(new KeyValuePair<InvoiceView, IList<ChargeHistoryEx>>(invoice, new List<ChargeHistoryEx>() { ch }));
                    }
                    chList.Remove(ch);
                }

                //refunds and voids
                foreach (var ch in chList.OrderBy(i => i.ChargeDate.Value).ToList())
                {
                    if (ch.ChargeTypeID == ChargeTypeEnum.Charge || ch.ChargeTypeID == ChargeTypeEnum.AuthOnly)
                        throw new Exception(string.Format("Invalid charge type for ChargeHistory({0})", ch.ChargeHistoryID));
                    
                    var bs = bsWithSList.Where(i => i.Value1.BillingSubscriptionID == ch.BillingSubscriptionID).FirstOrDefault();
                    var sr = slRefundList.Where(i => i.ChargeHistoryID == ch.ChargeHistoryID).FirstOrDefault();
                    if (sr != null)
                    {
                        //Sale of refund found
                        var sl = invoiceList.SelectMany(i => i.Key.SaleList).Where(i => i.OrderSale.SaleID == sr.SaleID).FirstOrDefault();
                        if (sl == null)
                            throw new Exception(string.Format("Can't find Sale({0}) for Refund ChargeHistory({1})", sr.SaleID, ch.ChargeHistoryID));
                        invoiceList.Where(i => i.Key.SaleList.Contains(sl)).First().Value.Add(ch);
                    }
                    else
                    {                        
                        ChargeHistoryEx chCharge = null;
                        //try to find by Transaction Number
                        if (!string.IsNullOrEmpty(ch.TransactionNumber))
                        {
                            chCharge = invoiceList.SelectMany(i => i.Value).Where(i => i.TransactionNumber == ch.TransactionNumber).FirstOrDefault();
                        }
                        if (chCharge == null)
                        {
                            //try to find 1st charge in same BS with Amount >= ch.Amount and Date < ch.Date
                            chCharge = invoiceList.SelectMany(i => i.Value).Where(i => 
                                i.BillingSubscriptionID == ch.BillingSubscriptionID &&
                                i.Amount >= ch.Amount &&
                                i.ChargeDate.Value <= ch.ChargeDate.Value &&
                                (i.ChargeTypeID == ChargeTypeEnum.Charge || i.ChargeTypeID == ChargeTypeEnum.AuthOnly) &&
                                i.Success == true).FirstOrDefault();
                        }
                        if (chCharge == null)
                        {
                            //try to find 1st charge in same BS
                            chCharge = invoiceList.SelectMany(i => i.Value).Where(i => 
                                i.BillingSubscriptionID == ch.BillingSubscriptionID).FirstOrDefault();
                        }                            

                        if (chCharge != null)
                        {
                            invoiceList.Where(i => i.Value.Contains(chCharge)).First().Value.Add(ch);
                        }
                        else
                        {
                            throw new Exception(string.Format("Can't find Invoice for ChargeHistory({0})", ch.ChargeHistoryID));
                        }
                    }

                    chList.Remove(ch);
                }

                //Free items
                if (eslList.Count > 0)
                {
                    Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount> defaultSubscription = null;
                    if (billing.CampaignID != null)
                    {
                        Campaign c = dao.Load<Campaign>(billing.CampaignID);
                        Subscription s = dao.Load<Subscription>(c.SubscriptionID);
                        defaultSubscription = new Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount>()
                        {
                            Value2 = s
                        };
                        if (s.Recurring == true)
                        {
                            int rpID = subscriptionMap[s.SubscriptionID.Value];
                            defaultSubscription.Value3 = dao.Load<RecurringPlan>(rpID);
                        }
                    }
                    else
                    {
                        //try to get by existing subscriptions
                        var existing = bsWithSList.FirstOrDefault();
                        if (existing != null)
                        {
                            defaultSubscription = new Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount>()
                            {
                                Value2 = existing.Value2,
                                Value3 = existing.Value3
                            };
                        }
                    }

                    if (defaultSubscription == null)
                        throw new Exception("Can't find ProductID of account");

                    foreach (var esl in eslList.ToList())
                    {
                        var invoice = BuildFreeInvoice(esl.CreateDT.Value, defaultSubscription.Value2.ProductID.Value,
                            invoiceList, invoiceProductList);
                        var e = extraTrialShipList.Where(i => i.ExtraTrialShipID == esl.ExtraTrialShipID).FirstOrDefault();
                        var productList = new List<KeyValuePair<string, int>>() { new KeyValuePair<string, int>(e.ProductCode, e.Quantity.Value) };
                        BuildSale(esl.SaleID, invoice, null, productList, 0M,
                            esl.CreateDT.Value, SaleStatusEnum.Approved, productCodeMap);

                        eslList.Remove(esl);
                    }
                }

                //Process FailedCahrgeHistory
                //FailedCahrgeHistory -> convert to CHargeHistoryEx
                //chListToCreate and chAuthListToCreate should not contain same elements
                IList<Set<ChargeHistoryEx, ChargeHistoryExCurrency, AuthOnlyChargeDetails>> chListToCreate = new List<Set<ChargeHistoryEx, ChargeHistoryExCurrency, AuthOnlyChargeDetails>>();

                if (fchList.Count > 0)
                {
                    Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount> defaultSubscription = null;
                    if (billing.CampaignID != null)
                    {
                        Campaign c = dao.Load<Campaign>(billing.CampaignID);
                        Subscription s = dao.Load<Subscription>(c.SubscriptionID);
                        defaultSubscription = new Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount>()
                        {
                            Value2 = s
                        };
                        if (s.Recurring == true)
                        {
                            int rpID = subscriptionMap[s.SubscriptionID.Value];
                            defaultSubscription.Value3 = dao.Load<RecurringPlan>(rpID);
                        }
                    }
                    else
                    {
                        //try to get by existing subscriptions
                        var existing = bsWithSList.FirstOrDefault();
                        if (existing != null)
                        {
                            defaultSubscription = new Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount>()
                            {
                                Value2 = existing.Value2,
                                Value3 = existing.Value3
                            };
                        }
                    }

                    foreach (var fch in fchList.ToList())
                    {
                        AssertigyMID mid = null;
                        if (fch.MerchantAccountID != null)
                        {
                            mid = dao.Load<AssertigyMID>(fch.MerchantAccountID);
                        }
                        
                        // Create CHargeHistoryEx to be added
                        var ch = BuildChargeHistory(fch, mid, fchCurList, fchAuthList, chListToCreate);

                        //determine subscription that was attempted
                        //if Subscription was not logged - use default subscription
                        var fchCd = fchCdList.Where(i => i.FailedChargeHistoryID == fch.FailedChargeHistoryID).FirstOrDefault();
                        Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount> subscription = defaultSubscription;
                        if (fchCd != null && fchCd.SubscriptionID != null)
                        {
                            Subscription s = dao.Load<Subscription>(fchCd.SubscriptionID);
                            subscription = new Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount>()
                            {
                                Value2 = s
                            };
                            if (s.Recurring == true)
                            {
                                int rpID = subscriptionMap[s.SubscriptionID.Value];
                                subscription.Value3 = dao.Load<RecurringPlan>(rpID);
                            }
                        }

                        if (subscription == null)
                            throw new Exception("Can't find ProductID of account");

                        //determine type of sale
                        //only for trial subscription settings will be applied
                        int saleTypeID = SaleTypeEnum.Upsell;
                        if (fch.SaleTypeID != null)
                            saleTypeID = fch.SaleTypeID.Value;
                        else if (fchCd != null && fchCd.SaleTypeID != null)
                            saleTypeID = fchCd.SaleTypeID.Value;
                        if (saleTypeID != SaleTypeEnum.Billing && saleTypeID != SaleTypeEnum.Upsell)
                            saleTypeID = SaleTypeEnum.Upsell;
                        if (saleTypeID == SaleTypeEnum.Billing && subscription == null)
                            saleTypeID = SaleTypeEnum.Upsell;

                        //determine product code
                        IList<KeyValuePair<string, int>> products = new List<KeyValuePair<string, int>>();

                        var invoice = BuildInvoice(ch, subscription.Value2.ProductID.Value,
                            invoiceList, invoiceProductList);
                        if (saleTypeID == SaleTypeEnum.Billing)
                        {
                            BuildSale(null, invoice, subscription, 
                                products, invoice.Invoice.Amount.Value, 
                                fch.ChargeDate.Value, SaleStatusEnum.Declined, productCodeMap);
                        }
                        else
                        {
                            BuildSale(null, invoice, null,
                                products, invoice.Invoice.Amount.Value,
                                fch.ChargeDate.Value, SaleStatusEnum.Declined, productCodeMap);
                        }

                        fchList.Remove(fch);
                    }
                }                

                //Check if all sales and transactions processed
                if (bslList.Count > 0)
                    throw new Exception("BillingSale exists after account processing");
                if (uslList.Count > 0)
                    throw new Exception("UpsellSale exists after account processing");
                if (eslList.Count > 0)
                    throw new Exception("ExtraTrialShipSale exists after account processing");
                if (chList.Count > 0)
                    throw new Exception("ChargeHistoryEx exists after account processing");
                if (fchList.Count > 0)
                    throw new Exception("FailedChargeHistory exists after account processing");

                //Invoices of rebills go to Order of trial
                //2 Invoices in time frame 5 minutes with same RefererID go to same Order

                IList<KeyValuePair<InvoiceView, DateTime>> orderedInvoiceList = invoiceProductList.Select(i => new KeyValuePair<InvoiceView, DateTime>(i.Key, i.Key.SaleList[0].OrderSale.CreateDT.Value)).OrderBy(i => i.Value).ToList();
                OrderView o = null;
                KeyValuePair<InvoiceView, DateTime>? prevInvoice = null;
                foreach (var inv in orderedInvoiceList)
                {
                    //skip rebills
                    //rebill will be added later
                    if (inv.Key.SaleList[0].OrderSale.SaleType != OrderSaleTypeEnum.Rebill)
                    {
                        bool scrubbed = false;
                        var invChList = invoiceList[inv.Key].Where(i => i.BillingSubscriptionID != null).ToList();
                        if (invChList.Count > 0)
                        {
                            scrubbed = (
                                from ch in invChList
                                join sbs in bsList on ch.BillingSubscriptionID equals sbs.BillingSubscriptionID
                                select sbs).All(i => i.StatusTID == BillingSubscriptionStatusEnum.Scrubbed);
                        }

                        if (o != null && prevInvoice != null &&
                            o.Order.Scrub == scrubbed &&
                            prevInvoice.Value.Value.AddMinutes(5) >= inv.Value &&
                            invoiceProductList[inv.Key] == invoiceProductList[prevInvoice.Value.Key])
                        {
                            //don't create new order
                        }
                        else
                        {
                            //Create new order
                            o = BuildOrder(billing, invoiceProductList[inv.Key], scrubbed, inv.Value, OrderStatusEnum.Active);
                            orderList.Add(o);
                        }

                        prevInvoice = inv;
                        AssignOrder(o, inv.Key);
                    }
                }

                //Assign rebills to their OrderRecurringPlan Orders
                foreach (var rebill in rebillPlanMap)
                {
                    if (rebill.Key.Order != null || rebill.Key.Invoice.Order != null)
                        throw new Exception("Rebill should not be assigned");
                    OrderView rebillOrder = orderList.Where(i => i.GetAllPLans().Contains(rebill.Value)).FirstOrDefault();
                    AssignOrder(rebillOrder, rebill.Key.Invoice);
                }

                //Check if all Invoices are assigned to Orders
                if (invoiceList.Where(i => i.Key.Order == null).Count() > 0)
                    throw new Exception("Not all Invoices assigned");

                //Check BillingScrubbed and set all orders to Scrubbed
                if (billingScrub)
                {
                    foreach (var scrubOrder in orderList)
                    {
                        scrubOrder.Order.Scrub = true;
                    }
                }

                //Check orders and set to "New" status if no Approved sales
                foreach (var newOrder in orderList)
                {
                    if (newOrder.SaleList.All(i => i.OrderSale.SaleStatus != SaleStatusEnum.Approved))
                    {
                        newOrder.Order.OrderStatus = OrderStatusEnum.New;
                    }
                }

                //set Active subscriptions to Inactive for Plans without charges (STO only)
                var fullChList = invoiceList.SelectMany(i => i.Value).ToList();
                foreach (var subscription in bsWithSList.Where(i => i.Value3 != null))
                {
                    //For STO only
                    if (subscription.Value1.NextBillDate.Value < new DateTime(2010, 1, 1) ||
                        fullChList.Where(i => i.Success == true && i.BillingSubscriptionID == subscription.Value1.BillingSubscriptionID).Count() == 0)
                    {
                        var plan = trials[subscription.Value1.BillingSubscriptionID.Value].SaleList[0].PlanList[0];
                        if (plan.RecurringStatus == RecurringStatusEnum.Active)
                        {
                            plan.RecurringStatus = RecurringStatusEnum.Inactive;
                        }
                    }
                }

                //Set referer ID
                foreach (var slReferer in slRefererList)
                {
                    var order = orderList.Where(i => i.SaleList.Where(j => j.OrderSale.SaleID == slReferer.SaleID).Count() > 0).FirstOrDefault();
                    if (order != null)
                    {
                        order.Order.RefererID = slReferer.RefererID;
                    }
                }


                IDictionary<ABFRecord, IList<Shipment>> shipmentRequests = new Dictionary<ABFRecord, IList<Shipment>>();
                IList<KeyValuePair<ShippingNote, IList<Shipment>>> shipmentReturnNotes = new List<KeyValuePair<ShippingNote, IList<Shipment>>>();
                IList<KeyValuePair<ShippingNote, IList<Shipment>>> shipmentBlockedNotes = new List<KeyValuePair<ShippingNote, IList<Shipment>>>();

                var fullOrderSaleList = orderList.SelectMany(i => i.SaleList).ToList();
                foreach (var shRequest in shipperRequestList.OrderBy(i => i.CreateDT.Value))
                {
                    shipmentRequests[shRequest] = new List<Shipment>();

                    var sl = fullOrderSaleList.Where(i => i.OrderSale.SaleID == shRequest.SaleID).FirstOrDefault();
                    foreach (var sh in sl.ShipmentList)
                    {
                        shipmentRequests[shRequest].Add(sh.Shipment);

                        if (shRequest.RegID != null && shRequest.Completed == true)
                        {
                            sh.Shipment.ShipperRegID = shRequest.RegID.ToString();
                            sh.Shipment.ShipperID = (short)shRequest.ABFRecordID.Value;
                            sh.Shipment.ShipmentStatus = ShipmentStatusEnum.Shipped;
                            sh.Shipment.SendDT = shRequest.CreateDT;
                            sh.Shipment.ShipDT = shRequest.ShippedDT ?? shRequest.CreateDT;
                            sh.Shipment.TrackingNumber = fullSaleList.Where(i => i.SaleID == shRequest.SaleID).First().TrackingNumber;
                            if (sh.Shipment.TrackingNumber != null && sh.Shipment.TrackingNumber.Length > 100)
                            {
                                sh.Shipment.TrackingNumber = ExtractTrackingNumber(sh.Shipment.TrackingNumber);
                            }
                        }
                        else if (shRequest.RegID != null)
                        {
                            if (sh.Shipment.ShipmentStatus < ShipmentStatusEnum.Submitted)
                            {
                                sh.Shipment.ShipperRegID = shRequest.RegID.ToString();
                                sh.Shipment.ShipperID = (short)shRequest.ABFRecordID.Value;
                                sh.Shipment.ShipmentStatus = ShipmentStatusEnum.Submitted;
                                sh.Shipment.SendDT = shRequest.CreateDT;
                            }
                            if (sh.Shipment.ShipmentStatus == ShipmentStatusEnum.Submitted)
                            {
                                //check if Sale.TrackingNumber exists and set to shipped
                                var slTrack = fullSaleList.Where(i => i.SaleID == shRequest.SaleID).First().TrackingNumber;
                                if (!string.IsNullOrEmpty(slTrack))
                                {
                                    sh.Shipment.ShipmentStatus = ShipmentStatusEnum.Shipped;
                                    sh.Shipment.ShipDT = sh.Shipment.SendDT;
                                    sh.Shipment.TrackingNumber = slTrack;
                                    if (sh.Shipment.TrackingNumber != null && sh.Shipment.TrackingNumber.Length > 100)
                                    {
                                        sh.Shipment.TrackingNumber = ExtractTrackingNumber(sh.Shipment.TrackingNumber);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (sh.Shipment.ShipmentStatus < ShipmentStatusEnum.Submitted)
                            {
                                sh.Shipment.ShipperID = (short)shRequest.ABFRecordID.Value;
                                sh.Shipment.ShipmentStatus = ShipmentStatusEnum.SubmitError;
                                sh.Shipment.ErrorDT = shRequest.CreateDT;
                            }
                        }
                    }
                }

                foreach (var slReturn in slReturnList)
                {
                    var returnNote = new KeyValuePair<ShippingNote, IList<Shipment>>(
                        new ShippingNote() { 
                            CreateDT = slReturn.ReturnDate,
                            Note = slReturn.Reason,
                            NoteShipmentStatus = ShipmentStatusEnum.Returned
                        }, 
                        new List<Shipment>()
                        );
                    shipmentReturnNotes.Add(returnNote);

                    var sl = fullOrderSaleList.Where(i => i.OrderSale.SaleID == slReturn.SaleID).FirstOrDefault();
                    foreach (var sh in sl.ShipmentList)
                    {
                        returnNote.Value.Add(sh.Shipment);

                        if (sh.Shipment.ShipmentStatus < ShipmentStatusEnum.Submitted && sh.Shipment.SendDT == null)
                        {
                            sh.Shipment.SendDT = slReturn.ReturnDate;
                        }
                        if (sh.Shipment.ShipmentStatus < ShipmentStatusEnum.Shipped && sh.Shipment.SendDT == null)
                        {
                            sh.Shipment.ShipDT = slReturn.ReturnDate;
                        }
                        sh.Shipment.ReturnDT = slReturn.ReturnDate;
                        sh.Shipment.ShipmentStatus = ShipmentStatusEnum.Returned;
                    }
                }

                //Sales that have New status of shipments
                foreach (var sl in orderList
                    .SelectMany(i => i.SaleList)
                    .Where(i => i.OrderSale.SaleID != null)
                    .SelectMany(i => i.ShipmentList)
                    .Where(i => i.Shipment.ShipmentStatus == ShipmentStatusEnum.New || i.Shipment.ShipmentStatus == ShipmentStatusEnum.SubmitError)
                    .Select(i => i.Sale).Distinct())
                {
                    KeyValuePair<ShippingNote, IList<Shipment>>? blockedNote = new KeyValuePair<ShippingNote, IList<Shipment>>(
                            new ShippingNote()
                            {
                                NoteShipmentStatus = ShipmentStatusEnum.Blocked
                            },
                            new List<Shipment>()
                            );

                    decimal saleRefundAmount = 0M;
                    bool refundExists = false;
                    foreach (var slRef in slRefundList.Where(i => i.SaleID == sl.OrderSale.SaleID))
                    {
                        var ch = fullChList.Where(i => i.ChargeHistoryID == slRef.ChargeHistoryID).FirstOrDefault();
                        if (ch.Success == true)
                        {
                            refundExists = true;
                            var chCur = chCurList.Where(i => i.ChargeHistoryID == ch.ChargeHistoryID).FirstOrDefault();
                            if (chCur != null)
                            {
                                saleRefundAmount += Math.Abs(chCur.CurrencyAmount.Value);
                            }
                            else
                            {
                                saleRefundAmount += Math.Abs(ch.Amount.Value);
                            }
                        }
                    }

                    if (shippingBlockedDate != null)
                    {
                        blockedNote.Value.Key.CreateDT = shippingBlockedDate.Value;
                        blockedNote.Value.Key.Note = "Blocked by Admin";
                    }
                    else if (fullSaleList.Where(i => i.SaleID == sl.OrderSale.SaleID).First().NotShip == true)
                    {
                        blockedNote.Value.Key.CreateDT = sl.OrderSale.CreateDT;
                        blockedNote.Value.Key.Note = "Blocked because of \"NotShip\" attribute";
                    }
                    else if (sl.OrderSale.SaleType == OrderSaleTypeEnum.Rebill &&
                        (sl.OrderSale as RecurringSale).RecurringCycle == 1 &&
                        (bsWithSList.Where(i => i.Value3 != null && i.Value3.RecurringPlanID == rebillPlanMap[sl].RecurringPlanID).First().Value2.ShipFirstRebill == false))
                    {
                        blockedNote.Value.Key.CreateDT = sl.OrderSale.CreateDT;
                        blockedNote.Value.Key.Note = "Blocked because of \"Do not ship first rebill\" subscription setting";
                    }
                    else if (refundExists && sl.OrderSale.CreateDT.Value.AddDays(5) < DateTime.Now)
                    {
                        blockedNote.Value.Key.CreateDT = sl.OrderSale.CreateDT;
                        blockedNote.Value.Key.Note = "Blocked because of refund";
                    }
                    else if (refundExists && saleRefundAmount >= sl.OrderSale.ChargedAmount)
                    {
                        blockedNote.Value.Key.CreateDT = sl.OrderSale.CreateDT;
                        blockedNote.Value.Key.Note = "Blocked because of full refund";
                    }
                    else if (sl.OrderSale.ChargedAmount == 0M && bsList.All(i => i.StatusTID != BillingSubscriptionStatusEnum.Active))
                    {
                        blockedNote.Value.Key.CreateDT = sl.OrderSale.CreateDT;
                        blockedNote.Value.Key.Note = "Blocked because of Inactive account";
                    }
                    else
                    {
                        blockedNote = null;
                    }

                    if (blockedNote != null)
                    {
                        foreach (var sh in sl.ShipmentList)
                        {
                            sh.Shipment.ShipmentStatus = ShipmentStatusEnum.Blocked;
                            blockedNote.Value.Value.Add(sh.Shipment);
                        }                        
                        shipmentBlockedNotes.Add(blockedNote.Value);
                    }
                }

                //BEGIN TRANSACTION///////////////////////////////////////////

                //Save FailedChargeHistory as ChargeHistoryEx
                foreach (var fch in chListToCreate)
                {
                    if (fch.Value3 != null)
                    {
                        dao.Save(fch.Value3);
                    }
                    else
                    {
                        dao.Save(fch.Value1);
                        if (fch.Value2 != null)
                        {
                            fch.Value2.ChargeHistoryID = fch.Value1.ChargeHistoryID;
                            dao.Save(fch.Value2);
                        }
                    }
                }

                //Create Orders
                foreach (var order in orderQueueList)
                {
                    SaveOrder(dao, order, rebillPlanMap, invoiceList);
                    OrderQueueNote note = new OrderQueueNote();
                    note.OrderID = order.Order.OrderID;
                    note.Reason = "Can't find acceptable MID to process order";
                    dao.Save(note);
                }
                foreach (var order in orderList)
                {
                    SaveOrder(dao, order, rebillPlanMap, invoiceList);
                }

                //Add shipping notes
                foreach (var note in shipmentRequests)
                {
                    if (note.Value.Count > 0)
                    {
                        ShipperRequest r = new ShipperRequest()
                        {
                                CreateDT = note.Key.CreateDT,
                                Request = "", //dont' save request
                                Response = note.Key.Response,
                                ShipperID = (short)note.Key.ABFRecordID.Value,
                        };
                        if (note.Key.RegID != null)
                        {
                            r.ResponseShipmentStatus = ShipmentStatusEnum.Submitted;
                        }
                        else
                        {
                            r.ResponseShipmentStatus = ShipmentStatusEnum.SubmitError;
                        }

                        dao.Save(r);

                        foreach (var sh in note.Value)
                        {
                            var link = new ShipmentShipperRequest()
                            {
                                ShipmentID = sh.ShipmentID,
                                ShipperRequestID = r.ShipperRequestID
                            };
                            dao.Save(link);
                        }

                        if (note.Key.RegID != null && note.Key.Completed == true)
                        {
                            r = new ShipperRequest()
                            {
                                CreateDT = note.Key.ShippedDT ?? note.Key.CreateDT,
                                Request = "", //dont' save request
                                Response = note.Key.StatusResponse,
                                ShipperID = (short)note.Key.ABFRecordID.Value,
                                ResponseShipmentStatus = ShipmentStatusEnum.Shipped
                            };
                            dao.Save(r);

                            foreach (var sh in note.Value)
                            {
                                var link = new ShipmentShipperRequest()
                                {
                                    ShipmentID = sh.ShipmentID,
                                    ShipperRequestID = r.ShipperRequestID
                                };
                                dao.Save(link);
                            }
                        }
                    }
                }
                //Add shipping return notes
                foreach (var note in shipmentReturnNotes)
                {
                    if (note.Value.Count > 0)
                    {
                        dao.Save(note.Key);

                        foreach (var sh in note.Value)
                        {
                            var link = new ShipmentShippingNote()
                            {
                                ShipmentID = sh.ShipmentID,
                                ShippingNoteID = note.Key.ShippingNoteID
                            };
                            dao.Save(link);
                        }
                    }
                }
                //Add shipping blocked notes
                foreach (var note in shipmentBlockedNotes)
                {
                    if (note.Value.Count > 0)
                    {
                        dao.Save(note.Key);

                        foreach (var sh in note.Value)
                        {
                            var link = new ShipmentShippingNote()
                            {
                                ShipmentID = sh.ShipmentID,
                                ShippingNoteID = note.Key.ShippingNoteID
                            };
                            dao.Save(link);
                        }
                    }
                }

                //Create OrderRecurringPlanHistory
                foreach (var bsStatusHistory in bsStatusHistoryList)
                {
                    if (trials.ContainsKey(bsStatusHistory.BillingSubscriptionID.Value))
                    {
                        var planID = trials[bsStatusHistory.BillingSubscriptionID.Value].SaleList[0].PlanList[0].OrderRecurringPlanID;
                        OrderRecurringPlanStatusHistory planStatusHistory = new OrderRecurringPlanStatusHistory();
                        planStatusHistory.OrderRecurringPlanID = planID;
                        planStatusHistory.CreateDT = bsStatusHistory.CreateDT;
                        planStatusHistory.RecurringStatus = MapRecurringStatus(bsStatusHistory.StatusTID.Value);
                        dao.Save(planStatusHistory);
                    }
                }

                //Update BillingSubscriptionPlan
                foreach (var bsPlan in bsPlanList)
                {
                    if (!trials.ContainsKey(bsPlan.BillingSubscriptionID.Value))
                    {
                        //try to find recurring plan with same status
                        var bs = bsList.Where(i => i.BillingSubscriptionID == bsPlan.BillingSubscriptionID).FirstOrDefault();
                        var moveToRP = trials.Select(i => i.Value).Where(i => i.SaleList[0].PlanList[0].RecurringStatus == MapRecurringStatus(bs.StatusTID.Value)).FirstOrDefault();
                        if (moveToRP == null)
                        {
                            throw new Exception("Can't find OrderRecurringPlan for BillingSubscriptionPlan");
                        }
                        else
                        {
                            bsPlan.OrderRecurringPlanID = moveToRP.SaleList[0].PlanList[0].OrderRecurringPlanID;
                        }
                    }
                    else
                    {
                        bsPlan.OrderRecurringPlanID = trials[bsPlan.BillingSubscriptionID.Value].SaleList[0].PlanList[0].OrderRecurringPlanID;
                    }
                    bsPlan.BillingSubscriptionID = null;
                    dao.Save(bsPlan);
                }

                foreach (var ignoreBsQueue in ignoreBsQueueList)
                {
                    if (!trials.ContainsKey(ignoreBsQueue.BillingSubscriptionID.Value))
                    {
                        ignoreBsQueue.BillingSubscriptionID = null;
                        ignoreBsQueue.BillingID = billing.BillingID;
                    }
                    else
                    {
                        ignoreBsQueue.OrderRecurringPlanID = trials[ignoreBsQueue.BillingSubscriptionID.Value].SaleList[0].PlanList[0].OrderRecurringPlanID;
                        ignoreBsQueue.BillingSubscriptionID = null;
                    }
                    dao.Save(ignoreBsQueue);
                }

                //Leave so far for history
                /*
                */

                //Delete Sale dependencies                
                if (saleIDs.Length > 2)
                {
                    q = new MySqlCommand(@"
                            update Sale set SaleTypeID = 10 where SaleID in @saleIDList;
                            delete from BillingSale where SaleID in @saleIDList;
                            delete from UpsellSale where SaleID in @saleIDList;
                            delete from ExtraTrialShipSale where SaleID in @saleIDList;
                            delete from CustomShipperRecordToSend where SaleID in @saleIDList;
                            delete from KeymailRecordToSend where SaleID in @saleIDList;
                            delete from NPFRecordToSend where SaleID in @saleIDList;
                            delete from ReturnedSale where SaleID in @saleIDList;
                            delete from AggUnsentShipments where SaleID in @saleIDList;

                            delete from ABFRecord where SaleID in @saleIDList;
                            delete from ALFRecord where SaleID in @saleIDList;
                            delete from CustomShipperRecord where SaleID in @saleIDList;
                            delete from KeymailRecord where SaleID in @saleIDList;
                            delete from GFRecord where SaleID in @saleIDList;
                            delete from MBRecord where SaleID in @saleIDList;
                            delete from NPFRecord where SaleID in @saleIDList;
                            delete from TFRecord where SaleID in @saleIDList;
                        ".Replace("@saleIDList", saleIDs));
                    dao.ExecuteNonQuery(q);
                }

                //Delete Billing dependencies
                q = new MySqlCommand(@"
                        delete from EmergencyQueue where BillingID = @billingID;
                        delete authFch from AuthOnlyFailedChargeDetails authFch
                            inner join FailedChargeHistory fch on fch.FailedChargeHistoryID = authFch.FailedChargeHistoryID
                            where fch.BillingID = @billingID;
                        delete fchC from FailedChargeHistoryCurrency fchC
                            inner join FailedChargeHistory fch on fch.FailedChargeHistoryID = fchC.FailedChargeHistoryID
                            where fch.BillingID = @billingID;
                        delete fchD from FailedChargeHistoryDetails fchD
                            inner join FailedChargeHistory fch on fch.FailedChargeHistoryID = fchD.FailedChargeHistoryID
                            where fch.BillingID = @billingID;
                        delete from FailedChargeHistory where BillingID = @billingID;
                        delete from Upsell where BillingID = @billingID;
                        delete from ExtraTrialShip where BillingID = @billingID;
                    ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billingID;
                dao.ExecuteNonQuery(q);

                //Delete BillingSubscription dependencies
                foreach (var bs in bsList)
                {
                    q = new MySqlCommand(@"
                        delete from QueueRebill where BillingSubscriptionID = @billingSubscriptionID;
                        delete from BillingSubscriptionDeclineReason where BillingSubscriptionID = @billingSubscriptionID;
                        delete from BillingSubscriptionDiscount where BillingSubscriptionID = @billingSubscriptionID;
                        delete from BillingSubscriptionRebillDiscount where BillingSubscriptionID = @billingSubscriptionID;
                        delete from BillingSubscriptionStatusHistory where BillingSubscriptionID = @billingSubscriptionID;
                        delete sri from SaleRequestItem sri
                            inner join SaleRequest sr on sr.SaleRequestID = sri.SaleRequestID
                            where sr.AutoProcess = 0 and sr.CreateDT < '2012-03-01 00:00:00' and sr.BillingSubscriptionID = @billingSubscriptionID;
                        delete from SaleRequest
                            where AutoProcess = 0 and CreateDT < '2012-03-01 00:00:00' and BillingSubscriptionID = @billingSubscriptionID;
                        update ChargeHistoryEx set BillingSubscriptionID = null where BillingSubscriptionID = @billingSubscriptionID;
                        delete from BillingSubscription where BillingSubscriptionID = @billingSubscriptionID;
                    ");
                    q.Parameters.Add("@billingSubscriptionID", MySqlDbType.Int32).Value = bs.BillingSubscriptionID;
                    dao.ExecuteNonQuery(q);
                }

                dao.CommitTransaction();                
                success = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
                success = false;                
            }
            if (success)
                logger.Info("SUCCESS");
            else
                logger.Info("ERROR");

            return success;
        }

        static OrderView BuildOrder(Billing billing, int productID, bool scrubbed, DateTime orderDate, int orderStatus)
        {
            OrderView res = new OrderView();
            res.Billing = billing;
            res.Order = new Order();
            res.Order.Affiliate = billing.Affiliate;
            res.Order.BillingID = billing.BillingID;
            res.Order.CampaignID = billing.CampaignID;
            res.Order.CreateDT = orderDate;
            res.Order.IP = billing.IP;
            res.Order.ProductID = productID;
            res.Order.Scrub = scrubbed;
            res.Order.SubAffiliate = billing.SubAffiliate;
            res.Order.URL = billing.URL;
            res.Order.OrderStatus = orderStatus;

            res.SaleList = new List<OrderSaleView>();
            res.InvoiceList = new List<InvoiceView>();

            return res;
        }

        static OrderSaleView BuildSale(long? saleID, InvoiceView invoice, 
            Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount> subscription, 
            IList<KeyValuePair<string, int>> productList, decimal amount, DateTime saleDate, int saleStatus, 
            IDictionary<string, IDictionary<Inventory, int>> productCodeMap)
        {
            OrderSaleView res = new OrderSaleView();
            res.OrderSale = new OrderSale();
            res.OrderSale.SaleID = saleID;
            res.OrderSale.Quantity = 1;
            res.OrderSale.CreateDT = saleDate;
            res.OrderSale.PurePrice = amount;
            res.OrderSale.SaleStatus = saleStatus;
            res.OrderSale.SaleType = (subscription != null && subscription.Value3 != null ? OrderSaleTypeEnum.Trial : OrderSaleTypeEnum.Upsell);

            //order.SaleList.Add(res);
            //res.Order = order;

            if (invoice != null)
            {
                invoice.SaleList.Add(res);
                res.Invoice = invoice;

                res.OrderSale.ProcessDT = invoice.Invoice.ProcessDT;
            }

            res.ProductList = new List<OrderProductView>();
            res.ShipmentList = new List<ShipmentView>();
            res.PlanList = new List<OrderRecurringPlan>();

            if (productList != null)
            {
                foreach (var product in productList)
                {
                    string productCode = product.Key;
                    int quantity = product.Value;

                    IDictionary<Inventory, int> inventoryList = null;
                    if (productCode != "BILL" && productCode != "ILLEGAL_SKU" && productCode != "SKU_ILLEGAL" && !productCodeMap.ContainsKey(productCode))
                    {
                        productCode = productCode.Trim();
                        if (productCode != "BILL" && productCode != "ILLEGAL_SKU" && productCode != "SKU_ILLEGAL" && !productCodeMap.ContainsKey(productCode))
                        {
                            //set fake SKU
                            inventoryList = new Dictionary<Inventory, int>();
                            inventoryList.Add(new Inventory(){ 
                                SKU = "SKU_DELETED",
                                Product = "Deleted inventory",
                                InventoryType = InventoryTypeEnum.Inventory //to mark shipments
                            }, 1);
                            //throw new Exception(string.Format("ProductCode({0}) was not found", productCode));
                        }
                        else
                        {
                            inventoryList = productCodeMap[productCode];
                        }
                    }
                    else if (productCode != "BILL" && productCode != "ILLEGAL_SKU" && productCode != "SKU_ILLEGAL")
                    {
                        inventoryList = productCodeMap[productCode];
                    }

                    //BILL - specific product code that means no shipments and no products, just billing
                    if (productCode != "BILL" && productCode != "ILLEGAL_SKU" && productCode != "SKU_ILLEGAL")
                    {
                        foreach (var p in inventoryList)
                        {
                            OrderProductView op = new OrderProductView();
                            op.Sale = res;
                            res.ProductList.Add(op);
                            op.ProductSKU = new ProductSKU()
                            {
                                ProductSKU_ = p.Key.SKU,
                                ProductName = p.Key.Product
                            };
                            op.OrderProduct = new OrderProduct();
                            op.OrderProduct.ProductSKU = p.Key.SKU;
                            op.OrderProduct.Quantity = p.Value * quantity;

                            if (res.OrderSale.SaleStatus == SaleStatusEnum.Approved && p.Key.InventoryType == InventoryTypeEnum.Inventory)
                            {
                                //Add shipments
                                for (int i = 0; i < op.OrderProduct.Quantity; i++)
                                {
                                    ShipmentView sh = new ShipmentView();
                                    sh.Sale = res;
                                    res.ShipmentList.Add(sh);
                                    sh.ProductSKU = new ProductSKU()
                                    {
                                        ProductSKU_ = p.Key.SKU,
                                        ProductName = p.Key.Product
                                    };
                                    sh.Shipment = new Shipment();
                                    sh.Shipment.CreateDT = res.OrderSale.ProcessDT;
                                    sh.Shipment.ProductSKU = p.Key.SKU;
                                    sh.Shipment.ShipmentStatus = ShipmentStatusEnum.New;
                                }
                            }

                            if (!productCodeMap.ContainsKey(productCode))
                            {
                                res.OrderSale.SaleName = "Deleted product: " + productCode;
                            }
                        }
                    }
                    else if (productCode == "ILLEGAL_SKU")
                    {
                        res.OrderSale.SaleName = "Illegal transaction";
                    }
                    else if (productCode == "SKU_ILLEGAL")
                    {
                        res.OrderSale.SaleName = "Illegal transaction";
                    }
                }
            }

            //Add subscription
            if (res.OrderSale.SaleType == OrderSaleTypeEnum.Trial)
            {
                OrderRecurringPlan orp = new OrderRecurringPlan();
                res.PlanList.Add(orp);                
                if (subscription.Value4 != null)
                {
                    orp.DiscountTypeID = subscription.Value4.DiscountTypeID;
                    orp.DiscountValue = subscription.Value4.DiscountValue;
                }
                orp.RecurringPlanID = subscription.Value3.RecurringPlanID;
                orp.TrialInterim = subscription.Value2.InitialInterim;
                orp.RecurringStatus = (res.OrderSale.SaleStatus == SaleStatusEnum.Approved ? MapRecurringStatus(subscription.Value1.StatusTID.Value) : RecurringStatusEnum.New);
                orp.StartDT = (res.OrderSale.SaleStatus == SaleStatusEnum.Approved ? subscription.Value1.CreateDT : null);
                orp.NextCycleDT = (res.OrderSale.SaleStatus == SaleStatusEnum.Approved ? subscription.Value1.NextBillDate : null);
            }

            return res;
        }

        static OrderSaleView BuildRebillSale(long? saleID, InvoiceView invoice,
            OrderRecurringPlan plan, IDictionary<OrderSaleView, OrderRecurringPlan> rebills,
            IList<KeyValuePair<string, int>> productList, decimal amount, DateTime saleDate, int saleStatus,
            IDictionary<string, IDictionary<Inventory, int>> productCodeMap)
        {
            if (plan.RecurringStatus == RecurringStatusEnum.New)
                throw new Exception("Rebill occurred for subscription with Status = new");

            var prevRebill = rebills.Where(i => i.Value == plan).Select(i => i.Key.OrderSale).Cast<RecurringSale>().OrderBy(i => i.CreateDT.Value).LastOrDefault();

            OrderSaleView res = new OrderSaleView();
            RecurringSale res1 = new RecurringSale();
            res.OrderSale = res1;
            res1.SaleID = saleID;
            res1.Quantity = 1;
            res1.CreateDT = saleDate;
            res1.PurePrice = amount;
            res1.SaleStatus = saleStatus;
            res1.SaleType = OrderSaleTypeEnum.Rebill;
            //res1.OrderSale.OrderRecurringPlanID =  will be set in rebillPlanMap
            rebills.Add(new KeyValuePair<OrderSaleView, OrderRecurringPlan>(res, plan));

            res1.ProcessDT = invoice.Invoice.ProcessDT;
            res1.RecurringCycle = (prevRebill != null ? (prevRebill.SaleStatus == SaleStatusEnum.Approved ? prevRebill.RecurringCycle.Value + 1 : prevRebill.RecurringCycle.Value) : 1);
            res1.ReAttempt = (prevRebill != null && prevRebill.SaleStatus == SaleStatusEnum.Declined);// (saleStatus == SaleStatusEnum.Declined && prevRebill != null && prevRebill.SaleStatus == SaleStatusEnum.Declined);

            invoice.SaleList.Add(res);
            res.Invoice = invoice;

            res.ProductList = new List<OrderProductView>();
            res.ShipmentList = new List<ShipmentView>();
            res.PlanList = new List<OrderRecurringPlan>();

            //order.SaleList.Add(res);
            //res.Order = order;

            if (productList != null)
            {
                foreach (var product in productList)
                {
                    string productCode = product.Key;
                    int quantity = product.Value;

                    IDictionary<Inventory, int> inventoryList = null;
                    if (productCode != "BILL" && productCode != "ILLEGAL_SKU" && productCode != "SKU_ILLEGAL" && !productCodeMap.ContainsKey(productCode))
                    {
                        productCode = productCode.Trim();
                        if (productCode != "BILL" && productCode != "ILLEGAL_SKU" && productCode != "SKU_ILLEGAL" && !productCodeMap.ContainsKey(productCode))
                        {
                            //set fake SKU
                            inventoryList = new Dictionary<Inventory, int>();
                            inventoryList.Add(new Inventory()
                            {
                                SKU = "SKU_DELETED",
                                Product = "Deleted inventory",
                                InventoryType = InventoryTypeEnum.Inventory //to mark shipments
                            }, 1);
                            //throw new Exception(string.Format("ProductCode({0}) was not found", productCode));
                        }
                        else
                        {
                            inventoryList = productCodeMap[productCode];
                        }
                    }
                    else if (productCode != "BILL" && productCode != "ILLEGAL_SKU" && productCode != "SKU_ILLEGAL")
                    {
                        inventoryList = productCodeMap[productCode];
                    }

                    //BILL - specific product code that means no shipments and no products, just billing
                    if (productCode != "BILL" && productCode != "ILLEGAL_SKU" && productCode != "SKU_ILLEGAL")
                    {
                        foreach (var p in inventoryList)
                        {
                            OrderProductView op = new OrderProductView();
                            op.Sale = res;
                            res.ProductList.Add(op);
                            op.ProductSKU = new ProductSKU()
                            {
                                ProductSKU_ = p.Key.SKU,
                                ProductName = p.Key.Product
                            };
                            op.OrderProduct = new OrderProduct();
                            op.OrderProduct.ProductSKU = p.Key.SKU;
                            op.OrderProduct.Quantity = p.Value * quantity;

                            if (res.OrderSale.SaleStatus == SaleStatusEnum.Approved && p.Key.InventoryType == InventoryTypeEnum.Inventory)
                            {
                                //Add shipments
                                for (int i = 0; i < op.OrderProduct.Quantity; i++)
                                {
                                    ShipmentView sh = new ShipmentView();
                                    sh.Sale = res;
                                    res.ShipmentList.Add(sh);
                                    sh.ProductSKU = new ProductSKU()
                                    {
                                        ProductSKU_ = p.Key.SKU,
                                        ProductName = p.Key.Product
                                    };
                                    sh.Shipment = new Shipment();
                                    sh.Shipment.CreateDT = res.OrderSale.ProcessDT;
                                    sh.Shipment.ProductSKU = p.Key.SKU;
                                    sh.Shipment.ShipmentStatus = ShipmentStatusEnum.New;
                                }
                            }

                            if (!productCodeMap.ContainsKey(productCode))
                            {
                                res.OrderSale.SaleName = "Deleted product: " + productCode;
                            }
                        }
                    }
                    else if (productCode == "ILLEGAL_SKU")
                    {
                        res.OrderSale.SaleName = "Illegal transaction";
                    }
                    else if (productCode == "SKU_ILLEGAL")
                    {
                        res.OrderSale.SaleName = "Illegal transaction";
                    }
                }
            }

            return res;
        }

        static InvoiceView BuildInvoice(ChargeHistoryEx ch, IList<ChargeHistoryExCurrency> curList, IList<AuthOnlyChargeDetails> authList, int productID,
            IDictionary<InvoiceView, IList<ChargeHistoryEx>> invoiceList,
            IDictionary<InvoiceView, int> invoiceProductList)
        {
            InvoiceView res = new InvoiceView();
            var cur = curList.Where(i => i.ChargeHistoryID == ch.ChargeHistoryID).FirstOrDefault();
            var auth = authList.Where(i => i.ChargeHistoryID == ch.ChargeHistoryID).FirstOrDefault();
            if (cur != null)
            {
                res.Currency = new Currency()
                {
                    CurrencyID = cur.CurrencyID
                };
            }
            else if (auth != null && auth.RequestedCurrencyID != null)
            {
                res.Currency = new Currency()
                {
                    CurrencyID = auth.RequestedCurrencyID
                };
            }

            res.Invoice = new Invoice();
            res.Invoice.Amount = (cur != null ? cur.CurrencyAmount : ch.Amount);
            res.Invoice.AuthAmount = (auth != null ? (auth.RequestedCurrencyAmount != null ? auth.RequestedCurrencyAmount : auth.RequestedAmount) : 0M);
            res.Invoice.CreateDT = ch.ChargeDate;
            res.Invoice.CurrencyID = (res.Currency != null ? res.Currency.CurrencyID : null);
            res.Invoice.InvoiceStatus = (ch.Success == true ? InvoiceStatusEnum.Paid : InvoiceStatusEnum.New);
            res.Invoice.ProcessDT = ch.ChargeDate;
            res.SaleList = new List<OrderSaleView>();

            invoiceList.Add(new KeyValuePair<InvoiceView, IList<ChargeHistoryEx>>(
                res,
                new List<ChargeHistoryEx>() { ch }));

            invoiceProductList.Add(new KeyValuePair<InvoiceView, int>(
                res,
                productID
                ));

            return res;
        }

        static InvoiceView BuildInvoice(Set<ChargeHistoryEx, ChargeHistoryExCurrency, AuthOnlyChargeDetails> chSet, int productID,
            IDictionary<InvoiceView, IList<ChargeHistoryEx>> invoiceList,
            IDictionary<InvoiceView, int> invoiceProductList)
        {
            InvoiceView res = new InvoiceView();
            var ch = chSet.Value1;
            var cur = chSet.Value2;
            var auth = chSet.Value3;
            if (cur != null)
            {
                res.Currency = new Currency()
                {
                    CurrencyID = cur.CurrencyID
                };
            }
            else if (auth != null && auth.RequestedCurrencyID != null)
            {
                res.Currency = new Currency()
                {
                    CurrencyID = auth.RequestedCurrencyID
                };
            }

            res.Invoice = new Invoice();
            res.Invoice.Amount = (cur != null ? cur.CurrencyAmount : ch.Amount);
            res.Invoice.AuthAmount = (auth != null ? (auth.RequestedCurrencyAmount != null ? auth.RequestedCurrencyAmount : auth.RequestedAmount) : 0M);
            res.Invoice.CreateDT = ch.ChargeDate;
            res.Invoice.CurrencyID = (res.Currency != null ? res.Currency.CurrencyID : null);
            res.Invoice.InvoiceStatus = (ch.Success == true ? InvoiceStatusEnum.Paid : InvoiceStatusEnum.New);
            res.Invoice.ProcessDT = ch.ChargeDate;
            res.SaleList = new List<OrderSaleView>();

            invoiceList.Add(new KeyValuePair<InvoiceView, IList<ChargeHistoryEx>>(
                res,
                new List<ChargeHistoryEx>() { ch }));

            invoiceProductList.Add(new KeyValuePair<InvoiceView, int>(
                res,
                productID
                ));

            return res;
        }

        static InvoiceView BuildFreeInvoice(DateTime saleDate, int productID,
            IDictionary<InvoiceView, IList<ChargeHistoryEx>> invoiceList,
            IDictionary<InvoiceView, int> invoiceProductList)
        {
            InvoiceView res = new InvoiceView();
            res.Invoice = new Invoice();
            res.Invoice.Amount = 0M;
            res.Invoice.AuthAmount = 0M;
            res.Invoice.CreateDT = saleDate;
            res.Invoice.CurrencyID = null;
            res.Invoice.InvoiceStatus = InvoiceStatusEnum.Paid;
            res.Invoice.ProcessDT = saleDate;
            res.SaleList = new List<OrderSaleView>();

            invoiceList.Add(new KeyValuePair<InvoiceView,IList<ChargeHistoryEx>>(
                res, 
                new List<ChargeHistoryEx>()));

            invoiceProductList.Add(new KeyValuePair<InvoiceView, int>(
                res,
                productID
                ));

            return res;
        }

        static Set<ChargeHistoryEx, ChargeHistoryExCurrency, AuthOnlyChargeDetails> BuildChargeHistory(FailedChargeHistory fch, AssertigyMID mid,
            IList<FailedChargeHistoryCurrency> fchCurList, IList<AuthOnlyFailedChargeDetails> fchAuthList,
            IList<Set<ChargeHistoryEx, ChargeHistoryExCurrency, AuthOnlyChargeDetails>> chListToCreate)
        {
            Set<ChargeHistoryEx, ChargeHistoryExCurrency, AuthOnlyChargeDetails> res = null;            

            //auth only transaction
            var fchAuth = fchAuthList.Where(i => i.FailedChargeHistoryID == fch.FailedChargeHistoryID).FirstOrDefault();
            if (fchAuth != null)
            {
                var res1 = new AuthOnlyChargeDetails()
                {
                    Amount = fchAuth.Amount,
                    AuthorizationCode = string.Empty,
                    BillingSubscriptionID = null,
                    ChargeDate = fch.ChargeDate,
                    ChargeTypeID = ChargeTypeEnum.AuthOnly,
                    ChildMID = (mid != null ? mid.MID : null),
                    MerchantAccountID = (mid != null ? mid.AssertigyMIDID : null),
                    RequestedAmount = fchAuth.RequestedAmount,
                    RequestedCurrencyAmount = fchAuth.RequestedCurrencyAmount,
                    RequestedCurrencyID = fchAuth.RequestedCurrencyID,
                    Response = fchAuth.Response,
                    Success = false,
                    TransactionNumber = string.Empty
                };
                res = new Set<ChargeHistoryEx, ChargeHistoryExCurrency, AuthOnlyChargeDetails>() { 
                    Value1 = res1,
                    Value2 = null,
                    Value3 = res1
                };
            }
            else
            {
                var res1 = new ChargeHistoryEx() 
                { 
                    Amount = fch.Amount,
                    AuthorizationCode = string.Empty,
                    BillingSubscriptionID = null,
                    ChargeDate = fch.ChargeDate,
                    ChargeTypeID = ChargeTypeEnum.Charge,
                    ChildMID = (mid != null ? mid.MID : null),
                    MerchantAccountID = (mid != null ? mid.AssertigyMIDID : null),
                    Response = fch.Response,
                    Success = false,
                    TransactionNumber = string.Empty
                };
                var fchCur = fchCurList.Where(i => i.FailedChargeHistoryID == fch.FailedChargeHistoryID).FirstOrDefault();
                ChargeHistoryExCurrency res2 = null;
                if (fchCur != null)
                {
                    res2 = new ChargeHistoryExCurrency()
                    {
                        CurrencyAmount = fchCur.CurrencyAmount,
                        CurrencyID = fchCur.CurrencyID
                    };
                }
                res = new Set<ChargeHistoryEx, ChargeHistoryExCurrency, AuthOnlyChargeDetails>()
                {
                    Value1 = res1,
                    Value2 = res2,
                    Value3 = null
                };
            }

            chListToCreate.Add(res);

            return res;
        }

        static KeyValuePair<string, int> GetBillingSaleProduct(BillingSale bsl, Set<BillingSubscription, Subscription, RecurringPlan, BillingSubscriptionRebillDiscount> bs, IList<ChargeDetails> cdList)
        {
            //1st try get from BillingSale
            if (!string.IsNullOrEmpty(bsl.ProductCode))
            {
                return new KeyValuePair<string, int>(bsl.ProductCode, bsl.Quantity ?? 1);
            }
            //2nd try get from ChargeDetails
            var cd = cdList.Where(i => i.ChargeHistoryID == bsl.ChargeHistoryID).FirstOrDefault();
            if (cd != null)
            {
                return new KeyValuePair<string, int>(cd.SKU, 1);
            }
            //3rd from subscription
            return new KeyValuePair<string, int>(bs.Value2.ProductCode, bs.Value2.Quantity ?? 1);
        }

        static IList<KeyValuePair<string, int>> GetUpsellListProducts(IList<Upsell> upsellList)
        {
            return upsellList.Select(i => new KeyValuePair<string, int>(i.ProductCode, i.Quantity.Value)).ToList();
        }

        static IList<KeyValuePair<string, int>> GetUpsellSaleListProducts(IList<UpsellSale> upsellSaleList, IList<Upsell> fullUpsellList)
        {
            var upsellList = (
                from usl in upsellSaleList
                join u in fullUpsellList on usl.UpsellID equals u.UpsellID
                select u).ToList();
            return GetUpsellListProducts(upsellList);
        }

        static decimal GetUpsellAmount(long saleID, IList<ChargeHistoryExSale> saleList, InvoiceView defaultValue)
        {
            var saleAmount = saleList.Where(i => i.SaleID == saleID).FirstOrDefault();
            if (saleAmount != null)
            {
                return (saleAmount.CurrencyAmount != null ? saleAmount.CurrencyAmount.Value : saleAmount.Amount.Value);
            }
            return defaultValue.Invoice.Amount.Value;
        }

        static void AssignOrder(OrderView o, InvoiceView i)
        {
            o.InvoiceList.Add(i);
            i.Order = o;
            foreach (var sl in i.SaleList)
            {
                o.SaleList.Add(sl);
                sl.Order = o;
            }
        }

        static int MapRecurringStatus(int statusTID)
        {
            if (statusTID == BillingSubscriptionStatusEnum.Active)
                return RecurringStatusEnum.Active;
            if (statusTID == BillingSubscriptionStatusEnum.Inactive)
                return RecurringStatusEnum.Inactive;
            if (statusTID == BillingSubscriptionStatusEnum.PendingReturn)
                return RecurringStatusEnum.Inactive;
            if (statusTID == BillingSubscriptionStatusEnum.Returned)
                return RecurringStatusEnum.Inactive;
            if (statusTID == BillingSubscriptionStatusEnum.Declined)
                return RecurringStatusEnum.Declined;
            if (statusTID == BillingSubscriptionStatusEnum.PreCancel)
                return RecurringStatusEnum.Inactive;
            if (statusTID == BillingSubscriptionStatusEnum.ReturnedNoRMA)
                return RecurringStatusEnum.ReturnedNoRMA;
            if (statusTID == BillingSubscriptionStatusEnum.Scrubbed)
                return RecurringStatusEnum.Scrubbed;
            if (statusTID == BillingSubscriptionStatusEnum.OrderCompleted)
                return RecurringStatusEnum.Completed;
            if (statusTID == BillingSubscriptionStatusEnum.ChargebackRetrievalRequest)
                return RecurringStatusEnum.Inactive;
            if (statusTID == BillingSubscriptionStatusEnum.ChargebackPending)
                return RecurringStatusEnum.Inactive;
            if (statusTID == BillingSubscriptionStatusEnum.ChargebackDisputeWon)
                return RecurringStatusEnum.Inactive;
            if (statusTID == BillingSubscriptionStatusEnum.ChargebackDisputeLost)
                return RecurringStatusEnum.Inactive;
            if (statusTID == BillingSubscriptionStatusEnum.ChargebackDisputeFinal)
                return RecurringStatusEnum.Inactive;
            return RecurringStatusEnum.Inactive;
        }

        static void SaveOrder(IDao dao, OrderView order, IDictionary<OrderSaleView, OrderRecurringPlan> rebillPlanMap, IDictionary<InvoiceView, IList<ChargeHistoryEx>> invoiceList)
        {
            dao.Save(order.Order);
            foreach (var inv in order.InvoiceList)
            {
                inv.Invoice.OrderID = order.Order.OrderID;
                dao.Save(inv.Invoice);

                foreach (var sl in inv.SaleList)
                {
                    sl.OrderSale.InvoiceID = inv.Invoice.InvoiceID;
                }
                foreach (var ch in invoiceList[inv])
	            {
		            ChargeHistoryInvoice chInv = new ChargeHistoryInvoice();
                    chInv.ChargeHistoryID = ch.ChargeHistoryID;
                    chInv.InvoiceID = inv.Invoice.InvoiceID;
                    dao.Save(chInv);
	            }
            }
            foreach (var sl in order.SaleList)
            {
                sl.OrderSale.OrderID = order.Order.OrderID;
                SaveSale(dao, sl, rebillPlanMap, invoiceList);
            }
        }

        static void SaveSale(IDao dao, OrderSaleView sale, IDictionary<OrderSaleView, OrderRecurringPlan> rebillPlanMap, IDictionary<InvoiceView, IList<ChargeHistoryEx>> invoiceList)
        {
            if (sale.OrderSale.SaleType == OrderSaleTypeEnum.Rebill)
            {
                RecurringSale recSl = (RecurringSale)sale.OrderSale;
                recSl.OrderRecurringPlanID = rebillPlanMap[sale].OrderRecurringPlanID;
                if (recSl.OrderRecurringPlanID == null)
                    throw new Exception("Attempt to save Rebill before OrderRecurringPlan");
                dao.Save(recSl);
            }
            else
            {
                dao.Save(sale.OrderSale);
                foreach (var plan in sale.PlanList)
                {
                    plan.SaleID = sale.OrderSale.SaleID;
                    dao.Save(plan);
                }
            }

            foreach (var p in sale.ProductList)
            {
                p.OrderProduct.SaleID = sale.OrderSale.SaleID;
                dao.Save(p.OrderProduct);
            }

            foreach (var sh in sale.ShipmentList)
            {
                sh.Shipment.SaleID = sale.OrderSale.SaleID;
                dao.Save(sh.Shipment);
            }
        }

        static string ExtractTrackingNumber(string trackingNumber)
        {
            string res = trackingNumber.Substring(0, 100);
            var matches = System.Text.RegularExpressions.Regex.Matches(trackingNumber, @"\d{10,}");
            for (int i = 0; i < matches.Count; i++)
            {
                res = matches[i].Value;
            }
            return res;
        }

        //implements inseting into OrderSale for existing SaleID
        public class OrderSaleDataProvider : EntityDataProvider<OrderSale>
        {
            private SaleDataProvider saleDataProvider = new SaleDataProvider();

            private const string INSERT_COMMAND = "INSERT INTO OrderSale(SaleID, SaleName, SaleType, Quantity, OrderID, PurePrice, SaleStatus, InvoiceID, CreateDT, ProcessDT) VALUES(@SaleID, @SaleName, @SaleType, @Quantity, @OrderID, @PurePrice, @SaleStatus, @InvoiceID, @CreateDT, @ProcessDT);";

            public override void Save(OrderSale entity, IMySqlCommandCreater cmdCreater)
            {
                MySqlCommand cmd = cmdCreater.CreateCommand();
                Sale sale = null;

                if (entity.SaleID == null)
                {
                    cmd.CommandText = INSERT_COMMAND;
                    //Create Sale record
                    sale = new Sale()
                    {
                        CreateDT = entity.CreateDT,
                        NotShip = true,
                        SaleTypeID = SaleTypeEnum.OrderSale,
                        TrackingNumber = null
                    };
                    saleDataProvider.Save(sale, cmdCreater);
                }
                else
                {
                    cmd.CommandText = INSERT_COMMAND;
                }

                cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID ?? sale.SaleID;
                cmd.Parameters.Add("@SaleName", MySqlDbType.VarChar).Value = entity.SaleName;
                cmd.Parameters.Add("@SaleType", MySqlDbType.Int32).Value = entity.SaleType;
                cmd.Parameters.Add("@Quantity", MySqlDbType.Int32).Value = entity.Quantity;
                cmd.Parameters.Add("@OrderID", MySqlDbType.Int64).Value = entity.OrderID;
                cmd.Parameters.Add("@PurePrice", MySqlDbType.Decimal).Value = entity.PurePrice;
                cmd.Parameters.Add("@SaleStatus", MySqlDbType.Int32).Value = entity.SaleStatus;
                cmd.Parameters.Add("@InvoiceID", MySqlDbType.Int64).Value = entity.InvoiceID;
                cmd.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = entity.CreateDT;
                cmd.Parameters.Add("@ProcessDT", MySqlDbType.Timestamp).Value = entity.ProcessDT;


                cmd.ExecuteNonQuery();
                if (entity.SaleID == null)
                    entity.SaleID = sale.SaleID; //Convert.ToInt64(cmd.ExecuteScalar());
            }

            public override OrderSale Load(object key, IMySqlCommandCreater cmdCreater)
            {
                throw new NotImplementedException();
            }

            public override OrderSale Load(System.Data.DataRow row)
            {
                throw new NotImplementedException();
            }
        }

        public class RecurringSaleDataProvider : EntityDataProvider<RecurringSale>
        {
            private OrderSaleDataProvider orderSaleDataProvider = new OrderSaleDataProvider();

            private const string INSERT_COMMAND = "INSERT INTO RecurringSale(SaleID, OrderRecurringPlanID, RecurringCycle, ReAttempt) VALUES(@SaleID, @OrderRecurringPlanID, @RecurringCycle, @ReAttempt);";

            public override void Save(RecurringSale entity, IMySqlCommandCreater cmdCreater)
            {
                MySqlCommand cmd = cmdCreater.CreateCommand();

                if (entity.SaleID == null)
                {
                    cmd.CommandText = INSERT_COMMAND;
                }
                else
                {
                    cmd.CommandText = INSERT_COMMAND;
                }

                orderSaleDataProvider.Save(entity, cmdCreater);

                cmd.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = entity.SaleID;
                cmd.Parameters.Add("@OrderRecurringPlanID", MySqlDbType.Int64).Value = entity.OrderRecurringPlanID;
                cmd.Parameters.Add("@RecurringCycle", MySqlDbType.Int32).Value = entity.RecurringCycle;
                cmd.Parameters.Add("@ReAttempt", MySqlDbType.Bit).Value = entity.ReAttempt;

                cmd.ExecuteNonQuery();
            }

            public override RecurringSale Load(object key, IMySqlCommandCreater cmdCreater)
            {
                throw new NotImplementedException();
            }

            public override RecurringSale Load(System.Data.DataRow row)
            {
                throw new NotImplementedException();
            }
        }
    }
}
