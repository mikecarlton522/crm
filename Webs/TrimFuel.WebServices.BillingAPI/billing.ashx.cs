using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Views;
using TrimFuel.WebServices.BillingAPI.Logic;
using TrimFuel.WebServices.BillingAPI.Logic.SlickTicket;
using TrimFuel.WebServices.BillingAPI.Model;

namespace TrimFuel.WebServices.BillingAPI
{
    /// <summary>
    /// Summary description for billing1
    /// </summary>
    public class billing : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

        private billing_ws api = new billing_ws();
        private IList<string> invalidInput = new List<string>();
        private HttpContext context;
        private string username;
        private string password;

        private const string FUNCTION_VOID = "void";
        private const string FUNCTION_REFUND = "refund";
        private const string FUNCTION_REFUNDSALE = "refundsale";
        private const string FUNCTION_CREATEPROSPECT = "createprospect";
        private const string FUNCTION_CHARGE = "charge";
        private const string FUNCTION_CHARGEMULTIPLEITEMS = "chargemultipleitems";
        private const string FUNCTION_CHARGESALES = "chargesales";
        private const string FUNCTION_CHARGEEXISTINGCUSTOMER = "chargeexistingcustomer";
        private const string FUNCTION_CREATESUBSCRIPTION = "createsubscription";
        private const string FUNCTION_CANCELSUBSCRIPTION = "cancelsubscription";
        private const string FUNCTION_REBILL = "rebill";
        private const string FUNCTION_CHARGELOOKUP = "chargelookup";
        private const string FUNCTION_USERLOOKUP = "userlookup";
        private const string FUNCTION_GETCUSTOMERDETAIL = "getcustomerdetail";
        private const string FUNCTION_ACTIVEPLANLOOKUP = "activeplanlookup";
        private const string FUNCTION_ADDNOTE = "addnote";
        private const string FUNCTION_UPDATEBILLINGINFO = "updatebillinginfo";
        private const string FUNCTION_UPDATESHIPPINGINFO = "updateshippinginfo";
        private const string FUNCTION_CREATETICKET = "createticket";
        private const string FUNCTION_SENDFREEITEM = "sendfreeitem";

        public void ProcessRequest(HttpContext context)
        {
            this.context = context;
            this.context.Response.ContentType = "text/xml";

            this.username = Utility.TryGetStr(context.Request["username"]);
            this.password = Utility.TryGetStr(context.Request["password"]);

            string function = context.Request["function"];

            if (function != null)
                function = function.ToLower();

            switch (function)
            {
                case FUNCTION_VOID: Void(); break;
                case FUNCTION_REFUND: Refund();  break;
                case FUNCTION_REFUNDSALE: RefundSale();  break;
                case FUNCTION_CREATEPROSPECT: CreateProspect(); break;
                case FUNCTION_CHARGE: Charge(); break;
                case FUNCTION_CHARGEMULTIPLEITEMS: ChargeMultipleItems(); break;
                case FUNCTION_CHARGESALES: ChargeSales(); break;
                case FUNCTION_CHARGEEXISTINGCUSTOMER: ChargeExistingCustomer(); break;
                case FUNCTION_CREATESUBSCRIPTION: CreateSubscription(); break;
                case FUNCTION_CANCELSUBSCRIPTION: CancelSubscription(); break;
                case FUNCTION_REBILL: Rebill(); break;
                case FUNCTION_CHARGELOOKUP: ChargeLookup(); break;
                case FUNCTION_USERLOOKUP: UserLookup(); break;
                case FUNCTION_GETCUSTOMERDETAIL: GetCustomerDetail(); break;
                case FUNCTION_ACTIVEPLANLOOKUP: ActivePlanLookup(); break;
                case FUNCTION_ADDNOTE: AddNote(); break;
                case FUNCTION_UPDATEBILLINGINFO: UpdateBillingInfo(); break;
                case FUNCTION_UPDATESHIPPINGINFO: UpdateShippingInfo(); break;
                case FUNCTION_CREATETICKET: CreateTicket(); break;
                case FUNCTION_SENDFREEITEM: SendFreeItem(); break;
                default: break;
            }
        }

        private void Void()
        {
            BusinessError<ChargeHistory> res = null;

            long? chargeHistoryID = Utility.TryGetLong(context.Request["chargeHistoryID"]);

            if (chargeHistoryID == null) invalidInput.Add("chargeHistoryID");

            if (invalidInput.Count > 0)
                res = new BusinessError<ChargeHistory>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.Void(username, password, chargeHistoryID.Value);

            RenderResponse<BusinessError<ChargeHistory>>(context.Response, res);
        }

