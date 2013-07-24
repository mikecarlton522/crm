using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Configuration;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Business;
using TrimFuel.Model.Enums;
using TrimFuel.Model.Views;
using TrimFuel.Business.Gateways;

namespace CoAction_BillQueuedTrials
{
    class Program
    {
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            int limit = int.Parse(ConfigurationSettings.AppSettings["limit"]);

            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

                //get unbilled trials
                MySqlCommand q = new MySqlCommand(@"
                    select * from BillingSale bsl
                    inner join Sale sl on sl.SaleID = bsl.SaleID
                    where sl.CreateDT >= '2011-08-12 00:00:00'
                    and sl.SaleTypeID = 1
                    and bsl.ChargeHistoryID is null or bsl.ChargeHistoryID = 0
                    order by sl.SaleID desc
                    limit @limit
                ");
                q.Parameters.Add("@limit", MySqlDbType.Int32).Value = limit;

                IList<BillingSale> trials = dao.Load<BillingSale>(q);
                foreach (var item in trials)
                {
                    ProcessUnbilledTrial(dao, logger, item);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void ProcessUnbilledTrial(IDao dao, ILog logger, BillingSale bsl)
        {
            try
            {
                logger.Info("------------------------------");
                logger.Info("Trial Sale #" + bsl.SaleID.Value.ToString());

                BillingSubscription bs = dao.Load<BillingSubscription>(bsl.BillingSubscriptionID);
                if (bs == null)
                {
                    throw new Exception("Can't find BillingSubscription");
                }

                logger.Info("Billing#" + bs.BillingID.Value.ToString());

                //Try to find existed trial charges (no matter successful or not)
                //If trail charges exist - remove unbilled trial and set subscription to Inactive
                MySqlCommand q = new MySqlCommand(@"
                    select ch.* from ChargeHistoryEx ch
                    inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID
                    inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID
                    where bs.BillingID = @billingID
                    and cd.SaleTypeID = 1
                ");
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = bs.BillingID;
                IList<ChargeHistoryEx> chList = dao.Load<ChargeHistoryEx>(q);

                if (chList.Count > 0)
                {
                    //Remove trial

                    logger.Info("Trial already exists for this account");

                    bool inactiveTrialSubscription = true;
                    foreach (var item in chList)
                    {
                        if (item.BillingSubscriptionID.Value == bs.BillingSubscriptionID.Value)
                        {
                            inactiveTrialSubscription = false;
                            break;
                        }
                    }

                    try
                    {
                        dao.BeginTransaction();

                        RemoveTrial(dao, logger, bsl.SaleID.Value);

                        if (inactiveTrialSubscription)
                        {
                            bs.StatusTID = BillingSubscriptionStatusEnum.Scrubbed;
                            dao.Save<BillingSubscription>(bs);
                        }

                        dao.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        dao.RollbackTransaction();
                        logger.Error(ex);
                    }
                }
                else
                {
                    Billing b = dao.Load<Billing>(bs.BillingID);
                    if (b == null)
                    {
                        throw new Exception("Can't find Billing");
                    }
                    Subscription s = dao.Load<Subscription>(bs.SubscriptionID);
                    if (s == null)
                    {
                        throw new Exception("Can't find Subscription");
                    }
                    ChargeTrial(dao, logger, bsl, bs, b, s);
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void RemoveTrial(IDao dao, ILog logger, long saleID)
        {
            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand(@"
                            delete from BillingSale where SaleID = @saleID;
                            delete from Sale where SaleID = @saleID;
                        ");
                q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = saleID;

                dao.ExecuteNonQuery(q);

                logger.Info("Trial removed");

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        private static void ChargeTrial(IDao dao, ILog logger, BillingSale sale, BillingSubscription billingSubscription, Billing billing, Subscription subscription)
        {
            MerchantService merchantService = new MerchantService();
            SaleService saleService = new SaleService();
            AssertigyMID assertigyMid = null;
            try
            {
                //TODO: Implement dao.BeginTopTransaction() 
                dao.BeginTransaction();

                if (subscription.InitialBillAmount == null)
                {
                    throw new Exception("Invalid Subscription(" + subscription.SubscriptionID.Value + ") settings: InitialBillAmount");
                }

                decimal amount = subscription.InitialBillAmount.Value;
                decimal? shippingAmount = subscription.InitialShipping;

                if (shippingAmount != null)
                {
                    amount += shippingAmount.Value;
                }

                Set<NMICompany, AssertigyMID> mid = null;
                mid = merchantService.ChooseRandomNMIMerchantAccount(subscription.ProductID.Value, billing, amount);

                if (mid != null)
                {
                    NMICompany nmiCompany = mid.Value1;
                    assertigyMid = mid.Value2;

                    Currency cur = saleService.GetCurrencyByProduct(subscription.ProductID.Value);

                    if (shippingAmount != null)
                    {
                        SaleChargeDetails saleCD = new SaleChargeDetails();
                        saleCD.SaleID = sale.SaleID;
                        saleCD.SaleChargeTypeID = SaleChargeTypeEnum.Shipping;
                        saleCD.Amount = shippingAmount.Value;
                        if (cur != null)
                        {
                            saleCD.CurrencyID = cur.CurrencyID;
                            saleCD.CurrencyAmount = saleCD.Amount;
                            saleCD.Amount = cur.ConvertToUSD(saleCD.CurrencyAmount.Value);
                        }
                        dao.Save<SaleChargeDetails>(saleCD);
                    }

                    Product product = dao.Load<Product>(subscription.ProductID);
                    if (product == null)
                    {
                        throw new Exception("Product was not found");
                    }

                    IPaymentGateway paymentGateway = saleService.GetGatewayByMID(assertigyMid);

                    BusinessError<GatewayResult> paymentResult = paymentGateway.Sale(assertigyMid.MID,
                        nmiCompany.GatewayUsername, nmiCompany.GatewayPassword, amount, cur,
                        sale.SaleID.Value, billing, product);

                    if (paymentResult.State == BusinessErrorState.Success)
                    {
                        billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Active;
                        if (subscription.InitialInterim != null)
                        {
                            billingSubscription.NextBillDate = DateTime.Now.AddDays(subscription.InitialInterim.Value);
                        }
                        logger.Info("Successful charge");
                    }
                    else
                    {
                        dao.RollbackTransaction();

                        billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Declined;
                        logger.Info("Failed charge");

                        dao.BeginTransaction();

                        RemoveTrial(dao, logger, sale.SaleID.Value);
                        sale = null;
                    }

                    dao.Save<BillingSubscription>(billingSubscription);

                    Set<Paygea, ChargeHistoryEx> chargeLog = saleService.ChargeLogging(paymentResult, billing, billingSubscription,
                        subscription.ProductCode, SaleTypeEnum.Billing, assertigyMid, ChargeTypeEnum.Charge, amount, cur);

                    if (sale != null)
                    {
                        sale.PaygeaID = chargeLog.Value1.PaygeaID;
                        sale.ChargeHistoryID = chargeLog.Value2.ChargeHistoryID;
                        dao.Save<BillingSale>(sale);
                    }
                }
                else
                {
                    billingSubscription.StatusTID = BillingSubscriptionStatusEnum.Inactive;
                    dao.Save<BillingSubscription>(billingSubscription);

                    logger.Info("No MID installed. Subscription is set to Inactive");
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }
    }
}
