using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class MerchantService : BaseService
    {
        private SaleService saleService { get { return new SaleService(); } }

        public void UpdateAssertigyMIDDailyCap(AssertigyMID assertigyMID, decimal amount)
        {
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select m.* from AssertigyMIDDailyCap m " +
                    "where m.CreateDT = @today and m.AssertigyMIDID = @assertigyMIDID");
                q.Parameters.Add("@assertigyMIDID", MySqlDbType.Int32).Value = assertigyMID.AssertigyMIDID;
                q.Parameters.Add("@today", MySqlDbType.Timestamp).Value = DateTime.Today;

                AssertigyMIDDailyCap cap = dao.Load<AssertigyMIDDailyCap>(q).FirstOrDefault();
                if (cap == null)
                {
                    cap = new AssertigyMIDDailyCap();
                    cap.AssertigyMIDID = assertigyMID.AssertigyMIDID;
                    cap.CreateDT = DateTime.Today;
                }

                cap.TotalAmount = Utility.Add(cap.TotalAmount, (double)amount);

                dao.Save<AssertigyMIDDailyCap>(cap);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public int? DeterminePaymentType(Billing b)
        {
            int? res = null;
            try
            {
                string decrypedCard = b.CreditCardCnt.DecryptedCreditCard;
                if (!string.IsNullOrEmpty(decrypedCard))
                {
                    if (decrypedCard[0] == '3')
                    {
                        res = PaymentTypeEnum.AmericanExpress;
                    }
                    else if (decrypedCard[0] == '4')
                    {
                        res = PaymentTypeEnum.Visa;
                    }
                    else if (decrypedCard[0] == '5')
                    {
                        res = PaymentTypeEnum.Mastercard;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public Set<NMICompany, AssertigyMID> ChooseRandomNMIMerchantAccount(int productID, Billing b, decimal amount)
        {
            Set<NMICompany, AssertigyMID> res = null;
            try
            {
                int? paymentType = DeterminePaymentType(b);
                string paymentTypeFilter = "";
                string forbiddenTrialFilter = "";
                if (paymentType != null)
                {
                    paymentTypeFilter = "inner join AssertigyMIDPaymentType mpt on mpt.AssertigyMIDID = m.AssertigyMIDID and mpt.PaymentTypeID = " + paymentType.ToString() + " ";
                    forbiddenTrialFilter = "and not exists (select * from AssertigyMIDForbidTrial amForbid where amForbid.AssertigyMIDID = m.AssertigyMIDID and amForbid.PaymentTypeID = mpt.PaymentTypeID) ";
                }
                //Get all valid AssertigyMID
                MySqlCommand q = new MySqlCommand(@"
                    select round(m.MonthlyCap - IfNull(sum(ch.Amount), 0.0)) as RemainingCap, m.* from AssertigyMID m " +
                    paymentTypeFilter + @"
                    inner join NMIMerchantAccountProduct p on m.AssertigyMIDID = p.AssertigyMIDID
                    left join ChargeHistoryEx ch on ch.MerchantAccountID = m.AssertigyMIDID and ch.Success = 1 and ch.Amount > 0 and ch.ChargeDate >= @monthStartDate
                    where p.UseForTrial = 1 and p.ProductID = @productID and m.Visible = 1 " +
                    forbiddenTrialFilter + @"
                    group by m.AssertigyMIDID
                    having RemainingCap > @amount
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@monthStartDate", MySqlDbType.Timestamp).Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1, 0, 0, 0);
                q.Parameters.Add("@amount", MySqlDbType.Decimal).Value = amount;

                IList<AssertigyMIDCapView> availableAssertigyMID = dao.Load<AssertigyMIDCapView>(q);

                AssertigyMID variateAssertigyMID = Utility.GetRandom<AssertigyMID>(new Random(),
                    availableAssertigyMID.Select(item => new KeyValuePair<long, AssertigyMID>(Convert.ToInt64(item.RemainingCap), item.AssertigyMID)));
                if (variateAssertigyMID == null)
                {
                    throw new Exception(string.Format("Can't choose AssertigyMID for Product({0})", productID));
                }

                NMICompany nmiCompany = GetNMICompanyByAssertigyMID(variateAssertigyMID.AssertigyMIDID);
                if (nmiCompany == null)
                {
                    throw new Exception(string.Format("Can't find NMICompany for AssertigyMID({0})", variateAssertigyMID.AssertigyMIDID));
                }

                res = new Set<NMICompany, AssertigyMID>()
                {
                    Value1 = nmiCompany,
                    Value2 = variateAssertigyMID
                };
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public Set<NMICompany, AssertigyMID> ChooseLastSuccessfulNMIMerchantAccount(int productID, Billing b, decimal amount)
        {
            Set<NMICompany, AssertigyMID> res = null;
            try
            {
                int? paymentType = b.CreditCardCnt.TryGetCardType();
                string paymentTypeFilter = "";
                if (paymentType != null)
                {
                    paymentTypeFilter = "inner join AssertigyMIDPaymentType mpt on mpt.AssertigyMIDID = m.AssertigyMIDID and mpt.PaymentTypeID = " + paymentType.ToString() + " ";
                }
                //Get all valid AssertigyMID
                MySqlCommand q = new MySqlCommand(@"
                    select m.* from AssertigyMID m " +
                    paymentTypeFilter + @"
                    inner join NMIMerchantAccountProduct p on p.AssertigyMIDID = m.AssertigyMIDID
                    inner join ChargeHistoryEx ch on ch.MerchantAccountID = m.AssertigyMIDID
                    inner join ChargeHistoryInvoice chi on chi.ChargeHistoryID = ch.ChargeHistoryID
                    inner join Invoice i on i.InvoiceID = chi.InvoiceID
                    inner join Orders o on o.OrderID = i.OrderID
                    where ch.Amount >= 0 and ch.Success = 1 and p.ProductID = @productID and m.Visible = 1 and o.ProductID = @productID and o.BillingID = @billingID
                    order by ch.CHargeHistoryID desc
                    limit 1
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;

                AssertigyMID assertigyMID = dao.Load<AssertigyMID>(q).FirstOrDefault();
                if (assertigyMID != null)
                {
                    NMICompany nmiCompany = GetNMICompanyByAssertigyMID(assertigyMID.AssertigyMIDID);
                    if (nmiCompany == null)
                    {
                        throw new Exception(string.Format("Can't find NMICompany for AssertigyMID({0})", assertigyMID.AssertigyMIDID));
                    }

                    res = new Set<NMICompany, AssertigyMID>()
                    {
                        Value1 = nmiCompany,
                        Value2 = assertigyMID
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public NMICompany GetNMICompanyByAssertigyMID(int? assertigyMIDID)
        {
            NMICompany res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select c.* from NMICompany c " +
                    "inner join NMICompanyMID m on c.NMICompanyID = m.NMICompanyID " +
                    "where m.AssertigyMIDID = @assertigyMIDID");
                q.Parameters.Add("@assertigyMIDID", MySqlDbType.Int32).Value = assertigyMIDID;

                res = dao.Load<NMICompany>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public Set<NMICompany, AssertigyMID> GetMerchantOfLastSuccessfulCharge(long billingID)
        {
            Set<NMICompany, AssertigyMID> res = null;
            try
            {
                ChargeHistoryEx chargeHistory = saleService.GetLastSuccessfulCharge(billingID);
                if (chargeHistory != null)
                {
                    res = new Set<NMICompany, AssertigyMID>();
                    res.Value2 = EnsureLoad<AssertigyMID>(chargeHistory.MerchantAccountID);
                    res.Value1 = GetNMICompanyByAssertigyMID(res.Value2.AssertigyMIDID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public Set<NMICompany, AssertigyMID> GetMerchantOfLastSuccessfulCharge(long billingID, int productID)
        {
            Set<NMICompany, AssertigyMID> res = null;
            try
            {
                ChargeHistoryEx chargeHistory = saleService.GetLastSuccessfulCharge(billingID, productID);
                if (chargeHistory != null)
                {
                    res = new Set<NMICompany, AssertigyMID>();
                    res.Value2 = EnsureLoad<AssertigyMID>(chargeHistory.MerchantAccountID);
                    res.Value1 = GetNMICompanyByAssertigyMID(res.Value2.AssertigyMIDID);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        //public MerchantAccount ChooseMerchantAccount(int productID)
        //{
        //    MerchantAccount res = null;
        //    try
        //    {
        //        //Get all valid merchant accounts
        //        MySqlCommand q = new MySqlCommand("select map.* from MerchantAccountProduct map " +
        //            "inner join MerchantAccount ma on ma.MerchantAccountID = map.MerchantAccountID " +
        //            "where map.ProductID = @productID and map.Percentage <> 0 and ma.Active = 1");
        //        q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

        //        IList<MerchantAccountProduct> availableMerchantList = dao.Load<MerchantAccountProduct>(q);

        //        MerchantAccountProduct variateMerchantAccount = Utility.GetRandom<MerchantAccountProduct>(new Random(),
        //            availableMerchantList.Select(item => new KeyValuePair<int, MerchantAccountProduct>(item.Percentage.Value, item)));

        //        if (variateMerchantAccount == null)
        //        {
        //            throw new Exception(string.Format("Can't choose MerchantAccount for Product({0})", productID));
        //        }

        //        res = EnsureLoad<MerchantAccount>(variateMerchantAccount.MerchantAccountID);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(GetType(), ex);
        //        res = null;
        //    }
        //    return res;
        //}

        //private void UpdateMerchantAccountDailyCap(MerchantAccount merchantAccount, decimal amount)
        //{
        //    try
        //    {
        //        dao.BeginTransaction();

        //        MySqlCommand cmd = new MySqlCommand("update MerchantAccountDailyCap " +
        //            "set Amount = Amount + @amount " +
        //            "where MerchantAccountID = @merchantAccountID and currentDate = @today");
        //        cmd.Parameters.Add("@amount", MySqlDbType.Decimal).Value = amount;
        //        cmd.Parameters.Add("@merchantAccountID", MySqlDbType.Int32).Value = merchantAccount.MerchantAccountID;
        //        cmd.Parameters.Add("@today", MySqlDbType.Timestamp).Value = DateTime.Today;

        //        dao.ExecuteNonQuery(cmd);

        //        dao.CommitTransaction();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(GetType(), ex);
        //        dao.RollbackTransaction();
        //    }
        //}

        public IEnumerable<IGrouping<MIDCategory, AssertigyMID>> GetMIDs()
        {
            IEnumerable<IGrouping<MIDCategory, AssertigyMID>> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(
                    " select g.* from MIDCategory g");
                IList<MIDCategory> groups = dao.Load<MIDCategory>(q);
                groups.Add(new MIDCategory(){
                    MIDCategoryID = -1,
                    DisplayName = "Not currently assigned"
                });

                q = new MySqlCommand(
                    " select a.* from AssertigyMID a where a.Visible = 1 order by a.MIDCategoryID");
                IList<AssertigyMID> mids = dao.Load<AssertigyMID>(q);

                res = mids.GroupBy(i => groups.Where(g => g.MIDCategoryID == (i.MIDCategoryID ?? new Nullable<int>(-1))).FirstOrDefault());
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<AssertigyMIDAmountView> GetProjectedRevenueByMIDs(DateTime startDate, DateTime endDate, IList<decimal> attritionList)
        {
            IList<AssertigyMIDAmountView> res = new List<AssertigyMIDAmountView>();
            try
            {
                string sql = @"
                    select Sum(BillAmount) as Amount, am.AssertigyMIDID, am.MID, am.DisplayName from
		            (
		            select bs.BillingSubscriptionID, Max(ch.ChargeHistoryID) as LastChargeHistoryID, (IfNull(s.RegularBillAmount, 0.0) + IfNull(s.RegularShipping, 0.0)) as BillAmount from BillingSubscription bs
		            inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID
		            inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
		            inner join Billing b on b.BillingID = bs.BillingID
		            where ch.Amount > 0 and ch.Success = 1
		            and s.Recurring = 1
		            and bs.NextBillDate >= '2010-01-01 00:00:00'
		            and bs.StatusTID = 1 and DATE_ADD(bs.NextBillDate, INTERVAL @rebillNumber*IfNull(s.RegularInterim, 1000) DAY) between @startDate and @endDate
		            group by bs.BillingSubscriptionID
		            ) bill
		            inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bill.LastChargeHistoryID
		            inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
		            group by am.AssertigyMIDID
		            order by am.DisplayName
                ";
                //30 - limit of rebill number
                decimal currentAttrition = 1.00M;
                for (int rebillNumber = 0; rebillNumber < 30; rebillNumber++)
                {
                    decimal currentRebillAttrition = 1.00M;
                    if (attritionList != null && attritionList.Count > rebillNumber)
                    {
                        currentRebillAttrition = 1.00M - attritionList[rebillNumber];
                    }
                    else if (attritionList != null && attritionList.Count > 0)
                    {
                        currentRebillAttrition = 1.00M - attritionList.Last();
                    }
                    currentAttrition = currentAttrition * currentRebillAttrition;

                    MySqlCommand q = new MySqlCommand(sql);
                    q.Parameters.Add("@rebillNumber", MySqlDbType.Int32).Value = rebillNumber;
                    q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate;
                    q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate;
                    var res2 = dao.Load<AssertigyMIDAmountView>(q);
                    foreach (var item in res2)
                    {
                        item.Amount = item.Amount * currentAttrition;
                        var existingItem = res.FirstOrDefault(i => i.MID.AssertigyMIDID == item.MID.AssertigyMIDID);
                        if (existingItem == null)
                        {
                            res.Add(item);
                        }
                        else
                        {
                            existingItem.Amount += item.Amount;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }
            return res;
        }

        private const decimal REBILL_ATTRITION_1 = 0.35M;
        private const decimal REBILL_ATTRITION_2 = 0.35M;
        private const decimal REBILL_ATTRITION_3_AND_FOLLOW = 0.40M;
        public void UpdateAgrProjectedRevenuesNextMonth()
        {
            //todo: config
            
            try
            {
                dao.BeginTransaction();

                DateTime nextMonth = DateTime.Today.AddMonths(1);
                DateTime nextNextMonth = nextMonth.AddMonths(1);

                MySqlCommand q = new MySqlCommand(@"
                    delete from AgrMIDProjectedRevenue
                    where Year = @year and Month = @month
                ");
                q.Parameters.Add("@year",  MySqlDbType.Int32).Value = nextMonth.Year;
                q.Parameters.Add("@month", MySqlDbType.Int32).Value = nextMonth.Month;
                dao.ExecuteNonQuery(q);

                foreach (var item in GetProjectedRevenueByMIDs(new DateTime(nextMonth.Year, nextMonth.Month, 1, 0, 0, 0),
                    new DateTime(nextNextMonth.Year, nextNextMonth.Month, 1, 0, 0, 0),
                    new List<decimal>(){REBILL_ATTRITION_1, REBILL_ATTRITION_2, REBILL_ATTRITION_3_AND_FOLLOW}))
                {
                    AgrMIDProjectedRevenue revenue = new AgrMIDProjectedRevenue();
                    revenue.Year = nextMonth.Year;
                    revenue.Month = nextMonth.Month;
                    revenue.MerchantAccountID = item.MID.AssertigyMIDID;
                    revenue.ProjectedRevenue = item.Amount;
                    dao.Save(revenue);
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
}
