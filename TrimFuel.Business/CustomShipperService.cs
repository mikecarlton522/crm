using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Views;
using MySql.Data.MySqlClient;
using TrimFuel.Business.Gateways;
using System.Xml.Serialization;
using TrimFuel.Business.Utils;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using TrimFuel.Business.Gateways.CustomShipperGateways;


namespace TrimFuel.Business
{
    public class CustomShipperService : ShipperService
    {
        public override int ShipperID
        {
            get { return TrimFuel.Model.Enums.ShipperEnum.CustomShipper; }
        }

        public override string TableName
        {
            get { return "CustomShipperRecord"; }
        }

        public override bool SendShippingOrderInternal(Registration r, Billing b, IList<SaleShipperView> saleList)
        {
            try
            {
                long? regID = saleList[0].SaleID;

                StringBuilder request = new StringBuilder();

                List<InventoryView> inventoryList = new List<InventoryView>();
                foreach (SaleShipperView sale in saleList)
                {
                    //Do not allow dupes
                    MySqlCommand q = new MySqlCommand("select * from CustomShipperRecordToSend " +
                        "where SaleID = @saleID");
                    q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = sale.SaleID;
                    if (dao.Load<CustomShipperRecordToSend>(q).Count > 0)
                    {
                        throw new Exception(string.Format("Sale({0}) already exists in Pending CustomShipper Orders", sale.SaleID));
                    }

                    IList<InventoryView> invList = inventoryService.GetInventoryListForShipping(sale.ProductCode, ShipperID);
                    inventoryList.AddRange(invList);
                    //remove validation, sales can contain Inventories with InventoryType = Service
                    //if (invList.Count == 0)
                    //{
                    //    throw new Exception(string.Format("Can't find Inventories for ProductCode({0}), Sale({1})", sale.ProductCode, sale.SaleID));
                    //}

                    request.AppendLine(PrepareRequestLine(r, b, invList, regID.Value, b.CreateDT.Value, sale.Quantity.Value));
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

        private string PrepareRequestLine(Registration r, Billing b, IList<InventoryView> inventoryList, long regId, DateTime saleDate, int quantity)
        {
            string res = Utility.LoadFromEmbeddedResource(typeof(CustomShipperService), "Gateways.CustomShipperGateways.LineTemplate.tpl");

            res = res.Replace("##SALE_ID##", regId.ToString());
            res = res.Replace("##DATE##", saleDate.ToShortDateString());
            res = res.Replace("##FirstName##", PrepareRequestField(b.FirstName));
            res = res.Replace("##LastName##", PrepareRequestField(b.LastName));
            res = res.Replace("##Address1##", PrepareRequestField(b.Address1));
            res = res.Replace("##Address2##", PrepareRequestField(b.Address2));
            res = res.Replace("##Town##", PrepareRequestField(b.City));
            res = res.Replace("##State##", PrepareRequestField(b.State));
            res = res.Replace("##PostCode##", PrepareRequestField(b.Zip));
            res = res.Replace("##Country##", PrepareRequestField(b.Country));
            res = res.Replace("##PHONE##", PrepareRequestField(b.Phone));
            res = res.Replace("##EMAIL##", PrepareRequestField(b.Email));

            string[] skuList = new string[inventoryList.Count];
            for (int i = 0; i < inventoryList.Count; i++)
            {
                skuList[i] = string.Format("{0}({1})", inventoryList[i].SKU, inventoryList[i].Quantity * quantity);
            }
            res = res.Replace("##SKULIST##", string.Join(",", skuList));

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

        public override bool IsPendingStateImplementation
        {
            get { return true; }
        }

        public override void SendPendingOrders()
        {
            switch (Config.Current.APPLICATION_ID)
            {
                
                case ApplicationEnum.CoActionCRM:
                    ProcessCoActionCanadaFulfillment();
                    break;

                case "localhost":
                case ApplicationEnum.OrangeStarCRM:
                    ProcessOrangeStarFulfillment();
                    break;
            }
        }

        private void OnOrderSent(IList<SaleShipperView> saleList, string apiRequest, long? regID)
        {
            try
            {
                dao.BeginTransaction();

                foreach (SaleShipperView sale in saleList)
                {
                    CustomShipperRecordToSend record = new CustomShipperRecordToSend();

                    record.SaleID = sale.SaleID;
                    record.Request = apiRequest;
                    record.RegID = regID;
                    record.CreateDT = DateTime.Now;

                    dao.Save<CustomShipperRecordToSend>(record);
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
            OnOrderShippedBase<CustomShipperRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnTrackingNumberUpdated(long regID, string trackingNumber, DateTime shippedDate)
        {
            OnTrackingNumberUpdatedBase<CustomShipperRecord>(regID, trackingNumber, shippedDate);
        }

        public override void OnOrderReturned(long regID, string reason, DateTime? returnDate)
        {
            throw new NotImplementedException();
        }

        #region CoActionCanadaFulfillment

        private void ProcessCoActionCanadaFulfillment()
        {
            try
            {
                dao.BeginTransaction();

                var cfg = GetAndCheckConfig();

                MySqlCommand q = new MySqlCommand("select null as CustomShipperRecordToSendID, null as SaleID, RegID, Request, min(CreateDT) as CreateDT from CustomShipperRecordToSend " +
                    "group by RegID");

                StringBuilder attach = new StringBuilder();
                attach.AppendLine(Utility.LoadFromEmbeddedResource(typeof(CustomShipperService), "Gateways.CustomShipperGateways.HeaderTemplate.tpl"));
                IList<CustomShipperRecordToSend> pendingList = dao.Load<CustomShipperRecordToSend>(q);
                foreach (var item in pendingList)
                {
                    attach = attach.Append(item.Request);
                }

                if (pendingList.Count > 0)
                {
                    CoActionCanadaFulfillmentGateway gtw = new CoActionCanadaFulfillmentGateway(cfg);

                    if (gtw.PostShipments(attach.ToString()))
                    {
                        q = new MySqlCommand("select * from CustomShipperRecordToSend group by SaleID");
                        IList<CustomShipperRecordToSend> pendingSaleList = dao.Load<CustomShipperRecordToSend>(q);
                        foreach (var item in pendingSaleList)
                        {
                            CustomShipperRecord rec = new CustomShipperRecord();
                            rec.Completed = false;
                            rec.CreateDT = DateTime.Now;
                            rec.RegID = item.RegID;
                            rec.Request = item.Request;
                            rec.Response = "Sent.";
                            rec.SaleID = item.SaleID;
                            rec.ShippedDT = null;
                            rec.StatusResponse = null;
                            dao.Save<CustomShipperRecord>(rec);

                            OnOrderSentBase<CustomShipperRecord>(rec);
                        }

                        q = new MySqlCommand("select s.* from CustomShipperRecordToSend kr " +
                            "inner join Sale s on s.SaleID = kr.SaleID " +
                            "group by s.SaleID");
                        IList<SaleShipperView> pendingSaleViewList = dao.Load<Sale>(q).Select(i => new SaleShipperView()
                        {
                            SaleID = i.SaleID,
                            SaleTypeID = i.SaleTypeID
                            //Other fields are not needed
                        }).ToList();
                        OnSalesSent(pendingSaleViewList);

                        dao.ExecuteNonQuery(new MySqlCommand("delete from CustomShipperRecordToSend"));
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

        #endregion

        #region OrangeStarFulfillment

        private void ProcessOrangeStarFulfillment()
        {
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("select null as CustomShipperRecordToSendID, SaleID, RegID, Request, min(CreateDT) as CreateDT from CustomShipperRecordToSend " +
                    "group by RegID");

                StringBuilder attach = new StringBuilder();
                attach.AppendLine(Utility.LoadFromEmbeddedResource(typeof(CustomShipperService), "Gateways.CustomShipperGateways.HeaderTemplate.tpl"));
                IList<CustomShipperRecordToSend> pendingList = dao.Load<CustomShipperRecordToSend>(q);

                foreach (var item in pendingList)
                {
                    var customShipperToSend = GetCustomShipperToSend(item.SaleID);
                    var request = item.Request.Trim();
                    if (customShipperToSend != null)
                    {
                        attach = attach.Append(request + 
                                                ",RV " +
                                                customShipperToSend.BillAmount.ToString() + "," +
                                                customShipperToSend.PartNumber + "," +
                                                customShipperToSend.Quantity.ToString() + "," +
                                                customShipperToSend.UnitWeight.ToString() + "," +
                                                customShipperToSend.Description + "," +
                                                "\n\r");   
                    }
                    else
                        attach = attach.Append(request + ",\"RV\"" + "\n\r");  
                }

                if (pendingList.Count > 0)
                {
                    if (SendOrangeStarFulfillmentEmail(attach.ToString()))
                    {
                        q = new MySqlCommand("select * from CustomShipperRecordToSend group by SaleID");
                        IList<CustomShipperRecordToSend> pendingSaleList = dao.Load<CustomShipperRecordToSend>(q);
                        foreach (var item in pendingSaleList)
                        {
                            CustomShipperRecord rec = new CustomShipperRecord();
                            rec.Completed = false;
                            rec.CreateDT = DateTime.Now;
                            rec.RegID = item.RegID;
                            rec.Request = item.Request;
                            rec.Response = "Sent.";
                            rec.SaleID = item.SaleID;
                            rec.ShippedDT = null;
                            rec.StatusResponse = null;
                            dao.Save<CustomShipperRecord>(rec);

                            OnOrderSentBase<CustomShipperRecord>(rec);
                        }

                        q = new MySqlCommand("select s.* from CustomShipperRecordToSend kr " +
                            "inner join Sale s on s.SaleID = kr.SaleID " +
                            "group by s.SaleID");
                        IList<SaleShipperView> pendingSaleViewList = dao.Load<Sale>(q).Select(i => new SaleShipperView()
                        {
                            SaleID = i.SaleID,
                            SaleTypeID = i.SaleTypeID
                            //Other fields are not needed
                        }).ToList();
                        OnSalesSent(pendingSaleViewList);

                        dao.ExecuteNonQuery(new MySqlCommand("delete from CustomShipperRecordToSend"));
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

        public CustomShipperToSendView GetCustomShipperToSend(long? saleId)
        {
            MySqlCommand q = new MySqlCommand();

            q = new MySqlCommand("select coalesce(chs.Amount, ch.Amount) as BillAmount, t.ProductCode as PartNumber, t.Quantity, pcinfo.Description, pcinfo.Weight as UnitWeight " + 
                                "from " + 
                                "(   select us.SaleID, s.ProductCode, s.Quantity, us.ChargeHistoryID from Upsell s " + 
                                "    inner join UpsellSale us on us.UpsellID = s.UpsellID " + 
                                "    union " + 
                                "    select s.SaleID, s.ProductCode, s.Quantity, s.ChargeHistoryID as ID from BillingSale s " + 
                                "    union " +
                                "    select ss.SaleID, s.ProductCode, s.Quantity, null as ChargeHistoryID from ExtraTrialShip s " +
                                "    inner join ExtraTrialShipSale ss on ss.ExtraTrialShipID = s.ExtraTrialShipID " + 
                                ") t " + 
                                "left join ChargeHistoryEx ch on ch.ChargeHistoryID = t.ChargeHistoryID " +
                                "left join ChargeHistoryExSale chs on chs.SaleID = t.SaleID " +
                                "left join ProductCode pc on pc.ProductCode = t.ProductCode " +
                                "left join ProductCodeInfo pcinfo on pcinfo.ProductCodeID = pc.ProductCodeID " +
                                "where t.SaleID = @saleID");

            q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleId;
            return dao.Load<CustomShipperToSendView>(q).FirstOrDefault();
        }
       
        private const string ROB_EMAIL = "rob@trianglemediacorp.com";
        private const string ATTACHMENT_FILE_NAME = "orders.csv";
        private const string SUBJECT_TMP = "OrangeStar Daily Orders For Processing: {0}";
        private const string BODY = "Attached are today's orders for fulfillment.  Please contact me if there any problems at rob@trianglemediacorp.com or +31 (0)646610050.  Thank you!";

        private const string SMTP = "smtp.gmail.com";
        private int SMTP_PORT = 587;

        private const string FROM = "dtcoder@gmail.com";
        private const string FROM_PASSWORD = "eugene123";

        private bool SendOrangeStarFulfillmentEmail(string attachContent)
        {
            bool res = false;

            try
            {
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

                    message.To.Add(new MailAddress(ROB_EMAIL));

                    message.Attachments.Add(data);

                    NetworkCredential credentials = new NetworkCredential(FROM, FROM_PASSWORD);

                    SmtpClient client = new SmtpClient(SMTP, SMTP_PORT);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = credentials;

                    client.Send(message);

                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
