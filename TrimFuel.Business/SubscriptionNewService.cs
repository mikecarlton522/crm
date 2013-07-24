using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class SubscriptionNewService : BaseService
    {
        public IList<RecurringPlanView> GetRecurringPlanByProduct(int? productID)
        {
            IList<RecurringPlanView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select rp.*, p.ProductName from RecurringPlan rp
                    inner join Product p on p.ProductID = rp.ProductID
                    where (@productID is null or @productID = rp.ProductID)
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                res = dao.Load<RecurringPlanView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<RecurringPlanCycle> GetRecurringPlanCycleList(int? recurringPlanID)
        {
            IList<RecurringPlanCycle> res = new List<RecurringPlanCycle>();
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from RecurringPlanCycle rpc
                    where @recurringPlanID = rpc.RecurringPlanID
                    order by rpc.Cycle
                ");
                q.Parameters.Add("@recurringPlanID", MySqlDbType.Int32).Value = @recurringPlanID;
                res = dao.Load<RecurringPlanCycle>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<RecurringPlanCycleView> GetRecurringPlanCycles(int? recurringPlanID)
        {
            IList<RecurringPlanCycleView> res = new List<RecurringPlanCycleView>();
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from RecurringPlanCycle rpc
                    where @recurringPlanID = rpc.RecurringPlanID
                    order by rpc.Cycle
                ");
                q.Parameters.Add("@recurringPlanID", MySqlDbType.Int32).Value = @recurringPlanID;
                IList<RecurringPlanCycle> cycleList = dao.Load<RecurringPlanCycle>(q);
                if (cycleList.Count > 0)
                {
                    q = new MySqlCommand(@"
                        select c.* from RecurringPlanCycle rpc
                        inner join RecurringPlanConstraint c on c.RecurringPlanCycleID = rpc.RecurringPlanCycleID
                        where @recurringPlanID = rpc.RecurringPlanID
                    ");
                    q.Parameters.Add("@recurringPlanID", MySqlDbType.Int32).Value = @recurringPlanID;
                    IList<RecurringPlanConstraint> constraintList = dao.Load<RecurringPlanConstraint>(q);

                    q = new MySqlCommand(@"
                        select sh.* from RecurringPlanCycle rpc
                        inner join RecurringPlanShipment sh on sh.RecurringPlanCycleID = rpc.RecurringPlanCycleID
                        where @recurringPlanID = rpc.RecurringPlanID
                    ");
                    q.Parameters.Add("@recurringPlanID", MySqlDbType.Int32).Value = @recurringPlanID;
                    IList<RecurringPlanShipment> shipmentList = dao.Load<RecurringPlanShipment>(q);

                    foreach (var cycle in cycleList)
                    {
                        RecurringPlanCycleView view = new RecurringPlanCycleView();
                        view.Cycle = cycle;
                        view.Constraint = constraintList.FirstOrDefault(i => i.RecurringPlanCycleID == view.Cycle.RecurringPlanCycleID);
                        view.ShipmentList = shipmentList.Where(i => i.RecurringPlanCycleID == view.Cycle.RecurringPlanCycleID).ToList();
                        res.Add(view);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public RecurringPlanCycleView GetRecurringPlanCycle(int? recurringPlanCycleID)
        {
            RecurringPlanCycleView res = new RecurringPlanCycleView();
            try
            {
                res.Cycle = EnsureLoad<RecurringPlanCycle>(recurringPlanCycleID);
                res.Constraint = Load<RecurringPlanConstraint>(new RecurringPlanConstraint.ID() { RecurringPlanCycleID = recurringPlanCycleID });
                MySqlCommand q = new MySqlCommand(@"
                    select sh.* from RecurringPlanShipment sh
                    where @recurringPlanCycleID = sh.RecurringPlanCycleID
                ");
                q.Parameters.Add("@recurringPlanCycleID", MySqlDbType.Int32).Value = recurringPlanCycleID;
                res.ShipmentList = dao.Load<RecurringPlanShipment>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public RecurringPlan InsertOrUpdatePlan(int? recurringPlanID, int? productID, string name)
        {
            RecurringPlan res = null;
            try
            {
                dao.BeginTransaction();

                if (recurringPlanID != null)
                {
                    res = EnsureLoad<RecurringPlan>(recurringPlanID);
                }
                else
                {
                    res = new RecurringPlan();
                }
                res.ProductID = productID;
                res.Name = name;
                dao.Save(res);

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

        public bool DeletePlanCycle(int? recurringPlanCycleID)
        {
            bool res = true;
            try
            {
                dao.BeginTransaction();


                RecurringPlanCycle cycle = EnsureLoad<RecurringPlanCycle>(recurringPlanCycleID);
                IList<RecurringPlanCycle> toUpdateList = GetRecurringPlanCycleList(cycle.RecurringPlanID).Where(i => i.Cycle > cycle.Cycle).ToList();
                foreach (var item in toUpdateList)
                {
                    item.Cycle--;
                    dao.Save(item);
                }

                MySqlCommand q = new MySqlCommand(@"
                        delete from RecurringPlanShipment
                        where RecurringPlanCycleID = @recurringPlanCycleID
                    ");
                q.Parameters.Add("@recurringPlanCycleID", MySqlDbType.Int32).Value = cycle.RecurringPlanCycleID;
                dao.ExecuteNonQuery(q);

                q = new MySqlCommand(@"
                        delete from RecurringPlanConstraint
                        where RecurringPlanCycleID = @recurringPlanCycleID
                    ");
                q.Parameters.Add("@recurringPlanCycleID", MySqlDbType.Int32).Value = cycle.RecurringPlanCycleID;
                dao.ExecuteNonQuery(q);

                q = new MySqlCommand(@"
                        delete from RecurringPlanCycle
                        where RecurringPlanCycleID = @recurringPlanCycleID
                    ");
                q.Parameters.Add("@recurringPlanCycleID", MySqlDbType.Int32).Value = cycle.RecurringPlanCycleID;
                dao.ExecuteNonQuery(q);

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

        public RecurringPlanCycleView InsertPlanCycle(int? recurringPlanID, int? interim, int? nextCycle,
            int? chargeTypeID, decimal? amount, 
            IList<KeyValuePair<string, int>> shipments)
        {
            RecurringPlanCycleView res = new RecurringPlanCycleView();
            try
            {
                dao.BeginTransaction();

                if (interim == null)
                {
                    interim = 30;
                }

                IList<RecurringPlanCycle> cycleList = GetRecurringPlanCycleList(recurringPlanID);

                res.Cycle = new RecurringPlanCycle() { 
                    RecurringPlanID = recurringPlanID,
                    Interim = interim,
                    Recurring = false, //no matter, will be updated
                    RetryInterim = cycleList.Count + 1,
                    Cycle = cycleList.Count + 1
                };
                dao.Save(res.Cycle);

                if (chargeTypeID != null)
                {
                    res.Constraint = new RecurringPlanConstraint()
                    {
                        ChargeTypeID = chargeTypeID,
                        RecurringPlanCycleID = res.Cycle.RecurringPlanCycleID,
                        Amount = amount,
                        ShippingAmount = 0M,
                        TaxAmount = 0M
                    };
                    dao.Save(res.Constraint);
                }

                res.ShipmentList = new List<RecurringPlanShipment>();
                foreach (var item in shipments)
                {
                    RecurringPlanShipment shipment = new RecurringPlanShipment()
                    {
                        RecurringPlanCycleID = res.Cycle.RecurringPlanCycleID,
                        ProductSKU = item.Key,
                        Quantity = item.Value
                    };
                    dao.Save(shipment);
                    
                    res.ShipmentList.Add(shipment);
                }

                cycleList.Add(res.Cycle);
                foreach (var item in cycleList)
                {
                    if (nextCycle == null || item.Cycle < nextCycle)
                    {
                        item.Recurring = false;
                    }
                    else
                    {
                        item.Recurring = true;
                    }
                    dao.Save(item);
                }

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

        public RecurringPlanCycleView UpdatePlanCycle(int? recurringPlanCycleID, int? interim, int? nextCycle,
            int? chargeTypeID, decimal? amount,
            IList<KeyValuePair<string, int>> shipments)
        {
            RecurringPlanCycleView res = new RecurringPlanCycleView();
            try
            {
                dao.BeginTransaction();

                if (interim == null)
                {
                    interim = 30;
                }

                res.Cycle = EnsureLoad<RecurringPlanCycle>(recurringPlanCycleID);
                res.Cycle.Interim = interim;
                dao.Save(res.Cycle);

                MySqlCommand q = null;

                res.Constraint = Load<RecurringPlanConstraint>(new RecurringPlanConstraint.ID() { RecurringPlanCycleID = recurringPlanCycleID });
                if (chargeTypeID != null)
                {
                    if (res.Constraint == null)
                    {
                        res.Constraint = new RecurringPlanConstraint()
                        {
                            RecurringPlanCycleID = res.Cycle.RecurringPlanCycleID,
                        };
                    }
                    res.Constraint.ChargeTypeID = chargeTypeID;
                    res.Constraint.Amount = amount;
                    res.Constraint.ShippingAmount = 0M;
                    res.Constraint.TaxAmount = 0M;
                    dao.Save(res.Constraint);
                }
                else if (res.Constraint != null)
                {
                    q = new MySqlCommand(@"
                        delete from RecurringPlanConstraint
                        where RecurringPlanCycleID = @recurringPlanCycleID
                    ");
                    q.Parameters.Add("@recurringPlanCycleID", MySqlDbType.Int32).Value = res.Cycle.RecurringPlanCycleID;
                    dao.ExecuteNonQuery(q);

                    res.Constraint = null;
                }

                q = new MySqlCommand(@"
                        delete from RecurringPlanShipment
                        where RecurringPlanCycleID = @recurringPlanCycleID
                    ");
                q.Parameters.Add("@recurringPlanCycleID", MySqlDbType.Int32).Value = res.Cycle.RecurringPlanCycleID;
                dao.ExecuteNonQuery(q);

                res.ShipmentList = new List<RecurringPlanShipment>();
                foreach (var item in shipments)
                {
                    RecurringPlanShipment shipment = new RecurringPlanShipment()
                    {
                        RecurringPlanCycleID = res.Cycle.RecurringPlanCycleID,
                        ProductSKU = item.Key,
                        Quantity = item.Value
                    };
                    dao.Save(shipment);

                    res.ShipmentList.Add(shipment);
                }

                IList<RecurringPlanCycle> cyclesToUpdate = GetRecurringPlanCycleList(res.Cycle.RecurringPlanID);
                if (cyclesToUpdate.Last().Cycle == res.Cycle.Cycle)
                {
                    res.Cycle = cyclesToUpdate.Last();

                    foreach (var item in cyclesToUpdate)
                    {
                        if (nextCycle == null || item.Cycle < nextCycle)
                        {
                            item.Recurring = false;
                        }
                        else
                        {
                            item.Recurring = true;
                        }
                        dao.Save(item);
                    }
                }

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

        public IList<ProductSKU> GetProductList()
        {
            IList<ProductSKU> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from ProductSKU
                    order by ProductName
                ");

                res = dao.Load<ProductSKU>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<GroupProductSKU> GetGroupedProductList(int productID)
        {
            IList<GroupProductSKU> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select distinct 
                        GROUP_CONCAT(CONCAT(CAST(rpsh.Quantity as CHAR), 'x ', p.ProductName) ORDER BY p.ProductName SEPARATOR ' + ') as GroupProductName,
                        GROUP_CONCAT(CONCAT(CAST(rpsh.Quantity as CHAR), 'x', p.ProductSKU) ORDER BY p.ProductSKU SEPARATOR '_') as GroupProductSKU,
                        SUM(rpsh.Quantity) as GroupVolume
                    from RecurringPlanShipment rpsh
                    inner join ProductSKU p on p.ProductSKU = rpsh.ProductSKU
                    inner join RecurringPlanCycle rpc on rpc.RecurringPlanCycleID = rpsh.RecurringPlanCycleID and rpc.Recurring = 1
                    inner join RecurringPlan rp on rp.RecurringPlanID = rpc.RecurringPlanID
                    where rp.ProductID = @productID
                    group by rpc.RecurringPlanCycleID
                    order by GroupVolume
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                res = dao.Load<GroupProductSKU>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<View<int>> GetPlanFrequency(int productID, string groupProductSKU)
        {
            IList<View<int>> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select distinct rpc.Interim as Value
                    from RecurringPlanShipment rpsh
                    inner join ProductSKU p on p.ProductSKU = rpsh.ProductSKU
                    inner join RecurringPlanCycle rpc on rpc.RecurringPlanCycleID = rpsh.RecurringPlanCycleID and rpc.Recurring = 1
                    inner join RecurringPlan rp on rp.RecurringPlanID = rpc.RecurringPlanID
                    where rp.ProductID = @productID
                    group by rpc.RecurringPlanCycleID
                    having GROUP_CONCAT(CONCAT(CAST(rpsh.Quantity as CHAR), 'x', p.ProductSKU) ORDER BY p.ProductSKU SEPARATOR '_') = @groupProductSKU
                    order by rpc.Interim
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@groupProductSKU", MySqlDbType.VarChar).Value = groupProductSKU;

                res = dao.Load<View<int>>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public RecurringPlanView GetPlan(int recurringPlanID)
        {
            RecurringPlanView res = null;
            try
            {
                RecurringPlan res2 = EnsureLoad<RecurringPlan>(recurringPlanID);
                res = new RecurringPlanView()
                {
                    Name = res2.Name,
                    RecurringPlanID = res2.RecurringPlanID,
                    ProductID = res2.ProductID,
                    CycleList = GetRecurringPlanCycles(res2.RecurringPlanID)
                };
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<RecurringPlanView> GetPlanList(int productID, string groupProductSKU, int planFrequency)
        {
            IList<RecurringPlanView> res = null;
            try
            {
                Product p = EnsureLoad<Product>(productID);

                MySqlCommand q = new MySqlCommand(@"
                    select distinct rp.*
                    from RecurringPlanShipment rpsh
                    inner join ProductSKU p on p.ProductSKU = rpsh.ProductSKU
                    inner join RecurringPlanCycle rpc on rpc.RecurringPlanCycleID = rpsh.RecurringPlanCycleID and rpc.Recurring = 1
                    inner join RecurringPlan rp on rp.RecurringPlanID = rpc.RecurringPlanID
                    where rp.ProductID = @productID and rpc.Interim = @planFrequency
                    group by rpc.RecurringPlanCycleID
                    having GROUP_CONCAT(CONCAT(CAST(rpsh.Quantity as CHAR), 'x', p.ProductSKU) ORDER BY p.ProductSKU SEPARATOR '_') = @groupProductSKU
                    order by rpc.Interim
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@groupProductSKU", MySqlDbType.VarChar).Value = groupProductSKU;
                q.Parameters.Add("@planFrequency", MySqlDbType.Int32).Value = planFrequency;

                IList<RecurringPlan> res2 = dao.Load<RecurringPlan>(q);
                res = new List<RecurringPlanView>();
                foreach (var item in res2)
                {
                    res.Add(new RecurringPlanView() { 
                        Name = item.Name,
                        ProductName = p.ProductName,
                        RecurringPlanID = item.RecurringPlanID,
                        ProductID = item.ProductID,
                        CycleList = GetRecurringPlanCycles(item.RecurringPlanID)
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public BusinessError<bool> DeleteRecurringPlan(int recurringPlanID)
        {
            BusinessError<bool> res = new BusinessError<bool>(true, BusinessErrorState.Success, null);

            try
            {
                dao.BeginTransaction();

                //Check number of subscriptions in use
                int subscriptionsCount = 0;
                MySqlCommand q = new MySqlCommand(@"
                    select count(*) as Value from OrderRecurringPlan 
                    where RecurringPlanID = @recurringPlanID
                ");
                q.Parameters.Add("@recurringPlanID", MySqlDbType.Int32).Value = recurringPlanID;
                subscriptionsCount = dao.Load<View<int>>(q).Single().Value.Value;

                if (subscriptionsCount > 0)
                {
                    res.ReturnValue = false;
                    res.State = BusinessErrorState.Error;
                    res.ErrorMessage = "There are currently " + subscriptionsCount.ToString() + " users associated with this plan.  Please move these users to a new plan and try again.";
                }
                else
                {
                    q = new MySqlCommand(@"
                        delete rp, c, cc, csh
                        from RecurringPlan rp
                        inner join RecurringPlanCycle c on c.RecurringPlanID = rp.RecurringPlanID
                        inner join RecurringPlanConstraint cc on cc.RecurringPlanCycleID = c.RecurringPlanCycleID
                        inner join RecurringPlanShipment csh on csh.RecurringPlanCycleID = c.RecurringPlanCycleID
                        where rp.RecurringPlanID = @recurringPlanID
                    ");
                    q.Parameters.Add("@recurringPlanID", MySqlDbType.Int32).Value = recurringPlanID;
                    dao.ExecuteNonQuery(q);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(ex);

                res.ErrorMessage = "Unrecognized error occurred.";
                res.State = BusinessErrorState.Error;
                res.ReturnValue = false;
            }

            return res;
        }

        public CampaignRecurringPlanView GetRecurringPlanByCampaign(int campaignID)
        {
            CampaignRecurringPlanView res = new CampaignRecurringPlanView();

            try
            {
                res.CampaignRecurringPlan = GetRecurringPlanByCampaign_(campaignID);

                if (res.CampaignRecurringPlan != null)
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select ctp.* from CampaignTrialProduct ctp
                        where ctp.CampaignRecurringPlanID = @campaignRecurringPlanID
                    ");
                    q.Parameters.Add("@campaignRecurringPlanID", MySqlDbType.Int32).Value = res.CampaignRecurringPlan.CampaignRecurringPlanID;
                    res.ProductList = dao.Load<CampaignTrialProduct>(q).ToList();

                    res.RecurringPlan = GetPlan(res.CampaignRecurringPlan.RecurringPlanID.Value);
                }
                else
                {
                    res.ProductList = new List<CampaignTrialProduct>();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }

            return res;
        }

        public CampaignRecurringPlan GetRecurringPlanByCampaign_(int campaignID)
        {
            CampaignRecurringPlan res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select crp.* from CampaignRecurringPlan crp
                    where crp.CampaignID = @campaignID
                ");
                q.Parameters.Add("@campaignID", MySqlDbType.Int32).Value = campaignID;
                res = dao.Load<CampaignRecurringPlan>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }

            return res;
        }

        public void UpdateCampaignSubscription(int campaignID, int? recurringPlanID, decimal? trialPrice, int? trialInterim, IList<KeyValuePair<string, int>> products)
        {
            try
            {
                dao.BeginTransaction();

                CampaignRecurringPlan existingPlan = GetRecurringPlanByCampaign_(campaignID);
                if (recurringPlanID == null)
                {
                    if (existingPlan != null)
                    {
                        //delete
                        MySqlCommand q = new MySqlCommand(@"
                            delete from CampaignTrialProduct 
                            where CampaignRecurringPlanID = @campaignRecurringPlanID
                        ");
                        q.Parameters.Add("@campaignRecurringPlanID", MySqlDbType.Int32).Value = existingPlan.CampaignRecurringPlanID;
                        dao.ExecuteNonQuery(q);

                        q = new MySqlCommand(@"
                            delete from CampaignRecurringPlan
                            where CampaignRecurringPlanID = @campaignRecurringPlanID
                        ");
                        q.Parameters.Add("@campaignRecurringPlanID", MySqlDbType.Int32).Value = existingPlan.CampaignRecurringPlanID;
                        dao.ExecuteNonQuery(q);
                    }
                }
                else
                {
                    if (existingPlan != null)
                    {
                        //delete products
                        MySqlCommand q = new MySqlCommand(@"
                            delete from CampaignTrialProduct 
                            where CampaignRecurringPlanID = @campaignRecurringPlanID
                        ");
                        q.Parameters.Add("@campaignRecurringPlanID", MySqlDbType.Int32).Value = existingPlan.CampaignRecurringPlanID;
                        dao.ExecuteNonQuery(q);
                    }
                    else
                    {
                        existingPlan = new CampaignRecurringPlan();
                        existingPlan.CampaignID = campaignID;
                    }
                    existingPlan.RecurringPlanID = recurringPlanID;
                    existingPlan.TrialInterim = trialInterim;
                    existingPlan.TrialPrice = trialPrice;
                    dao.Save(existingPlan);

                    foreach (var item in products)
                    {
                        CampaignTrialProduct product = new CampaignTrialProduct()
                        {
                            CampaignRecurringPlanID = existingPlan.CampaignRecurringPlanID,
                            ProductSKU = item.Key,
                            Quantity = item.Value
                        };
                        dao.Save(product);
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Works only for recurring subscriptions
        /// </summary>
        /// <param name="subscriptionID"></param>
        /// <returns></returns>
        public RecurringPlanView GetOrCreateRecurringPlanBySubscriptionID(int subscriptionID)
        {
            RecurringPlanView res = null;
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand(@"
                    select t2.RecurringPlanID as Value from
                    (
                    select 
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
                    where s.Recurring = 1 and s.SubscriptionID = @subscriptionID
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
                ");
                q.Parameters.Add("@subscriptionID", MySqlDbType.Int32).Value = subscriptionID;
                View<int> recurringPlanID = dao.Load<View<int>>(q).FirstOrDefault();
                if (recurringPlanID != null)
                {
                    res = GetPlan(recurringPlanID.Value.Value);
                }
                else
                {
                    Subscription s = EnsureLoad<Subscription>(subscriptionID);

                    //validation
                    if (!s.Recurring.Value)
                        throw new Exception(string.Format("Subscription({0}) is not recurring", subscriptionID));
                    if (string.IsNullOrEmpty(s.SKU2))
                        throw new Exception(string.Format("Subscription({0}) has invalid SKU2 value", subscriptionID));
                    if (s.QuantitySKU2 == null)
                        throw new Exception(string.Format("Subscription({0}) has invalid QuantitySKU2 value", subscriptionID));
                    if (s.ProductID == null)
                        throw new Exception(string.Format("Subscription({0}) has invalid ProductID value", subscriptionID));
                    if (s.SecondShipping == null || s.SecondBillAmount == null)
                        throw new Exception(string.Format("Subscription({0}) has invalid 1st rebill cycle amount values", subscriptionID));
                    if (s.RegularShipping == null || s.RegularBillAmount == null)
                        throw new Exception(string.Format("Subscription({0}) has invalid regular rebill cycle amount values", subscriptionID));
                    if (s.SecondInterim == null)
                        throw new Exception(string.Format("Subscription({0}) has invalid 1st rebill cycle interim value", subscriptionID));
                    if (s.RegularInterim == null)
                        throw new Exception(string.Format("Subscription({0}) has invalid regular rebill cycle interim value", subscriptionID));

                    IList<InventoryView> invList = new InventoryService().GetInventoryListByProductCode(s.SKU2);
                    if (invList == null || invList.Count == 0)
                        throw new Exception(string.Format("Subscription({0}) has invalid SKU2({1}) value", subscriptionID, s.SKU2));

                    Product product = EnsureLoad<Product>(s.ProductID);

                    //Create RecurringPlan
                    RecurringPlan rp = new RecurringPlan();
                    rp.ProductID = s.ProductID;
                    rp.Name = s.DisplayName ?? "";
                    dao.Save(rp);

                    if (s.SecondInterim == s.RegularInterim &&
                        s.ShipFirstRebill == true &&
                        (s.SecondShipping + s.SecondBillAmount == s.RegularShipping + s.RegularBillAmount))
                    {
                        //Create 1 cycle
                        RecurringPlanCycle rpc = new RecurringPlanCycle();
                        rpc.RecurringPlanID = rp.RecurringPlanID;
                        rpc.Recurring = true;
                        rpc.Cycle = 1;
                        rpc.Interim = s.RegularInterim;
                        rpc.RetryInterim = 7;//not in use so far
                        dao.Save(rpc);

                        RecurringPlanConstraint rpp = new RecurringPlanConstraint();
                        rpp.RecurringPlanCycleID = rpc.RecurringPlanCycleID;
                        rpp.Amount = s.RegularShipping + s.RegularBillAmount;
                        rpp.ChargeTypeID = ChargeTypeEnum.Charge;
                        rpp.ShippingAmount = 0M;//not in use so far
                        rpp.TaxAmount = 0M;//not in use so far
                        dao.Save(rpp);

                        IList<RecurringPlanShipment> rpshList = new List<RecurringPlanShipment>();
                        foreach (var inv in invList)
                        {
                            RecurringPlanShipment rpsh = new RecurringPlanShipment();
                            rpsh.RecurringPlanCycleID = rpc.RecurringPlanCycleID;
                            rpsh.ProductSKU = inv.SKU;
                            rpsh.Quantity = inv.Quantity * s.QuantitySKU2;
                            dao.Save(rpsh);
                            rpshList.Add(rpsh);
                        }

                        res = new RecurringPlanView()
                        {
                            RecurringPlanID = rp.RecurringPlanID,
                            Name = rp.Name,
                            ProductID = rp.ProductID,
                            ProductName = product.ProductName,
                            CycleList = new List<RecurringPlanCycleView>() { 
                                new RecurringPlanCycleView() {
                                    Cycle = rpc,
                                    Constraint = rpp,
                                    ShipmentList = rpshList
                                }
                            }
                        };
                    }
                    else
                    {
                        //Create 1st cycle
                        RecurringPlanCycle rpc = new RecurringPlanCycle();
                        rpc.RecurringPlanID = rp.RecurringPlanID;
                        rpc.Recurring = false;
                        rpc.Cycle = 1;
                        rpc.Interim = s.SecondInterim;
                        rpc.RetryInterim = 7;//not in use so far
                        dao.Save(rpc);

                        RecurringPlanConstraint rpp = new RecurringPlanConstraint();
                        rpp.RecurringPlanCycleID = rpc.RecurringPlanCycleID;
                        rpp.Amount = s.SecondShipping + s.SecondBillAmount;
                        rpp.ChargeTypeID = ChargeTypeEnum.Charge;
                        rpp.ShippingAmount = 0M;//not in use so far
                        rpp.TaxAmount = 0M;//not in use so far
                        dao.Save(rpp);

                        IList<RecurringPlanShipment> rpshList = new List<RecurringPlanShipment>();
                        if (s.ShipFirstRebill == true)
                        {
                            foreach (var inv in invList)
                            {
                                RecurringPlanShipment rpsh = new RecurringPlanShipment();
                                rpsh.RecurringPlanCycleID = rpc.RecurringPlanCycleID;
                                rpsh.ProductSKU = inv.SKU;
                                rpsh.Quantity = inv.Quantity * s.QuantitySKU2;
                                dao.Save(rpsh);
                                rpshList.Add(rpsh);
                            }
                        }

                        res = new RecurringPlanView()
                        {
                            RecurringPlanID = rp.RecurringPlanID,
                            Name = rp.Name,
                            ProductID = rp.ProductID,
                            ProductName = product.ProductName,
                            CycleList = new List<RecurringPlanCycleView>() { 
                                new RecurringPlanCycleView() {
                                    Cycle = rpc,
                                    Constraint = rpp,
                                    ShipmentList = rpshList
                                }
                            }
                        };

                        //Create 2nd cycle
                        rpc = new RecurringPlanCycle();
                        rpc.RecurringPlanID = rp.RecurringPlanID;
                        rpc.Recurring = true;
                        rpc.Cycle = 2;
                        rpc.Interim = s.RegularInterim;
                        rpc.RetryInterim = 7;//not in use so far
                        dao.Save(rpc);

                        rpp = new RecurringPlanConstraint();
                        rpp.RecurringPlanCycleID = rpc.RecurringPlanCycleID;
                        rpp.Amount = s.RegularShipping + s.RegularBillAmount;
                        rpp.ChargeTypeID = ChargeTypeEnum.Charge;
                        rpp.ShippingAmount = 0M;//not in use so far
                        rpp.TaxAmount = 0M;//not in use so far
                        dao.Save(rpp);

                        rpshList = new List<RecurringPlanShipment>();
                        foreach (var inv in invList)
                        {
                            RecurringPlanShipment rpsh = new RecurringPlanShipment();
                            rpsh.RecurringPlanCycleID = rpc.RecurringPlanCycleID;
                            rpsh.ProductSKU = inv.SKU;
                            rpsh.Quantity = inv.Quantity * s.QuantitySKU2;
                            dao.Save(rpsh);
                            rpshList.Add(rpsh);
                        }

                        res.CycleList.Add(
                                new RecurringPlanCycleView()
                                {
                                    Cycle = rpc,
                                    Constraint = rpp,
                                    ShipmentList = rpshList
                                });
                    }
                }

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
    }
}
