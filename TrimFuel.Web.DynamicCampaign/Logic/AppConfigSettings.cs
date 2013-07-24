using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;

namespace TrimFuel.Web.DynamicCampaign.Logic
{
    public class AppConfigSettings
    {
        public const string CookieName = "HitsCookie";

        //public static string RelativePath
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["RelativeToRootPath"];
        //    }
        //}

        //public static string TemplatesDirectoryPath
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["TemplatesDirectoryPath"];
        //    }
        //}

        //public static string HostName
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["HostName"];
        //    }
        //}

        public static bool UseHTTPS
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["UseHTTPS"]);
            }

        }

        //public static bool TempConversion
        //{
        //    get
        //    {
        //        return bool.Parse(ConfigurationManager.AppSettings["TempConversion"]);
        //    }

        //}

        //public const string UpsellCustomPixel_mwresv_20527 = @"<img src='http://www.infitrax.com/adLead.php?AFID=[AFID]&CID=99' width='1' height='1' border='0'>";

        //public static string AssertigyURL
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["AssertigyURL"];
        //    }
        //}

        //public static string STO_URL
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["STO_URL"];
        //    }
        //}
    }
}
