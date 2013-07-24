using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using System.Net;

namespace TrimFuel.Business.Gateways
{
    public abstract class ShipmentGatewayBase : IShipmentGateway
    {
        protected const string DEFAULT_COUNTRY = "United States";

        protected class Shipment
        {
            public string SKU { get; set; }
            public string Name { get; set; }
            public int Quantity { get; set; }
        }

        public abstract IList<ShipmentGatewayResult<ShipmentPackageSubmitResult>> SubmitShipments(IList<ShipmentPackageView> packageList, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallbackWithCount canContinue);
        public virtual IList<ShipmentGatewayResult<ShipmentPackageShipResult>> CheckShipped(IList<string> shipperRegIDList, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallback canContinue)
        {
            return null;
        }
        public virtual bool IsCheckShippedImplemented
        {
            get { return false; }
        }
        public virtual IList<ShipmentGatewayResult<ShipmentPackageReturnResult>> CheckReturned(IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallback canContinue)
        {
            return null;
        }
        public virtual bool IsCheckReturnedImplemented
        {
            get { return false; }
        }

        protected string HTTPPost(string url, string data)
        {
            string res = null;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
                res = wc.UploadString(url, "POST", data);

            }
            return res;
        }

        protected string HTTPGet(string url)
        {
            string res = null;
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;
                res = wc.DownloadString(url);

            }
            return res;
        }
    }
}
