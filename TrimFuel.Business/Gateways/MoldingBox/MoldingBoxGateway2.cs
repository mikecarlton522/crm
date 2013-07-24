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
using TrimFuel.Model.Containers;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business.Gateways.MoldingBox
{
    public class MoldingBoxGateway2 : ShipmentGatewayOneByOne
    {
        private const string TEST_MODE_URL = "http://localhost/gateways/moldingbox/";

        protected override SubmitResult SubmitShipment(Person person, Address address, string email, string phone, IList<ShipmentGatewayBase.Shipment> products, IList<int> shippingOptionList, Product productGroup, DateTime orderDate, long uniqueID, SubmitAdditionalData additionalData, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode)
        {
            long? regID = null;

            string request = null;
            string response = null;
            
            List<API.Shipment> ShipmentList = new List<API.Shipment>();
            API.Shipment ThisShipment = new API.Shipment();
            ThisShipment.OrderID = uniqueID.ToString();
            ThisShipment.Orderdate = orderDate;

            /*!!!Company!!!*/
            ThisShipment.Company = "";

            ThisShipment.FirstName = person.FirstName;
            ThisShipment.LastName = person.LastName;
            ThisShipment.Address1 = address.Address1;
            ThisShipment.Address2 = address.Address2;
            ThisShipment.City = address.City;
            ThisShipment.State = address.State;
            ThisShipment.Zip = address.Zip;
            ThisShipment.Country = address.Country;
            ThisShipment.Email = email;
            ThisShipment.Phone = phone;
            ThisShipment.ShippingMethodID = (int)ShipMethodEnum.USPSFirstClass;
            List<API.Item> ItemList = new List<API.Item>();

            foreach (var i in products)
            {
                ItemList.Add(new API.Item()
                {
                    SKU = i.SKU,
                    Quantity = i.Quantity,
                    Description = i.Name
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

            if (testMode)
            {
                try
                {
                    response = HTTPPost(TEST_MODE_URL + "Post_Shipment.aspx",
                        "ApiKey=" + config[ShipperConfigEnum.MB_ApiKey].Value + "&" +
                        "Shipments=" + Utility.XmlSerializeUTF8(ShipmentList.ToArray()));
                    regID = Utility.TryGetInt(response);
                }
                catch (Exception ex)
                {
                    response = ex.ToString();
                }
            }
            else
            {
                using (API.MBAPI MBApi = new API.MBAPI())
                {
                    MBApi.Timeout = 4 * 60 * 1000;
                    try
                    {
                        List<API.Response> ResList = MBApi.Post_Shipment(config[ShipperConfigEnum.MB_ApiKey].Value, ShipmentList.ToArray()).ToList();
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
                    }
                    catch (Exception ex)
                    {
                        response = "Error: " + ex.Message;
                    }
                }
            }

            return new SubmitResult()
            {
                Request = request,
                Response = response,
                ShipperRegID = (regID != null ? regID.ToString() : null)
            };
        }

        public override bool IsCheckShippedImplemented
        {
            get
            {
                return true;
            }
        }


        public override IList<ShipmentGatewayResult<ShipmentPackageShipResult>> CheckShipped(IList<string> shipperRegIDList, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallback canContinue)
        {
            ShipmentGatewayResult<ShipmentPackageShipResult> res = new ShipmentGatewayResult<ShipmentPackageShipResult>();
            int[] MBShipmentIDList = shipperRegIDList.Select(i => Convert.ToInt32(i)).ToArray();
            try
            {
                res.Request = Utility.XmlSerializeUTF8(MBShipmentIDList);
            }
            catch
            {
                res.Request = "Can't serialize to XML";
            }
            res.PackageList = new List<ShipmentPackageShipResult>();

            if (testMode)
            {
                try
                {
                    string response = HTTPPost(TEST_MODE_URL + "Retrieve_Shipment_Status.aspx",
                        "ApiKey=" + config[ShipperConfigEnum.MB_ApiKey].Value + "&" +
                        "MBShipmentIDs=" + res.Request
                        );
                    res.Response = response;
                    if (res.Response.Trim() != "")
                        res.PackageList = (
                            from regID in shipperRegIDList
                            select new ShipmentPackageShipResult()
                            {
                                ShipperRegID = regID,
                                TrackingNumber = res.Response,
                                ShipDT = DateTime.Now
                            }).ToList();
                }
                catch (Exception ex)
                {
                    res.Response = ex.ToString();
                }
            }
            else
            {
                using (API.MBAPI MBApi = new API.MBAPI())
                {
                    try
                    {
                        List<API.StatusResponse> ResList = MBApi.Retrieve_Shipment_Status(config[ShipperConfigEnum.MB_ApiKey].Value, MBShipmentIDList).ToList();
                        res.PackageList = (
                            from regID in shipperRegIDList
                            join order in ResList.Where(j => j.RequestSuccessfullyReceived && j.ShipmentExists && j.ShipmentStatusID == 4) on regID equals order.MBShipmentID.ToString()
                            select new ShipmentPackageShipResult()
                            {
                                ShipperRegID = regID,
                                TrackingNumber = order.TrackingNumber,
                                ShipDT = DateTime.Now
                            }).ToList();
                    }
                    catch (Exception ex)
                    {
                        res.Response = ex.ToString();
                    }
                }
                string trackingNumber = null;
            }


            return new List<ShipmentGatewayResult<ShipmentPackageShipResult>>() 
            { 
                res 
            };
        }
    }
}
