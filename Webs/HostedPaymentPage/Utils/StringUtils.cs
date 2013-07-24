using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace Payment_test.Utils
{
    public static class StringUtils
    {
        public static bool IsDomainRecord(this string text, bool httpRequire = false)
        {
            bool isMatch = true;
            if (httpRequire)
            {
                isMatch = isMatch & Regex.IsMatch(text, @"^(ht|f)tp(s?)\:\/\/[0-9a-zA-Z]([-.\w]*[0-9a-zA-Z])*(:(0-9)*)*(\/?)([a-zA-Z0-9\-\.\?\,\'\/\\\+&amp;%\$#_]*)?$", RegexOptions.IgnoreCase);
            }
            else
            {
                isMatch = isMatch & Regex.IsMatch(text, @"^(www\.)?([\w\.\-]+)\.\w{2,4}$", RegexOptions.IgnoreCase);
            }

            isMatch = isMatch & Regex.IsMatch(text, @"\.[^0-9]\w+$", RegexOptions.IgnoreCase);
            isMatch = isMatch & (!Regex.IsMatch(text, @"(\.\.)+"));

            return isMatch;
        }

        public static bool IsNumber(this string text)
        {
            bool isMatch = true;
            isMatch = isMatch & Regex.IsMatch(text, "^[0-9.]*$", RegexOptions.IgnoreCase);
            return isMatch;
        }
    }
}