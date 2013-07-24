using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Views;

namespace STO_ProcessAdditionalBillingSalesForSeveralTransactions
{
    class Program
    {
        //static int MID_ID = 41;
        //static long[] TRANSACTION_LIST = {303457, 303467, 304186, 304219, 304248, 304298, 304309, 304345,
        //                                 304357, 304378, 304413, 304442, 304459, 304469, 304472, 304485};
        //Local Test Data
        static int MID_ID = 17;
        static long[] TRANSACTION_LIST = {179753, 179752, 179751, 179750, 179749, 179748};

        static ILog logger = null;
        static IDao dao = null;
        static void Main(string[] args)
        {
            logger = LogManager.GetLogger(typeof(Program));
            log4net.Config.XmlConfigurator.Configure();
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            SaleService saleService = new SaleService();
            try
            {
                AssertigyMID mid = dao.Load<AssertigyMID>(MID_ID);
                if (mid == null)
                {
                    throw new Exception("Can't find MID(" + MID_ID + ")");
                }


                foreach(long chID in TRANSACTION_LIST)
                {
                    logger.Info("Process transaction(" + chID + ")-------------------------------------");
                    try
                    {
                        ChargeHistoryEx ch = dao.Load<ChargeHistoryEx>(chID);
                        if (ch == null)
                        {
                            throw new Exception("Can't find ChargeHistoryEx(" + chID + ")");
                        }

                        BillingSubscription bs = dao.Load<BillingSubscription>(ch.BillingSubscriptionID);
                        if (bs == null)
                        {
                            throw new Exception("Can't find BillingSubscription(" + ch.BillingSubscriptionID + ") for ChargeHistoryEx(" + ch.ChargeHistoryID + ")");
                        }

                        Billing b = dao.Load<Billing>(bs.BillingID);
                        if (b == null)
                        {
                            throw new Exception("Can't find Billing(" + bs.BillingID + ") for BillingSubscription(" + bs.BillingSubscriptionID + ")");
                        }

                        logger.Info("BillingID: " + b.BillingID);

                        BusinessError<Set<BillingSale, BillingSubscription, AssertigyMID>> newSl = saleService.CreateBillingSale_ForExistedSubscription_WithSpecifiedMID_NoEmergencyQueue_NoEmails(b, bs, ch.Amount.Value, mid);
                        if (newSl.State == BusinessErrorState.Success)
                        {
                            newSl.ReturnValue.Value1.NotShip = true;
                            dao.Save<BillingSale>(newSl.ReturnValue.Value1);

                            logger.Info("Success");
                        }
                        else
                        {
                            logger.Info("Failure");
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("Fatal Error", ex);
            }
        }
    }
}
