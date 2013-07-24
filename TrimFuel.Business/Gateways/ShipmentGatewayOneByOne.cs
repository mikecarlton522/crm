using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using TrimFuel.Model;
using TrimFuel.Model.Containers;

namespace TrimFuel.Business.Gateways
{
    public abstract class ShipmentGatewayOneByOne : ShipmentGatewayBase
    {
        protected class SubmitAdditionalData
        {
            public long? BillingID { get; set; }

            public static SubmitAdditionalData CreateFromPackage(ShipmentPackageView package)
            {
                SubmitAdditionalData res = new SubmitAdditionalData();
                res.BillingID = (package.Billing != null ? package.Billing.BillingID : null);
                return res;
            }
        }

        protected abstract class Result
        {
            public string Request { get; set; }
            public string Response { get; set; }
        }

        protected class SubmitResult : Result
        {
            public string ShipperRegID { get; set; }
        }

        protected class ShipResult : Result
        {
            public string TrackingNumber { get; set; }
            public DateTime? ShippedDT { get; set; }
        }

        protected virtual string FixState(string state)
        {
            return state ?? "";
        }

        protected virtual string FixCountry(string country)
        {
            return country ?? "";
        }

        public override IList<ShipmentGatewayResult<ShipmentPackageSubmitResult>> SubmitShipments(IList<ShipmentPackageView> packageList, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallbackWithCount canContinue)
        {
            IList<ShipmentGatewayResult<ShipmentPackageSubmitResult>> res = new List<ShipmentGatewayResult<ShipmentPackageSubmitResult>>();

            int total = packageList.Count;
            int processed = 0;
            foreach (var item in packageList)
            {
                SubmitResult res2 = SubmitShipment(item, config, testMode);
                //res2 == null - no attempt occurred
                if (res2 != null)
                {
                    res.Add(new ShipmentGatewayResult<ShipmentPackageSubmitResult>()
                    {
                        Request = res2.Request,
                        Response = res2.Response,
                        PackageList = new List<ShipmentPackageSubmitResult>(){
                            new ShipmentPackageSubmitResult(){
                                ShipperRegID = res2.ShipperRegID,
                                ShipmentIDList = item.GetShipmentList().Select(i => i.Shipment.ShipmentID.Value).ToList()
                            }
                        }
                    });
                }
                processed++;
                //back control
                if (canContinue != null && !canContinue((decimal)processed / total, processed))
                {
                    break;
                }
            }

            return res;
        }

        private SubmitResult SubmitShipment(ShipmentPackageView package, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode)
        {
            if (package == null ||
                package.Registration == null ||
                package.Product == null ||
                package.SaleList == null || package.SaleList.Count == 0 ||
                package.GetShipmentList() == null || package.GetShipmentList().Count == 0)
            {
                return null;
            }

            SubmitResult res = new SubmitResult();
            try
            {
                Address address = Address.Create(package.Registration, package.RegistrationInfo);
                address.State = FixState(address.State);
                address.Country = FixCountry(address.Country);
                res = SubmitShipment(
                    Person.Create(package.Registration),
                    address,
                    package.Registration.Email, 
                    package.Registration.Phone,
                    package.GetShipmentList().GroupBy(i => i.InventorySKU).Select(i => new Shipment() { SKU = i.Key, Name = i.First().InventoryName, Quantity = i.Count() }).ToList(),
                    package.SaleList.Where(i => i.ShippingOption != null).Select(i => i.ShippingOption.ShippingOptionID.Value).ToList(),
                    package.Product,
                    package.SaleList.First().Sale.CreateDT.Value,
                    package.SaleList.First().Sale.SaleID.Value,
                    SubmitAdditionalData.CreateFromPackage(package),
                    config, testMode);
                if (res == null)
                {
                    throw new Exception("SendShipment returned null result");
                }
            }
            catch (Exception ex)
            {
                res.Response = ex.ToString();
            }

            return res;
        }

        /// <summary>
        /// Sends one shipment package to shipper using shipper API
        /// </summary>
        /// <returns>
        /// Request, Response - in any case, even in case of error
        /// ShipperRegID - only in case of successful submit
        /// </returns>
        protected abstract SubmitResult SubmitShipment(Person person, Address address, string email, string phone, IList<Shipment> products, IList<int> shippingOptionList, Product productGroup, DateTime orderDate, long uniqueID, SubmitAdditionalData additionalData, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode);

        public override IList<ShipmentGatewayResult<ShipmentPackageShipResult>> CheckShipped(IList<string> shipperRegIDList, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallback canContinue)
        {
            IList<ShipmentGatewayResult<ShipmentPackageShipResult>> res = new List<ShipmentGatewayResult<ShipmentPackageShipResult>>();

            int total = shipperRegIDList.Count;
            int processed = 0;
            foreach (string shipperRegID in shipperRegIDList)
            {
                if (!string.IsNullOrEmpty(shipperRegID))
                {
                    ShipResult t = new ShipResult();
                    try
                    {
                        t = CheckShipped(shipperRegID, config, testMode);
                    }
                    catch (Exception ex)
                    {
                        t.Response = ex.ToString();
                    }
                    res.Add(new ShipmentGatewayResult<ShipmentPackageShipResult>()
                    {
                        Request = t.Request,
                        Response = t.Response,
                        PackageList = new List<ShipmentPackageShipResult>()
                        {
                            new ShipmentPackageShipResult()
                            {
                                ShipperRegID = shipperRegID,
                                TrackingNumber = t.TrackingNumber,
                                ShipDT = t.ShippedDT
                            }
                        }
                    });
                }
                processed++;
                //back control
                if (canContinue != null && !canContinue(processed / total))
                {
                    break;
                }
            }

            return res;
        }

        /// <summary>
        /// Checks one shipment package for "Shipped" state and retrieves Tracking Number using shipper API
        /// </summary>
        /// <returns>
        /// Request, Response - in any case, even in case of error
        /// TrackingNumber and ShippedDT - only in case of package is shipped
        /// </returns>
        protected virtual ShipResult CheckShipped(string shipperRegID, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode)
        {
            return null;
        }
    }
}
