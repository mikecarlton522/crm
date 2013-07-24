using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Business.Gateways.AragonWhiteLabels;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class LeadService : BaseService
    {
        public void SaveLeadPost(LeadPost leadPost)
        {
            try
            {
                dao.Save<LeadPost>(leadPost);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public List<LeadPartnerConfigValue> GetLeadPartnerConfigValues(int? leadPartnerID, int? productID, int? leadTypeID)
        {
            List<LeadPartnerConfigValue> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select * from LeadPartnerConfigValue lc 
                                                    where lc.LeadPartnerID=@LeadPartnerID
                                                    and lc.LeadTypeID=@LeadTypeID
                                                    and lc.ProductID=@ProductID");
                q.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = leadPartnerID;
                q.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = leadTypeID;
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<LeadPartnerConfigValue>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<LeadPartnerConfigValue>();
            }
            return res;
        }

        public IList<LeadPartnerConfigValue> GetLeadPartnerConfigValues(int productID)
        {
            IList<LeadPartnerConfigValue> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("Select * from LeadPartnerConfigValue lc where lc.ProductID=@ProductID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<LeadPartnerConfigValue>(q).ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = new List<LeadPartnerConfigValue>();
            }
            return res;
        }

        public IList<LeadRoutingView> GetRoutingRules()
        {
            IList<LeadRoutingView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select p.ProductID, lr.LeadTypeID, lr.LeadPartnerID, lr.Percentage, p.ProductName as ProductName, lp.DisplayName as LeadPartnerName from Product p
                    left join LeadRouting lr on lr.ProductID = p.ProductID
                    left join LeadPartner lp on lp.LeadPartnerID = lr.LeadPartnerID
                    where p.ProductID > 0 and p.ProductIsActive = 1
                    order by p.ProductID asc, lr.Percentage desc
                    ");
                res = dao.Load<LeadRoutingView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<CampaignLeadRoutingView> GetRoutingRulesForCampaign()
        {
            IList<CampaignLeadRoutingView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select c.CampaignID, p.ProductID, clr.LeadTypeID, clr.LeadPartnerID, clr.Percentage, p.ProductName, lp.DisplayName as LeadPartnerName from CampaignLeadRouting clr
                    inner join LeadPartner lp on lp.LeadPartnerID = clr.LeadPartnerID
                    inner join Campaign c on c.CampaignID = clr.CampaignID
                    inner join Subscription s on s.SubscriptionID = c.SubscriptionID
                    inner join Product p on p.ProductID = s.ProductID
                    union
                    select c.CampaignID, p.ProductID, clr.LeadTypeID, clr.LeadPartnerID, clr.Percentage, p.ProductName, lp.DisplayName as LeadPartnerName from CampaignLeadRouting clr
                    inner join LeadPartner lp on lp.LeadPartnerID = clr.LeadPartnerID
                    inner join Campaign c on c.CampaignID = clr.CampaignID
                    inner join CampaignRecurringPlan crp on crp.CampaignID = c.CampaignID
                    inner join RecurringPlan rp on rp.RecurringPlanID = crp.RecurringPlanID
                    inner join Product p on p.ProductID = rp.ProductID
                    ");
                res = dao.Load<CampaignLeadRoutingView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<LeadRouting> GetRoutingRules(int productID)
        {
            IList<LeadRouting> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select lr.* from LeadRouting lr
                    where lr.ProductID = @productID
                    ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<LeadRouting>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<LeadPost> GetLeadPostListNotCompleted(int? leadTypeID)
        {
            IList<LeadPost> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select lp.* from LeadPost lp
                    where lp.LeadTypeID = @leadTypeID
                    and lp.Completed = 0
                    ");
                q.Parameters.Add("@leadTypeID", MySqlDbType.Int32).Value = leadTypeID;
                res = dao.Load<LeadPost>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public LeadRouting GetLeadRouting(int? leadPartnerID, int? productID, int? leadTypeID)
        {
            LeadRouting res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select lr.* from LeadRouting lr
                    where lr.ProductID = @productID
                    and lr.LeadPartnerID = @leadPartnerID
                    and lr.LeadTypeID = @leadTypeID
                    ");
                q.Parameters.Add("@leadPartnerID", MySqlDbType.Int32).Value = leadPartnerID;
                q.Parameters.Add("@leadTypeID", MySqlDbType.Int32).Value = leadTypeID;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                res = dao.Load<LeadRouting>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public Billing GetBillingByRegistrationID(long? registrationID)
        {
            Billing res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select b.* from Billing b
                    where b.RegistrationID = @registrationID
                    ");
                q.Parameters.Add("@registrationID", MySqlDbType.Int32).Value = registrationID;
                res = dao.Load<Billing>(q).SingleOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<LeadPartner> GetPartnerList()
        {
            IList<LeadPartner> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from LeadPartner
                    order by LeadPartnerID asc
                    ");
                res = dao.Load<LeadPartner>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<LeadType> GetLeadTypeList()
        {
            IList<LeadType> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from LeadType
                    order by LeadTypeID asc
                    ");
                res = dao.Load<LeadType>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public bool IsAlredySent(int? productID, long? registrationID, int? leadTypeID, int? leadPartnerID)
        {
            int res = 0;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select Count(*) from LeadPost 
                                                   Where RegistrationID=@RegistrationID AND LeadTypeID=@LeadTypeID 
                                                   AND ProductID=@ProductID AND LeadPartnerID=@LeadPartnerID");
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = leadTypeID;
                q.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = leadPartnerID;
                q.Parameters.Add("@RegistrationID", MySqlDbType.Int64).Value = registrationID;
                res = dao.ExecuteScalar<int>(q).Value;
                if (res > 0)
                    return true;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return false;
        }

        public IList<Billing> GetConfirmList(int? productID, DateTime startDate, DateTime endDate, bool phoneCheck)
        {
            IList<Billing> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select b.* from Billing b 
                                                   inner join BillingSubscription bs on bs.BillingID=b.BillingID
                                                   inner join Subscription s on bs.SubscriptionID=s.SubscriptionID
                                                   where bs.StatusTID=1 and s.ProductID=@ProductID
                                                   AND b.CreateDT Between @StartDate and @EndDate
                                                   union
                                                   select b.* from Billing b 
                                                   inner join Orders o on o.BillingID=b.BillingID
                                                   where o.OrderStatus=1 and o.ProductID=@ProductID
                                                   AND b.CreateDT Between @StartDate and @EndDate");
                q.CommandTimeout = 9999;
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res = dao.Load<Billing>(q);

                if (phoneCheck)
                {
                    res = res.Distinct(new BillingPhoneComparer()).ToList();

                    for (int i = 0; i < res.Count; i++)
                    {
                        q = new MySqlCommand(@"Select count(*) from LeadPost where TelePhone=@Phone 
                                         and (LeadTypeID=1 or LeadTypeID=2) and ProductID=@ProductID");
                        q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                        q.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = res[i].Phone;
                        var count = dao.ExecuteScalar<int>(q);
                        if (count != 0)
                        {
                            res.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<Billing> GetDeclinedList(int? productID, DateTime startDate, DateTime endDate, bool phoneCheck)
        {
            IList<Billing> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select b.* from Billing b 
                                                   inner join BillingSubscription bs on bs.BillingID=b.BillingID
                                                   inner join BillingSubscriptionStatusHistory bssh on bssh.BillingSubscriptionID=bs.BillingSubscriptionID
                                                   inner join Subscription s on bs.SubscriptionID=s.SubscriptionID
                                                   where s.ProductID=@ProductID
                                                   AND bs.StatusTID=" + TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Declined.ToString() + " AND bssh.StatusTID=" + TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Declined.ToString() + @"
                                                   AND (bssh.CreateDT Between @StartDate and @EndDate)
                                                   union
                                                   select b.* from Billing b 
                                                   inner join Orders o on o.BillingID=b.BillingID
                                                   inner join OrderSale os on os.OrderID=o.OrderID
                                                   inner join OrderRecurringPlan orp on orp.SaleID=os.SaleID
                                                   inner join OrderRecurringPlanStatusHistory orph on orph.OrderRecurringPlanID=orp.OrderRecurringPlanID
                                                   where o.ProductID=@ProductID
                                                   AND orp.RecurringStatus=" + TrimFuel.Model.Enums.RecurringStatusEnum.Declined.ToString() + " AND orph.RecurringStatus=" + TrimFuel.Model.Enums.RecurringStatusEnum.Declined.ToString() + @"
                                                   AND (orph.CreateDT Between @StartDate and @EndDate)");
                q.CommandTimeout = 9999;
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res = dao.Load<Billing>(q);

                if (phoneCheck)
                {
                    res = res.Distinct(new BillingPhoneComparer()).ToList();
                    for (int i = 0; i < res.Count; i++)
                    {
                        q = new MySqlCommand(@"Select count(*) from LeadPost where TelePhone=@Phone 
                                         and LeadTypeID=3 and ProductID=@ProductID");
                        q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                        q.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = res[i].Phone;
                        var count = dao.ExecuteScalar<int>(q);
                        if (count != 0)
                        {
                            res.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<Billing> GetInactiveList(int? productID, DateTime startDate, DateTime endDate, bool phoneCheck)
        {
            IList<Billing> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select b.* from Billing b 
                                                   inner join BillingSubscription bs on bs.BillingID=b.BillingID
                                                   inner join BillingSubscriptionStatusHistory bssh on bssh.BillingSubscriptionID=bs.BillingSubscriptionID
                                                   inner join Subscription s on bs.SubscriptionID=s.SubscriptionID
                                                   where s.ProductID=@ProductID
                                                   AND bs.StatusTID=" + TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Inactive.ToString() + " AND bssh.StatusTID=" + TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Inactive.ToString() + @"
                                                   AND (bssh.CreateDT Between @StartDate and @EndDate)
                                                   union
                                                   select b.* from Billing b 
                                                   inner join Orders o on o.BillingID=b.BillingID
                                                   inner join OrderSale os on os.OrderID=o.OrderID
                                                   inner join OrderRecurringPlan orp on orp.SaleID=os.SaleID
                                                   inner join OrderRecurringPlanStatusHistory orph on orph.OrderRecurringPlanID=orp.OrderRecurringPlanID
                                                   where o.ProductID=@ProductID
                                                   AND orp.RecurringStatus=" + TrimFuel.Model.Enums.RecurringStatusEnum.Inactive.ToString() + " AND orph.RecurringStatus=" + TrimFuel.Model.Enums.RecurringStatusEnum.Inactive.ToString() + @"
                                                   AND (orph.CreateDT Between @StartDate and @EndDate)");
                q.CommandTimeout = 9999;
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res = dao.Load<Billing>(q);

                if (phoneCheck)
                {
                    res = res.Distinct(new BillingPhoneComparer()).ToList();
                    for (int i = 0; i < res.Count; i++)
                    {
                        q = new MySqlCommand(@"Select count(*) from LeadPost where TelePhone=@Phone 
                                         and LeadTypeID=3 and ProductID=@ProductID");
                        q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                        q.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = res[i].Phone;
                        var count = dao.ExecuteScalar<int>(q);
                        if (count != 0)
                        {
                            res.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<Registration> GetAbandonList(int? productID, DateTime startDate, DateTime endDate, bool phoneCheck)
        {
            IList<Registration> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select r.* from Registration r 
                    inner join Campaign c on r.CampaignID=c.CampaignID 
                    inner Join Subscription s on c.SubscriptionID=s.SubscriptionID 
                    Where s.ProductID=@ProductID and r.CreateDT Between @StartDate and @EndDate and not exists 
                        (Select b.Phone from Billing b 
                        inner Join BillingSubscription bs on b.BillingID=bs.BillingID 
                        where b.Phone = r.Phone
                        union
                        Select b.Phone from Billing b 
                        inner Join Orders o on o.BillingID=b.BillingID 
                        where o.OrderStatus = 1 and b.Phone = r.Phone)
                    union
                    select r.* from Registration r 
                    inner join Campaign c on r.CampaignID=c.CampaignID 
                    inner join CampaignRecurringPlan crp on crp.CampaignID = c.CampaignID
                    inner join RecurringPlan rp on rp.RecurringPlanID = crp.RecurringPlanID
                    Where rp.ProductID=@ProductID and r.CreateDT Between @StartDate and @EndDate and not exists 
                        (Select b.Phone from Billing b 
                        inner Join BillingSubscription bs on b.BillingID=bs.BillingID 
                        where b.Phone = r.Phone
                        union
                        Select b.Phone from Billing b 
                        inner Join Orders o on o.BillingID=b.BillingID 
                        where o.OrderStatus = 1 and b.Phone = r.Phone)");
                q.CommandTimeout = 9999;
                q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res = dao.Load<Registration>(q);

                if (phoneCheck)
                {
                    res = res.Distinct(new RegistrationPhoneComparer()).ToList();
                    for (int i = 0; i < res.Count; i++)
                    {
                        q = new MySqlCommand(@"Select count(*) from LeadPost where TelePhone=@Phone 
                                         and LeadTypeID=1 and ProductID=@ProductID");
                        q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                        q.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = res[i].Phone;
                        var count = dao.ExecuteScalar<int>(q);
                        if (count != 0)
                        {
                            res.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<Billing> GetConfirmListForCampaign(int? campaignID, int? productID, DateTime startDate, DateTime endDate, bool phoneCheck)
        {
            IList<Billing> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select b.* from Billing b 
                                                   inner join BillingSubscription bs on bs.BillingID=b.BillingID
                                                   where bs.StatusTID=1 and b.CampaignID=@CampaignID
                                                   and b.CreateDT Between @StartDate and @EndDate
                                                   union
                                                   select b.* from Billing b 
                                                   inner Join Orders o on o.BillingID=b.BillingID 
                                                   where o.OrderStatus=1 and b.CampaignID=@CampaignID
                                                   and b.CreateDT Between @StartDate and @EndDate");
                q.CommandTimeout = 9999;
                q.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = campaignID;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res = dao.Load<Billing>(q);

                if (phoneCheck)
                {
                    res = res.Distinct(new BillingPhoneComparer()).ToList();

                    for (int i = 0; i < res.Count; i++)
                    {
                        q = new MySqlCommand(@"Select count(*) from LeadPost where TelePhone=@Phone 
                                         and (LeadTypeID=1 or LeadTypeID=2) and ProductID=@ProductID");
                        q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                        q.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = res[i].Phone;
                        var count = dao.ExecuteScalar<int>(q);
                        if (count != 0)
                        {
                            res.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<Billing> GetDeclinedListForCampaign(int? campaignID, int? productID, DateTime startDate, DateTime endDate, bool phoneCheck)
        {
            IList<Billing> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select b.* from Billing b 
                                                   inner join BillingSubscription bs on bs.BillingID=b.BillingID
                                                   inner join BillingSubscriptionStatusHistory bssh on bssh.BillingSubscriptionID=bs.BillingSubscriptionID
                                                   where b.CampaignID=@CampaignID
                                                   AND bs.StatusTID=" + TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Declined.ToString() + " AND bssh.StatusTID=" + TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Declined.ToString() + @"
                                                   AND (bssh.CreateDT Between @StartDate and @EndDate)
                                                   union
                                                   select b.* from Billing b 
                                                   inner join Orders o on o.BillingID=b.BillingID
                                                   inner join OrderSale os on os.OrderID=o.OrderID
                                                   inner join OrderRecurringPlan orp on orp.SaleID=os.SaleID
                                                   inner join OrderRecurringPlanStatusHistory orph on orph.OrderRecurringPlanID=orp.OrderRecurringPlanID
                                                   where b.CampaignID=@CampaignID
                                                   AND orp.RecurringStatus=" + TrimFuel.Model.Enums.RecurringStatusEnum.Declined.ToString() + " AND orph.RecurringStatus=" + TrimFuel.Model.Enums.RecurringStatusEnum.Declined.ToString() + @"
                                                   AND (orph.CreateDT Between @StartDate and @EndDate)");
                q.CommandTimeout = 9999;
                q.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = campaignID;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res = dao.Load<Billing>(q);

                if (phoneCheck)
                {
                    res = res.Distinct(new BillingPhoneComparer()).ToList();

                    for (int i = 0; i < res.Count; i++)
                    {
                        q = new MySqlCommand(@"Select count(*) from LeadPost where TelePhone=@Phone 
                                         and LeadTypeID=3 and ProductID=@ProductID");
                        q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                        q.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = res[i].Phone;
                        var count = dao.ExecuteScalar<int>(q);
                        if (count != 0)
                        {
                            res.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<Billing> GetInactiveListForCampaign(int? campaignID, int? productID, DateTime startDate, DateTime endDate, bool phoneCheck)
        {
            IList<Billing> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select b.* from Billing b 
                                                   inner join BillingSubscription bs on bs.BillingID=b.BillingID
                                                   inner join BillingSubscriptionStatusHistory bssh on bssh.BillingSubscriptionID=bs.BillingSubscriptionID
                                                   where b.CampaignID=@CampaignID
                                                   AND bs.StatusTID=" + TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Inactive.ToString() + " AND bssh.StatusTID=" + TrimFuel.Model.Enums.BillingSubscriptionStatusEnum.Inactive.ToString() + @"
                                                   AND (bssh.CreateDT Between @StartDate and @EndDate)
                                                   union
                                                   select b.* from Billing b 
                                                   inner join Orders o on o.BillingID=b.BillingID
                                                   inner join OrderSale os on os.OrderID=o.OrderID
                                                   inner join OrderRecurringPlan orp on orp.SaleID=os.SaleID
                                                   inner join OrderRecurringPlanStatusHistory orph on orph.OrderRecurringPlanID=orp.OrderRecurringPlanID
                                                   where b.CampaignID=@CampaignID
                                                   AND orp.RecurringStatus=" + TrimFuel.Model.Enums.RecurringStatusEnum.Inactive.ToString() + " AND orph.RecurringStatus=" + TrimFuel.Model.Enums.RecurringStatusEnum.Inactive.ToString() + @"
                                                   AND (orph.CreateDT Between @StartDate and @EndDate)");
                q.CommandTimeout = 9999;
                q.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = campaignID;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res = dao.Load<Billing>(q);

                if (phoneCheck)
                {
                    res = res.Distinct(new BillingPhoneComparer()).ToList();
                    for (int i = 0; i < res.Count; i++)
                    {
                        q = new MySqlCommand(@"Select count(*) from LeadPost where TelePhone=@Phone 
                                         and LeadTypeID=3 and ProductID=@ProductID");
                        q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                        q.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = res[i].Phone;
                        var count = dao.ExecuteScalar<int>(q);
                        if (count != 0)
                        {
                            res.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<Registration> GetAbandonListForCampaign(int? campaignID, int? productID, DateTime startDate, DateTime endDate, bool phoneCheck)
        {
            IList<Registration> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select r.* from Registration r 
                    Where r.CampaignID=@CampaignID and r.CreateDT Between @StartDate and @EndDate and not exists 
                        (Select b.Phone from Billing b inner Join BillingSubscription bs on b.BillingID=bs.BillingID where b.Phone = r.Phone
                        union
                        Select b.Phone from Billing b inner Join Orders o on b.BillingID=o.BillingID where b.Phone = r.Phone)");
                q.CommandTimeout = 9999;
                q.Parameters.Add("@CampaignID", MySqlDbType.Int32).Value = campaignID;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                res = dao.Load<Registration>(q);

                if (phoneCheck)
                {
                    res = res.Distinct(new RegistrationPhoneComparer()).ToList();
                    for (int i = 0; i < res.Count; i++)
                    {
                        q = new MySqlCommand(@"Select count(*) from LeadPost where TelePhone=@Phone 
                                         and LeadTypeID=1 and ProductID=@ProductID");
                        q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = productID;
                        q.Parameters.Add("@Phone", MySqlDbType.VarChar).Value = res[i].Phone;
                        var count = dao.ExecuteScalar<int>(q);
                        if (count != 0)
                        {
                            res.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public Registration GetAbandonByEmail(string email, out int productID)
        {
            Registration res = null;
            Product prod = null;
            productID = -1;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select r.*, p.* from Registration r 
                    inner join Campaign c on c.CampaignID = r.CampaignID
                    inner join Subscription s on s.SubscriptionID = c.SubscriptionID
                    inner join Product p on p.ProductID = s.ProductID
                    where r.Email = @Email
                    union
                    select r.*, p.* from Registration r 
                    inner join Campaign c on c.CampaignID = r.CampaignID
                    inner join CampaignRecurringPlan crp on crp.CampaignID = c.CampaignID
                    inner join RecurringPlan rp on rp.RecurringPlanID = crp.RecurringPlanID
                    inner join Product p on p.ProductID = rp.ProductID
                    where r.Email = @Email");
                q.Parameters.Add("@Email", MySqlDbType.VarChar).Value = email;
                res = dao.Load<Registration>(q).LastOrDefault<Registration>();
                prod = dao.Load<Product>(q).LastOrDefault<Product>();

                if (prod != null)
                    productID = prod.ProductID.Value;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public void SetRoutingRules(int productID, int leadTypeID, IDictionary<int, int> leadPartnerPercentage)
        {
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand(@"
                    delete from LeadRouting
                    where ProductID = @productID and leadTypeID = @leadTypeID
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@leadTypeID", MySqlDbType.Int32).Value = leadTypeID;
                dao.ExecuteNonQuery(q);

                foreach (var item in leadPartnerPercentage)
                {
                    LeadRouting l = new LeadRouting();
                    l.ProductID = productID;
                    l.LeadTypeID = leadTypeID;
                    l.LeadPartnerID = item.Key;
                    l.Percentage = item.Value;
                    dao.Save<LeadRouting>(l);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public void SetLeadConfigValues(int productID, IList<LeadPartnerConfigValue> settings)
        {
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand(@"
                    delete from LeadPartnerConfigValue
                    where ProductID = @productID
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                dao.ExecuteNonQuery(q);

                foreach (LeadPartnerConfigValue item in settings)
                {
                    dao.Save<LeadPartnerConfigValue>(item);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public class BillingPhoneComparer : IEqualityComparer<Billing>
        {
            public bool Equals(Billing x, Billing y)
            {
                return x.Phone == y.Phone;
            }

            public int GetHashCode(Billing obj)
            {
                return obj.Phone.GetHashCode();
            }
        }

        public class RegistrationPhoneComparer : IEqualityComparer<Registration>
        {
            public bool Equals(Registration x, Registration y)
            {
                return x.Phone == y.Phone;
            }

            public int GetHashCode(Registration obj)
            {
                return obj.Phone.GetHashCode();
            }
        }

        public void SendAragonClubLead(long registrationID, int clubID)
        {
            try
            {
                const int PRODUCT_ID = 20;

                Billing billing = new BillingService().GetLastBillingByProspectID(registrationID);
                if (billing == null)
                {
                    throw new Exception(string.Format("Can't find Billing for Registration({0})", registrationID));
                }

                MySqlCommand q = new MySqlCommand(@"
                    select coalesce(count(*), 0) as Value from AragonWhiteLabelRequest ar 
                    where ar.BillingID = @billingID and ar.ProductID = @productID and ar.CallCenterID = @callCenterID
                    and coalesce(ar.Completed,0) = 0
                ");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = PRODUCT_ID;
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billing.BillingID.Value;
                q.Parameters.Add("@callCenterID", MySqlDbType.Int32).Value = clubID;
                var res1 = dao.Load<View<int>>(q).Single();

                if (res1.Value.Value == 0)
                {
                    //Send lead
                    string request = null;
                    string response = null;
                    bool res = new AragonWhiteLabelGateway().SendLead(billing, clubID, PRODUCT_ID, out request, out response);

                    q = new MySqlCommand("insert into AragonWhiteLabelRequest (BillingID, ProductID, CallCenterID, Completed, Request, Response, CreateDT) values (@BillingID, @ProductID, @CallCenterID, @Completed, @Request, @Response, @CreateDT)");
                    q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billing.BillingID.Value;
                    q.Parameters.Add("@ProductID", MySqlDbType.Int32).Value = PRODUCT_ID;
                    q.Parameters.Add("@CallCenterID", MySqlDbType.Int32).Value = clubID;
                    q.Parameters.Add("@Completed", MySqlDbType.Bit).Value = res;
                    q.Parameters.Add("@Request", MySqlDbType.VarChar).Value = request;
                    q.Parameters.Add("@Response", MySqlDbType.VarChar).Value = response;
                    q.Parameters.Add("@CreateDT", MySqlDbType.Timestamp).Value = DateTime.Now;
                    dao.ExecuteNonQuery(q);

                    if (res)
                    {
                        string clubName = (AragonShoppingClubEnum.List.ContainsKey(clubID) ? AragonShoppingClubEnum.List[clubID] : "Shopping Club #" + clubID.ToString());
                        string note = string.Format("Customer signup for {0} Club.  Lead sent to Aragon.", clubName);
                        new DashboardService().AddBillingNote(billing.BillingID.Value, null, note);
                    }
                }
                else
                {
                    //already sent
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public IList<LeadPartnerAffiliateView> GetLeadPartnerAffiliateList()
        {
            IList<LeadPartnerAffiliateView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                            select * from LeadPartnerAffiliate lpa 
                            inner join Affiliate a on a.AffiliateID=lpa.AffiliateID
                            inner join LeadPartner lp on lp.LeadPartnerID=lpa.LeadPartnerID");
                res = dao.Load<LeadPartnerAffiliateView>(q);
            }
            catch (Exception ex)
            {
                res = new List<LeadPartnerAffiliateView>();
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public OutboundSalesView GetOutboundReport(DateTime? startDate, DateTime? endDate, int? leadType, int? leadPartnerID, string aff)
        {
            OutboundSalesView res1 = null;
            OutboundSalesView res2 = null;
            OutboundSalesView res3 = null;
            try
            {
                string billingFilter = GetBillingFilter(startDate, endDate, leadType, leadPartnerID, aff);
                string affFilter = "";
                if (!string.IsNullOrEmpty(aff))
                    affFilter = @" and LeadPartnerID in (select LeadPartnerID from LeadPartnerAffiliate lpa 
                            inner join Affiliate a on a.AffiliateID=lpa.AffiliateID
                            where a.Code = '" + aff + "') ";

                //                MySqlCommand q = new MySqlCommand(@"
                //                        select sum(t.GrossRevenue) as GrossRevenue, sum(t.Refunds) as Refunds, sum(t.Sales) as Sales, sum(t.NumberOfLeads) as NumberOfLeads, sum(t.NumberOfChargebacks) as NumberOfChargebacks, @LeadPartnerID as LeadPartnerID, sum(t.GrossRevenue) - sum(t.Refunds) + sum(t.TotalChargebacks) + sum(t.MerchantingFees) - sum(t.CustomerServiceCost) - sum(t.FulfillmentCost) as NetRevenue, sum(t.CostOfSales) as CostOfSales, case sum(t.Clicks) when 0 then -1 else (100 * sum(t.Sales) / sum(t.Clicks)) end as Conversion from
                //                        (
                //                            select case when ch.Amount > 0 then ch.Amount else 0 end as GrossRevenue, case when ch.Amount < 0 then 0 - ch.Amount else 0 end as Refunds, 1 as Sales, 0 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, 0 as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost, bs.BillingID 
                //                            from LeadPost lp
                //                            inner join Registration r on lp.RegistrationID=r.RegistrationID
                //                            inner join Billing b on b.Phone=r.Phone
                //                            inner join Affiliate a on a.Code = b.Affiliate
                //                            inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                //                            inner join BillingSubscription bs on bs.BillingID = b.BillingID
                //                            inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID
                //                            Where (lp.CreateDT Between @StartDate and @EndDate) 
                //                            and lp.LeadTypeID=@LeadTypeID
                //                            and lp.LeadPartnerID=@LeadPartnerID
                //                            union all
                //
                //                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 1 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, 0 as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost, 0 as BillingID from LeadPost Where (CreateDT Between @StartDate and @EndDate) and LeadTypeID=@LeadTypeID and LeadPartnerID=@LeadPartnerID
                //                            union all
                //
                //                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 0 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, Costs.CostPerSale as CostOfSales, 0 as Clicks, 0 as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost, Costs.BillingID
                //                            from 
                //                            (
                //                                Select coalesce(sub.CostPerSale, aff.CostPerSale, 0) as CostPerSale, b.BillingID
                //                                from LeadPost lp
                //                                inner join Registration r on lp.RegistrationID=r.RegistrationID
                //                                inner join Billing b on b.Phone=r.Phone
                //                                inner join Affiliate a on a.Code = b.Affiliate
                //                                inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                //                                inner join BillingSubscription bs on bs.BillingID = b.BillingID
                //                                inner join Affiliate aff on (b.Affiliate=aff.Code)
                //                                left join SubAffiliate sub on sub.AffiliateID=aff.AffiliateID
                //                                where 
                //                                (b.SubAffiliate = '' or sub.SubAffiliateID is null or b.SubAffiliate is null or sub.Code = b.SubAffiliate)
                //                                and (lp.CreateDT Between @StartDate and @EndDate) 
                //                                and lp.LeadTypeID=@LeadTypeID
                //                                and lp.LeadPartnerID=@LeadPartnerID
                //                                group by b.BillingID
                //                            ) Costs
                //                            union all
                //
                //                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 0 as NumberOfLeads, 1 as NumberOfChargebacks, Chargebacks.ChargebackAmount + Chargebacks.ChargebackFee as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, 0 as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost, Chargebacks.BillingID
                //                            from
                //                            (
                //                                	select sale.*, ch.Amount as ChargebackAmount, am.ChargebackFee as ChargebackFee, bs.BillingID 
                //                                    from LeadPost lp
                //                                    inner join Registration r on lp.RegistrationID=r.RegistrationID
                //                                    inner join Billing b on b.Phone=r.Phone
                //                                    inner join Affiliate a on a.Code = b.Affiliate
                //                                    inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                //                                    inner join BillingSubscription bs on bs.BillingID = b.BillingID
                //				                    inner join BillingSale bsale on bs.BillingSubscriptionID = bsale.BillingSubscriptionID
                //	                                inner join Sale sale on sale.SaleID=bsale.SaleID
                //                                    inner join SaleChargeback sc on sc.SaleId=sale.SaleID
                //                                    inner join AffiliateScrub affScrub on affScrub.BillingID=bs.BillingID
                //                                    inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID
                //                                    inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                //                                    Where (lp.CreateDT Between @StartDate and @EndDate) 
                //                                    and lp.LeadTypeID=@LeadTypeID
                //                                    and lp.LeadPartnerID=@LeadPartnerID
                //                                    union all
                //
                //                                    select sale.*, ch.Amount as ChargebackAmount, am.ChargebackFee as ChargebackFee, u.BillingID 
                //                                    from LeadPost lp
                //                                    inner join Registration r on lp.RegistrationID=r.RegistrationID
                //                                    inner join Billing b on b.Phone=r.Phone
                //                                    inner join Affiliate a on a.Code = b.Affiliate
                //                                    inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                //                                    inner join BillingSubscription bs on bs.BillingID = b.BillingID
                //				                    inner join Upsell u on u.BillingID = bs.BillingID
                //						            inner join UpsellSale us on us.UpsellID = u.UpsellID
                //	                                inner join Sale sale on us.SaleID=sale.SaleID
                //                                    inner join SaleChargeback sc on sc.SaleId=sale.SaleID
                //                                    inner join AffiliateScrub affScrub on affScrub.BillingID=u.BillingID
                //                                    inner join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID
                //                                    inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                //                                    Where (lp.CreateDT Between @StartDate and @EndDate) 
                //                                    and lp.LeadTypeID=@LeadTypeID
                //                                    and lp.LeadPartnerID=@LeadPartnerID
                //                                    union all
                //
                //                                    select sale.*, 0 as ChargebackAmount, 0 as ChargebackFee, o.BillingID 
                //                                    from LeadPost lp
                //                                    inner join Registration r on lp.RegistrationID=r.RegistrationID
                //                                    inner join Billing b on b.Phone=r.Phone
                //                                    inner join Affiliate a on a.Code = b.Affiliate
                //                                    inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                //                                    inner join BillingSubscription bs on bs.BillingID = b.BillingID
                //				                    inner join Orders o on o.BillingID = bs.BillingID
                //						            inner join OrderSale os on os.OrderID = o.OrderID
                //	                                inner join Sale sale on os.SaleID=sale.SaleID
                //                                    inner join SaleChargeback sc on sc.SaleId=sale.SaleID
                //                                    inner join AffiliateScrub affScrub on affScrub.BillingID=o.BillingID
                //                                    Where (lp.CreateDT Between @StartDate and @EndDate) 
                //                                    and lp.LeadTypeID=@LeadTypeID
                //                                    and lp.LeadPartnerID=@LeadPartnerID
                //                            ) Chargebacks
                //                            union all
                //
                //                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 0 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, 0 as CostOfSales, 1 as Clicks, 0 as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost, bs.BillingID
                //                            from LeadPost lp
                //                            inner join Registration r on lp.RegistrationID=r.RegistrationID
                //                            inner join Billing b on b.Phone=r.Phone
                //                            inner join Affiliate a on a.Code = b.Affiliate
                //                            inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                //                            inner join BillingSubscription bs on bs.BillingID = b.BillingID
                //			                inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
                //			                inner join Campaign cmp on cmp.SubscriptionID = s.SubscriptionID
                //                            inner join (select * from Conversion where CreateDT between @StartDate and @EndDate) c on cmp.CampaignID = c.CampaignID
                //			                Where (lp.CreateDT Between @StartDate and @EndDate) 
                //                            and lp.LeadTypeID=@LeadTypeID
                //                            and lp.LeadPartnerID=@LeadPartnerID
                //                            union all
                //
                //                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 0 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, Abs(ch.Amount) * IfNull(am.ProcessingRate, 0) / 100 - IfNull(am.TransactionFee, 0) as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost, b.BillingID
                //                            from LeadPost lp
                //                            inner join Registration r on lp.RegistrationID=r.RegistrationID
                //                            inner join Billing b on b.Phone=r.Phone
                //                            inner join Affiliate a on a.Code = b.Affiliate
                //                            inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                //                            inner join BillingSubscription bs on bs.BillingID = b.BillingID
                //                            inner join ChargeHistoryEx ch on ch.BillingSubscriptionID = bs.BillingSubscriptionID
                //			                inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
                //			                inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
                //                            inner join AffiliateScrub affScrub on affScrub.BillingID=b.BillingID
                //			                where ch.Success = 1
                //                            and (lp.CreateDT Between @StartDate and @EndDate) 
                //                            and lp.LeadTypeID=@LeadTypeID
                //                            and lp.LeadPartnerID=@LeadPartnerID
                //                            union all
                //
                //                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 0 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, 0 as MerchantingFees, FulfillmentCost, CustomerServiceCost, f.BillingID
                //                            from 
                //                            (
                //			                    select 0 as FulfillmentCost, IfNull(p.CustomerServiceCost, 0) as CustomerServiceCost, b.BillingID
                //                                from LeadPost lp
                //                                inner join Registration r on lp.RegistrationID=r.RegistrationID
                //                                inner join Billing b on b.Phone=r.Phone
                //                                inner join Affiliate a on a.Code = b.Affiliate
                //                                inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                //                                inner join BillingSubscription bs on bs.BillingID = b.BillingID
                //			                    inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
                //			                    inner join Product p on p.ProductID = s.ProductID
                //			                    where b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'
                //                                and (lp.CreateDT Between @StartDate and @EndDate) 
                //                                and lp.LeadTypeID=@LeadTypeID
                //                                and lp.LeadPartnerID=@LeadPartnerID
                //                                union all
                //
                //                                select IfNull(sh.FulfillmentCost, 0) as FulfillmentCost, 0 as CustomerServiceCost, b.BillingID
                //                                from LeadPost lp
                //                                inner join Registration r on lp.RegistrationID=r.RegistrationID
                //                                inner join Billing b on b.Phone=r.Phone
                //                                inner join Affiliate a on a.Code = b.Affiliate
                //                                inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                //                                inner join BillingSubscription bs on bs.BillingID = b.BillingID
                //			                    inner join AggBillingSale abs on abs.BillingID=bs.BillingID
                //			                    inner join AggShipperSale ship on ship.SaleID = abs.SaleID
                //			                    inner join Shipper sh on sh.ShipperID = ship.ShipperID
                //			                    where 
                //                                (lp.CreateDT Between @StartDate and @EndDate) 
                //                                and lp.LeadTypeID=@LeadTypeID
                //                                and lp.LeadPartnerID=@LeadPartnerID
                //                                union all
                //
                //                                select IfNull(sh.FulfillmentCost, 0) as FulfillmentCost, 0 as CustomerServiceCost, b.BillingID
                //                                from LeadPost lp
                //                                inner join Registration r on lp.RegistrationID=r.RegistrationID
                //                                inner join Billing b on b.Phone=r.Phone
                //                                inner join Affiliate a on a.Code = b.Affiliate
                //                                inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                //                                inner join BillingSubscription bs on bs.BillingID = b.BillingID
                //			                    inner join AggBillingSale abs on abs.BillingID=bs.BillingID
                //			                    inner join ReturnedSale rs on rs.SaleID = abs.SaleID
                //			                    inner join AggShipperSale ship on ship.SaleID = abs.SaleID
                //			                    inner join Shipper sh on sh.ShipperID = ship.ShipperID
                //                                where 
                //                                (lp.CreateDT Between @StartDate and @EndDate) 
                //                                and lp.LeadTypeID=@LeadTypeID
                //                                and lp.LeadPartnerID=@LeadPartnerID
                //                            ) f 
                //                        ) t
                //                          ");


                MySqlCommand q = new MySqlCommand(@"
                        select sum(t.GrossRevenue) as GrossRevenue, sum(t.Refunds) as Refunds, sum(t.Sales) as Sales, sum(t.NumberOfLeads) as NumberOfLeads, sum(t.NumberOfChargebacks) as NumberOfChargebacks, @LeadPartnerID as LeadPartnerID, sum(t.GrossRevenue) - sum(t.Refunds) + sum(t.TotalChargebacks) + sum(t.MerchantingFees) - sum(t.CustomerServiceCost) - sum(t.FulfillmentCost) as NetRevenue, sum(t.CostOfSales) as CostOfSales, case sum(t.NumberOfLeads) when 0 then -1 else (100 * sum(t.Sales) / sum(t.NumberOfLeads)) end as Conversion from
                        (
                            select case when ch.Amount > 0 then ch.Amount else 0 end as GrossRevenue, case when ch.Amount < 0 then 0 - ch.Amount else 0 end as Refunds, 0 as Sales, 0 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, 0 as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost from ChargeHistoryEx ch
                            inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID
                            where bs.BillingID in
                            (" + billingFilter + @")
                            union all

                            select 0 as GrossRevenue, 0 as Refunds, 1 as Sales, 0 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, 0 as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost from BillingSubscription bs
                            where bs.BillingID in
                            (" + billingFilter + @")
                            union all

                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 1 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, 0 as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost from LeadPost 
                            where (CreateDT Between @StartDate and @EndDate) and LeadTypeID=@LeadTypeID and LeadPartnerID=@LeadPartnerID
                            " + affFilter + @"
                            union all

                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 0 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, CostPerSale as CostOfSales, 0 as Clicks, 0 as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost from
                            (
                                Select coalesce(sub.CostPerSale, aff.CostPerSale, 0) as CostPerSale
                                from  Billing b
                                inner join BillingSubscription bs on bs.BillingID=b.BillingID
                                inner join Affiliate aff on (b.Affiliate=aff.Code)
                                left join SubAffiliate sub on sub.AffiliateID=aff.AffiliateID
                                where b.BillingID in 
                                (" + billingFilter + @")
                                and (b.SubAffiliate = '' or sub.SubAffiliateID is null or b.SubAffiliate is null or sub.Code = b.SubAffiliate)
                                group by b.BillingID
                            ) Costs
                            union all

                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 0 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, Abs(ch.Amount) * IfNull(am.ProcessingRate, 0) / 100 - IfNull(am.TransactionFee, 0) as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost
                            from ChargeHistoryEx ch
			                inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
			                inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID
			                inner join Billing b on bs.BillingID = b.BillingID
			                inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
                            inner join AffiliateScrub affScrub on affScrub.BillingID=b.BillingID
			                where ch.Success = 1
                            and b.BillingID in
                            (" + billingFilter + @")                           
                        ) t
                        ");

                q.CommandTimeout = 9999;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                q.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = leadType;
                q.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = leadPartnerID;
                res1 = dao.Load<OutboundSalesView>(q).SingleOrDefault();

                q = new MySqlCommand(@"
                        select sum(t.GrossRevenue) as GrossRevenue, sum(t.Refunds) as Refunds, sum(t.Sales) as Sales, sum(t.NumberOfLeads) as NumberOfLeads, sum(t.NumberOfChargebacks) as NumberOfChargebacks, @LeadPartnerID as LeadPartnerID, sum(t.GrossRevenue) - sum(t.Refunds) + sum(t.TotalChargebacks) + sum(t.MerchantingFees) - sum(t.CustomerServiceCost) - sum(t.FulfillmentCost) as NetRevenue, sum(t.CostOfSales) as CostOfSales, case sum(t.Clicks) when 0 then -1 else (100 * sum(t.Sales) / sum(t.Clicks)) end as Conversion from
                        (
                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 0 as NumberOfLeads, 1 as NumberOfChargebacks, Chargebacks.ChargebackAmount + Chargebacks.ChargebackFee as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, 0 as MerchantingFees, 0 as FulfillmentCost, 0 as CustomerServiceCost from
                            (
                                	select sale.*, ch.Amount as ChargebackAmount, am.ChargebackFee as ChargebackFee from BillingSale bsale
				                    inner join BillingSubscription bs on bs.BillingSubscriptionID = bsale.BillingSubscriptionID
	                                inner join Sale sale on sale.SaleID=bsale.SaleID
                                    inner join SaleChargeback sc on sc.SaleId=sale.SaleID
                                    inner join AffiliateScrub affScrub on affScrub.BillingID=bs.BillingID
                                    inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsale.ChargeHistoryID
                                    inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
				                    where bs.BillingID in 
                                    (" + billingFilter + @")
                                    union all
                                    select sale.*, ch.Amount as ChargebackAmount, am.ChargebackFee as ChargebackFee from Upsell u
						            inner join UpsellSale us on us.UpsellID = u.UpsellID
	                                inner join Sale sale on us.SaleID=sale.SaleID
                                    inner join SaleChargeback sc on sc.SaleId=sale.SaleID
                                    inner join AffiliateScrub affScrub on affScrub.BillingID=u.BillingID
                                    inner join ChargeHistoryEx ch on ch.ChargeHistoryID = us.ChargeHistoryID
                                    inner join AssertigyMID am on am.AssertigyMIDID = ch.MerchantAccountID
						            where u.BillingID in
                                    (" + billingFilter + @")
                                    union all
                                    select sale.*, 0 as ChargebackAmount, 0 as ChargebackFee from Orders o
						            inner join OrderSale os on os.OrderID = o.OrderID
	                                inner join Sale sale on os.SaleID=sale.SaleID
                                    inner join SaleChargeback sc on sc.SaleId=sale.SaleID
                                    inner join AffiliateScrub affScrub on affScrub.BillingID=o.BillingID
						            where o.BillingID in
                                    (" + billingFilter + @")
                            ) Chargebacks
                        ) t
                ");

                q.CommandTimeout = 9999;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                q.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = leadType;
                q.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = leadPartnerID;
                res2 = dao.Load<OutboundSalesView>(q).SingleOrDefault();

                q = new MySqlCommand(@"
                        select sum(t.GrossRevenue) as GrossRevenue, sum(t.Refunds) as Refunds, sum(t.Sales) as Sales, sum(t.NumberOfLeads) as NumberOfLeads, sum(t.NumberOfChargebacks) as NumberOfChargebacks, @LeadPartnerID as LeadPartnerID, sum(t.GrossRevenue) - sum(t.Refunds) + sum(t.TotalChargebacks) + sum(t.MerchantingFees) - sum(t.CustomerServiceCost) - sum(t.FulfillmentCost) as NetRevenue, sum(t.CostOfSales) as CostOfSales, case sum(t.Clicks) when 0 then -1 else (100 * sum(t.Sales) / sum(t.Clicks)) end as Conversion from
                        (
                            select 0 as GrossRevenue, 0 as Refunds, 0 as Sales, 0 as NumberOfLeads, 0 as NumberOfChargebacks, 0 as TotalChargebacks, 0 as CostOfSales, 0 as Clicks, 0 as MerchantingFees, f.FulfillmentCost as FulfillmentCost, f.CustomerServiceCost as CustomerServiceCost
                            from 
                            (
			                    select 0 as FulfillmentCost, IfNull(p.CustomerServiceCost, 0) as CustomerServiceCost
			                    from BillingSubscription bs
			                    inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
			                    inner join Product p on p.ProductID = s.ProductID
			                    inner join Billing b on bs.BillingID = b.BillingID
			                    where b.CreditCard <> '*...............' and b.CreditCard <> '4111111111111111'
                                and b.BillingID in
                                (" + billingFilter + @")
                                union all
                                select IfNull(sh.FulfillmentCost, 0) as FulfillmentCost, 0 as CustomerServiceCost
			                    from AggBillingSale bs
			                    inner join Billing b on bs.BillingID = b.BillingID
			                    inner join AggShipperSale ship on ship.SaleID = bs.SaleID
			                    inner join Shipper sh on sh.ShipperID = ship.ShipperID
                                where b.BillingID in
                                (" + billingFilter + @")
                                union all
                                select IfNull(sh.FulfillmentCost, 0) as FulfillmentCost, 0 as CustomerServiceCost
			                    from AggBillingSale bs
			                    inner join Billing b on bs.BillingID = b.BillingID
			                    inner join ReturnedSale rs on rs.SaleID = bs.SaleID
			                    inner join AggShipperSale ship on ship.SaleID = bs.SaleID
			                    inner join Shipper sh on sh.ShipperID = ship.ShipperID
                                where b.BillingID in
                                (" + billingFilter + @")
                            ) f
                        )t
                ");

                q.CommandTimeout = 9999;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                q.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = leadType;
                q.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = leadPartnerID;
                res3 = dao.Load<OutboundSalesView>(q).SingleOrDefault();

                res1.NumberOfChargebacks = res2.NumberOfChargebacks ?? 0;
                res1.NetRevenue += res2.NetRevenue ?? 0;
                res1.NetRevenue += res3.NetRevenue ?? 0;
            }
            catch (Exception ex)
            {
                res1 = new OutboundSalesView();
                logger.Error(GetType(), ex);
            }
            return res1;
        }

        private string GetBillingFilter(DateTime? startDate, DateTime? endDate, int? leadType, int? leadPartnerID, string aff)
        {
            StringBuilder res = null;
            try
            {
                if (!string.IsNullOrEmpty(aff))
                    aff = " and a.Code = '" + aff + "'";

                MySqlCommand q = new MySqlCommand(@"
                        select b.* from LeadPost lp
                        inner join Registration r on lp.RegistrationID=r.RegistrationID
                        inner join Billing b on b.Phone=r.Phone
                        inner join Affiliate a on a.Code = b.Affiliate
                        inner join LeadPartnerAffiliate lpa on lpa.AffiliateID=a.AffiliateID and lpa.LeadPartnerID=@LeadPartnerID
                        Where (lp.CreateDT Between @StartDate and @EndDate) 
                        and lp.LeadTypeID=@LeadTypeID
                        and lp.LeadPartnerID=@LeadPartnerID
                        " + aff);

                q.CommandTimeout = 9999;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                q.Parameters.Add("@LeadTypeID", MySqlDbType.Int32).Value = leadType;
                q.Parameters.Add("@LeadPartnerID", MySqlDbType.Int32).Value = leadPartnerID;
                var billingList =  dao.Load<Billing>(q);

                res = new StringBuilder();
                foreach (var billing in billingList)
                    res.Append(billing.BillingID.ToString() + ",");

                if (res.Length > 0)
                    res = res.Remove(res.Length - 1, 1);
                if (res.Length == 0)
                    res.Append("0");

            }
            catch (Exception ex)
            {
                res = new StringBuilder();
                logger.Error(GetType(), ex);
            }
            return res.ToString();
        }
    }
}
