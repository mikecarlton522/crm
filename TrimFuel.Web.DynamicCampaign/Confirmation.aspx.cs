using System;
using System.Collections.Specialized;
using System.Linq;

using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Web.DynamicCampaign.Logic;

using MySql.Data.MySqlClient;

namespace TrimFuel.Web.DynamicCampaign
{
    public partial class Confirmation : BaseDynamicPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.PageTypeID = PageTypeEnum.Confirmation;

            base.OnInit(e);

            Master.Head = base.CampaignPage.Header;

            Master.Title = base.CampaignPage.Title;

            Master.Pixel = this.GetPixels();
        }

        protected override void DoAction(PageActions action)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            html.Text = base.HTML;
        }

        protected override void FillDefaultPageValues()
        {
            base.FillDefaultPageValues();

            Registration registration = ShoppingCart.Registration;
            if (registration == null)
                registration = new Registration();

            Billing billing = ShoppingCart.Billing;
            BillingExternalInfo existedBilInfo = null;
            if (billing != null)
            {
                new EmailService().ProcessEmailQueue(billing.BillingID);
                existedBilInfo = Dao.Load<BillingExternalInfo>(billing.BillingID ?? 0);
            }

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
    }
}
