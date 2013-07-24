using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business.Utils;

namespace TrimFuel.Business.Gateways.NPFGateway
{
    public class NPFGateway2 : ShipmentGatewayBase
    {
        public override IList<ShipmentGatewayResult<ShipmentPackageSubmitResult>> SubmitShipments(IList<ShipmentPackageView> packageList, IDictionary<ShipperConfig.ID, ShipperConfig> config, bool testMode, CanContinueCallbackWithCount canContinue)
        {
            StringBuilder attach = new StringBuilder();
            attach.AppendLine(Utility.LoadFromEmbeddedResource(typeof(NPFGateway2), "HeaderTemplate.tpl"));
            ShipmentGatewayResult<ShipmentPackageSubmitResult> res = new ShipmentGatewayResult<ShipmentPackageSubmitResult>();
            res.PackageList = new List<ShipmentPackageSubmitResult>();
            foreach (var package in packageList)
            {
                if (package.SaleList != null && package.SaleList.Count > 0 && package.GetShipmentList().Count > 0)
                {
                    long? regID = package.SaleList[0].Sale.SaleID;
                    int orderNumber = 1;
                    foreach (var shipment in package.GetShipmentList().GroupBy(i => i.InventorySKU).Select(i => new Shipment() { SKU = i.Key, Name = i.First().InventoryName, Quantity = i.Count() }).ToList())
                    {
                        attach.AppendLine(PrepareRequestLine(package.Registration, package.Billing, shipment, regID.Value, orderNumber, package.SaleList[0].Sale.CreateDT.Value));
                        orderNumber++;
                    }
                    res.PackageList.Add(new ShipmentPackageSubmitResult()
                    {
                        ShipperRegID = regID.ToString(),
                        ShipmentIDList = package.GetShipmentList().Select(i => i.Shipment.ShipmentID.Value).ToList()
                    });
                }
            }

            if (res.PackageList.Count > 0)
            {
                bool res2 = true;                
                try
                {
                    if (testMode)
                    {
                        if (res2) res.Response = "Test NPF. SUCCESS";
                        else res.Response = "Test NPF. ERROR";
                    }
                    else
                    {
                        NPFGateway gtw = new NPFGateway(config);
                        res2 = gtw.PostShipments(attach.ToString());
                        if (res2) res.Response = "File successfuly sent";
                        else res.Response = "Error sending file to FTP";
                    }
                }
                catch (Exception ex)
                {
                    res.Response = ex.ToString();
                    res2 = false;
                }

                if (!res2)
                {
                    foreach (var item in res.PackageList)
                    {
                        item.ShipperRegID = null;
                    }
                }
            }

            return new List<ShipmentGatewayResult<ShipmentPackageSubmitResult>>(){
                res
            };
        }

        private string PrepareRequestField(string field)
        {
            if (field == null)
            {
                return null;
            }

            return field.Replace("\"", "\"\"");
        }

        private string PrepareRequestLine(Registration r, Billing b, Shipment inv, long regId, int orderNumber, DateTime saleDate)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(NPFGateway2), "LineTemplate.tpl");

            res = res.Replace("##SALE_ID##", regId.ToString() + "-" + orderNumber.ToString());
            res = res.Replace("##DATE##", saleDate.ToShortDateString());
            res = res.Replace("##FirstName##", PrepareRequestField(b.FirstName));
            res = res.Replace("##LastName##", PrepareRequestField(b.LastName));
            res = res.Replace("##Address1##", PrepareRequestField(b.Address1));
            res = res.Replace("##Address2##", PrepareRequestField(b.Address2));
            res = res.Replace("##Town##", PrepareRequestField(b.City));
            res = res.Replace("##State##", PrepareRequestField(b.State));
            res = res.Replace("##PostCode##", PrepareRequestField(b.Zip));
            res = res.Replace("##Country##", FixCountryISO2(b.Country));
            res = res.Replace("##PHONE##", PrepareRequestField(b.Phone));
            res = res.Replace("##EMAIL##", PrepareRequestField(b.Email));

            res = res.Replace("##QTY##", inv.Quantity.ToString());
            res = res.Replace("##BAR_CODE##", BarCodeBySKU(inv.SKU));
            res = res.Replace("##DESC##", inv.Name);
            res = res.Replace("##SKU##", inv.SKU);
            res = res.Replace("##SHIPPING_TYPE##", ShipperTypeBySKU(inv.SKU));

            return res;
        }

        public string FixCountryISO2(string country)
        {
            if (country == null)
                return "";
            return country.Replace("Australia", "AU");
        }

        public string BarCodeBySKU(string sku)
        {
            if (sku == "INT201")
                return "094922941169";
            if (sku == "INT209")
                return "094922941169";
            return "";
        }

        public string ShipperTypeBySKU(string sku)
        {
            //if (sku == "COL201")
            //    return "AP";

            //if (sku == "COL201T")
            //    return "AP";

            //return "EP";

            //2012-04-02: always return AP
            return "AP";
        }
    }
}
