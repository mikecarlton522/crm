using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using TrimFuel.Business;
using TrimFuel.Business.Dao;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.WebServices.BillingAPI.Model;
using TrimFuel.WebServices.BillingAPI.Logic;
using TrimFuel.WebServices.BillingAPI.Logic.SlickTicket;
using TrimFuel.Model.Views;
using TrimFuel.Model.Containers;
using MySql.Data.MySqlClient;

namespace TrimFuel.WebServices.BillingAPI
{
    /// <summary>
    /// Summary description for billing_ws
    /// </summary>
    [WebService(Namespace = "http://trianglecrm.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class billing_ws : System.Web.Services.WebService
    {
        private SaleService saleService = new SaleService();
        private BillingService billingService = new BillingService();
        private SubscriptionService subscriptionService = new SubscriptionService();
        private RegistrationService registrationService = new RegistrationService();
        private const int PRODUCT_ID = 1;
        private const int TP_MODE_ID = TrimFuel.Model.Enums.TPModeEnum.WebService;

        [WebMethod]
        public BusinessError<ChargeHistory> Void(string username, string password, long chargeHistoryID)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistory>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            BusinessError<ChargeHistoryEx> resEx = saleService.DoVoid(chargeHistoryID);
            return new BusinessError<ChargeHistory>()
            {
                ReturnValue = ChargeHistory.FromChargeHistoryEx(new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                    {
                        Value1 = resEx.ReturnValue
                    }
                ),
                State = resEx.State,
                ErrorMessage = resEx.ErrorMessage
            };
        }

        [WebMethod]
        public BusinessError<ChargeHistory> Refund(string username, string password, long chargeHistoryID, decimal refundAmount)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistory>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            BusinessError<ChargeHistoryEx> resEx = saleService.DoRefund(chargeHistoryID, refundAmount);
            return new BusinessError<ChargeHistory>()
            {
                ReturnValue = ChargeHistory.FromChargeHistoryEx(new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                    {
                        Value1 = resEx.ReturnValue
                    }
                ),
                State = resEx.State,
                ErrorMessage = resEx.ErrorMessage
            };
        }

        [WebMethod]
        public BusinessError<ChargeHistory> RefundSale(string username, string password, long saleID, decimal refundAmount)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistory>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            BusinessError<ChargeHistoryEx> resEx = saleService.DoRefundSale(saleID, refundAmount);
            return new BusinessError<ChargeHistory>()
            {
                ReturnValue = ChargeHistory.FromChargeHistoryEx(new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                {
                    Value1 = resEx.ReturnValue
                }
                ),
                State = resEx.State,
                ErrorMessage = resEx.ErrorMessage
            };
        }

        [WebMethod]
        public BusinessError<Prospect> CreateProspect(string username, string password,
            int productTypeID, [System.Xml.Serialization.XmlIgnore]bool productTypeIDSpecified,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate,
            string internalID,
            string customField1, string customField2, string customField3, string customField4, string customField5)
        {
            try
            {
                BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
                if (authResult.State == BusinessErrorState.Error)
                {
                    return new BusinessError<Prospect>()
                    {
                        ReturnValue = null,
                        State = BusinessErrorState.Error,
                        ErrorMessage = authResult.ErrorMessage
                    };
                }

                int? productGroupID = null;
                if (productTypeIDSpecified)
                {
                    productGroupID = productTypeID;
                }

                BusinessError<Registration> resReg = billingService.CreateProspect(productGroupID,
                    firstName, lastName,
                    address1, address2, city, state, zip, country,
                    phone, email, ip, affiliate, subAffiliate,
                    internalID,
                    customField1, customField2, customField3, customField4, customField5);

                try
                {
                    if (resReg.State == BusinessErrorState.Success)
                    {
                        new EventService().RegistrationAndConfirmation(null, productGroupID, email, zip, phone, firstName, lastName, resReg.ReturnValue.RegistrationID, EventTypeEnum.Registration);
                    }
                }
                catch
                { }

                return new BusinessError<Prospect>()
                {
                    ReturnValue = Prospect.FromRegistration(resReg.ReturnValue),
                    State = resReg.State,
                    ErrorMessage = resReg.ErrorMessage
                };
            }
            catch(Exception ex)
            {
                throw new SoapException();
            }
        }

        [WebMethod]
        public BusinessError<ChargeHistory> Charge(string username, string password, decimal amount, decimal shipping, [System.Xml.Serialization.XmlIgnore]bool shippingSpecified,
            int productTypeID, [System.Xml.Serialization.XmlIgnore]bool productTypeIDSpecified,
            int productID, [System.Xml.Serialization.XmlIgnore]bool productIDSpecified,
            int campaignID, [System.Xml.Serialization.XmlIgnore]bool campaignIDSpecified,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate,
            string internalID,
            long prospectID, [System.Xml.Serialization.XmlIgnore]bool prospectIDSpecified,
            int paymentType, [System.Xml.Serialization.XmlIgnore]bool paymentTypeSpecified,
            string creditCard, string cvv, int expMonth, int expYear,
            string description,
            string customField1, string customField2, string customField3, string customField4, string customField5)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistory>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            decimal? shippingAmount = null;
            if (shippingSpecified)
            {
                shippingAmount = shipping;
            }

