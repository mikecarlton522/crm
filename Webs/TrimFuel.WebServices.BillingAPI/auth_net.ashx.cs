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
    public class auth_net : IHttpHandler
    {
        private SaleService saleService = new SaleService();
        private BillingService billingService = new BillingService();

        private const int PRODUCT_ID = 1;
        private const int TP_MODE_ID = TrimFuel.Model.Enums.TPModeEnum.AuthoriseNET_Emulation;

        private const string FUNCTION_CHARGE = "AUTH_CAPTURE";
        private const string FUNCTION_AUTH_ONLY = "AUTH_ONLY";
        private const string FUNCTION_VOID = "VOID";
        private const string FUNCTION_REFUND = "CREDIT";
        private const string FUNCTION_CREATE_SUBSCRIPTION = "SUBSCRIBE";
        private const string FUNCTION_CAPTURE = "PRIOR_AUTH_CAPTURE";

        private const string DEFAULT_DELIM_CHAR = ",";
        private const string DEFAULT_ENCAP_CHAR1 = "\"";
        private const string DEFAULT_ENCAP_CHAR2 = "'";

        private const string DEFAULT_VERSION = "3.0";

        private const string VERSION_30_RESPONSE = "{0},,,{1},{2},{4},{3},,,{6},CC,{7},{8},,,,,,,,,,,,,,,,,,,,,,,,,";
        private const string VERSION_31_RESPONSE = "{0},,,{1},{2},{4},{3},,,{6},CC,{7},{8},,,,,,,,,,,,,,,,,,,,,,,,,,{5},,,,,,,,,,,,,,,,,,,,,,,,,,,,,";
        // 0 - Response Code
        // 1 - Response Text
        // 2 - Authcode
        // 3 - Transaction ID
        // 4 - AVS
        // 5 - CVV
        // 6 - Amount
        // 7 - TransactionType
        // 8 - Customer ID
        // 9 - Aff
        // 10 - SubAff

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string function = context.Request["x_type"];
            if (string.IsNullOrEmpty(function))
            {
                function = FUNCTION_CHARGE;
            }
            else
            {
                function = function.ToUpper();
            }

            IList<string> invalidInput = new List<string>();

            string username = Utility.TryGetStr(context.Request["x_login"]);
            string password = Utility.TryGetStr(context.Request["x_tran_key"]);

            string version = Utility.TryGetStr(context.Request["x_version"]);
            if (string.IsNullOrEmpty(version)) version = DEFAULT_VERSION;
            if (version != "3.0" && version != "3.1")
            {
                version = DEFAULT_VERSION;
                invalidInput.Add("x_version");
            }

            string delim_char = Utility.TryGetStr(context.Request["x_delim_char"]);
            if (string.IsNullOrEmpty(delim_char)) delim_char = DEFAULT_DELIM_CHAR;
            if (delim_char.Length > 1)
            {
                delim_char = DEFAULT_DELIM_CHAR;
                invalidInput.Add("x_delim_char");
            }

            string encap_char = Utility.TryGetStr(context.Request["x_encap_char"]);
            if (string.IsNullOrEmpty(encap_char) || encap_char == delim_char) encap_char = (delim_char != DEFAULT_ENCAP_CHAR1) ? DEFAULT_ENCAP_CHAR1 : DEFAULT_ENCAP_CHAR2;
            if (encap_char.Length > 1)
            {
                encap_char = (delim_char != DEFAULT_ENCAP_CHAR1) ? DEFAULT_ENCAP_CHAR1 : DEFAULT_ENCAP_CHAR2;
                invalidInput.Add("x_encap_char");
            }

            if (function == FUNCTION_CHARGE)
            {
                decimal? amount = Utility.TryGetDecimal(context.Request["x_amount"]);

                int? productTypeID = Utility.TryGetInt(context.Request["productTypeID"]);
                int? productID = Utility.TryGetInt(context.Request["productID"]);

                string firstName = Utility.TryGetStr(context.Request["x_first_name"]);
                string lastName = Utility.TryGetStr(context.Request["x_last_name"]);
                string address1 = Utility.TryGetStr(context.Request["x_address"]);
                string address2 = null; //Utility.TryGetStr(context.Request["address2"]);
                string city = Utility.TryGetStr(context.Request["x_city"]);
                string state = Utility.TryGetStr(context.Request["x_state"]);
                string country = Utility.TryGetStr(context.Request["x_country"]);
                if (country != null && country.ToLower() == "united states") country = "US";
                string zip = Utility.TryGetStr(context.Request["x_zip"]);
                string phone = Utility.TryGetStr(context.Request["x_phone"]);
                string email = Utility.TryGetStr(context.Request["x_email"]);
                string ip = Utility.TryGetStr(context.Request["x_customer_ip"]);
                
                string affiliate = Utility.TryGetStr(context.Request["merchant_defined_field_1"]);
                string subAffiliate = Utility.TryGetStr(context.Request["merchant_defined_field_2"]);
                
                string internalID = Utility.TryGetStr(context.Request["x_cust_id"]);
                
                string creditCard = Utility.TryGetStr(context.Request["x_card_num"]);//
                string expString = Utility.TryGetStr(context.Request["x_exp_date"]);//
                string cvv = Utility.TryGetStr(context.Request["x_card_code"]);

                if (string.IsNullOrEmpty(firstName)) invalidInput.Add("x_first_name");
                if (string.IsNullOrEmpty(lastName)) invalidInput.Add("x_last_name");
                if (string.IsNullOrEmpty(address1)) invalidInput.Add("x_address");
                if (string.IsNullOrEmpty(city)) invalidInput.Add("x_city");
                if (string.IsNullOrEmpty(state)) invalidInput.Add("x_state");
                if (string.IsNullOrEmpty(zip)) invalidInput.Add("x_zip");
                //if (string.IsNullOrEmpty(phone)) insufficientFields.Add("phone");
                //if (string.IsNullOrEmpty(email)) insufficientFields.Add("email");
                //if (string.IsNullOrEmpty(ip)) insufficientFields.Add("ipadress");
                //if (string.IsNullOrEmpty(internalID)) invalidInput.Add("orderid");

                if (string.IsNullOrEmpty(creditCard)) invalidInput.Add("x_card_num");
                //if (string.IsNullOrEmpty(cvv)) invalidInput.Add("x_card_code");
                if (amount == null) invalidInput.Add("x_amount");
                if (context.Request["productTypeID"] != null && context.Request["productTypeID"].Trim().Length > 0 && productTypeID == null) invalidInput.Add("productTypeID");
                if (context.Request["productID"] != null && context.Request["productID"].Trim().Length > 0 && productID == null) invalidInput.Add("productID");

                int expMonth = 0;
                int expYear = 0;

                if (!string.IsNullOrEmpty(expString))
                {
                    if (Regex.IsMatch(expString, @"^\d{4}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = 2000 + int.Parse(expString.Substring(2));
                    }
                    else if (Regex.IsMatch(expString, @"^\d{2}/\d{2}$") || Regex.IsMatch(expString, @"^\d{2}-\d{2}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = 2000 + int.Parse(expString.Substring(3));
                    }
                    else if (Regex.IsMatch(expString, @"^\d{6}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = int.Parse(expString.Substring(2));
                    }
                    else if (Regex.IsMatch(expString, @"^\d{2}/\d{4}$") || Regex.IsMatch(expString, @"^\d{2}-\d{4}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = int.Parse(expString.Substring(3));
                    }
                    else
                    {
                        invalidInput.Add("x_exp_date");
                    }
                }
                else
                {
                    invalidInput.Add("x_exp_date");
                }

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response, version, delim_char, encap_char,
                        new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))),
                        function, internalID);
                }
                else
                {
                    //determine payment type
                    int paymentType = PaymentTypeEnum.Visa;
                    if (!string.IsNullOrEmpty(creditCard) && creditCard.ToCharArray()[0] == '5') paymentType = PaymentTypeEnum.Mastercard;

                    RenderResponse(context.Response, version, delim_char, encap_char,
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
            else if (function == FUNCTION_AUTH_ONLY)
            {
                decimal? amount = Utility.TryGetDecimal(context.Request["x_amount"]);

                int? productTypeID = Utility.TryGetInt(context.Request["productTypeID"]);

                string firstName = Utility.TryGetStr(context.Request["x_first_name"]);
                string lastName = Utility.TryGetStr(context.Request["x_last_name"]);
                string address1 = Utility.TryGetStr(context.Request["x_address"]);
                string address2 = null; //Utility.TryGetStr(context.Request["address2"]);
                string city = Utility.TryGetStr(context.Request["x_city"]);
                string state = Utility.TryGetStr(context.Request["x_state"]);
                string country = Utility.TryGetStr(context.Request["x_country"]);
                if (country != null && country.ToLower() == "united states") country = "US";
                string zip = Utility.TryGetStr(context.Request["x_zip"]);
                string phone = Utility.TryGetStr(context.Request["x_phone"]);
                string email = Utility.TryGetStr(context.Request["x_email"]);
                string ip = Utility.TryGetStr(context.Request["x_customer_ip"]);

                string affiliate = Utility.TryGetStr(context.Request["merchant_defined_field_1"]);
                string subAffiliate = Utility.TryGetStr(context.Request["merchant_defined_field_2"]);

                string internalID = Utility.TryGetStr(context.Request["x_cust_id"]);

                string creditCard = Utility.TryGetStr(context.Request["x_card_num"]);//
                string expString = Utility.TryGetStr(context.Request["x_exp_date"]);//
                string cvv = Utility.TryGetStr(context.Request["x_card_code"]);

                if (string.IsNullOrEmpty(firstName)) invalidInput.Add("x_first_name");
                if (string.IsNullOrEmpty(lastName)) invalidInput.Add("x_last_name");
                if (string.IsNullOrEmpty(address1)) invalidInput.Add("x_address");
                if (string.IsNullOrEmpty(city)) invalidInput.Add("x_city");
                if (string.IsNullOrEmpty(state)) invalidInput.Add("x_state");
                if (string.IsNullOrEmpty(zip)) invalidInput.Add("x_zip");
                //if (string.IsNullOrEmpty(phone)) insufficientFields.Add("phone");
                //if (string.IsNullOrEmpty(email)) insufficientFields.Add("email");
                //if (string.IsNullOrEmpty(ip)) insufficientFields.Add("ipadress");
                //if (string.IsNullOrEmpty(internalID)) invalidInput.Add("orderid");

                if (string.IsNullOrEmpty(creditCard)) invalidInput.Add("x_card_num");
                //if (string.IsNullOrEmpty(cvv)) invalidInput.Add("x_card_code");
                if (amount == null) invalidInput.Add("x_amount");
                if (context.Request["productTypeID"] != null && context.Request["productTypeID"].Trim().Length > 0 && productTypeID == null) invalidInput.Add("productTypeID");

                int expMonth = 0;
                int expYear = 0;

                if (!string.IsNullOrEmpty(expString))
                {
                    if (Regex.IsMatch(expString, @"^\d{4}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = 2000 + int.Parse(expString.Substring(2));
                    }
                    else if (Regex.IsMatch(expString, @"^\d{2}/\d{2}$") || Regex.IsMatch(expString, @"^\d{2}-\d{2}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = 2000 + int.Parse(expString.Substring(3));
                    }
                    else if (Regex.IsMatch(expString, @"^\d{6}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = int.Parse(expString.Substring(2));
                    }
                    else if (Regex.IsMatch(expString, @"^\d{2}/\d{4}$") || Regex.IsMatch(expString, @"^\d{2}-\d{4}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = int.Parse(expString.Substring(3));
                    }
                    else
                    {
                        invalidInput.Add("x_exp_date");
                    }
                }
                else
                {
                    invalidInput.Add("x_exp_date");
                }

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response, version, delim_char, encap_char,
                        new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))),
                        function, internalID);
                }
                else
                {
                    //determine payment type
                    int paymentType = PaymentTypeEnum.Visa;
                    if (!string.IsNullOrEmpty(creditCard) && creditCard.ToCharArray()[0] == '5') paymentType = PaymentTypeEnum.Mastercard;

                    RenderResponse(context.Response, version, delim_char, encap_char,
                        AuthOnly(username, password, amount.Value, null,
                            productTypeID,
                            firstName, lastName,
                            address1, address2, city, state, zip, country,
                            phone, email, ip, affiliate, subAffiliate, internalID,
                            paymentType, creditCard, cvv, expMonth, expYear),
                        function, internalID);
                }
            }
            else if (function == FUNCTION_CREATE_SUBSCRIPTION)
            {
                int? planID = Utility.TryGetInt(context.Request["planID"]);
                bool? chargeForTrial = TryGetBool(context.Request["chargeForTrial"]);

                string firstName = Utility.TryGetStr(context.Request["x_first_name"]);
                string lastName = Utility.TryGetStr(context.Request["x_last_name"]);
                string address1 = Utility.TryGetStr(context.Request["x_address"]);
                string address2 = null; //Utility.TryGetStr(context.Request["address2"]);
                string city = Utility.TryGetStr(context.Request["x_city"]);
                string state = Utility.TryGetStr(context.Request["x_state"]);
                string country = Utility.TryGetStr(context.Request["x_country"]);
                if (country != null && country.ToLower() == "united states") country = "US";
                string zip = Utility.TryGetStr(context.Request["x_zip"]);
                string phone = Utility.TryGetStr(context.Request["x_phone"]);
                string email = Utility.TryGetStr(context.Request["x_email"]);
                string ip = Utility.TryGetStr(context.Request["x_customer_ip"]);

                string affiliate = Utility.TryGetStr(context.Request["merchant_defined_field_1"]);
                string subAffiliate = Utility.TryGetStr(context.Request["merchant_defined_field_2"]);

                string internalID = Utility.TryGetStr(context.Request["x_cust_id"]);

                string creditCard = Utility.TryGetStr(context.Request["x_card_num"]);//
                string expString = Utility.TryGetStr(context.Request["x_exp_date"]);//
                string cvv = Utility.TryGetStr(context.Request["x_card_code"]);


                if (planID == null) invalidInput.Add("planID");
                if (chargeForTrial == null) invalidInput.Add("chargeForTrial");

                if (string.IsNullOrEmpty(firstName)) invalidInput.Add("x_first_name");
                if (string.IsNullOrEmpty(lastName)) invalidInput.Add("x_last_name");
                if (string.IsNullOrEmpty(address1)) invalidInput.Add("x_address");
                if (string.IsNullOrEmpty(city)) invalidInput.Add("x_city");
                if (string.IsNullOrEmpty(state)) invalidInput.Add("x_state");
                if (string.IsNullOrEmpty(zip)) invalidInput.Add("x_zip");
                //if (string.IsNullOrEmpty(phone)) insufficientFields.Add("phone");
                //if (string.IsNullOrEmpty(email)) insufficientFields.Add("email");
                //if (string.IsNullOrEmpty(ip)) insufficientFields.Add("ipadress");
                //if (string.IsNullOrEmpty(internalID)) invalidInput.Add("orderid");

                if (string.IsNullOrEmpty(creditCard)) invalidInput.Add("x_card_num");
                //if (string.IsNullOrEmpty(cvv)) invalidInput.Add("x_card_code");

                int expMonth = 0;
                int expYear = 0;

                if (!string.IsNullOrEmpty(expString))
                {
                    if (Regex.IsMatch(expString, @"^\d{4}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = 2000 + int.Parse(expString.Substring(2));
                    }
                    else if (Regex.IsMatch(expString, @"^\d{2}/\d{2}$") || Regex.IsMatch(expString, @"^\d{2}-\d{2}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = 2000 + int.Parse(expString.Substring(3));
                    }
                    else if (Regex.IsMatch(expString, @"^\d{6}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = int.Parse(expString.Substring(2));
                    }
                    else if (Regex.IsMatch(expString, @"^\d{2}/\d{4}$") || Regex.IsMatch(expString, @"^\d{2}-\d{4}$"))
                    {
                        expMonth = int.Parse(expString.Substring(0, 2));
                        expYear = int.Parse(expString.Substring(3));
                    }
                    else
                    {
                        invalidInput.Add("x_exp_date");
                    }
                }
                else
                {
                    invalidInput.Add("x_exp_date");
                }

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response, version, delim_char, encap_char,
                        new BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))),
                        function, internalID);
                }
                else
                {
                    //determine payment type
                    int paymentType = PaymentTypeEnum.Visa;
                    if (!string.IsNullOrEmpty(creditCard) && creditCard.ToCharArray()[0] == '5') paymentType = PaymentTypeEnum.Mastercard;

                    RenderResponse(context.Response, version, delim_char, encap_char,
                        CreateSubscription(username, password, 
                            planID.Value, chargeForTrial.Value,
                            firstName, lastName,
                            address1, address2, city, state, zip, country,
                            phone, email, ip, affiliate, subAffiliate, internalID,
                            paymentType, creditCard, cvv, expMonth, expYear),
                        function, internalID);
                }
            }
            else if (function == FUNCTION_VOID)
            {
                long? chargeHistoryID = Utility.TryGetInt(context.Request["x_trans_id"]);

                if (chargeHistoryID == null) invalidInput.Add("x_trans_id");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response, version, delim_char, encap_char,
                        new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))),
                        function, string.Empty);
                }
                else
                {
                    string internalID = null;
                    RenderResponse(context.Response, version, delim_char, encap_char,
                        Void(username, password, chargeHistoryID.Value, out internalID),
                        function, internalID);
                }
            }
            else if (function == FUNCTION_REFUND)
            {
                long? chargeHistoryID = Utility.TryGetInt(context.Request["x_trans_id"]);
                decimal? refundAmount = Utility.TryGetDecimal(context.Request["x_amount"]);

                if (chargeHistoryID == null) invalidInput.Add("x_trans_id");
                if (refundAmount == null) invalidInput.Add("x_amount");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response, version, delim_char, encap_char,
                        new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))),
                        function, string.Empty);
                }
                else
                {
                    string internalID = null;
                    RenderResponse(context.Response, version, delim_char, encap_char,
                        Refund(username, password, chargeHistoryID.Value, refundAmount.Value, out internalID),
                        function, internalID);
                }
            }
            else if (function == FUNCTION_CAPTURE)
            {
                long? chargeHistoryID = Utility.TryGetInt(context.Request["x_trans_id"]);
                decimal? amount = Utility.TryGetDecimal(context.Request["x_amount"]);

                if (chargeHistoryID == null) invalidInput.Add("x_trans_id");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response, version, delim_char, encap_char,
                        new BusinessError<ChargeHistoryEx>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))),
                        function, string.Empty);
                }
                else
                {
                    string internalID = null;
                    RenderResponse(context.Response, version, delim_char, encap_char,
                        Capture(username, password, chargeHistoryID.Value, amount, out internalID),
                        function, internalID);
                }
            }
            else
            {
                RenderResponse(context.Response, version, delim_char, encap_char, new BusinessError<ChargeHistoryEx>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = string.Format("Unrecognized transaction type: {0}.", context.Request["x_type"])
                },
                string.Empty, string.Empty);
            }
        }

        private decimal GetAmountOfChargeHistory(ChargeHistoryEx chargeHistory)
        {
            return GetAmountOfChargeHistory(new Set<ChargeHistoryEx, FailedChargeHistoryView>() { 
                Value1 = chargeHistory
            });
        }

        private decimal GetAmountOfChargeHistory(Set<ChargeHistoryEx, FailedChargeHistoryView> chargeHistory)
        {
            if (chargeHistory == null || chargeHistory.Value1 == null && chargeHistory.Value2 == null)
            {
                return 0.0M;
            }
            decimal res = 0.0M;
            if (chargeHistory.Value1 != null)
            {
                if (chargeHistory.Value1.ChargeTypeID == ChargeTypeEnum.AuthOnly ||
                    chargeHistory.Value1.ChargeTypeID == ChargeTypeEnum.VoidAuthOnly)
                {
                    AuthOnlyChargeDetails cd = saleService.Load<AuthOnlyChargeDetails>(chargeHistory.Value1.ChargeHistoryID);
                    if (cd != null && cd.RequestedAmount != null)
                    {
                        res = cd.RequestedAmount.Value;
                    }
                }
                else
                {
                    res = chargeHistory.Value1.Amount.Value;
                }
            }
            else
            {
                AuthOnlyFailedChargeDetails cd = saleService.Load<AuthOnlyFailedChargeDetails>(chargeHistory.Value2.FailedChargeHistoryID);
                if (cd != null && cd.RequestedAmount != null)
                {
                    res = cd.RequestedAmount.Value;
                }
                else
                {
                    res = chargeHistory.Value2.Amount.Value;
                }
            }
            return Math.Abs(res);
        }

        private void RenderResponse(HttpResponse response, string version, string delimChar, string encapChar, BusinessError<ChargeHistoryEx> responseObject, string function, string internalID)
        {
            string res = string.Empty;
            if (version == "3.0") res = VERSION_30_RESPONSE.Replace(",", delimChar);
            if (version == "3.1") res = VERSION_31_RESPONSE.Replace(",", delimChar);

            string responseCode = string.Empty;
            string responseText = string.Empty;
            string authCode = string.Empty;
            string transactionId = string.Empty;
            string avs = string.Empty;
            string cvv = string.Empty;
            string amount = string.Empty;
            string transactionType = function.ToLower();
            string customerId = internalID;
            string aff = string.Empty;
            string subAff = string.Empty;

            // 0 - Response Code
            // 1 - Response Text
            // 2 - Authcode
            // 3 - Transaction ID
            // 4 - AVS
            // 5 - CVV
            // 6 - Amount
            // 7 - TransactionType
            // 8 - Customer ID
            // 9 - Aff
            // 10 - SubAff

            if (responseObject.ReturnValue == null && responseObject.State == BusinessErrorState.Error)
            {
                responseCode = "3";
                responseText = responseObject.ErrorMessage;
            }
            else
            {
                TrimFuel.Business.Gateways.NetworkMerchants.NetworkMerchantsGatewayResponseParams nmiParams = new TrimFuel.Business.Gateways.NetworkMerchants.NetworkMerchantsGatewayResponseParams(responseObject.ReturnValue.Response);
                if (responseObject.State == BusinessErrorState.Success)
                {
                    responseCode = "1";
                }
                else
                {
                    try { responseCode = nmiParams.GetParam("response"); } 
                    catch { }                    
                    if (responseCode != "2" && responseCode != "3") responseCode = "3";
                }
                try { responseText = nmiParams.GetParam("responsetext"); }
                catch { }
                authCode = responseObject.ReturnValue.AuthorizationCode;
                try { avs = nmiParams.GetParam("avsresponse"); }
                catch { }
                try { cvv = nmiParams.GetParam("cvvresponse"); }
                catch { }

                amount = GetAmountOfChargeHistory(responseObject.ReturnValue).ToString();
                transactionId = responseObject.ReturnValue.ChargeHistoryID.Value.ToString();
            }

            response.Write(string.Format(res,
                EncapsulateField(responseCode, delimChar, encapChar),
                EncapsulateField(responseText, delimChar, encapChar),
                EncapsulateField(authCode, delimChar, encapChar),
                EncapsulateField(transactionId, delimChar, encapChar),
                EncapsulateField(avs, delimChar, encapChar),
                EncapsulateField(cvv, delimChar, encapChar),
                EncapsulateField(amount, delimChar, encapChar),
                EncapsulateField(transactionType, delimChar, encapChar),
                EncapsulateField(customerId, delimChar, encapChar)));
        }

        private void RenderResponse(HttpResponse response, string version, string delimChar, string encapChar, BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> responseObject, string function, string internalID)
        {
            string res = string.Empty;
            if (version == "3.0") res = VERSION_30_RESPONSE.Replace(",", delimChar);
            if (version == "3.1") res = VERSION_31_RESPONSE.Replace(",", delimChar);

            string responseCode = string.Empty;
            string responseText = string.Empty;
            string authCode = string.Empty;
            string transactionId = string.Empty;
            string avs = string.Empty;
            string cvv = string.Empty;
            string amount = string.Empty;
            string transactionType = function.ToLower();
            string customerId = internalID;
            string aff = string.Empty;
            string subAff = string.Empty;

            // 0 - Response Code
            // 1 - Response Text
            // 2 - Authcode
            // 3 - Transaction ID
            // 4 - AVS
            // 5 - CVV
            // 6 - Amount
            // 7 - TransactionType
            // 8 - Customer ID
            // 9 - Aff
            // 10 - SubAff

            if ((responseObject.ReturnValue == null || responseObject.ReturnValue.Value1 == null && responseObject.ReturnValue.Value2 == null) && responseObject.State == BusinessErrorState.Error)
            {
                responseCode = "3";
                responseText = responseObject.ErrorMessage;
            }
            else
            {
                string nmiResponse = string.Empty;
                if (responseObject.ReturnValue.Value1 != null)
                {
                    nmiResponse = responseObject.ReturnValue.Value1.Response;
                }
                else if (responseObject.ReturnValue.Value2 != null)
                {
                    nmiResponse = responseObject.ReturnValue.Value2.Response;
                }

                TrimFuel.Business.Gateways.NetworkMerchants.NetworkMerchantsGatewayResponseParams nmiParams = new TrimFuel.Business.Gateways.NetworkMerchants.NetworkMerchantsGatewayResponseParams(nmiResponse);
                if (responseObject.State == BusinessErrorState.Success)
                {
                    responseCode = "1";
                }
                else
                {
                    try { responseCode = nmiParams.GetParam("response"); }
                    catch { }
                    if (responseCode != "2" && responseCode != "3") responseCode = "3";
                }
                
                try { responseText = nmiParams.GetParam("responsetext"); }
                catch { }
                
                if (responseObject.ReturnValue.Value1 != null)
                {
                    authCode = responseObject.ReturnValue.Value1.AuthorizationCode;
                }
                else
                {
                    try { authCode = nmiParams.GetParam("authcode"); }
                    catch { }
                }
                
                try { avs = nmiParams.GetParam("avsresponse"); }
                catch { }
                
                try { cvv = nmiParams.GetParam("cvvresponse"); }
                catch { }

                amount = GetAmountOfChargeHistory(responseObject.ReturnValue).ToString();
                transactionId = "0";
                if (responseObject.ReturnValue.Value1 != null)
                {
                    transactionId = responseObject.ReturnValue.Value1.ChargeHistoryID.Value.ToString();
                }
            }

            response.Write(string.Format(res,
                EncapsulateField(responseCode, delimChar, encapChar),
                EncapsulateField(responseText, delimChar, encapChar),
                EncapsulateField(authCode, delimChar, encapChar),
                EncapsulateField(transactionId, delimChar, encapChar),
                EncapsulateField(avs, delimChar, encapChar),
                EncapsulateField(cvv, delimChar, encapChar),
                EncapsulateField(amount, delimChar, encapChar),
                EncapsulateField(transactionType, delimChar, encapChar),
                EncapsulateField(customerId, delimChar, encapChar)));
        }

        private void RenderResponse(HttpResponse response, string version, string delimChar, string encapChar, BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>> responseObject, string function, string internalID)
        {
            if (responseObject.ReturnValue != null && (responseObject.ReturnValue.Value2 != null || responseObject.ReturnValue.Value3 != null))
            {
                RenderResponse(response, version, delimChar, encapChar, new BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>>() {
                    ReturnValue = new Set<ChargeHistoryEx,FailedChargeHistoryView>(){
                        Value1 = responseObject.ReturnValue.Value2,
                        Value2 = responseObject.ReturnValue.Value3,
                    },
                    State = responseObject.State,
                    ErrorMessage = responseObject.ErrorMessage
                }, function, internalID);
                return;
            }

            string res = string.Empty;
            if (version == "3.0") res = VERSION_30_RESPONSE.Replace(",", delimChar);
            if (version == "3.1") res = VERSION_31_RESPONSE.Replace(",", delimChar);

            string responseCode = string.Empty;
            string responseText = string.Empty;
            string authCode = string.Empty;
            string transactionId = string.Empty;
            string avs = string.Empty;
            string cvv = string.Empty;
            string amount = string.Empty;
            string transactionType = function.ToLower();
            string customerId = internalID;
            string aff = string.Empty;
            string subAff = string.Empty;

            // 0 - Response Code
            // 1 - Response Text
            // 2 - Authcode
            // 3 - Transaction ID
            // 4 - AVS
            // 5 - CVV
            // 6 - Amount
            // 7 - TransactionType
            // 8 - Customer ID
            // 9 - Aff
            // 10 - SubAff

            if (responseObject.State == BusinessErrorState.Error)
            {
                responseCode = "3";
                responseText = responseObject.ErrorMessage;
            }
            else
            {
                responseCode = "1";
            }

            response.Write(string.Format(res,
                EncapsulateField(responseCode, delimChar, encapChar),
                EncapsulateField(responseText, delimChar, encapChar),
                EncapsulateField(authCode, delimChar, encapChar),
                EncapsulateField(transactionId, delimChar, encapChar),
                EncapsulateField(avs, delimChar, encapChar),
                EncapsulateField(cvv, delimChar, encapChar),
                EncapsulateField(amount, delimChar, encapChar),
                EncapsulateField(transactionType, delimChar, encapChar),
                EncapsulateField(customerId, delimChar, encapChar)));
        }

        private string EncapsulateField(string field, string delimChar, string encapChar)
        {
            if (string.IsNullOrEmpty(field))
            {
                return string.Empty;
            }
            if (!field.Contains(delimChar))
            {
                return field;
            }
            return encapChar + field.Replace(encapChar, encapChar + encapChar) + encapChar;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public bool? TryGetBool(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            str = str.Trim().ToUpper();
            if (str == "1")
            {
                return true;
            }
            else if (str == "0")
            {
                return false;
            }
            else if (str == "TRUE")
            {
                return true;
            }
            else if (str == "FALSE")
            {
                return false;
            }
            else if (str == "T")
            {
                return true;
            }
            else if (str == "F")
            {
                return false;
            }
            else if (str == "YES")
            {
                return true;
            }
            else if (str == "NO")
            {
                return false;
            }
            else if (str == "Y")
            {
                return true;
            }
            else if (str == "N")
            {
                return false;
            }

            return null;
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

        public BusinessError<ChargeHistoryEx> Capture(string username, string password, long chargeHistoryID, decimal? amount, out string internalID)
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
            return billingService.DoCapture(chargeHistoryID, amount);
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

        public BusinessError<ChargeHistoryEx> AuthOnly(string username, string password, decimal amount, decimal? shippingAmount,
            int? productTypeID,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate, string internalID,
            int paymentType, string creditCard, string cvv, int expMonth, int expYear)
        {
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

            int productGroupID = PRODUCT_ID;
            if (productTypeID != null)
            {
                productGroupID = productTypeID.Value;
            }

            return billingService.AuthorizeTransaction(productGroupID, amount, 
                firstName, lastName,
                address1, address2, city, state, zip, country,
                phone, email, ip, affiliate, subAffiliate,
                internalID, null,
                paymentType, creditCard, cvv, expMonth, expYear);
        }

        public BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>> CreateSubscription(string username, string password,
            int planID, bool chargeForTrial,
            string firstName, string lastName,
            string address1, string address2, string city, string state, string zip, string country,
            string phone, string email, string ip, string affiliate, string subAffiliate, 
            string internalID,
            int paymentType, string creditCard, string cvv, int expMonth, int expYear)
        {
            BusinessError<TPClient> authResult = Membership.Authorise(TP_MODE_ID, username, password);
            if (authResult.State == BusinessErrorState.Error)
            {
                return new BusinessError<Set<BillingSubscription, ChargeHistoryEx, FailedChargeHistoryView>>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = authResult.ErrorMessage
                };
            }

            return billingService.CreateSubscription(planID, chargeForTrial,
                null,
                firstName, lastName, address1, address2, city, state, zip, country,
                phone, email, ip, affiliate, subAffiliate,
                internalID, null,
                paymentType, creditCard, cvv, expMonth, expYear,
                null);
        }
    }
}
