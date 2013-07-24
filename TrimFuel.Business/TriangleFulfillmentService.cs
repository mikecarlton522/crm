using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class TriangleFulfillmentService : ABFService
    {
        public override int ShipperID
        {
            get { return TrimFuel.Model.Enums.ShipperEnum.TF; }
        }

        public override string TableName
        {
            get { return "TFRecord"; }
        }

        protected override void OnOrderSent(IList<SaleShipperView> saleList, ABFApi.Order order, string response, long? regID)
        {
            try
            {
                dao.BeginTransaction();

                foreach (SaleShipperView sale in saleList)
                {
                    TFRecord tffRecord = new TFRecord();

                    tffRecord.SaleID = sale.SaleID;
                    try
                    {
                        tffRecord.Request = Utility.XmlSerializeUTF8(order);
                    }
                    catch
                    {
                        tffRecord.Request = "Can't serialize to XML";
                    }
                    tffRecord.Response = response;
                    tffRecord.RegID = regID;
                    tffRecord.Completed = false;
                    tffRecord.CreateDT = DateTime.Now;

                    dao.Save<TFRecord>(tffRecord);

                    OnOrderSentBase(tffRecord);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        protected override string GetConfig_ThreePLKey(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return config[ShipperConfigEnum.TF_ThreePLKey].Value;
        }
        protected override string GetConfig_Login(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return config[ShipperConfigEnum.TF_Login].Value;
        }
        protected override string GetConfig_Password(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return config[ShipperConfigEnum.TF_Password].Value;
        }
        protected override int GetConfig_FacilityID(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return Convert.ToInt32(config[ShipperConfigEnum.TF_FacilityID].Value);
        }
        protected override int GetConfig_ThreePLID(IDictionary<ShipperConfig.ID, ShipperConfig> config)
        {
            return Convert.ToInt32(config[ShipperConfigEnum.TF_ThreePLID].Value);
        }

        public override void OnOrderShipped(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnOrderShippedBase<TFRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnTrackingNumberUpdated(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnTrackingNumberUpdatedBase<TFRecord>(regID, trackingNumber, shippedDate);
        }

        protected override ABFApi.OrderLineItem GetOrderLineItem(SaleShipperView sale, InventoryView inv)
        {
            return new ABFApi.OrderLineItem()
            {
                //SKU = FixSKU(inv.SKU).Replace("ABF", "TF"),
                SKU = inv.SKU,
                Qty = Convert.ToDecimal(sale.Quantity * inv.Quantity),
                Packed = 0M //Convert.ToDecimal(sale.Quantity * inv.Quantity)
            };
        }

        private string FixSKU(string sku)
        {
            //if (sku == null)
            //    return null;

            ////CoAction SKU fix, COL... -> TF-COL...
            //if (sku.StartsWith("COL201T") ||
            //    sku.StartsWith("COL214T")) 
            //    return "TF-" + sku;
            return sku;
        }
    }
}
