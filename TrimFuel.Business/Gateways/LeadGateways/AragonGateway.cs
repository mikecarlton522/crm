using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using System.Web;

namespace TrimFuel.Business.Gateways.LeadGateways
{
    public class AragonGateway : BaseLeadGateway, ILeadGateway
    {
        private const string URL = "https://ios.aragoninc.com/occ.php?";

        public void SaveConfirm(TrimFuel.Model.Billing bill, TrimFuel.Model.Views.LeadRoutingView routingRule)
        {
            string campaign_id = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.Aragon_CampaignID, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            string company_id = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.Aragon_CompanyID, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            string password = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.Aragon_Password, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            StringBuilder strPostURL = new StringBuilder(URL);
            strPostURL.Append("company_id=" + HttpUtility.UrlEncode(company_id));
            strPostURL.Append("&password=" + HttpUtility.UrlEncode(password));
            strPostURL.Append("&campaign_id=" + HttpUtility.UrlEncode(campaign_id));
            strPostURL.Append("&unique_id=" + bill.BillingID);
            strPostURL.Append("&first_name=" + HttpUtility.UrlEncode(bill.FirstName));
            strPostURL.Append("&last_name=" + HttpUtility.UrlEncode(bill.LastName));
            strPostURL.Append("&street_address1=" + HttpUtility.UrlEncode(bill.Address1));
            strPostURL.Append("&street_address2=" + HttpUtility.UrlEncode(bill.Address2));
            strPostURL.Append("&city=" + HttpUtility.UrlEncode(bill.City));
            strPostURL.Append("&state=" + HttpUtility.UrlEncode(bill.State));
            strPostURL.Append("&zip=" + HttpUtility.UrlEncode(bill.Zip));
            strPostURL.Append("&phone=" + HttpUtility.UrlEncode(bill.Phone));
            strPostURL.Append("&email=" + HttpUtility.UrlEncode(bill.Email));
            strPostURL.Append("&card_type=" + HttpUtility.UrlEncode(GetCardType(bill.PaymentTypeID)));
            strPostURL.Append("&last_four=" + HttpUtility.UrlEncode(bill.CreditCardCnt.DecryptedCreditCardRight4));
            strPostURL.Append("&cvv=" + HttpUtility.UrlEncode(bill.CVV));
            strPostURL.Append("&date_ordered=" + bill.CreateDT.Value.ToString("yyyy-MM-dd hh:mm:ss"));
            strPostURL.Append("&cardholder=" + HttpUtility.UrlEncode(bill.FirstName + " " + bill.LastName));
            strPostURL.Append("&products_ordered=" + HttpUtility.UrlEncode(routingRule.ProductName));

            SaveLeadPostNotCompleted(bill, routingRule, strPostURL.ToString());
        }

        public void SaveAbandon(TrimFuel.Model.Registration reg, TrimFuel.Model.Views.LeadRoutingView routingRule)
        {
            throw new NotImplementedException();
        }

        public void SaveDeclined(TrimFuel.Model.Billing bill, TrimFuel.Model.Views.LeadRoutingView routingRule)
        {
            throw new NotImplementedException();
        }

        public void SaveInactive(TrimFuel.Model.Billing bill, TrimFuel.Model.Views.LeadRoutingView routingRule)
        {
            throw new NotImplementedException();
        }

        private string GetCardType(int? paymanetID)
        {
            string ccTypeString = string.Empty;
            if (paymanetID == 2)
                ccTypeString = "VI";
            if (paymanetID == 3)
                ccTypeString = "MC";
            if (paymanetID == 1)
                ccTypeString = "Amex";
            
            return ccTypeString;
        }
    }
}
