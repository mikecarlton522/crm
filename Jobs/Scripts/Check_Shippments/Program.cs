using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using log4net;
using TrimFuel.Model;
using TrimFuel.Business.Dao;
using TrimFuel.Business;
using System.Configuration;
using TrimFuel.Model.Views;
using System.Net.Mail;

namespace Check_Shippments
{
    class Program
    {
        static string _stoConnectionString = ConfigurationManager.ConnectionStrings["TrimFuel"].ConnectionString;
        static string[] emails = ConfigurationManager.AppSettings["emails"].Split(',');
        static ILog logger = null;

        static DateTime STO_ENDDATE = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 5, 0, 0).AddDays(0);
        static DateTime STO_STARTDATE = STO_ENDDATE.AddDays(-4);
        static DateTime CLIENT_ENDDATE = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 6, 0, 0).AddDays(0);
        static DateTime CLIENT_STARTDATE = CLIENT_ENDDATE.AddDays(-4);

        static string emailBody = string.Empty;
        static string SHIPPMENT_EMAIL_TEMPLATE = "<br/><b>{0} ({1})</b><br/>shippments to be sent: {2}<br/>shipments sent: {3}<br/>shipments queues: {4}<br/>";
        static string REBILL_EMAIL_TEMPLATE = "<br/><b>{0}</b><br/>number of queued rebils: {1}<br/>";

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            log4net.ThreadContext.Properties["ApplicationID"] = "All clients";
            logger = LogManager.GetLogger(typeof(Program));
            

            ProcessShippmentsSTO();
            ProcessShippmentsClients();
            if (!string.IsNullOrEmpty(emailBody))
            {
                SendEmails(emailBody, "Shipments error");
                emailBody = string.Empty;
            }

            ProcessRebills();
            if (!string.IsNullOrEmpty(emailBody))
            {
                SendEmails(emailBody, "Rebills error");
            }
        }

        #region Shippments

        static void ProcessShippmentsSTO()
        {
            //STO
            IDao dao = new MySqlDao(_stoConnectionString);
            ProcessShippments(dao, STO_STARTDATE, STO_ENDDATE, "Triangle (STO)");
        }

        static void ProcessShippmentsClients()
        {
            IDao dao = new MySqlDao(_stoConnectionString);
            MySqlCommand q = new MySqlCommand("SELECT * FROM TPClient WHERE ProcessShipments=1");
            var clients = dao.Load<TPClient>(q);
            foreach (var client in clients)
            {
                //create Client DAO
                dao = new MySqlDao(client.ConnectionString);
                ProcessShippments(dao, CLIENT_STARTDATE, CLIENT_ENDDATE, client.Name);
            }
        }

        static void ProcessShippments(IDao dao, DateTime startDate, DateTime endDate, string clientName)
        {
            var shippments = GetNotSendedGroupByShipper(dao, startDate, endDate);
            foreach (var shippment in shippments)
            {
                try
                {
                    var shipperService = ShipperService.GetShipperServiceByShipperID(shippment.Key.Value);
                    string tableName = string.Empty;
                    if (shipperService != null)
                        tableName = shipperService.TableName;
                    else
                    {
                        if (shippment.Key == 2)
                            tableName = "TSNRecord";
                    }

                    int sendedCount = GetSendedCountByTableName(dao, startDate, endDate, tableName);
                    int notSendedCount = shippment.Value ?? 0;

                    if ((float)notSendedCount * 100 / sendedCount > 1)
                    {
                        string error = string.Format("{3} {2}: Not sended shippments - {0}, Sent - {1}", notSendedCount, sendedCount, tableName, clientName);
                        emailBody += string.Format(SHIPPMENT_EMAIL_TEMPLATE, clientName, tableName, notSendedCount + sendedCount, sendedCount, notSendedCount);
                        throw new Exception(error);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(typeof(Program), ex);
                }
            }
        }

        static Dictionary<int?, int?> GetNotSendedGroupByShipper(IDao dao, DateTime startDate, DateTime endDate)
        {
            Dictionary<int?, int?> result = new Dictionary<int?, int?>();
            List<ShippmentsToSendView> shippments = null;
            try
            {
                shippments = GetNotSendedSalesCount(1, startDate, endDate, dao);
                ModifyShippmentDictionary(shippments, result);

                shippments = GetNotSendedSalesCount(2, startDate, endDate, dao);
                ModifyShippmentDictionary(shippments, result);

                shippments = GetNotSendedSalesCount(3, startDate, endDate, dao);
                ModifyShippmentDictionary(shippments, result);

                shippments = GetNotSendedSalesCount(5, startDate, endDate, dao);
                ModifyShippmentDictionary(shippments, result);
            }
            catch (Exception ex)
            {
                logger.Error(typeof(Program), ex);
            }
            return result;
        }

        static List<ShippmentsToSendView> GetNotSendedSalesCount(int nSaleTypeID, DateTime startDate, DateTime endDate, IDao dao)
        {
            string query = string.Empty;
            switch (nSaleTypeID)
            {
                case 1:
                    {
                        query = "select Count(*) Value, sp.ShipperID" +
                            " from  BillingSale bsale" +
                            " inner join Sale on Sale.SaleID = bsale.SaleID" +
                            " inner join BillingSubscription bs on bs.BillingSubscriptionID = bsale.BillingSubscriptionID" +
                            " inner join Subscription S on S.SubscriptionID = bs.SubscriptionID" +
                            " inner join Billing b on b.BillingID = bs.BillingID" +
                            " inner join Registration r on r.RegistrationID = b.RegistrationID" +
                            " inner join ShipperProduct sp on sp.ProductID = S.ProductID" +
                            " left join TSNRecord tsn on tsn.SaleID = Sale.SaleID and tsn.RegID is not null" +
                            " left join ABFRecord abf on abf.SaleID = Sale.SaleID and abf.RegID is not null" +
                            " left join MBRecord mb on mb.SaleID = Sale.SaleID and mb.RegID is not null" +
                            " left join TFRecord tf on tf.SaleID = Sale.SaleID and tf.RegID is not null" +
                            " left join ALFRecord alf on alf.SaleID = Sale.SaleID and alf.RegID is not null" +
                            " left join GFRecord gf on gf.SaleID = Sale.SaleID and gf.RegID is not null" +
                            " left join NPFRecord npf on npf.SaleID = Sale.SaleID and npf.RegID is not null" +
                            " left join NPFRecordToSend npfP on npfP.SaleID = Sale.SaleID and npfP.RegID is not null" +
                            " left join KeymailRecord km on km.SaleID = Sale.SaleID and km.RegID is not null" +
                            " left join KeymailRecordToSend kmP on kmP.SaleID = Sale.SaleID and kmP.RegID is not null" +
                            " left join ShippingBlocked block on block.BillingID = b.BillingID" +
                            " where Sale.SaleTypeID = 1 and (Sale.CreateDT between @StartDate and @EndDate)" +
                            " and coalesce(Sale.NotShip,0) <> 1" +
                            " and block.ShippingBlockedID is null" +
                            " and tsn.TSNRecordID is null" +
                            " and abf.ABFRecordID is null" +
                            " and mb.MBRecordID is null" +
                            " and tf.TFRecordID is null" +
                            " and alf.ALFRecordID is null" +
                            " and gf.GFRecordID is null" +
                            " and npf.NPFRecordID is null" +
                            " and npfP.NPFRecordToSendID is null" +
                            " and km.KeymailRecordID is null" +
                            " and kmP.KeymailRecordToSendID is null" +
                            " and not exists (select * from SaleRefund inner join ChargeHistoryEx chrefund on chrefund.ChargeHistoryID = SaleRefund.ChargeHistoryID where SaleRefund.SaleID = Sale.SaleID and chrefund.Success = 1)" +
                            " group by sp.ShipperID";
                        break;
                    }
                case 2:
                    {
                        query = "Select Count(*) Value, sp.ShipperID" +
                            " From  BillingSale bsale" +
                            " inner join Sale on Sale.SaleID = bsale.SaleID" +
                            " inner join BillingSubscription BS on BS.BillingSubscriptionID = bsale.BillingSubscriptionID" +
                            " inner join ChargeHistoryEx CH on CH.ChargeHistoryID = bsale.ChargeHistoryID" +
                            " inner join Billing B on B.BillingID = BS.BillingID" +
                            " inner join Subscription S on S.SubscriptionID = BS.SubscriptionID" +
                            " inner join Registration R on R.RegistrationID = B.RegistrationID" +
                            " inner join ShipperProduct sp on sp.ProductID = S.ProductID" +
                            " left join TSNRecord tsn on tsn.SaleID = Sale.SaleID and tsn.RegID is not null" +
                            " left join ABFRecord abf on abf.SaleID = Sale.SaleID and abf.RegID is not null" +
                            " left join MBRecord mb on mb.SaleID = Sale.SaleID and mb.RegID is not null" +
                            " left join TFRecord tf on tf.SaleID = Sale.SaleID and tf.RegID is not null" +
                            " left join ALFRecord alf on alf.SaleID = Sale.SaleID and alf.RegID is not null" +
                            " left join GFRecord gf on gf.SaleID = Sale.SaleID and gf.RegID is not null" +
                            " left join NPFRecord npf on npf.SaleID = Sale.SaleID and npf.RegID is not null" +
                            " left join NPFRecordToSend npfP on npfP.SaleID = Sale.SaleID and npfP.RegID is not null" +
                            " left join KeymailRecord km on km.SaleID = Sale.SaleID and km.RegID is not null" +
                            " left join KeymailRecordToSend kmP on kmP.SaleID = Sale.SaleID and kmP.RegID is not null" +
                            " left join ShippingBlocked block on block.BillingID = B.BillingID" +
                            " where CH.Success = 1 and (Sale.CreateDT between @StartDate and @EndDate)" +
                            " and coalesce(Sale.NotShip,0) <> 1" +
                            " and Sale.SaleTypeID = 2 and (bsale.RebillCycle > 1 or S.ShipFirstRebill = 1)" +
                            " and block.ShippingBlockedID is null" +
                            " and tsn.TSNRecordID is null" +
                            " and abf.ABFRecordID is null" +
                            " and mb.MBRecordID is null" +
                            " and tf.TFRecordID is null" +
                            " and alf.ALFRecordID is null" +
                            " and gf.GFRecordID is null" +
                            " and npf.NPFRecordID is null" +
                            " and npfP.NPFRecordToSendID is null" +
                            " and km.KeymailRecordID is null" +
                            " and kmP.KeymailRecordToSendID is null" +
                            " and not exists (select * from SaleRefund inner join ChargeHistoryEx chrefund on chrefund.ChargeHistoryID = SaleRefund.ChargeHistoryID where SaleRefund.SaleID = Sale.SaleID and chrefund.Success = 1)" +
                            " group by sp.ShipperID";
                        break;
                    }
                case 3:
                    {
                        query = "Select Count(*) Value, sp.ShipperID" +
                            " From UpsellSale usale inner join Sale on Sale.SaleID = usale.SaleID" +
                            " inner join Upsell U on U.UpsellID = usale.UpsellID" +
                            " inner join Billing B on B.BillingID = U.BillingID" +
                            " inner join BillingSubscription BS on BS.BillingID = B.BillingID" +
                            " inner join Subscription S on S.SubscriptionID = BS.SubscriptionID" +
                            " inner join Registration R on R.RegistrationID = B.RegistrationID" +
                            " inner join ShipperProduct sp on sp.ProductID = S.ProductID" +
                            " left join TSNRecord tsn on tsn.SaleID = Sale.SaleID and tsn.RegID is not null" +
                            " left join ABFRecord abf on abf.SaleID = Sale.SaleID and abf.RegID is not null" +
                            " left join MBRecord mb on mb.SaleID = Sale.SaleID and mb.RegID is not null" +
                            " left join TFRecord tf on tf.SaleID = Sale.SaleID and tf.RegID is not null" +
                            " left join ALFRecord alf on alf.SaleID = Sale.SaleID and alf.RegID is not null" +
                            " left join GFRecord gf on gf.SaleID = Sale.SaleID and gf.RegID is not null" +
                            " left join NPFRecord npf on npf.SaleID = Sale.SaleID and npf.RegID is not null" +
                            " left join NPFRecordToSend npfP on npfP.SaleID = Sale.SaleID and npfP.RegID is not null" +
                            " left join KeymailRecord km on km.SaleID = Sale.SaleID and km.RegID is not null" +
                            " left join KeymailRecordToSend kmP on kmP.SaleID = Sale.SaleID and kmP.RegID is not null" +
                            " where (Sale.CreateDT between @StartDate and @EndDate)" +
                            " and coalesce(Sale.NotShip,0) <> 1" +
                            " and tsn.TSNRecordID is null" +
                            " and abf.ABFRecordID is null" +
                            " and mb.MBRecordID is null" +
                            " and tf.TFRecordID is null" +
                            " and alf.ALFRecordID is null" +
                            " and gf.GFRecordID is null" +
                            " and npf.NPFRecordID is null" +
                            " and npfP.NPFRecordToSendID is null" +
                            " and km.KeymailRecordID is null" +
                            " and kmP.KeymailRecordToSendID is null" +
                            " and Sale.SaleTypeID = 3 and B.billingid not in (Select BillingID From ShippingBlocked)" +
                            " group by sp.ShipperID";
                        break;
                    }
                case 5:
                    {
                        query = "Select Count(*) Value, sp.ShipperID" +
                            " From ExtraTrialShipSale usale inner join Sale on Sale.SaleID = usale.SaleID" +
                            " inner join ExtraTrialShip ETS on ETS.ExtraTrialShipID = usale.ExtraTrialShipID" +
                            " inner join Billing B on B.BillingID = usale.BillingID" +
                            " inner join BillingSubscription BS on BS.BillingID = B.BillingID" +
                            " inner join Subscription S on S.SubscriptionID = BS.SubscriptionID" +
                            " inner join Registration R on R.RegistrationID = B.RegistrationID" +
                            " inner join ShipperProduct sp on sp.ProductID = S.ProductID" +
                            " left join TSNRecord tsn on tsn.SaleID = Sale.SaleID and tsn.RegID is not null" +
                            " left join ABFRecord abf on abf.SaleID = Sale.SaleID and abf.RegID is not null" +
                            " left join MBRecord mb on mb.SaleID = Sale.SaleID and mb.RegID is not null" +
                            " left join TFRecord tf on tf.SaleID = Sale.SaleID and tf.RegID is not null" +
                            " left join ALFRecord alf on alf.SaleID = Sale.SaleID and alf.RegID is not null" +
                            " left join GFRecord gf on gf.SaleID = Sale.SaleID and gf.RegID is not null" +
                            " left join NPFRecord npf on npf.SaleID = Sale.SaleID and npf.RegID is not null" +
                            " left join NPFRecordToSend npfP on npfP.SaleID = Sale.SaleID and npfP.RegID is not null" +
                            " left join KeymailRecord km on km.SaleID = Sale.SaleID and km.RegID is not null" +
                            " left join KeymailRecordToSend kmP on kmP.SaleID = Sale.SaleID and kmP.RegID is not null" +
                            " where (Sale.CreateDT between @StartDate and @EndDate)" +
                            " and BS.StatusTID = 1" +
                            " and coalesce(Sale.NotShip,0) <> 1" +
                            " and tsn.TSNRecordID is null" +
                            " and abf.ABFRecordID is null" +
                            " and mb.MBRecordID is null" +
                            " and tf.TFRecordID is null" +
                            " and alf.ALFRecordID is null" +
                            " and gf.GFRecordID is null" +
                            " and npf.NPFRecordID is null" +
                            " and npfP.NPFRecordToSendID is null" +
                            " and km.KeymailRecordID is null" +
                            " and kmP.KeymailRecordToSendID is null" +
                            " and Sale.SaleTypeID = 5 and B.billingid not in (Select BillingID From ShippingBlocked)" +
                            " group by sp.ShipperID";
                        break;
                    }
            }
            MySqlCommand q = new MySqlCommand(query);
            q.CommandTimeout = 9999;
            q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
            q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
            return dao.Load<ShippmentsToSendView>(q).ToList();
        }

        static void ModifyShippmentDictionary(List<ShippmentsToSendView> shippments, Dictionary<int?, int?> result)
        {
            foreach (var shippment in shippments)
            {
                if (result.ContainsKey(shippment.ShipperID))
                    result[shippment.ShipperID] += shippment.Count;
                else
                    result.Add(shippment.ShipperID, shippment.Count);
            }
        }

        static int GetSendedCountByTableName(IDao dao, DateTime startDate, DateTime endDate, string table)
        {
            int returnValue = 0;
            try
            {
                MySqlCommand q = new MySqlCommand("SELECT Count(*) as Value FROM " + table + @" rec
                                                   inner join Sale s on s.SaleID=rec.SaleID
                                                   WHERE rec.RegID is not null 
                                                   AND s.CreateDT between @StartDate and @EndDate");
                q.CommandTimeout = 9999;
                q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = startDate;
                q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = endDate;
                returnValue += dao.ExecuteScalar<int>(q) ?? 0;
            }
            catch (Exception ex)
            {
                logger.Error(typeof(Program), ex);
            }
            return returnValue;
        }

        #endregion

        #region Rebills

        static void ProcessRebills()
        {
            IDao dao = new MySqlDao(_stoConnectionString);
            int diff = GetDiffCount(dao);
            ProcessClientRebills(diff, "STO");

            dao = new MySqlDao(_stoConnectionString);
            MySqlCommand q = new MySqlCommand("SELECT * FROM TPClient WHERE ProcessRebills=1");
            var clients = dao.Load<TPClient>(q);
            foreach (var client in clients)
            {
                dao = new MySqlDao(client.ConnectionString);
                diff = GetDiffCount(dao);
                ProcessClientRebills(diff, client.Name);
            }
        }

        static void ProcessClientRebills(int diff, string clientName)
        {
            try
            {
                if (diff > 0)
                {
                    string error = string.Format("Rebills for Client {0} not processed. Number of queued rebils: {1}", clientName, diff);
                    emailBody += string.Format(REBILL_EMAIL_TEMPLATE, clientName, diff);
                    throw new Exception(error);
                }
            }
            catch (Exception ex)
            {
                logger.Error(typeof(Program), ex);
            }
        }

        static int GetDiffCount(IDao dao)
        {
            int? count = 0;

            MySqlCommand q = new MySqlCommand(@"
                        select Count(*) as Value FROM
						(
						    SELECT distinct bs.BillingSubscriptionID
	                        from BillingSubscription bs
	                        inner join Billing b on b.BillingID = bs.BillingID
	                        inner join Subscription s on s.SubscriptionID = bs.SubscriptionID
	                        where s.ProductID > 0 and s.Recurring = 1
	                        and (bs.NextBillDate between @StartDate and @EndDate)
	                        and bs.StatusTID=1
	                        order by bs.NextBillDate asc
                        ) t
                    ");

            q.CommandTimeout = 9999;
            q.Parameters.Add("@StartDate", MySqlDbType.DateTime).Value = DateTime.Parse("2011-01-01 00:00:00");
            q.Parameters.Add("@EndDate", MySqlDbType.DateTime).Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);
            count = dao.ExecuteScalar<int>(q);

            return count ?? 0;
        }

        #endregion

        private static void SendEmails(string body, string subject)
        {
            MailMessage mail = new MailMessage();
            mail.Body = body;
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.From = new MailAddress("donotreply@trianglecrm.com", "TriangleCRM Automated Reporting");
            foreach (var email in emails)
            {
                mail.To.Add(email);
            }
            SmtpClient smtp = new SmtpClient("relay.jangosmtp.net");
            smtp.Send(mail);
        }
    }
}
