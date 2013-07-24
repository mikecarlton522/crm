using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;

namespace TrimFuel.Business.Utils
{
    public static class HtmlHelper
    {
        //TODO: in config
        public const string IMAGE_URL = "http://d3oimv5qppjae2.cloudfront.net/";

        public static string DDL(IEnumerable<KeyValuePair<string, string>> valueList, string selectedValue)
        {
            string res = string.Empty;

            foreach (KeyValuePair<string, string> item in valueList)
            {
                res += string.Format("<option value='{0}' {1}>{2}</option>", item.Key, (item.Key == selectedValue) ? "selected" : string.Empty, item.Value);
            }

            return res;
        }

        public static string DDLStateFullName(string selectedStateShortName)
        {
            IList<USState> states = (new PageService()).GetUSStates();
            if (states == null)
                return null;

            return DDL(states.Select(s => new KeyValuePair<string, string>(s.ShortName, s.FullName)), selectedStateShortName);
        }

        //public static string DDLCountry(string selectedCountry)
        //{
        //    IList<Countr> states = (new PageService()).GetUSStates();
        //    if (states == null)
        //        return null;

        //    return DDL(states.Select(s => new KeyValuePair<string, string>(s.ShortName, s.FullName)), selectedStateShortName);
        //}

        public static string DDLMonth(int? selectedMonth)
        {
            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                {"1", "Jan (1)"},
                {"2", "Feb (2)"},
                {"3", "Mar (3)"},
                {"4", "Apr (4)"},
                {"5", "May (5)"},
                {"6", "Jun (6)"},
                {"7", "Jul (7)"},
                {"8", "Aug (8)"},
                {"9", "Sep (9)"},
                {"10", "Oct (10)"},
                {"11", "Nov (11)"},
                {"12", "Dec (12)"}
            };
            return DDL(values, (selectedMonth != null) ? selectedMonth.Value.ToString() : null);
        }

        public static string DDLYear(int startYear, int endYear, int? selectedYear)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();
            for (int i = startYear; i < endYear + 1; i++)
            {
                values.Add(i.ToString(), i.ToString());
            }
            return DDL(values, (selectedYear != null) ? selectedYear.Value.ToString() : null);
        }

        private const int DDL_YEAR_DEFAULT_START = 2010;
        private const int DDL_YEAR_DEFAULT_END = 2025;
        public static string DDLYear(int? selectedYear)
        {
            return DDLYear(DDL_YEAR_DEFAULT_START, DDL_YEAR_DEFAULT_END, selectedYear);
        }

        public static string DDLPaymentType(int? selectedPaymentTypeID)
        {
            Dictionary<string, string> values = new Dictionary<string, string>()
            {
                {"2", "Visa"},
                {"3", "MasterCard"}
            };
            return DDL(values, (selectedPaymentTypeID != null) ? selectedPaymentTypeID.Value.ToString() : null);
        }

        public static string Pixels(string affiliateCode, int pageID, string clickID, long? billingID)
        {
            return (new PageService()).GetPixelsString(affiliateCode, pageID, clickID, billingID);
        }

        public static string Pixels(string affiliateCode, int[] pageIDs, string clickID, long? billingID)
        {
            StringBuilder res = new StringBuilder();
            foreach (var pageID in pageIDs)
                res.AppendLine(Pixels(affiliateCode, pageID, clickID, billingID));

            return res.ToString();
        }

        public static void Counter(int pageTypeID, int campaignID, string affiliateCode, string subAffiliateCode)
        {
            Counter c = new Counter()
            {
                PageTypeID = pageTypeID,
                CampaignID = campaignID,
                AffiliateCode = affiliateCode,
                SubAffiliateCode = subAffiliateCode
            };

            c.AddStatistic();
        }

        public static string FormatPrice(decimal? price)
        {
            if (price != null)
            {
                return price.Value.ToString("0.00");
            }
            return null;
        }
    }
}
