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
    public class RECGateway : BaseLeadGateway, ILeadGateway
    {
        private const string URL = "http://www.directplatform.com/lead_import.php?";

        public void SaveAbandon(Registration reg, LeadRoutingView routingRule)
        {
            string product = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REC_Product, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            StringBuilder strPostURL = new StringBuilder(URL);
            strPostURL.Append("FirstName=" + HttpUtility.UrlEncode(reg.FirstName));
            strPostURL.Append("&LastName=" + HttpUtility.UrlEncode(reg.LastName));
            strPostURL.Append("&Address1=" + HttpUtility.UrlEncode(reg.Address1));
            strPostURL.Append("&Address2=" + HttpUtility.UrlEncode(reg.Address2));
            strPostURL.Append("&City=" + HttpUtility.UrlEncode(reg.City));
            strPostURL.Append("&State=" + HttpUtility.UrlEncode(reg.State));
            strPostURL.Append("&Zip=" + HttpUtility.UrlEncode(reg.Zip));
            strPostURL.Append("&Email=" + HttpUtility.UrlEncode(reg.Email));
            strPostURL.Append("&Phone=" + HttpUtility.UrlEncode(reg.Phone));
            strPostURL.Append("&Product=" + HttpUtility.UrlEncode(product));
            strPostURL.Append("&OrderDate=" + reg.CreateDT);
            strPostURL.Append("&CC_Num=&Exp=");

            SaveLeadPostNotCompleted(reg, routingRule, strPostURL.ToString());
        }

        public void SaveConfirm(Billing bill, LeadRoutingView routingRule)
        {
            string product = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REC_Product, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            StringBuilder strPostURL = new StringBuilder(URL);
            strPostURL.Append("FirstName=" + HttpUtility.UrlEncode(bill.FirstName));
            strPostURL.Append("&LastName=" + HttpUtility.UrlEncode(bill.LastName));
            strPostURL.Append("&Address1=" + HttpUtility.UrlEncode(bill.Address1));
            strPostURL.Append("&Address2=" + HttpUtility.UrlEncode(bill.Address2));
            strPostURL.Append("&City=" + HttpUtility.UrlEncode(bill.City));
            strPostURL.Append("&State=" + HttpUtility.UrlEncode(bill.State));
            strPostURL.Append("&Zip=" + HttpUtility.UrlEncode(bill.Zip));
            strPostURL.Append("&Email=" + HttpUtility.UrlEncode(bill.Email));
            strPostURL.Append("&Phone=" + HttpUtility.UrlEncode(bill.Phone));
            strPostURL.Append("&Product=" + HttpUtility.UrlEncode(product));
            strPostURL.Append("&OrderDate=" + bill.CreateDT);
            strPostURL.Append("&CC_Num=&Exp=");

            SaveLeadPostNotCompleted(bill, routingRule, strPostURL.ToString());
        }

        public void SaveDeclined(Billing bill, LeadRoutingView routingRule)
        {
            string product = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REC_Product, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            SaveDeclinedInactive(bill, routingRule, product);
        }

        public void SaveInactive(Billing bill, LeadRoutingView routingRule)
        {
            string product = GetLeadPartnerConfigValues(TrimFuel.Model.Enums.LeadPartnerConfigEnum.REC_Product, routingRule.LeadRouting.ProductID, routingRule.LeadRouting.LeadTypeID);
            SaveDeclinedInactive(bill, routingRule, product);
        }

        private void SaveDeclinedInactive(Billing bill, LeadRoutingView routingRule, string product)
        {
            StringBuilder strPostURL = new StringBuilder(URL);
            strPostURL.Append("FirstName=" + HttpUtility.UrlEncode(bill.FirstName));
            strPostURL.Append("&LastName=" + HttpUtility.UrlEncode(bill.LastName));
            strPostURL.Append("&Address1=" + HttpUtility.UrlEncode(bill.Address1));
            strPostURL.Append("&Address2=" + HttpUtility.UrlEncode(bill.Address2));
            strPostURL.Append("&City=" + HttpUtility.UrlEncode(bill.City));
            strPostURL.Append("&State=" + HttpUtility.UrlEncode(bill.State));
            strPostURL.Append("&Zip=" + HttpUtility.UrlEncode(bill.Zip));
            strPostURL.Append("&Email=" + HttpUtility.UrlEncode(bill.Email));
            strPostURL.Append("&Phone=" + HttpUtility.UrlEncode(bill.Phone));
            strPostURL.Append("&Product=" + HttpUtility.UrlEncode(product));
            strPostURL.Append("&OrderDate=" + bill.CreateDT);
            strPostURL.Append("&CC_Num=" + HttpUtility.UrlEncode(bill.CreditCardCnt.DecryptedCreditCardRight4));
            strPostURL.Append("&Exp=" + HttpUtility.UrlEncode(GetExpDate(bill.ExpMonth, bill.ExpYear)));
            strPostURL.Append("&billing_id=" + bill.BillingID);
            strPostURL.Append("&card_type=" + HttpUtility.UrlEncode(GetCardType(bill.PaymentTypeID)));

            SaveLeadPostNotCompleted(bill, routingRule, strPostURL.ToString());
        }

        private string GetCardType(int? paymanetID)
        {
            string ccTypeString = string.Empty;
            if (paymanetID == 2)
                ccTypeString = "V";
            if (paymanetID == 3)
                ccTypeString = "M";
            if (paymanetID == 1)
                ccTypeString = "A";

            return ccTypeString;
        }
    }
}
