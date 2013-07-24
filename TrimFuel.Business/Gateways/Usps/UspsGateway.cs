using System;
using System.Net;
using System.Xml.Linq;

using TrimFuel.Business.Utils;

namespace TrimFuel.Business.Gateways.UspsGateway
{
    public class UspsGateway
    {
        private const string TEST_URL = "http://testing.shippingapis.com/ShippingAPITest.dll?API=TrackV2&XML=";
        private const string TEST_USERNAME = "901TRIAN4916";
        private const string TEST_PASSWORD = "593GW02ZT680";

        private const string LIVE_URL = "http://production.shippingapis.com/ShippingAPI.dll?API=TrackV2&XML=";
        private const string LIVE_USERNAME = "901TRIAN4916";
        private const string LIVE_PASSWORD = "593GW02ZT680";

        private string GetResponse(string request)
        {
            WebClient wc = new WebClient();
            return wc.DownloadString(request);
        }

        public void GetTrackInfo(string trackID, bool test, out string response)
        {
            string url = test ? TEST_URL : LIVE_URL;
            string username = test ? TEST_USERNAME : LIVE_USERNAME;
            string password = test ? TEST_PASSWORD : LIVE_PASSWORD;

            XElement xml = new XElement("TrackFieldRequest", new XAttribute("USERID", TEST_USERNAME),
                new XElement("TrackID", new XAttribute("ID", trackID)));            

            string request = url + xml.ToString();

            response = GetResponse(request);
        }
    }
}