        private void Refund()
        {
            BusinessError<ChargeHistory> res = null;

            long? chargeHistoryID = Utility.TryGetLong(context.Request["chargeHistoryID"]);
            decimal? refundAmount = Utility.TryGetDecimal(context.Request["refundAmount"]);

            if (chargeHistoryID == null) invalidInput.Add("chargeHistoryID");
            if (refundAmount == null) invalidInput.Add("refundAmount");

            if (invalidInput.Count > 0)
                res = new BusinessError<ChargeHistory>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.Refund(username, password, chargeHistoryID.Value, refundAmount.Value);

            RenderResponse<BusinessError<ChargeHistory>>(context.Response, res);
        }

        private void RefundSale()
        {
            BusinessError<ChargeHistory> res = null;

            long? saleID = Utility.TryGetLong(context.Request["saleID"]);
            decimal? refundAmount = Utility.TryGetDecimal(context.Request["refundAmount"]);

            if (saleID == null) invalidInput.Add("saleID");
            if (refundAmount == null) invalidInput.Add("refundAmount");

            if (invalidInput.Count > 0)
                res = new BusinessError<ChargeHistory>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.RefundSale(username, password, saleID.Value, refundAmount.Value);

            RenderResponse<BusinessError<ChargeHistory>>(context.Response, res);
        }

        private void CreateProspect()
        {
            BusinessError<Prospect> res = null;

            int? productTypeID = Utility.TryGetInt(context.Request["productTypeID"]);
            bool productTypeIDSpecified = productTypeID.HasValue;
            string firstName = Utility.TryGetStr(context.Request["firstName"]);
            string lastName = Utility.TryGetStr(context.Request["lastName"]);
            string address1 = Utility.TryGetStr(context.Request["address1"]);
            string address2 = Utility.TryGetStr(context.Request["address2"]);
            string city = Utility.TryGetStr(context.Request["city"]);
            string state = Utility.TryGetStr(context.Request["state"]);
            string zip = Utility.TryGetStr(context.Request["zip"]);
            string country = Utility.TryGetStr(context.Request["country"]);
            string phone = Utility.TryGetStr(context.Request["phone"]);
            string email = Utility.TryGetStr(context.Request["email"]);
            string ip = Utility.TryGetStr(context.Request["ip"]);
            string affiliate = Utility.TryGetStr(context.Request["affiliate"]);
            string subAffiliate = Utility.TryGetStr(context.Request["subAffiliate"]);
            string internalID = Utility.TryGetStr(context.Request["internalID"]);
            string customField1 = Utility.TryGetStr(context.Request["customField1"]);
            string customField2 = Utility.TryGetStr(context.Request["customField2"]);
            string customField3 = Utility.TryGetStr(context.Request["customField3"]);
            string customField4 = Utility.TryGetStr(context.Request["customField4"]);
            string customField5 = Utility.TryGetStr(context.Request["customField5"]);

            if (invalidInput.Count > 0)
                res = new BusinessError<Prospect>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.CreateProspect(username, password, productTypeIDSpecified ? (int)productTypeID : 0, productTypeIDSpecified, firstName, lastName, address1, address2, city, zip, state, country, phone, email, ip, affiliate, subAffiliate, internalID, customField1, customField2, customField3, customField4, customField5);

            RenderResponse<BusinessError<Prospect>>(context.Response, res);
        }

