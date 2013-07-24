using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

namespace TrimFuel.Web.Admin.Logic.BillingAPI
{
    public class BillingAPI_NMI : IBillingAPI
    {
        private string HttpPOSTRequest(string body)
        {
            string res = null;

            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(URL);
            httpRequest.Method = "POST";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            StreamWriter strOut = new StreamWriter(httpRequest.GetRequestStream());
            strOut.Write(body);
            strOut.Close();

            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            StreamReader strIn = new StreamReader(httpResponse.GetResponseStream());
            res = strIn.ReadToEnd();
            strIn.Close();

            return res;
        }

        public BillingAPI_NMI(string url)
        {
            URL = url;
        }

        public string URL { get; set; }

        #region IBillingAPI Members

        public string Void(string username, string password, string chargeHistoryID)
        {
            string request = string.Empty;

            request += "type=void";
            if (!string.IsNullOrEmpty(username)) request += "&username=" + username;
            if (!string.IsNullOrEmpty(password)) request += "&password=" + password;
            if (!string.IsNullOrEmpty(chargeHistoryID)) request += "&transactionid=" + chargeHistoryID;

            return HttpPOSTRequest(request);
        }

        public string Refund(string username, string password, string amount, string chargeHistoryID)
        {
            string request = string.Empty;

            request += "type=refund";
            if (!string.IsNullOrEmpty(username)) request += "&username=" + username;
            if (!string.IsNullOrEmpty(password)) request += "&password=" + password;
            if (!string.IsNullOrEmpty(amount)) request += "&amount=" + amount;
            if (!string.IsNullOrEmpty(chargeHistoryID)) request += "&transactionid=" + chargeHistoryID;

            return HttpPOSTRequest(request);
        }

        public string Charge(string username, string password, string amount, string shipping, string firstName, string lastName, string address1, string address2, string city, string state, string zip, string phone, string email, string ip, string affiliate, string subAffiliate, string internalID, string paymentType, string creditCard, string cvv, string expMonth, string expYear)
        {
            string request = string.Empty;

            //Correct values
            if (expYear != null && expYear.Length == 4)
            {
                expYear = expYear.Substring(2);
            }

            request += "type=sale";
            if (!string.IsNullOrEmpty(username)) request += "&username=" + username;
            if (!string.IsNullOrEmpty(password)) request += "&password=" + password;
            if (!string.IsNullOrEmpty(amount)) request += "&amount=" + amount;
            //if (!string.IsNullOrEmpty(shipping)) request += "&shipping=" + shipping;
            if (!string.IsNullOrEmpty(firstName)) request += "&firstname=" + firstName;
            if (!string.IsNullOrEmpty(lastName)) request += "&lastname=" + lastName;
            if (!string.IsNullOrEmpty(address1)) request += "&address1=" + address1;
            if (!string.IsNullOrEmpty(address2)) request += "&address2=" + address2;
            if (!string.IsNullOrEmpty(city)) request += "&city=" + city;
            if (!string.IsNullOrEmpty(state)) request += "&state=" + state;
            if (!string.IsNullOrEmpty(zip)) request += "&zip=" + zip;
            if (!string.IsNullOrEmpty(phone)) request += "&phone=" + phone;
            if (!string.IsNullOrEmpty(email)) request += "&email=" + email;
            if (!string.IsNullOrEmpty(ip)) request += "&ipadress=" + ip;
            if (!string.IsNullOrEmpty(affiliate)) request += "&merchant_defined_field_1=" + affiliate;
            if (!string.IsNullOrEmpty(subAffiliate)) request += "&merchant_defined_field_2=" + subAffiliate;
            if (!string.IsNullOrEmpty(internalID)) request += "&orderid=" + internalID;
            //if (!string.IsNullOrEmpty(paymentType)) request += "&paymentType=" + paymentType;
            if (!string.IsNullOrEmpty(creditCard)) request += "&ccnumber=" + creditCard;
            if (!string.IsNullOrEmpty(cvv)) request += "&cvv=" + cvv;
            if (!string.IsNullOrEmpty(expMonth) && !string.IsNullOrEmpty(expYear)) request += "&ccexp=" + expMonth + expYear;

            return HttpPOSTRequest(request);
        }

        #endregion
    }
}
