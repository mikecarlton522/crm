using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Containers;
using TrimFuel.Business.Utils;
using TrimFuel.Business.Gateways;
using TrimFuel.Business.Gateways.DefaultEmail;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class EmailService : BaseService
    {
        //TODO: config
        private const string TEST_EMAIL = "dtcoder@gmail.com";

        private IEmailGateway emailGateway = new DefaultEmailGateway();

        private SubscriptionService subscriptionService { get { return new SubscriptionService(); } }
        private RegistrationService registrationService { get { return new RegistrationService(); } }
        private RefererService refererService { get { return new RefererService(); } }
        private SaleService saleService { get { return new SaleService(); } }

        public Email SendEmail(DynamicEmail dynamicEmail, Billing billing, decimal? shippingAmount, decimal? productAmount, string merchantName, string billingCancelCode, string refererCode, DateTime createDT)
        {
            if (billing.Email == "email@email.me" || IsBlockedEmail(billing.Email))
            {
                return null;
            }
            
            Email email = null;
            try
            {
                dao.BeginTransaction();

                if (billing.Email == null || billing.Email.Length <= 1)
                {
                    throw new Exception(string.Format("Invalid email in Billing({0}).", billing.BillingID));
                }

                string ownRefererCode = null;
                string refererPassword = null;
                if (billing.BillingID != null)
                {
                    Referer ownReferer = refererService.GetOwnRefererByBilling(billing.BillingID.Value);
                    if (ownReferer != null)
                    {
                        ownRefererCode = ownReferer.RefererCode;
                        refererPassword = ownReferer.Password;
                    }
                }

                email = new Email();
                email.DynamicEmailID = dynamicEmail.DynamicEmailID;
                email.BillingID = billing.BillingID;
                email.Email_ = billing.Email;
                email.CreateDT = createDT;
                dao.Save<Email>(email);

                var product = dao.Load<Product>(dynamicEmail.ProductID);

                BusinessError<GatewayResult> emailResult = emailGateway.SendEmail(email.EmailID.Value, dynamicEmail, billing, shippingAmount, productAmount, merchantName, billingCancelCode, ownRefererCode, refererCode, refererPassword, createDT, product);

                email.Subject = emailResult.ReturnValue.ResponseParams.GetParam("subject");
                email.Body = emailResult.ReturnValue.Request;
                email.Response = emailResult.ReturnValue.Response;
                dao.Save<Email>(email);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                email = null;
            }
            return email;
        }

        private string NullToEmpty(string val)
        {
            if (val == null)
            {
                return string.Empty;
            }
            return val;
        }

        public void SendTestEmail(DynamicEmail dynamicEmail, string targetEmail)
        {
            //Billing billing = new Billing()
            //{
            //    BillingID = 0,
            //    Address1 = 
            //};
            //BusinessError<GatewayResult> emailResult = emailGateway.SendEmail(0, dynamicEmail, billing, shippingAmount, productAmount, merchantName, billingCancelCode, ownRefererCode, refererCode);
        }

        #region Confirmation Emails

        //public Email SendConfirmationTrialEmail(InvoiceView2 invoice)
        //{
        //    Email email = null;
        //    try
        //    {
        //        var dynamicEmail = GetActiveDynamicEmails(invoice.Invoice.Order.Order.ProductID.Value, null, DynamicEmailTypeEnum.OrderConfirmation).FirstOrDefault();
        //        if (dynamicEmail != null)
        //        {
        //            string body = dynamicEmail.Content;
        //            body = body.Replace("##FNAME##", NullToEmpty(invoice.Invoice.Order.Billing.FirstName));
        //            body = body.Replace("##LNAME##", NullToEmpty(invoice.Invoice.Order.Billing.LastName));
        //            body = body.Replace("##ADD1##", NullToEmpty(invoice.Invoice.Order.Billing.Address1));
        //            body = body.Replace("##ADD2##", NullToEmpty(invoice.Invoice.Order.Billing.Address2));
        //            body = body.Replace("##CITY##", NullToEmpty(invoice.Invoice.Order.Billing.City));
        //            body = body.Replace("##STATE##", NullToEmpty(invoice.Invoice.Order.Billing.State));
        //            body = body.Replace("##ZIP##", NullToEmpty(invoice.Invoice.Order.Billing.Zip));
        //            body = body.Replace("##PHONE##", NullToEmpty(invoice.Invoice.Order.Billing.Phone));
        //            body = body.Replace("##EMAIL##", NullToEmpty(invoice.Invoice.Order.Billing.Email));

        //            var registration = dao.Load<Registration>(invoice.Invoice.Order.Billing.BillingID);

        //            if (registration != null)
        //            {
        //                body = body.Replace("##SHIPPING_FNAME##", NullToEmpty(registration.FirstName));
        //                body = body.Replace("##SHIPPING_LNAME##", NullToEmpty(registration.LastName));
        //                body = body.Replace("##SHIPPING_ADD1##", NullToEmpty(registration.Address1));
        //                body = body.Replace("##SHIPPING_ADD2##", NullToEmpty(registration.Address2));
        //                body = body.Replace("##SHIPPING_CITY##", NullToEmpty(registration.City));
        //                body = body.Replace("##SHIPPING_STATE##", NullToEmpty(registration.State));
        //                body = body.Replace("##SHIPPING_ZIP##", NullToEmpty(registration.Zip));
        //                body = body.Replace("##SHIPPING_PHONE##", NullToEmpty(registration.Phone));
        //                body = body.Replace("##SHIPPING_EMAIL##", NullToEmpty(registration.Email));
        //            }
        //            else
        //            {
        //                body = body.Replace("##SHIPPING_FNAME##", string.Empty);
        //                body = body.Replace("##SHIPPING_LNAME##", string.Empty);
        //                body = body.Replace("##SHIPPING_ADD1##", string.Empty);
        //                body = body.Replace("##SHIPPING_ADD2##", string.Empty);
        //                body = body.Replace("##SHIPPING_CITY##", string.Empty);
        //                body = body.Replace("##SHIPPING_STATE##", string.Empty);
        //                body = body.Replace("##SHIPPING_ZIP##", string.Empty);
        //                body = body.Replace("##SHIPPING_PHONE##", string.Empty);
        //                body = body.Replace("##SHIPPING_EMAIL##", string.Empty);
        //            }

        //            string refererPassword = null;
        //            Referer referer = refererService.GetOwnRefererByBilling(invoice.Invoice.Order.Billing.BillingID.Value);
        //            if (referer != null)
        //            {
        //                refererPassword = referer.Password;
        //            }
        //            body = body.Replace("##PASSWORD##", NullToEmpty(refererPassword));

        //            string creditCardType = null;
        //            string creditCard = invoice.Invoice.Order.Billing.CreditCardCnt.DecryptedCreditCard;
        //            if (creditCard.StartsWith("4"))
        //            {
        //                creditCardType = "Visa";
        //            }
        //            else if (creditCard.StartsWith("5"))
        //            {
        //                creditCardType = "MasterCard";
        //            }
        //            else
        //            {
        //                creditCardType = "card";
        //            }
        //            creditCard = creditCard.Substring(creditCard.Length - 4, 4);

        //            body = body.Replace("##CARDTYPE##", NullToEmpty(creditCardType));
        //            body = body.Replace("##LAST4##", NullToEmpty(creditCard));

        //            body = body.Replace("##BILLINGID##", invoice.Invoice.Order.Billing.BillingID.Value.ToString());

        //            Referer saleReferer = null;
        //            if (invoice.Invoice.Order.Order.RefererID != null)
        //            {
        //                saleReferer = Load<Referer>(invoice.Invoice.Order.Order.RefererID);
        //            }


        //            body = body.Replace("##MID##", invoice.ChargeResult.MIDName);
        //            body = body.Replace("##SH_AMOUNT##", Utility.FormatPrice(invoice.Invoice.Invoice.Amount));
        //            body = body.Replace("##PRODUCT_AMOUNT##", Utility.FormatPrice(0M));
        //            body = body.Replace("##TOTAL_AMOUNT##", Utility.FormatPrice(invoice.Invoice.Invoice.Amount));
        //            body = body.Replace("##REFERER_CODE##", (saleReferer != null ? NullToEmpty(saleReferer.RefererCode) : string.Empty));
        //            body = body.Replace("##DESCRIPTION##", invoice.Invoice.SaleList.
        //                Where(i => i.OrderSale.SaleStatus == SaleStatusEnum.Approved && i.OrderSale.SaleType == OrderSaleTypeEnum.Trial).
        //                FirstOrDefault().
        //                OrderSale.SaleName);

        //            BillingExternalInfo billingInfo = dao.Load<BillingExternalInfo>(invoice.Invoice.Order.Billing.BillingID);
        //            body = body.Replace("##INTERNAL_ID##", (billingInfo != null ? NullToEmpty(billingInfo.InternalID) : string.Empty));

        //            //Empty
        //            body = body.Replace("##ID##", string.Empty);
        //            body = body.Replace("##OWN_REFERER_CODE##", string.Empty);
        //            body = body.Replace("##CANCELCODE##", string.Empty);
        //            body = body.Replace("##REACTIVATION_LINK##", string.Empty);

        //            //new variables
        //            var product = dao.Load<Product>(invoice.Invoice.Order.Order.ProductID);
        //            if (product != null)
        //                body = body.Replace("##PRODUCT_NAME##", NullToEmpty(product.ProductName));
        //            body = body.Replace("##ORDER_DATE##", NullToEmpty(invoice.Invoice.Order.Order.CreateDT.Value.ToShortDateString()));
        //            //new variables

        //            email = SendEmail(dynamicEmail.DynamicEmailID.Value, dynamicEmail.FromName, dynamicEmail.FromAddress, invoice.Invoice.Order.Billing.FullName, invoice.Invoice.Order.Billing.Email, dynamicEmail.Subject, body, invoice.Invoice.Order.Billing.BillingID);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(ex);
        //    }
        //    return email;
        //}

        //public Email SendConfirmationEmail(int productID, Billing billing, string midDisplayName,
        //    decimal shippingAmount, decimal productAmount, string refererCode, string description, ChargeHistoryWithSalesView chargeView)
        //{
        //    if (billing == null)
        //    {
        //        return null;
        //    }
        //    if (billing.Email == "email@email.me")
        //    {
        //        return null;
        //    }
        //    bool isTestCase = saleService.IsTestCreditCard(billing.CreditCard);

        //    DynamicEmail dynamicEmail = null;
        //    Registration registration = null;
        //    try
        //    {
        //        MySqlCommand q = new MySqlCommand("select * from DynamicEmail " +
        //            "where ProductID = @productID and Active = 1 and DynamicEmailTypeID = 1");
        //        q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

        //        dynamicEmail = dao.Load<DynamicEmail>(q).FirstOrDefault();
        //        registration = dao.Load<Registration>(billing.BillingID);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(GetType(), ex);
        //        dynamicEmail = null;
        //    }

        //    Email email = null;
        //    if (dynamicEmail != null)
        //    {
        //        try
        //        {
        //            dao.BeginTransaction();

        //            string body = dynamicEmail.Content;
        //            body = body.Replace("##FNAME##", NullToEmpty(billing.FirstName));
        //            body = body.Replace("##LNAME##", NullToEmpty(billing.LastName));
        //            body = body.Replace("##ADD1##", NullToEmpty(billing.Address1));
        //            body = body.Replace("##ADD2##", NullToEmpty(billing.Address2));
        //            body = body.Replace("##CITY##", NullToEmpty(billing.City));
        //            body = body.Replace("##STATE##", NullToEmpty(billing.State));
        //            body = body.Replace("##ZIP##", NullToEmpty(billing.Zip));
        //            body = body.Replace("##PHONE##", NullToEmpty(billing.Phone));
        //            body = body.Replace("##EMAIL##", NullToEmpty(billing.Email));

        //            if (registration != null)
        //            {
        //                body = body.Replace("##SHIPPING_FNAME##", NullToEmpty(registration.FirstName));
        //                body = body.Replace("##SHIPPING_LNAME##", NullToEmpty(registration.LastName));
        //                body = body.Replace("##SHIPPING_ADD1##", NullToEmpty(registration.Address1));
        //                body = body.Replace("##SHIPPING_ADD2##", NullToEmpty(registration.Address2));
        //                body = body.Replace("##SHIPPING_CITY##", NullToEmpty(registration.City));
        //                body = body.Replace("##SHIPPING_STATE##", NullToEmpty(registration.State));
        //                body = body.Replace("##SHIPPING_ZIP##", NullToEmpty(registration.Zip));
        //                body = body.Replace("##SHIPPING_PHONE##", NullToEmpty(registration.Phone));
        //                body = body.Replace("##SHIPPING_EMAIL##", NullToEmpty(registration.Email));
        //            }
        //            else
        //            {
        //                body = body.Replace("##SHIPPING_FNAME##", string.Empty);
        //                body = body.Replace("##SHIPPING_LNAME##", string.Empty);
        //                body = body.Replace("##SHIPPING_ADD1##", string.Empty);
        //                body = body.Replace("##SHIPPING_ADD2##", string.Empty);
        //                body = body.Replace("##SHIPPING_CITY##", string.Empty);
        //                body = body.Replace("##SHIPPING_STATE##", string.Empty);
        //                body = body.Replace("##SHIPPING_ZIP##", string.Empty);
        //                body = body.Replace("##SHIPPING_PHONE##", string.Empty);
        //                body = body.Replace("##SHIPPING_EMAIL##", string.Empty);
        //            }

        //            string refererPassword = null;
        //            Referer referer = refererService.GetOwnRefererByBilling(billing.BillingID.Value);
        //            if (referer != null)
        //            {
        //                refererPassword = referer.Password;
        //            }
        //            body = body.Replace("##PASSWORD##", NullToEmpty(refererPassword));

        //            string creditCardType = null;
        //            string creditCard = billing.CreditCardCnt.DecryptedCreditCard;
        //            if (creditCard.StartsWith("4"))
        //            {
        //                creditCardType = "Visa";
        //            }
        //            else if (creditCard.StartsWith("5"))
        //            {
        //                creditCardType = "MasterCard";
        //            }
        //            else
        //            {
        //                creditCardType = "card";
        //            }
        //            creditCard = creditCard.Substring(creditCard.Length - 4, 4);

        //            body = body.Replace("##CARDTYPE##", NullToEmpty(creditCardType));
        //            body = body.Replace("##LAST4##", NullToEmpty(creditCard));

        //            body = body.Replace("##BILLINGID##", billing.BillingID.Value.ToString());
        //            if (!isTestCase)
        //            {
        //                body = body.Replace("##MID##", midDisplayName);
        //                body = body.Replace("##SH_AMOUNT##", Utility.FormatPrice(shippingAmount));
        //                body = body.Replace("##PRODUCT_AMOUNT##", Utility.FormatPrice(productAmount));
        //                body = body.Replace("##TOTAL_AMOUNT##", Utility.FormatPrice(productAmount + shippingAmount));
        //                body = body.Replace("##REFERER_CODE##", refererCode);
        //                body = body.Replace("##DESCRIPTION##", description);
        //            }
        //            else
        //            {
        //                body = body.Replace("##MID##", "Test MID");
        //                body = body.Replace("##SH_AMOUNT##", Utility.FormatPrice(10M));
        //                body = body.Replace("##PRODUCT_AMOUNT##", Utility.FormatPrice(50M));
        //                body = body.Replace("##TOTAL_AMOUNT##", Utility.FormatPrice(60M));
        //                body = body.Replace("##REFERER_CODE##", string.Empty);
        //                body = body.Replace("##DESCRIPTION##", "Test case description");
        //            }

        //            BillingExternalInfo billingInfo = dao.Load<BillingExternalInfo>(billing.BillingID);
        //            body = body.Replace("##INTERNAL_ID##", (billingInfo != null ? NullToEmpty(billingInfo.InternalID) : string.Empty));

        //            //Empty
        //            body = body.Replace("##ID##", string.Empty);
        //            body = body.Replace("##OWN_REFERER_CODE##", string.Empty);
        //            body = body.Replace("##CANCELCODE##", string.Empty);
        //            body = body.Replace("##REACTIVATION_LINK##", string.Empty);

        //            //new variables
        //            var product = dao.Load<Product>(productID);
        //            if (product != null)
        //                body = body.Replace("##PRODUCT_NAME##", NullToEmpty(product.ProductName));
        //            else
        //                body = body.Replace("##PRODUCT_NAME##", string.Empty);

        //            if (chargeView.ChargeHistoryView != null && chargeView.ChargeHistoryView.Value1 != null && chargeView.ChargeHistoryView.Value1.ChargeDate != null)
        //                body = body.Replace("##ORDER_DATE##", chargeView.ChargeHistoryView.Value1.ChargeDate.Value.ToShortDateString());
        //            else
        //                body = body.Replace("##ORDER_DATE##", string.Empty);
        //            //new variables

        //            email = SendEmail(dynamicEmail.DynamicEmailID.Value, dynamicEmail.FromName, dynamicEmail.FromAddress, billing.FullName, billing.Email, dynamicEmail.Subject, body, billing.BillingID);

        //            dao.CommitTransaction();
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error(GetType(), ex);
        //            dao.RollbackTransaction();
        //            email = null;
        //        }
        //    }
        //    return email;
        //}

        //public Email SendConfirmationEmail(Billing billing, Subscription subscription, string merchantName, Referer referer, DateTime createDT)
        //{
        //    if (billing.Email == "email@email.me")
        //    {
        //        return null;
        //    }

        //    Email email = null;
        //    try
        //    {
        //        dao.BeginTransaction();

        //        MySqlCommand q = new MySqlCommand("select * from DynamicEmail " +
        //            "where ProductID = @productID and Active = 1 and DynamicEmailTypeID = 1");
        //        q.Parameters.Add("@productID", MySqlDbType.Int32).Value = subscription.ProductID;

        //        DynamicEmail dynamicEmail = dao.Load<DynamicEmail>(q).FirstOrDefault();
        //        if (dynamicEmail == null)
        //        {
        //            throw new Exception(string.Format("Can't find DynamicEmail for Product({0}).", subscription.ProductID));
        //        }

        //        string refererCode = null;
        //        if (referer != null)
        //        {
        //            refererCode = referer.RefererCode;
        //        }

        //        email = SendEmail(dynamicEmail, billing, subscription.InitialShipping, subscription.InitialBillAmount, merchantName, null, refererCode, createDT);

        //        dao.CommitTransaction();
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(GetType(), ex);
        //        dao.RollbackTransaction();
        //        email = null;
        //    }
        //    return email;
        //}

        public string ReplaceMainVarsConfirmationEmail(DynamicEmail dynamicEmail, Registration registration, Billing billing, Sale sale, bool isTestCase, int? productID)
        {
            string body = string.Empty;
            if (dynamicEmail != null)
            {
                body = dynamicEmail.Content;
                body = body.Replace("##FNAME##", NullToEmpty(billing.FirstName));
                body = body.Replace("##LNAME##", NullToEmpty(billing.LastName));
                body = body.Replace("##ADD1##", NullToEmpty(billing.Address1));
                body = body.Replace("##ADD2##", NullToEmpty(billing.Address2));
                body = body.Replace("##CITY##", NullToEmpty(billing.City));
                body = body.Replace("##STATE##", NullToEmpty(billing.State));
                body = body.Replace("##ZIP##", NullToEmpty(billing.Zip));
                body = body.Replace("##PHONE##", NullToEmpty(billing.Phone));
                body = body.Replace("##EMAIL##", NullToEmpty(billing.Email));

                if (registration != null)
                {
                    body = body.Replace("##SHIPPING_FNAME##", NullToEmpty(registration.FirstName));
                    body = body.Replace("##SHIPPING_LNAME##", NullToEmpty(registration.LastName));
                    body = body.Replace("##SHIPPING_ADD1##", NullToEmpty(registration.Address1));
                    body = body.Replace("##SHIPPING_ADD2##", NullToEmpty(registration.Address2));
                    body = body.Replace("##SHIPPING_CITY##", NullToEmpty(registration.City));
                    body = body.Replace("##SHIPPING_STATE##", NullToEmpty(registration.State));
                    body = body.Replace("##SHIPPING_ZIP##", NullToEmpty(registration.Zip));
                    body = body.Replace("##SHIPPING_PHONE##", NullToEmpty(registration.Phone));
                    body = body.Replace("##SHIPPING_EMAIL##", NullToEmpty(registration.Email));
                }
                else
                {
                    body = body.Replace("##SHIPPING_FNAME##", string.Empty);
                    body = body.Replace("##SHIPPING_LNAME##", string.Empty);
                    body = body.Replace("##SHIPPING_ADD1##", string.Empty);
                    body = body.Replace("##SHIPPING_ADD2##", string.Empty);
                    body = body.Replace("##SHIPPING_CITY##", string.Empty);
                    body = body.Replace("##SHIPPING_STATE##", string.Empty);
                    body = body.Replace("##SHIPPING_ZIP##", string.Empty);
                    body = body.Replace("##SHIPPING_PHONE##", string.Empty);
                    body = body.Replace("##SHIPPING_EMAIL##", string.Empty);
                }

                string refererPassword = null;
                Referer referer = refererService.GetOwnRefererByBilling(billing.BillingID.Value);
                if (referer != null)
                {
                    refererPassword = referer.Password;
                }
                body = body.Replace("##PASSWORD##", NullToEmpty(refererPassword));

                string creditCardType = null;
                string creditCard = billing.CreditCardCnt.DecryptedCreditCard;
                if (creditCard.StartsWith("4"))
                {
                    creditCardType = "Visa";
                }
                else if (creditCard.StartsWith("5"))
                {
                    creditCardType = "MasterCard";
                }
                else
                {
                    creditCardType = "card";
                }
                creditCard = creditCard.Substring(creditCard.Length - 4, 4);

                body = body.Replace("##CARDTYPE##", NullToEmpty(creditCardType));
                body = body.Replace("##LAST4##", NullToEmpty(creditCard));

                body = body.Replace("##BILLINGID##", billing.BillingID.Value.ToString());
                if (!isTestCase)
                {
                    SaleFullInfo inf = saleService.GetSaleInfo(sale);

                    body = body.Replace("##MID##", (inf != null && inf.ChargeMID != null ? NullToEmpty(inf.ChargeMID.DisplayName) : string.Empty));
                    body = body.Replace("##SH_AMOUNT##", (inf != null ? Utility.FormatPrice(inf.ShippingAmount) : string.Empty));
                    body = body.Replace("##PRODUCT_AMOUNT##", (inf != null ? Utility.FormatPrice(inf.ProductAmount) : string.Empty));
                    body = body.Replace("##REFERER_CODE##", (inf != null && inf.Referer != null ? NullToEmpty(inf.Referer.RefererCode) : string.Empty));
                    body = body.Replace("##DESCRIPTION##", (inf != null && inf.Details != null ? NullToEmpty(inf.Details.Description) : string.Empty));
                }
                else
                {
                    body = body.Replace("##MID##", "Test MID");
                    body = body.Replace("##SH_AMOUNT##", Utility.FormatPrice(10M));
                    body = body.Replace("##PRODUCT_AMOUNT##", Utility.FormatPrice(50M));
                    body = body.Replace("##REFERER_CODE##", string.Empty);
                    body = body.Replace("##DESCRIPTION##", "Test case description");
                }

                BillingExternalInfo billingInfo = dao.Load<BillingExternalInfo>(billing.BillingID);
                body = body.Replace("##INTERNAL_ID##", (billingInfo != null ? NullToEmpty(billingInfo.InternalID) : string.Empty));

                //Empty
                body = body.Replace("##ID##", string.Empty);
                body = body.Replace("##OWN_REFERER_CODE##", string.Empty);
                body = body.Replace("##CANCELCODE##", string.Empty);
                body = body.Replace("##REACTIVATION_LINK##", string.Empty);

                //new variables
                var product = dao.Load<Product>(productID);
                if (product != null)
                    body = body.Replace("##PRODUCT_NAME##", NullToEmpty(product.ProductName));
                else
                    body = body.Replace("##PRODUCT_NAME##", string.Empty);
                if (sale != null && sale.CreateDT != null)
                    body = body.Replace("##ORDER_DATE##", NullToEmpty(sale.CreateDT.Value.ToShortDateString()));
                else
                    body = body.Replace("##ORDER_DATE##", string.Empty);
                //new variables

                //new variables for 2Box
                RegistrationInfo regInfo = registrationService.GetRegistrationInfo(registration.RegistrationID.Value);
                body = body.Replace("##CPF##", (billingInfo != null ? NullToEmpty(billingInfo.CustomField1) : string.Empty));
                body = body.Replace("##BAIRRO##", (regInfo != null ? NullToEmpty(regInfo.Neighborhood) : string.Empty));

                body = body.Replace("##CARD_HOLDER##", (billingInfo != null ? NullToEmpty(billingInfo.CustomField4) : string.Empty));
                body = body.Replace("##SHIPPING_NUMERO##", (regInfo != null ? NullToEmpty(regInfo.CustomField1) : string.Empty));
                body = body.Replace("##SHIPPING_COMPLEMENTO##", (regInfo != null ? NullToEmpty(regInfo.CustomField2) : string.Empty));
                //new variables for 2Box
            }
            return body;
        }

        public EmailQueue PushConfirmationEmailToQueue(long? billingID, long? saleID)
        {
            EmailQueue emailQueue = null;
            try
            {
                emailQueue = new EmailQueue()
                {
                    SaleID = saleID,
                    BillingID = billingID,
                    CreateDT = DateTime.Now,
                    Completed = false
                };
                dao.Save<EmailQueue>(emailQueue);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                emailQueue = null;
            }
            return emailQueue;
        }

        /// <summary>
        /// Process EmailQueue by BillingID
        /// </summary>
        /// <param name="billingID"></param>
        public void ProcessEmailQueue(long? billingID)
        {
            try
            {
                // if Billing exists and need to send email
                var billing = dao.Load<Billing>(billingID);
                if (billing == null)
                    return;
                if (billing.Email == "email@email.me")
                    return;
                bool isTestCase = saleService.IsTestCreditCard(billing.CreditCard);

                //find all not complete records with this billingID
                MySqlCommand q = new MySqlCommand("Select * From EmailQueue where Completed <> true and BillingID=@BillingID order by EmailQueueID");
                q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                var emailQueueList = dao.Load<EmailQueue>(q);
                if (emailQueueList == null || emailQueueList.Count == 0)
                    return;

                string emailBody = string.Empty;
                StringBuilder reviewTable = new StringBuilder("<table border='1' cellpadding='5'><tr><td>Product Name</td><td>Description</td><td>MID</td><td>Order Date</td><td>Subtotal</td><td>Discount</td><td>Total</td></tr>");
                StringBuilder reviewTableFrench = new StringBuilder("<table border='1' cellpadding='5'><tr><td>Produit</td><td>Description</td><td>MID</td><td>Date de la commande</td><td>Total partiel</td><td>Rabais</td><td>Total</td></tr>");
                string reviewTableTr = "<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td><td>{4}</td><td>{5}</td><td>{6}</td></tr>";
                decimal? totalAmount = 0;
                DynamicEmail dynamicEmail = null;
                try
                {
                    foreach (var item in emailQueueList)
                    {
                        if (isTestCase)
                        {
                            //test order
                            q = new MySqlCommand("select s.* from BillingSubscription bs inner join Subscription s on s.SubscriptionID=bs.SubscriptionID " +
                                "where bs.BillingID = @BillingID order by bs.BillingSubscriptionID desc limit 1");
                            q.Parameters.Add("@BillingID", MySqlDbType.Int64).Value = billingID;
                            var subscription = dao.Load<Subscription>(q).SingleOrDefault();
                            var registration = dao.Load<Registration>(billing.RegistrationID);

                            if (subscription != null)
                            {
                                dynamicEmail = GetConfirmationDynamicEmail(subscription.ProductID, billing.CampaignID);
                                emailBody = ReplaceMainVarsConfirmationEmail(dynamicEmail, registration, billing, null, isTestCase, subscription.ProductID);
                            }
                        }
                        else
                        {
                            Sale sale = dao.Load<Sale>(item.SaleID);
                            if (sale == null && !isTestCase)
                                continue;

                            Registration registration = null;
                            Product product = null;
                            PromoGiftSale giftSale = null;
                            PromoGift promoGift = null;
                            var productID = GetProductIdBySaleID(item.SaleID);

                            product = dao.Load<Product>(productID);
                            dynamicEmail = GetConfirmationDynamicEmail(productID, billing.CampaignID);
                            registration = dao.Load<Registration>(billing.RegistrationID);

                            q = new MySqlCommand("select * from PromoGiftSale where SaleID = @SaleID");
                            q.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = sale.SaleID;
                            giftSale = dao.Load<PromoGiftSale>(q).FirstOrDefault();

                            if (giftSale != null)
                            {
                                q = new MySqlCommand("select * from PromoGift where PromoGiftID = @PromoGiftID");
                                q.Parameters.Add("@PromoGiftID", MySqlDbType.Int64).Value = giftSale.PromoGiftID;
                                promoGift = dao.Load<PromoGift>(q).FirstOrDefault();
                            }

                            if (string.IsNullOrEmpty(emailBody))
                                emailBody = ReplaceMainVarsConfirmationEmail(dynamicEmail, registration, billing, sale, isTestCase, productID);

                            SaleFullInfo inf = saleService.GetSaleInfo(sale);
                            totalAmount += inf != null ? inf.TotalAmount : 0;

                            //add new tr to REVIEW_ORDER_TABLE
                            reviewTable.Append(string.Format(reviewTableTr,
                                product != null ? product.ProductName : string.Empty,
                                    inf != null && inf.Details != null ? NullToEmpty(inf.Details.Description) : string.Empty,
                                    inf != null && inf.ChargeMID != null ? NullToEmpty(inf.ChargeMID.DisplayName) : string.Empty,
                                    sale.CreateDT != null ? sale.CreateDT.Value.ToShortDateString() : string.Empty,
                                    giftSale != null ? Utility.FormatPrice((inf != null ? inf.TotalAmount : 0) - giftSale.RedeemAmount) : Utility.FormatPrice(inf != null ? inf.TotalAmount : 0),
                                    giftSale != null ? promoGift.GiftNumber : "0",
                                    Utility.FormatPrice(inf != null ? inf.TotalAmount : 0)));
                            //add new tr to REVIEW_ORDER_TABLE_FRENCH
                            reviewTableFrench.Append(string.Format(reviewTableTr,
                                product != null ? product.ProductName : string.Empty,
                                    inf != null && inf.Details != null ? NullToEmpty(inf.Details.Description) : string.Empty,
                                    inf != null && inf.ChargeMID != null ? NullToEmpty(inf.ChargeMID.DisplayName) : string.Empty,
                                    sale.CreateDT != null ? sale.CreateDT.Value.ToShortDateString() : string.Empty,
                                    giftSale != null ? Utility.FormatPrice((inf != null ? inf.TotalAmount : 0) - giftSale.RedeemAmount) : Utility.FormatPrice(inf != null ? inf.TotalAmount : 0),
                                    giftSale != null ? promoGift.GiftNumber : "0",
                                    Utility.FormatPrice(inf != null ? inf.TotalAmount : 0)));
                        }
                    }
                    reviewTable.Append("</table>");
                    reviewTableFrench.Append("</table>");
                    if (!isTestCase)
                    {
                        emailBody = emailBody.Replace("##ORDER_REVIEW_TABLE_FRENCH##", reviewTableFrench.ToString());
                        emailBody = emailBody.Replace("##ORDER_REVIEW_TABLE##", reviewTable.ToString());
                        emailBody = emailBody.Replace("##TOTAL_AMOUNT##", Utility.FormatPrice(totalAmount));
                    }
                    else
                    {
                        emailBody = emailBody.Replace("##ORDER_REVIEW_TABLE_FRENCH##", string.Empty);
                        emailBody = emailBody.Replace("##ORDER_REVIEW_TABLE##", string.Empty);
                        emailBody = emailBody.Replace("##TOTAL_AMOUNT##", Utility.FormatPrice(60M));
                    }

                    if (!IsBlockedEmail(billing.Email) && dynamicEmail != null)
                    {
                        //save email to DB
                        var email = new Email();
                        email.DynamicEmailID = dynamicEmail.DynamicEmailID;
                        email.BillingID = billing.BillingID;
                        email.Email_ = billing.Email;
                        email.CreateDT = DateTime.Now;
                        email.Body = emailBody;
                        email.Subject = dynamicEmail.Subject;
                        dao.Save<Email>(email);
                        BusinessError<GatewayResult> emailResult = emailGateway.SendEmail(dynamicEmail.FromName,
                                                                                          dynamicEmail.FromAddress,
                                                                                          billing.FullName,
                                                                                          billing.Email,
                                                                                          dynamicEmail.Subject,
                                                                                          emailBody);
                        email.Response = emailResult.ReturnValue.Response;
                        dao.Save<Email>(email);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                    emailBody = string.Empty;
                }

                //mark EmailQueue items as completed
                foreach (var item in emailQueueList)
                {
                    item.Completed = true;
                    dao.Save<EmailQueue>(item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void ProcessEmailQueue()
        {
            MySqlCommand q = new MySqlCommand("Select * From EmailQueue where Completed <> true group by BillingID");
            var emailQueueList = dao.Load<EmailQueue>(q);
            if (emailQueueList == null || emailQueueList.Count == 0)
                return;

            foreach (var item in emailQueueList)
            {
                ProcessEmailQueue(item.BillingID);
            }
        }

        private int? GetProductIdBySaleID(long? saleID)
        {
            int? res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"Select p.* from BillingSale bSale
                                                    inner join BillingSubscription bs on bs.BillingSubscriptionID=bSale.BillingSubscriptionID
                                                    inner join Subscription s on s.SubscriptionID=bs.SubscriptionID
                                                    inner join Product p on p.ProductID=s.ProductID
                                                    where bSale.SaleID=@SaleID
                                                    union all
                                                    Select p.* from UpsellSale uSale
                                                    inner join Upsell u on u.UpsellID=uSale.UpsellID
                                                    inner join UpsellType ut on ut.UpsellTypeID=u.UpsellTypeID
                                                    inner join Product p on p.ProductID=ut.ProductID
                                                    where uSale.SaleID=@SaleID
                                                    union all
                                                    Select p.* from OrderSale oSale
                                                    inner join Orders o on o.OrderID=oSale.OrderID
                                                    inner join Product p on p.ProductID=o.ProductID
                                                    where oSale.SaleID=@SaleID");

                q.Parameters.Add("@SaleID", MySqlDbType.Int64).Value = saleID;
                var product = dao.Load<Product>(q).FirstOrDefault();

                if (product != null)
                    res = product.ProductID;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        private DynamicEmail GetConfirmationDynamicEmail(int? productID, int? campaignID)
        {
            DynamicEmail res = null;
            try
            {
                //try get DynamicEmail by CampaignID
                var q = new MySqlCommand("select * from DynamicEmail " +
                               "where Active = 1 and DynamicEmailTypeID = 1 and CampaignID=@campaignID");
                q.Parameters.Add("@campaignID", MySqlDbType.Int32).Value = campaignID;
                res = dao.Load<DynamicEmail>(q).FirstOrDefault();
                if (res == null)
                {
                    //try get DynamicEmail by ProductID
                    q = new MySqlCommand("select * from DynamicEmail " +
                            "where ProductID = @productID and Active = 1 and DynamicEmailTypeID = 1");
                    q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                    res = dao.Load<DynamicEmail>(q).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        #endregion

        public Email SendShippingEmail(Billing billing, Subscription subscription, IList<InventoryView> products, string trackingNumber)
        {
            if (billing.Email == "email@email.me")
            {
                return null;
            }

            Email email = null;
            try
            {
                dao.BeginTransaction();

                DynamicEmail dynamicEmail = GetActiveDynamicEmails(subscription.ProductID.Value, billing.CampaignID, DynamicEmailTypeEnum.Shipment).FirstOrDefault();

                if (dynamicEmail != null)
                {
                    string strItemTemplate = "<tr height=\"30\"><td width=\"35%\"><font face=\"geneva, verdana, arial\" size=\"2\" color=\"#000000\">##ITEM_SKU##</font></td><td width=\"65%\"><font face=\"geneva, verdana, arial\" size=\"2\" color=\"#000000\">##ITEM_COUNT##</font></td></tr><tr><td colspan=\"2\" nowrap=\"nowrap\"><img src=\"http://d3oimv5qppjae2.cloudfront.net/email/spacer-cccccc.gif\" height=\"1\" width=\"100%\" /></td></tr>";
                    string strProductsTemplate = "";

                    foreach (InventoryView iv in products)
                    {
                        strProductsTemplate += strItemTemplate.Replace("##ITEM_SKU##", iv.Product).Replace("##ITEM_COUNT##", iv.Quantity.ToString());
                    }

                    dynamicEmail.Content = dynamicEmail.Content.Replace("##SHIPPING_PRODUCTS##", strProductsTemplate);
                    dynamicEmail.Content = dynamicEmail.Content.Replace("##TRACKING_NUMBER##", trackingNumber);

                    SendEmail(dynamicEmail, billing, null, null, null, null, null, DateTime.Now);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                email = null;
            }
            return email;
        }

        public Email SendShippingEmail(Billing billing, Product product, IList<Set<Shipment, ProductSKU>> products, string trackingNumber, DateTime shipDate)
        {
            if (billing.Email == "email@email.me")
            {
                return null;
            }

            Email email = null;
            try
            {
                dao.BeginTransaction();

                DynamicEmail dynamicEmail = GetActiveDynamicEmails(product.ProductID.Value, billing.CampaignID, DynamicEmailTypeEnum.Shipment).FirstOrDefault();

                if (dynamicEmail != null)
                {
                    string strItemTemplate = "<tr height=\"30\"><td width=\"35%\"><font face=\"geneva, verdana, arial\" size=\"2\" color=\"#000000\">##ITEM_SKU##</font></td><td width=\"65%\"><font face=\"geneva, verdana, arial\" size=\"2\" color=\"#000000\">##ITEM_COUNT##</font></td></tr><tr><td colspan=\"2\" nowrap=\"nowrap\"><img src=\"http://d3oimv5qppjae2.cloudfront.net/email/spacer-cccccc.gif\" height=\"1\" width=\"100%\" /></td></tr>";
                    string strProductsTemplate = "";

                    foreach (var iv in products.GroupBy(i => i.Value2.ProductName))
                    {
                        strProductsTemplate += strItemTemplate.Replace("##ITEM_SKU##", iv.Key).Replace("##ITEM_COUNT##", iv.Count().ToString());
                    }

                    dynamicEmail.Content = dynamicEmail.Content.Replace("##SHIPPING_PRODUCTS##", strProductsTemplate);
                    dynamicEmail.Content = dynamicEmail.Content.Replace("##TRACKING_NUMBER##", trackingNumber);

                    SendEmail(dynamicEmail, billing, null, null, null, null, null, DateTime.Now);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                email = null;
            }
            return email;
        }

        public Email SendRefundEmail(int productID, Billing billing, decimal? refundAmount)
        {

            Email email = null;
            try
            {
                dao.BeginTransaction();

                DynamicEmail dynamicEmail = GetActiveDynamicEmails(productID, billing.CampaignID, DynamicEmailTypeEnum.Refund).FirstOrDefault();

                if (dynamicEmail != null)
                {
                    string body = dynamicEmail.Content.Replace("##REFUND_AMOUNT##", Utility.FormatPrice(refundAmount));

                    email = SendEmail(dynamicEmail.DynamicEmailID.Value, dynamicEmail.FromName, dynamicEmail.FromAddress,
                    billing.FullName, billing.Email, dynamicEmail.Subject, body, billing.BillingID);
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                email = null;
            }
            return email;
        }

        public void SendCancellationEmailsOnCurrentDateAndHour(int productID)
        {
            try
            {
                foreach (DynamicEmail email in GetActiveDynamicEmails(productID, null, DynamicEmailTypeEnum.CancelledSubscribers))
                {
                    int hours = (email.Days != null) ? email.Days.Value : 0;
                    DateTime cancelDateTime = DateTime.Now.AddHours(-hours);

                    try
                    {
                        foreach (Set<Billing, Subscription, BillingCancelCode> b in subscriptionService.GetCancelledSubscriptions(email.ProductID.Value, cancelDateTime))
                        {
                            //TODO: hack email
                            b.Value1.Email = TEST_EMAIL;

                            SendEmail(email, b.Value1, b.Value2.InitialShipping, b.Value2.InitialBillAmount, null, b.Value3.CancelCode, null, DateTime.Now);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(GetType(), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public void SendReactivationEmailsOnCurrentDateAndHour(int productID)
        {
            try
            {
                foreach (DynamicEmail email in GetActiveDynamicEmails(productID, null, DynamicEmailTypeEnum.InactiveSubscribers))
                {
                    int hours = (email.Days != null) ? email.Days.Value : 0;
                    DateTime cancelDateTime = DateTime.Now.AddHours(-hours);

                    try
                    {
                        foreach (Set<Billing, Subscription> b in subscriptionService.GetSubscriptionsToReactivate(email.ProductID.Value, cancelDateTime))
                        {
                            //TODO: hack email
                            b.Value1.Email = TEST_EMAIL;

                            SendEmail(email, b.Value1, b.Value2.InitialShipping, b.Value2.InitialBillAmount, null, null, null, DateTime.Now);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(GetType(), ex);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        private IList<DynamicEmail> GetActiveDynamicEmails(int productID, int? campaignID, int dynamicEmailTypeID)
        {
            IList<DynamicEmail> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from DynamicEmail " +
                    "where ProductID = @productID and Active = 1 and DynamicEmailTypeID = @cancelledSubscribers");
                q.Parameters.Add("@cancelledSubscribers", MySqlDbType.Int32).Value = dynamicEmailTypeID;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                res = dao.Load<DynamicEmail>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<DynamicEmail> GetActiveDynamicEmails(int dynamicEmailTypeID)
        {
            IList<DynamicEmail> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from DynamicEmail " +
                    "where Active = 1 and ProductID is not null and DynamicEmailTypeID = @dynamicEmailTypeID");
                q.Parameters.Add("@dynamicEmailTypeID", MySqlDbType.Int32).Value = dynamicEmailTypeID;

                res = dao.Load<DynamicEmail>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public Email SendAbandonEmail(Registration registration, DynamicEmail dynamicEmail)
        {
            Email email = null;
            try
            {
                string body = dynamicEmail.Content
                    .Replace("##FNAME##", registration.FirstName)
                    .Replace("##LNAME##", registration.LastName)
                    .Replace("##ADD1##", registration.Address1)
                    .Replace("##ADD2##", registration.Address2)
                    .Replace("##CITY##", registration.City)
                    .Replace("##STATE##", registration.State)
                    .Replace("##ZIP##", registration.Zip)
                    .Replace("##PHONE##", registration.Phone)
                    .Replace("##EMAIL##", registration.Email)
                    ;

                var q = new MySqlCommand("Select * From Billing where RegistrationID=@RegistrationID limit 1");
                q.Parameters.Add("@RegistrationID", MySqlDbType.Int32).Value = registration.RegistrationID;
                var billing = dao.Load<Billing>(q).SingleOrDefault();
                var billingID = billing != null ? billing.BillingID : null;

                email = SendEmail(dynamicEmail.DynamicEmailID.Value, dynamicEmail.FromName, dynamicEmail.FromAddress,
                    registration.FullName, registration.Email, dynamicEmail.Subject, body, billingID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                email = null;
            }
            return email;
        }

        public Email SendNewsletterEmail(Registration registration, DynamicEmail dynamicEmail)
        {

            Email email = null;
            try
            {
                string body = dynamicEmail.Content
                    .Replace("##FNAME##", registration.FirstName)
                    .Replace("##LNAME##", registration.LastName)
                    .Replace("##ADD1##", registration.Address1)
                    .Replace("##ADD2##", registration.Address2)
                    .Replace("##CITY##", registration.City)
                    .Replace("##STATE##", registration.State)
                    .Replace("##ZIP##", registration.Zip)
                    .Replace("##PHONE##", registration.Phone)
                    .Replace("##EMAIL##", registration.Email)
                    ;

                var q = new MySqlCommand("Select * From Billing where RegistrationID=@RegistrationID limit 1");
                q.Parameters.Add("@RegistrationID", MySqlDbType.Int32).Value = registration.RegistrationID;
                var billing = dao.Load<Billing>(q).SingleOrDefault();
                var billingID = billing != null ? billing.BillingID : null;

                email = SendEmail(dynamicEmail.DynamicEmailID.Value, dynamicEmail.FromName, dynamicEmail.FromAddress,
                    registration.FullName, registration.Email, dynamicEmail.Subject, body, billingID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                email = null;
            }
            return email;
        }

        public Email SendRMAEmail(int billingID, string rma)
        {
            Billing billing = dao.Load<Billing>(billingID);

            if (billing == null)
                return null;

            if (billing.Email == "email@email.me")
                return null;

            Email email = null;
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select * from DynamicEmail where DynamicEmailTypeID = 12");

                DynamicEmail dynamicEmail = dao.Load<DynamicEmail>(q).FirstOrDefault();
                if (dynamicEmail == null)
                {
                    throw new Exception("Can't find DynamicEmail for RMA.");
                }

                string body = dynamicEmail.Content;
                body = body.Replace("##FNAME##", NullToEmpty(billing.FirstName));
                body = body.Replace("##LNAME##", NullToEmpty(billing.LastName));
                body = body.Replace("##ADD1##", NullToEmpty(billing.Address1));
                body = body.Replace("##ADD2##", NullToEmpty(billing.Address2));
                body = body.Replace("##CITY##", NullToEmpty(billing.City));
                body = body.Replace("##STATE##", NullToEmpty(billing.State));
                body = body.Replace("##ZIP##", NullToEmpty(billing.Zip));
                body = body.Replace("##PHONE##", NullToEmpty(billing.Phone));
                body = body.Replace("##EMAIL##", NullToEmpty(billing.Email));
                body = body.Replace("##RMA##", rma);

                email = SendEmail(dynamicEmail.DynamicEmailID.Value, dynamicEmail.FromName, dynamicEmail.FromAddress,
                    billing.FullName, billing.Email, dynamicEmail.Subject, body, billingID);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                email = null;
            }
            return email;
        }

        public Email SendRefererPassword(Referer referer, int productID)
        {
            Email email = null;
            try
            {
                DynamicEmail dynamicEmail = GetActiveDynamicEmails(productID, null, DynamicEmailTypeEnum.RefererPasswordRecovery).FirstOrDefault();
                if (dynamicEmail != null)
                {
                    string body = dynamicEmail.Content
                        .Replace("##FNAME##", referer.FirstName)
                        .Replace("##PASSWORD##", referer.Password);

                    var q = new MySqlCommand("Select b.* From Billing b inner join RefererBilling rb on rb.BillingID=b.BillingID where rb.RefererID=@RefererID limit 1");
                    q.Parameters.Add("@RefererID", MySqlDbType.Int32).Value = referer.RefererID;
                    var billing = dao.Load<Billing>(q).SingleOrDefault();
                    var billingID = billing != null ? billing.BillingID : null;

                    email = SendEmail(dynamicEmail.DynamicEmailID.Value, dynamicEmail.FromName, dynamicEmail.FromAddress,
                        referer.FullName, referer.Username, dynamicEmail.Subject, body, billingID);
                }
                else
                {
                    throw new Exception(string.Format("Can't find DynamicEmail for Product({0}) for \"Referer Password Recovery\" group.", productID));
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                email = null;
            }
            return email;
        }

        public Email SendRefererPromotion1(Referer referer, int productID)
        {
            Email email = null;
            try
            {
                DynamicEmail dynamicEmail = GetActiveDynamicEmails(productID, null, DynamicEmailTypeEnum.RefererPromotion1).FirstOrDefault();
                if (dynamicEmail != null)
                {
                    string body = dynamicEmail.Content
                        .Replace("##FNAME##", referer.FirstName)
                        .Replace("##USERNAME##", referer.Username)
                        .Replace("##PASSWORD##", referer.Password)
                        .Replace("##REFERRALID##", referer.RefererCode);

                    var q = new MySqlCommand("Select b.* From Billing b inner join RefererBilling rb on rb.BillingID=b.BillingID where rb.RefererID=@RefererID limit 1");
                    q.Parameters.Add("@RefererID", MySqlDbType.Int32).Value = referer.RefererID;
                    var billing = dao.Load<Billing>(q).SingleOrDefault();
                    var billingID = billing != null ? billing.BillingID : null;

                    email = SendEmail(dynamicEmail.DynamicEmailID.Value, dynamicEmail.FromName, dynamicEmail.FromAddress,
                        referer.FullName, referer.Username, dynamicEmail.Subject, body, billingID);
                }
                else
                {
                    throw new Exception(string.Format("Can't find DynamicEmail for Product({0}) for \"Referer Promotion 1\" group.", productID));
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                email = null;
            }
            return email;
        }

        public Email SendEmail(int dynamicEmailID, string fromName, string fromAddress, string toName, string toAddress, string subject, string body, long? billingID)
        {
            if (toAddress == "email@email.me" || IsBlockedEmail(toAddress))
            {
                return null;
            }

            Email email = null;
            try
            {
                dao.BeginTransaction();

                email = new Email();
                email.BillingID = billingID;
                email.DynamicEmailID = dynamicEmailID;
                email.Email_ = toAddress;
                email.Subject = subject;
                email.Body = body;
                email.CreateDT = DateTime.Now;
               
                dao.Save<Email>(email);

                BusinessError<GatewayResult> emailResult = emailGateway.SendEmail(fromName, fromAddress, toName, toAddress, subject, body);

                email.Response = emailResult.ReturnValue.Response;
                dao.Save<Email>(email);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                email = null;
            }
            return email;
        }

        public IList<DynamicEmailType> GetDynamicEmailType()
        {
            IList<DynamicEmailType> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select * from DynamicEmailType " +
                    "order by SortOrder");

                res = dao.Load<DynamicEmailType>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        //private int[] DYNAMIC_EMAIL_IGNORE_LIST = { 28, 34, 30, 37, 42, 22, 4, 43, 33, 36, 35, 5, 31, 32 };
        public IList<ProductEmailTypeView> GetEmailTypeByProduct(int productID)
        {
            IList<ProductEmailTypeView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select t.*, de.DynamicEmailID, de.Active, " +
                    "case when de.DynamicEmailID is null then 0 else (select count(*) from Email where DynamicEmailID = de.DynamicEmailID) end as EmailCount " +
                    "from (" +
                    "select distinct det.ID as DynamicEmailTypeID, det.DisplayName, " +
                    "case det.ID when @DynamicEmqailType_Abandons then coalesce(de.Days, -1) when @DynamicEmqailType_Newsletter then coalesce(de.Days, -1) else coalesce(de.Days, 0) end as Hours, " +
                    "gsDeCustom.StoreID as GiftCertificateDynamicEmail_StoreID, " +
                    "case de.DynamicEmailTypeID when @DynamicEmqailType_GiftCertificate then gsDeCustom.Name else '' end as CustomName, " +
                    "det.SortOrder " +
                    "from DynamicEmailType det " +
                    "left join DynamicEmail de on de.DynamicEmailTypeID = det.ID " +
                    "left join Store gsDeCustom on de.DynamicEmailTypeID = @DynamicEmqailType_GiftCertificate " +
                    //"and de.DynamicEmailID not in (" + string.Join(", ", DYNAMIC_EMAIL_IGNORE_LIST.Select(i => i.ToString()).ToArray()) + ") " +
                    "union " +
                    "select @DynamicEmqailType_Abandons as DynamicEmailTypeID, 'Abandons' as DisplayName, -1 as Hours, null as GiftCertificateDynamicEmail_StoreID, '' as CustomName, 3 as SortOrder " +
                    "union " +
                    "select @DynamicEmqailType_Newsletter as DynamicEmailTypeID, 'Newsletter' as DisplayName, -1 as Hours, null as GiftCertificateDynamicEmail_StoreID, '' as CustomName, 11 as SortOrder " +
                    "order by SortOrder, Hours " +
                    ") t " +
                    "left join GiftCertificateDynamicEmail gsDe on t.DynamicEmailTypeID = @DynamicEmqailType_GiftCertificate and gsDe.StoreID = t.GiftCertificateDynamicEmail_StoreID " +
                    "left join DynamicEmail de on de.DynamicEmailTypeID = t.DynamicEmailTypeID and coalesce(de.Days, 0) = t.Hours and de.ProductID = @productID " +
                    //"and de.DynamicEmailID not in (" + string.Join(", ", DYNAMIC_EMAIL_IGNORE_LIST.Select(i => i.ToString()).ToArray()) + ") " +
                    "and (t.DynamicEmailTypeID <> @DynamicEmqailType_GiftCertificate or de.DynamicEmailID = gsDe.DynamicEmailID) " +
                    "");
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@DynamicEmqailType_GiftCertificate", MySqlDbType.Int32).Value = TrimFuel.Model.Enums.DynamicEmailTypeEnum.GiftCertificateGiven;
                q.Parameters.Add("@DynamicEmqailType_Abandons", MySqlDbType.Int32).Value = TrimFuel.Model.Enums.DynamicEmailTypeEnum.Abandons;
                q.Parameters.Add("@DynamicEmqailType_Newsletter", MySqlDbType.Int32).Value = TrimFuel.Model.Enums.DynamicEmailTypeEnum.Newsletter;

                res = dao.Load<ProductEmailTypeView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        //private int[] DYNAMIC_EMAIL_IGNORE_LIST = { 28, 34, 30, 37, 42, 22, 4, 43, 33, 36, 35, 5, 31, 32 };
        public IList<CampaignEmailTypeView> GetDynamicEmailByCampaign(int productID, int campaignID)
        {
            IList<CampaignEmailTypeView> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select de.CampaignID, de.ProductID, t.*, de.DynamicEmailID, de.Active, 0 as EmailCount " +
                    "from (" +
                    "select distinct det.ID as DynamicEmailTypeID, det.DisplayName, " +
                    "case det.ID when @DynamicEmqailType_Abandons then coalesce(de.Days, -1) when @DynamicEmqailType_Newsletter then coalesce(de.Days, -1) else coalesce(de.Days, 0) end as Hours, " +
                    "gsDeCustom.StoreID as GiftCertificateDynamicEmail_StoreID, " +
                    "case de.DynamicEmailTypeID when @DynamicEmqailType_GiftCertificate then gsDeCustom.Name else '' end as CustomName, " +
                    "det.SortOrder " +
                    "from DynamicEmailType det " +
                    "left join DynamicEmail de on de.DynamicEmailTypeID = det.ID " +
                    "left join Store gsDeCustom on de.DynamicEmailTypeID = @DynamicEmqailType_GiftCertificate " +
                    "union " +
                    "select @DynamicEmqailType_Abandons as DynamicEmailTypeID, 'Abandons' as DisplayName, -1 as Hours, null as GiftCertificateDynamicEmail_StoreID, '' as CustomName, 3 as SortOrder " +
                    "union " +
                    "select @DynamicEmqailType_Newsletter as DynamicEmailTypeID, 'Newsletter' as DisplayName, -1 as Hours, null as GiftCertificateDynamicEmail_StoreID, '' as CustomName, 11 as SortOrder " +
                    "order by SortOrder, Hours " +
                    ") t " +
                    "left join GiftCertificateDynamicEmail gsDe on t.DynamicEmailTypeID = @DynamicEmqailType_GiftCertificate and gsDe.StoreID = t.GiftCertificateDynamicEmail_StoreID " +
                    "left join DynamicEmail prod_de on prod_de.DynamicEmailTypeID = t.DynamicEmailTypeID and coalesce(prod_de.Days, 0) = t.Hours and prod_de.ProductID = @productID " +
                    " and (t.DynamicEmailTypeID <> @DynamicEmqailType_GiftCertificate or prod_de.DynamicEmailID = gsDe.DynamicEmailID) " +
                    "left join DynamicEmail camp_de on camp_de.DynamicEmailTypeID = t.DynamicEmailTypeID and coalesce(camp_de.Days, 0) = t.Hours and camp_de.CampaignID = @campaignID " +
                    " and (t.DynamicEmailTypeID <> @DynamicEmqailType_GiftCertificate or camp_de.DynamicEmailID = gsDe.DynamicEmailID) " +
                    "left join DynamicEmail de on de.DynamicEmailID = coalesce(camp_de.DynamicEmailID, prod_de.DynamicEmailID) " +
                    "");
                q.Parameters.Add("@campaignID", MySqlDbType.Int32).Value = campaignID;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                q.Parameters.Add("@DynamicEmqailType_GiftCertificate", MySqlDbType.Int32).Value = TrimFuel.Model.Enums.DynamicEmailTypeEnum.GiftCertificateGiven;
                q.Parameters.Add("@DynamicEmqailType_Abandons", MySqlDbType.Int32).Value = TrimFuel.Model.Enums.DynamicEmailTypeEnum.Abandons;
                q.Parameters.Add("@DynamicEmqailType_Newsletter", MySqlDbType.Int32).Value = TrimFuel.Model.Enums.DynamicEmailTypeEnum.Newsletter;

                res = dao.Load<CampaignEmailTypeView>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public DynamicEmail CreateDynamicEmail(int? productID, int? campaignID, byte? dynamicEmailTypeID, short? days, bool? active,
            string fromName, string fromAddress, string subject, string landing, string landingLink, string content,
            int? giftCertificateDynamicEmail_StoreID)
        {
            DynamicEmail email = null;
            try
            {
                dao.BeginTransaction();

                email = new DynamicEmail();
                email.ProductID = productID;
                email.CampaignID = campaignID;
                email.DynamicEmailTypeID = dynamicEmailTypeID;
                email.Days = days;
                email.Active = active;
                email.FromName = fromName;
                email.FromAddress = fromAddress;
                email.Subject = subject;
                email.Landing = landing;
                email.LandingLink = landingLink;
                email.Content = content;

                if (dynamicEmailTypeID == TrimFuel.Model.Enums.DynamicEmailTypeEnum.GiftCertificateGiven)
                {
                    GiftCertificateDynamicEmail customEmail = new GiftCertificateDynamicEmail();
                    customEmail.FillFromDynamicEmail(email);
                    customEmail.StoreID = giftCertificateDynamicEmail_StoreID;
                    dao.Save<GiftCertificateDynamicEmail>(customEmail);
                    email = customEmail;
                }
                else
                {
                    dao.Save<DynamicEmail>(email);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                email = null;
            }
            return email;
        }

        public DynamicEmail UpdateDynamicEmail(int dynamicEmailID, int? productID, int? campaignID, byte? dynamicEmailTypeID, short? days, bool? active,
            string fromName, string fromAddress, string subject, string landing, string landingLink, string content)
        {
            DynamicEmail email = null;
            try
            {
                dao.BeginTransaction();

                email = EnsureLoad<DynamicEmail>(dynamicEmailID);
                email.ProductID = productID;
                email.CampaignID = campaignID;
                email.DynamicEmailTypeID = dynamicEmailTypeID;
                email.Days = days;
                email.Active = active;
                email.FromName = fromName;
                email.FromAddress = fromAddress;
                email.Subject = subject;
                email.Landing = landing;
                email.LandingLink = landingLink;
                email.Content = content;

                dao.Save<DynamicEmail>(email);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                email = null;
            }
            return email;
        }

        public void SetDynamicEmailState(int dynamicEmailID, bool active)
        {
            try
            {
                dao.BeginTransaction();

                DynamicEmail email = EnsureLoad<DynamicEmail>(dynamicEmailID);
                email.Active = active;
                dao.Save<DynamicEmail>(email);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public void DeleteDynamicEmail(int dynamicEmailID)
        {
            try
            {
                dao.BeginTransaction();

                DynamicEmail email = EnsureLoad<DynamicEmail>(dynamicEmailID);
                MySqlCommand q = new MySqlCommand("delete from DynamicEmail where DynamicEmailID = @dynamicEmailID");
                q.Parameters.Add("dynamicEmailID", MySqlDbType.Int32).Value = dynamicEmailID;
                dao.ExecuteNonQuery(q);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public TPClientEmail SendTPClientEmail(int adminID, string fromAddress, string toAddress, string subject, string body, int tpClientID)
        {
            if (toAddress == "email@email.me")
            {
                return null;
            }

            TPClientEmail email = null;
            try
            {
                dao.BeginTransaction();

                email = new TPClientEmail();
                email.AdminID = adminID;
                email.Content = body;
                email.Subject = subject;
                email.CreateDT = DateTime.Now;
                email.From = fromAddress;
                email.To = toAddress;
                email.TPClientID = tpClientID;

                dao.Save<TPClientEmail>(email);

                //send email
                try
                {
                    BusinessError<GatewayResult> emailResult = emailGateway.SendEmail(fromAddress, fromAddress, toAddress, toAddress, subject, body);
                    email.Response = emailResult.ReturnValue.Response;
                    dao.Save<TPClientEmail>(email);
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                email = null;
            }
            return email;
        }

        private bool IsBlockedEmail(string email)
        {
            MySqlCommand q = new MySqlCommand(@"SELECT * FROM BlockedEmailDomain WHERE Name = @Name");
            q.Parameters.Add("@Name", MySqlDbType.VarChar).Value = DomainName(email);
           
            return dao.Load<BlockedEmailDomain>(q).FirstOrDefault() != null;
        }

        private string DomainName(string email)
        {
            return email.Substring(email.IndexOf('@') + 1);
        }

    }
}
