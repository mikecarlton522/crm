using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Business.Gateways;
using System.Xml.Serialization;
using TrimFuel.Business.Utils;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using TrimFuel.Business.Gateways.NPFGateway;


namespace TrimFuel.Business
{
    public class NPFService : ShipperService
    {
        private const string ATTACHMENT_FILE_NAME = "orders.csv";
        //private const string SUBJECT_TMP = "Daily Orders For Processing: {0}";
        //private const string BODY = "Attached are today's orders for fulfillment.  Please contact me if there any problems at rob@trianglemediacorp.com or +31 (0)646610050.  Thank you!";

        //private const string SMTP = "smtp.gmail.com";
        //private int SMTP_PORT = 587;

        //private const string FROM = "dtcoder@gmail.com";
        //private const string FROM_PASSWORD = "eugene123";
        //private const string FROM = "rob@trianglemediacorp.com";

        public override int ShipperID
        {
            get { return TrimFuel.Model.Enums.ShipperEnum.NPF; }
        }

        public override string TableName
        {
            get { return "NPFRecord"; }
        }

        public override bool SendShippingOrderInternal(Registration r, Billing b, IList<SaleShipperView> saleList)
        {
            try
            {
                long? regID = saleList[0].SaleID;

                StringBuilder request = new StringBuilder();

                int orderNumber = 1;

                List<InventoryView> inventoryList = new List<InventoryView>();
                foreach (SaleShipperView sale in saleList)
                {
                    //Do not allow dupes
                    MySqlCommand q = new MySqlCommand("select * from NPFRecordToSend " +
                        "where SaleID = @saleID");
                    q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = sale.SaleID;
                    if (dao.Load<NPFRecordToSend>(q).Count > 0)
                    {
                        throw new Exception(string.Format("Sale({0}) already exists in Pending NPF Orders", sale.SaleID));
                    }

                    IList<InventoryView> invList = inventoryService.GetInventoryListForShipping(sale.ProductCode, ShipperID);
                    inventoryList.AddRange(invList);
                    //remove validation, sales can contain Inventories with InventoryType = Service
                    //if (invList.Count == 0)
                    //{
                    //    throw new Exception(string.Format("Can't find Inventories for ProductCode({0}), Sale({1})", sale.ProductCode, sale.SaleID));
                    //}
                    foreach (InventoryView inv in invList)
                    {
                        inv.Quantity = sale.Quantity * inv.Quantity;
                        request.AppendLine(PrepareRequestLine(r, b, inv, regID.Value, orderNumber, b.CreateDT.Value));
                        orderNumber++;
                    }
                }

                if (inventoryList.Count == 0)
                {
                    throw new Exception(string.Format("Can't find Inventories for ProductCodes({0}), Sales({1})", string.Join(",", saleList.Select(i => i.ProductCode).ToArray()), string.Join(",", saleList.Select(i => i.SaleID.ToString()).ToArray())));
                }

                OnOrderSent(saleList, request.ToString(), regID);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            //Sales only set to Pending Sent, so return false
            return false;
        }

        private string PrepareRequestField(string field)
        {
            if (field == null)
            {
                return null;
            }

            return field.Replace("\"", "\"\"");
        }

        private string PrepareRequestLine(Registration r, Billing b, InventoryView inv, long regId, int orderNumber, DateTime saleDate)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(NPFService), "Gateways.NPFGateway.LineTemplate.tpl");

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
            res = res.Replace("##DESC##", inv.Product);
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

        public override void CheckShipmentsState()
        {
            throw new NotImplementedException();
        }

        public override void CheckReturns()
        {
            throw new NotImplementedException();
        }

        public override bool IsPendingStateImplementation
        {
            get { return true; }
        }

        public override void SendPendingOrders()
        {
            try
            {
                dao.BeginTransaction();

                var cfg = GetAndCheckConfig();

                MySqlCommand q = new MySqlCommand("select null as NPFRecordToSendID, null as SaleID, RegID, Request, min(CreateDT) as CreateDT from NPFRecordToSend " +
                    "group by RegID");

                StringBuilder attach = new StringBuilder();
                attach.AppendLine(Utility.LoadFromEmbeddedResource(typeof(NPFService), "Gateways.NPFGateway.HeaderTemplate.tpl"));
                IList<NPFRecordToSend> pendingList = dao.Load<NPFRecordToSend>(q);
                foreach (var item in pendingList)
                {
                    attach = attach.Append(item.Request);
                }

                if (pendingList.Count > 0)
                {
                    NPFGateway gtw = new NPFGateway(cfg);
                    if (gtw.PostShipments(attach.ToString()))
                    {

                        q = new MySqlCommand("select * from NPFRecordToSend group by SaleID");
                        IList<NPFRecordToSend> pendingSaleList = dao.Load<NPFRecordToSend>(q);
                        foreach (var item in pendingSaleList)
                        {
                            NPFRecord rec = new NPFRecord();
                            rec.Completed = false;
                            rec.CreateDT = DateTime.Now;
                            rec.RegID = item.RegID;
                            rec.Request = item.Request;
                            rec.Response = "Sent.";
                            rec.SaleID = item.SaleID;
                            rec.ShippedDT = null;
                            rec.StatusResponse = null;
                            dao.Save<NPFRecord>(rec);

                            OnOrderSentBase<NPFRecord>(rec);
                        }

                        q = new MySqlCommand("select s.* from NPFRecordToSend kr " +
                            "inner join Sale s on s.SaleID = kr.SaleID " +
                            "group by s.SaleID");
                        IList<SaleShipperView> pendingSaleViewList = dao.Load<Sale>(q).Select(i => new SaleShipperView()
                        {
                            SaleID = i.SaleID,
                            SaleTypeID = i.SaleTypeID
                            //Other fields are not needed
                        }).ToList();
                        OnSalesSent(pendingSaleViewList);

                        dao.ExecuteNonQuery(new MySqlCommand("delete from NPFRecordToSend"));
                    }
                }
                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        private void OnOrderSent(IList<SaleShipperView> saleList, string apiRequest, long? regID)
        {
            try
            {
                dao.BeginTransaction();

                foreach (SaleShipperView sale in saleList)
                {
                    NPFRecordToSend record = new NPFRecordToSend();

                    record.SaleID = sale.SaleID;
                    record.Request = apiRequest;
                    record.RegID = regID;
                    record.CreateDT = DateTime.Now;

                    dao.Save<NPFRecordToSend>(record);

                    //OnOrderSentBase(record);
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        public override void OnOrderShipped(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnOrderShippedBase<NPFRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnTrackingNumberUpdated(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnTrackingNumberUpdatedBase<NPFRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnOrderReturned(long regID, string reason, DateTime? returnDate)
        {
            throw new NotImplementedException();
        }
    }
}
