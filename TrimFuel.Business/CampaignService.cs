using System;
using System.Collections.Generic;
using System.Linq;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Business.Dao.EntityDataProviders.Views;

namespace TrimFuel.Business
{
    public class CampaignService : BaseService
    {
        public Campaign GetCampaignByID(int campaignID)
        {
            Campaign res;

            try
            {
                res = dao.Load<Campaign>(campaignID);
            }
            catch
            {
                res = null;
            }

            return res;
        }

        public IList<Campaign> GetCampaignsList()
        {
            IList<Campaign> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select c.* from Campaign c");

                res = dao.Load<Campaign>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<CampaignPage> GetCampaignPages(int campaignID)
        {
            IList<CampaignPage> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select cp.* from CampaignPage cp where cp.CampaignID = @campaignID");
                q.Parameters.AddWithValue("@campaignID", campaignID);

                res = dao.Load<CampaignPage>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public CampaignUpsell GetCampaignUpsell(int campaignPageID)
        {
            CampaignUpsell res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select cu.* from CampaignUpsell cu where cu.CampaignPageID = @campaignPageID");
                q.Parameters.AddWithValue("@campaignPageID", campaignPageID);

                res = dao.Load<CampaignUpsell>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);

                res = null;
            }

            return res;
        }

        public IList<CampaignAffiliate> GetCampaignAffiliateList(int campaignID)
        {
            IList<CampaignAffiliate> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from CampaignAffiliate where CampaignID = @campaignID");
                q.Parameters.AddWithValue("@campaignID", campaignID);

                res = dao.Load<CampaignAffiliate>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<CampaignLeadRouting> GetCampaignLeadRoutingList(int campaignID)
        {
            IList<CampaignLeadRouting> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from CampaignLeadRouting where CampaignID = @campaignID");
                q.Parameters.AddWithValue("@campaignID", campaignID);

                res = dao.Load<CampaignLeadRouting>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public CampaignView GetCampaignView(int campaignID)
        {
            CampaignView res = new CampaignView();
            try
            {
                res.Campaign = EnsureLoad<Campaign>(campaignID);
                res.Affiliates = GetCampaignAffiliateList(campaignID);
                res.LeadPartners = GetCampaignLeadRoutingList(campaignID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public BusinessError<CampaignView> SaveCampaignView(CampaignView campaignView)
        {
            BusinessError<CampaignView> res = new BusinessError<CampaignView>(null, BusinessErrorState.Error, null);
            try
            {
                dao.BeginTransaction();

                // cleanup sub-entities
                if (campaignView.Campaign.CampaignID != null)
                {
                    MySqlCommand q = new MySqlCommand(@"delete from CampaignAffiliate where CampaignID = @campaignID");
                    q.Parameters.Add("@campaignID", MySqlDbType.Int32).Value = campaignView.Campaign.CampaignID;
                    dao.ExecuteNonQuery(q);

                    q = new MySqlCommand(@"delete from CampaignLeadRouting where CampaignID = @campaignID");
                    q.Parameters.Add("@campaignID", MySqlDbType.Int32).Value = campaignView.Campaign.CampaignID;
                    dao.ExecuteNonQuery(q);
                }

                dao.Save<Campaign>(campaignView.Campaign);

                foreach (var item in campaignView.Affiliates)
                {
                    if (item.CampaignID == null)
                        item.CampaignID = campaignView.Campaign.CampaignID;
                    dao.Save(item);
                }

                foreach (var item in campaignView.LeadPartners)
                {
                    if (item.CampaignID == null)
                        item.CampaignID = campaignView.Campaign.CampaignID;
                    dao.Save(item);
                }

                res.ReturnValue = campaignView;
                res.State = BusinessErrorState.Success;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public Campaign CreateDuplicateCampaign(int campaignID)
        {
            Campaign duplicate = null;
            try
            {
                dao.BeginTransaction();

                Campaign campaign = EnsureLoad<Campaign>(campaignID);
                duplicate = new Campaign();

                duplicate.Active = campaign.Active;
                duplicate.CreateDT = DateTime.Now;
                duplicate.DisplayName = campaign.DisplayName + " (2)";
                duplicate.EnableFitFactory = campaign.EnableFitFactory;
                duplicate.IsMerchant = campaign.IsMerchant;
                duplicate.IsSave = campaign.IsSave;
                duplicate.IsSTO = campaign.IsSTO;
                duplicate.ParentCampaignID = campaign.ParentCampaignID;
                duplicate.Percentage = campaign.Percentage;
                duplicate.Redirect = campaign.Redirect;
                duplicate.SendUserEmail = campaign.SendUserEmail;
                duplicate.SubscriptionID = campaign.SubscriptionID;
                duplicate.URL = campaign.URL;
                dao.Save(duplicate);

                IList<CampaignPage> campaignPages = GetCampaignPages(campaignID);

                foreach (CampaignPage campaignPage in campaignPages)
                {
                    SaveCampaignPage(campaignPage.Header, campaignPage.HTML, campaignPage.Title, duplicate.CampaignID.Value, campaignPage.PageTypeID);
                }

                var plan = new SubscriptionNewService().GetRecurringPlanByCampaign(campaign.CampaignID.Value);
                if (plan != null && plan.RecurringPlan != null)
                {
                    new SubscriptionNewService().UpdateCampaignSubscription(duplicate.CampaignID.Value, plan.CampaignRecurringPlan.RecurringPlanID,
                        plan.CampaignRecurringPlan.TrialPrice, plan.CampaignRecurringPlan.TrialInterim,
                        plan.ProductList.Select(i => new KeyValuePair<string, int>(i.ProductSKU, i.Quantity.Value)).ToList());
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                duplicate = null;
                dao.RollbackTransaction();
            }
            return duplicate;
        }

        public BusinessError<Campaign> CreateCampaign(string displayName, int? subscriptionID, int? percentage, bool active, bool redirect,
            bool isSave, int? parentCampaignID, bool enableFitFactory, string url, bool isSto, bool sendUserEmail, bool isMerchant,
            bool isRiskScoring, bool isDupeChecking, bool IsExternal, int? shipperID)
        {
            BusinessError<Campaign> res = new BusinessError<Campaign>(null, BusinessErrorState.Error, null);
            try
            {
                dao.BeginTransaction();

                //todo: some validation?

                Campaign campaign = new Campaign();
                campaign.DisplayName = displayName;
                campaign.SubscriptionID = subscriptionID;
                campaign.Percentage = percentage;
                campaign.Active = active;
                campaign.CreateDT = DateTime.Now;
                campaign.Redirect = redirect;
                campaign.IsSave = isSave;
                campaign.ParentCampaignID = parentCampaignID;
                campaign.EnableFitFactory = enableFitFactory;
                campaign.URL = url;
                campaign.IsSTO = isSto;
                campaign.SendUserEmail = sendUserEmail;
                campaign.IsMerchant = isMerchant;
                campaign.IsRiskScoring = isRiskScoring;
                campaign.IsDupeChecking = isDupeChecking;
                campaign.IsExternal = IsExternal;
                campaign.ShipperID = shipperID;

                dao.Save<Campaign>(campaign);

                res.ReturnValue = campaign;
                res.State = BusinessErrorState.Success;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public BusinessError<Campaign> UpdateCampaign(int campaignID, string displayName, int? subscriptionID, int? percentage, bool active, bool redirect,
            bool isSave, int? parentCampaignID, bool enableFitFactory, string url, bool isSto, bool sendUserEmail, bool isMerchant,
            bool isRiskScoring, bool isDupeChecking, bool IsExternal, int? shipperID)
        {
            BusinessError<Campaign> res = new BusinessError<Campaign>(null, BusinessErrorState.Error, null);
            try
            {
                dao.BeginTransaction();

                //todo: some validation?

                Campaign campaign = EnsureLoad<Campaign>(campaignID);
                campaign.DisplayName = displayName;
                campaign.SubscriptionID = subscriptionID;
                campaign.Percentage = percentage;
                campaign.Active = active;
                campaign.CreateDT = DateTime.Now;
                campaign.Redirect = redirect;
                campaign.IsSave = isSave;
                campaign.ParentCampaignID = parentCampaignID;
                campaign.EnableFitFactory = enableFitFactory;
                campaign.URL = url;
                campaign.IsSTO = isSto;
                campaign.SendUserEmail = sendUserEmail;
                campaign.IsMerchant = isMerchant;
                campaign.IsRiskScoring = isRiskScoring;
                campaign.IsDupeChecking = isDupeChecking;
                campaign.IsExternal = IsExternal;
                campaign.ShipperID = shipperID;

                dao.Save<Campaign>(campaign);

                res.ReturnValue = campaign;
                res.State = BusinessErrorState.Success;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public BusinessError<CampaignPage> SaveCampaignPage(string header, string html, string title, int campaignID, int pageTypeID)
        {
            BusinessError<CampaignPage> res = new BusinessError<CampaignPage>(null, BusinessErrorState.Error, null);

            try
            {
                MySqlCommand q = new MySqlCommand("select * from CampaignPage where CampaignID = @campaignID and PageTypeID = @pageTypeID");
                q.Parameters.AddWithValue("@campaignID", campaignID);
                q.Parameters.AddWithValue("@pageTypeID", pageTypeID);

                CampaignPage campaignPage = dao.Load<CampaignPage>(q).FirstOrDefault();

                if (campaignPage == null)
                    campaignPage = new CampaignPage();

                campaignPage.Header = header;
                campaignPage.HTML = html;
                campaignPage.Title = title;
                campaignPage.CampaignID = campaignID;
                campaignPage.PageTypeID = pageTypeID;

                dao.Save<CampaignPage>(campaignPage);

                res.ReturnValue = campaignPage;
                res.State = BusinessErrorState.Success;

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public void ToggleActive(int campaignID, bool active)
        {
            try
            {
                MySqlCommand q = new MySqlCommand("update Campaign set Active = @archive where CampaignID = @campaignID");
                q.Parameters.AddWithValue("@archive", active);
                q.Parameters.AddWithValue("@campaignID", campaignID);

                dao.ExecuteNonQuery(q);
            }
            catch
            {
                logger.ErrorFormat("Couldn't toggle Campaign {0}", campaignID);
            }
        }

        public int GetNumberOfRegistrations(int campaignID)
        {
            int numberOfRegistrations = 0;

            try
            {
                MySqlCommand q = new MySqlCommand("select count(*) as Amount from Registration where CampaignID = @campaignID");
                q.Parameters.AddWithValue("@campaignID", campaignID);

                numberOfRegistrations = (int)dao.ExecuteScalar<int>(q);
            }
            catch
            {
                logger.ErrorFormat("Couldn't fetch number of registrations for campaign {0}", campaignID);
            }

            return numberOfRegistrations;
        }

        public IList<PageType> GetPageTypes()
        {
            IList<PageType> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select pt.* from PageType pt");

                res = dao.Load<PageType>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public CampaignControl GetCampaignControl(string name)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand("select * from CampaignControl where Name = @name");
                cmd.Parameters.AddWithValue("@name", name);

                return dao.Load<CampaignControl>(cmd).FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }

        public TermView GetTerms(int campaignID)
        {
            TermView res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select 
                        t1.DisplayName CorporationName, 
                        t1.Body CorporationBody, 
                        t2.DisplayName ReturnAddressName, 
                        t2.Body ReturnAddressBody, 
                        ct.PhoneNumber Phone, 
                        ct.EmailAddress Email, 
                        t3.Body MembershipTerms, 
                        t3.Body2 StraightSaleTerms,
                        t4.Body as Outline,
                        t5.Body as PrivacyPolicy
                    FROM CampaignTerm ct
                    left join Term t1 on t1.TermID = ct.Corporation
                    left join Term t2 on t2.TermID = ct.WarehouseReturnAddress
                    left join Term t3 on t3.TermID = ct.ProductSpecific
                    left join Term t4 on t4.TermID = ct.Outline                    
                    left join Term t5 on t5.TermID = ct.PrivacyPolicy
                    where ct.CampaignID = @campaignID
                ");
                q.Parameters.AddWithValue("@campaignID", campaignID);

                res = dao.Load<TermView>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        public IList<CampaignDetailsView> GetCampaignsWithDetailsOldStructure()
        {
            IList<CampaignDetailsView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select c.CampaignID, c.DisplayName, t.DisplayName Corporation, ct.PhoneNumber Phone, ct.EmailAddress Email, c.URL, count(r.RegistrationID) RegistrationCount, c.IsMerchant, c.IsExternal, c.Active
                    from Campaign c
                    left join CampaignTerm ct on c.CampaignID = ct.CampaignID
                    left join Term t on t.TermID = ct.Corporation
                    left join Registration r on c.CampaignID = r.CampaignID
                    where c.CampaignID not in (select CampaignID from CampaignRecurringPlan)
                    group by c.CampaignID");

                res = dao.Load<CampaignDetailsView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<CampaignDetailsView> GetCampaignsWithDetailsNewStructure()
        {
            IList<CampaignDetailsView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select c.CampaignID, c.DisplayName, t.DisplayName Corporation, ct.PhoneNumber Phone, ct.EmailAddress Email, c.URL, count(r.RegistrationID) RegistrationCount, c.IsMerchant, c.IsExternal, c.Active
                    from Campaign c
                    left join CampaignTerm ct on c.CampaignID = ct.CampaignID
                    left join Term t on t.TermID = ct.Corporation
                    left join Registration r on c.CampaignID = r.CampaignID
                    where c.CampaignID in (select CampaignID from CampaignRecurringPlan)
                    group by c.CampaignID");

                res = dao.Load<CampaignDetailsView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public bool SaveCampaignUpsell(int? campaignPageID, string productCode, int? quantity, decimal? price, int? subscriptionID, bool isSubscription, int? recurringPlanID)
        {
            bool res = false;
            try
            {
                var campaignUpsell = GetCampaignUpsell(campaignPageID ?? 0);
                if (campaignUpsell != null && string.IsNullOrEmpty(productCode) && !isSubscription)
                {
                    //remove upsell from page
                    MySqlCommand q = new MySqlCommand("Delete from CampaignUpsell where CampaignUpsellID=@CampaignUpsellID");
                    q.Parameters.Add("@CampaignUpsellID", MySqlDbType.Int32).Value = campaignUpsell.CampaignUpsellID;
                    dao.ExecuteNonQuery(q);
                    res = true;
                }
                else
                {
                    if (string.IsNullOrEmpty(productCode) && subscriptionID == null && recurringPlanID == null)
                    {
                        //empty code and empty subscription
                        return true;
                    }

                    var campaignPage = dao.Load<CampaignPage>(campaignPageID);

                    if (campaignPage != null)
                    {
                        if (campaignUpsell == null)
                            campaignUpsell = new CampaignUpsell()
                            {
                                CampaignPageID = campaignPageID
                            };
                        campaignUpsell.CampaignID = campaignPage.CampaignID;
                        campaignUpsell.Price = price;
                        campaignUpsell.ProductCode = productCode;
                        campaignUpsell.Quantity = quantity;

                        if (isSubscription)
                        {
                            campaignUpsell.SubscriptionID = subscriptionID;
                            campaignUpsell.RecurringPlanID = recurringPlanID;
                            campaignUpsell.ProductCode = string.Empty;
                            campaignUpsell.Price = null;
                            campaignUpsell.Quantity = null;
                        }
                        else
                        {
                            campaignUpsell.SubscriptionID = null;
                            campaignUpsell.RecurringPlanID = null;
                        }

                        dao.Save<CampaignUpsell>(campaignUpsell);
                        res = true;
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

        public bool DeleteCampaignPage(int campaignID, int pageTypeID)
        {
            bool res = false;
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select * from CampaignPage where CampaignID = @campaignID and PageTypeID = @pageTypeID");
                q.Parameters.AddWithValue("@campaignID", campaignID);
                q.Parameters.AddWithValue("@pageTypeID", pageTypeID);

                CampaignPage campaignPage = dao.Load<CampaignPage>(q).FirstOrDefault();

                if (campaignPage != null)
                {
                    q = new MySqlCommand("Delete from CampaignUpsell where CampaignPageID=@CampaignPageID");
                    q.Parameters.Add("@CampaignPageID", MySqlDbType.Int32).Value = campaignPage.CampaignPageID;
                    dao.ExecuteNonQuery(q);

                    q = new MySqlCommand("Delete from CampaignPage where CampaignPageID=@CampaignPageID");
                    q.Parameters.Add("@CampaignPageID", MySqlDbType.Int32).Value = campaignPage.CampaignPageID;
                    dao.ExecuteNonQuery(q);
                }
                dao.CommitTransaction();
                res = true;
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                res = false;
                logger.Error(GetType(), ex);
            }
            return res;
        }
    }
}