        private void Charge()
        {
            BusinessError<ChargeHistory> res = null;

            decimal? amount = Utility.TryGetDecimal(context.Request["amount"]);
            decimal? shipping = Utility.TryGetDecimal(context.Request["shipping"]);
            bool shippingSpecified = shipping.HasValue;
            int? productTypeID = Utility.TryGetInt(context.Request["productTypeID"]);
            bool productTypeIDSpecified = productTypeID.HasValue;
            int? productID = Utility.TryGetInt(context.Request["productID"]);
            bool productIDSpecified = productID.HasValue;
            int? campaignID = Utility.TryGetInt(context.Request["campaignID"]);
            bool campaignIDSpecified = campaignID.HasValue;
            string firstName = Utility.TryGetStr(context.Request["firstName"]);
            string lastName = Utility.TryGetStr(context.Request["lastName"]);
            string address1 = Utility.TryGetStr(context.Request["address1"]);
            string address2 = Utility.TryGetStr(context.Request["address2"]);
            string city = Utility.TryGetStr(context.Request["city"]);
            string state = Utility.TryGetStr(context.Request["state"]);
            string zip = Utility.TryGetStr(context.Request["zip"]);
            string country = Utility.TryGetStr(context.Request["country"]);
            string phone = Utility.TryGetStr(context.Request["phone"]);
            string email = Utility.TryGetStr(context.Request["email"]);
            string ip = Utility.TryGetStr(context.Request["ip"]);
            string affiliate = Utility.TryGetStr(context.Request["affiliate"]);
            string subAffiliate = Utility.TryGetStr(context.Request["subAffiliate"]);
            string internalID = Utility.TryGetStr(context.Request["internalID"]);
            int? prospectID = Utility.TryGetInt(context.Request["prospectID"]);
            bool prospectIDSpecified = prospectID.HasValue;
            int? paymentType = Utility.TryGetInt(context.Request["paymentType"]);
            bool paymentTypeSpecified = paymentType.HasValue;
            string creditCard = Utility.TryGetStr(context.Request["creditCard"]);
            string cvv = Utility.TryGetStr(context.Request["cvv"]);
            int? expMonth = Utility.TryGetInt(context.Request["expMonth"]);
            int? expYear = Utility.TryGetInt(context.Request["expYear"]);
            string description = Utility.TryGetStr(context.Request["description"]);
            string customField1 = Utility.TryGetStr(context.Request["customField1"]);
            string customField2 = Utility.TryGetStr(context.Request["customField2"]);
            string customField3 = Utility.TryGetStr(context.Request["customField3"]);
            string customField4 = Utility.TryGetStr(context.Request["customField4"]);
            string customField5 = Utility.TryGetStr(context.Request["customField5"]);

            if (expMonth == null) invalidInput.Add("expMonth");
            if (expYear == null) invalidInput.Add("expYear");

            if (invalidInput.Count > 0)
                res = new BusinessError<ChargeHistory>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.Charge(username, password, amount.Value, shippingSpecified ? shipping.Value : 0, shippingSpecified, productTypeIDSpecified ? productTypeID.Value : 0, productTypeIDSpecified, productIDSpecified ? productID.Value : 0, productIDSpecified, campaignIDSpecified ? campaignID.Value : 0, campaignIDSpecified, firstName, lastName, address1, address2, city, state, zip, country, phone, email, ip, affiliate, subAffiliate, internalID, prospectIDSpecified ? prospectID.Value : 0, prospectIDSpecified, paymentTypeSpecified ? paymentType.Value : 0, paymentTypeSpecified, creditCard, cvv, expMonth.Value, expYear.Value, description, customField1, customField2, customField3, customField4, customField5);

            RenderResponse<BusinessError<ChargeHistory>>(context.Response, res);
        }

