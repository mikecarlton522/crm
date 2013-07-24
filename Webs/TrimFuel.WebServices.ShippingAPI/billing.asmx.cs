using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using TrimFuel.Business.Gateways;

namespace TrimFuel.WebServices.ShippingAPI
{
    /// <summary>
    /// Summary description for billing
    /// </summary>
    [WebService(Namespace = "http://trianglecrm.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class billing : System.Web.Services.WebService
    {
        JobService jobService = new JobService();
        SubscriptionPlanService subscriptionPlanService = new SubscriptionPlanService();
        MerchantService merchantService = new MerchantService();
        SaleService saleService = new SaleService();

        [WebMethod]
        public void ProcessVoidQueue()
        {
            jobService.ProcessVoidQueueReadyForRefund();
        }

        [WebMethod]
        public void UpdateAgrProjectedRevenuesNextMonth()
        {
            merchantService.UpdateAgrProjectedRevenuesNextMonth();
        }

        [WebMethod]
        public void ProcessSubscriptionPlans()
        {
            subscriptionPlanService.ProcessSubscriptionPlans();
        }

        [WebMethod]
        public void CreateReferer(int billingID)
        {
            RefererService refererService = new RefererService();

            Billing billing = refererService.Load<Billing>(billingID);

            if (billing != null)
            {
                refererService.CreateOrGetRefererFromBilling(billing, null);
            }

        }

        [WebMethod]
        public void DoBPagCancel(long chargeHistoryID)
        {
            var res = new SaleService().DoVoid(chargeHistoryID);
            if (res != null && res.State == BusinessErrorState.Success)
            {
                HttpContext.Current.Response.Write(string.Format("BPag Refund Success"));
            }
            else
            {
                HttpContext.Current.Response.Write("BPag Refund Error");
            }
        }

        [WebMethod]
        public RequestResponseObject PerformTxn(long billingID, int assertigyMIDID, int productID, decimal amount, long saleID, int installments)
        {
            PaymentExVars.Installments = installments;
            var res = saleService.PerformTxn(billingID, productID, amount, assertigyMIDID, saleID);
            return new RequestResponseObject()
                {
                     Request = HttpUtility.UrlEncode(res.ReturnValue.Request),
                     Response = HttpUtility.UrlEncode(res.ReturnValue.Response)
                };
        }
    }
}
