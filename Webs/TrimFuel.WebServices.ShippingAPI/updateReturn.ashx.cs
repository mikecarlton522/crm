using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Business.Utils;
using TrimFuel.Business;

namespace TrimFuel.WebServices.ShippingAPI
{
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    //[WebService(Namespace = "http://tempuri.org/")]
    //[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class updateReturn : IHttpHandler
    {
        private ReturnSaleService service = new ReturnSaleService();

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            long? saleID = Utility.TryGetLong(context.Request["saleid"]);
            string reason = Utility.TryGetStr(HttpUtility.UrlDecode(context.Request["reason"]));

            if (saleID == null)
            {
                context.Response.Write("Error: Invalid SaleID");
            }
            else
            {
                BusinessError<bool> res = service.ReturnSale(saleID.Value, reason, null, 0);
                if (res.State == BusinessErrorState.Success)
                {
                    context.Response.Write("Success");
                }
                else
                {
                    context.Response.Write("Error: " + res.ErrorMessage);
                }
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