        private void ChargeMultipleItems()
        {
            BusinessError<ChargeHistoryList> res = null;

            int? productID1 = Utility.TryGetInt(context.Request["productID1"]);
            int? productID2 = Utility.TryGetInt(context.Request["productID2"]);
            int? productID3 = Utility.TryGetInt(context.Request["productID3"]);
            int? productID4 = Utility.TryGetInt(context.Request["productID4"]);
            int? productID5 = Utility.TryGetInt(context.Request["productID5"]);
            int? productTypeID1 = Utility.TryGetInt(context.Request["productTypeID1"]);
            int? productTypeID2 = Utility.TryGetInt(context.Request["productTypeID2"]);
            int? productTypeID3 = Utility.TryGetInt(context.Request["productTypeID3"]);
            int? productTypeID4 = Utility.TryGetInt(context.Request["productTypeID4"]);
            int? productTypeID5 = Utility.TryGetInt(context.Request["productTypeID5"]);
            decimal? amount1 = Utility.TryGetDecimal(context.Request["amount1"]);
            decimal? amount2 = Utility.TryGetDecimal(context.Request["amount2"]);
            decimal? amount3 = Utility.TryGetDecimal(context.Request["amount3"]);
            decimal? amount4 = Utility.TryGetDecimal(context.Request["amount4"]);
            decimal? amount5 = Utility.TryGetDecimal(context.Request["amount5"]);
            decimal? shipping1 = Utility.TryGetDecimal(context.Request["shipping1"]);
            decimal? shipping2 = Utility.TryGetDecimal(context.Request["shipping2"]);
            decimal? shipping3 = Utility.TryGetDecimal(context.Request["shipping3"]);
            decimal? shipping4 = Utility.TryGetDecimal(context.Request["shipping4"]);
            decimal? shipping5 = Utility.TryGetDecimal(context.Request["shipping5"]);
            int? campaignID = Utility.TryGetInt(context.Request["campaignID"]);
            bool campaignIDSpecified = campaignID.HasValue;
            string firstName = Utility.TryGetStr(context.Request["firstName"]);
            string lastName = Utility.TryGetStr(context.Request["lastName"]);
            string address1 = Utility.TryGetStr(context.Request["address1"]);
            string address2 = Utility.TryGetStr(context.Request["address2"]);
            string city = Utility.TryGetStr(context.Request["city"]);
            string state = Utility.TryGetStr(context.Request["state"]);
            string zip = Utility.TryGetStr(context.Request["zip"]);
            string country = Utility.TryGetStr(context.Request["country"]);
            string phone = Utility.TryGetStr(context.Request["phone"]);
            string email = Utility.TryGetStr(context.Request["email"]);
            string ip = Utility.TryGetStr(context.Request["ip"]);
            string affiliate = Utility.TryGetStr(context.Request["affiliate"]);
            string subAffiliate = Utility.TryGetStr(context.Request["subAffiliate"]);
            string internalID = Utility.TryGetStr(context.Request["internalID"]);
            int? prospectID = Utility.TryGetInt(context.Request["prospectID"]);
            bool prospectIDSpecified = prospectID.HasValue;
            int? paymentType = Utility.TryGetInt(context.Request["paymentType"]);
            bool paymentTypeSpecified = paymentType.HasValue;
            string creditCard = Utility.TryGetStr(context.Request["creditCard"]);
            string cvv = Utility.TryGetStr(context.Request["cvv"]);
            int? expMonth = Utility.TryGetInt(context.Request["expMonth"]);
            int? expYear = Utility.TryGetInt(context.Request["expYear"]);
            string description = Utility.TryGetStr(context.Request["description"]);
            string customField1 = Utility.TryGetStr(context.Request["customField1"]);
            string customField2 = Utility.TryGetStr(context.Request["customField2"]);
            string customField3 = Utility.TryGetStr(context.Request["customField3"]);
            string customField4 = Utility.TryGetStr(context.Request["customField4"]);
            string customField5 = Utility.TryGetStr(context.Request["customField5"]);

            ItemList itemList = new ItemList();

            if (productID1.HasValue)
            {
                if (amount1 == null) invalidInput.Add("amount1");
                else if (productTypeID1 == null) invalidInput.Add("productTypeID1");
                else if (shipping1 == null) invalidInput.Add("shipping1");
                else itemList.Add(new Item() { Amount = amount1.Value, ProductID = productID1.Value, ProductTypeID = productTypeID1.Value, Shipping = shipping1.Value });
            }
            if (productID2.HasValue)
            {
                if (amount2 == null) invalidInput.Add("amount2");
                else if (productTypeID2 == null) invalidInput.Add("productTypeID2");
                else if (shipping2 == null) invalidInput.Add("shipping2");
                else itemList.Add(new Item() { Amount = amount2.Value, ProductID = productID2.Value, ProductTypeID = productTypeID2.Value, Shipping = shipping2.Value });
            }
            if (productID3.HasValue)
            {
                if (amount3 == null) invalidInput.Add("amount3");
                else if (productTypeID3 == null) invalidInput.Add("productTypeID3");
                else if (shipping3 == null) invalidInput.Add("shipping3");
                else itemList.Add(new Item() { Amount = amount3.Value, ProductID = productID3.Value, ProductTypeID = productTypeID3.Value, Shipping = shipping3.Value });
            }
            if (productID4.HasValue)
            {
                if (amount4 == null) invalidInput.Add("amount4");
                else if (productTypeID4 == null) invalidInput.Add("productTypeID4");
                else if (shipping4 == null) invalidInput.Add("shipping4");
                else itemList.Add(new Item() { Amount = amount4.Value, ProductID = productID4.Value, ProductTypeID = productTypeID4.Value, Shipping = shipping4.Value });
            }
            if (productID5.HasValue)
            {
                if (amount5 == null) invalidInput.Add("amount5");
                else if (productTypeID5 == null) invalidInput.Add("productTypeID5");
                else if (shipping5 == null) invalidInput.Add("shipping5");
                else itemList.Add(new Item() { Amount = amount5.Value, ProductID = productID5.Value, ProductTypeID = productTypeID5.Value, Shipping = shipping5.Value });
            }

            if (expMonth == null) invalidInput.Add("expMonth");
            if (expYear == null) invalidInput.Add("expYear");

            if (invalidInput.Count > 0)
                res = new BusinessError<ChargeHistoryList>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.ChargeMultipleItems(username, password, itemList, campaignIDSpecified ? campaignID.Value : 0, campaignIDSpecified, firstName, lastName, address1, address2, city, state, zip, country, phone, email, ip, affiliate, subAffiliate, internalID, prospectIDSpecified ? prospectID.Value : 0, prospectIDSpecified, paymentTypeSpecified ? paymentType.Value : 0, paymentTypeSpecified, creditCard, cvv, expMonth.Value, expYear.Value, description, customField1, customField2, customField3, customField4, customField5);

            RenderResponse<BusinessError<ChargeHistoryList>>(context.Response, res);
        }

