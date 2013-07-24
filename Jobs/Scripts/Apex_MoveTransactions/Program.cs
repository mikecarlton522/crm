using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Configuration;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Model.Views;
using TrimFuel.Business;

namespace Apex_MoveTransactions
{
    class Program
    {
        /*
         * update 
         * old Registrations to 'Import1'
         * new Billings to 'Import2'
         * 
         * remove:
         * Notes
         * BillingSubscription
         * BillingExternalInfo
         * Billing
         * RegistrationInfo
         * Registration
        */
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog logger = LogManager.GetLogger(typeof(Program));
            int limit = int.Parse(ConfigurationSettings.AppSettings["limit"]);

            try
            {
                IDao dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

                MySqlCommand q = new MySqlCommand(@"
                    select ch.*, cd.* from ChargeHistoryEx ch
                    inner join ChargeDetails cd on cd.ChargeHistoryID = ch.ChargeHistoryID
                    inner join BillingSubscription bs on bs.BillingSubscriptionID = ch.BillingSubscriptionID
                    inner join Billing b on b.BillingID = bs.BillingID
                    where b.URL = 'Import1' and ch.MerchantAccountID <> 54
                    order by ch.Amount desc
                ");
                IList<ChargeDetails> chList = dao.Load<ChargeDetails>(q);
                foreach (var item in chList)
                {
                    if (limit <= 0)
                    {
                        break;
                    }
                    MoveToNewCustomer(logger, dao, item);
                    limit--;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        static void MoveToNewCustomer(ILog logger, IDao dao, ChargeDetails ch)
        {
            logger.Info("----------------------------------------");
            logger.Info("ChargeHistory: " + (ch.ChargeHistoryID ?? new long?(0)).Value.ToString());
            try
            {
                dao.BeginTransaction();

                BillingSubscription bs = dao.Load<BillingSubscription>(ch.BillingSubscriptionID);
                if (bs == null)
                {
                    throw new Exception("Can't find BillingSubscription");
                }

                logger.Info("BillingSubscription: " + bs.BillingSubscriptionID.Value);

                Billing b = dao.Load<Billing>(bs.BillingID);
                if (b == null)
                {
                    throw new Exception("Can't find Billing");
                }

                BillingSubscription newBS = FindNewCustomerSubscription(logger, dao, bs, b);
                if (newBS == null)
                {
                    newBS = FindNewCustomerSubscription2(logger, dao, bs, b);
                }
                if (newBS == null)
                {
                    newBS = FindNewCustomerSubscription3(logger, dao, bs, b);
                }
                if (newBS == null)
                {
                    logger.Info("Error: can't find appropriate subscription with new customer");
                }
                else
                {
                    AssertigyMID am = dao.Load<AssertigyMID>(ch.MerchantAccountID);
                    if (am == null)
                    {
                        logger.Info("Error: can't find AssertigyMID");
                    }

                    MySqlCommand q = null;

                    q = new MySqlCommand(@"
                        select PaygeaID as Value from Paygea 
                        where BillingID = @billingID and Response = @response
                    ");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = bs.BillingID;
                    q.Parameters.Add("@response", MySqlDbType.Text).Value = ch.Response;

                    long? paygeaID = dao.Load<View<long>>(q).FirstOrDefault().Value;
                    if (paygeaID == null)
                    {
                        logger.Info("Error: can't find Paygea");
                    }

                    q = new MySqlCommand(@"
                        update Paygea set BillingID = @billingID where PaygeaID = @paygeaID
                    ");
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = newBS.BillingID;
                    q.Parameters.Add("@paygeaID", MySqlDbType.Int64).Value = paygeaID;
                    dao.ExecuteNonQuery(q);

                    ChargeDetails newCH = new ChargeDetails();
                    newCH.FillFromChargeHistory(ch);
                    newCH.ChargeHistoryID = null;
                    newCH.SaleTypeID = ch.SaleTypeID;
                    newCH.SKU = ch.SKU;
                    newCH.BillingSubscriptionID = newBS.BillingSubscriptionID;
                    dao.Save<ChargeDetails>(newCH);

                    SaleService ss = new SaleService();

                    Notes notes = new Notes();
                    notes.BillingID = (int)newBS.BillingID;
                    notes.AdminID = 0;
                    notes.CreateDT = newCH.ChargeDate;
                    notes.Content = ss.CreateTransactionNote(am, newCH.ChargeTypeID.Value, newCH.SaleTypeID, newCH.Amount.Value, null, null, newCH.Success.Value);
                    dao.Save<Notes>(notes);                    

                    BillingSale newBSL = null;
                    if (ch.Success.Value)
                    {
                        BillingSale bsl = FindSale(logger, dao, ch);
                        if (bsl == null)
                        {
                            throw new Exception("Can't find BillingSale");
                        }

                        logger.Info("Sale: " + bsl.SaleID.ToString());

                        newBSL = new BillingSale();
                        newBSL.FillFromSale(bsl);
                        newBSL.SaleID = null;
                        newBSL.PaygeaID = bsl.PaygeaID;
                        newBSL.RebillCycle = CalculateRebillCycle(logger, dao, newBS);
                        if (newBSL.RebillCycle == null)
                        {
                            throw new Exception("Can't calculate rebill cycle");
                        }
                        newBSL.ProductCode = bsl.ProductCode;
                        newBSL.Quantity = bsl.Quantity;
                        newBSL.ChargeHistoryID = newCH.ChargeHistoryID;
                        newBSL.BillingSubscriptionID = newBS.BillingSubscriptionID;
                        dao.Save<BillingSale>(newBSL);


                        q = new MySqlCommand(@"
                            select * from SaleRefund where SaleID = @saleID
                        ");
                        q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = bsl.SaleID;
                        IList<SaleRefund> saleRefunds = dao.Load<SaleRefund>(q);
                        foreach (var item in saleRefunds)
                        {
                            logger.Info("Refund ChargeHistory: " + item.ChargeHistoryID.ToString());
                            
                            ChargeHistoryEx refundCh = dao.Load<ChargeHistoryEx>(item.ChargeHistoryID);
                            if (refundCh == null)
                            {
                                throw new Exception("Can't find refund ChargeHistory");
                            }                            

                            ChargeHistoryEx newRefundCH = new ChargeHistoryEx();
                            newRefundCH.ChargeTypeID = refundCh.ChargeTypeID;
                            newRefundCH.MerchantAccountID = refundCh.MerchantAccountID;
                            newRefundCH.ChargeDate = refundCh.ChargeDate;
                            newRefundCH.Amount = refundCh.Amount;
                            newRefundCH.AuthorizationCode = refundCh.AuthorizationCode;
                            newRefundCH.TransactionNumber = refundCh.TransactionNumber;
                            newRefundCH.Response = refundCh.Response;
                            newRefundCH.Success = refundCh.Success;
                            newRefundCH.ChildMID = refundCh.ChildMID;
                            newRefundCH.BillingSubscriptionID = newBS.BillingSubscriptionID;
                            dao.Save<ChargeHistoryEx>(newRefundCH);

                            q = new MySqlCommand(@"
                                select PaygeaID as Value from Paygea 
                                where BillingID = @billingID and Response = @response
                            ");
                            q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = bs.BillingID;
                            q.Parameters.Add("@response", MySqlDbType.Text).Value = refundCh.Response;

                            long? refundPaygeaID = dao.Load<View<long>>(q).FirstOrDefault().Value;
                            if (refundPaygeaID == null)
                            {
                                logger.Info("Error: can't find refund Paygea");
                            }

                            q = new MySqlCommand(@"
                                update Paygea set BillingID = @billingID where PaygeaID = @paygeaID
                            ");
                            q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = newBS.BillingID;
                            q.Parameters.Add("@paygeaID", MySqlDbType.Int64).Value = refundPaygeaID;
                            dao.ExecuteNonQuery(q);

                            Notes refundNotes = new Notes();
                            refundNotes.BillingID = (int)newBS.BillingID;
                            refundNotes.AdminID = 0;
                            refundNotes.CreateDT = newRefundCH.ChargeDate;
                            refundNotes.Content = ss.CreateTransactionNote(am, newRefundCH.ChargeTypeID.Value, newCH.SaleTypeID, newRefundCH.Amount.Value, null, null, newRefundCH.Success.Value);
                            dao.Save<Notes>(refundNotes);

                            logger.Info("Refund new ChargeHistory: " + newRefundCH.ChargeHistoryID.ToString());

                            item.SaleID = newBSL.SaleID;
                            item.ChargeHistoryID = newRefundCH.ChargeHistoryID;
                            dao.Save<SaleRefund>(item);

                            q = new MySqlCommand(@"
                                delete from ChargeHistoryEx where ChargeHistoryID = @chargeHistoryID
                            ");
                            q.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = refundCh.ChargeHistoryID;
                            dao.ExecuteNonQuery(q);
                        }

                        q = new MySqlCommand(@"
                            delete from BillingSale where SaleID = @saleID
                        ");
                        q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = bsl.SaleID;
                        dao.ExecuteNonQuery(q);

                        q = new MySqlCommand(@"
                            delete from Sale where SaleID = @saleID
                        ");
                        q.Parameters.Add("@saleID", MySqlDbType.Int64).Value = bsl.SaleID;
                        dao.ExecuteNonQuery(q);
                    }

                    if (newCH.ChargeDate > newBS.LastBillDate)
                    {
                        newBS.LastBillDate = newCH.ChargeDate;
                        dao.Save<BillingSubscription>(newBS);
                    }

                    q = new MySqlCommand(@"
                            delete from ChargeDetails where ChargeHistoryID = @chargeHistoryID
                        ");
                    q.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = ch.ChargeHistoryID;
                    dao.ExecuteNonQuery(q);

                    q = new MySqlCommand(@"
                            delete from ChargeHistoryEx where ChargeHistoryID = @chargeHistoryID
                        ");
                    q.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = ch.ChargeHistoryID;
                    dao.ExecuteNonQuery(q);

                    logger.Info("Moved");
                    logger.Info("New BillingSubscription: " + newBS.BillingSubscriptionID.ToString());
                    logger.Info("New Billing: " + newBS.BillingID.ToString());
                    logger.Info("New ChargeHistory: " + newCH.ChargeHistoryID.ToString());
                    if (newBSL != null)
                    {
                        logger.Info("New Sale: " + newBSL.SaleID.ToString());
                    }
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                dao.RollbackTransaction();
            }
        }

        static int? CalculateRebillCycle(ILog logger, IDao dao, BillingSubscription bs)
        {
            int? res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select count(*) as Value from BillingSale bsl
                    inner join Sale sl on sl.SaleID = bsl.SaleID
                    where sl.SaleTypeID = 2 and bsl.BillingSubscriptionID = @billingSubscriptionID
                ");
                q.Parameters.Add("@billingSubscriptionID", MySqlDbType.Int32).Value = bs.BillingSubscriptionID;

                res = dao.Load<View<int>>(q).FirstOrDefault().Value;
                res += 1;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        static BillingSale FindSale(ILog logger, IDao dao, ChargeHistoryEx ch)
        {
            BillingSale res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from BillingSale bsl
                    inner join Sale sl on sl.SaleID = bsl.SaleID
                    where bsl.ChargeHistoryID = @chargeHistoryID
                ");
                q.Parameters.Add("@chargeHistoryID", MySqlDbType.Int64).Value = ch.ChargeHistoryID;

                res = dao.Load<BillingSale>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return res;
        }

        static BillingSubscription FindNewCustomerSubscription(ILog logger, IDao dao, BillingSubscription bs, Billing b)
        {
            BillingSubscription res = null;

            try
            {
                //First search without CC
                MySqlCommand q = new MySqlCommand(@" 
                    select b.* from Billing b
                    inner join BillingSubscription bs on bs.BillingID = b.BillingID                    
                    where b.URL = 'Import2' and bs.SubscriptionID = @subscriptionID
                    and IfNull(b.FirstName, '') = IfNull(@firstName, '')
                    and IfNull(b.LastName, '') = IfNull(@lastName, '')
                    and IfNull(b.CVV, '') = IfNull(@cvv, '')
                    and IfNull(b.ExpMonth, 0) = IfNull(@expMonth, 0)
                    and IfNull(b.ExpYear, 0) = IfNull(@expYear, 0)
                    and IfNull(b.City, '') = IfNull(@city, '')
                    and IfNull(b.State, '') = IfNull(@state, '')
                    and IfNull(b.Country, '') = IfNull(@country, '')
                    and IfNull(b.Email, '') = IfNull(@email, '')
                    and IfNull(b.Phone, '') = IfNull(@phone, '')
                    order by b.BillingID asc"
                );
                //  and IfNull(b.Address1, '') = IfNull(@address1, '')
                //  and IfNull(b.Address2, '') = IfNull(@address2, '')
                //  and IfNull(b.Zip, '') = IfNull(@zip, '')
                q.Parameters.Add("@subscriptionID", MySqlDbType.Int32).Value = bs.SubscriptionID;
                q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = b.FirstName;
                q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = b.LastName;
                q.Parameters.Add("@cvv", MySqlDbType.VarChar).Value = b.CVV;
                q.Parameters.Add("@expMonth", MySqlDbType.Int32).Value = b.ExpMonth;
                q.Parameters.Add("@expYear", MySqlDbType.Int32).Value = b.ExpYear;
                //q.Parameters.Add("@address1", MySqlDbType.VarChar).Value = b.Address1;
                //q.Parameters.Add("@address2", MySqlDbType.VarChar).Value = b.Address2;
                q.Parameters.Add("@city", MySqlDbType.VarChar).Value = b.City;
                q.Parameters.Add("@state", MySqlDbType.VarChar).Value = b.State;
                //q.Parameters.Add("@zip", MySqlDbType.VarChar).Value = b.Zip;
                q.Parameters.Add("@country", MySqlDbType.VarChar).Value = b.Country;
                q.Parameters.Add("@email", MySqlDbType.VarChar).Value = b.Email;
                q.Parameters.Add("@phone", MySqlDbType.VarChar).Value = b.Phone;

                Billing found = null;

                foreach (Billing match in dao.Load<Billing>(q))
                {
                    if (b.CreditCard == match.CreditCard)
                    {
                        //Exact match
                        found = match;
                        break;
                    }
                    else
                    {
                        bool matchCC = false;
                        try
                        {
                            matchCC = (b.CreditCardCnt.DecryptedCreditCard == match.CreditCardCnt.DecryptedCreditCard);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("ERROR: Can't decrypt CC of match(" + match.BillingID.ToString() + ") for account(" + b.BillingID + "). Account will be skipped.", ex);
                        }
                        if (matchCC)
                        {
                            found = match;
                            break;
                        }
                    }
                }

                if (found != null)
                {
                    q = new MySqlCommand(@"
                        select bs.* from BillingSubscription bs
                        where bs.BillingID = @billingID and bs.SubscriptionID = @subscriptionID
                    ");
                    q.Parameters.Add("@subscriptionID", MySqlDbType.Int32).Value = bs.SubscriptionID;
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = found.BillingID;
                    res = dao.Load<BillingSubscription>(q).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        static BillingSubscription FindNewCustomerSubscription2(ILog logger, IDao dao, BillingSubscription bs, Billing b)
        {
            BillingSubscription res = null;

            try
            {
                //First search without CC
                MySqlCommand q = new MySqlCommand(@" 
                    select b.* from Billing b
                    inner join BillingSubscription bs on bs.BillingID = b.BillingID                    
                    where b.URL = 'Import2' and bs.SubscriptionID = @subscriptionID
                    and IfNull(b.FirstName, '') = IfNull(@firstName, '')
                    and IfNull(b.LastName, '') = IfNull(@lastName, '')
                    and IfNull(b.ExpMonth, 0) = IfNull(@expMonth, 0)
                    and IfNull(b.ExpYear, 0) = IfNull(@expYear, 0)
                    and IfNull(b.State, '') = IfNull(@state, '')
                    and IfNull(b.Country, '') = IfNull(@country, '')
                    and IfNull(b.Email, '') = IfNull(@email, '')
                    and IfNull(b.Phone, '') = IfNull(@phone, '')
                    order by b.BillingID asc"
                );
                q.Parameters.Add("@subscriptionID", MySqlDbType.Int32).Value = bs.SubscriptionID;
                q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = b.FirstName;
                q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = b.LastName;
                q.Parameters.Add("@expMonth", MySqlDbType.Int32).Value = b.ExpMonth;
                q.Parameters.Add("@expYear", MySqlDbType.Int32).Value = b.ExpYear;
                q.Parameters.Add("@state", MySqlDbType.VarChar).Value = b.State;
                q.Parameters.Add("@country", MySqlDbType.VarChar).Value = b.Country;
                q.Parameters.Add("@email", MySqlDbType.VarChar).Value = b.Email;
                q.Parameters.Add("@phone", MySqlDbType.VarChar).Value = b.Phone;

                Billing found = null;

                foreach (Billing match in dao.Load<Billing>(q))
                {
                    if (b.CreditCard == match.CreditCard)
                    {
                        //Exact match
                        found = match;
                        break;
                    }
                    else
                    {
                        bool matchCC = false;
                        try
                        {
                            matchCC = (b.CreditCardCnt.DecryptedCreditCard == match.CreditCardCnt.DecryptedCreditCard);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("ERROR: Can't decrypt CC of match(" + match.BillingID.ToString() + ") for account(" + b.BillingID + "). Account will be skipped.", ex);
                        }
                        if (matchCC)
                        {
                            found = match;
                            break;
                        }
                    }
                }

                if (found != null)
                {
                    q = new MySqlCommand(@"
                        select bs.* from BillingSubscription bs
                        where bs.BillingID = @billingID and bs.SubscriptionID = @subscriptionID
                    ");
                    q.Parameters.Add("@subscriptionID", MySqlDbType.Int32).Value = bs.SubscriptionID;
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = found.BillingID;
                    res = dao.Load<BillingSubscription>(q).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }

        static BillingSubscription FindNewCustomerSubscription3(ILog logger, IDao dao, BillingSubscription bs, Billing b)
        {
            BillingSubscription res = null;

            try
            {
                //First search without CC
                MySqlCommand q = new MySqlCommand(@" 
                    select b.* from Billing b
                    inner join BillingSubscription bs on bs.BillingID = b.BillingID                    
                    where b.URL = 'Import2' and bs.SubscriptionID = @subscriptionID
                    and IfNull(b.FirstName, '') = IfNull(@firstName, '')
                    and IfNull(b.LastName, '') = IfNull(@lastName, '')
                    and IfNull(b.CVV, '') = IfNull(@cvv, '')
                    and IfNull(b.ExpMonth, 0) = IfNull(@expMonth, 0)
                    and IfNull(b.ExpYear, 0) = IfNull(@expYear, 0)
                    and IfNull(b.City, '') = IfNull(@city, '')
                    and IfNull(b.State, '') = IfNull(@state, '')
                    and IfNull(b.Country, '') = IfNull(@country, '')
                    and IfNull(b.Email, '') = IfNull(@email, '')
                    and IfNull(b.Phone, '') = IfNull(@phone, '')
                    order by b.BillingID asc"
                );
                q.Parameters.Add("@subscriptionID", MySqlDbType.Int32).Value = bs.SubscriptionID;
                q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = b.FirstName;
                q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = b.LastName;
                q.Parameters.Add("@cvv", MySqlDbType.VarChar).Value = b.CVV;
                q.Parameters.Add("@expMonth", MySqlDbType.Int32).Value = b.ExpMonth;
                q.Parameters.Add("@expYear", MySqlDbType.Int32).Value = b.ExpYear;
                q.Parameters.Add("@city", MySqlDbType.VarChar).Value = b.City;
                q.Parameters.Add("@state", MySqlDbType.VarChar).Value = b.State;
                q.Parameters.Add("@country", MySqlDbType.VarChar).Value = b.Country;
                q.Parameters.Add("@email", MySqlDbType.VarChar).Value = b.Email;
                q.Parameters.Add("@phone", MySqlDbType.VarChar).Value = b.Phone;

                Billing found = dao.Load<Billing>(q).FirstOrDefault();

                if (found != null)
                {
                    q = new MySqlCommand(@"
                        select bs.* from BillingSubscription bs
                        where bs.BillingID = @billingID and bs.SubscriptionID = @subscriptionID
                    ");
                    q.Parameters.Add("@subscriptionID", MySqlDbType.Int32).Value = bs.SubscriptionID;
                    q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = found.BillingID;
                    res = dao.Load<BillingSubscription>(q).FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return res;
        }
    }
}
