using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;

namespace TrimFuel.Web.Admin.Logic.BillingAPI
{
    public class BillingAPI_AuthNET : IBillingAPI
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

        public BillingAPI_AuthNET(string url)
        {
            URL = url;
        }

        public string URL { get; set; }

        #region IBillingAPI Members

        public string Void(string username, string password, string chargeHistoryID)
        {
            string request = string.Empty;

            request += "x_type=VOID";
            if (!string.IsNullOrEmpty(username)) request += "&x_login=" + username;
            if (!string.IsNullOrEmpty(password)) request += "&x_tran_key=" + password;
            if (!string.IsNullOrEmpty(chargeHistoryID)) request += "&x_trans_id=" + chargeHistoryID;

            return HttpPOSTRequest(request);
        }

        public string Refund(string username, string password, string amount, string chargeHistoryID)
        {
            string request = string.Empty;

            request += "x_type=CREDIT";
            if (!string.IsNullOrEmpty(username)) request += "&x_login=" + username;
            if (!string.IsNullOrEmpty(password)) request += "&x_tran_key=" + password;
            if (!string.IsNullOrEmpty(amount)) request += "&x_amount=" + amount;
            if (!string.IsNullOrEmpty(chargeHistoryID)) request += "&x_trans_id=" + chargeHistoryID;

            return HttpPOSTRequest(request);
        }

        public string Charge(string username, string password, string amount, string shipping, string firstName, string lastName, string address1, string address2, string city, string state, string zip, string phone, string email, string ip, string affiliate, string subAffiliate, string internalID, string paymentType, string creditCard, string cvv, string expMonth, string expYear)
        {
            string request = string.Empty;

            request += "x_type=AUTH_CAPTURE";
            if (!string.IsNullOrEmpty(username)) request += "&x_login=" + username;
            if (!string.IsNullOrEmpty(password)) request += "&x_tran_key=" + password;
            if (!string.IsNullOrEmpty(amount)) request += "&x_amount=" + amount;
            //if (!string.IsNullOrEmpty(shipping)) request += "&shipping=" + shipping;
            if (!string.IsNullOrEmpty(firstName)) request += "&x_first_name=" + firstName;
            if (!string.IsNullOrEmpty(lastName)) request += "&x_last_name=" + lastName;
            if (!string.IsNullOrEmpty(address1)) request += "&x_address=" + address1;
            //if (!string.IsNullOrEmpty(address2)) request += "&address2=" + address2;
            if (!string.IsNullOrEmpty(city)) request += "&x_city=" + city;
            if (!string.IsNullOrEmpty(state)) request += "&x_state=" + state;
            if (!string.IsNullOrEmpty(zip)) request += "&x_zip=" + zip;
            if (!string.IsNullOrEmpty(phone)) request += "&x_phone=" + phone;
            if (!string.IsNullOrEmpty(email)) request += "&x_email=" + email;
            if (!string.IsNullOrEmpty(ip)) request += "&x_customer_ip=" + ip;
            if (!string.IsNullOrEmpty(affiliate)) request += "&merchant_defined_field_1=" + affiliate;
            if (!string.IsNullOrEmpty(subAffiliate)) request += "&merchant_defined_field_2=" + subAffiliate;
            if (!string.IsNullOrEmpty(internalID)) request += "&x_cust_id=" + internalID;
            //if (!string.IsNullOrEmpty(paymentType)) request += "&paymentType=" + paymentType;
            if (!string.IsNullOrEmpty(creditCard)) request += "&x_card_num=" + creditCard;
            if (!string.IsNullOrEmpty(cvv)) request += "&x_card_code=" + cvv;
            if (!string.IsNullOrEmpty(expMonth) && !string.IsNullOrEmpty(expYear)) request += "&x_exp_date=" + expMonth + expYear;

            return HttpPOSTRequest(request);
        }

        #endregion
    }
}
