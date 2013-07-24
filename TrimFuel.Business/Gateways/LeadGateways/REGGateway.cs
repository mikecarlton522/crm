using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using System.Net;
using System.Web;
using TrimFuel.Model.Containers;

namespace TrimFuel.Business.Gateways.LeadGateways
{
    public class REGGateway : BaseLeadGateway, ILeadGateway
    {
        private const string URLConfirm = "https://rtf.revenuegrp.com/data/leadpost.php?";
        private const string URLAbandon = "https://rtf.revenuegrp.com/data/leadpost.php?";
        private const string URLUnsub = "https://rtf.revenuegrp.com/data/unsub.php?";

        public void SaveAbandon(Registration reg, LeadRoutingView routingRule)
        {
            string clientid = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REG_ClientID, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            string clientkey = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REG_ClientKey, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            string campaigncode = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REG_CampaignCode, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);

            StringBuilder strPostURL = new StringBuilder(URLConfirm);
            strPostURL.Append("firstname=" + HttpUtility.UrlEncode(reg.FirstName));
            strPostURL.Append("&lastname=" + HttpUtility.UrlEncode(reg.LastName));
            strPostURL.Append("&email=" + HttpUtility.UrlEncode(reg.Email));
            strPostURL.Append("&address1=" + HttpUtility.UrlEncode(reg.Address1));
            strPostURL.Append("&address2=" + HttpUtility.UrlEncode(reg.Address2));
            strPostURL.Append("&city=" + HttpUtility.UrlEncode(reg.City));
            strPostURL.Append("&state=" + HttpUtility.UrlEncode(reg.State));
            strPostURL.Append("&zip=" + HttpUtility.UrlEncode(reg.Zip));
            strPostURL.Append("&homephone=" + HttpUtility.UrlEncode(reg.Phone));
            strPostURL.Append("&ip=" + HttpUtility.UrlEncode(reg.IP));
            strPostURL.Append("&url=" + HttpUtility.UrlEncode(reg.URL));
            strPostURL.Append("&leaddate=" + HttpUtility.UrlEncode(string.Format("{0:yyyy-MM-dd hh:mm:ss}", reg.CreateDT)));
            strPostURL.Append("&clientid=" + HttpUtility.UrlEncode(clientid));
            strPostURL.Append("&clientkey=" + HttpUtility.UrlEncode(clientkey));
            strPostURL.Append("&campaigncode=" + HttpUtility.UrlEncode(campaigncode));

            SaveLeadPostNotCompleted(reg, routingRule, strPostURL.ToString()); ;
        }

        public void SaveUnsub(Registration reg, LeadRoutingView routingRule)
        {
            string clientid = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REG_ClientID, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            string clientkey = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REG_ClientKey, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);

            StringBuilder strPostURL = new StringBuilder(URLUnsub);
            strPostURL.Append("&email=" + HttpUtility.UrlEncode(reg.Email));
            strPostURL.Append("&unsubdate=" + HttpUtility.UrlEncode(string.Format("{0:yyyy-MM-dd}", reg.CreateDT)));
            strPostURL.Append("&clientid=" + HttpUtility.UrlEncode(clientid));
            strPostURL.Append("&clientkey=" + HttpUtility.UrlEncode(clientkey));

            SaveLeadPostNotCompleted(reg, routingRule, strPostURL.ToString());
        }

        public void SaveConfirm(Billing bill, LeadRoutingView routingRule)
        {
            string clientid = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REG_ClientID, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            string clientkey = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REG_ClientKey, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            string campaigncode = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REG_CampaignCode, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);

            StringBuilder strPostURL = new StringBuilder(URLConfirm);
            strPostURL.Append("firstname=" + HttpUtility.UrlEncode(bill.FirstName));
            strPostURL.Append("&lastname=" + HttpUtility.UrlEncode(bill.LastName));
            strPostURL.Append("&email=" + HttpUtility.UrlEncode(bill.Email));
            strPostURL.Append("&address1=" + HttpUtility.UrlEncode(bill.Address1));
            strPostURL.Append("&address2=" + HttpUtility.UrlEncode(bill.Address2));
            strPostURL.Append("&city=" + HttpUtility.UrlEncode(bill.City));
            strPostURL.Append("&state=" + HttpUtility.UrlEncode(bill.State));
            strPostURL.Append("&zip=" + HttpUtility.UrlEncode(bill.Zip));
            strPostURL.Append("&country=" + HttpUtility.UrlEncode(bill.Country));
            strPostURL.Append("&homephone=" + HttpUtility.UrlEncode(bill.Phone));
            strPostURL.Append("&ip=" + HttpUtility.UrlEncode(bill.IP));
            strPostURL.Append("&url=" + HttpUtility.UrlEncode(bill.URL));
            strPostURL.Append("&leaddate=" + HttpUtility.UrlEncode(string.Format("{0:yyyy-MM-dd hh:mm:ss}", bill.CreateDT)));
            strPostURL.Append("&clientid=" + HttpUtility.UrlEncode(clientid));
            strPostURL.Append("&clientkey=" + HttpUtility.UrlEncode(clientkey));
            strPostURL.Append("&campaigncode=" + HttpUtility.UrlEncode(campaigncode));

            SaveLeadPostNotCompleted(bill, routingRule, strPostURL.ToString());
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
