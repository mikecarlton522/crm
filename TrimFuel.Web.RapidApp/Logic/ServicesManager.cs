using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace TrimFuel.Web.RapidApp.Logic
{
    public class ServicesManager
    {
        private string pathToXML = HttpContext.Current.Server.MapPath("~/Data/Services.xml");

        public List<FulfillmentServiceItem> GetAllFulfillmentServices()
        {
            var services = new List<FulfillmentServiceItem>();
            try
            {
                XDocument doc = XDocument.Load(pathToXML);
                foreach (XElement el in doc.Root.Elements())
                {
                    if (el.Name.LocalName.ToLower() == "fulfillment")
                    {
                        foreach (XElement child in el.Elements())
                        {
                            var serItem = new FulfillmentServiceItem();
                            serItem.ID = child.Attribute("ID") == null ? 0 : Convert.ToInt32(child.Attribute("ID").Value);
                            serItem.CustomDevelopmentFee = TryGetDouble(child.Attribute("CustomDevelopmentFee"));
                            serItem.KittingAndAsemblyFee = TryGetDouble(child.Attribute("KittingAndAsemblyFee"));
                            serItem.ServiceName = child.Attribute("Name") == null ? string.Empty : child.Attribute("Name").Value;
                            serItem.ReturnsFee = TryGetDouble(child.Attribute("ReturnsFee"));
                            serItem.SetupFee = TryGetDouble(child.Attribute("SetupFee"));
                            serItem.ShipmentFee = TryGetDouble(child.Attribute("ShipmentFee"));
                            serItem.SpecialLaborFee = TryGetDouble(child.Attribute("SpecialLaborFee"));
                            serItem.ShipmentSKUFee = TryGetDouble(child.Attribute("ShipmentSKUFee"));

                            serItem.CustomDevelopmentFeeRetail = TryGetDouble(child.Attribute("CustomDevelopmentFeeRetail"));
                            serItem.KittingAndAsemblyFeeRetail = TryGetDouble(child.Attribute("KittingAndAsemblyFeeRetail"));
                            serItem.ReturnsFeeRetail = TryGetDouble(child.Attribute("ReturnsFeeRetail"));
                            serItem.SetupFeeRetail = TryGetDouble(child.Attribute("SetupFeeRetail"));
                            serItem.ShipmentFeeRetail = TryGetDouble(child.Attribute("ShipmentFeeRetail"));
                            serItem.SpecialLaborFeeRetail = TryGetDouble(child.Attribute("SpecialLaborFeeRetail"));
                            serItem.ShipmentSKUFeeRetail = TryGetDouble(child.Attribute("ShipmentSKUFeeRetail"));

                            // get config items
                            Dictionary<string, string> configDict = new Dictionary<string, string>();
                            foreach (XElement config in child.Elements())
                            {
                                configDict.Add(config.Attribute("key").Value, string.Empty);
                            }
                            serItem.ConfigFields = configDict;

                            services.Add(serItem);
                        }
                    }
                }
            }
            catch
            {
                return new List<FulfillmentServiceItem>();
            }
            return services;
        }

        public List<OutboundServiceItem> GetAllOutboundServices()
        {
            var services = new List<OutboundServiceItem>();
            try
            {
                XDocument doc = XDocument.Load(pathToXML);
                foreach (XElement el in doc.Root.Elements())
                {
                    if (el.Name.LocalName.ToLower() == "outbound")
                    {
                        foreach (XElement child in el.Elements())
                        {
                            var serItem = new OutboundServiceItem();
                            serItem.ID = child.Attribute("ID") == null ? 0 : Convert.ToInt32(child.Attribute("ID").Value);
                            serItem.ServiceName = child.Attribute("Name") == null ? string.Empty : child.Attribute("Name").Value;
                            serItem.SetupFee = TryGetDouble(child.Attribute("SetupFee"));
                            serItem.MonthlyFee = TryGetDouble(child.Attribute("MonthlyFee"));
                            serItem.PerPourFee = TryGetDouble(child.Attribute("PerPourFee"));

                            serItem.MonthlyFeeRetail = TryGetDouble(child.Attribute("MonthlyFeeRetail"));
                            serItem.PerPourFeeRetail = TryGetDouble(child.Attribute("PerPourFeeRetail"));
                            serItem.SetupFeeRetail = TryGetDouble(child.Attribute("SetupFeeRetail"));
                            services.Add(serItem);
                        }
                    }
                }
            }
            catch
            {
                return new List<OutboundServiceItem>();
            }
            return services;
        }

        public List<GatewayServiceItem> GetAllGatewayServices()
        {
            var services = new List<GatewayServiceItem>();
            try
            {
                XDocument doc = XDocument.Load(pathToXML);
                foreach (XElement el in doc.Root.Elements())
                {
                    if (el.Name.LocalName.ToLower() == "gateway")
                    {
                        foreach (XElement child in el.Elements())
                        {
                            var serItem = new GatewayServiceItem();
                            serItem.ID = child.Attribute("ID") == null ? 0 : Convert.ToInt32(child.Attribute("ID").Value);
                            serItem.ServiceName = child.Attribute("Name") == null ? string.Empty : child.Attribute("Name").Value;
                            serItem.ChargebackFee = TryGetDouble(child.Attribute("ChargebackFee"));
                            serItem.ChargebackRepresentationFee = TryGetDouble(child.Attribute("ChargebackRepresentationFee"));
                            serItem.TransactionFee = TryGetDouble(child.Attribute("TransactionFee"));
                            serItem.ChargebackFeeRetail = TryGetDouble(child.Attribute("ChargebackFeeRetail"));
                            serItem.ChargebackRepresentationFeeRetail = TryGetDouble(child.Attribute("ChargebackRepresentationFeeRetail"));
                            serItem.TransactionFeeRetail = TryGetDouble(child.Attribute("TransactionFeeRetail"));
                            serItem.DiscountRateRetail = TryGetDouble(child.Attribute("DiscountRateRetail"));
                            serItem.DiscountRate = TryGetDouble(child.Attribute("DiscountRate"));

                            serItem.GatewayFee = TryGetDouble(child.Attribute("GatewayFee"));
                            serItem.GatewayFeeRetail = TryGetDouble(child.Attribute("GatewayFeeRetail"));

                            services.Add(serItem);
                        }
                    }
                }
            }
            catch
            {
                return new List<GatewayServiceItem>();
            }
            return services;
        }

        public List<ServiceItem> GetServiceList()
        {
            List<ServiceItem> res = new List<ServiceItem>();
            try
            {
                XDocument doc = XDocument.Load(pathToXML);
                foreach (XElement el in doc.Root.Elements())
                {
                    var services = new List<BaseServiceItem>();
                    foreach (XElement child in el.Elements())
                    {
                        services.Add(new BaseServiceItem()
                        {
                            ID = TryGetInt(child.Attribute("ID")),
                            ServiceName = child.Attribute("Name").Value.ToString(),
                        });
                    }
                    res.Add(new ServiceItem()
                    {
                        Name = el.Name.LocalName,
                        ID = TryGetInt(el.Attribute("ID")),
                        DisplayName = el.Attribute("DisplayName") == null ? string.Empty : el.Attribute("DisplayName").Value,
                        Services = services
                    });
                }
            }
            catch
            {
                return null;
            }
            return res;
        }

        public void SaveFulfillmentSettings(Dictionary<string, string> settings, int serviceID)
        {
            XDocument doc = XDocument.Load(pathToXML);
            foreach (XElement el in doc.Root.Elements())
            {
                if (el.Name.LocalName.ToLower() == "fulfillment")
                {
                    foreach (XElement child in el.Elements())
                    {
                        int currServiceID = child.Attribute("ID") == null ? 0 : Convert.ToInt32(child.Attribute("ID").Value);
                        if (currServiceID == serviceID)
                        {
                            //update fees
                            foreach (var setting in settings)
                                child.Attribute(setting.Key).Value = setting.Value;
                        }
                    }
                }
            }
            doc.Save(pathToXML);
        }

        public void SaveOutboundSettings(Dictionary<string, string> settings, int serviceID)
        {
            XDocument doc = XDocument.Load(pathToXML);
            foreach (XElement el in doc.Root.Elements())
            {
                if (el.Name.LocalName.ToLower() == "outbound")
                {
                    foreach (XElement child in el.Elements())
                    {
                        int currServiceID = child.Attribute("ID") == null ? 0 : Convert.ToInt32(child.Attribute("ID").Value);
                        if (currServiceID == serviceID)
                        {
                            //update fees
                            foreach (var setting in settings)
                                child.Attribute(setting.Key).Value = setting.Value;
                        }
                    }
                }
            }
            doc.Save(pathToXML);
        }

        public void SaveGatewaySettings(Dictionary<string, string> settings, string name)
        {
            XDocument doc = XDocument.Load(pathToXML);
            foreach (XElement el in doc.Root.Elements())
            {
                if (el.Name.LocalName.ToLower() == "gateway")
                {
                    foreach (XElement child in el.Elements())
                    {
                        string currName = child.Attribute("Name").Value;
                        if (currName == name)
                        {
                            //update fees
                            foreach (var setting in settings)
                                child.Attribute(setting.Key).Value = setting.Value;
                        }
                    }
                }
            }
            doc.Save(pathToXML);
        }

        private double TryGetDouble(XAttribute attr)
        {
            return attr == null ? 0 : Convert.ToDouble(attr.Value);
        }
        private int TryGetInt(XAttribute attr)
        {
            return attr == null ? 0 : Convert.ToInt32(attr.Value);
        }
    }
}
