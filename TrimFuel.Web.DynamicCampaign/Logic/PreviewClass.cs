using System;
using System.Collections.Generic;
using System.Web;
using System.IO;

namespace TrimFuel.Web.DynamicCampaign.Logic
{
    public class PreviewClass
    {
        private static readonly string path = AppConfigSettings.TemplatesDirectoryPath + "preview";

        public static string HTML
        {
            set
            {
                File.WriteAllText(path + ".html", value);
            }
            get
            {
                return File.ReadAllText(path + ".html");
            }
        }

        public static string Header
        {
            set
            {
                File.WriteAllText(path + ".header", value);
            }
            get
            {
                return File.ReadAllText(path + ".header");
            }
        }
    }
}
