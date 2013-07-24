using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TrimFuel.Business;
using TrimFuel.Business.Flow;
using TrimFuel.Model.Views;
using TrimFuel.WebServices.ShippingAPI.Model;

namespace TrimFuel.WebServices.ShippingAPI
{
    /// <summary>
    /// Summary description for order
    /// </summary>
    [WebService(Namespace = "http://trianglecrm.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class order : System.Web.Services.WebService
    {
        [WebMethod]
        public BusinessError<bool> ProcessOrder(long orderID)
        {
            var res1 = new OrderFlow().ProcessOrder(new OrderBuilder().LoadByID(orderID).Order);
            return new BusinessError<bool>() { 
                State = res1.State,
                ErrorMessage = res1.ErrorMessage,
                ReturnValue = (res1.State == BusinessErrorState.Success)
            };
        }

        [WebMethod]
        public void AutoProcessOrders()
        {
            new OrderService().AutoProcessOrders();
        }

        [WebMethod]
        public BusinessError<ChargeHistoryList> UpdateAccount(long billingID, bool scrubOrders, bool blockShipments, bool voidTransactions, int adminID)
        {
            var res1 = new OrderService().UpdateAccount(billingID, scrubOrders, blockShipments, voidTransactions, adminID);
            BusinessError<ChargeHistoryList> res = new BusinessError<ChargeHistoryList>() 
            {
                State = res1.State,
                ErrorMessage = res1.ErrorMessage,
                ReturnValue = ChargeHistoryList.FromChargeHistoryViewList(res1.ReturnValue)
            };
            if (res1.ReturnValue != null && res1.ReturnValue.Count > 0)
            {
                res.ErrorMessage += string.Join("##", res1.ReturnValue.Select(i => i.ChargeHistory.Response).ToArray());
            }
            if (string.IsNullOrEmpty(res.ErrorMessage) && res.State == BusinessErrorState.Success) res.ErrorMessage = "Success";
            if (string.IsNullOrEmpty(res.ErrorMessage) && res.State == BusinessErrorState.Error) res.ErrorMessage = "Error";
            return res;
        }

        [WebMethod]
        public void SendShipments()
        {
            new OrderShipmentService().SubmitPendingShipments();
        }

        [WebMethod]
        public void CheckShippedShipments()
        {
            new OrderShipmentService().CheckShippedShipments();
        }

        [WebMethod]
        public void ProcessRebills()
        {
            new RebillService().ProcessRebills();
        }
    }
}