        private void ChargeSales()
        {
            BusinessError<ChargeHistorySales> res = null;

            int? productTypeID = Utility.TryGetInt(context.Request["productTypeID"]);
            bool productTypeIDSpecified = productTypeID.HasValue;
            int? productID1 = Utility.TryGetInt(context.Request["productID1"]);
            int? productID2 = Utility.TryGetInt(context.Request["productID2"]);
            int? productID3 = Utility.TryGetInt(context.Request["productID3"]);
            int? productID4 = Utility.TryGetInt(context.Request["productID4"]);
            int? productID5 = Utility.TryGetInt(context.Request["productID5"]);
            decimal? amount1 = Utility.TryGetDecimal(context.Request["amount1"]);
            decimal? amount2 = Utility.TryGetDecimal(context.Request["amount2"]);
            decimal? amount3 = Utility.TryGetDecimal(context.Request["amount3"]);
            decimal? amount4 = Utility.TryGetDecimal(context.Request["amount4"]);
            decimal? amount5 = Utility.TryGetDecimal(context.Request["amount5"]);
            int? campaignID = Utility.TryGetInt(context.Request["campaignID"]);
            bool campaignIDSpecified = campaignID.HasValue;
            string firstName = Utility.TryGetStr(context.Request["firstName"]);
            string lastName = Utility.TryGetStr(context.Request["lastName"]);
            string address1 = Utility.TryGetStr(context.Request["address1"]);
            string address2 = Utility.TryGetStr(context.Request["address2"]);
            string city = Utility.TryGetStr(context.Request["city"]);
            string state = Utility.TryGetStr(context.Request["state"]);
            string zip = Utility.TryGetStr(context.Request["zip"]);
            string country = Utility.TryGetStr(context.Request["country"]);
            string phone = Utility.TryGetStr(context.Request["phone"]);
            string email = Utility.TryGetStr(context.Request["email"]);
            string ip = Utility.TryGetStr(context.Request["ip"]);
            string affiliate = Utility.TryGetStr(context.Request["affiliate"]);
            string subAffiliate = Utility.TryGetStr(context.Request["subAffiliate"]);
            string internalID = Utility.TryGetStr(context.Request["internalID"]);
            int? prospectID = Utility.TryGetInt(context.Request["prospectID"]);
            bool prospectIDSpecified = prospectID.HasValue;
            int? paymentType = Utility.TryGetInt(context.Request["paymentType"]);
            bool paymentTypeSpecified = paymentType.HasValue;
            string creditCard = Utility.TryGetStr(context.Request["creditCard"]);
            string cvv = Utility.TryGetStr(context.Request["cvv"]);
            int? expMonth = Utility.TryGetInt(context.Request["expMonth"]);
            int? expYear = Utility.TryGetInt(context.Request["expYear"]);
            string description = Utility.TryGetStr(context.Request["description"]);
            string customField1 = Utility.TryGetStr(context.Request["customField1"]);
            string customField2 = Utility.TryGetStr(context.Request["customField2"]);
            string customField3 = Utility.TryGetStr(context.Request["customField3"]);
            string customField4 = Utility.TryGetStr(context.Request["customField4"]);
            string customField5 = Utility.TryGetStr(context.Request["customField5"]);

            ProductList productList = new ProductList();

            if (productID1.HasValue)
            {
                if (amount1 == null) invalidInput.Add("amount1");
                else productList.Add(new ProductDesc(){ Amount = amount1.Value, ProductID = productID1.Value });
            }
            if (productID2.HasValue)
            {
                if (amount2 == null) invalidInput.Add("amount2");
                else productList.Add(new ProductDesc() { Amount = amount2.Value, ProductID = productID2.Value });
            }
            if (productID3.HasValue)
            {
                if (amount3 == null) invalidInput.Add("amount3");
                else productList.Add(new ProductDesc() { Amount = amount3.Value, ProductID = productID3.Value });
            }
            if (productID4.HasValue)
            {
                if (amount4 == null) invalidInput.Add("amount4");
                else productList.Add(new ProductDesc() { Amount = amount4.Value, ProductID = productID4.Value });
            }
            if (productID5.HasValue)
            {
                if (amount5 == null) invalidInput.Add("amount5");
                else productList.Add(new ProductDesc() { Amount = amount5.Value, ProductID = productID5.Value });
            }

            if (expMonth == null) invalidInput.Add("expMonth");
            if (expYear == null) invalidInput.Add("expYear");

            if (invalidInput.Count > 0)
                res = new BusinessError<ChargeHistorySales>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.ChargeSales(username, password, productTypeIDSpecified ? productTypeID.Value : 0, productTypeIDSpecified, productList, campaignIDSpecified ? campaignID.Value : 0, campaignIDSpecified, firstName, lastName, address1, address2, city, state, zip, country, phone, email, ip, affiliate, subAffiliate, internalID, prospectIDSpecified ? prospectID.Value : 0, prospectIDSpecified, paymentTypeSpecified ? paymentType.Value : 0, paymentTypeSpecified, creditCard, cvv, expMonth.Value, expYear.Value, customField1, customField2, customField3, customField4, customField5);

            RenderResponse<BusinessError<ChargeHistorySales>>(context.Response, res);
        }