            int productGroupID = PRODUCT_ID;
            if (productTypeIDSpecified)
            {
                productGroupID = productTypeID;
            }

            int? productCodeID = null;
            if (productIDSpecified)
            {
                productCodeID = productID;
            }

            long? registrationID = null;
            if (prospectIDSpecified)
            {
                registrationID = prospectID;
            }

            int? billingCampaignID = null;
            if (campaignIDSpecified)
            {
                billingCampaignID = campaignID;
            }

            int? paymentTypeID = paymentType;
            if (!paymentTypeSpecified)
            {
                paymentTypeID = (new CreditCard(creditCard)).TryGetCardType();
            }

            BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> resEx = saleService.BillAsUpsell(productGroupID, productCodeID, amount, shippingAmount,
                billingCampaignID,
                firstName, lastName,
                address1, address2, city, state, zip, country,
                phone, email, ip, affiliate, subAffiliate,
                internalID, registrationID,
                paymentTypeID,
                creditCard, cvv, expMonth, expYear,
                description,
                customField1, customField2, customField3, customField4, customField5);

            if (resEx.ReturnValue != null && resEx.ReturnValue.Value1 != null && resEx.ReturnValue.Value1.BillingSubscriptionID != null)
            {
                var bs = saleService.Load<BillingSubscription>(resEx.ReturnValue.Value1.BillingSubscriptionID);
                if (bs != null)
                    new EmailService().ProcessEmailQueue(bs.BillingID);
            }

