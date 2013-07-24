using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

namespace TrimFuel.Web.Admin.Logic.BillingAPI
{
    public class BillingAPI_WebService : IBillingAPI
    {
        private string HttpSOAPRequest(string action, string body)
        {
            string res = null;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpRequest.Headers.Add("SOAPAction", "\"http://trianglecrm.com/" + action + "\"");
            httpRequest.ContentType = "text/xml; charset=utf-8";
            httpRequest.Accept = "text/xml";
            httpRequest.Method = "POST";

            StreamWriter strOut = new StreamWriter(httpRequest.GetRequestStream());
            strOut.Write(body);
            strOut.Close();

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
            res = strIn.ReadToEnd();
            strIn.Close();

            return res;
        }

        public BillingAPI_WebService(string url)
        {
            URL = url;
        }

        public string URL { get; set; }

        #region IBillingAPI Members

        public string Void(string username, string password, string chargeHistoryID)
        {
            string request = string.Empty;
            request += "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
            request += "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            request += "<soap:Body>";
            request += "<Void xmlns=\"http://trianglecrm.com/\">";

            if (!string.IsNullOrEmpty(username)) request += "<username>" + username + "</username>";
            if (!string.IsNullOrEmpty(password)) request += "<password>" + password + "</password>";
            if (!string.IsNullOrEmpty(chargeHistoryID)) request += "<chargeHistoryID>" + chargeHistoryID + "</chargeHistoryID>";

            request += "</Void>";
            request += "</soap:Body>";
            request += "</soap:Envelope>";

            return HttpSOAPRequest("Void", request);
        }

        public string Refund(string username, string password, string amount, string chargeHistoryID)
        {
            string request = string.Empty;
            request += "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
            request += "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            request += "<soap:Body>";
            request += "<Refund xmlns=\"http://trianglecrm.com/\">";

            if (!string.IsNullOrEmpty(username)) request += "<username>" + username + "</username>";
            if (!string.IsNullOrEmpty(password)) request += "<password>" + password + "</password>";
            if (!string.IsNullOrEmpty(amount)) request += "<refundAmount>" + amount + "</refundAmount>";
            if (!string.IsNullOrEmpty(chargeHistoryID)) request += "<chargeHistoryID>" + chargeHistoryID + "</chargeHistoryID>";

            request += "</Refund>";
            request += "</soap:Body>";
            request += "</soap:Envelope>";

            return HttpSOAPRequest("Refund", request);
        }

        public string Charge(string username, string password, string amount, string shipping, string firstName, string lastName, string address1, string address2, string city, string state, string zip, string phone, string email, string ip, string affiliate, string subAffiliate, string internalID, string paymentType, string creditCard, string cvv, string expMonth, string expYear)
        {
            string request = string.Empty;
            request += "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
            request += "<soap:Envelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">";
            request += "<soap:Body>";
            request += "<Charge xmlns=\"http://trianglecrm.com/\">";

            if (!string.IsNullOrEmpty(username)) request += "<username>" + username + "</username>";
            if (!string.IsNullOrEmpty(password)) request += "<password>" + password + "</password>";
            if (!string.IsNullOrEmpty(amount)) request += "<amount>" + amount + "</amount>";
            if (!string.IsNullOrEmpty(shipping)) request += "<shipping>" + shipping + "</shipping>";
            if (!string.IsNullOrEmpty(firstName)) request += "<firstName>" + firstName + "</firstName>";
            if (!string.IsNullOrEmpty(lastName)) request += "<lastName>" + lastName + "</lastName>";
            if (!string.IsNullOrEmpty(address1)) request += "<address1>" + address1 + "</address1>";
            if (!string.IsNullOrEmpty(address2)) request += "<address2>" + address2 + "</address2>";
            if (!string.IsNullOrEmpty(city)) request += "<city>" + city + "</city>";
            if (!string.IsNullOrEmpty(state)) request += "<state>" + state + "</state>";
            if (!string.IsNullOrEmpty(zip)) request += "<zip>" + zip + "</zip>";
            if (!string.IsNullOrEmpty(phone)) request += "<phone>" + phone + "</phone>";
            if (!string.IsNullOrEmpty(email)) request += "<email>" + email + "</email>";
            if (!string.IsNullOrEmpty(ip)) request += "<ip>" + ip + "</ip>";
            if (!string.IsNullOrEmpty(affiliate)) request += "<affiliate>" + affiliate + "</affiliate>";
            if (!string.IsNullOrEmpty(subAffiliate)) request += "<subAffiliate>" + subAffiliate + "</subAffiliate>";
            if (!string.IsNullOrEmpty(internalID)) request += "<internalID>" + internalID + "</internalID>";
            if (!string.IsNullOrEmpty(paymentType)) request += "<paymentType>" + paymentType + "</paymentType>";
            if (!string.IsNullOrEmpty(creditCard)) request += "<creditCard>" + creditCard + "</creditCard>";
            if (!string.IsNullOrEmpty(cvv)) request += "<cvv>" + cvv + "</cvv>";
            if (!string.IsNullOrEmpty(expMonth)) request += "<expMonth>" + expMonth + "</expMonth>";
            if (!string.IsNullOrEmpty(expYear)) request += "<expYear>" + expYear + "</expYear>";

            request += "</Charge>";
            request += "</soap:Body>";
            request += "</soap:Envelope>";

            return HttpSOAPRequest("Charge", request);
        }

        #endregion
    }
}
