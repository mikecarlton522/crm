using System;
using System.Globalization;
using System.Linq;

using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Web.DynamicCampaign.Logic;
using TrimFuel.Business.ABFApi;
using TrimFuel.Business.Utils;
using System.Collections.Generic;
using System.Text;
using TrimFuel.Business.Gateways;

namespace TrimFuel.Web.DynamicCampaign
{
    public partial class BillingPage : BaseDynamicPage
    {
        string SpecialOffer
        {
            get
            {
                return Request["__specialOffer"];
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.PageTypeID = PageTypeEnum.Billing;

            base.OnInit(e);

            Master.Head = base.CampaignPage.Header;

            Master.Title = base.CampaignPage.Title;

            Master.Pixel = base.GetPixels();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            html.Text = base.HTML;
        }

        protected override void FillDefaultPageValues()
        {
            base.FillDefaultPageValues();
            ShoppingCart.SubscriptionID = SubscriptionID.Value;

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
            if (existedReg.RegistrationID != null)
                existedRegInfo = Dao.Load<RegistrationInfo>(
                    new MySql.Data.MySqlClient.MySqlCommand(
                        "SELECT * FROM RegistrationInfo Where RegistrationID=" + existedReg.RegistrationID.ToString()))
                    .SingleOrDefault();

            if (existedRegInfo == null)
                existedRegInfo = new RegistrationInfo();

            string shippingCountry = existedRegInfo.Country;
            shippingCountry = string.IsNullOrEmpty(shippingCountry) ? US_COUNTRY : shippingCountry;

            string billingCountry = existedRegInfo.Country;
            billingCountry = string.IsNullOrEmpty(billingCountry) ? US_COUNTRY : billingCountry;

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

            if (ShoppingCart.Subscription != null)
            {
                var productPrice = (decimal)ShoppingCart.Subscription.InitialBillAmount;
                var shippingPrice = (decimal)ShoppingCart.Subscription.InitialShipping;
                var totalPrice = productPrice + shippingPrice;

                SetValueWithDefault(ControlNames.Billing_Product_Price, productPrice.ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
                SetValueWithDefault(ControlNames.Billing_Shipping_Price, shippingPrice.ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
                SetValueWithDefault(ControlNames.Billing_Total_Price, totalPrice.ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
            }

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
                SetValueWithDefault(ControlNames.Billing_State_US, existedBil.State ?? existedReg.State);
            else
            {
                if (billingCountry == UK_COUNTRY)
                    SetValueWithDefault(ControlNames.Billing_State_UK, existedBil.State ?? existedReg.State);
                else
                    if (billingCountry == CANADA_COUNTRY)
                        SetValueWithDefault(ControlNames.Billing_State_Canada, existedBil.State ?? existedReg.State);
                    else
                        if (shippingCountry == AUSTRALIA_COUNTRY)
                            SetValueWithDefault(ControlNames.Billing_State_Australia, existedBil.State ?? existedReg.State);
                        else
                            if (shippingCountry == ARGENTINA_COUNTRY)
                                SetValueWithDefault(ControlNames.Billing_State_Argentina, existedBil.State ?? existedReg.State);
                            else
                                if (shippingCountry == BRASIL_COUNTRY)
                                    SetValueWithDefault(ControlNames.Billing_State_Brasil, existedBil.State ?? existedReg.State);
                                else
                                    SetValueWithDefault(ControlNames.Billing_State_Other, existedBil.State ?? existedReg.State);
            }
        }

        protected override void DoAction(PageActions action)
        {
            var subscripton = new Dictionary<int, int>();
            subscripton.Add(SubscriptionID.Value, 1);

            var registrationService = new RegistrationService();
            var coupon = registrationService.GetCouponForSubscription(GetValue(ControlNames.Coupon_Code), SubscriptionID.Value);

            switch (action)
            {
                case PageActions.Coupon:
                    {
                        ShoppingCart.SubscriptionID = SubscriptionID.Value;

                        if (ShoppingCart.Subscription != null)
                        {
                            var productPrice = (decimal)ShoppingCart.Subscription.InitialBillAmount;
                            var shippingPrice = (decimal)ShoppingCart.Subscription.InitialShipping;
                            var totalPrice = productPrice + shippingPrice;

                            SetValueWithDefault(ControlNames.Billing_Product_Price, productPrice.ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
                            SetValueWithDefault(ControlNames.Billing_Shipping_Price, shippingPrice.ToString("C", CultureInfo.CreateSpecificCulture("en-US")));
                            SetValueWithDefault(ControlNames.Billing_Total_Price, totalPrice.ToString("C", CultureInfo.CreateSpecificCulture("en-US")));

                            if (coupon != null)
                                SetValueWithDefault(ControlNames.Billing_Total_Price, coupon.ApplyDiscount(totalPrice, DiscountType.Any).ToString("C", CultureInfo.CreateSpecificCulture("en-US")));

                        }

                        break;
                    }

                //case PageActions.Debit:
                //    {
                //        if (Config.Current.APPLICATION_ID == ApplicationEnum.TwoBoxCampaign)
                //            UpdateBillingRegistration_TwoBox();
                //        else
                //            UpdateBillingRegistration();

                //        Billing existedBil = ShoppingCart.Billing;
                //        if (existedBil == null)
                //            existedBil = new Billing();

                //        var debitCardRequest = new SaleService().CreateDebitCardRequest(existedBil.BillingID, SubscriptionID);
                //        Response.Redirect(string.Format("/PagadorRedirect.aspx?BillingID={0}&OrderNumber={1}", debitCardRequest.BillingID, debitCardRequest.DebitCardRequestID));
                        
                //        break;
                //    }

                case PageActions.Billing:
                    {
                        if (Utility.TryGetInt(Request["Installments"]) != null)
                            PaymentExVars.Installments = Utility.TryGetInt(Request["Installments"]).Value;

                        if (Config.Current.APPLICATION_ID == ApplicationEnum.TwoBoxCampaign)
                            UpdateBillingRegistration_TwoBox();
                        else
                            UpdateBillingRegistration();

                        Billing existedBil = ShoppingCart.Billing;
                        if (existedBil == null)
                            existedBil = new Billing();

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
                        if (existedReg.RegistrationID != null)
                            existedRegInfo = Dao.Load<RegistrationInfo>(
                                new MySql.Data.MySqlClient.MySqlCommand(
                                    "SELECT * FROM RegistrationInfo Where RegistrationID=" + existedReg.RegistrationID.ToString()))
                                .SingleOrDefault();
                        else
                            existedRegInfo = new RegistrationInfo();

                        string billingCountry = GetValueWithDefault(ControlNames.Billing_Country, string.IsNullOrEmpty(existedBil.Country) ? existedRegInfo.Country : existedBil.Country);
                        billingCountry = string.IsNullOrEmpty(billingCountry) ? US_COUNTRY : billingCountry;
                        string shippingCountry = GetValueWithDefault(ControlNames.Shipping_Country, existedRegInfo != null ? existedRegInfo.Country : US_COUNTRY);
                        shippingCountry = string.IsNullOrEmpty(shippingCountry) ? US_COUNTRY : shippingCountry;

                        var res = new SaleService().CreateComplexSaleFromDynamicCampaign(existedBil.BillingID, existedReg.RegistrationID,
                            null,
                            GetValueWithDefault(ControlNames.Billing_First_Name, string.IsNullOrEmpty(existedBil.FirstName) ? existedReg.FirstName : existedBil.FirstName),
                            GetValueWithDefault(ControlNames.Billing_Last_Name, string.IsNullOrEmpty(existedBil.LastName) ? existedReg.LastName : existedBil.LastName),
                            GetValueWithDefault(ControlNames.Billing_Address_1, string.IsNullOrEmpty(existedBil.Address1) ? existedReg.Address1 : existedBil.Address1),
                            GetValueWithDefault(ControlNames.Billing_Address_2, string.IsNullOrEmpty(existedBil.Address2) ? existedReg.Address2 : existedBil.Address2),
                            GetValueWithDefault(ControlNames.Billing_City, string.IsNullOrEmpty(existedBil.City) ? existedReg.City : existedBil.City),
                            GetBillingState(billingCountry, existedBil, existedReg),
                            string.IsNullOrEmpty(existedBil.Zip) ? existedReg.Zip : existedBil.Zip,
                            billingCountry,
                            string.IsNullOrEmpty(existedBil.Phone) ? existedReg.Phone : existedBil.Phone,
                            GetValueWithDefault(ControlNames.Billing_Email, string.IsNullOrEmpty(existedBil.Email) ? existedReg.Email : existedBil.Email),
                            GetValueWithDefault(ControlNames.Shipping_First_Name, existedReg.FirstName),
                            GetValueWithDefault(ControlNames.Shipping_Last_Name, existedReg.LastName),
                            GetValueWithDefault(ControlNames.Shipping_Address_1, existedReg.Address1),
                            GetValueWithDefault(ControlNames.Shipping_Address_2, existedReg.Address2),
                            GetValueWithDefault(ControlNames.Shipping_City, existedReg.City),
                            GetShippingState(shippingCountry, existedReg.State),
                            existedReg.Zip,
                            shippingCountry,
                            existedReg.Phone,
                            GetValueWithDefault(ControlNames.Shipping_Email, existedReg.Email),
                            Campaign.CampaignID,
                            Affiliate,
                            SubAffiliate,
                            Request.UserHostAddress,
                            Request.Url.Host + Request.RawUrl,
                            Utility.TryGetInt(GetValue(ControlNames.CC_Type)),
                            existedBil.CreditCardCnt.DecryptedCreditCard,
                            GetValue(ControlNames.CC_CVV),
                            Utility.TryGetInt(GetValue(ControlNames.CC_Month)),
                            Utility.TryGetInt(GetValue(ControlNames.CC_Year)),
                            CID,
                            !string.IsNullOrEmpty(SpecialOffer),
                            coupon != null ? GetValue(ControlNames.Coupon_Code) : "",
                            "",
                            null,
                            new List<string>(),
                            subscripton,
                            new Dictionary<int, int>()
                        );

                        if (res != null && res.ReturnValue != null)
                        {
                            ShoppingCart.Registration = res.ReturnValue.Registration;
                            ShoppingCart.Billing = res.ReturnValue.ParentBilling;
                            if (res.ReturnValue.BillingSales != null && res.ReturnValue.BillingSales.Count > 0)
                            {
                                ShoppingCart.BillingSubscription = res.ReturnValue.BillingSales.Last().Value2;
                            }
                            ShoppingCart.AssertigyMID = res.ReturnValue.AssertigyMID;
                            //UpdateBillingCPFAndNeighborhoodandAndNumeroAndComplementoAndCardHolder(ShoppingCart.Billing.BillingID);
                            //UpdateRegistrationNeighborhoodAndNumeroAndComplemento(ShoppingCart.Registration.RegistrationID);
                        }

                        if (res == null || res.State == BusinessErrorState.Error)
                        {
                            if (Config.Current.APPLICATION_ID == ApplicationEnum.TwoBoxCampaign)
                            {
                                //hack for 2box
                                SetErrorMessage("Seu cartão foi recusado. Por favor, verifique se toda a informação fornecida é correta ou tente usar outro cartão ou contatar o suporte ao cliente em 4003-3833 para concluir seu pedido.");
                            }
                            else
                            {
                                if (res != null)
                                    SetErrorMessage(res.ErrorMessage);
                            }
                            FillDefaultPageValues();
                        }
                        else
                        {
                            GoToUpsell();
                        }
                        break;
                    }

                case PageActions.BillingAsync:
                    {
                        if (Config.Current.APPLICATION_ID == ApplicationEnum.TwoBoxCampaign)
                            UpdateBillingRegistration_TwoBox();
                        else
                            UpdateBillingRegistration();

                        Billing existedBil = ShoppingCart.Billing;
                        if (existedBil == null)
                            existedBil = new Billing();

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
                        else
                            existedRegInfo = new RegistrationInfo();

                        string billingCountry = GetValueWithDefault(ControlNames.Billing_Country, existedBil.Country ?? (existedRegInfo != null ? existedRegInfo.Country : US_COUNTRY));
                        billingCountry = string.IsNullOrEmpty(billingCountry) ? US_COUNTRY : billingCountry;
                        string shippingCountry = GetValueWithDefault(ControlNames.Shipping_Country, existedRegInfo != null ? existedRegInfo.Country : US_COUNTRY);
                        shippingCountry = string.IsNullOrEmpty(shippingCountry) ? US_COUNTRY : shippingCountry;

                        var res = new SaleService().CreateComplexSaleFromDynamicCampaign(existedBil.BillingID, existedReg.RegistrationID,
                            null,
                            GetValueWithDefault(ControlNames.Billing_First_Name, existedBil.FirstName ?? existedReg.FirstName),
                            GetValueWithDefault(ControlNames.Billing_Last_Name, existedBil.LastName ?? existedReg.LastName),
                            GetValueWithDefault(ControlNames.Billing_Address_1, existedBil.Address1 ?? existedReg.Address1),
                            GetValueWithDefault(ControlNames.Billing_Address_2, existedBil.Address2 ?? existedReg.Address2),
                            GetValueWithDefault(ControlNames.Billing_City, existedBil.City ?? existedReg.City),
                            GetBillingState(billingCountry, existedBil, existedReg),
                            GetValueWithDefault(ControlNames.Billing_Zip, existedBil.Zip ?? existedReg.Zip),
                            billingCountry,
                            GetValueWithDefault(ControlNames.Billing_Home_Tel, existedBil.Phone ?? existedReg.Phone),
                            GetValueWithDefault(ControlNames.Billing_Email, existedReg.Email),
                            GetValueWithDefault(ControlNames.Shipping_First_Name, existedReg.FirstName),
                            GetValueWithDefault(ControlNames.Shipping_Last_Name, existedReg.LastName),
                            GetValueWithDefault(ControlNames.Shipping_Address_1, existedReg.Address1),
                            GetValueWithDefault(ControlNames.Shipping_Address_2, existedReg.Address2),
                            GetValueWithDefault(ControlNames.Shipping_City, existedReg.City),
                            GetShippingState(shippingCountry, existedReg.State),
                            GetValueWithDefault(ControlNames.Shipping_Zip, existedReg.Zip),
                            shippingCountry,
                            GetValueWithDefault(ControlNames.Shipping_Home_Tel, existedReg.Phone),
                            GetValueWithDefault(ControlNames.Shipping_Email, existedReg.Email),
                            Campaign.CampaignID,
                            Affiliate,
                            SubAffiliate,
                            Request.UserHostAddress,
                            Request.Url.Host + Request.RawUrl,
                            Utility.TryGetInt(GetValue(ControlNames.CC_Type)),
                            GetValue(ControlNames.CC_Number),
                            GetValue(ControlNames.CC_CVV),
                            Utility.TryGetInt(GetValue(ControlNames.CC_Month)),
                            Utility.TryGetInt(GetValue(ControlNames.CC_Year)),
                            CID,
                            !string.IsNullOrEmpty(SpecialOffer),
                            "",
                            "",
                            null,
                            new List<string>(),
                            subscripton,
                            new Dictionary<int, int>()
                        );

                        if (res != null && res.ReturnValue != null)
                        {
                            ShoppingCart.Registration = res.ReturnValue.Registration;
                            ShoppingCart.Billing = res.ReturnValue.ParentBilling;
                            if (res.ReturnValue.BillingSales != null && res.ReturnValue.BillingSales.Count > 0)
                            {
                                ShoppingCart.BillingSubscription = res.ReturnValue.BillingSales.Last().Value2;
                            }
                            ShoppingCart.AssertigyMID = res.ReturnValue.AssertigyMID;
                            //UpdateBillingCPFAndNeighborhoodandAndNumeroAndComplementoAndCardHolder(ShoppingCart.Billing.BillingID);
                            //UpdateRegistrationNeighborhoodAndNumeroAndComplemento(ShoppingCart.Registration.RegistrationID);
                        }

                        if (res == null || res.State == BusinessErrorState.Error)
                        {
                            Response.Write(string.Format("{{success: false, bid: {0}, message: '{1}'}}",
                                ShoppingCart.Billing.BillingID, res.ErrorMessage.Replace("'", "&lsquo;")));
                        }
                        else
                        {
                            Response.Write(string.Format("{{success: true, url: '{0}'}}",
                                 GetUpsellUrl()));
                        }
                        break;
                    }
            }
        }

        //private string PrepareRedirectToHostedPaymentPage()
        //{
        //    StringBuilder page = new StringBuilder();
        //    var subscription = Dao.Load<Subscription>(SubscriptionID);
        //    if (subscription != null)
        //    {
        //        MySql.Data.MySqlClient.MySqlCommand q = new MySql.Data.MySqlClient.MySqlCommand("Select * From ProductCode where ProductCode=@ProductCode");
        //        q.Parameters.AddWithValue("@ProductCode", subscription.ProductCode);
        //        var productCode = Dao.Load<ProductCode>(q).FirstOrDefault();

        //        page.Append("<html>");
        //        page.Append(@"<body onload='document.forms[""form""].submit()'>");
        //        page.AppendFormat("<form name='form' action='{0}' method='post'>", HostedPaymentPageURL);
        //        page.AppendFormat("<input type='hidden' name='Name' value='{0}'>", API_USERNAME);
        //        page.AppendFormat("<input type='hidden' name='Pass' value='{0}'>", API_PASSWORD);
        //        page.AppendFormat("<input type='hidden' name='PTID' value='{0}-{1}'>", productCode.ProductCodeID, subscription.Quantity);
        //        page.AppendFormat("<input type='hidden' name='PDes' value='{0}'>", subscription.ProductName);
        //        page.AppendFormat("<input type='hidden' name='TCost' value='{0}'>", TCOST);
        //        page.AppendFormat("<input type='hidden' name='CancelURL' value='http://{0}/{1}'>", Request.Url.Authority, GetUpsellUrl());
        //        page.AppendFormat("<input type='hidden' name='ConfirmURL' value='http://{0}/{1}'>", Request.Url.Authority, GetUpsellUrl());
        //        page.AppendFormat("<input type='hidden' name='SCost' value='{0}'>", subscription.InitialBillAmount + subscription.InitialShipping);
        //        page.AppendFormat("<input type='hidden' name='SubscrID' value='{0}'>", SubscriptionID);
        //        page.Append("</form>");
        //        page.Append("</body>");
        //        page.Append("</html>");
        //    }

        //    return page.ToString();
        //}
    }
}
