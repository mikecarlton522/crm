using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TrimFuel.Model.Containers;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TrimFuel.Business.Utils;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    //Same implementation of another interface
    //Both implementation should be supported
    public class AtLastFulfillmentGateway2 : ShipmentGatewayOneByOne
    {
        private const string SHIPMENTS_URL = "https://api.atlastfulfillment.com/post_shipments.aspx";
        private const string STATUS_URL = "https://api.atlastfulfillment.com/shipments.aspx";
        private const string RETURNS_URL = "https://api.atlastfulfillment.com/returns.aspx";

        private const string TEST_SHIPMENTS_URL = "http://localhost/gateways/atlastfulfillment/post_shipments.aspx";
        private const string TEST_STATUS_URL = "http://localhost/gateways/atlastfulfillment/shipments.aspx";
        private const string TEST_RETURNS_URL = "https://localhost/gateways/atlastfulfillment/returns.aspx";

        private string GetResponse(string request, bool testMode)
        {
            string res = null;

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                res = wc.UploadString((!testMode ? SHIPMENTS_URL : TEST_SHIPMENTS_URL), "POST", request);
                
            }

            return res;
        }

        private string GetResponseGET(string request, bool testMode)
        {
            string res = null;

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                res = wc.DownloadString((!testMode ? STATUS_URL : TEST_STATUS_URL) + "?" + request);
            }

            return res;
        }

        private string GetResponseReturn(string request, bool testMode)
        {
            string res = null;

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                res = wc.DownloadString((!testMode ? RETURNS_URL : TEST_RETURNS_URL) + "?" + request);
            }

            return res;
        }

        private ShipMethodEnum GetShipMethodByCountry(string country)
        {
            ShipMethodEnum res = ShipMethodEnum.POM1C;
            if (country != null &&
                (country.ToLower() == "ca" || country.ToLower() == "can" || country.ToLower() == "canada"))
            {
                res = ShipMethodEnum.XPOCPF;
            }
            return res;
        }

        protected override ShipmentGatewayOneByOne.SubmitResult SubmitShipment(Person person, Address address, string email, string phone, 
            IList<ShipmentGatewayBase.Shipment> products, IList<int> shippingOptionList, 
            Product productGroup, DateTime orderDate, long uniqueID, 
            SubmitAdditionalData additionalData,
            IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode)
        {
            Orders orders = new Orders();
            orders.apiKey = config[ShipperConfigEnum.ALF_ApiKey].Value;
            orders.OrderList = new List<Order>();

            Order order = new Order();
            order.orderID = uniqueID;
            order.AllowDuplicate = false;
            order.ShipMethod = GetShipMethodByCountry(address.Country);
            order.CustomerInfo = new CustomerInfo();
            order.CustomerInfo.FirstName = person.FirstName;
            order.CustomerInfo.LastName = person.LastName;
            order.CustomerInfo.Address1 = address.Address1;
            order.CustomerInfo.Address2 = address.Address2;
            order.CustomerInfo.City = address.City;
            order.CustomerInfo.State = address.State;
            order.CustomerInfo.Zip = address.Zip;
            order.CustomerInfo.Country = address.Country;
            order.CustomerInfo.Email = email;
            order.CustomerInfo.Phone = phone;
            string company = null;
            if (Config.Current.APPLICATION_ID == "coaction.trianglecrm.com")
            {
                company = "PureCollagenSkin.com";
            }
            order.DropShipInfo = new DropShipInfo()
            {
                CompanyName = company
            };

            order.OrderItems = new OrderItems();
            order.OrderItems.OrderItemList = new List<OrderItem>();
            foreach (var i in products)
            {
                order.OrderItems.OrderItemList.Add(new OrderItem()
                {
                    SKU = i.SKU,
                    Qty = i.Quantity
                });
            }

            orders.OrderList.Add(order);

            string request = null;
            string response = null;

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer objSerializer = new XmlSerializer(typeof(Orders));
            using (MemoryStream stream = new MemoryStream())
            {
                using (XmlTextWriter xtWriter = new XmlTextWriter(stream, new UTF8Encoding(false)))
                {
                    objSerializer.Serialize(xtWriter, orders, ns);

                    xtWriter.Flush();

                    stream.Seek(0, SeekOrigin.Begin);
                    request = Encoding.UTF8.GetString(stream.ToArray());
                }
            }

            response = GetResponse(request, testMode);
            if (response != null && response[0] != '<')
            {
                response = response.Substring(1);
            }
            XmlSerializer objDeserializer = new XmlSerializer(typeof(OrdersResponse), "");
            OrdersResponse ordersResponse = null;
            using (StringReader sr = new StringReader(response))
            {
                ordersResponse = (OrdersResponse)objDeserializer.Deserialize(sr);
            }

            long? regID = null;
            if (ordersResponse != null &&
                ordersResponse.Shipments != null &&
                ordersResponse.Shipments.ShipmentList != null &&
                ordersResponse.Shipments.ShipmentList.Count > 0 &&
                ordersResponse.Shipments.ShipmentList[0].orderID == uniqueID)
            {
                regID = ordersResponse.Shipments.ShipmentList[0].id;
            }

            return new SubmitResult()
            {
                Request = request,
                Response = response,
                ShipperRegID = (regID != null ? regID.ToString() : null)
            };
        }

        public override bool IsCheckShippedImplemented
        {
            get
            {
                return true;
            }
        }

        protected override ShipmentGatewayOneByOne.ShipResult CheckShipped(string shipperRegID, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode)
        {
            System.Threading.Thread.Sleep(1000);

            string request = "key=" + config[ShipperConfigEnum.ALF_ApiKey].Value;
            request += "&shipmentID=" + shipperRegID.ToString();
            string response = GetResponseGET(request, testMode);
            if (response != null && response[0] != '<')
            {
                response = response.Substring(1);
            }

            XmlSerializer objDeserializer = new XmlSerializer(typeof(Shipments), "");
            Shipments shipmentStatus = null;
            using (StringReader sr = new StringReader(response))
            {
                shipmentStatus = (Shipments)objDeserializer.Deserialize(sr);
            }

            string trackingNumber = null;
            DateTime? shipDT = null;
            if (shipmentStatus.ShipmentList != null &&
                shipmentStatus.ShipmentList.Count > 0 &&
                shipmentStatus.ShipmentList[0].ShipmentStatus != null &&
                shipmentStatus.ShipmentList[0].ShipmentStatus.ToUpper() == "SHIPPED")
            {
                trackingNumber = shipmentStatus.ShipmentList[0].Tracking;
                shipDT = DateTime.Now;
            }
            return new ShipResult() {
                Request = request,
                Response = response,
                TrackingNumber = trackingNumber,
                ShippedDT = shipDT
            };
        }

        public override bool IsCheckReturnedImplemented
        {
            get
            {
                return true;
            }
        }

        public override IList<ShipmentGatewayResult<ShipmentPackageReturnResult>> CheckReturned(IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallback canContinue)
        {
            string request = "key=" + config[ShipperConfigEnum.ALF_ApiKey].Value;
            request += "&startTimestamp=" + String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-5));
            request += "&endTimestamp=" + String.Format("{0:yyyy-MM-dd}", DateTime.Now);
            string response = GetResponseReturn(request, testMode);
            if (response != null && response[0] != '<')
            {
                response = response.Substring(1);
            }

            Dictionary<long, KeyValuePair<string, DateTime?>> res = new Dictionary<long, KeyValuePair<string, DateTime?>>();
            XDocument doc = XDocument.Parse(response);
            foreach (var returnElem in doc.Root.Elements())
            {
                if (returnElem.Attribute("shipmentID") != null)
                {
                    long? shippmentID = Utility.TryGetLong(returnElem.Attribute("shipmentID").Value);
                    if (shippmentID != null)
                    {
                        string reason = string.Empty;
                        string timeStamp = string.Empty;

                        foreach (var item in returnElem.Elements())
                        {
                            if (item.Name.LocalName.ToLower() == "reason")
                                reason = item.Value;
                            if (item.Name.LocalName.ToLower() == "timestamp")
                                timeStamp = item.Value;
                        }

                        DateTime temp = DateTime.Now;
                        DateTime? returnDate = null;
                        if (DateTime.TryParse(timeStamp, out temp))
                        {
                            returnDate = temp;
                        }

                        if (!res.ContainsKey(shippmentID.Value))
                            res.Add(shippmentID.Value, new KeyValuePair<string, DateTime?>(reason, returnDate));
                    }
                }
            }

            return new List<ShipmentGatewayResult<ShipmentPackageReturnResult>>() {
                new ShipmentGatewayResult<ShipmentPackageReturnResult>()
                {
                    Request = request,
                    Response = response,
                    PackageList = res.Select(i => new ShipmentPackageReturnResult(){
                        ShipperRegID = i.Key.ToString(),
                        ReturnDT = i.Value.Value,
                        Reason = i.Value.Key
                    }).ToList()
                }
            };
        }
    }
}
