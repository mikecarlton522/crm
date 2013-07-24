using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business.Flow
{
    public class MIDBalancing : BaseService
    {
        public MIDBalancing()
        {
            IList<AssertigyMIDCapView2> midListWithMonthlyRevenue = null;
            IList<NMICompany> midCompanyList = null;
            IList<NMICompanyMID> midCompanyMap = null;
            IList<AssertigyMIDPaymentType> midPaymentTypeMap = null;
            IList<NMIMerchantAccountProduct> productMap = null;
            IList<ClosedMIDRouting> routingMap = null;
            IList<ClosedMIDQueue> queueMap = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select IfNull(m.MonthlyCap - IfNull(sum(coalesce(chCur.CurrencyAmount, ch.Amount)), 0.0), 0.0) as RemainingCap, m.* from AssertigyMID m
                    left join ChargeHistoryEx ch on ch.MerchantAccountID = m.AssertigyMIDID and ch.Success = 1 and ch.Amount > 0 and ch.ChargeDate >= @monthStartDate
                    left join ChargeHistoryExCurrency chCur on chCur.ChargeHistoryID = ch.ChargeHistoryID
                    group by m.AssertigyMIDID
                ");
                q.Parameters.Add("@monthStartDate", MySqlDbType.Timestamp).Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1, 0, 0, 0);
                midListWithMonthlyRevenue = dao.Load<AssertigyMIDCapView2>(q);

                q = new MySqlCommand(@"
                    select * from NMICompany
                ");
                midCompanyList = dao.Load<NMICompany>(q);

                q = new MySqlCommand(@"
                    select * from NMICompanyMID
                ");
                midCompanyMap = dao.Load<NMICompanyMID>(q);

                q = new MySqlCommand(@"
                    select * from AssertigyMIDPaymentType
                ");
                midPaymentTypeMap = dao.Load<AssertigyMIDPaymentType>(q);

                q = new MySqlCommand(@"
                    select * from NMIMerchantAccountProduct
                ");
                productMap = dao.Load<NMIMerchantAccountProduct>(q);

                q = new MySqlCommand(@"
                    select * from ClosedMIDRouting
                ");
                routingMap = dao.Load<ClosedMIDRouting>(q);

                q = new MySqlCommand(@"
                    select * from ClosedMIDQueue
                ");
                queueMap = dao.Load<ClosedMIDQueue>(q);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            Init(midListWithMonthlyRevenue, midCompanyList, midCompanyMap,
                midPaymentTypeMap, productMap,
                routingMap, queueMap);
        }

        public MIDBalancing(IList<AssertigyMIDCapView2> midListWithMonthlyRevenue, IList<NMICompany> midCompanyList, IList<NMICompanyMID> midCompanyMap,
            IList<AssertigyMIDPaymentType> midPaymentTypeMap, IList<NMIMerchantAccountProduct> productMap,
            IList<ClosedMIDRouting> routingMap, IList<ClosedMIDQueue> queueMap)
        {
            Init(midListWithMonthlyRevenue, midCompanyList, midCompanyMap,
                midPaymentTypeMap, productMap,
                routingMap, queueMap);
        }

        protected void Init(IList<AssertigyMIDCapView2> midListWithMonthlyRevenue, IList<NMICompany> midCompanyList, IList<NMICompanyMID> midCompanyMap,
            IList<AssertigyMIDPaymentType> midPaymentTypeMap, IList<NMIMerchantAccountProduct> productMap,
            IList<ClosedMIDRouting> routingMap, IList<ClosedMIDQueue> queueMap)
        {
            if (midListWithMonthlyRevenue == null)
                throw new ArgumentNullException("midListWithMonthlyRevenue");
            if (midCompanyList == null)
                throw new ArgumentNullException("midCompanyList");
            if (midCompanyMap == null)
                throw new ArgumentNullException("midCompanyMap");
            if (midPaymentTypeMap == null)
                throw new ArgumentNullException("midPaymentTypeMap");
            if (productMap == null)
                throw new ArgumentNullException("midPaymentTypeMap");
            if (routingMap == null)
                throw new ArgumentNullException("routingMap");
            if (queueMap == null)
                throw new ArgumentNullException("queueMap");

            MIDListWithMonthlyRevenue = midListWithMonthlyRevenue;
            MIDCompanyList = midCompanyList;
            MIDCompanyMap = midCompanyMap;
            MIDPaymentTypeMap = midPaymentTypeMap;
            ProductMap = productMap;
            RoutingMap = routingMap;
            QueueMap = queueMap;
        }

        public IList<AssertigyMIDCapView2> MIDListWithMonthlyRevenue { get; set; }
        public IList<NMICompany> MIDCompanyList { get; set; }
        public IList<NMICompanyMID> MIDCompanyMap { get; set; }
        public IList<AssertigyMIDPaymentType> MIDPaymentTypeMap { get; private set; }
        public IList<NMIMerchantAccountProduct> ProductMap { get; private set; }
        public IList<ClosedMIDRouting> RoutingMap { get; private set; }
        public IList<ClosedMIDQueue> QueueMap { get; private set; }

        public BusinessError<Set<NMICompany, AssertigyMID>> ChooseRandom(int productID, int paymentTypeID, decimal? amount)
        {
            var res2 = ChooseRandomInternal(productID, paymentTypeID, amount);
            var res = new BusinessError<Set<NMICompany, AssertigyMID>>()
            {
                State = res2.State,
                ErrorMessage = res2.ErrorMessage,
                ReturnValue = null
            };
            if (res2.ReturnValue != null)
            {
                res.ReturnValue = new Set<NMICompany, AssertigyMID>()
                {
                    Value1 = res2.ReturnValue.Value1,
                    Value2 = res2.ReturnValue.Value2.MID
                };
            }
            return res;
        }

        public virtual BusinessError<Set<NMICompany, AssertigyMIDCapView2>> ChooseRandomInternal(int productID, int paymentTypeID, decimal? amount)
        {
            var res = new BusinessError<Set<NMICompany, AssertigyMIDCapView2>>()
            {
                State = BusinessErrorState.Error,
                ErrorMessage = string.Format("Can't find MID for ProductID = {0}, PaymentTypeID = {1}, Requested Amount = {2}.", productID, paymentTypeID, amount),
                ReturnValue = null
            }; 
            
            try
            {
                if (amount == null)
                    amount = 0M;

                var q = (
                    from am in MIDListWithMonthlyRevenue
                    join cam in MIDCompanyMap on am.MID.AssertigyMIDID.Value equals cam.AssertigyMIDID.Value
                    join c in MIDCompanyList on cam.NMICompanyID.Value equals c.NMICompanyID.Value
                    join mpt in MIDPaymentTypeMap on am.MID.AssertigyMIDID.Value equals mpt.AssertigyMIDID.Value
                    join pm in ProductMap on am.MID.AssertigyMIDID.Value equals pm.AssertigyMIDID.Value
                    where
                        !(am.MID.Deleted ?? false) &&
                        am.MID.Visible.Value &&
                        c.Active.Value &&
                        mpt.PaymentTypeID == paymentTypeID &&
                        pm.ProductID == productID &&
                        pm.UseForTrial.Value &&
                        am.RemainingCap > amount
                    select new { Company = c, MID = am }
                    ).Distinct();

                var availableList = q.Select(i => new Set<NMICompany, AssertigyMIDCapView2>()
                {
                    Value1 = i.Company,
                    Value2 = i.MID
                }).ToList();

                Set<NMICompany, AssertigyMIDCapView2> res2 = null;
                if (availableList.Count > 0)
                {
                    res2 = Utility.GetRandom<Set<NMICompany, AssertigyMIDCapView2>>(new Random(), availableList, i => i.Value2.RemainingCap.Value);
                }

                if (res2 != null)
                {
                    res.State = BusinessErrorState.Success;
                    res.ErrorMessage = string.Empty;
                    res.ReturnValue = res2;
                }
                else
                {
                    res.ErrorMessage = string.Format("Can't find MID for ProductID = {0}, PaymentTypeID = {1}, Requested Amount = {2}.", productID, paymentTypeID, amount);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);

            }
            return res;
        }

        public BusinessError<Set<NMICompany, AssertigyMID>> ChooseForExisting(int? existingAssertigyMIDID, int productID, int paymentTypeID, decimal? amount)
        {
            var res2 = ChooseForExistingInternal(existingAssertigyMIDID, productID, paymentTypeID, amount);
            var res = new BusinessError<Set<NMICompany, AssertigyMID>>()
            {
                State = res2.State,
                ErrorMessage = res2.ErrorMessage,
                ReturnValue = null
            };
            if (res2.ReturnValue != null)
            {
                res.ReturnValue = new Set<NMICompany, AssertigyMID>()
                {
                    Value1 = res2.ReturnValue.Value1,
                    Value2 = res2.ReturnValue.Value2.MID
                };
            }
            return res;
        }

        protected virtual BusinessError<Set<NMICompany, AssertigyMIDCapView2>> ChooseForExistingInternal(int? existingAssertigyMIDID, int productID, int paymentTypeID, decimal? amount)
        {
            BusinessError<Set<NMICompany, AssertigyMIDCapView2>> res = new BusinessError<Set<NMICompany, AssertigyMIDCapView2>>(
                null,
                BusinessErrorState.Error,
                string.Format("Can't find MID({0})", existingAssertigyMIDID));

            try 
	        {	        
                if (amount == null)
                {
                    amount = 0M;
                }

                Set<NMICompany, AssertigyMIDCapView2> resMID = null;
                if (existingAssertigyMIDID != null)
                {
                    var q = (
                        from am in MIDListWithMonthlyRevenue
                        join cam in MIDCompanyMap on am.MID.AssertigyMIDID.Value equals cam.AssertigyMIDID.Value
                        join c in MIDCompanyList on cam.NMICompanyID.Value equals c.NMICompanyID.Value
                        where
                            am.MID.AssertigyMIDID == existingAssertigyMIDID
                        select new { Company = c, MID = am }
                        ).Distinct();

                    resMID = q.Select(i => new Set<NMICompany, AssertigyMIDCapView2>()
                     {
                         Value1 = i.Company,
                         Value2 = i.MID
                     }).ToList().FirstOrDefault();
                }

                if (resMID != null)
                {
                    //1. Route MID
                    var q2 = (
                        from am in MIDListWithMonthlyRevenue
                        join cam in MIDCompanyMap on am.MID.AssertigyMIDID.Value equals cam.AssertigyMIDID.Value
                        join c in MIDCompanyList on cam.NMICompanyID.Value equals c.NMICompanyID.Value
                        join r in RoutingMap on am.MID.AssertigyMIDID.Value equals r.AssertigyMIDID
                        where
                            r.ClosedMIDID == existingAssertigyMIDID &&
                            r.PaymentTypeID == paymentTypeID &&
                            r.Percentage > 0
                        select new { Company = c, MID = am, RoutingRule = r }
                        ).Distinct();

                    var routingMIDs = q2.Select(i => new Set<NMICompany, AssertigyMIDCapView2, ClosedMIDRouting>()
                    {
                        Value1 = i.Company,
                        Value2 = i.MID,
                        Value3 = i.RoutingRule
                    }).ToList();

                    if (routingMIDs.Count > 0)
                    {
                        //if total percentage < 100 add self MID
                        int totalPercentage = routingMIDs.Sum(i => i.Value3.Percentage.Value);
                        if (totalPercentage < 100)
                        {
                            routingMIDs.Add(new Set<NMICompany, AssertigyMIDCapView2, ClosedMIDRouting>()
                            {
                                Value1 = resMID.Value1,
                                Value2 = resMID.Value2,
                                Value3 = new ClosedMIDRouting()
                                {
                                    Percentage = 100 - totalPercentage
                                }
                            });
                        }
                        resMID = Utility.GetRandom<Set<NMICompany, AssertigyMIDCapView2, ClosedMIDRouting>>(new Random(),
                            routingMIDs,
                            i => i.Value3.Percentage.Value);
                    }

                    //2. Check if MID available and accepts PaymentType, if not then Random MID will be choosen
                    if (resMID.Value1.Active.Value &&
                        !(resMID.Value2.MID.Deleted ?? false) &&
                        resMID.Value2.MID.Visible.Value &&
                        MIDPaymentTypeMap.Where(i => i.AssertigyMIDID == resMID.Value2.MID.AssertigyMIDID && i.PaymentTypeID == paymentTypeID).Count() > 0)
                    {
                    }
                    else
                    {
                        resMID = ChooseRandomInternal(productID, paymentTypeID, amount).ReturnValue;
                        if (resMID == null)
                        {
                            res.ErrorMessage = string.Format("Can't find MID for ProductID = {0}, PaymentTypeID = {1}, Requested Amount = {2}.", productID, paymentTypeID, amount);
                        }
                    }
                }
                else
                {
                    resMID = ChooseRandomInternal(productID, paymentTypeID, amount).ReturnValue;
                    if (resMID == null)
                    {
                        res.ErrorMessage = string.Format("Can't find MID for ProductID = {0}, PaymentTypeID = {1}, Requested Amount = {2}.", productID, paymentTypeID, amount);
                    }
                }

                if (resMID != null)
                {
                    //3. Check if MID is queued
                    if (ProductMap.Where(i => i.ProductID == productID && i.AssertigyMIDID == resMID.Value2.MID.AssertigyMIDID && i.QueueRebills.Value).Count() > 0)
                    {
                        res.ErrorMessage = string.Format("MID({0}) is Queued for ProductID({1}).", resMID.Value2.MID.AssertigyMIDID, productID);
                        res.ReturnValue = resMID;
                    }
                    else if (QueueMap.Where(i => i.PaymentTypeID == paymentTypeID && i.ClosedMIDID == resMID.Value2.MID.AssertigyMIDID && i.Queued.Value).Count() > 0)
                    {
                        res.ErrorMessage = string.Format("MID({0}) is Queued for PaymentTypeID({1}).", resMID.Value2.MID.AssertigyMIDID, paymentTypeID);
                        res.ReturnValue = resMID;
                    }
                    else
                    {
                        //4. Check CAP
                        if (resMID.Value2.RemainingCap < amount)
                        {
                            res.ErrorMessage = string.Format("MID({0}) is at Cap.", resMID.Value2.MID.AssertigyMIDID);
                            res.ReturnValue = resMID;
                        }
                        else
                        {
                            res.State = BusinessErrorState.Success;
                            res.ErrorMessage = string.Empty;
                            res.ReturnValue = resMID;
                        }
                    }
                }
	        }
	        catch (Exception ex)
	        {
                logger.Error(ex);
	        }

            return res;
        }
    }
}