            return new BusinessError<ChargeHistory>()
            {
                ReturnValue = ChargeHistory.FromChargeHistoryEx(resEx.ReturnValue),
                State = resEx.State,
                ErrorMessage = resEx.ErrorMessage
            };
        }

        [WebMethod]
        public BusinessError<ChargeHistoryList> ChargeMultipleItems(string username, string password, ItemList itemList,
            int campaignID, [System.Xml.Serialization.XmlIgnore]bool campaignIDSpecified,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate,
            string internalID,
            long prospectID, [System.Xml.Serialization.XmlIgnore]bool prospectIDSpecified,
            int paymentType, [System.Xml.Serialization.XmlIgnore]bool paymentTypeSpecified,
            string creditCard, string cvv, int expMonth, int expYear,
            string description,
            string customField1, string customField2, string customField3, string customField4, string customField5)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistoryList>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            try
            {
                ChargeHistoryList res = new ChargeHistoryList();

                string latestError = "";

                foreach (Item item in itemList)
                {
                    int productGroupID = PRODUCT_ID;
                    if (item.ProductTypeID.HasValue)
                    {
                        productGroupID = item.ProductTypeID.Value;
                    }

                    int? productCodeID = null;
                    if (item.ProductID.HasValue)
                    {
                        productCodeID = item.ProductID.Value;
                    }

                    long? registrationID = null;
                    if (prospectIDSpecified)
                    {
                        registrationID = prospectID;
                    }

                    int? billingCampaignID = null;
                    if (campaignIDSpecified)
                    {
                        billingCampaignID = campaignID;
                    }

                    int? paymentTypeID = paymentType;
                    if (!paymentTypeSpecified)
                    {
                        paymentTypeID = (new CreditCard(creditCard)).TryGetCardType();
                    }

                    BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> resEx = saleService.BillAsUpsell(productGroupID, productCodeID, item.Amount, item.Shipping,
                        billingCampaignID,
                        firstName, lastName,
                        address1, address2, city, state, zip, country,
                        phone, email, ip, affiliate, subAffiliate,
                        internalID, registrationID,
                        paymentTypeID,
                        creditCard, cvv, expMonth, expYear,
                        description,
                        customField1, customField2, customField3, customField4, customField5);

                    if (resEx.ReturnValue != null && resEx.ReturnValue.Value1 != null)
                    {
                        var bs = saleService.Load<BillingSubscription>(resEx.ReturnValue.Value1.BillingSubscriptionID);
                        new EmailService().ProcessEmailQueue(bs.BillingID);

                        res.Add(ChargeHistory.FromChargeHistoryEx(resEx.ReturnValue));
                    }
                    else
                    {
                        res.Add(new ChargeHistory() { Amount = item.Amount + item.Shipping, ChargeHistoryID = 0, Date = DateTime.Now, MID = null, Success = false });

                        latestError = resEx.ErrorMessage;
                    }
                }

                foreach (var ch in res)
                {
                    var chItem = saleService.Load<ChargeHistoryEx>(ch.ChargeHistoryID);
                    if (chItem != null && chItem.BillingSubscriptionID != null)
                    {
                        var bs = saleService.Load<BillingSubscription>(chItem.BillingSubscriptionID);
                        if (bs != null)
                            new EmailService().ProcessEmailQueue(bs.BillingID);
                    }
                }

                return new BusinessError<ChargeHistoryList>()
                {
                    ReturnValue = res,
                    State = string.IsNullOrEmpty(latestError) ? BusinessErrorState.Success : BusinessErrorState.Error,
                    ErrorMessage = latestError
                };
            }
            catch(Exception ex)
            {
                return new BusinessError<ChargeHistoryList>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = ex.Message
                };
            }
        }

        [WebMethod]
        public BusinessError<ChargeHistoryList> ChargeMultipleItemsEx(string username, string password,
            string amount1, string shipping1, string productTypeID1, string productID1,
            string amount2, string shipping2, string productTypeID2, string productID2,
            string amount3, string shipping3, string productTypeID3, string productID3,
            string amount4, string shipping4, string productTypeID4, string productID4,
            string amount5, string shipping5, string productTypeID5, string productID5,
            string campaignID, string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate, string internalID, string prospectID,
            string paymentType, string creditCard, string cvv, string expMonth, string expYear,
            string description, string customField1, string customField2, string customField3, string customField4, string customField5
            )
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistoryList>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            try
            {
                ChargeHistoryList res = new ChargeHistoryList();

                string latestError = "";

                Billing billing = null;

                ItemList itemList = new ItemList();

                itemList.Add(new Item() { 
                    Amount = Convert.ToDecimal(amount1), 
                    Shipping = Convert.ToDecimal(shipping1), 
                    ProductTypeID = Convert.ToInt32(productTypeID1), 
                    ProductID = Convert.ToInt32(productID1) });

                if(!string.IsNullOrEmpty(amount2) && !string.IsNullOrEmpty(shipping2) && !string.IsNullOrEmpty(productTypeID2) && !string.IsNullOrEmpty(productID2))
                    itemList.Add(new Item() { Amount = Convert.ToDecimal(amount2), Shipping = Convert.ToDecimal(shipping2), ProductTypeID = Convert.ToInt32(productTypeID2), ProductID = Convert.ToInt32(productID2) });

                if (!string.IsNullOrEmpty(amount3) && !string.IsNullOrEmpty(shipping3) && !string.IsNullOrEmpty(productTypeID3) && !string.IsNullOrEmpty(productID3))
                    itemList.Add(new Item() { Amount = Convert.ToDecimal(amount3), Shipping = Convert.ToDecimal(shipping3), ProductTypeID = Convert.ToInt32(productTypeID3), ProductID = Convert.ToInt32(productID3) });

                if (!string.IsNullOrEmpty(amount4) && !string.IsNullOrEmpty(shipping4) && !string.IsNullOrEmpty(productTypeID4) && !string.IsNullOrEmpty(productID4))
                    itemList.Add(new Item() { Amount = Convert.ToDecimal(amount4), Shipping = Convert.ToDecimal(shipping4), ProductTypeID = Convert.ToInt32(productTypeID4), ProductID = Convert.ToInt32(productID4) });

                if (!string.IsNullOrEmpty(amount5) && !string.IsNullOrEmpty(shipping5) && !string.IsNullOrEmpty(productTypeID5) && !string.IsNullOrEmpty(productID5))
                    itemList.Add(new Item() { Amount = Convert.ToDecimal(amount5), Shipping = Convert.ToDecimal(shipping5), ProductTypeID = Convert.ToInt32(productTypeID5), ProductID = Convert.ToInt32(productID5) });

                foreach (Item item in itemList)
                {
                    int productGroupID = PRODUCT_ID;
                    if (item.ProductTypeID.HasValue && item.ProductTypeID > -1)
                    {
                        productGroupID = item.ProductTypeID.Value;
                    }

                    int? productCodeID = null;
                    if (item.ProductID.HasValue && item.ProductID > -1)
                    {
                        productCodeID = item.ProductID.Value;
                    }

                    long? registrationID = null;
                    if (!string.IsNullOrEmpty(prospectID))
                    {
                        registrationID = Convert.ToInt32(prospectID);
                    }

                    if (billing != null)
                        registrationID = billing.RegistrationID;

                    int? billingCampaignID = null;
                    if (!string.IsNullOrEmpty(campaignID))
                    {
                        billingCampaignID = Convert.ToInt32(campaignID);
                    }

                    BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> resEx = saleService.BillAsUpsell(productGroupID, productCodeID, item.Amount, item.Shipping,
                        billingCampaignID,
                        firstName, lastName,
                        address1, address2, city, state, zip, country,
                        phone, email, ip, affiliate, subAffiliate,
                        internalID, registrationID,
                        Convert.ToInt32(paymentType),
                        creditCard, cvv, Convert.ToInt32(expMonth), Convert.ToInt32(expYear),
                        description,
                        customField1, customField2, customField3, customField4, customField5);

                    if (resEx.ReturnValue != null && resEx.ReturnValue.Value2 != null)
                    {
                        billing = saleService.Load<Billing>(resEx.ReturnValue.Value2.BillingID);
                    }                    

                    if (resEx.ReturnValue != null && resEx.ReturnValue.Value1 != null)
                    {
                        var bs = saleService.Load<BillingSubscription>(resEx.ReturnValue.Value1.BillingSubscriptionID);
                        new EmailService().ProcessEmailQueue(bs.BillingID);

                        res.Add(ChargeHistory.FromChargeHistoryEx(resEx.ReturnValue));
                    }
                    else
                    {
                        res.Add(new ChargeHistory() { Amount = item.Amount + item.Shipping, ChargeHistoryID = 0, Date = DateTime.Now, MID = null, Success = false });

                        latestError = resEx.ErrorMessage;
                    }
                }

                foreach (var ch in res)
                {
                    var chItem = saleService.Load<ChargeHistoryEx>(ch.ChargeHistoryID);
                    if (chItem != null && chItem.BillingSubscriptionID != null)
                    {
                        var bs = saleService.Load<BillingSubscription>(chItem.BillingSubscriptionID);
                        if (bs != null)
                            new EmailService().ProcessEmailQueue(bs.BillingID);
                    }
                }

                return new BusinessError<ChargeHistoryList>()
                {
                    ReturnValue = res,
                    State = string.IsNullOrEmpty(latestError) ? BusinessErrorState.Success : BusinessErrorState.Error,
                    ErrorMessage = latestError
                };
            }
            catch (Exception ex)
            {
                return new BusinessError<ChargeHistoryList>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = ex.Message
                };
            }
        }

        [WebMethod]
        public BusinessError<ChargeHistorySales> ChargeSales(string username, string password,
            int productTypeID, [System.Xml.Serialization.XmlIgnore]bool productTypeIDSpecified,
            ProductList productList,
            int campaignID, [System.Xml.Serialization.XmlIgnore]bool campaignIDSpecified,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate,
            string internalID,
            long prospectID, [System.Xml.Serialization.XmlIgnore]bool prospectIDSpecified,
            int paymentType, [System.Xml.Serialization.XmlIgnore]bool paymentTypeSpecified,
            string creditCard, string cvv, int expMonth, int expYear,
            string customField1, string customField2, string customField3, string customField4, string customField5)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistorySales>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            int productGroupID = PRODUCT_ID;
            if (productTypeIDSpecified)
            {
                productGroupID = productTypeID;
            }

            long? registrationID = null;
            if (prospectIDSpecified)
            {
                registrationID = prospectID;
            }

            int? billingCampaignID = null;
            if (campaignIDSpecified)
            {
                billingCampaignID = campaignID;
            }

            int? paymentTypeID = paymentType;
            if (!paymentTypeSpecified)
            {
                paymentTypeID = (new CreditCard(creditCard)).TryGetCardType();
            }

            IList<KeyValuePair<int, decimal>> productCodeList = null;
            if (productList != null)
            {
                productCodeList = new List<KeyValuePair<int, decimal>>();
                foreach (var item in productList)
	            {
                    productCodeList.Add(new KeyValuePair<int,decimal>(item.ProductID, item.Amount));
	            }
            }

            BusinessError<ChargeHistoryWithSalesView> resEx = saleService.BillAsUpsell(productGroupID, productCodeList,
                billingCampaignID,
                firstName, lastName,
                address1, address2, city, state, zip, country,
                phone, email, ip, affiliate, subAffiliate,
                internalID, registrationID,
                paymentTypeID,
                creditCard, cvv, expMonth, expYear,
                customField1, customField2, customField3, customField4, customField5);

            if (resEx.ReturnValue != null && resEx.ReturnValue.ChargeHistoryView != null 
                        && resEx.ReturnValue.ChargeHistoryView.Value1 != null
                        && resEx.ReturnValue.ChargeHistoryView.Value1.BillingSubscriptionID != null)
            {
                var bs = saleService.Load<BillingSubscription>(resEx.ReturnValue.ChargeHistoryView.Value1.BillingSubscriptionID);
                if (bs != null)
                    new EmailService().ProcessEmailQueue(bs.BillingID);
            }

            return new BusinessError<ChargeHistorySales>()
            {
                ReturnValue = ChargeHistorySales.FromChargeWithSales(resEx.ReturnValue),
                State = resEx.State,
                ErrorMessage = resEx.ErrorMessage
            };
        }

        [WebMethod]
        public BusinessError<ChargeHistory> ChargeExistingCustomer(string username, string password, 
            int prospectID, [XmlIgnore]bool prospectIDSpecified,
            int billingID, [XmlIgnore]bool billingIDSpecified,
            int productID, int productTypeID, decimal amount)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                if (authResult.State == BusinessErrorState.Error)
                {
                    return new BusinessError<ChargeHistory>()
                    {
                        ReturnValue = null,
                        State = BusinessErrorState.Error,
                        ErrorMessage = authResult.ErrorMessage
                    };
                }
            }

            if (billingIDSpecified)
            {
                var resEx = saleService.BillAsUpsell(billingID, productID, productTypeID, amount);

                new EmailService().ProcessEmailQueue(billingID);

                return new BusinessError<ChargeHistory>()
                {
                    ReturnValue = ChargeHistory.FromChargeHistoryEx(resEx.ReturnValue),
                    State = resEx.State,
                    ErrorMessage = resEx.ErrorMessage
                };
            }
            else if (prospectIDSpecified)
            {
                Billing billing = billingService.GetLastBillingByProspectID(prospectID);

                if (billing != null)
                {
                    var resEx = saleService.BillAsUpsell((int)billing.BillingID, productID, productTypeID, amount);

                    new EmailService().ProcessEmailQueue(billing.BillingID);

                    return new BusinessError<ChargeHistory>()
                    {
                        ReturnValue = ChargeHistory.FromChargeHistoryEx(resEx.ReturnValue),
                        State = resEx.State,
                        ErrorMessage = resEx.ErrorMessage
                    };
                }
                else
                {
                    return new BusinessError<ChargeHistory>()
                    {
                        ReturnValue = null,
                        State = BusinessErrorState.Error,
                        ErrorMessage = "Please specify a valid BillingID or ProspectID"
                    };
                }
            }
            else
            {
                return new BusinessError<ChargeHistory>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "Please specify BillingID or ProspectID"
                };
            }
        }

        [WebMethod]
        public BusinessError<PlanResult> CreateSubscription(string username, string password,
            int planID, bool chargeForTrial,
            int campaignID, [System.Xml.Serialization.XmlIgnore]bool campaignIDSpecified,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate,
            string internalID,
            long prospectID, [System.Xml.Serialization.XmlIgnore]bool prospectIDSpecified,
            int paymentType, [System.Xml.Serialization.XmlIgnore]bool paymentTypeSpecified,
            string creditCard, string cvv, int expMonth, int expYear,
            string description)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<PlanResult>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            long? registrationID = null;
            if (prospectIDSpecified)
            {
                registrationID = prospectID;
            }

            int? billingCampaignID = null;
            if (campaignIDSpecified)
            {
                billingCampaignID = campaignID;
            }

            int? paymentTypeID = paymentType;
            if (!paymentTypeSpecified)
            {
                paymentTypeID = (new CreditCard(creditCard)).TryGetCardType();
            }

            BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>> resEx = billingService.CreateSubscription(planID, chargeForTrial,
                billingCampaignID,
                firstName, lastName,
                address1, address2, city, state, zip, country,
                phone, email, ip, affiliate, subAffiliate,
                internalID, registrationID,
                paymentTypeID,
                creditCard, cvv, expMonth, expYear,
                description);

            return new BusinessError<PlanResult>()
            {
                ReturnValue = PlanResult.FromBillingSubscriptionAndChargeHistoryEx(resEx.ReturnValue),
                State = resEx.State,
                ErrorMessage = resEx.ErrorMessage
            };
        }

        [WebMethod]
        public BusinessError<int> CancelSubscription(string username, string password,
            long billingID, [System.Xml.Serialization.XmlIgnore]bool billingIDSpecified, string phone, string email)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<int>()
                {
                    ReturnValue = 0,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            if (!billingIDSpecified)
            {
                Billing billing = billingService.GetLastBillingByPhoneOrEmail(phone, email);

                if (billing != null)
                {
                    billingID = (long)billing.BillingID;

                    billingIDSpecified = true;
                }
            }

            if (!billingIDSpecified)
            {
                return new BusinessError<int>()
                {
                    ReturnValue = 0,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "No customer found."
                };
            }
            else
            {
                IList<BillingSubscription> billingSubscriptions = subscriptionService.GetBillingSubscriptionsByBilling(billingID);

                if (billingSubscriptions.Count() > 0)
                {
                    foreach (BillingSubscription billingSubscription in billingSubscriptions)
                    {
                        subscriptionService.CancelSubscription(billingSubscription);
                    }

                    Notes note = new Notes
                    {
                        AdminID = 0,
                        BillingID = (int)billingID,
                        Content = "Account status changed to inactive: API call",
                        CreateDT = DateTime.Now
                    };

                    billingService.Save<Notes>(note);

                    return new BusinessError<int>()
                    {
                        ReturnValue = billingSubscriptions.Count(),
                        State = BusinessErrorState.Success,
                        ErrorMessage = string.Empty
                    };
                }
                else
                {
                    return new BusinessError<int>()
                    {
                        ReturnValue = 0,
                        State = BusinessErrorState.Error,
                        ErrorMessage = "No subscription found."
                    };
                }
            }
        }

        [WebMethod]
        public BusinessError<ChargeHistory> Rebill(string username, string password, decimal amount,
            string internalID)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistory>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> resEx = saleService.BillAsUpsell(PRODUCT_ID, amount,
                internalID);
            return new BusinessError<ChargeHistory>()
            {
                ReturnValue = ChargeHistory.FromChargeHistoryEx(resEx.ReturnValue),
                State = resEx.State,
                ErrorMessage = resEx.ErrorMessage
            };
        }

        [WebMethod]
        public BusinessError<ChargeHistory> ChargeLookup(string username, string password, long chargeHistoryID)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistory>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            BusinessError<ChargeHistoryEx> resEx = saleService.GetChargeHistoryByID(chargeHistoryID);

            if (resEx.State == BusinessErrorState.Success)
            {
                BusinessError<ChargeHistory> res = new BusinessError<ChargeHistory>();

                res.ReturnValue = ChargeHistory.FromChargeHistoryEx(new Set<ChargeHistoryEx, FailedChargeHistoryView>() { Value1 = resEx.ReturnValue });
                res.ReturnValue.SaleID = saleService.GetSaleIDByChargeHistoryID(chargeHistoryID);
                res.State = BusinessErrorState.Success;
                res.ErrorMessage = null;

                return res;
            }
            else
            {
                return new BusinessError<ChargeHistory>()
                {
                    ReturnValue = ChargeHistory.FromChargeHistoryEx(new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                        {
                            Value1 = resEx.ReturnValue
                        }),
                    State = resEx.State,
                    ErrorMessage = resEx.ErrorMessage
                };
            }
        }

        [WebMethod]
        public BusinessError<ChargeHistoryList> UserLookup(string username, string password, string internalID)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistoryList>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            IList<ChargeHistoryEx> resEx = saleService.GetChargeHistoryByInternalID(internalID);
            if (resEx == null)
            {
                return new BusinessError<ChargeHistoryList>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "Unrecognized error occurred"
                };
            }

            ChargeHistoryList res = new ChargeHistoryList();
            foreach (ChargeHistoryEx ch in resEx)
            {
                res.Add(ChargeHistory.FromChargeHistoryEx(new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                    {
                        Value1 = ch
                    }
                ));
            }
            return new BusinessError<ChargeHistoryList>()
            {
                ReturnValue = res,
                State = BusinessErrorState.Success,
                ErrorMessage = string.Empty
            };
        }

        [WebMethod]
        public BusinessError<UserInfoList> GetCustomerDetail(string username, string password, string phoneNumber, long prospectID)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<UserInfoList>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            Registration registration = null;
            IList<Registration> registrations = new List<Registration>();;

            if (prospectID > 0)
                registration = registrationService.Load<Registration>(prospectID);

            if (registration == null)
            {
                Regex regex = new Regex(@"^\d{10}$");                

                if (regex.IsMatch(phoneNumber))
                    registrations = registrationService.GetRegistrationsByPhoneNumber(phoneNumber);

                else
                {
                    return new BusinessError<UserInfoList>()
                    {
                        ReturnValue = null,
                        State = BusinessErrorState.Error,
                        ErrorMessage = string.Format("\"{0}\" is not a valid phonenumber.", phoneNumber)
                    };
                }
            }

            if (registration == null && registrations.Count <= 0)
            {
                return new BusinessError<UserInfoList>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = string.Format("Prospect(ID:{0}, Phone:{1}) could not be found.", prospectID, phoneNumber)
                };
            }

            if (registration != null)
                registrations.Add(registration);

            UserInfoList userInfoList = new UserInfoList();

            foreach (Registration reg in registrations)
            {
                Billing billing = billingService.GetLastBillingByProspectID((long)reg.RegistrationID);

                IList<ChargeHistoryEx> chargeHistoryEx = saleService.GetChargeHistoryByProspectID((long)reg.RegistrationID);

                ChargeHistoryList chargeHistoryList = new ChargeHistoryList();
                if (chargeHistoryEx != null)
                {
                    foreach (ChargeHistoryEx charge in chargeHistoryEx)
                    {
                        chargeHistoryList.Add(ChargeHistory.FromChargeHistoryEx(new Set<ChargeHistoryEx, FailedChargeHistoryView>()
                        {
                            Value1 = charge
                        }));
                    }
                }

                UserInfo userInfo = new UserInfo();
                userInfo.Shipping = Shipping.FromRegistration(reg);
                userInfo.Billing = BillingInfo.FromBilling(billing);
                userInfo.ChargeHistoryList = chargeHistoryList;

                userInfoList.Add(userInfo);
            }

            return new BusinessError<UserInfoList>()
            {
                ReturnValue = userInfoList,
                State = BusinessErrorState.Success,
                ErrorMessage = string.Empty
            };
        }

        [WebMethod]
        public BusinessError<BillingSubscriptionList> ActivePlanLookup(string username, string password, string phone, string email)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<BillingSubscriptionList>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            Billing billing = billingService.GetLastBillingByPhoneOrEmail(phone, email);

            if (billing == null)
            {
                return new BusinessError<BillingSubscriptionList>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "No customer found."
                };
            }
            else
            {
                BillingSubscriptionList res = new BillingSubscriptionList();

                IList<BillingSubscription> billingSubscriptions = subscriptionService.GetBillingSubscriptionsByBilling(billing.BillingID);

                foreach (BillingSubscription billingSubscription in billingSubscriptions)
                {
                    if (billingSubscription.StatusTID == BillingSubscriptionStatusEnum.Active)
                    {
                        res.Add(billingSubscription);
                    }
                }

                return new BusinessError<BillingSubscriptionList>()
                {
                    ReturnValue = res,
                    State = BusinessErrorState.Success,
                    ErrorMessage = string.Empty
                };
            }

        }

        [WebMethod]
        public BusinessError<Notes> AddNote(string username, string password,
            long prospectID, [System.Xml.Serialization.XmlIgnore]bool prospectIDSpecified,
            string internalID,
            string content)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<Notes>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            Billing billing = null;

            if (prospectIDSpecified)
                billing = billingService.GetLastBillingByProspectID(prospectID);

            if (billing == null && !string.IsNullOrEmpty(internalID))
                billing = billingService.GetLastBillingByInternalID(internalID);

            if (billing != null)
            {
                Notes note = new Notes
                {
                    AdminID = 0,
                    BillingID = (int)billing.BillingID,
                    Content = content,
                    CreateDT = DateTime.Now
                };

                if (billingService.Save<Notes>(note))
                {
                    return new BusinessError<Notes>()
                    {
                        ReturnValue = note,
                        State = BusinessErrorState.Success,
                        ErrorMessage = string.Empty
                    };
                }
                else
                {
                    return new BusinessError<Notes>()
                    {
                        ReturnValue = null,
                        State = BusinessErrorState.Error,
                        ErrorMessage = "Note could not be saved."
                    };
                }
            }
            else
            {
                return new BusinessError<Notes>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "Invalid parameter: billingID."
                };
            }
        }

        [WebMethod]
        public BusinessError<Billing> UpdateBillingInfo(string username, string password,
            long prospectID, [System.Xml.Serialization.XmlIgnore]bool prospectIDSpecified,
            string internalID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip,
            string phone, string email,
            int paymentType, [System.Xml.Serialization.XmlIgnore]bool paymentTypeSpecified,
            string creditCard, string cvv, int expMonth, [System.Xml.Serialization.XmlIgnore]bool expMonthSpecified, int expYear, [System.Xml.Serialization.XmlIgnore]bool expYearSpecified)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<Billing>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            Billing billing = null;

            if (prospectIDSpecified)
                billing = billingService.GetLastBillingByProspectID(prospectID);

            if (billing == null && !string.IsNullOrEmpty(internalID))
                billing = billingService.GetLastBillingByInternalID(internalID);

            if (billing == null)
            {
                return new BusinessError<Billing>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "Customer could not be found."
                };
            }
            else
            {
                billing.FirstName = string.IsNullOrEmpty(firstName) ? billing.FirstName : firstName;
                billing.LastName = string.IsNullOrEmpty(lastName) ? billing.LastName : lastName;
                billing.Address1 = string.IsNullOrEmpty(address1) ? billing.Address1 : address1;
                billing.Address2 = string.IsNullOrEmpty(address2) ? billing.Address2 : address2;
                billing.City = string.IsNullOrEmpty(city) ? billing.City : city;
                billing.State = string.IsNullOrEmpty(state) ? billing.State : state;
                billing.Zip = string.IsNullOrEmpty(zip) ? billing.Zip : zip;
                billing.Phone = string.IsNullOrEmpty(phone) ? billing.Phone : phone;
                billing.Email = string.IsNullOrEmpty(email) ? billing.Email : email;
                billing.PaymentTypeID = paymentTypeSpecified ? paymentType : billing.PaymentTypeID;
                billing.CreditCard = string.IsNullOrEmpty(creditCard) ? billing.CreditCard : creditCard;
                billing.CVV = string.IsNullOrEmpty(cvv) ? billing.CVV : cvv;
                billing.ExpMonth = expMonthSpecified ? expMonth : billing.ExpMonth;
                billing.ExpYear = expYearSpecified ? expYear : billing.ExpYear;

                if (billingService.Save<Billing>(billing))
                {
                    return new BusinessError<Billing>()
                    {
                        ReturnValue = billing,
                        State = BusinessErrorState.Success,
                        ErrorMessage = string.Empty
                    };
                }

                return new BusinessError<Billing>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "A problem occured while saving data."
                };
            }
        }

        [WebMethod]
        public BusinessError<Prospect> UpdateShippingInfo(string username, string password,
            long prospectID, [System.Xml.Serialization.XmlIgnore]bool prospectIDSpecified,
            string internalID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip,
            string phone, string email)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<Prospect>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            Registration registration = null;

            //try to get Registration by prospectID
            if (prospectIDSpecified)
                registration = registrationService.Load<Registration>(prospectID);

            //try to get Registration by internalID
            if (registration == null && !string.IsNullOrEmpty(internalID))
            {
                Billing billing = billingService.GetLastBillingByInternalID(internalID);

                if (billing != null)
                    registration = billingService.Load<Registration>(billing.RegistrationID);
            }

            if (registration == null)
            {
                return new BusinessError<Prospect>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "Customer could not be found."
                };
            }
            else
            {
                registration.FirstName = string.IsNullOrEmpty(firstName) ? registration.FirstName : firstName;
                registration.LastName = string.IsNullOrEmpty(lastName) ? registration.LastName : lastName;
                registration.Address1 = string.IsNullOrEmpty(address1) ? registration.Address1 : address1;
                registration.Address2 = string.IsNullOrEmpty(address2) ? registration.Address2 : address2;
                registration.City = string.IsNullOrEmpty(city) ? registration.City : city;
                registration.State = string.IsNullOrEmpty(state) ? registration.State : state;
                registration.Zip = string.IsNullOrEmpty(zip) ? registration.Zip : zip;
                registration.Phone = string.IsNullOrEmpty(phone) ? registration.Phone : phone;
                registration.Email = string.IsNullOrEmpty(email) ? registration.Email : email;

                if (registrationService.Save<Registration>(registration))
                {
                    return new BusinessError<Prospect>()
                    {
                        ReturnValue = Prospect.FromRegistration(registration),
                        State = BusinessErrorState.Success,
                        ErrorMessage = string.Empty
                    };
                }

                return new BusinessError<Prospect>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "A problem occured while saving data."
                };
            }






        }

        [WebMethod]
        public BusinessError<Ticket> CreateTicket(string username, string password,
            int toAdminID, int fromAdminID, string subject, string content, int priority,
            long billingID, [System.Xml.Serialization.XmlIgnore]bool billingIDSpecified)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<Ticket>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            if (priority > 4 || priority < 1)
            {
                return new BusinessError<Ticket>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "Priority must be one of these values: 1 (Low), 2 (Medium), 3 (High) or 4 (Urgent)."
                };
            }

            BaseService dbService = new BaseService();
            SlickTicketService stService = new SlickTicketService();

            Admin toAdmin;
            Admin fromAdmin;
            User toUser;
            User fromUser;

            BusinessError<Ticket> res = new BusinessError<Ticket>();

            try
            {

                toAdmin = dbService.Load<Admin>(toAdminID);
                if (toAdmin != null)
                {
                    toUser = stService.GetUserByName(toAdmin.DisplayName);
                    if (toUser == null)
                    {
                        return new BusinessError<Ticket>()
                        {
                            ReturnValue = null,
                            State = BusinessErrorState.Error,
                            ErrorMessage = string.Format("Admin({0}) could not be found.", toAdminID)
                        };
                    }
                }
                else
                {
                    return new BusinessError<Ticket>()
                    {
                        ReturnValue = null,
                        State = BusinessErrorState.Error,
                        ErrorMessage = string.Format("Admin({0}) could not be found.", toAdminID)
                    };
                }

                fromAdmin = dbService.Load<Admin>(fromAdminID);
                if (fromAdmin != null)
                {
                    fromUser = stService.GetUserByName(fromAdmin.DisplayName);
                    if (fromUser == null)
                    {
                        return new BusinessError<Ticket>()
                        {
                            ReturnValue = null,
                            State = BusinessErrorState.Error,
                            ErrorMessage = string.Format("Admin({0}) could not be found.", fromAdminID)
                        };
                    }
                }
                else
                {
                    return new BusinessError<Ticket>()
                    {
                        ReturnValue = null,
                        State = BusinessErrorState.Error,
                        ErrorMessage = string.Format("Admin({0}) could not be found.", fromAdminID)
                    };
                }
                
                try
                {
                    stService.CreateTicket(subject, content, toUser.SubUnit, priority, fromUser.UserID, fromUser.SubUnit, billingID);

                    Ticket resValue = new Ticket()
                    {
                        Body = content,
                        Subject = subject
                    };

                    res.ReturnValue = resValue;
                    res.State = BusinessErrorState.Success;
                    res.ErrorMessage = null;
                }
                catch
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = string.Format(ex.Message);
            }

            return res;
        }

        [WebMethod]
        public BusinessError<bool> SendFreeItem(string username, string password,
            long prospectID, int productID, int quantity)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<bool>()
                {
                    ReturnValue = false,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            Billing billing = billingService.GetLastBillingByProspectID(prospectID);

            if (quantity <= 0)
            {
                return new BusinessError<bool>()
                {
                    ReturnValue = false,
                    State = BusinessErrorState.Error,
                    ErrorMessage = "Quantity must be at least 1"
                };
            }

            if (billing == null)
            {
                return new BusinessError<bool>()
                {
                    ReturnValue = false,
                    State = BusinessErrorState.Error,
                    ErrorMessage = string.Format("Prospect({0}) was not found", prospectID)
                };
            }

            Product product = (new ProductService()).GetProduct(productID);

            if (product == null)
            {
                return new BusinessError<bool>()
                {
                    ReturnValue = false,
                    State = BusinessErrorState.Error,
                    ErrorMessage = string.Format("Product({0}) was not found", productID)
                };
            }

            try
            {
                saleService.CreateExtraTrialShipSale(billing, product.Code, quantity);

                (new GeneralShipperService()).SendExtraTrialShips((long)billing.BillingID);

                return new BusinessError<bool>()
                {
                    ReturnValue = true,
                    State = BusinessErrorState.Success,
                    ErrorMessage = null
                };
            }
            catch (Exception ex)
            {
                return new BusinessError<bool>()
                {
                    ReturnValue = false,
                    State = BusinessErrorState.Error,
                    ErrorMessage = ex.Message
                };
            }
        }
    }
}
