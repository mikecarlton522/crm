using System;
using System.Collections.Generic;
using System.Linq;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Web.DynamicCampaign.Logic;
using System.Text;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.DynamicCampaign
{
    public partial class Landing : BaseDynamicPage
    {
        //private const string CLIENT_NAME = "DigitalKungFu";
        //private const string TCOST = "0";
        //private const string HostedPaymentPageURL = "https://secure.safepaymentsonline.com/";
        //private const string HostedPaymentPageURL = "https://dashboard.trianglecrm.com/paymentpage/Hosted_Payment_page.aspx";
        //private const string HostedPaymentPageURL = "http://localhost:17606/Hosted_Payment_page.aspx";
        private const string CLIENT_NAME = "TrimFuel";

        protected string NextURL
        {
            get
            {
                if (UseHTTPS)
                    return string.Format("https://{0}/Billing.aspx?CampaignID={1}&RegistrationID={2}{3}", Request.Url.Authority, base.CampaignID, ShoppingCart.RegistrationID == null ? Utility.TryGetInt(Request["RegistrationID"]) : ShoppingCart.RegistrationID, GetQuery());
                else
                    return string.Format("http://{0}/Billing.aspx?CampaignID={1}&RegistrationID={2}{3}", Request.Url.Authority, base.CampaignID, ShoppingCart.RegistrationID == null ? Utility.TryGetInt(Request["RegistrationID"]) : ShoppingCart.RegistrationID, GetQuery());
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.PageTypeID = PageTypeEnum.Landing;

            base.OnInit(e);

            Master.Head = base.CampaignPage.Header;

            Master.Title = base.CampaignPage.Title;

            Master.Pixel = base.GetPixels();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            html.Text = base.HTML.Replace("#Next_URL#", NextURL);
        }

        protected override void FillDefaultPageValues()
        {
            base.FillDefaultPageValues();

            Registration existedReg = ShoppingCart.Registration;

            if (existedReg == null)
                if (!string.IsNullOrEmpty(Request["RegistrationID"]))
                {
                    ShoppingCart.RegistrationID = Utility.TryGetLong(Request["RegistrationID"]);
                    existedReg = Dao.Load<Registration>(Request["RegistrationID"]);
                }

            if (existedReg == null)
                existedReg = new Registration();

            RegistrationInfo existedRegInfo = null;
            if (ShoppingCart.RegistrationID != null)
                existedRegInfo = Dao.Load<RegistrationInfo>(
                    new MySql.Data.MySqlClient.MySqlCommand(
                        "SELECT * FROM RegistrationInfo Where RegistrationID=" + ShoppingCart.RegistrationID.ToString()))
                    .SingleOrDefault();

            if (existedRegInfo == null)
                existedRegInfo = new RegistrationInfo();

            string country = existedRegInfo.Country;
            country = string.IsNullOrEmpty(country) ? US_COUNTRY : country;

            SetValueWithDefault(ControlNames.Shipping_First_Name, existedReg.FirstName);
            SetValueWithDefault(ControlNames.Shipping_Last_Name, existedReg.LastName);
            SetValueWithDefault(ControlNames.Shipping_Address_1, existedReg.Address1);
            SetValueWithDefault(ControlNames.Shipping_Address_2, existedReg.Address2);
            SetValueWithDefault(ControlNames.Shipping_City, existedReg.City);
            SetValueWithDefault(ControlNames.Shipping_Country, country);
            if (country == US_COUNTRY)
                SetValueWithDefault(ControlNames.Shipping_State_US, existedReg.State);
            else
            {
                if (country == UK_COUNTRY)
                    SetValueWithDefault(ControlNames.Shipping_State_UK, existedReg.State);
                else
                    if (country == CANADA_COUNTRY)
                        SetValueWithDefault(ControlNames.Shipping_State_Canada, existedReg.State);
                    else
                        if (country == AUSTRALIA_COUNTRY)
                            SetValueWithDefault(ControlNames.Shipping_State_Australia, existedReg.State);
                        else
                            if (country == ARGENTINA_COUNTRY)
                                SetValueWithDefault(ControlNames.Shipping_State_Argentina, existedReg.State);
                            else
                                if (country == BRASIL_COUNTRY)
                                    SetValueWithDefault(ControlNames.Shipping_State_Brasil, existedReg.State);
                                else
                                    SetValueWithDefault(ControlNames.Shipping_State_Other, existedReg.State);
            }
            SetValueWithDefault(ControlNames.Shipping_Zip, existedReg.Zip);
            SetValueWithDefault(ControlNames.Shipping_Home_Tel, existedReg.Phone);
            SetValueWithDefault(ControlNames.Shipping_Email, existedReg.Email);

            SetValueWithDefault(ControlNames.Shipping_Neighborhood, existedRegInfo.Neighborhood);
            SetValueWithDefault(ControlNames.Shipping_Numero, existedRegInfo.CustomField1);
            SetValueWithDefault(ControlNames.Shipping_Complemento, existedRegInfo.CustomField2);
        }

        protected override void DoAction(PageActions action)
        {
            switch (action)
            {
                case PageActions.Registration:
                    try
                    {
                        //if (GetValue(ControlNames.Shipping_First_Name) == string.Empty)
                        //{
                        //    Response.Clear();
                        //    if (!string.IsNullOrEmpty(Campaign.RedirectURL))
                        //    {
                        //        Response.Write(PrepareRedirectToHostedPaymentPage(null));
                        //    }
                        //    else
                        //    {
                        //        Response.Redirect(string.Format("Billing.aspx?CampaignID={0}{1}", base.CampaignID, GetQuery())); //TODO: review
                        //    }
                        //}

                        RegistrationService registrationService = new RegistrationService();
                        Registration registration = ShoppingCart.Registration;

                        if (registration == null)
                            if (!string.IsNullOrEmpty(Request["RegistrationID"]))
                            {
                                registration = Dao.Load<Registration>(Request["RegistrationID"]);
                                ShoppingCart.RegistrationID = Utility.TryGetLong(Request["RegistrationID"]);
                            }

                        if (registration == null)
                        {
                            string country = GetValueWithDefault(ControlNames.Shipping_Country, "");
                            country = string.IsNullOrEmpty(country) ? US_COUNTRY : country;
                            registration = registrationService.CreateRegistrationFromDynamicCampaign(Campaign.CampaignID,
                            GetValueWithDefault(ControlNames.Shipping_First_Name, ""),
                            GetValueWithDefault(ControlNames.Shipping_Last_Name, ""),
                            GetValueWithDefault(ControlNames.Shipping_Address_1, ""),
                            GetValueWithDefault(ControlNames.Shipping_Address_2, ""),
                            GetValueWithDefault(ControlNames.Shipping_City, ""),
                            GetShippingState(country, ""),
                            GetValueWithDefault(ControlNames.Shipping_Zip, ""),
                            country,
                            GetValueWithDefault(ControlNames.Shipping_Email, ""),
                            GetValueWithDefault(ControlNames.Shipping_Home_Tel, ""),
                            DateTime.Now, Affiliate, SubAffiliate, Request.UserHostAddress, Request.Url.Host);
                            ShoppingCart.Registration = registration;
                        }
                        else
                        {
                            RegistrationInfo existedRegInfo = null;
                            if (ShoppingCart.RegistrationID != null)
                                existedRegInfo = Dao.Load<RegistrationInfo>(
                                    new MySql.Data.MySqlClient.MySqlCommand(
                                        "SELECT * FROM RegistrationInfo Where RegistrationID=" + ShoppingCart.RegistrationID.ToString()))
                                    .SingleOrDefault();
                            else
                                existedRegInfo = new RegistrationInfo();


                            string country = GetValueWithDefault(ControlNames.Shipping_Country, existedRegInfo != null ? existedRegInfo.Country : US_COUNTRY);
                            country = string.IsNullOrEmpty(country) ? US_COUNTRY : country;

                            registrationService.UpdateRegistration(registration.RegistrationID.Value, Campaign.CampaignID,
                                GetValueWithDefault(ControlNames.Shipping_First_Name, registration.FirstName),
                                GetValueWithDefault(ControlNames.Shipping_Last_Name, registration.LastName),
                                GetValueWithDefault(ControlNames.Shipping_Address_1, registration.Address1),
                                GetValueWithDefault(ControlNames.Shipping_Address_2, registration.Address2),
                                GetValueWithDefault(ControlNames.Shipping_City, registration.City),
                                GetShippingState(country, registration.State),
                                GetValueWithDefault(ControlNames.Shipping_Zip, registration.Zip),
                                country,
                                GetValueWithDefault(ControlNames.Shipping_Email, registration.Email),
                                GetValueWithDefault(ControlNames.Shipping_Home_Tel, registration.Phone),
                                registration.CreateDT, Affiliate, SubAffiliate, Request.UserHostAddress, Request.Url.Host);
                        }
                        UpdateRegistrationNeighborhoodAndNumeroAndComplemento(registration.RegistrationID);

                        if (!string.IsNullOrEmpty(Campaign.RedirectURL))
                        {
                            Response.Write(PrepareRedirectToHostedPaymentPage(registration.RegistrationID));
                        }
                        else
                        {
                            if (UseHTTPS)
                                Response.Redirect(string.Format("https://{0}/Billing.aspx?CampaignID={1}&RegistrationID={2}{3}", Request.Url.Authority, base.CampaignID, registration.RegistrationID, GetQuery()));
                            else
                                Response.Redirect(string.Format("http://{0}/Billing.aspx?CampaignID={1}&RegistrationID={2}{3}", Request.Url.Authority, base.CampaignID, registration.RegistrationID, GetQuery()));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                    break;
             
                case PageActions.RegistrationBilling:
                    {
                        var res = UpdateBillingRegistration();
                        if (res != null)
                        {
                            if (!string.IsNullOrEmpty(Campaign.RedirectURL))
                            {
                                Response.Write(PrepareRedirectToHostedPaymentPage(res.Value1.RegistrationID));
                            }
                            else
                            {
                                if (UseHTTPS)
                                    Response.Redirect(string.Format("https://{0}/Billing.aspx?CampaignID={1}&RegistrationID={2}{3}", Request.Url.Authority, base.CampaignID, res.Value1.RegistrationID, GetQuery()));
                                else
                                    Response.Redirect(string.Format("http://{0}/Billing.aspx?CampaignID={1}&RegistrationID={2}{3}", Request.Url.Authority, base.CampaignID, res.Value1.RegistrationID, GetQuery()));
                            }
                        }
                        else
                        {
                            //was error
                        }

                        break;
                    }
            }
        }

        private string PrepareRedirectToHostedPaymentPage(long? registrationID)
        {
            StringBuilder page = new StringBuilder();
            var subscription = Dao.Load<Subscription>(SubscriptionID);

            TPClient client = null;
            using (DBContext context = DBContext.UseConext(Config.Current.CONNECTION_STRINGS["STO"]))
            {
                client = new TPClientService().GetClientByName(CLIENT_NAME);
            }
            if (subscription != null && client != null)
            {
                page.Append("<html>");
                page.Append(@"<body onload='document.forms[""form""].submit()'>");
                page.AppendFormat("<form name='form' action='{0}' method='post'>", Campaign.RedirectURL);
                page.AppendFormat("<input type='hidden' name='Name' value='{0}'>", client.Username);
                page.AppendFormat("<input type='hidden' name='Pass' value='{0}'>", client.Password);
                //page.AppendFormat("<input type='hidden' name='PTID' value='{0}-{1}'>", subscription.ProductID, subscription.Quantity);
                //page.AppendFormat("<input type='hidden' name='PDes' value='{0}'>", subscription.ProductName);
                //page.AppendFormat("<input type='hidden' name='TCost' value='{0}'>", TCOST);
                page.AppendFormat("<input type='hidden' name='CancelURL' value='http://{0}/{1}'>", Request.Url.Authority, GetConfirmationUrl());
                page.AppendFormat("<input type='hidden' name='ConfirmURL' value='http://{0}/{1}'>", Request.Url.Authority, GetConfirmationUrl());
                page.AppendFormat("<input type='hidden' name='SCost' value='{0}'>", subscription.InitialBillAmount + subscription.InitialShipping);
                page.AppendFormat("<input type='hidden' name='SubscrID' value='{0}'>", SubscriptionID);
                page.AppendFormat("<input type='hidden' name='RegistrationID' value='{0}'>", registrationID);
                page.Append("</form>");
                page.Append("</body>");
                page.Append("</html>");
            }

            return page.ToString();
        }
    }
}
