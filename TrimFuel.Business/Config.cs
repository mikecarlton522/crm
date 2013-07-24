using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Configuration;
using System.Configuration;
using System.Web;
using TrimFuel.Model;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class Config
    {
        public static Config Current
        {
            get
            {
                Config config = LoadFromStorage();
                if (config == null)
                {
                    config = new Config();
                    config.Load();
                    SaveToStorage(config);
                }
                return config;
            }
        }

        private static Config LoadFromStorage()
        {
            if (HttpContext.Current != null)
            {
                return (Config) HttpContext.Current.Items["TrimFuelConfigurationObject"];
            }
            return LoadFromStaticStorage();
        }

        private static void SaveToStorage(Config config)
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Items["TrimFuelConfigurationObject"] = config;
            }
            else
            {
                SaveToStaticStorage(config);
            }
        }

        private static Config staticConfigStorage = null;

        private static Config LoadFromStaticStorage()
        {
            return staticConfigStorage;
        }

        private static void SaveToStaticStorage(Config config)
        {
            staticConfigStorage = config;
        }

        public IDictionary<string, string> CONNECTION_STRINGS { get; private set; }
        public string CURRENT_CONNECTION_STRING { get; set; }
        public string APPLICATION_ID { get; private set; }
        public string EMAIL_GATEWAY_URL { get; private set; }
        public string NETWORKMERCHANTS_URL { get; private set; }
        public string MSC_URL { get; private set; }
        public string MPS_URL { get; private set; }
        public string IPG_URL { get; private set; }
        public string PAGADOR_URL { get; private set; }
        public string PRISM_PAY_URL { get; private set; }
        public string EMP_URL { get; private set; }
        public string SHW_GATEWAY_URL { get; private set; }
        public string BAD_CUSTOMER_URL { get; set; }
        public string MAX_MIND_URL { get; private set; }
        public string CBG_URL { get; private set; }
        public string CBG_PRODUCT_ID { get; private set; }

        public bool SHIPPING_TEST_MODE { get; private set; }

        //2012-02-13 Config from DB should be used instead
        //public bool ABF_TEST_MODE { get; private set; }
        //public string ABF_THREE_PL_KEY { get; private set; }
        //public int? ABF_THREE_PL_ID { get; private set; }
        //public string ABF_LOGIN { get; private set; }
        //public string ABF_PASSWORD { get; private set; }
        //public int? ABF_FACILITY_ID { get; private set; }

        //2012-02-13 Config from DB should be used instead
        //public bool TF_TEST_MODE { get; private set; }
        //public string TF_THREE_PL_KEY { get; private set; }
        //public int? TF_THREE_PL_ID { get; private set; }
        //public string TF_LOGIN { get; private set; }
        //public string TF_PASSWORD { get; private set; }
        //public int? TF_FACILITY_ID { get; private set; }

        //2012-02-13 Config from DB should be used instead
        //public string[] KEYMAIL_EMAIL_ADDRESS { get; private set; }

        public string[] DAILY_SHIPMENTS_EMAILS { get; private set; }

        //public IDictionary<int, string> ADMIN_RESTRICT_LEVELS { get; private set; }

        //2012-02-13 Config from DB should be used instead
        //public bool AT_LAST_FULFILLMENT_TEST_MODE { get; private set; }
        //public string AT_LAST_FULFILLMENT_API_KEY { get; private set; }
        //public bool MOLDING_BOX_TEST_MODE { get; private set; }
        //public string MOLDING_BOX_API_KEY { get; private set; }

        //public bool GO_FULFILLMENT_TEST_MODE { get; private set; }
        //public string GO_FULFILLMENT_URL { get; private set; }
        //public string GO_FULFILLMENT_ORIGINID { get; private set; }
        //public string GO_FULFILLMENT_USERID { get; private set; }
        //public string GO_FULFILLMENT_PASSWORD { get; private set; }

        //public bool NPF_TEST_MODE { get; private set; }
        //public string NPF_URL { get; private set; }
        //public string NPF_USERNAME { get; private set; }
        //public string NPF_PASSWORD { get; private set; }

        // config settings from DB

        //public IDictionary<ShipperConfig.ID, string> ShipperConfig { get; private set; }

        //private void LoadShipperConfig()
        //{
        //    this.ShipperConfig = new Dictionary<ShipperConfig.ID, string>();
        //    var configs = new TrimFuel.Business.ShipperConfigService().GetAllConfigs();
        //    foreach (var config in configs)
        //    {
        //        this.ShipperConfig.Add(
        //            ShipperConfigList.Values.Where(u => (u.ShipperID == config.ShipperID) && (u.Key == config.Key)).
        //                SingleOrDefault(), config.Value);
        //    }
        //}

        // -----------------------

        protected void Load()
        {
            TrimFuelSection section = (TrimFuelSection) ConfigurationManager.GetSection("TrimFuel");
            if (section == null)
            {
                throw new Exception("TrimFuel configuration error. Can't find configuration section.");
            }
            if (section.Applications == null)
            {
                throw new Exception(
                    "TrimFuel configuration error. Can't find Applications configuration, please check 'Applications' section.");
            }

            if (section.AppAuthMode == TrimFuelSection.AppAuthModeEnum.Static)
            {
                if (section.StaticAppAuth == null)
                {
                    throw new Exception(
                        "TrimFuel configuration error. AppAuthMode is 'Static' but StaticAppAuth section was not found.");
                }
                AppSettings settings = section.Applications[section.StaticAppAuth.ApplicationId];
                if (settings == null)
                {
                    throw new Exception("TrimFuel configuration error. Can't find Application(" +
                                        section.StaticAppAuth.ApplicationId + ") specified in StaticAppAuth section.");
                }
                LoadApplication(settings);
            }
            else if (section.AppAuthMode == TrimFuelSection.AppAuthModeEnum.WebHostName)
            {
                if (HttpContext.Current == null)
                {
                    throw new Exception(
                        "TrimFuel configuration error. AppAuthMode is 'WebHostName' but HttpContext.Current is null.");
                }
                AppSettings settings = section.Applications[HttpContext.Current.Request.Url.Host.ToLower()];
                if (settings == null)
                {
                    throw new Exception("TrimFuel configuration error. Can't find Application(" +
                                        HttpContext.Current.Request.Url.Host.ToLower() + ").");
                }
                LoadApplication(settings);
            }
            else
            {
                throw new Exception("TrimFuel configuration error. Can't determine AppAuthMode.");
            }
        }

        protected void LoadApplication(AppSettings settings)
        {
            APPLICATION_ID = settings.ApplicationId;
            if (settings.ConnectionStrings != null)
            {
                CONNECTION_STRINGS = new Dictionary<string, string>();
                foreach (var item in settings.ConnectionStrings)
                {
                    CONNECTION_STRINGS.Add(((ConnectionStringSettings) item).Name,
                                           ((ConnectionStringSettings) item).ConnectionString);
                }
            }
            if (settings.EmailServer != null)
            {
                EMAIL_GATEWAY_URL = settings.EmailServer.URL;
            }
            if (settings.NMI != null)
            {
                NETWORKMERCHANTS_URL = settings.NMI.URL;
            }
            if (settings.MSC != null)
            {
                MSC_URL = settings.MSC.URL;
            }
            if (settings.MPS != null)
            {
                MPS_URL = settings.MPS.URL;
            }
            if (settings.IPG != null)
            {
                IPG_URL = settings.IPG.URL;
            }
            if (settings.Pagador != null)
            {
                PAGADOR_URL = settings.Pagador.URL;
            }
            if (settings.PrismPay != null)
            {
                PRISM_PAY_URL = settings.PrismPay.URL;
            }
            if (settings.EMP != null)
            {
                EMP_URL = settings.EMP.URL;
            }
            if (settings.SHW != null)
            {
                SHW_GATEWAY_URL = settings.SHW.URL;
            }
            if (settings.CBG != null)
            {
                CBG_URL = settings.CBG.URL;
                CBG_PRODUCT_ID = settings.CBG.ProductID;
            }
            if (settings.BadCustomer != null)
            {
                BAD_CUSTOMER_URL = settings.BadCustomer.URL;
            }
            if (settings.MaxMind != null)
            {
                MAX_MIND_URL = settings.MaxMind.URL;
            }
            if (settings.Shipping != null)
            {
                SHIPPING_TEST_MODE = settings.Shipping.TestMode;
            }
            else
            {
                SHIPPING_TEST_MODE = false;
            }
            //if (settings.ABF != null)
            //{
            //    ABF_TEST_MODE = settings.ABF.TestMode;
            //    if (settings.ABF.ThreePLKey != null)
            //    {
            //        ABF_THREE_PL_KEY = settings.ABF.ThreePLKey;
            //    }
            //    if (settings.ABF.ThreePLID != null)
            //    {
            //        ABF_THREE_PL_ID = settings.ABF.ThreePLID;
            //    }
            //    if (settings.ABF.Login != null)
            //    {
            //        ABF_LOGIN = settings.ABF.Login;
            //    }
            //    if (settings.ABF.Password != null)
            //    {
            //        ABF_PASSWORD = settings.ABF.Password;
            //    }
            //    if (settings.ABF.FacilityID != null)
            //    {
            //        ABF_FACILITY_ID = settings.ABF.FacilityID;
            //    }
            //}
            //if (settings.TF != null)
            //{
            //    TF_TEST_MODE = settings.TF.TestMode;
            //    if (settings.TF.ThreePLKey != null)
            //    {
            //        TF_THREE_PL_KEY = settings.TF.ThreePLKey;
            //    }
            //    if (settings.TF.ThreePLID != null)
            //    {
            //        TF_THREE_PL_ID = settings.TF.ThreePLID;
            //    }
            //    if (settings.TF.Login != null)
            //    {
            //        TF_LOGIN = settings.TF.Login;
            //    }
            //    if (settings.TF.Password != null)
            //    {
            //        TF_PASSWORD = settings.TF.Password;
            //    }
            //    if (settings.TF.FacilityID != null)
            //    {
            //        TF_FACILITY_ID = settings.TF.FacilityID;
            //    }
            //}
            //if (settings.AtLastFulfillment != null)
            //{
            //    AT_LAST_FULFILLMENT_TEST_MODE = settings.AtLastFulfillment.TestMode;
            //    if (settings.AtLastFulfillment.ApiKey != null)
            //    {
            //        AT_LAST_FULFILLMENT_API_KEY = settings.AtLastFulfillment.ApiKey;
            //    }
            //}
            //if (settings.MoldingBox != null)
            //{
            //    MOLDING_BOX_TEST_MODE = settings.MoldingBox.TestMode;
            //    if (settings.MoldingBox.ApiKey != null)
            //    {
            //        MOLDING_BOX_API_KEY = settings.MoldingBox.ApiKey;
            //    }
            //}

            //if (settings.Keymail != null)
            //{
            //    KEYMAIL_EMAIL_ADDRESS = settings.Keymail.Emails.Split(',');
            //}
            //if (settings.AdminRestrictLevels != null)
            //{
            //    ADMIN_RESTRICT_LEVELS = new Dictionary<int, string>();
            //    foreach (var item in settings.AdminRestrictLevels)
            //    {
            //        ADMIN_RESTRICT_LEVELS.Add(((AdminRestrictLevel)item).RestrictLevel, ((AdminRestrictLevel)item).Name);
            //    }
            //}
            if (settings.Reports != null)
            {
                DAILY_SHIPMENTS_EMAILS = settings.Reports.DailyShipmentsEmails.Split(',');
            }
            //if (settings.GoFulfillment != null)
            //{
            //    GO_FULFILLMENT_TEST_MODE = settings.GoFulfillment.TestMode;
            //    if (settings.GoFulfillment.URL != null)
            //    {
            //        GO_FULFILLMENT_URL = settings.GoFulfillment.URL;
            //    }
            //    if (settings.GoFulfillment.OriginId != null)
            //    {
            //        GO_FULFILLMENT_ORIGINID = settings.GoFulfillment.OriginId;
            //    }
            //    if (settings.GoFulfillment.UserId != null)
            //    {
            //        GO_FULFILLMENT_USERID = settings.GoFulfillment.UserId;
            //    }
            //    if (settings.GoFulfillment.Password != null)
            //    {
            //        GO_FULFILLMENT_PASSWORD = settings.GoFulfillment.Password;
            //    }
            //}
            //if (settings.NPF != null)
            //{
            //    NPF_TEST_MODE = settings.NPF.TestMode;
            //    if (settings.NPF.URL != null)
            //    {
            //        NPF_URL = settings.NPF.URL;
            //    }
            //    if (settings.NPF.Username != null)
            //    {
            //        NPF_USERNAME = settings.NPF.Username;
            //    }
            //    if (settings.NPF.Password != null)
            //    {
            //        NPF_PASSWORD = settings.NPF.Password;
            //    }
            //}
        }
    }
}
