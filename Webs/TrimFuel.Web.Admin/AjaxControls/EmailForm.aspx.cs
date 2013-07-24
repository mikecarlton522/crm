using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Gateways.DefaultEmail;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class EmailForm : System.Web.UI.Page
    {
        private int? DynamicEmailID
        {
            get { return Utility.TryGetInt(hdnDynamicEmailID.Value) ?? Utility.TryGetInt(Request["dynamicEmailID"]); }
        }
        protected string Body;

        protected DynamicEmail DynamicEmail { get; set; }
        protected DynamicEmailType DynamicEmailType { get; set; }
        protected Product Product { get; set; }
        protected Billing Billing { get; set; }
        protected int? BillingID
        {
            get { return Utility.TryGetInt(hdnBillingID.Value) ?? Utility.TryGetInt(Request["billingID"]); }
        }
        protected string CancelCode { get { return Request["cancelCode"]; } }
        protected string RMA { get { return Request["RMA"]; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitProps();
                if (DynamicEmail == null)
                    return;
                ReplaceVariables();
                DataBind();
            }
        }

        private void InitProps()
        {
            BaseService serv = new BaseService();
            if (DynamicEmailID != null)
            {
                DynamicEmail = serv.Load<DynamicEmail>(DynamicEmailID);
                DynamicEmailType = serv.Load<DynamicEmailType>(DynamicEmail.DynamicEmailTypeID);
                Product = serv.Load<Product>(DynamicEmail.ProductID);
                Billing = serv.Load<Billing>(BillingID);
            }

            if (DynamicEmail != null)
                Body = DynamicEmail.Content;
        }

        private void ReplaceVariables()
        {
            if (Billing != null)
            {
                Body = Body.Replace("##FNAME##", Billing.FirstName);
                Body = Body.Replace("##LNAME##", Billing.LastName);
                Body = Body.Replace("##ADD1##", Billing.Address1);
                Body = Body.Replace("##ADD2##", Billing.Address2);
                Body = Body.Replace("##CITY##", Billing.City);
                Body = Body.Replace("##STATE##", Billing.State);
                Body = Body.Replace("##ZIP##", Billing.Zip);
                Body = Body.Replace("##PHONE##", Billing.Phone);
                Body = Body.Replace("##EMAIL##", Billing.Email);
                Body = Body.Replace("##RMA##", RMA);

                //string creditCardType = null;
                //string creditCard = Billing.CreditCardCnt.DecryptedCreditCard;
                //if (creditCard.StartsWith("4"))
                //{
                //    creditCardType = "Visa";
                //}
                //else if (creditCard.StartsWith("5"))
                //{
                //    creditCardType = "MasterCard";
                //}
                //else
                //{
                //    creditCardType = "card";
                //}
                //creditCard = creditCard.Substring(creditCard.Length - 4, 4);
                //Body = Body.Replace("##CARDTYPE##", creditCardType);
                //Body = Body.Replace("##LAST4##", creditCard);
                Body = Body.Replace("##BILLINGID##", Billing.BillingID.Value.ToString());
                Body = Body.Replace("##CANCELCODE##", CancelCode);
                Body = Body.Replace("##TRACKING_NUMBER##", "");

                string REACTIVATION_LINK_TEMPLATE = "http://www.ecigsbrandoffer.com/reactivate/default.asp?b={0}";
                Body = Body.Replace("##REACTIVATION_LINK##", string.Format(REACTIVATION_LINK_TEMPLATE, Billing.BillingID));
            }
        }

        protected void bCancel_Click(object sender, EventArgs e)
        {
        }

        protected void bSendEmail_Click(object sender, EventArgs e)
        {
            InitProps();
            try
            {
                IEmailGateway emailGateway = new DefaultEmailGateway();

                string email = Utility.TryGetStr(Billing.Email) ?? string.Empty;
                string fromName = Utility.TryGetStr(tbFromName.Text) ?? string.Empty;
                string fromAddress = Utility.TryGetStr(tbFromAddress.Text) ?? string.Empty;
                string subject = Utility.TryGetStr(tbSubject.Text) ?? string.Empty;
                string body = Utility.TryGetStr(tbBody.Text) ?? string.Empty;

                var emailToSave = new TrimFuel.Model.Email();
                emailToSave.DynamicEmailID = DynamicEmailID;
                emailToSave.Email_ = email;
                emailToSave.CreateDT = DateTime.Now;
                new EmailService().Save<Email>(emailToSave);

                var emailResult = emailGateway.SendEmail(fromName, fromAddress, "", email, subject, body);

                emailToSave.Subject = emailResult.ReturnValue.ResponseParams.GetParam("subject");
                emailToSave.Body = emailResult.ReturnValue.Request;
                emailToSave.Response = emailResult.ReturnValue.Response;
                emailToSave.BillingID = Billing.BillingID;
                new EmailService().Save<Email>(emailToSave);

                phMessage2.Visible = true;
                bSendEmail.Visible = false;
            }
            catch (Exception ex)
            {
                phMessage3.Visible = true;
                lSendEmailError.Text = ex.ToString();
            }

            //DataBind();
        }
    }
}