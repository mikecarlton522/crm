using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Business;

namespace NS_CreateNotExistingSubscriptions
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

                //get not existing subscriptions
                MySqlCommand q = new MySqlCommand(@"
                    select min(t1.SubscriptionID) as Value from
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
                    left join 
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
                    where t2.RecurringPlanID is null
                    group by t1.ProductID, t1.Cycles
                ");

                IList<View<int>> subscriptionList = dao.Load<View<int>>(q);
                var srv = new SubscriptionNewService();
                foreach (var item in subscriptionList)
                {
                    var res = srv.GetOrCreateRecurringPlanBySubscriptionID(item.Value.Value);
                    if (res == null)
                    {
                        logger.Error(string.Format("Can't create RecurringPlan for Subscription({0})", item.Value));
                    }
                    else
                    {
                        logger.Error(string.Format("RecurringPlan({0}) created for Subscription({1})", res.RecurringPlanID, item.Value));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
