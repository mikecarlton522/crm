using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Containers;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using System.Web;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Gateways.TSN
{
    public class TSNGateway : ShipmentGatewayOneByOne
    {
        private const string URL = "https://www.orderyourstoday.com/InclineHealth/";
        private const string TEST_URL = "http://localhost/gateways/tsn/";

        private string GetResponse(string request, bool testMode)
        {
            string res = null;
            if (testMode)
                res = HTTPGet(TEST_URL + "orderinputapi.aspx?" + request);
            else
                res = HTTPGet(URL + "orderinputapi.asp?" + request);
            return HttpUtility.UrlDecode(res);
        }

        private string GetResponseTrackingNum(string request, bool testMode)
        {
            string res = null;
            if (testMode)
                res = HTTPGet(TEST_URL + "orderstatus.aspx?" + request);
            else
                res = HTTPGet(URL + "orderstatus.asp?" + request);
            return HttpUtility.UrlDecode(res);
        }

        protected override SubmitResult SubmitShipment(Person person, Address address, string email, string phone, IList<ShipmentGatewayBase.Shipment> products, IList<int> shippingOptionList, Model.Product productGroup, DateTime orderDate, long uniqueID, SubmitAdditionalData additionalData, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode)
        {
            SubmitResult res = new SubmitResult();
            res.Request = Utility.LoadFromEmbeddedResource(typeof(TSNGateway), "tsn.txt");
            res.Request = res.Request
                .Replace("##USERNAME##", UrlEncode(config[ShipperConfigEnum.TSN_Username].Value))
                .Replace("##PASSWORD##", UrlEncode(config[ShipperConfigEnum.TSN_Password].Value))
                .Replace("##EMAIL##", UrlEncode(email))
                .Replace("##FNAME##", UrlEncode(person.FirstName))
                .Replace("##LNAME##", UrlEncode(person.LastName))
                .Replace("##ADDRESS##", UrlEncode(address.FullAddress))
                .Replace("##CITY##", UrlEncode(address.City))
                .Replace("##STATE##", UrlEncode(address.State))
                .Replace("##ZIP##", UrlEncode(address.Zip))
                .Replace("##PHONE##", UrlEncode(phone))
                .Replace("##SHIPPING_PRICE##", UrlEncode((0.00M).ToString()))
                .Replace("##SALEID##", UrlEncode(uniqueID.ToString()));

            string skuLine = Utility.LoadFromEmbeddedResource(typeof(TSNGateway), "tsnsku.txt");
            string skuList = string.Empty;
            int lineNumber = 1;
            foreach (var shipment in products)
	        {
                skuList = skuList + skuLine
                    .Replace("##LINE_NUMBER##", UrlEncode(String.Format("{0:00}", lineNumber)))
                    .Replace("##SKU##", UrlEncode(shipment.SKU))
                    .Replace("##PRICE##", UrlEncode((0.00M).ToString()))
                    .Replace("##QTY##", UrlEncode(shipment.Quantity.ToString()));
                lineNumber++;
	        }

            res.Request = res.Request.Replace("##SKU_LIST##", skuList);

            try
            {
                res.Response = GetResponse(res.Request, testMode);
                if (ExtractResponseParam(res.Response, "result") == "0")
                {
                    res.ShipperRegID = ExtractResponseParam(res.Response, "orderid");
                }
            }
            catch (Exception ex)
            {
                res.Response = ex.ToString();
            }

            return res;
        }

        public override bool IsCheckShippedImplemented
        {
            get
            {
                return true;
            }
        }

        protected override ShipResult CheckShipped(string shipperRegID, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode)
        {
            ShipResult res = new ShipResult();
            res.Request = Utility.LoadFromEmbeddedResource(typeof(TSNGateway), "tsnstatus.txt");
            res.Request = res.Request
                .Replace("##USERNAME##", UrlEncode(config[ShipperConfigEnum.TSN_Username].Value))
                .Replace("##PASSWORD##", UrlEncode(config[ShipperConfigEnum.TSN_Password].Value))
                .Replace("##REGID##", UrlEncode(shipperRegID));

            try
            {
                res.Response = GetResponseTrackingNum(res.Request, testMode);
                if (ExtractResponseParam(res.Response, "result") == "0" && !string.IsNullOrEmpty(ExtractResponseParam(res.Response, "TrackingNum")))
                {
                    res.TrackingNumber = ExtractResponseParam(res.Response, "TrackingNum");
                    res.ShippedDT = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                res.Response = ex.ToString();
            }

            return res;
        }

        private string UrlEncode(string val)
        {
            if (string.IsNullOrEmpty(val))
                return string.Empty;
            return HttpUtility.UrlEncode(val);
        }

        private string ExtractResponseParam(string response, string paramName)
        {
            return new DelimitedResponseParams(response, '&').GetParam(paramName);
        }
    }
}
