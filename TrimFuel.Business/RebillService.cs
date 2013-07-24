using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Flow;

namespace TrimFuel.Business
{
    public class RebillService : BaseService
    {
        public void ProcessRebills()
        {
            try
            {
                IList<OrderRecurringPlan> rebillList = GetPendingRebillList();
                if (rebillList == null)
                {
                    throw new Exception("Can't get pending rebills list");
                }

                IList<AssertigyMIDAmountView> midListWithDailyRebillRevenue = GetRebillRevenue(
                    new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0),
                    new DateTime(DateTime.Today.AddDays(1).Year, DateTime.Today.AddDays(1).Month, DateTime.Today.AddDays(1).Day, 0, 0, 0));
                if (midListWithDailyRebillRevenue == null)
                {
                    throw new Exception("Can't get daily rebill revenue");
                }

                MIDBalancing midBalancing = new MIDBalancing();

                using (var rebillJob = new JobProcess("Rebills"))
                {
                    decimal progress = 0.0M;
                    StringBuilder currentState = null;
                    int successfulCount = 0;
                    int failedCount = 0;
                    int queuedCount = 0;
                    int notProcessedCount = 0;
                    int totalProcessed = 0;

                    foreach (var rebill in rebillList)
	                {
                        totalProcessed = successfulCount + failedCount + queuedCount + notProcessedCount;
                        progress = 100M * Math.Round((decimal)totalProcessed / rebillList.Count, 2);
                        currentState = new StringBuilder();
                        currentState.AppendLine(string.Format("Rebills processing: {0} from {1} rebills processed.", totalProcessed, rebillList.Count));
                        currentState.AppendLine(string.Format("Succeeded: {0}", successfulCount));
                        currentState.AppendLine(string.Format("Failed: {0}", failedCount));
                        currentState.AppendLine(string.Format("Queued: {0}", queuedCount));
                        //currentState.AppendLine(string.Format("Completed: {0}", notProcessedCount));
                        if (successfulCount + failedCount > 0)
                            currentState.AppendLine(string.Format("Conversion Rate: {0}%", Math.Round(100.00M * (decimal)successfulCount / (decimal)(successfulCount + failedCount))));
                        else
                            currentState.AppendLine("Conversion Rate: N/A");

                        if (rebillJob.CheckAvailabilityAndUpdateProgress(progress, currentState.ToString()))
                        {
                            var res = ProcessRebill(rebill, midBalancing, midListWithDailyRebillRevenue);
                            if (res != null)
                            {
                                if (res.Invoice.Invoice.InvoiceStatus == InvoiceStatusEnum.Paid)
                                {
                                    successfulCount++;
                                    if (res.ChargeResult != null && res.ChargeResult.ChargeHistory.MerchantAccountID != null)
                                    {
                                        AssertigyMIDAmountView mid = midListWithDailyRebillRevenue.First(i => i.MID.AssertigyMIDID == res.ChargeResult.ChargeHistory.MerchantAccountID);
                                        mid.Amount += res.Invoice.Invoice.Amount;
                                    }
                                }
                                else
                                {
                                    failedCount++;
                                }
                            }
                            else
                            {
                                queuedCount++;
                            }
                        }
                        else
                        {
                            break;
                        }
	                }
                    totalProcessed = successfulCount + failedCount + queuedCount + notProcessedCount;
                    currentState = new StringBuilder();
                    currentState.AppendLine(string.Format("Rebills processing: {0} from {1} rebills processed.", totalProcessed, rebillList.Count));
                    currentState.AppendLine(string.Format("Succeeded: {0}", successfulCount));
                    currentState.AppendLine(string.Format("Failed: {0}", failedCount));
                    currentState.AppendLine(string.Format("Queued: {0}", queuedCount));
                    //currentState.AppendLine(string.Format("Completed: {0}", notProcessedCount));
                    if (successfulCount + failedCount > 0)
                        currentState.AppendLine(string.Format("Conversion Rate: {0}%", Math.Round(100.00M * (decimal)successfulCount / (decimal)(successfulCount + failedCount))));
                    else
                        currentState.AppendLine("Conversion Rate: N/A");

                    rebillJob.CheckAvailabilityAndUpdateProgress(100M, currentState.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected InvoiceView2 ProcessRebill(OrderRecurringPlan orderPlan, MIDBalancing midBalancing, IList<AssertigyMIDAmountView> midListWithDailyRebillRevenue)
        {
            InvoiceView2 res = null;
            try
            {
                var orderPlanView = new OrderService().GetPlan(orderPlan.OrderRecurringPlanID.Value);
                var res2 = new RebillOrderFlow().ProcessRebill(orderPlanView, midBalancing, midListWithDailyRebillRevenue);
                if (res2.ReturnValue != null)
                    res = res2.ReturnValue;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public virtual IList<OrderRecurringPlan> GetPendingRebillList()
        {
            IList<OrderRecurringPlan> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select orp.* from OrderRecurringPlan orp
                    where orp.RecurringStatus = @recurringStatus_Active and orp.NextCycleDT < @endDate
                    order by orp.NextCycleDT
                    limit 10000
                ");
                DateTime tomorrow = DateTime.Today.AddDays(1);
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0);
                q.Parameters.Add("@recurringStatus_Active", MySqlDbType.Int32).Value = RecurringStatusEnum.Active;
                res = dao.Load<OrderRecurringPlan>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public IList<AssertigyMIDAmountView> GetRebillRevenue(DateTime fromDate, DateTime toDate)
        {
            IList<AssertigyMIDAmountView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select IfNull(sum(ch.Amount), 0.0) as Amount, m.* from AssertigyMID m
                    left join (
                        select coalesce(chCur.CurrencyAmount, ch.Amount) as Amount, ch.MerchantAccountID from ChargeHistoryEx ch
                        inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID
                        left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                        where ch.Success = 1 and ch.Amount > 0 and ch.ChargeDate >= @fromDate and ch.ChargeDate < @toDate and cd.SaleTypeID = @saleType_Rebill
                        union all
                        select coalesce(chCur.CurrencyAmount, ch.Amount) as Amount, ch.MerchantAccountID from ChargeHistoryEx ch
                        inner join ChargeHistoryInvoice chInv on chInv.ChargeHistoryID = ch.ChargeHistoryID
                        inner join Invoice inv on inv.InvoiceID = chInv.InvoiceID
                        inner join OrderSale osl on osl.InvoiceID = chInv.InvoiceID
                        left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                        where ch.Success = 1 and ch.Amount > 0 and ch.ChargeDate >= @fromDate and ch.ChargeDate < @toDate and osl.SaleType = @orderSaleType_Rebill
                    ) ch on ch.MerchantAccountID = m.AssertigyMIDID
                    group by m.AssertigyMIDID
                ");
                q.Parameters.Add("@fromDate", MySqlDbType.Timestamp).Value = fromDate;
                q.Parameters.Add("@toDate", MySqlDbType.Timestamp).Value = toDate;
                q.Parameters.Add("@saleType_Rebill", MySqlDbType.Int32).Value = SaleTypeEnum.Rebill;
                q.Parameters.Add("@orderSaleType_Rebill", MySqlDbType.Int32).Value = OrderSaleTypeEnum.Rebill;
                res = dao.Load<AssertigyMIDAmountView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }
    }
}