        private void ChargeExistingCustomer()
        {
            BusinessError<ChargeHistory> res = null;

            int? prospectID = Utility.TryGetInt(context.Request["prospectID"]);
            bool prospectIDSpecified = prospectID.HasValue;
            int? billingID = Utility.TryGetInt(context.Request["billingID"]);
            bool billingIDSpecified = billingID.HasValue;
            int? productID = Utility.TryGetInt(context.Request["productID"]);
            int? productTypeID = Utility.TryGetInt(context.Request["productTypeID"]);
            decimal? amount = Utility.TryGetDecimal(context.Request["amount"]);

            if (productID == null) invalidInput.Add("productID");
            if (productTypeID == null) invalidInput.Add("productTypeID");
            if (amount == null) invalidInput.Add("amount");

            if (invalidInput.Count > 0)
                res = new BusinessError<ChargeHistory>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.ChargeExistingCustomer(username, password, prospectIDSpecified ? prospectID.Value : 0, prospectIDSpecified, billingIDSpecified ? billingID.Value : 0, billingIDSpecified, productID.Value, productTypeID.Value, amount.Value);

            RenderResponse<BusinessError<ChargeHistory>>(context.Response, res);
        }

        private void CreateSubscription()
        {
            BusinessError<PlanResult> res = null;

            int? planID = Utility.TryGetInt(context.Request["planID"]);
            bool? chargeForTrial = Utility.TryGetBool(context.Request["chargeForTrial"]);
            int? campaignID = Utility.TryGetInt(context.Request["campaignID"]);
            bool campaignIDSpecified = campaignID.HasValue;
            string firstName = Utility.TryGetStr(context.Request["firstName"]);
            string lastName = Utility.TryGetStr(context.Request["lastName"]);
            string address1 = Utility.TryGetStr(context.Request["address1"]);
            string address2 = Utility.TryGetStr(context.Request["address2"]);
            string city = Utility.TryGetStr(context.Request["city"]);
            string state = Utility.TryGetStr(context.Request["state"]);
            string zip = Utility.TryGetStr(context.Request["zip"]);
            string country = Utility.TryGetStr(context.Request["country"]);
            string phone = Utility.TryGetStr(context.Request["phone"]);
            string email = Utility.TryGetStr(context.Request["email"]);
            string ip = Utility.TryGetStr(context.Request["ip"]);
            string affiliate = Utility.TryGetStr(context.Request["affiliate"]);
            string subAffiliate = Utility.TryGetStr(context.Request["subAffiliate"]);
            string internalID = Utility.TryGetStr(context.Request["internalID"]);
            int? prospectID = Utility.TryGetInt(context.Request["prospectID"]);
            bool prospectIDSpecified = prospectID.HasValue;
            int? paymentType = Utility.TryGetInt(context.Request["paymentType"]);
            bool paymentTypeSpecified = paymentType.HasValue;
            string creditCard = Utility.TryGetStr(context.Request["creditCard"]);
            string cvv = Utility.TryGetStr(context.Request["cvv"]);
            int? expMonth = Utility.TryGetInt(context.Request["expMonth"]);
            int? expYear = Utility.TryGetInt(context.Request["expYear"]);
            string description = Utility.TryGetStr(context.Request["description"]);

            if (planID == null) invalidInput.Add("planID");
            if (chargeForTrial == null) invalidInput.Add("chargeForTrial");
            if (expMonth == null) invalidInput.Add("expMonth");
            if (expYear == null) invalidInput.Add("expYear");

            if (invalidInput.Count > 0)
                res = new BusinessError<PlanResult>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.CreateSubscription(username, password, planID.Value, chargeForTrial.Value, campaignIDSpecified ? campaignID.Value : 0, campaignIDSpecified, firstName, lastName, address1, address2, city, state, zip, country, phone, email, ip, affiliate, subAffiliate, internalID, prospectIDSpecified ? prospectID.Value : 0, prospectIDSpecified, paymentTypeSpecified ? paymentType.Value : 0, paymentTypeSpecified, creditCard, cvv, expMonth.Value, expYear.Value, description);

            RenderResponse<BusinessError<PlanResult>>(context.Response, res);
        }

