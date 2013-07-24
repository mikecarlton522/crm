using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using MySql.Data.MySqlClient;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.Admin
{
    public partial class tc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int id = 0;

            if (!string.IsNullOrEmpty(Request["id"]) && int.TryParse(Request["id"], out id))
            {
                if (!string.IsNullOrEmpty(Request["type"]))
                {
                    CampaignService campaignService = new CampaignService();


                    string type = Request["type"];

                    Campaign campaign  = campaignService.GetCampaignByID(id);

                    string output = string.Empty;
                    TermView view = campaignService.GetTerms(id);

                    switch (type)
                    {
                        case "pp": //show privacy policy
                            output = GetPrivacyPolicy(view, campaign.URL);
                            break;

                        case "tc": //show terms and conditions
                            output = GetTermsAndConditions(view, campaign.URL);
                            break;

                        case "contact": //show contact info
                            output = GetContactInfo(view, campaign.URL);
                            break;

                        default:
                            output = string.Empty;
                            break;

                    }
                    Response.Write(output);

                }
            }
        }

        private string GetPrivacyPolicy(TermView view, string domain)
        {
            if (view == null)
                return string.Empty;

            StringBuilder privacyPolicy = new StringBuilder(view.PrivacyPolicy);

            privacyPolicy.Replace("##CORPORATION##", view.CorporationBody);
            privacyPolicy.Replace("##CORPORATION_NAME##", view.CorporationName);
            privacyPolicy.Replace("##RETURN##", view.ReturnAddressBody);
            privacyPolicy.Replace("##RETURN_NAME##", view.ReturnAddressName);
            privacyPolicy.Replace("##SUPPORTPHONE##", view.Phone);
            privacyPolicy.Replace("##SUPPORTEMAIL##", view.Email);
            privacyPolicy.Replace("##DOMAIN##", domain);

            privacyPolicy.Replace("\n", "<br />");

            return privacyPolicy.ToString();
        }

        private string GetTermsAndConditions(TermView view, string domain)
        {
            if (view == null)
                return string.Empty;

            StringBuilder outline = new StringBuilder(view.Outline);

            outline.Replace("##TERMS_MEMBERSHIP##", view.MembershipTerms);
            outline.Replace("##CORPORATION##", view.CorporationBody);
            outline.Replace("##CORPORATION_NAME##", view.CorporationName);
            outline.Replace("##RETURN##", view.ReturnAddressBody);
            outline.Replace("##RETURN_NAME##", view.ReturnAddressName);
            outline.Replace("##SUPPORTPHONE##", view.Phone);
            outline.Replace("##SUPPORTEMAIL##", view.Email);
            outline.Replace("##DOMAIN##", domain);

            outline.Replace("\n", "<br />");

            return outline.ToString();
        }

        private string GetContactInfo(TermView view, string domain)
        {
            if (view == null)
                return string.Empty;

            StringBuilder contact = new StringBuilder();

            contact.AppendLine("Contact Information for ##DOMAIN##");
            contact.AppendLine();
            contact.AppendLine("##RETURN##");
            contact.AppendLine();
            contact.AppendLine("Company Address (Please do NOT send products or packages to this address):");
            contact.AppendLine("##CORPORATION##");
            contact.AppendLine();
            contact.AppendLine("Support Telephone Number: ##SUPPORTPHONE##");
            contact.AppendLine("Support Email Address: ##SUPPORTEMAIL##");

            contact.Replace("##CORPORATION##", view.CorporationBody);
            contact.Replace("##CORPORATION_NAME##", view.CorporationName);
            contact.Replace("##RETURN##", view.ReturnAddressBody);
            contact.Replace("##RETURN_NAME##", view.ReturnAddressName);
            contact.Replace("##SUPPORTPHONE##", view.Phone);
            contact.Replace("##SUPPORTEMAIL##", view.Email);
            contact.Replace("##DOMAIN##", domain);

            contact.Replace("\n", "<br />");

            return contact.ToString();
        }
    }
}