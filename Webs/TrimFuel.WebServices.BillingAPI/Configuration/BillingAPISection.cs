using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using TrimFuel.Business.Configuration;

namespace TrimFuel.WebServices.BillingAPI.Configuration
{
    public class TrimFuelExtensionSection : ConfigurationSection
    {
        [ConfigurationProperty("Applications", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(AppSettingsExtensionCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public AppSettingsExtensionCollection Applications
        {
            get { return (AppSettingsExtensionCollection)base["Applications"]; }
        }
    }

    public class AppSettingsExtension : ConfigurationElement
    {
        [ConfigurationProperty("ApplicationId", IsRequired = true, IsKey = true)]
        public string ApplicationId
        {
            get { return (string)base["ApplicationId"]; }
        }

        [ConfigurationProperty("BillingAPI", IsRequired = true)]
        public BillingAPIConfigurationElement BillingAPI
        {
            get { return (BillingAPIConfigurationElement)base["BillingAPI"]; }
        }
    }

    public class AppSettingsExtensionCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AppSettingsExtension();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AppSettingsExtension)element).ApplicationId;
        }

        new public AppSettingsExtension this[string applicationId]
        {
            get
            {
                return (AppSettingsExtension)BaseGet(applicationId);
            }
        }
    }

    public class BillingAPIConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("TPClientID", IsRequired = true)]
        [IntegerValidator()]
        public int TPClientID
        {
            get { return (int)base["TPClientID"]; }
        }

        [ConfigurationProperty("MasterDB", IsRequired = false)]
        public MasterDBConfigurationElement MasterDB
        {
            get { return (MasterDBConfigurationElement)base["MasterDB"]; }
        }

        [ConfigurationProperty("Username", IsRequired = false)]
        public string Username
        {
            get { return (string)base["Username"]; }
        }

        [ConfigurationProperty("Password", IsRequired = false)]
        public string Password
        {
            get { return (string)base["Password"]; }
        }
    }

    public class MasterDBConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("ConnectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)base["ConnectionString"]; }
        }
    }
}
