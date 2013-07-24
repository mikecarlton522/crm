using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TrimFuel.Model;

namespace TrimFuel.Business.Gateways.AragonWhiteLabels
{
    public class AragonWhiteLabelGateway
    {
        private const string URL = "https://crm.aragoninc.com/clubjava/crmapi.nsf/gateway";
        //private const string URL = "http://localhost/gateways/AragonWhiteLabels/gateway.aspx";
        private const string REQUEST = "q_system_key=##KEY##&q_action=LEAD_ADD&q_lead_callcenter=##CALLCENTER##&q_lead_first_name=##B_F_NAME##&q_lead_middle_name=##B_M_NAME##&q_lead_last_name=##B_L_NAME##&q_lead_ship_address1=##B_ADDRESS##&q_lead_ship_address2=##B_ADDRESS2##&q_lead_ship_address3=##B_ADDRESS3##&q_lead_ship_city=##B_CITY##&q_lead_ship_state=##B_STATE##&q_lead_ship_zip=##B_ZIP##&q_lead_ship_country=##B_COUNTRY##&q_lead_email=##B_EMAIL##&q_lead_phone=##B_PHONE##&q_lead_order=##B_DATE##&q_lead_sold=##PRODUCT##&q_lead_record=##REC##&q_lead_extid=##B_ID##";
        private const string KEY = "8PQMT4";

        public bool SendLead(Billing billing, int clubID, int productID, out string request, out string response)
        {
            bool res = false;
            request = "";
            response = "";
            try
            {
                request = PrepareRequest(billing, productID, clubID);
                response = SendRequest(request);
                res = ProcessResponse(response);
            }
            catch (Exception ex)
            {
                response = ex.ToString();
                res = false;
            }
            return res;
        }

        private static string PrepareRequest(Billing billing, int productID, int callCenterID)
        {
            string request = REQUEST;

            request = request.Replace("##KEY##", KEY);
            request = request.Replace("##CALLCENTER##", callCenterID.ToString());

            request = request.Replace("##B_F_NAME##", billing.FirstName);
            request = request.Replace("##B_M_NAME##", "");
            request = request.Replace("##B_L_NAME##", billing.LastName);
            request = request.Replace("##B_ADDRESS##", billing.Address1);
            request = request.Replace("##B_ADDRESS2##", billing.Address2);
            request = request.Replace("##B_ADDRESS3##", "");
            request = request.Replace("##B_CITY##", billing.City);
            request = request.Replace("##B_STATE##", billing.State);
            request = request.Replace("##B_ZIP##", billing.Zip);
            request = request.Replace("##B_COUNTRY##", FixCountry(billing.Country));
            request = request.Replace("##B_EMAIL##", billing.Email);
            request = request.Replace("##B_PHONE##", billing.Phone);
            request = request.Replace("##B_DATE##", billing.CreateDT.Value.ToString("yyyyMMdd"));

            request = request.Replace("##PRODUCT##", productID.ToString());
            request = request.Replace("##REC##", "");

            request = request.Replace("##B_ID##", billing.BillingID.ToString());

            return request;
        }

        private static string SendRequest(string request)
        {
            WebClient wc = new WebClient();
            wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
            return wc.UploadString(URL, "POST", request);
        }

        private static bool ProcessResponse(string response)
        {
            if (string.IsNullOrEmpty(response))
                return false;

            if (response.Contains("code=1"))
                return true; // Success
            else
                return false; // Error
        }

        private static string FixCountry(string country)
        {
            if (country == null)
            {
                return null;
            }
            if (country.ToLower() == "uk" ||
                country.ToLower() == "united kingdom")
            {
                return "GB";
            }
            return country;
        }
    }
}
