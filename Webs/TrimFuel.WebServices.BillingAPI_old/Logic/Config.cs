using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.WebServices.BillingAPI_old.Configuration;
using System.Configuration;

namespace TrimFuel.WebServices.BillingAPI_old.Logic
{
    public class Config
    {
        private const string BILLING_API_CONFIG_KEY = "BillingAPIConfig";
        public static BillingAPIConfigurationElement Current 
        {
            get 
            {
                if (HttpContext.Current.Items[BILLING_API_CONFIG_KEY] == null)
                {
                    TrimFuelExtensionSection section = (TrimFuelExtensionSection)ConfigurationManager.GetSection("TrimFuelExtension");
                    if (section == null)
                    {
                        throw new Exception("Configuration error. Please contact support.");
                    }
                    if (section.Applications == null)
                    {
                        throw new Exception("Configuration error. Please contact support.");
                    }
                    AppSettingsExtension app = section.Applications[TrimFuel.Business.Config.Current.APPLICATION_ID];
                    if (app == null)
                    {
                        throw new Exception("Configuration error. Please contact support.");
                    }
                    if (app.BillingAPI == null)
                    {
                        throw new Exception("Configuration error. Please contact support.");
                    }

                    HttpContext.Current.Items[BILLING_API_CONFIG_KEY] = app.BillingAPI;
                }
                return (BillingAPIConfigurationElement)HttpContext.Current.Items[BILLING_API_CONFIG_KEY];
            }
        }
    }
}
