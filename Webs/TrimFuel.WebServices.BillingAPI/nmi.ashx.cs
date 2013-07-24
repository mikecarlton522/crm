using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.WebServices.BillingAPI.Model;
using TrimFuel.WebServices.BillingAPI.Logic;
using TrimFuel.Business.Utils;
using System.Text.RegularExpressions;
using TrimFuel.Model.Views;

namespace TrimFuel.WebServices.BillingAPI
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    public class nmi : IHttpHandler
    {
        private SaleService saleService = new SaleService();
        private const int PRODUCT_ID = 1;
        private const int TP_MODE_ID = TrimFuel.Model.Enums.TPModeEnum.NMI_Emulation;

        private const string FUNCTION_CHARGE = "sale";
        private const string FUNCTION_VOID = "void";
        private const string FUNCTION_REFUND = "refund";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string function = context.Request["type"];
            if (function != null)
            {
                function = function.ToLower();
            }

            IList<string> invalidInput = new List<string>();

            string username = Utility.TryGetStr(context.Request["username"]);
            string password = Utility.TryGetStr(context.Request["password"]);

            if (function == FUNCTION_CHARGE)
            {
                decimal? amount = Utility.TryGetDecimal(context.Request["amount"]); //
                int? productTypeID = Utility.TryGetInt(context.Request["productTypeID"]);
                int? productID = Utility.TryGetInt(context.Request["productID"]);
                string firstName = Utility.TryGetStr(context.Request["firstname"]);
                string lastName = Utility.TryGetStr(context.Request["lastname"]);
                string address1 = Utility.TryGetStr(context.Request["address1"]);
                string address2 = Utility.TryGetStr(context.Request["address2"]);
                string city = Utility.TryGetStr(context.Request["city"]);
                string state = Utility.TryGetStr(context.Request["state"]);
                string zip = Utility.TryGetStr(context.Request["zip"]);
                string country = Utility.TryGetStr(context.Request["country"]);
                string phone = Utility.TryGetStr(context.Request["phone"]);
                string email = Utility.TryGetStr(context.Request["email"]);
                string ip = Utility.TryGetStr(context.Request["ipadress"]);
                string affiliate = Utility.TryGetStr(context.Request["merchant_defined_field_1"]);
                string subAffiliate = Utility.TryGetStr(context.Request["merchant_defined_field_2"]);
                string internalID = Utility.TryGetStr(context.Request["orderid"]);
                string creditCard = Utility.TryGetStr(context.Request["ccnumber"]);//
                string expString = Utility.TryGetStr(context.Request["ccexp"]);//
                string cvv = Utility.TryGetStr(context.Request["cvv"]);

                int temp = 0;
                if (string.IsNullOrEmpty(firstName)) invalidInput.Add("firstname");
                if (string.IsNullOrEmpty(lastName)) invalidInput.Add("lastname");
                if (string.IsNullOrEmpty(address1)) invalidInput.Add("address1");
                if (string.IsNullOrEmpty(city)) invalidInput.Add("city");
                if (string.IsNullOrEmpty(state)) invalidInput.Add("state");
                if (string.IsNullOrEmpty(zip)) invalidInput.Add("zip");
                //if (string.IsNullOrEmpty(phone)) insufficientFields.Add("phone");
                //if (string.IsNullOrEmpty(email)) insufficientFields.Add("email");
                //if (string.IsNullOrEmpty(ip)) insufficientFields.Add("ipadress");
                //if (string.IsNullOrEmpty(internalID)) invalidInput.Add("orderid");
                if (string.IsNullOrEmpty(creditCard)) invalidInput.Add("ccnumber");
                //if (string.IsNullOrEmpty(cvv)) invalidInput.Add("cvv");
                if (amount == null) invalidInput.Add("amount");
                if (context.Request["productTypeID"] != null && context.Request["productTypeID"].Trim().Length > 0 && productTypeID == null) invalidInput.Add("productTypeID");
                if (context.Request["productID"] != null && context.Request["productID"].Trim().Length > 0 && productID == null) invalidInput.Add("productID");
                if (string.IsNullOrEmpty(expString) || expString.Length != 4 || !Regex.IsMatch(expString, @"^\d{4}$", RegexOptions.IgnoreCase)) invalidInput.Add("ccexp");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response,
                        new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))),
                        function, internalID);
                }
                else
                {
                    //determine payment type
                    int paymentType = PaymentTypeEnum.Visa;
                    if (!string.IsNullOrEmpty(creditCard) && creditCard.ToCharArray()[0] == '5') paymentType = PaymentTypeEnum.Mastercard;

                    //determine exp date
                    int.TryParse(expString.Substring(0, 2), out temp);
                    int expMonth = temp;
                    int.TryParse(expString.Substring(2, 2), out temp);
                    int expYear = 2000 + temp;

                    RenderResponse(context.Response,
                        Charge(username, password, amount.Value, null,
                            productTypeID,
                            productID,
                            firstName, lastName,
                            address1, address2, city, state, zip, country,
                            phone, email, ip, affiliate, subAffiliate, internalID,
                            paymentType, creditCard, cvv, expMonth, expYear),
                        function, internalID);
                }
            }
            else if (function == FUNCTION_VOID)
            {
                long? chargeHistoryID = Utility.TryGetInt(context.Request["transactionid"]);

                if (chargeHistoryID == null) invalidInput.Add("transactionid");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response,
                        new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))),
                        function, string.Empty);
                }
                else
                {
                    string internalID = null;
                    RenderResponse(context.Response,
                        Void(username, password, chargeHistoryID.Value, out internalID),
                        function, internalID);
                }
            }
            else if (function == FUNCTION_REFUND)
            {
                long? chargeHistoryID = Utility.TryGetInt(context.Request["transactionid"]);
                decimal? refundAmount = Utility.TryGetDecimal(context.Request["amount"]);

                if (chargeHistoryID == null) invalidInput.Add("transactionid");
                if (refundAmount == null) invalidInput.Add("amount");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response,
                        new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))),
                        function, string.Empty);
                }
                else
                {
                    string internalID = null;
                    RenderResponse(context.Response,
                        Refund(username, password, chargeHistoryID.Value, refundAmount.Value, out internalID),
                        function, internalID);
                }
            }
            else
            {
                RenderResponse(context.Response, new BusinessError<ChargeHistoryEx>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = string.Format("Unrecognized transaction type \"{0}\"", context.Request["type"])
                },
                string.Empty, string.Empty);
            }
        }

        private void RenderResponse(HttpResponse response, BusinessError<ChargeHistoryEx> responseObject, string function, string internalID)
        {
            if (responseObject.ReturnValue == null && responseObject.State == BusinessErrorState.Error)
            {
                response.Write(string.Format("response=3&responsetext={0}&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid={1}&type={2}&response_code=300",
                    responseObject.ErrorMessage, internalID, function));
            }
            else
            {
                string res = responseObject.ReturnValue.Response;
                //Substitute transaction numer
                if (!string.IsNullOrEmpty(responseObject.ReturnValue.TransactionNumber))
                {
                    res = res.Replace(responseObject.ReturnValue.TransactionNumber, responseObject.ReturnValue.ChargeHistoryID.Value.ToString());
                }
                //Substitute orderid
                res = Regex.Replace(res, @"orderid=\s*\d*", "orderid=" + internalID, RegexOptions.IgnoreCase);
                response.Write(res);
            }
        }

        private void RenderResponse(HttpResponse response, BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> responseObject, string function, string internalID)
        {
            if ((responseObject.ReturnValue == null || responseObject.ReturnValue.Value1 == null && responseObject.ReturnValue.Value2 == null) && responseObject.State == BusinessErrorState.Error)
            {
                response.Write(string.Format("response=3&responsetext={0}&authcode=&transactionid=&avsresponse=&cvvresponse=&orderid={1}&type={2}&response_code=300",
                    responseObject.ErrorMessage, internalID, function));
            }
            else
            {
                string res = string.Empty;
                if (responseObject.ReturnValue.Value1 != null)
                {
                    res = responseObject.ReturnValue.Value1.Response;
                }
                else if (responseObject.ReturnValue.Value2 != null)
                {
                    res = responseObject.ReturnValue.Value2.Response;
                }                
                //Substitute transaction numer
                if (responseObject.ReturnValue.Value1 != null)
                {
                    if (!string.IsNullOrEmpty(responseObject.ReturnValue.Value1.TransactionNumber))
                    {
                        res = res.Replace(responseObject.ReturnValue.Value1.TransactionNumber, responseObject.ReturnValue.Value1.ChargeHistoryID.Value.ToString());
                    }
                }
                //Substitute orderid
                res = Regex.Replace(res, @"orderid=\s*\d*", "orderid=" + internalID, RegexOptions.IgnoreCase);
                response.Write(res);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string FindInternalIDByChargeHistoryID(long chargeHistoryID)
        {
            string res = string.Empty;
            //Find transaction
            ChargeHistoryEx ch = saleService.Load<ChargeHistoryEx>(chargeHistoryID);
            if (ch == null)
                return res;

            //Find BillingSubscription
            BillingSubscription bs = saleService.Load<BillingSubscription>(ch.BillingSubscriptionID);
            if (bs == null)
                return res;

            //Find BillingSubscription
            BillingExternalInfo bei = saleService.Load<BillingExternalInfo>(bs.BillingID);
            if (bei == null)
                return res;

            return bei.InternalID;
        }

        public BusinessError<ChargeHistoryEx> Void(string username, string password, long chargeHistoryID, out string internalID)
        {
            internalID = string.Empty;

            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistoryEx>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            internalID = FindInternalIDByChargeHistoryID(chargeHistoryID);
            return saleService.DoVoid(chargeHistoryID);
        }

        public BusinessError<ChargeHistoryEx> Refund(string username, string password, long chargeHistoryID, decimal refundAmount, out string internalID)
        {
            internalID = string.Empty;

            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<ChargeHistoryEx>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            internalID = FindInternalIDByChargeHistoryID(chargeHistoryID);
            return saleService.DoRefund(chargeHistoryID, refundAmount);
        }

        public BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> Charge(string username, string password, decimal amount, decimal? shippingAmount,
            int? productTypeID,
            int? productID,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate, string internalID,
            int paymentType, string creditCard, string cvv, int expMonth, int expYear)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            int productGroupID = PRODUCT_ID;
            if (productTypeID != null)
            {
                productGroupID = productTypeID.Value;
            }

            return saleService.BillAsUpsell(productGroupID, productID, amount, shippingAmount,
                null,
                firstName, lastName,
                address1, address2, city, state, zip, country,
                phone, email, ip, affiliate, subAffiliate, 
                internalID, null,
                paymentType, creditCard, cvv, expMonth, expYear,
                null,
                null, null, null, null, null);
        }
    }
}
