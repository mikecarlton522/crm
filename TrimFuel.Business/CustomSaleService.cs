using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Model;

namespace TrimFuel.Business
{
    public class CustomSaleService : SaleService
    {
        public bool Refund45ForLastChargeWithAmountGreaterThan45(long? billingID)
        {
            bool res = false;

            try
            {
                var sales = GetSaleListByBillingID(billingID).OrderByDescending(u => u.CreateDT);
                foreach (var sale in sales)
                {
                    var sInfo = GetSaleInfo(sale);
                    if (sInfo != null)
                    {
                        if (GetAmountWithRefunds(sInfo, sale.SaleID) >= 45)
                        {
                            var resEx = DoRefund(sInfo.Charge, 45);

                            res = resEx.State == BusinessErrorState.Success;

                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = false;
            }

            return res;
        }

        public bool FullRefundForLastChargeWithAmountGreaterThan45(long? billingID)
        {
            bool res = false;
            try
            {
                var sales = GetSaleListByBillingID(billingID).OrderByDescending(u => u.CreateDT);
                foreach (var sale in sales)
                {
                    var sInfo = GetSaleInfo(sale);
                    if (sInfo != null)
                    {
                        var amount = GetAmountWithRefunds(sInfo, sale.SaleID);
                        if (amount >= 45)
                        {
                            var resEx = DoRefund(sInfo.Charge, amount);
                            
                            res = resEx.State == BusinessErrorState.Success;
                            
                            break;
                        }
                    }
                }

                res = true;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = false;
            }

            return res;
        }

        public bool BillAsUpsell(decimal amount, long? billingID)
        {
            bool res = false;

            Billing billing = dao.Load<Billing>(billingID);

            if (billingID != null)
            {
                BillingSubscription bs = subscriptionService.GetBillingSubscriptionByBilling(billingID);

                if (bs != null)
                {
                    Subscription s = subscriptionService.Load<Subscription>(bs.SubscriptionID);

                    var resEx = BillAsUpsell(billing, (int)s.ProductID, null, amount, null, null);

                    res = resEx.State == BusinessErrorState.Success;
                }
            }

            return res;
        }

        private decimal GetAmountWithRefunds(SaleFullInfo sInfo, long? saleID)
        {
            decimal? res = 0;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select coalesce(sum(Amount), 0) from ChargeHistoryEx ch
                                                    inner join SaleRefund sr on sr.ChargeHistoryID=ch.ChargeHistoryID
                                                    where sr.SaleID=@SaleID
                                                    and ch.Success=1");
                q.Parameters.Add("@SaleID", MySqlDbType.UInt64).Value = saleID;
                res = dao.ExecuteScalar<decimal>(q);
                res += sInfo.TotalAmount;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = 0;
            }
            return res ?? 0;
        }


    }
}
