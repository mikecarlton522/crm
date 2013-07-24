using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using TrimFuel.Business;

namespace TrimFuel.WebServices
{
    /// <summary>
    /// Summary description for ABFService_
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class ABFService_ : System.Web.Services.WebService
    {
        private ShipperService shipperService = new ABFService();

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
    }
}
