using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TrimFuel.Business.Utils
{
    public class Counter
    {
        private const string COOKIE_BASE_KEY = "lastVisited";
        private const string COOKIE_VALUE= "true";

        private string CounterCookieKey
        {
            get
            {
                return string.Format("{0}{1}_{2}", COOKIE_BASE_KEY, PageTypeID, CampaignID);
            }
        }

        public int PageTypeID { get; set; }
        public int CampaignID { get; set; }
        public string AffiliateCode { get; set; }
        public string SubAffiliateCode { get; set; }

        private bool CookieExists
        {
            get
            {
                HttpCookie cookie = HttpContext.Current.Request.Cookies[CounterCookieKey];

                if (cookie != null)
                {
                    return true;
                }

                return false;
            }
        }

        private void AddCookie()
        {
            HttpCookie cookie = new HttpCookie(CounterCookieKey, COOKIE_VALUE);
            cookie.Expires = DateTime.Now.AddMonths(1).AddMinutes(5);

            HttpContext.Current.Response.AppendCookie(cookie);
        }

        public void AddStatistic()
        {
            if (!CookieExists)
            {
                AddCookie();

                (new PageService()).AddStatistic(PageTypeID, CampaignID, AffiliateCode, SubAffiliateCode);
            }
        }
    }
}
