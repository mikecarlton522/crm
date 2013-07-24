using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Web.DynamicCampaign.Logic;
using MySql.Data.MySqlClient;

namespace TrimFuel.Web.DynamicCampaign
{
    public partial class Upsell : BaseDynamicPage
    {
        private BillingSubscription _billingSubscription;
        
        private int _step = -1;

        protected override void OnInit(EventArgs e)
        {
            _billingSubscription = ShoppingCart.BillingSubscription;

            if (_step == -1)
                _step = int.Parse(Request["Step"] ?? "0");

            if (_step == 0)
                Stop("Upsell step not specified");

            else if (_step == 1)
                base.PageTypeID = PageTypeEnum.Upsell_1;

            else if (_step == 2)
                base.PageTypeID = PageTypeEnum.Upsell_2;

            else if (_step == 3)
                base.PageTypeID = PageTypeEnum.Upsell_3;

            base.OnInit(e);

            Master.Head = base.CampaignPage.Header;

            Master.Title = base.CampaignPage.Title;

            Master.Pixel = this.GetPixels();            
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            html.Text = base.HTML;
        }

        protected override NameValueCollection ObtainPageValues()
        {
            var val = base.ObtainPageValues();
            
            return val;
        }

        protected override void DoAction(PageActions action)
        {
            switch (action)
            {
                case PageActions.Upsell:
                    var up = (new CampaignService()).GetCampaignUpsell(CampaignPage.CampaignPageID);
                    var billing = ShoppingCart.Billing;
                    var subscription = Dao.Load<Subscription>(Campaign.SubscriptionID);
                    var saleService = new SaleService();
                    var upType = saleService.GetOrCreateUpsell(subscription.ProductID, up.ProductCode, (short)up.Quantity, up.Price);
                    if (up != null && billing != null && subscription != null && upType != null)
                    {
                        var res = saleService.CreateUpsellSale(billing, upType, 1,
                            null, null, null, null, null, null, false);
                    }
                    GoToUpsell();
                    break;
                case PageActions.Confirmation:
                    GoToUpsell();
                    break;
            }
        }

        protected override void FillDefaultPageValues()
        {
            base.FillDefaultPageValues();

            Registration registration = ShoppingCart.Registration;
            if (registration == null)
                registration = new Registration();

            Billing billing = ShoppingCart.Billing;
            BillingExternalInfo existedBilInfo = null;

            if (existedBilInfo == null)
                existedBilInfo = new BillingExternalInfo();

            BillingSubscription bs = ShoppingCart.BillingSubscription;
            if (bs == null)
            {
                if (string.IsNullOrEmpty(Campaign.RedirectURL) || string.IsNullOrEmpty(Request["BillingID"]))
                    bs = new BillingSubscription();
                else
                    bs = Dao.Load<BillingSubscription>(
                    new MySql.Data.MySqlClient.MySqlCommand(
                        string.Format("SELECT * FROM BillingSubscription Where BillingID={0} and SubscriptionID={1}", Request["BillingID"], SubscriptionID)))
                    .SingleOrDefault();
            }

            if (bs == null)
                bs = new BillingSubscription();

            AssertigyMID am = ShoppingCart.AssertigyMID;
            if (am == null)
            {
                if (bs.BillingSubscriptionID != null)
                    am = Dao.Load<AssertigyMID>(
                    new MySql.Data.MySqlClient.MySqlCommand(
                        string.Format("SELECT * FROM AssertigyMID Where AssertigyMIDID = (Select MerchantAccountID from ChargeHistoryEx where BillingSubscriptionID={0} order by ChargeDate desc limit 1)", bs.BillingSubscriptionID)))
                    .SingleOrDefault();
            }

            var res = new NameValueCollection();

            RegistrationInfo existedRegInfo = null;
            if (registration.RegistrationID != null)
                existedRegInfo = Dao.Load<RegistrationInfo>(
                    new MySql.Data.MySqlClient.MySqlCommand(
                        "SELECT * FROM RegistrationInfo Where RegistrationID=" + registration.RegistrationID.ToString()))
                    .SingleOrDefault();

            if (existedRegInfo == null)
                existedRegInfo = new RegistrationInfo();

            SetValueWithDefault(ControlNames.Confirmation_Address_1, registration.Address1);
            SetValueWithDefault(ControlNames.Confirmation_Address_2, registration.Address2);
            SetValueWithDefault(ControlNames.Confirmation_First_Name, registration.FirstName);
            SetValueWithDefault(ControlNames.Confirmation_Last_Name, registration.LastName);
            SetValueWithDefault(ControlNames.Confirmation_City, registration.City);
            SetValueWithDefault(ControlNames.Confirmation_Home_Tel, registration.Phone);
            SetValueWithDefault(ControlNames.Confirmation_State, registration.State);
            SetValueWithDefault(ControlNames.Confirmation_Zip, registration.Zip);
            SetValueWithDefault(ControlNames.Confirmation_Email, registration.Email);
            SetValueWithDefault(ControlNames.Confirmation_Country, existedRegInfo != null ? existedRegInfo.Country : US_COUNTRY);
            SetValueWithDefault(ControlNames.Confirmation_Order_ID, bs.CustomerReferenceNumber);
            SetValueWithDefault(ControlNames.Confirmation_Friendly_Name, (am != null ? am.DisplayName : "**TEST RECORD**"));

            SetValueWithDefault(ControlNames.Confirmation_Neighborhood, existedRegInfo.Neighborhood);
            SetValueWithDefault(ControlNames.Confirmation_Numero, existedRegInfo.CustomField1);
            SetValueWithDefault(ControlNames.Confirmation_Complemento, existedRegInfo.CustomField2);

            SetValueWithDefault(ControlNames.Confirmation_CPF, existedBilInfo.CustomField1);
            SetValueWithDefault(ControlNames.Confirmation_Card_Holder, existedBilInfo.CustomField4);

        }

        //protected override string GetPixels()
        //{
        //    string pixel = string.Empty;

        //    if (!string.IsNullOrEmpty(base.SubAffiliate) && !string.IsNullOrEmpty(base.Affiliate) && base.Affiliate.ToLower() == "mwresv" && base.SubAffiliate.ToLower() == "20527")
        //        pixel = @"<img src='http://www.infitrax.com/adLead.php?AFID=[AFID]&CID=99' width='1' height='1' border='0'>"; //TODO: review

        //    pixel = base.GetPixels(pixel);

        //    Subscription subscription = base.Dao.Load<Subscription>(_billingSubscription.SubscriptionID);

        //    pixel = pixel.Replace("##BillingID##", _billingSubscription.BillingID.ToString());

        //    pixel = pixel.Replace("##ClickID##", base.CID);

        //    if (pixel.Contains("##PRICE##") && subscription.InitialShipping != null)
        //        pixel = pixel.Replace("##PRICE##", (subscription.InitialShipping + subscription.InitialBillAmount).ToString());

        //    return pixel;
        //}    
    }
}
