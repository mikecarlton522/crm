using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using TrimFuel.Business.Configuration;

namespace ErrorLogApplication.Model
{
    public class Application : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsRequired = true, IsKey = true)]
        public string ApplicationName
        {
            get { return (string)base["Name"]; }
        }

        [ConfigurationProperty("LogPath", IsRequired = true)]
        public string LogPath
        {
            get { return (string)base["LogPath"]; }
        }

        [ConfigurationProperty("Max", IsRequired = true)]
        public int Max
        {
            get { return Convert.ToInt32(base["Max"].ToString()); }
        }
    }

    [ConfigurationCollection(typeof(Application))]
    public class ApplicationCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new Application();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Application)element).ApplicationName;
        }

        new public Application this[string ApplicationName]
        {
            get
            {
                return (Application)BaseGet(ApplicationName);
            }
        }
    }

    public class TrimFuelSection : ConfigurationSection
    {
        [ConfigurationProperty("Applications", IsDefaultCollection = true)]
        [ConfigurationCollection( typeof( Application ), AddItemName="Application")]
        public ApplicationCollection Applications
        {
            get { return (ApplicationCollection)base["Applications"]; }
        }
    }

    public class Config
    {
        static ApplicationCollection applications = null;
        public static ApplicationCollection Applications
        {
            get
            {
                if (applications == null)
                    applications = GetApplications();

                return applications;
            }
        }

        private static ApplicationCollection GetApplications()
        {
            TrimFuelSection trimFuelSection = (TrimFuelSection)ConfigurationManager.GetSection("TrimFuel");
            return trimFuelSection.Applications;
        }
    }
}
