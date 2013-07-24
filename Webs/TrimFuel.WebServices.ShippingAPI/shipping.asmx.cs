using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TrimFuel.Business;

namespace TrimFuel.WebServices.ShippingAPI
{
    /// <summary>
    /// Summary description for shipping
    /// </summary>
    [WebService(Namespace = "http://trianglecrm.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class shipping : System.Web.Services.WebService
    {
        GeneralShipperService shipperService = new GeneralShipperService();

        [WebMethod]
        public BusinessError<bool> SendSale(long saleID)
        {
            return shipperService.SendSale(saleID);
        }

        [WebMethod]
        public BusinessError<bool> SendUpsells(long billingID)
        {
            return shipperService.SendUpsells(billingID);
        }

        [WebMethod]
        public BusinessError<bool> SendExtraTrialShips(long billingID)
        {
            return shipperService.SendExtraTrialShips(billingID);
        }

        [WebMethod]
        public BusinessError<bool> SendExtraTrialShipsUsingCustomShipper(long billingID, int shipperID)
        {
            return shipperService.SendExtraTrialShips(billingID, shipperID);
        }

        [WebMethod]
        public void SendPendingShipments()
        {
            shipperService.SendPendingOrders();
        }

        [WebMethod]
        public void CheckShipmentsState()
        {
            shipperService.CheckShipmentsState();
        }

        [WebMethod]
        public void CheckReturns()
        {
            shipperService.CheckReturns();
        }

        [WebMethod]
        public void DailyShipmentReport()
        {
            new ReportService().DailyShipmentReport();
        }

        [WebMethod]
        public void UpdateTrackingNumber(long saleid, string trackingNumber)
        {
            if (saleid == 0 || string.IsNullOrEmpty(trackingNumber))
                return;

            new GeneralShipperService().UpdateTrackingNumber(saleid, trackingNumber);
        }
    }
}
