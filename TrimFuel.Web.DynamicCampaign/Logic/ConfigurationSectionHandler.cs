using System;
using System.Collections.Generic;
using System.Web;
using System.Configuration;

namespace TrimFuel.Web.DynamicCampaign.Logic
{
    public class ConfigurationSectionHandler : ConfigurationSection
    {
        public ConfigurationSectionHandler()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        [ConfigurationProperty("CampaignDomains")]
        public CampaignDomainCollection CampaignDomains
        {
            get
            {
                return this["CampaignDomains"] as CampaignDomainCollection;
            }
        }

    }

    public class CampaignDomainElement : ConfigurationElement
    {
        [ConfigurationProperty("Domain", IsRequired = true, IsKey = true)]
        public String Domain
        {
            get
            { return (String)this["Domain"]; }
            set
            { this["Domain"] = value; }
        }

        [ConfigurationProperty("CampaignID", IsRequired = true)]
        public int CampaignID
        {
            get
            { return (int)this["CampaignID"]; }
            set
            { this["CampaignID"] = value; }
        }
    }

    public class CampaignDomainCollection : ConfigurationElementCollection
    {
        public CampaignDomainElement this[string name]
        {
            get
            {
                if (base.BaseGet(name) != null)
                {
                    return base.BaseGet(name) as CampaignDomainElement;
                }
                else
                {
                    base.InitializeDefault();
                    return new CampaignDomainElement();
                }
            }
            set
            {
                this.BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CampaignDomainElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CampaignDomainElement)element).Domain;
        }
    }
}
