using System;
using System.Linq;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using System.Collections.Generic;
using TrimFuel.Web.DynamicCampaign.Logic;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Views;

namespace TrimFuel.Web.DynamicCampaign
{
    public partial class PreLander : BaseDynamicPage
    {
        string SpecialOffer
        {
            get
            {
                return Request["__specialOffer"];
            }
        }

        protected string NextURL
        {
            get
            {
                return string.Format("http://{0}/Landing.aspx?CampaignID={1}&RegistrationID={2}{3}", Request.Url.Authority, CampaignID, ShoppingCart.RegistrationID == null ? Utility.TryGetInt(Request["RegistrationID"]) : ShoppingCart.RegistrationID, GetQuery());
            }
        }

        protected override void OnInit(EventArgs e)
        {
            string[] arr = Request.Url.Host.Split(".".ToCharArray());

            base.PageTypeID = PageTypeEnum.PreLander;

            base.OnInit(e);

            Master.Head = base.CampaignPage.Header;

            Master.Title = base.CampaignPage.Title;

            Master.Pixel = base.GetPixels();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _html.Text = base.HTML.Replace("#Next_URL#", NextURL);
        }

        protected override void FillDefaultPageValues()
        {
            base.FillDefaultPageValues();

            Billing existedBil = ShoppingCart.Billing;
            if (existedBil == null)
                existedBil = new Billing();

            BillingExternalInfo existedBilInfo = Dao.Load<BillingExternalInfo>(existedBil.BillingID ?? 0);
            if (existedBilInfo == null)
                existedBilInfo = new BillingExternalInfo();

            Registration existedReg = ShoppingCart.Registration;
            if (existedReg == null)
                if (!string.IsNullOrEmpty(Request["RegistrationID"]))
                {
                    existedReg = Dao.Load<Registration>(Request["RegistrationID"]);
                    ShoppingCart.RegistrationID = Utility.TryGetLong(Request["RegistrationID"]);
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

            string shippingCountry = existedRegInfo.Country;
            shippingCountry = string.IsNullOrEmpty(shippingCountry) ? US_COUNTRY : shippingCountry;

            string billingCountry = existedBil != null ? existedBil.Country : shippingCountry;

            SetValueWithDefault(ControlNames.Billing_First_Name, existedBil.FirstName ?? existedReg.FirstName);
            SetValueWithDefault(ControlNames.Billing_Last_Name, existedBil.LastName ?? existedReg.LastName);
            SetValueWithDefault(ControlNames.Billing_Address_1, existedBil.Address1 ?? existedReg.Address1);
            SetValueWithDefault(ControlNames.Billing_Address_2, existedBil.Address2 ?? existedReg.Address2);
            SetValueWithDefault(ControlNames.Billing_City, existedBil.City ?? existedReg.City);
            SetValueWithDefault(ControlNames.Billing_Country, billingCountry);
            SetValueWithDefault(ControlNames.Billing_Zip, existedBil.Zip ?? existedReg.Zip);
            SetValueWithDefault(ControlNames.Billing_Home_Tel, existedBil.Phone ?? existedReg.Phone);
            SetValueWithDefault(ControlNames.Billing_Email, existedReg.Email);
            SetValueWithDefault(ControlNames.Shipping_First_Name, existedReg.FirstName);
            SetValueWithDefault(ControlNames.Shipping_Last_Name, existedReg.LastName);
            SetValueWithDefault(ControlNames.Shipping_Address_1, existedReg.Address1);
            SetValueWithDefault(ControlNames.Shipping_Address_2, existedReg.Address2);
            SetValueWithDefault(ControlNames.Shipping_City, existedReg.City);
            SetValueWithDefault(ControlNames.Shipping_Country, shippingCountry);
            SetValueWithDefault(ControlNames.Shipping_Zip, existedReg.Zip);
            SetValueWithDefault(ControlNames.Shipping_Home_Tel, existedReg.Phone);
            SetValueWithDefault(ControlNames.Shipping_Email, existedReg.Email);

            SetValueWithDefault(ControlNames.Billing_CPF, existedBilInfo.CustomField1);
            SetValueWithDefault(ControlNames.Billing_Numero, existedBilInfo.CustomField2 ?? existedRegInfo.CustomField1);
            SetValueWithDefault(ControlNames.Billing_Complemento, existedBilInfo.CustomField3 ?? existedRegInfo.CustomField2);
            SetValueWithDefault(ControlNames.Billing_Card_Holder, existedBilInfo.CustomField4);

            SetValueWithDefault(ControlNames.Shipping_Neighborhood, existedRegInfo.Neighborhood);
            SetValueWithDefault(ControlNames.Shipping_Numero, existedRegInfo.CustomField1);
            SetValueWithDefault(ControlNames.Shipping_Complemento, existedRegInfo.CustomField2);

            if (shippingCountry == US_COUNTRY)
                SetValueWithDefault(ControlNames.Shipping_State_US, existedReg.State);
            else
            {
                if (shippingCountry == UK_COUNTRY)
                    SetValueWithDefault(ControlNames.Shipping_State_UK, existedReg.State);
                else
                    if (shippingCountry == CANADA_COUNTRY)
                        SetValueWithDefault(ControlNames.Shipping_State_Canada, existedReg.State);
                    else
                        if (shippingCountry == AUSTRALIA_COUNTRY)
                            SetValueWithDefault(ControlNames.Shipping_State_Australia, existedReg.State);
                        else
                            if (shippingCountry == ARGENTINA_COUNTRY)
                                SetValueWithDefault(ControlNames.Shipping_State_Argentina, existedReg.State);
                            else
                                if (shippingCountry == BRASIL_COUNTRY)
                                    SetValueWithDefault(ControlNames.Shipping_State_Brasil, existedReg.State);
                                else
                                    SetValueWithDefault(ControlNames.Shipping_State_Other, existedReg.State);
            }

            if (billingCountry == US_COUNTRY)
                SetValueWithDefault(ControlNames.Billing_State_US, existedBil.State);
            else
            {
                if (billingCountry == UK_COUNTRY)
                    SetValueWithDefault(ControlNames.Billing_State_UK, existedBil.State);
                else
                    if (billingCountry == CANADA_COUNTRY)
                        SetValueWithDefault(ControlNames.Billing_State_Canada, existedBil.State);
                    else
                        if (shippingCountry == AUSTRALIA_COUNTRY)
                            SetValueWithDefault(ControlNames.Billing_State_Australia, existedBil.State);
                        else
                            if (shippingCountry == ARGENTINA_COUNTRY)
                                SetValueWithDefault(ControlNames.Billing_State_Argentina, existedBil.State);
                            else
                                if (shippingCountry == BRASIL_COUNTRY)
                                    SetValueWithDefault(ControlNames.Billing_State_Brasil, existedBil.State);
                                else
                                    SetValueWithDefault(ControlNames.Billing_State_Other, existedBil.State);
            }
        }

        protected override void DoAction(PageActions action)
        {
            switch (action)
            {
                case PageActions.RegistrationBilling:
                    {
                        Set<Registration, Billing> res = null;
                        if (Config.Current.APPLICATION_ID == ApplicationEnum.TwoBoxCampaign)
                            res = UpdateBillingRegistration_TwoBox();
                        else
                            res = UpdateBillingRegistration();
                        if (res != null)
                        {
                            Response.Redirect(string.Format("http://{0}/Landing.aspx?CampaignID={1}&RegistrationID={2}{3}", Request.Url.Authority, CampaignID, res.Value1.RegistrationID, GetQuery()));
                        }
                        else
                        {
                            //was error
                        }

                        break;
                    }
            }
        }
    }
}
