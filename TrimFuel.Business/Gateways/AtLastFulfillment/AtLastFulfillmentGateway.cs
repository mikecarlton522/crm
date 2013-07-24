using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Gateways.AtLastFulfillment
{
    public class AtLastFulfillmentGateway
    {
        public AtLastFulfillmentGateway(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            this.config = config;
        }

        private IDictionary<ShipperConfig.ID, ShipperConfig> config = null;

        private const string SHIPMENTS_URL = "https://api.atlastfulfillment.com/post_shipments.aspx";
        private const string STATUS_URL = "https://api.atlastfulfillment.com/shipments.aspx";
        private const string RETURNS_URL = "https://api.atlastfulfillment.com/returns.aspx";
        //private const string STATUS_URL = "http://localhost/gateways/AtLast.aspx";
        private string GetResponse(string request)
        {
            WebClient wc = new WebClient();
            wc.Encoding = Encoding.UTF8;
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return wc.UploadString(SHIPMENTS_URL, "POST", request);
        }

        private string GetResponseGET(string request)
        {
            string res = null;

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                res = wc.DownloadString(STATUS_URL + "?" + request);
            }

            return res;
        }

        private string GetResponseReturn(string request)
        {
            string res = null;

            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                res = wc.DownloadString(RETURNS_URL + "?" + request);
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

        public long? PostShipment(long saleID, Registration registration, string country, IList<InventoryView> inventories, string company, out string request, out string response)
        {
            Orders orders = new Orders();
            orders.apiKey = config[ShipperConfigEnum.ALF_ApiKey].Value;
            orders.OrderList = new List<Order>();

            Order order = new Order();
            order.orderID = saleID;
            order.AllowDuplicate = false;
            order.ShipMethod = GetShipMethodByCountry(country);
            order.CustomerInfo = new CustomerInfo();
            order.CustomerInfo.FirstName = registration.FirstName;
            order.CustomerInfo.LastName = registration.LastName;
            order.CustomerInfo.Address1 = registration.Address1;
            order.CustomerInfo.Address2 = registration.Address2;
            order.CustomerInfo.City = registration.City;
            order.CustomerInfo.State = registration.State;
            order.CustomerInfo.Zip = registration.Zip;
            order.CustomerInfo.Country = country;
            order.CustomerInfo.Email = registration.Email;
            order.CustomerInfo.Phone = registration.Phone;
            order.DropShipInfo = new DropShipInfo()
            {
                 CompanyName = company
            };

            order.OrderItems = new OrderItems();
            order.OrderItems.OrderItemList = new List<OrderItem>();
            foreach (InventoryView i in inventories)
            {
                order.OrderItems.OrderItemList.Add(new OrderItem()
                {
                    SKU = i.SKU,
                    Qty = (i.Quantity != null ? i.Quantity.Value : 1)
                });
            }

            orders.OrderList.Add(order);

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

            response = GetResponse(request);
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
                ordersResponse.Shipments.ShipmentList[0].orderID == saleID)
            {
                regID = ordersResponse.Shipments.ShipmentList[0].id;
            }

            return regID;
        }

        public string CheckTrackingNumber(long regID)
        {
            System.Threading.Thread.Sleep(1000);

            string request = "key=" + config[ShipperConfigEnum.ALF_ApiKey].Value;
            request += "&shipmentID=" + regID.ToString();
            string response = GetResponseGET(request);
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
            if (shipmentStatus.ShipmentList != null &&
                shipmentStatus.ShipmentList.Count > 0 &&
                shipmentStatus.ShipmentList[0].ShipmentStatus != null &&
                shipmentStatus.ShipmentList[0].ShipmentStatus.ToUpper() == "SHIPPED")
            {
                trackingNumber = shipmentStatus.ShipmentList[0].Tracking;
            }
            return trackingNumber;
        }

        //public bool CheckReturn(long regID)
        //{
        //    string request = "key=" + Config.Current.AT_LAST_FULFILLMENT_API_KEY;
        //    request += "&shipmentID=" + regID.ToString();
        //    string response = GetResponseGET(request);
        //    if (response != null && response[0] != '<')
        //    {
        //        response = response.Substring(1);
        //    }

        //    XmlSerializer objDeserializer = new XmlSerializer(typeof(Shipments), "");
        //    Shipments shipmentStatus = null;
        //    using (StringReader sr = new StringReader(response))
        //    {
        //        shipmentStatus = (Shipments)objDeserializer.Deserialize(sr);
        //    }

        //    bool returned = false;
        //    if (shipmentStatus.ShipmentList != null &&
        //        shipmentStatus.ShipmentList.Count > 0 &&
        //        shipmentStatus.ShipmentList[0].ShipmentStatus != null &&
        //        shipmentStatus.ShipmentList[0].ShipmentStatus.ToUpper() == "RETURNED")
        //    {
        //        returned = true;
        //    }
        //    return returned;
        //}

        public Dictionary<long, KeyValuePair<string, DateTime?>> GetReturns()
        {
            string request = "key=" + config[ShipperConfigEnum.ALF_ApiKey].Value;
            request += "&startTimestamp=" + String.Format("{0:yyyy-MM-dd}", DateTime.Now.AddDays(-5));
            request += "&endTimestamp=" + String.Format("{0:yyyy-MM-dd}", DateTime.Now);
            string response = GetResponseReturn(request);
            if (response != null && response[0] != '<')
            {
                response = response.Substring(1);
            }

            Dictionary<long, KeyValuePair<string, DateTime?>> res = new Dictionary<long, KeyValuePair<string, DateTime?>>();
            XDocument doc = XDocument.Parse(response);
            //XDocument doc = XDocument.Load("C://test.xml");
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

            return res;
        }
    }
}
