using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Flow
{
    public class PlanFlow : BaseService
    {
        public void ProcessPlan(OrderRecurringPlan plan)
        {
            try
            {
                plan.RecurringStatus = RecurringStatusEnum.Active;
                plan.StartDT = DateTime.Now;
                plan.NextCycleDT = DateTime.Now.AddDays(plan.TrialInterim.Value);
                dao.Save(plan);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public OrderRecurringPlan UpdatePlan(OrderRecurringPlan orderRecurringPlan, int recurringPlanID, DateTime nextCycleDate, int recurringStatus)
        {
            OrderRecurringPlan res = orderRecurringPlan;
            try
            {
                dao.BeginTransaction();

                res.RecurringPlanID = recurringPlanID;
                res.NextCycleDT = nextCycleDate;
                res.RecurringStatus = recurringStatus;
                dao.Save(res);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(ex);
                res = null;
            }
            return res;
        }

        public OrderRecurringPlan UpdatePlan(long orderRecurringPlanID, int recurringPlanID, DateTime nextCycleDate, int recurringStatus)
        {
            OrderRecurringPlan res = null;

            try
            {
                res = EnsureLoad<OrderRecurringPlan>(orderRecurringPlanID);
                res = UpdatePlan(res, recurringPlanID, nextCycleDate, recurringStatus);
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(ex);
                res = null;
            }

            return res;
        }

        public OrderRecurringPlan SetPlanStatus(OrderRecurringPlan orderRecurringPlan, int? recurringStatus)
        {
            OrderRecurringPlan res = orderRecurringPlan;
            try
            {
                dao.BeginTransaction();

                res.RecurringStatus = recurringStatus;
                dao.Save(res);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(ex);
                res = null;
            }
            return res;
        }
    }
}
