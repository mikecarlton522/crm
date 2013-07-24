//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Services;
//using TrimFuel.Business;
//using TrimFuel.Model;
//using TrimFuel.Model.Enums;
//using TrimFuel.WebServices.BillingAPI.Model;

//namespace TrimFuel.WebServices.BillingAPI
//{
//    /// <summary>
//    /// Summary description for billing_api
//    /// </summary>
//    [WebService(Namespace = "http://tempuri.org/")]
//    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
//    [System.ComponentModel.ToolboxItem(false)]
//    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
//    // [System.Web.Script.Services.ScriptService]
//    public class billing_api : System.Web.Services.WebService
//    {
//        private SaleService saleService = new SaleService();
//        private const int PRODUCT_ID = 1;

//        [WebMethod]
//        public BusinessError<ChargeHistory> Void(long chargeHistoryID)
//        {
//            BusinessError<ChargeHistoryEx> resEx = saleService.DoVoid(chargeHistoryID);
//            return new BusinessError<ChargeHistory>(){
//                ReturnValue = ChargeHistory.FromChargeHistoryEx(resEx.ReturnValue), 
//                State = resEx.State, 
//                ErrorMessage = resEx.ErrorMessage
//            };
//        }

//        [WebMethod]
//        public BusinessError<ChargeHistory> Refund(long chargeHistoryID, decimal refundAmount)
//        {
//            BusinessError<ChargeHistoryEx> resEx = saleService.DoRefund(chargeHistoryID, refundAmount);
//            return new BusinessError<ChargeHistory>()
//            {
//                ReturnValue = ChargeHistory.FromChargeHistoryEx(resEx.ReturnValue),
//                State = resEx.State,
//                ErrorMessage = resEx.ErrorMessage
//            };
//        }

//        [WebMethod]
//        public BusinessError<ChargeHistory> Charge(decimal amount, decimal shipping, [System.Xml.Serialization.XmlIgnore]bool shippingSpecified, 
//            string firstName, string lastName,
//            string address1, string address2, string city, string state, string zip,
//            string phone, string email, string ip, string affiliate, string subAffiliate, string internalID,
//            int paymentType, string creditCard, string cvv, int expMonth, int expYear)
//        {
//            decimal? shippingAmount = null;
//            if (shippingSpecified)
//            {
//                shippingAmount = shipping;
//            }

//            BusinessError<ChargeHistoryEx> resEx = saleService.BillAsUpsell_Old(PRODUCT_ID, amount, shippingAmount, firstName, lastName,
//                address1, address2, city, state, zip, 
//                phone, email, ip, affiliate, subAffiliate, internalID,
//                paymentType, creditCard, cvv, expMonth, expYear);
//            return new BusinessError<ChargeHistory>()
//            {
//                ReturnValue = ChargeHistory.FromChargeHistoryEx(resEx.ReturnValue),
//                State = resEx.State,
//                ErrorMessage = resEx.ErrorMessage
//            };
//        }
//    }
//}
