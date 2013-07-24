using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace TrimFuel.Business.Configuration
{
    public class TrimFuelSection : ConfigurationSection
    {
        public enum AppAuthModeEnum
        {
            Static = 1,
            WebHostName = 2
        }

        [ConfigurationProperty("AppAuthMode", IsRequired = true)]
        public AppAuthModeEnum AppAuthMode 
        {
            get { return (AppAuthModeEnum)base["AppAuthMode"]; }
        }

        [ConfigurationProperty("StaticAppAuth", IsRequired = false)]
        public StaticAppAuthConfigurationElement StaticAppAuth
        {
            get { return (StaticAppAuthConfigurationElement)base["StaticAppAuth"]; }
        }

        [ConfigurationProperty("Applications", IsDefaultCollection = true)]
        [ConfigurationCollection(typeof(AppSettingsCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public AppSettingsCollection Applications
        {
            get { return (AppSettingsCollection)base["Applications"]; }
        }
        
    }

    public class AppSettings : ConfigurationElement
    {
        [ConfigurationProperty("ApplicationId", IsRequired = true, IsKey = true)]
        public string ApplicationId
        {
            get { return (string)base["ApplicationId"]; }
        }

        [ConfigurationProperty("ConnectionStrings", IsRequired = true)]
        [ConfigurationCollection(typeof(ConnectionStringSettingsCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ConnectionStringSettingsCollection ConnectionStrings 
        {
            get { return (ConnectionStringSettingsCollection)base["ConnectionStrings"]; }
        }

        [ConfigurationProperty("Shipping", IsRequired = false)]
        public ShippingConfigurationElement Shipping
        {
            get { return (ShippingConfigurationElement)base["Shipping"]; }
        }

        [ConfigurationProperty("EmailServer", IsRequired = false)]
        public GatewayConfigurationElement EmailServer
        {
            get { return (GatewayConfigurationElement)base["EmailServer"]; }
        }

        [ConfigurationProperty("NMI", IsRequired = false)]
        public GatewayConfigurationElement NMI
        {
            get { return (GatewayConfigurationElement)base["NMI"]; }
        }

        [ConfigurationProperty("MSC", IsRequired = false)]
        public GatewayConfigurationElement MSC
        {
            get { return (GatewayConfigurationElement)base["MSC"]; }
        }

        [ConfigurationProperty("CBG", IsRequired = false)]
        public CBGGatewayConfigurationElement CBG
        {
            get { return (CBGGatewayConfigurationElement)base["CBG"]; }
        }
        
        [ConfigurationProperty("MPS", IsRequired = false)]
        public GatewayConfigurationElement MPS
        {
            get { return (GatewayConfigurationElement)base["MPS"]; }
        }

        [ConfigurationProperty("IPG", IsRequired = false)]
        public GatewayConfigurationElement IPG
        {
            get { return (GatewayConfigurationElement)base["IPG"]; }
        }

        [ConfigurationProperty("Pagador", IsRequired = false)]
        public GatewayConfigurationElement Pagador
        {
            get { return (GatewayConfigurationElement)base["Pagador"]; }
        }

        [ConfigurationProperty("PrismPay", IsRequired = false)]
        public GatewayConfigurationElement PrismPay
        {
            get { return (GatewayConfigurationElement)base["PrismPay"]; }
        }

        [ConfigurationProperty("EMP", IsRequired = false)]
        public GatewayConfigurationElement EMP
        {
            get { return (GatewayConfigurationElement)base["EMP"]; }
        }

        [ConfigurationProperty("SHW", IsRequired = false)]
        public GatewayConfigurationElement SHW
        {
            get { return (GatewayConfigurationElement)base["SHW"]; }
        }

        [ConfigurationProperty("BadCustomer", IsRequired = false)]
        public GatewayConfigurationElement BadCustomer
        {
            get { return (GatewayConfigurationElement)base["BadCustomer"]; }
        }

        [ConfigurationProperty("MaxMind", IsRequired = false)]
        public GatewayConfigurationElement MaxMind
        {
            get { return (GatewayConfigurationElement)base["MaxMind"]; }
        }

        [ConfigurationProperty("ABF", IsRequired = false)]
        public ABFConfigurationElement ABF
        {
            get { return (ABFConfigurationElement)base["ABF"]; }
        }

        [ConfigurationProperty("TF", IsRequired = false)]
        public ABFConfigurationElement TF
        {
            get { return (ABFConfigurationElement)base["TF"]; }
        }

        [ConfigurationProperty("AtLastFulfillment", IsRequired = false)]
        public AtLastFulfillmentConfigurationElement AtLastFulfillment
        {
            get { return (AtLastFulfillmentConfigurationElement)base["AtLastFulfillment"]; }
        }

        [ConfigurationProperty("MoldingBox", IsRequired = false)]
        public MoldingBoxConfigurationElement MoldingBox
        {
            get { return (MoldingBoxConfigurationElement) base["MoldingBox"]; }
        }

        [ConfigurationProperty("Keymail", IsRequired = false)]
        public KeymailConfigurationElement Keymail
        {
            get { return (KeymailConfigurationElement)base["Keymail"]; }
        }

        [ConfigurationProperty("Reports", IsRequired = false)]
        public ReportConfigurationElement Reports
        {
            get { return (ReportConfigurationElement)base["Reports"]; }
        }

        [ConfigurationProperty("NPF", IsRequired = false)]
        public NPFConfigurationElement NPF
        {
            get { return (NPFConfigurationElement)base["NPF"]; }
        }

        [ConfigurationProperty("GoFulfillment", IsRequired = false)]
        public GoFulfillmentConfigurationElement GoFulfillment
        {
            get { return (GoFulfillmentConfigurationElement)base["GoFulfillment"]; }
        }

        //[ConfigurationProperty("AdminRestrictLevels", IsDefaultCollection = false)]
        //[ConfigurationCollection(typeof(AdminRestrictLevelCollection),
        //    AddItemName = "add",
        //    ClearItemsName = "clear",
        //    RemoveItemName = "remove")]
        //public AdminRestrictLevelCollection AdminRestrictLevels
        //{
        //    get { return (AdminRestrictLevelCollection)base["AdminRestrictLevels"]; }
        //}
    }

    public class AppSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new AppSettings();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((AppSettings)element).ApplicationId;
        }

        new public AppSettings this[string applicationId]
        {
            get
            {
                return (AppSettings)BaseGet(applicationId);
            }
        }
    }

    public class AdminRestrictLevel : ConfigurationElement
    {
        [ConfigurationProperty("RestrictLevel", IsRequired = true, IsKey = true)]
        [IntegerValidator()]
        public int RestrictLevel
        {
            get { return (int)base["RestrictLevel"]; }
        }

        [ConfigurationProperty("Name", IsRequired = true)]
        public string Name
        {
            get { return (string)base["Name"]; }
        }
    }

    //public class AdminRestrictLevelCollection : ConfigurationElementCollection
    //{
    //    protected override ConfigurationElement CreateNewElement()
    //    {
    //        return new AdminRestrictLevel();
    //    }

    //    protected override object GetElementKey(ConfigurationElement element)
    //    {
    //        return ((AdminRestrictLevel)element).RestrictLevel;
    //    }

    //    public AdminRestrictLevel this[int restrictLevel]
    //    {
    //        get
    //        {
    //            return (AdminRestrictLevel)BaseGet(restrictLevel);
    //        }
    //    }
    //}

    public class ShippingConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("TestMode", IsRequired=true)]
        public bool TestMode 
        {
            get { return (bool)base["TestMode"]; }
        }
    }

    public class GatewayConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("URL", IsRequired = true)]
        public string URL
        {
            get { return (string)base["URL"]; }
        }
    }

    public class CBGGatewayConfigurationElement : GatewayConfigurationElement
    {
        [ConfigurationProperty("ProductID", IsRequired = false)]
        public string ProductID
        {
            get { return (string)base["ProductID"]; }
        }
    }

    public class ABFConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("TestMode", IsRequired = true)]
        public bool TestMode
        {
            get { return (bool)base["TestMode"]; }
        }

        [ConfigurationProperty("ThreePLKey", IsRequired = false)]
        public string ThreePLKey
        {
            get { return (string)base["ThreePLKey"]; }
        }

        [ConfigurationProperty("Login", IsRequired = false)]
        public string Login
        {
            get { return (string)base["Login"]; }
        }

        [ConfigurationProperty("Password", IsRequired = false)]
        public string Password
        {
            get { return (string)base["Password"]; }
        }

        [ConfigurationProperty("FacilityID", IsRequired = false)]
        public int? FacilityID
        {
            get { return (int?)base["FacilityID"]; }
        }

        [ConfigurationProperty("ThreePLID", IsRequired = false)]
        public int? ThreePLID
        {
            get { return (int?)base["ThreePLID"]; }
        }
    }

    public class GoFulfillmentConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("TestMode", IsRequired = true)]
        public bool TestMode
        {
            get { return (bool)base["TestMode"]; }
        }

        [ConfigurationProperty("URL", IsRequired = false)]
        public string URL
        {
            get { return (string)base["URL"]; }
        }

        [ConfigurationProperty("OriginId", IsRequired = false)]
        public string OriginId
        {
            get { return (string)base["OriginId"]; }
        }

        [ConfigurationProperty("UserId", IsRequired = false)]
        public string UserId
        {
            get { return (string)base["UserId"]; }
        }

        [ConfigurationProperty("Password", IsRequired = false)]
        public string Password
        {
            get { return (string)base["Password"]; }
        }
    }

    public class NPFConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("TestMode", IsRequired = true)]
        public bool TestMode
        {
            get { return (bool)base["TestMode"]; }
        }

        [ConfigurationProperty("URL", IsRequired = false)]
        public string URL
        {
            get { return (string)base["URL"]; }
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

    public class AtLastFulfillmentConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("TestMode", IsRequired = true)]
        public bool TestMode
        {
            get { return (bool)base["TestMode"]; }
        }

        [ConfigurationProperty("ApiKey", IsRequired = false)]
        public string ApiKey
        {
            get { return (string)base["ApiKey"]; }
        }
    }

    public class MoldingBoxConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("TestMode", IsRequired = true)]
        public bool TestMode
        {
            get { return (bool)base["TestMode"]; }
        }

        [ConfigurationProperty("ApiKey", IsRequired = true)]
        public string ApiKey
        {
            get { return (string)base["ApiKey"]; }
        }
    }

    public class StaticAppAuthConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("ApplicationId", IsRequired = true)]
        public string ApplicationId
        {
            get { return (string)base["ApplicationId"]; }
        }
    }

    public class KeymailConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("Emails", IsRequired = true)]
        public string Emails
        {
            get { return (string)base["Emails"]; }
        }
    }

    public class ReportConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("DailyShipmentsEmails", IsRequired = true)]
        public string DailyShipmentsEmails
        {
            get { return (string)base["DailyShipmentsEmails"]; }
        }
    }
}
