using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Model.Enums;
using TrimFuel.Business.Gateways;
using TrimFuel.Model.Containers;
using System.Web;
using TrimFuel.Business.Utils;

namespace TrimFuel.Business
{
    public class BillingService : BaseService
    {
        private RegistrationService registrationService { get { return new RegistrationService(); } }
        private SaleService saleService { get { return new SaleService(); } }
        private SubscriptionService subscriptionService { get { return new SubscriptionService(); } }
        private MerchantService merchantService { get { return new MerchantService(); } }
        private EmailService emailService { get { return new EmailService(); } }
        private RefererService refererService { get { return new RefererService(); } }

        public BusinessError<Registration> CreateProspect(int? productID,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate,
            string internalID,
            string customField1, string customField2, string customField3, string customField4, string customField5)
        {
            BusinessError<Registration> res = new BusinessError<Registration>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                dao.BeginTransaction();

                IList<string> insufficientFields = new List<string>();
                if (string.IsNullOrEmpty(firstName)) insufficientFields.Add("firstName");
                if (string.IsNullOrEmpty(lastName)) insufficientFields.Add("lastName");
                //if (string.IsNullOrEmpty(address1)) insufficientFields.Add("address1");
                //if (string.IsNullOrEmpty(city)) insufficientFields.Add("city");
                //if (string.IsNullOrEmpty(state)) insufficientFields.Add("state");
                //if (string.IsNullOrEmpty(zip)) insufficientFields.Add("zip");
                //if (string.IsNullOrEmpty(phone)) insufficientFields.Add("phone");
                //if (string.IsNullOrEmpty(email)) insufficientFields.Add("email");
                //if (string.IsNullOrEmpty(ip)) insufficientFields.Add("ip");

                if (insufficientFields.Count > 0)
                {
                    res.ErrorMessage = string.Format("Insufficient data, the following fields are required: {0}.", string.Join(", ", insufficientFields.ToArray()));
                }
                else
                {
                    if (!string.IsNullOrEmpty(internalID) &&
                        GetLastBillingByInternalID(internalID) != null)
                    {
                        res.ErrorMessage = "User with InternalID(" + internalID + ") already exists.";
                    }
                    else
                    {
                        int? campaignID = null;
                        if (productID != null)
                        {
                            Campaign campaign = GetOrCreateCampaignByProduct(productID.Value);
                            if (campaign != null)
                            {
                                campaignID = campaign.CampaignID;
                            }
                        }

                        Registration reg = registrationService.CreateRegistration(campaignID,
                            firstName, lastName,
                            address1, address2, city, state, zip, country,
                            email, phone, DateTime.Now, affiliate, subAffiliate, ip, null);

                        Billing bill = registrationService.CreateBilling(campaignID,
                            reg.RegistrationID,
                            firstName, lastName,
                            null, null, null, null, null,
                            address1, address2, city, state, zip, country,
                            email, phone, DateTime.Now, affiliate, subAffiliate, ip, null);

                        BillingExternalInfo billingInfo = new BillingExternalInfo();
                        billingInfo.BillingID = bill.BillingID;
                        billingInfo.InternalID = internalID;
                        billingInfo.CustomField1 = customField1;
                        billingInfo.CustomField2 = customField2;
                        billingInfo.CustomField3 = customField3;
                        billingInfo.CustomField4 = customField4;
                        billingInfo.CustomField5 = customField5;
                        dao.Save<BillingExternalInfo>(billingInfo);

                        res.State = BusinessErrorState.Success;
                        res.ReturnValue = reg;
                        res.ErrorMessage = string.Empty;
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
            return res;
        }

        private const string CAMPAIGN_NAME_TEMPLATE = "Billing API, {0}";
        private Campaign GetOrCreateCampaignByProduct(int productID)
        {
            Campaign res = null;

            try
            {
                dao.BeginTransaction();

                Product p = EnsureLoad<Product>(productID);
                string campName = string.Format(CAMPAIGN_NAME_TEMPLATE, p.ProductName);
                MySqlCommand q = new MySqlCommand(@"
                    select c.* from Campaign c
                    inner join Subscription s on s.SubscriptionID = c.SubscriptionID
                    where c.DisplayName = @displayName and s.ProductID = @productID
                ");
                q.Parameters.Add("@displayName", MySqlDbType.VarChar).Value = campName;
                q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;

                res = dao.Load<Campaign>(q).FirstOrDefault();
                if (res == null)
                {
                    Subscription s = subscriptionService.GetOrCreateOneTimeSaleSubscription(productID);
                    res = new Campaign();
                    res.DisplayName = campName;
                    res.SubscriptionID = s.SubscriptionID;
                    res.Percentage = 100;
                    res.Active = true;
                    res.CreateDT = DateTime.Today;
                    res.Redirect = false;
                    res.IsSave = false;
                    res.EnableFitFactory = false;
                    res.IsSTO = false;
                    res.IsExternal = true;
                    res.SendUserEmail = true;
                    dao.Save<Campaign>(res);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }

            return res;
        }

        public Billing GetLastBillingByInternalID(string internalID)
        {
            if (string.IsNullOrEmpty(internalID))
            {
                return null;
            }

            Billing res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select b.* from Billing b " +
                    "inner join BillingExternalInfo bei on bei.BillingID = b.BillingID " +
                    "where bei.InternalID = @internalID " +
                    "order by b.BillingID desc");
                q.Parameters.Add("@internalID", MySqlDbType.VarChar).Value = internalID;

                res = dao.Load<Billing>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public Billing GetLastBillingByPhoneOrEmail(string phone, string email)
        {
            if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(email))
                return null;

            Billing res = null;

            try
            {
                if (!string.IsNullOrEmpty(phone))
                {
                    MySqlCommand q = new MySqlCommand("select * from Billing where Phone = @phone");
                    q.Parameters.Add("@phone", phone);

                    res = dao.Load<Billing>(q).FirstOrDefault();
                }
                else if (!string.IsNullOrEmpty(email))
                {
                    MySqlCommand q = new MySqlCommand("select * from Billing where Email = @email");
                    q.Parameters.Add("@email", email);

                    res = dao.Load<Billing>(q).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public Billing GetLastBillingByProspectID(long prospectID)
        {
            Billing res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select b.* from Billing b where b.RegistrationID = @prospectID ");
                q.Parameters.Add("@prospectID", MySqlDbType.Int64).Value = prospectID;

                res = dao.Load<Billing>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public BusinessError<Set<Registration, Billing>> CreateOrUpdateBillingByProspectOrInternalID(long? registrationID, string internalID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            string customField1, string customField2, string customField3, string customField4, string customField5)
        {
            BusinessError<Set<Registration, Billing>> res = new BusinessError<Set<Registration, Billing>>(null, BusinessErrorState.Error, "Unrecognized error occurred.");
            try
            {
                dao.BeginTransaction();

                Registration reg = null;
                Billing bill = null;

                if (registrationID != null)
                {
                    reg = dao.Load<Registration>(registrationID);
                    if (reg == null)
                    {
                        //ProspectID doesn't exist
                        res.ErrorMessage = "ProspectID(" + registrationID.Value.ToString() + ") was not found.";
                    }
                    else
                    {
                        bill = GetLastBillingByInternalID(internalID);
                        if (bill != null)
                        {
                            if (bill.RegistrationID != reg.RegistrationID)
                            {
                                //Billing by InternalID exists but ProspectID doesn't match
                                res.ErrorMessage = string.Format("ProspectID({0}) doesn't match to InternalID({1})", registrationID, internalID);
                            }
                            else
                            {
                                //Billing by InternalID exists and ProspectID matches
                                registrationService.UpdateBilling(bill.BillingID.Value, campaignID, registrationID, firstName, lastName,
                                    creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city,
                                    state, zip, country, email, phone, bill.CreateDT, affiliate, subAffiliate, ip, url);
                                bill = EnsureLoad<Billing>(bill.BillingID);

                                res.State = BusinessErrorState.Success;
                                res.ErrorMessage = string.Empty;
                                res.ReturnValue = new Set<Registration, Billing>() { Value1 = reg, Value2 = bill };
                            }
                        }
                        else
                        {
                            MySqlCommand q = new MySqlCommand(
                                " select b.* from Billing b" +
                                " where b.RegistrationID = @registrationID"
                                );
                            q.Parameters.Add("@registrationID", MySqlDbType.Int64).Value = registrationID;
                            bill = dao.Load<Billing>(q).FirstOrDefault();

                            if (bill != null)
                            {
                                if (string.IsNullOrEmpty(internalID))
                                {
                                    //Billing by InternalID doesn't exist, 
                                    //Billing by ProspectID exists and InternalID is not specified
                                    registrationService.UpdateBilling(bill.BillingID.Value, campaignID, registrationID, firstName, lastName,
                                        creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city,
                                        state, zip, country, email, phone, bill.CreateDT, affiliate, subAffiliate, ip, url);
                                    bill = EnsureLoad<Billing>(bill.BillingID);

                                    res.State = BusinessErrorState.Success;
                                    res.ErrorMessage = string.Empty;
                                    res.ReturnValue = new Set<Registration, Billing>() { Value1 = reg, Value2 = bill };
                                }
                                else
                                {
                                    BillingExternalInfo billingInfo = null;
                                    q = new MySqlCommand(
                                        " select bei.* from BillingExternalInfo bei" +
                                        " where bei.BillingID = @billingID"
                                        );
                                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = bill.BillingID;
                                    billingInfo = dao.Load<BillingExternalInfo>(q).FirstOrDefault();
                                    if (billingInfo == null || string.IsNullOrEmpty(billingInfo.InternalID))
                                    {
                                        //Billing by InternalID doesn't exist, 
                                        //Billing by ProspectID exists, 
                                        //InternalID specified and Billing by ProspectID has empty InternalID
                                        registrationService.UpdateBilling(bill.BillingID.Value, campaignID, registrationID, firstName, lastName,
                                            creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city,
                                            state, zip, country, email, phone, bill.CreateDT, affiliate, subAffiliate, ip, url);
                                        bill = EnsureLoad<Billing>(bill.BillingID);

                                        if (billingInfo == null)
                                        {
                                            billingInfo = new BillingExternalInfo();
                                            billingInfo.BillingID = bill.BillingID;
                                        }
                                        billingInfo.InternalID = internalID;
                                        billingInfo.CustomField1 = customField1;
                                        billingInfo.CustomField2 = customField2;
                                        billingInfo.CustomField3 = customField3;
                                        billingInfo.CustomField4 = customField4;
                                        billingInfo.CustomField5 = customField5;
                                        dao.Save<BillingExternalInfo>(billingInfo);

                                        res.State = BusinessErrorState.Success;
                                        res.ErrorMessage = string.Empty;
                                        res.ReturnValue = new Set<Registration, Billing>() { Value1 = reg, Value2 = bill };
                                    }
                                    else
                                    {
                                        //Billing by InternalID doesn't exist, 
                                        //Billing by ProspectID exists, 
                                        //InternalID specified but Billing by ProspectID has wrong InternalID
                                        res.ErrorMessage = string.Format("InternalID({0}) doesn't match to ProspectID({1})", internalID, registrationID);
                                    }
                                }
                            }
                            else
                            {
                                //Billing by InternalID doesn't exist, Billing by ProspectID doesn't exist
                                bill = registrationService.CreateBilling(campaignID, registrationID, firstName, lastName,
                                    creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city,
                                    state, zip, country, email, phone, DateTime.Now, affiliate, subAffiliate, ip, url);
                                BillingExternalInfo billingInfo = new BillingExternalInfo();
                                billingInfo.BillingID = bill.BillingID;
                                billingInfo.InternalID = internalID;
                                billingInfo.CustomField1 = customField1;
                                billingInfo.CustomField2 = customField2;
                                billingInfo.CustomField3 = customField3;
                                billingInfo.CustomField4 = customField4;
                                billingInfo.CustomField5 = customField5;
                                dao.Save<BillingExternalInfo>(billingInfo);

                                res.State = BusinessErrorState.Success;
                                res.ErrorMessage = string.Empty;
                                res.ReturnValue = new Set<Registration, Billing>() { Value1 = reg, Value2 = bill };
                            }
                        }
                    }
                }
                else
                {
                    bill = GetLastBillingByInternalID(internalID);
                    if (bill != null)
                    {
                        //ProspectID is not specified, Billing by InternalID exists
                        reg = EnsureLoad<Registration>(bill.RegistrationID);

                        registrationService.UpdateBilling(bill.BillingID.Value, campaignID, reg.RegistrationID, firstName, lastName,
                            creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city,
                            state, zip, country, email, phone, bill.CreateDT, affiliate, subAffiliate, ip, url);
                        bill = EnsureLoad<Billing>(bill.BillingID);

                        res.State = BusinessErrorState.Success;
                        res.ErrorMessage = string.Empty;
                        res.ReturnValue = new Set<Registration, Billing>() { Value1 = reg, Value2 = bill };
                    }
                    else
                    {
                        //ProspectID is not specified, Billing by InternalID doesn't exist
                        reg = registrationService.CreateRegistration(campaignID, firstName, lastName, address1,
                            address2, city, state, zip, country, email, phone, DateTime.Now, affiliate, subAffiliate, ip, url);

                        bill = registrationService.CreateBilling(campaignID, reg.RegistrationID, firstName, lastName,
                            creditCard, cvv, paymentTypeID, expMonth, expYear, address1, address2, city,
                            state, zip, country, email, phone, DateTime.Now, affiliate, subAffiliate, ip, url);
                        BillingExternalInfo billingInfo = new BillingExternalInfo();
                        billingInfo.BillingID = bill.BillingID;
                        billingInfo.InternalID = internalID;
                        billingInfo.CustomField1 = customField1;
                        billingInfo.CustomField2 = customField2;
                        billingInfo.CustomField3 = customField3;
                        billingInfo.CustomField4 = customField4;
                        billingInfo.CustomField5 = customField5;
                        dao.Save<BillingExternalInfo>(billingInfo);

                        res.State = BusinessErrorState.Success;
                        res.ErrorMessage = string.Empty;
                        res.ReturnValue = new Set<Registration, Billing>() { Value1 = reg, Value2 = bill };
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res.ReturnValue = null;
            }
            return res;
        }

        public bool IsChargesBlocked(Billing billing, out string chargeBlockReason)
        {
            bool res = false;
            chargeBlockReason = null;
            try
            {
                if (billing != null && billing.BillingID != null)
                {
                    MySqlCommand q = new MySqlCommand(@"
                        select * from BillingStopCharge
                        where BillingID = @billingID
                        order by BillingID desc
                        limit 1
                    ");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = billing.BillingID;
                    var block = dao.Load<BillingStopCharge>(q).FirstOrDefault();
                    if (block != null)
                    {
                        res = true;
                        chargeBlockReason = block.StopReason;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        private BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>> CreateSubscription(Billing billing, Subscription subscription, string description, bool chargeForTrial)
        {
            BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>> res = new BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            
            string chargeBlockReason = null;
            if (IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            BillingSale sale = null;
            BillingSubscription billingSubscription = null;

            bool isTestCase = ((!SaleService.CHARGE) || saleService.IsTestCreditCard(billing.CreditCard));

            AssertigyMID assertigyMid = null;

            if (!chargeForTrial)
            {
                //Create subscription in any case
                billingSubscription = saleService.CreateBillingSubscription(billing, subscription);

                res.State = BusinessErrorState.Success;
                res.ErrorMessage = string.Empty;
                res.ReturnValue = new Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>()
                {
                    Value1 = billingSubscription
                };
            }
            else
            {
                try
                {
                    //TODO: Implement dao.BeginTopTransaction() 
                    dao.BeginTransaction();

                    billingSubscription = saleService.CreateBillingSubscription(billing, subscription);

                    if (subscription.InitialBillAmount == null)
                    {
                        res.ErrorMessage = "Invalid Subscription(" + subscription.SubscriptionID.Value + ") settings: InitialBillAmount";
                        throw new Exception("Invalid Subscription(" + subscription.SubscriptionID.Value + ") settings: InitialBillAmount");
                    }

                    decimal amount = subscription.InitialBillAmount.Value;
                    decimal? shippingAmount = subscription.InitialShipping;

                    if (!isTestCase)
                    {
                        sale = new BillingSale();
                        sale.SaleTypeID = SaleTypeEnum.Billing;
                        sale.TrackingNumber = null;
                        sale.CreateDT = DateTime.Now;
                        sale.NotShip = false;
                        sale.BillingSubscriptionID = billingSubscription.BillingSubscriptionID;
                        sale.RebillCycle = 0; //TODO ?
                        sale.ChargeHistoryID = null;
                        sale.PaygeaID = null;
                        sale.ProductCode = subscription.ProductCode;
                        sale.Quantity = subscription.Quantity;
                        dao.Save<BillingSale>(sale);

                        if (shippingAmount != null)
                        {
                            amount += shippingAmount.Value;
                        }

                        Set<NMICompany, AssertigyMID> mid = null;
                        mid = merchantService.ChooseRandomNMIMerchantAccount(subscription.ProductID.Value, billing, amount);

                        if (mid != null)
                        {
                            NMICompany nmiCompany = mid.Value1;
                            assertigyMid = mid.Value2;

                            Currency cur = saleService.GetCurrencyByProduct(subscription.ProductID.Value);

                            if (shippingAmount != null)
                            {
                                SaleChargeDetails saleCD = new SaleChargeDetails();
                                saleCD.SaleID = sale.SaleID;
                                saleCD.SaleChargeTypeID = SaleChargeTypeEnum.Shipping;
                                saleCD.Amount = shippingAmount.Value;
                                if (cur != null)
                                {
                                    saleCD.CurrencyID = cur.CurrencyID;
                                    saleCD.CurrencyAmount = saleCD.Amount;
                                    saleCD.Amount = cur.ConvertToUSD(saleCD.CurrencyAmount.Value);
                                }
                                dao.Save<SaleChargeDetails>(saleCD);
                            }

                            if (!string.IsNullOrEmpty(description))
                            {
                                SaleDetails sd = new SaleDetails();
                                sd.SaleID = sale.SaleID;
                                sd.Description = description;
                                dao.Save<SaleDetails>(sd);
                            }

                            Product product = EnsureLoad<Product>(subscription.ProductID);

                            IPaymentGateway paymentGateway = saleService.GetGatewayByMID(assertigyMid);

                            BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                                nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                                sale.SaleID.Value, billing, product);

                            if (paymentResult.State == BusinessErrorState.Success)
                            {
                                res.State = BusinessErrorState.Success;
                            }
                            else
                            {
                                dao.RollbackTransaction();

                                sale = null;

                                res.State = BusinessErrorState.Error;
                                billingSubscription = null;
                                //billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Declined;

                                dao.BeginTransaction();
                            }

                            //dao.Save<BillingSubscription>(billingSubscription);

                            Set<Paygea, ChargeHistoryEx, FailedChargeHistoryView> chargeLog = saleService.ChargeLogging(paymentResult, billing, billingSubscription,
                                subscription.ProductCode, SaleTypeEnum.Billing, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                            if (sale != null)
                            {
                                sale.PaygeaID = chargeLog.Value1.PaygeaID;
                                sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                                dao.Save<BillingSale>(sale);
                            }

                            res.ErrorMessage = chargeLog.Value1.Response;
                            res.ReturnValue = new Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>()
                            {
                                Value1 = billingSubscription,
                                Value2 = chargeLog.Value2,
                                Value3 = chargeLog.Value3
                            };
                        }
                        else
                        {
                            billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Inactive;
                            dao.Save<BillingSubscription>(billingSubscription);

                            res.ErrorMessage = "No MID installed";
                            res.ReturnValue = new Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>()
                            {
                                Value1 = billingSubscription
                            };
                        }
                    }
                    else
                    {
                        if (shippingAmount != null)
                        {
                            amount += shippingAmount.Value;
                        }
                        if (billing.FirstName.ToLower() == "success")
                        {
                            billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Active;
                            res.State = BusinessErrorState.Success;
                            res.ReturnValue = new Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>()
                            {
                                Value1 = billingSubscription,
                                Value2 = new ChargeHistoryEx()
                                {
                                    ChargeHistoryID = 0,
                                    ChildMID = "Test",
                                    ChargeDate = DateTime.Now,
                                    Success = true,
                                    Amount = amount,
                                    Response = "response=1&responsetext=Test case: SUCCESS&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid=&type=sale&response_code=100"
                                }
                            };
                            res.ErrorMessage = "Test case: SUCCESS";
                        }
                        else
                        {
                            billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Declined;
                            res.State = BusinessErrorState.Error;
                            res.ReturnValue = new Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>()
                            {
                                Value1 = billingSubscription,
                                Value3 = new FailedChargeHistoryView()
                                {
                                    FailedChargeHistoryID = 0,
                                    MerchantAccountID = null,
                                    ChildMID = "Test",
                                    BillingID = billing.BillingID,
                                    SaleTypeID = SaleTypeEnum.Upsell,
                                    ChargeDate = DateTime.Now,
                                    Success = false,
                                    Amount = amount,
                                    Response = "response=2&responsetext=Test case: ERROR&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid=&type=sale&response_code=200"
                                }
                            };
                            res.ErrorMessage = "Test case: ERROR";
                        }
                    }
                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                    dao.RollbackTransaction();
                    res.State = BusinessErrorState.Error;
                }

                if (res != null && subscription.ProductID != null && res.State == BusinessErrorState.Success)
                {
                    //emailService.SendConfirmationEmail(subscription.ProductID.Value, billing, sale);
                    emailService.PushConfirmationEmailToQueue(billing.BillingID, sale == null ? null : sale.SaleID);

                    try
                    {
                        new EventService().RegistrationAndConfirmation(null, subscription.ProductID, billing.Email, billing.Zip, billing.Phone, billing.FirstName, billing.LastName, billing.RegistrationID, EventTypeEnum.OrderConfirmation);
                    }
                    catch
                    { }
                }
            }

            return res;
        }

        public BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>> CreateSubscription(int subscriptionID, bool chargeForTrial,
            int? campaignID,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate,
            string internalID, long? registrationID,
            int? paymentTypeID, string creditCard, string cvv, int expMonth, int expYear,
            string description)
        {
            BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>> res = new BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                IList<string> insufficientFields = new List<string>();
                if (string.IsNullOrEmpty(firstName)) insufficientFields.Add("firstName");
                if (string.IsNullOrEmpty(lastName)) insufficientFields.Add("lastName");
                if (string.IsNullOrEmpty(address1)) insufficientFields.Add("address1");
                if (string.IsNullOrEmpty(city)) insufficientFields.Add("city");
                if (string.IsNullOrEmpty(state)) insufficientFields.Add("state");
                if (string.IsNullOrEmpty(zip)) insufficientFields.Add("zip");
                //if (string.IsNullOrEmpty(phone)) insufficientFields.Add("phone");
                //if (string.IsNullOrEmpty(email)) insufficientFields.Add("email");
                //if (string.IsNullOrEmpty(ip)) insufficientFields.Add("ip");
                //if (string.IsNullOrEmpty(internalID)) insufficientFields.Add("internalID");
                if (string.IsNullOrEmpty(creditCard)) insufficientFields.Add("creditCard");
                //if (string.IsNullOrEmpty(cvv)) insufficientFields.Add("cvv");

                if (insufficientFields.Count > 0)
                {
                    res.ErrorMessage = string.Format("Insufficient data, the following fields are required: {0}.", string.Join(", ", insufficientFields.ToArray()));
                }
                else if (expMonth < 1 || expMonth > 12 || expYear < 2000 || expYear > 2100)
                {
                    res.ErrorMessage = string.Format("Invalid expire date.");
                }
                else if (paymentTypeID != null &&
                    paymentTypeID != PaymentTypeEnum.Visa &&
                    paymentTypeID != PaymentTypeEnum.Mastercard &&
                    paymentTypeID != PaymentTypeEnum.AmericanExpress &&
                    paymentTypeID != PaymentTypeEnum.Discover)
                {
                    res.ErrorMessage = string.Format("Invalid payment type.");
                }
                else if (!CreditCard.ValidateDecryptedFormat(creditCard))
                {
                    res.ErrorMessage = string.Format("Invalid Credit Card.");
                }
                else if (!string.IsNullOrEmpty(description) && description.Length > 1000)
                {
                    res.ErrorMessage = string.Format("Field description exceeds max length 1000 characters.");
                }
                else
                {
                    //Check subscription
                    Subscription s = Load<Subscription>(subscriptionID);
                    if (s == null)
                    {
                        res.ErrorMessage = "Plan(" + subscriptionID.ToString() + ") is not installed.";
                    }
                    else
                    {
                        if (campaignID != null)
                        {
                            Campaign cmp = Load<Campaign>(campaignID);
                            if (cmp == null)
                            {
                                campaignID = null;
                            }
                        }

                        BusinessError<Set<Registration, Billing>> billing = CreateOrUpdateBillingByProspectOrInternalID(registrationID, internalID,
                            firstName, lastName, address1, address2, city, state, zip, country,
                            phone, email, campaignID, affiliate, subAffiliate, ip, null,
                            paymentTypeID, creditCard, cvv, expMonth, expYear,
                            null, null, null, null, null);

                        if (billing.State == BusinessErrorState.Error)
                        {
                            res.ErrorMessage = billing.ErrorMessage;
                        }
                        else
                        {
                            try
                            {
                                if (registrationID == null && GetLastBillingByInternalID(internalID) == null)
                                {
                                    //was created new Registration
                                    if (billing.ReturnValue.Value1 != null)
                                    {
                                        new EventService().RegistrationAndConfirmation(null, s.ProductID, email, zip, phone, firstName, lastName, billing.ReturnValue.Value1.RegistrationID, EventTypeEnum.Registration);
                                    }
                                }
                            }
                            catch
                            { }

                            // get or create referer
                            try
                            {
                                Referer ownReferer = null;
                                BusinessError<Referer> createRefererResult = refererService.CreateOrGetRefererFromBilling(billing.ReturnValue.Value2, null, null);
                                if (createRefererResult != null && createRefererResult.State == BusinessErrorState.Success)
                                {
                                    ownReferer = createRefererResult.ReturnValue;
                                }

                                if (ownReferer != null)
                                {
                                    refererService.AddBillingToReferer(ownReferer, billing.ReturnValue.Value2);
                                }
                            }
                            catch
                            { }

                            res = CreateSubscription(billing.ReturnValue.Value2, s, description, chargeForTrial);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        private BusinessError<ChargeHistoryEx> AuthorizeTransaction(Billing billing, int productID, decimal amount)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");

            string chargeBlockReason = null;
            if (IsChargesBlocked(billing, out chargeBlockReason))
            {
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = chargeBlockReason;
                return res;
            }

            BillingSubscription billingSubscription = null;

            bool isTestCase = ((!SaleService.CHARGE) || saleService.IsTestCreditCard(billing.CreditCard));

            AssertigyMID assertigyMid = null;

            //Create subscription in any case
            BillingSubscription existedBillingSubscription = subscriptionService.GetBillingSubscriptionByBilling(billing.BillingID);
            billingSubscription = existedBillingSubscription;
            if (billingSubscription == null)
            {
                billingSubscription = saleService.CreateUpsellFakeBillingSubscriptionByProduct(billing, productID);
            }

            try
            {
                //TODO: Implement dao.BeginTopTransaction() 
                dao.BeginTransaction();

                if (!isTestCase)
                {
                    //If not new Sale(BillingSubscription already exists) try to use same Merchant as for previus Sales
                    Set<NMICompany, AssertigyMID> mid = null;
                    mid = merchantService.ChooseRandomNMIMerchantAccount(productID, billing, amount);

                    if (mid != null)
                    {
                        NMICompany nmiCompany = mid.Value1;
                        assertigyMid = mid.Value2;

                        Currency cur = saleService.GetCurrencyByProduct(productID);

                        Product product = EnsureLoad<Product>(productID);

                        IPaymentGateway paymentGateway = saleService.GetGatewayByMID(assertigyMid);

                        BusinessError<GatewayResult> paymentResult = paymentGateway.AuthOnly(assertigyMid.MID,
                            nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                            billing, product);

                        if (paymentResult.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;

                            if (existedBillingSubscription != null && billingSubscription.StatusTID == BillingSubscriptionStatusEnum.Scrubbed)
                            {
                                billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Active;
                            }
                        }
                        else
                        {
                            dao.RollbackTransaction();

                            //billingSubscription = existedBillingSubscription;
                            if (existedBillingSubscription == null)
                            {
                                billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Scrubbed;
                            }

                            res.State = BusinessErrorState.Error;

                            dao.BeginTransaction();
                        }

                        //if new billingSubscription created and charge failed, set to scrubbed
                        //if new billingSubscription created and charge success and status is scrubbed, set to active
                        dao.Save<BillingSubscription>(billingSubscription);

                        Set<Paygea, ChargeHistoryEx> chargeLog = saleService.ChargeLogging(paymentResult, billing, billingSubscription,
                            null, null, assertigyMid, ChargeTypeEnum.AuthOnly, amount, cur);

                        res.ErrorMessage = chargeLog.Value1.Response;
                        res.ReturnValue = chargeLog.Value2;
                    }
                    else
                    {
                        res.ErrorMessage = "No MID installed";
                    }
                }
                else
                {
                    if (existedBillingSubscription == null)
                    {
                        billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Scrubbed;
                        dao.Save<BillingSubscription>(billingSubscription);
                    }
                    if (billing.FirstName.ToLower() == "success")
                    {
                        res.State = BusinessErrorState.Success;
                        res.ReturnValue = new ChargeHistoryEx()
                        {
                            ChargeHistoryID = 0,
                            ChildMID = "Test",
                            ChargeDate = DateTime.Now,
                            Success = true,
                            Amount = amount,
                            Response = "response=1&responsetext=Test case: SUCCESS&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid=&type=auth&response_code=100"
                        };
                        res.ErrorMessage = "Test case: SUCCESS";
                    }
                    else
                    {
                        res.State = BusinessErrorState.Error;
                        res.ReturnValue = new ChargeHistoryEx()
                        {
                            ChargeHistoryID = 0,
                            ChildMID = "Test",
                            ChargeDate = DateTime.Now,
                            Success = false,
                            Amount = amount,
                            Response = "response=2&responsetext=Test case: ERROR&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid=&type=auth&response_code=200"
                        };
                        res.ErrorMessage = "Test case: ERROR";
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
                res.State = BusinessErrorState.Error;
            }

            return res;
        }

        public BusinessError<ChargeHistoryEx> AuthorizeTransaction(int productID,
        decimal amount,
        string firstName, string lastName,
        string address1, string address2, string city, string state, string zip, string country,
        string phone, string email, string ip, string affiliate, string subAffiliate,
        string internalID, long? registrationID,
        int paymentTypeID, string creditCard, string cvv, int expMonth, int expYear)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                IList<string> insufficientFields = new List<string>();
                if (string.IsNullOrEmpty(firstName)) insufficientFields.Add("firstName");
                if (string.IsNullOrEmpty(lastName)) insufficientFields.Add("lastName");
                if (string.IsNullOrEmpty(address1)) insufficientFields.Add("address1");
                if (string.IsNullOrEmpty(city)) insufficientFields.Add("city");
                if (string.IsNullOrEmpty(state)) insufficientFields.Add("state");
                if (string.IsNullOrEmpty(zip)) insufficientFields.Add("zip");
                //if (string.IsNullOrEmpty(phone)) insufficientFields.Add("phone");
                //if (string.IsNullOrEmpty(email)) insufficientFields.Add("email");
                //if (string.IsNullOrEmpty(ip)) insufficientFields.Add("ip");
                //if (string.IsNullOrEmpty(internalID)) insufficientFields.Add("internalID");
                if (string.IsNullOrEmpty(creditCard)) insufficientFields.Add("creditCard");
                //if (string.IsNullOrEmpty(cvv)) insufficientFields.Add("cvv");

                if (insufficientFields.Count > 0)
                {
                    res.ErrorMessage = string.Format("Insufficient data, the following fields are required: {0}.", string.Join(", ", insufficientFields.ToArray()));
                }
                else if (expMonth < 1 || expMonth > 12 || expYear < 2000 || expYear > 2100)
                {
                    res.ErrorMessage = string.Format("Invalid expire date.");
                }
                else if (amount <= 0M)
                {
                    res.ErrorMessage = string.Format("Invalid amount.");
                }
                else if (paymentTypeID != PaymentTypeEnum.Visa && paymentTypeID != PaymentTypeEnum.Mastercard)
                {
                    res.ErrorMessage = string.Format("Invalid payment type.");
                }
                else if (!CreditCard.ValidateDecryptedFormat(creditCard))
                {
                    res.ErrorMessage = string.Format("Invalid Credit Card.");
                }
                else
                {
                    //Check product
                    Product p = Load<Product>(productID);
                    if (p == null)
                    {
                        res.ErrorMessage = "ProductType(" + productID.ToString() + ") is not installed.";
                    }
                    else
                    {
                        BusinessError<Set<Registration, Billing>> billing = CreateOrUpdateBillingByProspectOrInternalID(registrationID, internalID,
                            firstName, lastName, address1, address2, city, state, zip, country,
                            phone, email, null, affiliate, subAffiliate, ip, null,
                            paymentTypeID, creditCard, cvv, expMonth, expYear,
                            null, null, null, null, null);

                        if (billing.State == BusinessErrorState.Error)
                        {
                            res.ErrorMessage = billing.ErrorMessage;
                        }
                        else
                        {
                            res = AuthorizeTransaction(billing.ReturnValue.Value2, productID, amount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public BusinessError<ChargeHistoryEx> DoCapture(long authChargeHistoryID, decimal? amount)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                if (amount != null && amount.Value <= 0M)
                {
                    res.ErrorMessage = "Invalid amount";
                }
                else
                {
                    ChargeHistoryEx ch = dao.Load<ChargeHistoryEx>(authChargeHistoryID);
                    if (ch == null)
                    {
                        res.ErrorMessage = string.Format("Transaction #{0} was not found", authChargeHistoryID);
                    }
                    else if (ch.ChargeTypeID != ChargeTypeEnum.AuthOnly)
                    {
                        res.ErrorMessage = "Can't apply Capture to this type of transaction";
                    }
                    else
                    {
                        res = DoCapture(ch, null, amount, null, null);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        private BusinessError<ChargeHistoryEx> DoCapture(ChargeHistoryEx authChargeHistory, ProductCode productCode, decimal? amount, decimal? shippingAmount, string description)
        {
            BusinessError<ChargeHistoryEx> res = new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error, "Unrecognized error occurred");

            Upsell upsell = null;
            UpsellSale sale = null;

            BillingSubscription billingSubscription = null;
            Billing billing = null;
            int productID = 0;

            try
            {
                billingSubscription = EnsureLoad<BillingSubscription>(authChargeHistory.BillingSubscriptionID);
                billing = EnsureLoad<Billing>(billingSubscription.BillingID);
                Subscription subscription = EnsureLoad<Subscription>(billingSubscription.SubscriptionID);
                productID = subscription.ProductID.Value;
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.State = BusinessErrorState.Error;
            }

            if (billingSubscription != null && billing != null)
            {
                string chargeBlockReason = null;
                if (IsChargesBlocked(billing, out chargeBlockReason))
                {
                    res.State = BusinessErrorState.Error;
                    res.ErrorMessage = chargeBlockReason;
                    return res;
                }

                bool isTestCase = ((!SaleService.CHARGE) || saleService.IsTestCreditCard(billing.CreditCard));

                try
                {
                    //TODO: Implement dao.BeginTopTransaction() 
                    dao.BeginTransaction();

                    string upsellProductCode = SaleService.BILL_UPSELL_PRODUCT_CODE;
                    if (productCode != null)
                    {
                        upsellProductCode = productCode.ProductCode_;
                    }

                    MySqlCommand q = new MySqlCommand("select * from UpsellType " +
                        "where ProductID = @productID and ProductCode = @productCode and Quantity = 1");
                    q.Parameters.Add("@productID", MySqlDbType.Int32).Value = productID;
                    q.Parameters.Add("@productCode", MySqlDbType.VarChar).Value = upsellProductCode;

                    UpsellType upsellType = dao.Load<UpsellType>(q).LastOrDefault();
                    if (upsellType == null)
                    {
                        upsellType = new UpsellType();
                        upsellType.ProductCode = upsellProductCode;
                        upsellType.Price = 0.00M;
                        upsellType.Quantity = 1;
                        upsellType.DisplayName = string.Empty;
                        upsellType.DropDown = false;
                        upsellType.Description = string.Empty;
                        upsellType.ProductID = productID;
                        dao.Save<UpsellType>(upsellType);
                    }

                    Currency cur = null;
                    AuthOnlyChargeDetails authChargeDetails = EnsureLoad<AuthOnlyChargeDetails>(authChargeHistory.ChargeHistoryID);
                    if (authChargeDetails.RequestedCurrencyID != null)
                    {
                        cur = EnsureLoad<Currency>(authChargeDetails.RequestedCurrencyID);
                    }

                    if (amount == null)
                    {
                        if (authChargeDetails.RequestedCurrencyAmount != null)
                        {
                            amount = authChargeDetails.RequestedCurrencyAmount;
                        }
                        else
                        {
                            amount = authChargeDetails.RequestedAmount;
                        }
                    }

                    if (!isTestCase)
                    {
                        upsell = new Upsell();
                        upsell.UpsellTypeID = upsellType.UpsellTypeID;
                        upsell.BillingID = billing.BillingID;
                        upsell.ProductCode = upsellType.ProductCode;
                        upsell.Quantity = upsellType.Quantity;
                        upsell.CreateDT = DateTime.Now;
                        upsell.Complete = false;
                        dao.Save<Upsell>(upsell);

                        sale = new UpsellSale();
                        sale.SaleTypeID = SaleTypeEnum.Upsell;
                        sale.TrackingNumber = null;
                        sale.CreateDT = DateTime.Now;
                        sale.NotShip = (upsellType.ProductCode == SaleService.BILL_UPSELL_PRODUCT_CODE);
                        sale.UpsellID = upsell.UpsellID;
                        sale.ChargeHistoryID = null;
                        sale.PaygeaID = null;
                        dao.Save<UpsellSale>(sale);

                        if (shippingAmount != null)
                        {
                            amount += shippingAmount.Value;
                        }

                        //If not new Sale(BillingSubscription already exists) try to use same Merchant as for previus Sales
                        AssertigyMID assertigyMid = EnsureLoad<AssertigyMID>(authChargeHistory.MerchantAccountID);
                        NMICompany nmiCompany = merchantService.GetNMICompanyByAssertigyMID(assertigyMid.AssertigyMIDID);

                        if (shippingAmount != null)
                        {
                            SaleChargeDetails saleCD = new SaleChargeDetails();
                            saleCD.SaleID = sale.SaleID;
                            saleCD.SaleChargeTypeID = SaleChargeTypeEnum.Shipping;
                            saleCD.Amount = shippingAmount.Value;
                            if (cur != null)
                            {
                                saleCD.CurrencyID = cur.CurrencyID;
                                saleCD.CurrencyAmount = saleCD.Amount;
                                saleCD.Amount = cur.ConvertToUSD(saleCD.CurrencyAmount.Value);
                            }
                            dao.Save<SaleChargeDetails>(saleCD);
                        }

                        if (!string.IsNullOrEmpty(description))
                        {
                            SaleDetails sd = new SaleDetails();
                            sd.SaleID = sale.SaleID;
                            sd.Description = description;
                            dao.Save<SaleDetails>(sd);
                        }

                        IPaymentGateway paymentGateway = saleService.GetGatewayByMID(assertigyMid);

                        BusinessError<GatewayResult> paymentResult = paymentGateway.Capture(nmiCompany.GatewayUsername, nmiCompany.GatewayPassword,
                            authChargeHistory, amount.Value, cur);

                        if (paymentResult.State == BusinessErrorState.Success)
                        {
                            res.State = BusinessErrorState.Success;

                            if (billingSubscription.StatusTID == BillingSubscriptionStatusEnum.Scrubbed)
                            {
                                billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Active;
                            }
                        }
                        else
                        {
                            dao.RollbackTransaction();

                            upsell = null;
                            sale = null;

                            res.State = BusinessErrorState.Error;

                            dao.BeginTransaction();
                        }

                        //if new billingSubscription created and charge failed, set to scrubbed
                        //if new billingSubscription created and charge success and status is scrubbed, set to active
                        dao.Save<BillingSubscription>(billingSubscription);

                        Set<Paygea, ChargeHistoryEx> chargeLog = saleService.ChargeLogging(paymentResult, billing, billingSubscription,
                            upsellType.ProductCode, SaleTypeEnum.Upsell, assertigyMid, ChargeTypeEnum.Charge, amount.Value, cur);

                        if (sale != null)
                        {
                            sale.PaygeaID = chargeLog.Value1.PaygeaID;
                            sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                            dao.Save<UpsellSale>(sale);
                        }

                        res.ErrorMessage = chargeLog.Value1.Response;
                        res.ReturnValue = chargeLog.Value2;
                    }
                    else
                    {
                        if (shippingAmount != null)
                        {
                            amount += shippingAmount.Value;
                        }
                        if (billing.FirstName.ToLower() == "success")
                        {
                            res.State = BusinessErrorState.Success;
                            res.ReturnValue = new ChargeHistoryEx()
                            {
                                ChargeHistoryID = 0,
                                ChildMID = "Test",
                                ChargeDate = DateTime.Now,
                                Success = true,
                                Amount = amount,
                                Response = "response=1&responsetext=Test case: SUCCESS&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid=&type=sale&response_code=100"
                            };
                            res.ErrorMessage = "Test case: SUCCESS";
                        }
                        else
                        {
                            res.State = BusinessErrorState.Error;
                            res.ReturnValue = new ChargeHistoryEx()
                            {
                                ChargeHistoryID = 0,
                                ChildMID = "Test",
                                ChargeDate = DateTime.Now,
                                Success = false,
                                Amount = amount,
                                Response = "response=2&responsetext=Test case: ERROR&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid=&type=sale&response_code=200"
                            };
                            res.ErrorMessage = "Test case: ERROR";
                        }
                    }
                    dao.CommitTransaction();
                }
                catch (Exception ex)
                {
                    logger.Error(GetType(), ex);
                    dao.RollbackTransaction();
                    res.State = BusinessErrorState.Error;
                }

                if (res != null && res.State == BusinessErrorState.Success)
                {
                    emailService.PushConfirmationEmailToQueue(billing.BillingID, sale == null ? null : sale.SaleID);
                    //emailService.SendConfirmationEmail(productID, billing, sale);
                }
            }

            return res;
        }

        public BusinessError<GatewayResult> DirectGatewayCharge(decimal amount, Currency currency,
            int assertigyMIDID,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip,
            string creditCard, string cvv, int expMonth, int expYear)
        {
            BusinessError<GatewayResult> res = new BusinessError<GatewayResult>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            try
            {
                AssertigyMID assertigyMID = EnsureLoad<AssertigyMID>(assertigyMIDID);
                NMICompany nmiCompany = merchantService.GetNMICompanyByAssertigyMID(assertigyMIDID);

                if (nmiCompany == null)
                {
                    throw new Exception("MID is not configured.");
                }

                IPaymentGateway paymentGateway = saleService.GetGatewayByMID(assertigyMID);

                Billing billing = new Billing();
                billing.FirstName = firstName;
                billing.LastName = lastName;
                billing.Address1 = address1;
                billing.Address2 = address2;
                billing.City = city;
                billing.State = state;
                billing.Zip = zip;
                billing.Country = country;
                billing.Phone = phone;
                billing.Email = email;
                billing.IP = ip;
                billing.CreditCard = creditCard;
                billing.CVV = cvv;
                billing.ExpMonth = expMonth;
                billing.ExpYear = expYear;

                Product product = new Product();
                product.Code = "";
                product.ProductName = "General Products";
                
                res = paymentGateway.Sale(assertigyMID.MID,
                    nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, currency,
                    0, billing, product);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res.State = BusinessErrorState.Error;
                //res.ErrorMessage = ex.ToString();
            }
            if (res == null)
            {
                res = new BusinessError<GatewayResult>(null, BusinessErrorState.Error, "Unrecognized error occurred");
            }
            return res;
        }

        public BillingAPIRequest LogBillingAPIRequest(HttpRequest request, StringBuilder requestString, StringBuilder responseString)
        {
            BillingAPIRequest res = null;
            try
            {
                res = new BillingAPIRequest();
                res.CreateDT = DateTime.Now;
                if (request != null)
                {
                    res.URL = request.Url.ToString();
                    res.Method = request.HttpMethod;
                    res.IP = request.UserHostAddress;

                }
                res.Request = requestString.ToString();
                res.Response = responseString.ToString();

                if (res.Request != null && res.Request.Length > 4000) res.Request = res.Request.Substring(0, 4000);
                if (res.Response != null && res.Response.Length > 4000) res.Response = res.Response.Substring(0, 4000);

                dao.Save<BillingAPIRequest>(res);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Billing> GetBillingListByAffiliate(string affiliate, DateTime startDate, DateTime endDate)
        {
            IList<Billing> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"select * from Billing Where (CreateDT Between @StartDate and @EndDate) and Affiliate=@Affiliate");
                q.CommandTimeout = 9999;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                q.Parameters.Add("@Affiliate", MySqlDbType.VarChar).Value = affiliate;
                res = dao.Load<Billing>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public bool SetAccountToInactive(long? billingID)
        {
            bool res = true;
            try
            {
                dao.BeginTransaction();
                var bsList = subscriptionService.GetBillingSubscriptionsByBilling(billingID);

                foreach (var bs in bsList)
                {
                    if (bs.StatusTID == 1)
                    {
                        bs.StatusTID = 0;
                        dao.Save<BillingSubscription>(bs);
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                res = false;
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public bool IsAccountActive(long? billingID)
        {
            bool res = false;
            try
            {
                var bsList = subscriptionService.GetBillingSubscriptionsByBilling(billingID);
                foreach (var bs in bsList)
                {
                    if (bs.StatusTID == 1)
                        res = true;
                }
            }
            catch (Exception ex)
            {
                res = false;
                logger.Error(GetType(), ex);
            }

            return res;
        }
    }
}