        private void CancelSubscription()
        {
            BusinessError<int> res = null;

            long? billingID = Utility.TryGetLong(context.Request["billingID"]);
            bool billingIDSpecified = billingID.HasValue;
            string phone = Utility.TryGetStr(context.Request["phone"]);
            string email = Utility.TryGetStr(context.Request["email"]);

            res = api.CancelSubscription(username, password, billingIDSpecified ? billingID.Value : 0, billingIDSpecified, phone, email);

            RenderResponse<BusinessError<int>>(context.Response, res);
        }

        private void Rebill()
        {
            BusinessError<ChargeHistory> res = null;

            decimal? amount = Utility.TryGetDecimal(context.Request["amount"]);
            string internalID = Utility.TryGetStr(context.Request["internalID"]);

            if (amount == null) invalidInput.Add("amount");

            if (invalidInput.Count > 0)
                res = new BusinessError<ChargeHistory>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.Rebill(username, password, amount.Value, internalID);

            RenderResponse<BusinessError<ChargeHistory>>(context.Response, res);
        }

        private void ChargeLookup()
        {
            BusinessError<ChargeHistory> res = null;

            long? chargeHistoryID = Utility.TryGetLong(context.Request["chargeHistoryID"]);

            if (chargeHistoryID == null) invalidInput.Add("chargeHistoryID");

            if (invalidInput.Count > 0)
                res = new BusinessError<ChargeHistory>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.ChargeLookup(username, password, chargeHistoryID.Value);

            RenderResponse<BusinessError<ChargeHistory>>(context.Response, res);
        }

        private void UserLookup()
        {
            BusinessError<ChargeHistoryList> res = null;

            string internalID = Utility.TryGetStr(context.Request["internalID"]);

            res = api.UserLookup(username, password, internalID);

            RenderResponse<BusinessError<ChargeHistoryList>>(context.Response, res);
        }

        private void GetCustomerDetail()
        {
            BusinessError<UserInfoList> res = null;

            string phone = Utility.TryGetStr(context.Request["phone"]);
            long? prospectID = Utility.TryGetLong(context.Request["prospectID"]);

            if (prospectID == null) invalidInput.Add("prospectID");

            if (invalidInput.Count > 0)
                res = new BusinessError<UserInfoList>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.GetCustomerDetail(username, password, phone, prospectID.Value);

            RenderResponse<BusinessError<UserInfoList>>(context.Response, res);
        }

        private void ActivePlanLookup()
        {
            BusinessError<BillingSubscriptionList> res = null;

            string phone = Utility.TryGetStr(context.Request["phone"]);
            string email = Utility.TryGetStr(context.Request["email"]);

            res = api.ActivePlanLookup(username, password, phone, email);

            RenderResponse<BusinessError<BillingSubscriptionList>>(context.Response, res);
        }

        private void AddNote()
        {
            BusinessError<Notes> res = null;

            long? prospectID = Utility.TryGetLong(context.Request["prospectID"]);
            bool prospectIDSpecified = prospectID.HasValue;
            string internalID = Utility.TryGetStr(context.Request["internalID"]);
            string content = Utility.TryGetStr(context.Request["content"]);

            res = api.AddNote(username, password, prospectIDSpecified ? prospectID.Value : 0, prospectIDSpecified, internalID, content);

            RenderResponse<BusinessError<Notes>>(context.Response, res);
        }

