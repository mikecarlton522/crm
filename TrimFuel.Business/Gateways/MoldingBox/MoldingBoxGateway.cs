using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using TrimFuel.Business.Utils;
using API = TrimFuel.Business.MBApi;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Gateways.MoldingBox
{
    public class MoldingBoxGateway
    {
        private IDictionary<ShipperConfig.ID, ShipperConfig> config = null;

        public MoldingBoxGateway(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            this.config = config;
        }

        public long? PostShipment(long saleID, Registration registration, string country, IList<InventoryView> inventories, out string request, out string response)
        {
            long? regID = null;

            request = null;
            response = null;

            API.MBAPI MBApi = new API.MBAPI();
            List<API.Shipment> ShipmentList = new List<API.Shipment>();
            API.Shipment ThisShipment = new API.Shipment();
            ThisShipment.OrderID = saleID.ToString();
            ThisShipment.Orderdate = DateTime.Now;

            /*!!!Company!!!*/
            ThisShipment.Company = "";

            ThisShipment.FirstName = registration.FirstName;
            ThisShipment.LastName = registration.LastName;
            ThisShipment.Address1 = registration.Address1;
            ThisShipment.Address2 = registration.Address2;
            ThisShipment.City = registration.City;
            ThisShipment.State = registration.State;
            ThisShipment.Zip = registration.Zip;
            ThisShipment.Country = country;
            ThisShipment.Email = registration.Email;
            ThisShipment.Phone = registration.Phone;
            ThisShipment.ShippingMethodID = (int) ShipMethodEnum.USPSFirstClass;
            List<API.Item> ItemList = new List<API.Item>();

            foreach (InventoryView i in inventories)
            {
                ItemList.Add(new API.Item()
                                 {
                                     SKU = i.SKU,
                                     Quantity = (i.Quantity != null ? i.Quantity.Value : 1),
                                     Description = i.Product
                                 });
            }

            ThisShipment.Items = ItemList.ToArray();
            ShipmentList.Add(ThisShipment);

            try
            {
                request = Utility.XmlSerializeUTF8(ThisShipment);
            }
            catch
            {
                request = "Can't serialize to XML";
            }

            List<API.Response> ResList =
                MBApi.Post_Shipment(config[ShipperConfigEnum.MB_ApiKey].Value, ShipmentList.ToArray()).ToList();
            API.Response res = ResList[0];
            if (res.SuccessfullyReceived)
            {
                regID = res.MBShipmentID;
            }

            try
            {
                response = Utility.XmlSerializeUTF8(res);
            }
            catch
            {
                response = "Can't serialize to XML";
            }
            
            return regID;
        }

        public string CheckTrackingNumber(int regID)
        {
            API.MBAPI MBApi = new API.MBAPI();
            List<int> MBShipmentIDList = new List<int>();
            MBShipmentIDList.Add(regID);
            List<API.StatusResponse> ResList =
                MBApi.Retrieve_Shipment_Status(config[ShipperConfigEnum.MB_ApiKey].Value, MBShipmentIDList.ToArray()).ToList();

            string trackingNumber = null;
            API.StatusResponse res = ResList[0];

            if (res.RequestSuccessfullyReceived && res.ShipmentExists && (res.ShipmentStatusID == 4))
            {
                trackingNumber = res.TrackingNumber;
            }
            return trackingNumber;
        }
    }
}
