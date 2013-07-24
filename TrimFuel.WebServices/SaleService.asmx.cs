using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace TrimFuel.WebServices
{
    /// <summary>
    /// Summary description for SaleService_
    /// </summary>
    [WebService(Namespace = "https://www.securetrialoffers.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class SaleService_ : System.Web.Services.WebService
    {
        [WebMethod]
        public long? CreateRegistration(int? campaignID, string firstName, string lastName, string address1,
            string address2, string city, string state, string zip, string country, string email, string phone,
            DateTime? createDT, string affiliate, string subAffiliate, string ip, string url)
        {
            TrimFuel.Business.RegistrationService s = new TrimFuel.Business.RegistrationService();

            Registration r = s.CreateRegistration(campaignID, firstName, lastName, address1,
                address2, city, state, zip, country, email, phone,
                createDT, affiliate, subAffiliate, ip, url);

            if (r != null)
            {
                return r.RegistrationID;
            }

            return null;
        }

        [WebMethod]
        public bool UpdateRegistration(long registrationID, int? campaignID, string firstName, string lastName, string address1,
            string address2, string city, string state, string zip, string country, string email, string phone,
            DateTime? createDT, string affiliate, string subAffiliate, string ip, string url)
        {
            TrimFuel.Business.RegistrationService s = new TrimFuel.Business.RegistrationService();

            return s.UpdateRegistration(registrationID, campaignID, firstName, lastName, address1,
                address2, city, state, zip, country, email, phone,
                createDT, affiliate, subAffiliate, ip, url);
        }

        [WebMethod]
        public long? CreateBilling(int? campaignID, long? registrationID, string firstName,
            string lastName, string creditCard, string cvv, int? paymentTypeID, int? expMonth,
            int? expYear, string address1, string address2, string city, string state, string zip,
            string country, string email, string phone, DateTime? createDT, string affiliate,
            string subAffiliate, string ip, string url)
        {
            TrimFuel.Business.RegistrationService s = new TrimFuel.Business.RegistrationService();

            Billing b = s.CreateBilling(campaignID, registrationID, firstName,
                lastName, creditCard, cvv, paymentTypeID, expMonth,
                expYear, address1, address2, city, state, zip,
                country, email, phone, createDT, affiliate,
                subAffiliate, ip, url);

            if (b != null)
            {
                return b.BillingID;
            }

            return null;
        }

        [WebMethod]
        public bool UpdateBilling(long billingID, int? campaignID, long? registrationID, string firstName,
            string lastName, string creditCard, string cvv, int? paymentTypeID, int? expMonth,
            int? expYear, string address1, string address2, string city, string state, string zip,
            string country, string email, string phone, DateTime? createDT, string affiliate,
            string subAffiliate, string ip, string url)
        {
            TrimFuel.Business.RegistrationService s = new TrimFuel.Business.RegistrationService();

            return s.UpdateBilling(billingID, campaignID, registrationID, firstName,
                lastName, creditCard, cvv, paymentTypeID, expMonth,
                expYear, address1, address2, city, state, zip,
                country, email, phone, createDT, affiliate,
                subAffiliate, ip, url);
        }

        [WebMethod]
        public int? AssignPlan(int? billingSubscriptionID, long billingID, int subscriptionID)
        {
            BillingSubscription bs = (new SubscriptionService()).
                AssignPlan(billingSubscriptionID, billingID, subscriptionID);

            if (bs != null)
            {
                return bs.BillingSubscriptionID;
            }

            return null;
        }

        [WebMethod]
        public BusinessError<bool> DoRefund(long billingID)
        {
            return (new SaleService()).ProcessRefundRequest(billingID);
        }

        [WebMethod]
        public bool RemoveRefundRequest(long billingID)
        {
            return (new SaleService()).RemoveRefundRequest(billingID);
        }

        [WebMethod]
        public BusinessError<Set<Registration, Billing>> CreateBillingSale(long? billingID, long? registrationID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            string partnerClickClickID, bool isSpecialOffer, string couponCode, string refererCode,
            int subscriptionID)
        {
            return (new SaleService()).CreateBillingSale(billingID, registrationID,
                firstName, lastName, address1, address2, city, state, zip, country,
                phone, email,
                paymentTypeID, creditCard, cvv, expMonth, expYear,
                campaignID, affiliate, subAffiliate, ip, url,
                partnerClickClickID, isSpecialOffer, couponCode, refererCode,
                subscriptionID);
        }

        [WebMethod]
        public BusinessError<Set<Registration, Billing>> CreateUpsellSale(long? billingID, long? registrationID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            string partnerClickClickID, string couponCode, string refererCode,
            int upsellTypeID, int quantity)
        {
            return (new SaleService()).CreateUpsellSale(billingID, registrationID,
                firstName, lastName, address1, address2, city, state, zip, country,
                phone, email,
                paymentTypeID, creditCard, cvv, expMonth, expYear,
                campaignID, affiliate, subAffiliate, ip, url,
                partnerClickClickID, couponCode, refererCode,
                upsellTypeID, quantity);
        }

        [WebMethod]
        public BusinessError<ComplexSaleView> CreateComplexSale(int? ownRefererID,
            string firstName, string lastName, string address1, string address2, string city, string state, string zip, string country,
            string phone, string email,
            int? paymentTypeID, string creditCard, string cvv, int? expMonth, int? expYear,
            int? campaignID, string affiliate, string subAffiliate, string ip, string url,
            string partnerClickClickID, bool isSpecialOffer, string couponCode, string refererCode,
            List<KeyValuePair<int, int>> subscriptions, List<KeyValuePair<int, int>> upsellTypes)
        {
            return (new SaleService()).CreateComplexSale(ownRefererID,
                firstName, lastName, address1, address2, city, state, zip, country,
                phone, email,
                campaignID, affiliate, subAffiliate, ip, url,
                paymentTypeID, creditCard, cvv, expMonth, expYear,
                partnerClickClickID, isSpecialOffer, couponCode, refererCode, null, null,
                subscriptions, upsellTypes);
        }

        [WebMethod]
        public BusinessError<BillingBadCustomer> ValidateCustomer(long billingID)
        {
            return new BusinessError<BillingBadCustomer>(null, BusinessErrorState.Error, "Stopped validating on 2011-02-01");
            //return (new SaleService()).ValidateCustomer(billingID);
        }

        [WebMethod]
        public BusinessError<FraudScore> ValidateFraud(long billingID, long saleID)
        {
            return (new SaleService()).ValidateFraud(billingID, saleID);
        }
    }
}