        private void UpdateBillingInfo()
        {
            BusinessError<Billing> res = null;

            long? prospectID = Utility.TryGetLong(context.Request["prospectID"]);
            bool prospectIDSpecified = prospectID.HasValue;
            string internalID = Utility.TryGetStr(context.Request["internalID"]);
            string firstName = Utility.TryGetStr(context.Request["firstName"]);
            string lastName = Utility.TryGetStr(context.Request["lastName"]);
            string address1 = Utility.TryGetStr(context.Request["address1"]);
            string address2 = Utility.TryGetStr(context.Request["address2"]);
            string city = Utility.TryGetStr(context.Request["city"]);
            string state = Utility.TryGetStr(context.Request["state"]);
            string zip = Utility.TryGetStr(context.Request["zip"]);
            string phone = Utility.TryGetStr(context.Request["phone"]);
            string email = Utility.TryGetStr(context.Request["email"]);
            int? paymentType = Utility.TryGetInt(context.Request["paymentType"]);
            bool paymentTypeSpecified = paymentType.HasValue;
            string creditCard = Utility.TryGetStr(context.Request["creditCard"]);
            string cvv = Utility.TryGetStr(context.Request["cvv"]);
            int? expMonth = Utility.TryGetInt(context.Request["expMonth"]);
            bool expMonthSpecified = expMonth.HasValue;
            int? expYear = Utility.TryGetInt(context.Request["expYear"]);
            bool expYearSpecified = expYear.HasValue;

            res = api.UpdateBillingInfo(username, password, prospectIDSpecified ? prospectID.Value : 0, prospectIDSpecified, internalID, firstName, lastName, address1, address2, city, state, zip, phone, email, paymentTypeSpecified ? paymentType.Value : 0, paymentTypeSpecified, creditCard, cvv, expMonthSpecified ? expMonth.Value : 0, expMonthSpecified, expYearSpecified ? expYear.Value : 0, expYearSpecified);

            RenderResponse<BusinessError<Billing>>(context.Response, res);
        }

        private void UpdateShippingInfo()
        {
            BusinessError<Prospect> res = null;

            long? prospectID = Utility.TryGetLong(context.Request["prospectID"]);
            bool prospectIDSpecified = prospectID.HasValue; 
            string internalID = Utility.TryGetStr(context.Request["internalID"]);
            string firstName = Utility.TryGetStr(context.Request["firstName"]);
            string lastName = Utility.TryGetStr(context.Request["lastName"]);
            string address1 = Utility.TryGetStr(context.Request["address1"]);
            string address2 = Utility.TryGetStr(context.Request["address2"]);
            string city = Utility.TryGetStr(context.Request["city"]);
            string state = Utility.TryGetStr(context.Request["state"]);
            string zip = Utility.TryGetStr(context.Request["zip"]);
            string phone = Utility.TryGetStr(context.Request["phone"]);
            string email = Utility.TryGetStr(context.Request["email"]);

            res = api.UpdateShippingInfo(username, password, prospectIDSpecified ? prospectID.Value : 0, prospectIDSpecified, internalID, firstName, lastName, address1, address2, city, state, zip, phone, email);

            RenderResponse<BusinessError<Prospect>>(context.Response, res);
        }

        private void CreateTicket()
        {
            BusinessError<Ticket> res = null;

            int? toAdminID = Utility.TryGetInt(context.Request["toAdminID"]);
            int? fromAdminID = Utility.TryGetInt(context.Request["fromAdminID"]);
            string subject = Utility.TryGetStr(context.Request["subject"]);
            string content = Utility.TryGetStr(context.Request["content"]);
            int? priority = Utility.TryGetInt(context.Request["priority"]);
            long? billingID = Utility.TryGetLong(context.Request["billingID"]);
            bool billingIDSpecified = billingID.HasValue;

            if (toAdminID == null) invalidInput.Add("toAdminID");
            if (fromAdminID == null) invalidInput.Add("fromAdminID");
            if (priority == null) invalidInput.Add("priority");

            if (invalidInput.Count > 0)
                res = new BusinessError<Ticket>(null, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.CreateTicket(username, password, toAdminID.Value, fromAdminID.Value, subject, content, priority.Value, billingIDSpecified ? billingID.Value : 0, billingIDSpecified);

            RenderResponse<BusinessError<Ticket>>(context.Response, res);
        }

        private void SendFreeItem()
        {
            BusinessError<bool> res = null;

            long? prospectID = Utility.TryGetLong(context.Request["prospectID"]);
            int? productID = Utility.TryGetInt(context.Request["productID"]);   
            int? quantity = Utility.TryGetInt(context.Request["quantity"]);

            if (prospectID == null) invalidInput.Add("prospectID");
            if (productID == null) invalidInput.Add("productID");
            if (quantity == null) invalidInput.Add("quantity");

            if (invalidInput.Count > 0)
                res = new BusinessError<bool>(false, BusinessErrorState.Error, ErrorMessage());

            else
                res = api.SendFreeItem(username, password, prospectID.Value, productID.Value, quantity.Value);

            RenderResponse<BusinessError<bool>>(context.Response, res);
        }

        #region Helper methods

        private void RenderResponse<T>(HttpResponse response, T responseObject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), "http://trianglecrm.com");

            serializer.Serialize(response.Output, responseObject);
        }

        private string ErrorMessage()
        {
            return "Invalid parameters. The following fields are missing or in bad format: " + string.Join(", ", invalidInput.ToArray());
        }

        #endregion
    }
}