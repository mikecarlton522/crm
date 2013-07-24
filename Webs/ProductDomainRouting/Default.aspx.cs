using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Business.Dao;
using log4net;
using TrimFuel.Business;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;

namespace ProductDomainRouting
{
    public partial class _Default : System.Web.UI.Page
    {
        protected static readonly ILog logger = LogManager.GetLogger(typeof(BaseService));

        protected void Page_Load(object sender, EventArgs e)
        {
            string affiliate = Utility.TryGetStr(Request["aff"]) ?? string.Empty;
            string subAffiliate = Utility.TryGetStr(Request["sub"]) ?? string.Empty;

            var url = GetRedirectURL(affiliate, subAffiliate);
            if (!string.IsNullOrEmpty(url))
            {
                Response.Redirect(url);
            }
        }

        private string GetRedirectURL(string affiliate, string subAffiliate)
        {
            string res = null;
            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
                MySqlCommand q = new MySqlCommand("select c.URL, pdr.ExtUrl, pdr.Percentage, pdr.SubAffiliate, pdr.Affiliate from ProductDomainRouting pdr " +
                    " left join Campaign c on c.CampaignID = pdr.CampaignID " +
                    " inner join ProductDomain pd on pd.ProductDomainID = pdr.ProductDomainID " +
                    " where pd.DomainName = @domain" +
                    " or pd.DomainName = @wwwdomain" +
                    " and (" +
                    " pdr.Affiliate=@Affiliate and pdr.SubAffiliate=@SubAffiliate" +
                    " or pdr.Affiliate=@Affiliate and pdr.SubAffiliate='*'" +
                    " or pdr.Affiliate='*' and pdr.SubAffiliate=@SubAffiliate" +
                    " or pdr.Affiliate='*' and pdr.SubAffiliate='*')");

                q.Parameters.Add("@domain", MySqlDbType.VarChar).Value = GetDomain();
                q.Parameters.Add("@wwwdomain", MySqlDbType.VarChar).Value = "www." + GetDomain();
                q.Parameters.Add("@Affiliate", MySqlDbType.VarChar).Value = affiliate;
                q.Parameters.Add("@SubAffiliate", MySqlDbType.VarChar).Value = subAffiliate;

                IList<CampaignRoutingView> list = dao.Load<CampaignRoutingView>(q);

                if (list != null && list.Count > 0)
                {
                    IList<CampaignRoutingView> tmpCList = null; ;
                    tmpCList = GetFilteredList(list, affiliate, subAffiliate);
                    if (tmpCList.Count == 0)
                    {
                        tmpCList = GetFilteredList(list, affiliate, "*");
                        if (tmpCList.Count == 0)
                        {
                            tmpCList = GetFilteredList(list, "*", subAffiliate);
                            if (tmpCList.Count == 0)
                                list = GetFilteredList(list, "*", "*");
                            else
                                list = tmpCList;
                        }
                        else
                        {
                            list = tmpCList;
                        }
                    }
                    else
                    {
                        list = tmpCList;
                    }
                }

                if (list != null && list.Count > 0)
                {
                    CampaignRoutingView resCamp = Utility.GetRandom<CampaignRoutingView>(new Random(),
                        list.Select(u => new KeyValuePair<int, CampaignRoutingView>(Convert.ToInt32(u.Percentage), u)));

                    if (resCamp != null)
                    {
                        res = string.IsNullOrEmpty(resCamp.URL) ? resCamp.ExtUrl : resCamp.URL;
                        res = res.Replace("#AFFILIATE#", affiliate).Replace("#SUBAFFILIATE#", subAffiliate);
                        if (!res.Contains("http"))
                            res = "http://" + res;
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

        private string GetDomain()
        {
            string lowerDomain = Request.Url.Host;
            if (lowerDomain.IndexOf("www.") == 0)
                lowerDomain = lowerDomain.Remove(0, 4);
            return lowerDomain;
        }

        private IList<CampaignRoutingView> GetFilteredList(IList<CampaignRoutingView> list, string affiliate, string subAffiliate)
        {
            return list.Where(u => u.Affiliate.ToLower() == affiliate.ToLower() && u.SubAffiliate.ToLower() == subAffiliate.ToLower()).ToList();
        }
    }
}
