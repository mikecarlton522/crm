using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Business;
using TrimFuel.Business.Utils;
using TrimFuel.Business.Flow;
using TrimFuel.Model;

namespace TrimFuel.WebServices.ShippingAPI
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    //[WebService(Namespace = "http://tempuri.org/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class updateTrackingNumber : IHttpHandler
    {
        private GeneralShipperService service = new GeneralShipperService();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            long? saleID = Utility.TryGetLong(context.Request["saleid"]);
            string trackingNumber = Utility.TryGetStr(context.Request["trackingnumber"]);

            if (saleID == null)
            {
                context.Response.Write("Error: Invalid SaleID");
                return;
            }
            
            if (trackingNumber == null)
            {
                context.Response.Write("Error: Invalid TrackingNumber");
                return;
            }

            OrderService os = new OrderService();
            SaleFlow sf = new SaleFlow();

            Shipment shipment = os.GetSaleShipments(saleID.Value).FirstOrDefault();

            if (shipment == null)
            {
                if (service.UpdateTrackingNumber(saleID.Value, trackingNumber))
                    context.Response.Write("Success");
                else
                    context.Response.Write("Error");
            }
            else
            {
                if (sf.UpdateTrackingNumber(saleID, trackingNumber, shipment.ShipperID, "API"))
                    context.Response.Write("Success");
                else
                    context.Response.Write("Error");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
