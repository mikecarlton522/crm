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
using TrimFuel.Model.Containers;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Gateways.GoFulfillmentGateway
{
    public class GoFulfillmentGateway2 : ShipmentGatewayOneByOne
    {
        private const string URL = "http://stockist.wlgfulfilment.co.uk/webclient";
        private const string TEST_URL = "http://localhost/gateways/gofulfillment";

        private string GetResponse(string request, string timestamp, string authority, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode)
        {
            string res = null;
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Clear();
                wc.Headers.Add("UserId", config[ShipperConfigEnum.GO_FULFILLMENT_UserId].Value);
                wc.Headers.Add("Timestamp", timestamp);
                wc.Headers.Add("Authority", authority);
                string url = (
                    !testMode 
                    ? URL + "/services/despatchrequestcreate/" + config[ShipperConfigEnum.GO_FULFILLMENT_OriginId].Value
                    : TEST_URL + "/services_despatchrequestcreate.aspx?originid=" + config[ShipperConfigEnum.GO_FULFILLMENT_OriginId].Value
                    );
                res = wc.UploadString(url, "POST", request);
            }
            return res;
        }

        private string GetTimeStamp(bool testMode)
        {
            string url = (
                !testMode
                ? URL + "/time"
                : TEST_URL + "/time.aspx");

            byte[] responseArray = null;
            using (WebClient wc = new WebClient())
            {
                responseArray = wc.DownloadData(url);
            }           
            return System.Text.Encoding.ASCII.GetString(responseArray);
        }

        private string SHA1(string s)
        {
            Encoding enc = Encoding.ASCII;
            byte[] buffer = enc.GetBytes(s);
            SHA1CryptoServiceProvider cryptoTransformSHA1 = new SHA1CryptoServiceProvider();
            string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
            return hash;
        }

        protected override string FixCountry(string country)
        {
            return base.FixCountry(country).Replace("United Kingdom", "GB");
        }

        protected override SubmitResult SubmitShipment(Person person, Address address, string email, string phone, 
            IList<ShipmentGatewayBase.Shipment> products, IList<int> shippingOptionList, 
            Product productGroup, DateTime orderDate, long uniqueID,  
            SubmitAdditionalData additionalData,
            IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode)
        {
            string timestamp = GetTimeStamp(testMode);

            //country code must be passed in first available reference field and can only be UK or FR
            string countryCode = address.Country == "FR" ? "FR" : "UK";

            XElement payload = new XElement("DespatchRequest",
                new XElement("document",
                    new XElement("uid", uniqueID),
                    new XElement("origin", config[ShipperConfigEnum.GO_FULFILLMENT_OriginId].Value),
                    new XElement("destination", "0"),
                    new XElement("documentdate", DateTime.Now.ToString("o")),
                    new XElement("documenttime", DateTime.Now.ToString("o")),
                    new XElement("ref1", additionalData.BillingID),
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

            foreach (var item in products)
            {
                payload.Add(new XElement("documentitems",
                    //new XElement("uid", "-1"),
                    new XElement("documentid", uniqueID),
                    new XElement("productid", item.SKU),
                    new XElement("qty", item.Quantity)));
            }

            payload.Add(
                new XElement("address",
                //new XElement("uid", "-1"),
                    new XElement("name", person.FullName),
                    new XElement("address1", address.Address1),
                    new XElement("address2", address.Address2),
                    new XElement("town", address.City),
                    new XElement("area", address.State),
                    new XElement("iso2", address.Country),
                    new XElement("postcode", address.Zip)));

            string request = payload.ToString().Replace("<DespatchRequest>", "<DespatchRequest xmlns = \"http://tempuri.org/DespatchRequest.xsd\">");

            string hashpass = SHA1(config[ShipperConfigEnum.GO_FULFILLMENT_Password].Value).ToLower();
            string authority = SHA1(string.Concat(timestamp, hashpass, request)).ToLower();

            string response = GetResponse(request, timestamp, authority, config, testMode);

            long? regID = null;
            if (response.Contains("<Result>true</Result>"))
            {
                regID = uniqueID;
            }

            return new SubmitResult()
            {
                Request = request,
                Response = response,
                ShipperRegID = (regID != null ? regID.ToString() : null)
            };
        }
    }
}
