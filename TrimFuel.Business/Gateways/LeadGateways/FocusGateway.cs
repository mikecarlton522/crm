using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TrimFuel.Business.Gateways.LeadGateways
{
    public class FocusGateway : BaseLeadGateway, ILeadGateway
    {
        private const string ABANDON_URL = "http://leadgen.focusservices.com/default.aspx?";

        public void SaveAbandon(Model.Registration reg, Model.Views.LeadRoutingView routingRule)
        {
            string clientCode = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.Focus_ClientCode, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            StringBuilder strPostURL = new StringBuilder(ABANDON_URL);
            strPostURL.Append("ClientCode=" + HttpUtility.UrlEncode(clientCode));
            strPostURL.Append("&Phone=" + HttpUtility.UrlEncode(reg.Phone));
            strPostURL.Append("&FirstName=" + HttpUtility.UrlEncode(reg.FirstName));
            strPostURL.Append("&LastName=" + HttpUtility.UrlEncode(reg.LastName));
            strPostURL.Append("&Address=" + HttpUtility.UrlEncode(reg.Address1));
            strPostURL.Append("&Address2=" + HttpUtility.UrlEncode(reg.Address2));
            strPostURL.Append("&City=" + HttpUtility.UrlEncode(reg.City));
            strPostURL.Append("&State=" + HttpUtility.UrlEncode(reg.State));
            strPostURL.Append("&Zip=" + HttpUtility.UrlEncode(reg.Zip));
            strPostURL.Append("&Email=" + HttpUtility.UrlEncode(reg.Email));

            SaveLeadPostNotCompleted(reg, routingRule, strPostURL.ToString());
        }

        public void SaveConfirm(Model.Billing bill, Model.Views.LeadRoutingView routingRule)
        {
            throw new NotImplementedException();
        }

        public void SaveDeclined(Model.Billing bill, Model.Views.LeadRoutingView routingRule)
        {
            throw new NotImplementedException();
        }

        public void SaveInactive(Model.Billing bill, Model.Views.LeadRoutingView routingRule)
        {
            throw new NotImplementedException();
        }
    }
}
