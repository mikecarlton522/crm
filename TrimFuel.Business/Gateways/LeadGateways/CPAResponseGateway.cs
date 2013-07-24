using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using System.Web;

namespace TrimFuel.Business.Gateways.LeadGateways
{
    public class CPAResponseGateway : BaseLeadGateway, ILeadGateway
    {
        const string ABANDON_URL = "https://app3.bposerver.com/leads/api/submitLead/?";

        public void SaveAbandon(Registration reg, LeadRoutingView routingRule)
        {
            string apiKey = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.CPAResponse_API_KEY, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            StringBuilder strPostURL = new StringBuilder(ABANDON_URL);
            strPostURL.Append("lead_first_name=" + HttpUtility.UrlEncode(reg.FirstName));
            strPostURL.Append("&lead_last_name=" + HttpUtility.UrlEncode(reg.LastName));
            strPostURL.Append("&lead_ship_address_1=" + HttpUtility.UrlEncode(reg.Address1));
            strPostURL.Append("&lead_ship_address_2=" + HttpUtility.UrlEncode(reg.Address2));
            strPostURL.Append("&lead_ship_city=" + HttpUtility.UrlEncode(reg.City));
            strPostURL.Append("&lead_ship_state=" + HttpUtility.UrlEncode(reg.State));
            strPostURL.Append("&lead_email=" + HttpUtility.UrlEncode(reg.Email));
            strPostURL.Append("&lead_phone=" + HttpUtility.UrlEncode(reg.Phone));
            strPostURL.Append("&lead_ship_zipcode=" + HttpUtility.UrlEncode(reg.Zip));
            strPostURL.Append("&api_key=" + HttpUtility.UrlEncode(apiKey));

            SaveLeadPostNotCompleted(reg, routingRule, strPostURL.ToString());
        }

        public void SaveConfirm(Billing bill, LeadRoutingView routingRule)
        {
            throw new NotImplementedException();
        }

        public void SaveDeclined(Billing bill, LeadRoutingView routingRule)
        {
            throw new NotImplementedException();
        }

        public void SaveInactive(Billing bill, LeadRoutingView routingRule)
        {
            throw new NotImplementedException();
        }
    }
}
