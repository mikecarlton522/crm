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
using System.Xml.Serialization;
using TrimFuel.Model.Views;

namespace TrimFuel.WebServices.BillingAPI
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    public class billing_old : IHttpHandler
    {
        private SaleService saleService = new SaleService();
        private const int PRODUCT_ID = 1;
        private const int TP_MODE_ID = TrimFuel.Model.Enums.TPModeEnum.PostGet;

        private const string FUNCTION_CHARGE = "charge";
        private const string FUNCTION_REBILL = "rebill";
        private const string FUNCTION_VOID = "void";
        private const string FUNCTION_REFUND = "refund";
        private const string FUNCTION_CHARGELOOKUP = "chargelookup";
        private const string FUNCTION_USERLOOKUP = "userlookup";

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/xml";
            
            string function = context.Request["function"];
            if (function != null)
            {
                function = function.ToLower();
            }

            IList<string> invalidInput = new List<string>();

            string username = Utility.TryGetStr(context.Request["username"]);
            string password = Utility.TryGetStr(context.Request["password"]);

            if (function == FUNCTION_CHARGE)
            {
                decimal? amount = Utility.TryGetDecimal(context.Request["amount"]);
                decimal? shippingAmount = Utility.TryGetDecimal(context.Request["shipping"]);
                int? productTypeID = Utility.TryGetInt(context.Request["productTypeID"]);
                int? productID = Utility.TryGetInt(context.Request["productID"]);
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
                int? paymentType = Utility.TryGetInt(context.Request["paymentType"]);
                string creditCard = Utility.TryGetStr(context.Request["creditCard"]);
                string cvv = Utility.TryGetStr(context.Request["cvv"]);
                int? expMonth = Utility.TryGetInt(context.Request["expMonth"]);
                int? expYear = Utility.TryGetInt(context.Request["expYear"]);

                if (amount == null) invalidInput.Add("amount");
                if (context.Request["shipping"] != null && context.Request["shipping"].Trim().Length > 0 && shippingAmount == null) invalidInput.Add("shipping");
                if (context.Request["productTypeID"] != null && context.Request["productTypeID"].Trim().Length > 0 && productTypeID == null) invalidInput.Add("productTypeID");
                if (context.Request["productID"] != null && context.Request["productID"].Trim().Length > 0 && productID == null) invalidInput.Add("productID");
                if (paymentType == null) invalidInput.Add("paymentType");
                if (expMonth == null) invalidInput.Add("expMonth");
                if (expYear == null) invalidInput.Add("expYear");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response,
                        new BusinessError<ChargeHistory>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))));
                }
                else
                {
                    RenderResponse(context.Response, 
                        Charge(username, password, amount.Value, shippingAmount,
                            productTypeID,
                            productID,
                            firstName, lastName,
                            address1, address2, city, state, zip, country, 
                            phone, email, ip, affiliate, subAffiliate, internalID, 
                            paymentType.Value, creditCard, cvv, expMonth.Value, expYear.Value));
                }
            }
            else if (function == FUNCTION_REBILL)
            {
                decimal? amount = Utility.TryGetDecimal(context.Request["amount"]);
                string internalID = Utility.TryGetStr(context.Request["internalID"]);

                if (amount == null) invalidInput.Add("amount");
                if (string.IsNullOrEmpty(internalID)) invalidInput.Add("internalID");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response,
                        new BusinessError<ChargeHistory>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))));
                }
                else
                {
                    RenderResponse(context.Response,
                        Rebill(username, password, amount.Value,
                        internalID));
                }
            }
            else if (function == FUNCTION_VOID)
            {
                long? chargeHistoryID = Utility.TryGetInt(context.Request["chargeHistoryID"]);

                if (chargeHistoryID == null) invalidInput.Add("chargeHistoryID");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response,
                        new BusinessError<ChargeHistory>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))));
                }
                else
                {
                    RenderResponse(context.Response,
                        Void(username, password, chargeHistoryID.Value));
                }
            }
            else if (function == FUNCTION_REFUND)
            {
                long? chargeHistoryID = Utility.TryGetInt(context.Request["chargeHistoryID"]);
                decimal? refundAmount = Utility.TryGetDecimal(context.Request["refundAmount"]);

                if (chargeHistoryID == null) invalidInput.Add("chargeHistoryID");
                if (refundAmount == null) invalidInput.Add("refundAmount");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response,
                        new BusinessError<ChargeHistory>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))));
                }
                else
                {
                    RenderResponse(context.Response,
                        Refund(username, password, chargeHistoryID.Value, refundAmount.Value));
                }
            }
            else if (function == FUNCTION_CHARGELOOKUP)
            {
                long? chargeHistoryID = Utility.TryGetInt(context.Request["chargeHistoryID"]);

                if (chargeHistoryID == null) invalidInput.Add("chargeHistoryID");

                if (invalidInput.Count > 0)
                {
                    RenderResponse(context.Response,
                        new BusinessError<ChargeHistory>(null, BusinessErrorState.Error,
                            string.Format("Invalid parameters. The following fields are missed or have invalid format: {0}.", string.Join(", ", invalidInput.ToArray()))));
                }
                else
                {
                    RenderResponse(context.Response,
                        ChargeLookup(username, password, chargeHistoryID.Value));
                }
            }
            else if (function == FUNCTION_USERLOOKUP)
            {
                string internalID = Utility.TryGetStr(context.Request["internalID"]);

                RenderResponse(context.Response,
                    UserLookup(username, password, internalID));
            }
            else
            {
                RenderResponse(context.Response, new BusinessError<ChargeHistory>()
                {
                    ReturnValue = null,
                    State = BusinessErrorState.Error,
                    ErrorMessage = string.Format("Unrecognized function \"{0}\"", context.Request["function"])
                });
            }
        }

        private void RenderResponse(HttpResponse response, BusinessError<ChargeHistory> responseObject)
        {
            XmlSerializer objSerializer = new XmlSerializer(typeof(BusinessError<ChargeHistory>), "http://trianglecrm.com");
            objSerializer.Serialize(response.Output, responseObject);
        }

        private void RenderResponse(HttpResponse response, BusinessError<ChargeHistoryList> responseObject)
        {
            XmlSerializer objSerializer = new XmlSerializer(typeof(BusinessError<ChargeHistoryList>), "http://trianglecrm.com");
            objSerializer.Serialize(response.Output, responseObject);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

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

        public BusinessError<ChargeHistory> Charge(string username, string password, decimal amount, decimal? shippingAmount, 
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
                return new BusinessError<ChargeHistory>()
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

            BusinessError<Set<ChargeHistoryEx, FailedChargeHistoryView>> resEx = saleService.BillAsUpsell(productGroupID, productID, amount, shippingAmount,
                null,
                firstName, lastName,
                address1, address2, city, state, zip, country,
                phone, email, ip, affiliate, subAffiliate, 
                internalID, null,
                paymentType, creditCard, cvv, expMonth, expYear, null,
                null, null, null, null, null);
            return new BusinessError<ChargeHistory>()
            {
                ReturnValue = ChargeHistory.FromChargeHistoryEx(resEx.ReturnValue),
                State = resEx.State,
                ErrorMessage = resEx.ErrorMessage
            };
        }

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
    }
}
