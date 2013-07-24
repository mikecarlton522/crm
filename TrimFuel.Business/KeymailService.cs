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
using TrimFuel.Model.Enums;


namespace TrimFuel.Business
{
    public class KeymailService : ShipperService
    {
        private const string ATTACHMENT_FILE_NAME = "orders.csv";
        private const string SUBJECT_TMP = "Daily Orders For Processing: {0}";
        private const string BODY = "Attached are today's orders for fulfillment.  Please contact me if there any problems at rob@trianglemediacorp.com or +31 (0)646610050.  Thank you!";

        private const string SMTP = "smtp.gmail.com";
        private int SMTP_PORT = 587;

        private const string FROM = "dtcoder@gmail.com";
        private const string FROM_PASSWORD = "eugene123";
        //private const string FROM = "rob@trianglemediacorp.com";

        public override int ShipperID
        {
            get { return TrimFuel.Model.Enums.ShipperEnum.Keymail; }
        }

        public override string TableName
        {
            get { return "KeymailRecord"; }
        }

        public override bool SendShippingOrderInternal(Registration r, Billing b, IList<SaleShipperView> saleList)
        {
            try
            {

                long? regID = saleList[0].SaleID;

                string requestSkuList = string.Empty;
                string skuTemplate = Utility.LoadFromEmbeddedResource(typeof(KeymailService), "Gateways.Keymail.SKUTemplate.tpl");

                List<InventoryView> inventoryList = new List<InventoryView>();
                foreach (SaleShipperView sale in saleList)
                {
                    //Do not allow dupes
                    MySqlCommand q = new MySqlCommand("select * from KeymailRecordToSend " +
                        "where SaleID = @saleID");
                    q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = sale.SaleID;
                    if (dao.Load<KeymailRecordToSend>(q).Count > 0)
                    {
                        throw new Exception(string.Format("Sale({0}) already exists in Pending Keymail Orders", sale.SaleID));
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
                        int saleQuantity = (sale.Quantity != null) ? sale.Quantity.Value : 1;
                        int invQuantity = (inv.Quantity != null) ? inv.Quantity.Value : 1;
                        requestSkuList += skuTemplate
                            .Replace("##SKU##", inv.SKU)
                            .Replace("##QUANTITY##", (saleQuantity * invQuantity).ToString());
                    }
                }

                if (inventoryList.Count == 0)
                {
                    throw new Exception(string.Format("Can't find Inventories for ProductCodes({0}), Sales({1})", string.Join(",", saleList.Select(i => i.ProductCode).ToArray()), string.Join(",", saleList.Select(i => i.SaleID.ToString()).ToArray())));
                }

                string request = PrepareRequestLine(r, b, requestSkuList, regID.Value);

                OnOrderSent(saleList, request, regID);
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

            return field.Replace("\"", "'");
        }

        private string PrepareRequestLine(Registration r, Billing b, string requestSkuList, long regId)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(KeymailService), "Gateways.Keymail.LineTemplate.tpl");

            res = res.Replace("##FirstName##", r.FirstName);
            res = res.Replace("##LastName##", r.LastName);
            res = res.Replace("##Address1##", r.Address1);
            res = res.Replace("##Address2##", r.Address2);
            res = res.Replace("##Address3##", string.Empty);
            res = res.Replace("##Town##", r.City);
            res = res.Replace("##State##", r.State);
            res = res.Replace("##PostCode##", r.Zip);
            res = res.Replace("##Country##", b.Country);
            res = res.Replace("##OrderID##", regId.ToString());

            res = res.Replace("##SKU_LIST##", requestSkuList);

            return res;
        }

        public override void CheckShipmentsState()
        {
            throw new NotImplementedException();
        }

        public override void CheckReturns()
        {
            throw new NotImplementedException();
        }

        private string KeymailSKUFix(string request)
        {
            //return request.Replace("ABF-", "UK-");
            return request;
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

                MySqlCommand q = new MySqlCommand("select null as KeymailRecordToSendID, null as SaleID, RegID, Request, min(CreateDT) as CreateDT from KeymailRecordToSend " +
                    "group by RegID");

                StringBuilder attach = new StringBuilder();
                IList<KeymailRecordToSend> pendingList = dao.Load<KeymailRecordToSend>(q);
                foreach (var item in pendingList)
                {
                    attach = attach.AppendLine(KeymailSKUFix(item.Request));
                }

                SendEmail(attach.ToString());

                q = new MySqlCommand("select * from KeymailRecordToSend group by SaleID");
                IList<KeymailRecordToSend> pendingSaleList = dao.Load<KeymailRecordToSend>(q);
                foreach (var item in pendingSaleList)
                {
                    KeymailRecord rec = new KeymailRecord();
                    rec.Completed = false;
                    rec.CreateDT = DateTime.Now;
                    rec.RegID = item.RegID;
                    rec.Request = KeymailSKUFix(item.Request);
                    rec.Response = "Sent.";
                    rec.SaleID = item.SaleID;
                    rec.ShippedDT = null;
                    rec.StatusResponse = null;
                    dao.Save<KeymailRecord>(rec);

                    OnOrderSentBase<KeymailRecord>(rec);
                }

                q = new MySqlCommand("select s.* from KeymailRecordToSend kr " +
                    "inner join Sale s on s.SaleID = kr.SaleID " +
                    "group by s.SaleID");
                IList<SaleShipperView> pendingSaleViewList = dao.Load<Sale>(q).Select(i => new SaleShipperView()
                {
                    SaleID = i.SaleID,
                    SaleTypeID = i.SaleTypeID
                    //Other fields are not needed
                }).ToList();
                OnSalesSent(pendingSaleViewList);

                dao.ExecuteNonQuery(new MySqlCommand("delete from KeymailRecordToSend"));

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
        }

        private void SendEmail(string attachContent)
        {
            var cfg = GetAndCheckConfig();
            var emailList = cfg[ShipperConfigEnum.KEYMAIL_Emails].Value.Split(',');

            using (Stream s = Utility.OpenStringAsStreamUTF8(attachContent))
            {
                Attachment data = new Attachment(s, "text/csv");
                data.ContentDisposition.CreationDate = DateTime.Now;
                data.ContentDisposition.ModificationDate = DateTime.Now;
                data.ContentDisposition.FileName = ATTACHMENT_FILE_NAME;

                MailMessage message = new MailMessage();
                message.From = new MailAddress(FROM);
                message.Subject = string.Format(SUBJECT_TMP, DateTime.Today.ToShortDateString());
                message.Body = BODY;
                foreach (string to in emailList)
                {
                    message.To.Add(new MailAddress(to));
                }
                message.Attachments.Add(data);

                NetworkCredential credentials = new NetworkCredential(FROM, FROM_PASSWORD);

                SmtpClient client = new SmtpClient(SMTP, SMTP_PORT);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = credentials;

                client.Send(message);
            }
        }

        private void OnOrderSent(IList<SaleShipperView> saleList, string apiRequest, long? regID)
        {
            try
            {
                dao.BeginTransaction();

                foreach (SaleShipperView sale in saleList)
                {
                    KeymailRecordToSend record = new KeymailRecordToSend();

                    record.SaleID = sale.SaleID;
                    record.Request = apiRequest;
                    record.RegID = regID;
                    record.CreateDT = DateTime.Now;

                    dao.Save<KeymailRecordToSend>(record);

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
            OnOrderShippedBase<KeymailRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnTrackingNumberUpdated(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnTrackingNumberUpdatedBase<KeymailRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnOrderReturned(long regID, string reason, DateTime? returnDate)
        {
            throw new NotImplementedException();
        }
    }
}
