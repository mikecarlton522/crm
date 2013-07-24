using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Business.Utils;

namespace TrimFuel.Business
{
    public class PageService : BaseService
    {
        public IList<Affiliate> GetAffiliates()
        {
            IList<Affiliate> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select a.* From Affiliate a where a.Active=1 and a.Deleted=0");

                res = dao.Load<Affiliate>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Pixel> GetPixelListByAffiliate(string affiliateCode)
        {
            IList<Pixel> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select p.* From Pixel as p " +
                    "inner join Affiliate as a on p.affiliateID=a.affiliateID " +
                    "where a.Code=@affiliateCode and a.Active=1 and p.Active=1");
                q.Parameters.Add("@affiliateCode", MySqlDbType.VarChar).Value = affiliateCode;

                res = dao.Load<Pixel>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Pixel> GetPixelListForPageByAffiliate(int pageID, string affiliateCode)
        {
            IList<Pixel> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select p.* From Pixel as p " +
                    "inner join Affiliate as a on p.affiliateID=a.affiliateID " +
                    "inner join PixelPage as pp on p.pixelID=pp.pixelID " +
                    "where a.Code=@affiliateCode and a.Active=1 and p.Active=1 and pp.PageID=@pageID");
                q.Parameters.Add("@affiliateCode", MySqlDbType.VarChar).Value = affiliateCode;
                q.Parameters.Add("@pageID", MySqlDbType.Int32).Value = pageID;

                res = dao.Load<Pixel>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public string GetPixelsString(string affiliateCode, int pageID, string clickID, long? billingID)
        {
            string res = null;
            try
            {                
                IList<Pixel> pxList = GetPixelListForPageByAffiliate(pageID, affiliateCode);
                if (pxList == null)
                {
                    throw new Exception(string.Format("Can't load Pixel List for PageID({0}) by AffiliateCode({1})", pageID, affiliateCode));
                }
                
                res = string.Empty;
                foreach (Pixel p in pxList)
                {
                    res += p.Code;
                }

                string sBillingID = (billingID != null) ? billingID.Value.ToString() : "0";

                res = res.Replace(System.Environment.NewLine, string.Empty);
                res = res.Replace("##ClickID##", clickID ?? string.Empty);
                res = res.Replace("##BillingID##", sBillingID);

                //Confirm pages
                if (billingID != null && (pageID == 3 || pageID == 4) && res.Contains("##PRICE##"))
                {
                    MySqlCommand q = new MySqlCommand("select s.* From BillingSubscription bs " + 
                        "inner join Subscription s on s.SubscriptionID = bs.SubscriptionID " +
                        "where bs.BillingID = @billingID");
                    q.Parameters.Add("@pageID", MySqlDbType.Int64).Value = billingID;

                    Subscription s = dao.Load<Subscription>(q).FirstOrDefault();

                    if (s != null)
                    {
                        string price = Utility.Add(s.InitialBillAmount, s.InitialShipping).ToString();
                        res = res.Replace("##PRICE##", price);
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

        public IList<USState> GetUSStates()
        {
            IList<USState> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from USState " +
                    "where ListAtEnd=0 or ListAtEnd=1 " +
                    "order by ListAtEnd asc, FullName asc");

                res = dao.Load<USState>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Country> GetCountries()
        {
            IList<Country> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select c.* from Country c " +
                    "order by c.Name");

                res = dao.Load<Country>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public TempConversion AddStatistic(int pageTypeID, int campaignID, string affiliateCode, string subAffiliateCode)
        {
            TempConversion res = null;
            try
            {
                dao.BeginTransaction();

                res = new TempConversion()
                {
                    PageTypeID = pageTypeID,
                    Affiliate = affiliateCode,
                    SubAffiliate = subAffiliateCode,
                    CreateDT = DateTime.Now,
                    CampaignID = campaignID,                    
                };

                dao.Save<TempConversion>(res);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res = null;
            }
            return res;
        }

        public IList<Conversion> GetStatusticsIndependentOnCompany(DateTime? startDate, DateTime? endDate)
        {
            IList<Conversion> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select null as ConversionID, c.PageTypeID, c.Affiliate, c.SubAffiliate, c.CreateDT, null as Flow, sum(c.Hits) as Hits, null as CampaignID, c.Hour " +
                    "from Conversion c " +
                    "where (@startDate is null or c.CreateDT >= @startDate) and (@endDate is null or c.CreateDT <= @endDate) " +
                    "group by c.PageTypeID, c.Affiliate, c.SubAffiliate, c.CreateDT, c.Hour");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate;

                res = dao.Load<Conversion>(q);

                q = new MySqlCommand("select null as ConversionID, c.PageTypeID, c.Affiliate, c.SubAffiliate, c.CreateDT, null as Flow, count(c.TempConversionID) as Hits, null as CampaignID, c.Hour " +
                    "from TempConversion c " +
                    "where (@startDate is null or c.CreateDT >= @startDate) and (@endDate is null or c.CreateDT <= @endDate) " +
                    "group by c.PageTypeID, c.Affiliate, c.SubAffiliate, c.CreateDT, c.Hour");
                q.Parameters.Add("@startDate", MySqlDbType.Timestamp).Value = startDate;
                q.Parameters.Add("@endDate", MySqlDbType.Timestamp).Value = endDate;

                res = res.Union(dao.Load<Conversion>(q)).
                    OrderBy(c => c.Affiliate).
                    OrderBy(c => c.SubAffiliate).
                    OrderBy(c => c.CreateDT).
                    OrderBy(c => c.Hour).
                    ToList();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }
    }
}
