using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using log4net;
using TrimFuel.Business.Dao;
using TrimFuel.Model.Views;

public partial class _Default : System.Web.UI.Page 
{
    protected static readonly ILog logger = LogManager.GetLogger(typeof(BaseService));

    private const string DEFAULT_AFF = "*";
    private Dictionary<string, int> PRODUCT_MAP = new Dictionary<string, int>() { 
        { "TrimFuel", 1 },
        { "AcaiFuelExtreme", 2 },
        { "ResV", 3 },
        { "AcaiFuelCleanse", 4 },
        { "ExtremeCleanseCombo", 5 },
        { "ResVCleanseCombo", 6 },
        { "AcaiMonster", 7 },
        { "NatureRenewCleanse", 8 },
        { "ExtremeResvCombo", 9 },
        { "Ecigarettes", 10 },
        { "CelebritySmile", 11 },
        { "MaleEnhancement", 12 },
        { "EcigarettesAuth", 13 },
        { "EcigarettesGBP", 14 },
        { "EcigarettesEuro", 15 },
        { "DietAuth", 16 },
        { "MaleEnhancementAuth", 17 }
    };

    protected void Page_Load(object sender, EventArgs e)
    {
        string product = Utility.TryGetStr(Request["product"]);
        string aff = Utility.TryGetStr(Request["aff"]);
        string sub = Utility.TryGetStr(Request["sub"]);

        if (product != null && PRODUCT_MAP.ContainsKey(product))
        {
            string url = null;
            if (!string.IsNullOrEmpty(aff))
            {
                url = GetRedirectURL(PRODUCT_MAP[product], aff);
            }
            //Try default if URL was not found
            if (string.IsNullOrEmpty(url))
            {
                url = GetRedirectURL(PRODUCT_MAP[product], DEFAULT_AFF);
            }
            if (!string.IsNullOrEmpty(url))
            {
                Response.Redirect(string.Format("{0}?aff={1}&sub={2}", url, aff, sub));
            }
        }
    }

    private string GetRedirectURL(int productID, string aff)
    {
        string res = null;
        try
        {
            IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            MySqlCommand q = new MySqlCommand("select c.URL, arm.Percentage from AffiliateRoutingMap arm " +
                "inner join Campaign c on c.CampaignID = arm.CampaignID " +
                "where arm.ProductID = @productID and arm.Affiliate = @affiliate");
            q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
            q.Parameters.Add("@affiliate", MySqlDbType.VarChar).Value = aff;

            IList<CampaignRoutingView> list = dao.Load<CampaignRoutingView>(q);
            if (list != null && list.Count > 0)
            {
                CampaignRoutingView resCamp = Utility.GetRandom<CampaignRoutingView>(new Random(), 
                    list.ToDictionary(i => Convert.ToInt32(i.Percentage)));

                if (resCamp != null)
                {
                    res = resCamp.URL;
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
}
