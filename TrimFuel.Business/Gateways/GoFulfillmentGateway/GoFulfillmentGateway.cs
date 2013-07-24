using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using System.Xml.Serialization;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Linq;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Gateways.GoFulfillmentGateway
{
    public class GoFulfillmentGateway
    {
        //private const string SHIPMENTS_URL = "http://109.69.48.40/wlgtesting/services/despatchrequestcreate/2237";
        //private const string STATUS_URL = "http://109.69.48.40/wlgtesting/services/despatchrequestcreate";
        private const string GO_FULFILLMENT_URL = "http://stockist.wlgfulfilment.co.uk/webclient";

        public GoFulfillmentGateway(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            this.config = config;
        }

        private IDictionary<ShipperConfig.ID, ShipperConfig> config = null;

        private string GetResponse(string request, string timestamp, string authority)
        {
            WebClient wc = new WebClient();
            wc.Headers.Clear();
            wc.Headers.Add("UserId", config[ShipperConfigEnum.GO_FULFILLMENT_UserId].Value);
            wc.Headers.Add("Timestamp", timestamp);
            wc.Headers.Add("Authority", authority);
            return wc.UploadString(GO_FULFILLMENT_URL + "/services/despatchrequestcreate/" + config[ShipperConfigEnum.GO_FULFILLMENT_OriginId].Value, "POST", request);
        }

        public long? PostShipment(long saleID, Registration registration, Billing b, string country, IList<KeyValuePair<InventoryView, long>> inventories, out string request, out string response)
        {
            string timestamp = GetTimeStamp();

            //country code must be passed in first available reference field and can only be UK or FR
            string countryCode = b.Country == "FR" ? "FR" : "UK";

            XElement payload = new XElement("DespatchRequest",
                new XElement("document",
                    new XElement("uid", saleID),
                    new XElement("origin", config[ShipperConfigEnum.GO_FULFILLMENT_OriginId].Value),
                    new XElement("destination", "0"),
                    new XElement("documentdate", DateTime.Now.ToString("o")),
                    new XElement("documenttime", DateTime.Now.ToString("o")),
                    new XElement("ref1", b.BillingID),
                    new XElement("ref2", countryCode),
                    new XElement("ref3", null),
                    new XElement("ref4", null),
                    new XElement("ref5", null),
                    new XElement("ref6", null),
                    new XElement("ref7", null),
                    new XElement("ref8", null),
                    new XElement("ref9", null),
                    new XElement("datecreated", DateTime.Now.ToString("o")),
                    new XElement("timecreated", DateTime.Now.ToString("o")),
                    new XElement("extensionid", "0")));

            foreach (var item in inventories)
            {
                payload.Add(new XElement("documentitems",
                    //new XElement("uid", "-1"),
                    new XElement("documentid", saleID),
                    new XElement("productid", FixSTO_SKU(item.Key.SKU)),
                    new XElement("qty", item.Key.Quantity)));
            }

            payload.Add(
                new XElement("address", 
                    //new XElement("uid", "-1"),
                    new XElement("name", registration.FullName),
                    new XElement("address1", registration.Address1),
                    new XElement("address2", registration.Address2),
                    new XElement("town", registration.City),
                    new XElement("area", registration.State),
                    new XElement("iso2", FixCountryISO2(country)),
                    new XElement("postcode", registration.Zip)));

            request = payload.ToString().Replace("<DespatchRequest>", "<DespatchRequest xmlns = \"http://tempuri.org/DespatchRequest.xsd\">");

            string hashpass = SHA1(config[ShipperConfigEnum.GO_FULFILLMENT_Password].Value).ToLower();
            string authority = SHA1(string.Concat(timestamp, hashpass, request)).ToLower();

            response = GetResponse(request, timestamp, authority);
            //if (response != null && response[0] != '<')
            //{
            //    response = response.Substring(1);
            //}
            if (response.Contains("<Result>true</Result>"))
            {
                //long? res = 0;

                return saleID;
            }
            return null;
        }

        private string FixSTO_SKU(string sku)
        {
            //if (sku == null)
            //    return null;

            //return sku.Replace("ABF-", "GO-");
            return sku;
        }

        private string FixCountryISO2(string country)
        {
            if (country == null)
                return "";
            return country.Replace("United Kingdom", "GB");
        }

        private string SHA1(string s)
        {
            Encoding enc = Encoding.ASCII;
            byte[] buffer = enc.GetBytes(s);
            SHA1CryptoServiceProvider cryptoTransformSHA1 =
            new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(
                cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");

            return hash;
        }

        private string GetTimeStamp()
        {
            string url = GO_FULFILLMENT_URL + "/time";

            WebClient wc = new WebClient();

            byte[] responseArray = wc.DownloadData(url);

            return System.Text.Encoding.ASCII.GetString(responseArray);
        }
    }
}
