using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business;

namespace Metabolab_BillImportedOrders
{
    class Program
    {
        private const int CAMPAIGN_ID = 10000;
        private const int FAKE_MID_ID = 36;
        private static DateTime NOT_RECURRING_NEXT_BILL_DATE = new DateTime(2020, 12, 31);

        static ILog logger = null;
        static IDao dao = null;
        static void Main(string[] args)
        {
            logger = LogManager.GetLogger(typeof(Program));
            log4net.Config.XmlConfigurator.Configure();
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

            try
            {
                logger.Info("Bill Imported accounts-------------------------------------------------------------------------");
                MySqlCommand q = new MySqlCommand(
                    " select bsl.*, sl.* from BillingSale bsl" +
                    " inner join Sale sl on bsl.SaleID = sl.SaleID" +
                    " inner join ChargeHistoryEx ch on ch.ChargeHistoryID = bsl.ChargeHistoryID" +
                    " inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID" +
                    " where ch.MerchantAccountID = @FAKE_MID_ID" +
                    " and bs.NextBillDate < @tomorrow" +
                    " order by ch.ChargeDate" +
                    " limit 20");
                q.Parameters.Add("@FAKE_MID_ID", MySqlDbType.Int32).Value = FAKE_MID_ID;
                q.Parameters.Add("@tomorrow", MySqlDbType.Timestamp).Value = DateTime.Today.AddDays(1);

                IList<BillingSale> slList = dao.Load<BillingSale>(q);
                foreach (BillingSale sl in slList)
                {
                    BillAndDeleteFakeSale(sl);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ERROR: fatal", ex);
            }
        }

        static void BillAndDeleteFakeSale(BillingSale sl)
        {
            try
            {
                logger.Info("Process Billing Sale: " + sl.SaleID + " -------------------------------------------------------------------------");
                ChargeHistoryEx ch = dao.Load<ChargeHistoryEx>(sl.ChargeHistoryID);
                if (ch == null)
                {
                    throw new Exception("Can't find ChargeHistoryEx(" + sl.ChargeHistoryID + ") for BillingSale(" + sl.SaleID + ")");
                }
                BillingSubscription bs = dao.Load<BillingSubscription>(ch.BillingSubscriptionID);
                if (bs == null)
                {
                    throw new Exception("Can't find BillingSubscription(" + ch.BillingSubscriptionID + ") for ChargeHistoryEx(" + ch.ChargeHistoryID + ")");
                }
                Subscription s = dao.Load<Subscription>(bs.SubscriptionID);
                if (s == null)
                {
                    throw new Exception("Can't find Subscription(" + bs.SubscriptionID + ") for BillingSubscription(" + bs.BillingSubscriptionID + ")");
                }
                Billing b = dao.Load<Billing>(bs.BillingID);
                if (b == null)
                {
                    throw new Exception("Can't find Billing(" + bs.BillingID + ") for BillingSubscription(" + bs.BillingSubscriptionID + ")");
                }

                MySqlCommand q = null;

                //logger.Info("deleting old Notes...");
                //q = new MySqlCommand(
                //    " delete from Notes where BillingID = @billingID");
                //q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;
                //dao.ExecuteNonQuery(q);

                SaleService saleService = new SaleService();
                BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> newSl = saleService.CreateBillingSale_ForExistedSubscription_WithRandomMID_MoEmergencyQueue_NoEmails(b, bs, ch.Amount.Value);

                logger.Info("Billing result: " + newSl.State);

                if (newSl.State == BusinessErrorState.Success)
                {
                    logger.Info("deleting old BillingSale...");
                    q = new MySqlCommand(
                        " delete from BillingSale where SaleID = @saleID");
                    q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = sl.SaleID;
                    dao.ExecuteNonQuery(q);

                    logger.Info("deleting old Sale...");
                    q = new MySqlCommand(
                        " delete from Sale where SaleID = @saleID");
                    q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = sl.SaleID;
                    dao.ExecuteNonQuery(q);

                    logger.Info("deleting old Paygea...");
                    q = new MySqlCommand(
                        " delete from Paygea where PaygeaID = @paygeaID");
                    q.Parameters.Add("@paygeaID", MySqlDbType.Int64).Value = sl.PaygeaID;
                    dao.ExecuteNonQuery(q);

                    logger.Info("deleting old ChargeHistoryEx...");
                    q = new MySqlCommand(
                        " delete from ChargeHistoryEx where ChargeHistoryID = @chargeHistoryID");
                    q.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = sl.ChargeHistoryID;
                    dao.ExecuteNonQuery(q);

                    logger.Info("updating LastBillDate and NextBillDate...");
                    bs.LastBillDate = DateTime.Now;
                    if (s.Recurring.Value)
                    {
                        bs.NextBillDate = DateTime.Now.AddDays(s.InitialInterim.Value);
                    }
                    else
                    {
                        bs.NextBillDate = NOT_RECURRING_NEXT_BILL_DATE;
                    }
                    dao.Save<BillingSubscription>(bs);
                }
                else
                {
                    logger.Info("updating LastBillDate and NextBillDate...");
                    bs.LastBillDate = DateTime.Now;
                    bs.NextBillDate = DateTime.Now.AddDays(7);
                    dao.Save<BillingSubscription>(bs);
                }
            }
            catch (Exception ex)
            {
                logger.Error("ERROR", ex);
            }
        }
    }
}
