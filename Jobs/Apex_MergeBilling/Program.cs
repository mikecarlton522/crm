using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using log4net;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;

namespace Apex_MergeBilling
{
    class Program
    {
        static ILog logger = null;
        static IDao dao = null;

        static void Main(string[] args)
        {
            logger = LogManager.GetLogger(typeof(Program));
            log4net.Config.XmlConfigurator.Configure();
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);

            try
            {
                logger.Info("Merge API accounts------------------------------------------------------------------------------");
                PerformMerge_API();
            }
            catch (Exception ex)
            {
                logger.Error("ERROR: fatal", ex);
            }
        }

        private static void PerformMerge_API()
        {
            IList<long> IDs = GetSuspectMasterAccountsID_API();
            foreach (long id in IDs)
            {
                Billing master = dao.Load<Billing>(id);
                //if exist (not deleted by previous merge work)
                if (master != null)
                {
                    IList<Billing> toMergeList = GetMergeAccounts_API(master);
                    if (toMergeList.Count > 0)
                    {
                        bool merged = true;
                        Set<Billing, BillingSubscription> masterWithBS = new Set<Billing, BillingSubscription>() { Value1 = master, Value2 = GetBS(master) };
                        IList<Set<Billing, BillingSubscription>> toMergeListWithBS = toMergeList.Select(i => new Set<Billing, BillingSubscription>() { Value1 = i, Value2 = GetBS(i) }).ToList();

                        //if master BS is null try find first account (most recently) with BS and set it as master
                        if (masterWithBS.Value2 == null)
                        {
                            Set<Billing, BillingSubscription> newMasterWithBS = null;
                            foreach (Set<Billing, BillingSubscription> item in toMergeListWithBS)
                            {
                                if (item.Value2 != null)
                                {
                                    newMasterWithBS = item;
                                    break;
                                }
                            }
                            if (newMasterWithBS != null)
                            {
                                //place old master to merge list and get new master
                                toMergeListWithBS.Insert(0, masterWithBS);
                                toMergeListWithBS.Remove(newMasterWithBS);

                                toMergeList.Insert(0, masterWithBS.Value1);
                                toMergeList.Remove(newMasterWithBS.Value1);

                                masterWithBS = newMasterWithBS;
                                master = newMasterWithBS.Value1;
                            }
                        }

                        try
                        {
                            dao.BeginTransaction();

                            foreach (Set<Billing, BillingSubscription> toMergeWithBS in toMergeListWithBS)
                            {
                                //Paygea, Notes, ChargeHistoryEx, FailedChargeHistory, Upsell
                                MySqlCommand cmd = null;

                                //Point all info to Master account
                                cmd = new MySqlCommand(
                                    " update Paygea set BillingID = @masterBillingID" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                cmd.Parameters.Add("@masterBillingID", MySqlDbType.Int64).Value = masterWithBS.Value1.BillingID;
                                dao.ExecuteNonQuery(cmd);

                                cmd = new MySqlCommand(
                                    "SELECT tb.TagID as Value FROM TagBillingLink tb where tb.BillingID = @masterBillingID");
                                cmd.Parameters.Add("@masterBillingID", MySqlDbType.Int64).Value = masterWithBS.Value1.BillingID;
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                var tagIDList = dao.Load<View<long>>(cmd).ToList();

                                foreach (var tagID in tagIDList)
                                {
                                    cmd = new MySqlCommand(
                                        "SELECT Count(*) as Value FROM TagBillingLink where BillingID = @masterBillingID AND TagID = @tagID");
                                    cmd.Parameters.Add("@masterBillingID", MySqlDbType.Int64).Value = masterWithBS.Value1.BillingID;
                                    cmd.Parameters.Add("@tagID", MySqlDbType.Int64).Value = tagID.Value;
                                    long count = dao.Load<View<long>>(cmd).Select(u => u.Value.Value).FirstOrDefault();

                                    if (count == 0)
                                    {
                                        cmd = new MySqlCommand(
                                            " update TagBillingLink set BillingID = @masterBillingID" +
                                            " where BillingID = @billingID" +
                                            " and TagID=@tagID"
                                            );
                                        cmd.Parameters.Add("@tagID", MySqlDbType.Int64).Value = tagID.Value;
                                        cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                        cmd.Parameters.Add("@masterBillingID", MySqlDbType.Int64).Value = masterWithBS.Value1.BillingID;
                                        dao.ExecuteNonQuery(cmd);
                                    }
                                    else
                                    {
                                        cmd = new MySqlCommand(
                                            " delete from TagBillingLink" +
                                            " where BillingID = @billingID" +
                                            " and TagID=@tagID"
                                            );
                                        cmd.Parameters.Add("@tagID", MySqlDbType.Int64).Value = tagID.Value;
                                        cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                        dao.ExecuteNonQuery(cmd);
                                    }
                                }

                                cmd = new MySqlCommand(
                                    " update Notes set BillingID = @masterBillingID" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                cmd.Parameters.Add("@masterBillingID", MySqlDbType.Int64).Value = masterWithBS.Value1.BillingID;
                                dao.ExecuteNonQuery(cmd);

                                cmd = new MySqlCommand(
                                    " update FailedChargeHistory set BillingID = @masterBillingID" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                cmd.Parameters.Add("@masterBillingID", MySqlDbType.Int64).Value = masterWithBS.Value1.BillingID;
                                dao.ExecuteNonQuery(cmd);

                                cmd = new MySqlCommand(
                                    " update Upsell set BillingID = @masterBillingID" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                cmd.Parameters.Add("@masterBillingID", MySqlDbType.Int64).Value = masterWithBS.Value1.BillingID;
                                dao.ExecuteNonQuery(cmd);

                                if (toMergeWithBS.Value2 != null)
                                {
                                    cmd = new MySqlCommand(
                                        " update BillingSubscription set BillingID = @masterBillingID" +
                                        " where BillingID = @billingID"
                                        );
                                    cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                    cmd.Parameters.Add("@masterBillingID", MySqlDbType.Int64).Value = masterWithBS.Value1.BillingID;
                                    dao.ExecuteNonQuery(cmd);
                                }

                                //Remove account

                                cmd = new MySqlCommand(
                                    " delete from BillingExternalInfo" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                dao.ExecuteNonQuery(cmd);

                                cmd = new MySqlCommand(
                                    " delete from Billing" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                dao.ExecuteNonQuery(cmd);

                                cmd = new MySqlCommand(
                                    " delete from RegistrationInfo" +
                                    " where RegistrationID = @registrationID"
                                    );
                                cmd.Parameters.Add("@registrationID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.RegistrationID;
                                dao.ExecuteNonQuery(cmd);

                                cmd = new MySqlCommand(
                                    " delete from Registration" +
                                    " where RegistrationID = @registrationID"
                                    );
                                cmd.Parameters.Add("@registrationID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.RegistrationID;
                                dao.ExecuteNonQuery(cmd);
                            }

                            dao.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            dao.RollbackTransaction();
                            logger.Error("ERROR: Can't merge API accounts for master(" + master.BillingID.ToString() + ")", ex);
                            merged = false;
                        }

                        if (merged)
                        {
                            logger.Info("Merged: " + master.BillingID.ToString() + " <- " + string.Join(",", toMergeList.Select(i => i.BillingID.ToString()).ToArray()));
                        }
                    }
                }
            }
        }

        private static IList<long> GetSuspectMasterAccountsID_API()
        {
            MySqlCommand q = new MySqlCommand(
                " select min(b.BillingID) as Value from Billing b" +
                " WHERE b.CreditCard <> '*...............'" +
                " group by b.FirstName, b.LastName, b.Zip, b.Country, b.CVV, b.ExpMonth, b.ExpYear, b.State, b.City, b.CampaignID" +
                " having count(*) > 1" +
                " order by Value desc"
            );
            return dao.Load<View<long>>(q).Select(i => i.Value.Value).ToList();
        }

        private static IList<Billing> GetMergeAccounts_API(Billing b)
        {
            //CreditCard is same or if hash is same check decrypted credit card
            string cc = null;
            try
            {
                cc = b.CreditCardCnt.DecryptedCreditCard;
            }
            catch (Exception ex)
            {
                logger.Error("ERROR: Can't decrypt CC for account(" + b.BillingID.ToString() + "). Accounts to merge will be found only by exact CC match in DB.", ex);
            }

            MySqlCommand q = null;
            if (string.IsNullOrEmpty(cc))
            {
                //No CC search hash, only exact matches of CC
                q = new MySqlCommand(
                    " select b.* from Billing b" +
                    " where b.CampaignID=@campaignID" +
                    " and IfNull(b.FirstName, '') = IfNull(@firstName, '')" +
                    " and IfNull(b.Address1, '') = IfNull(@address1, '')" +
                    " and IfNull(b.Address2, '') = IfNull(@address2, '')" +
                    " and IfNull(b.Email, '') = IfNull(@email, '')" +
                    " and IfNull(b.Phone, '') = IfNull(@phone, '')" +
                    " and IfNull(b.Affiliate, '') = IfNull(@affiliate, '')" +
                    " and IfNull(b.LastName, '') = IfNull(@lastName, '')" +
                    " and IfNull(b.CVV, '') = IfNull(@cvv, '')" +
                    " and IfNull(b.ExpMonth, 0) = IfNull(@expMonth, 0)" +
                    " and IfNull(b.ExpYear, 0) = IfNull(@expYear, 0)" +
                    " and IfNull(b.City, '') = IfNull(@city, '')" +
                    " and IfNull(b.State, '') = IfNull(@state, '')" +
                    " and IfNull(b.Zip, '') = IfNull(@zip, '')" +
                    " and IfNull(b.Country, '') = IfNull(@country, '')" +
                    " and b.CreditCard = @creditCard" +
                    " and b.BillingID <> @billingID" +
                    " order by b.BillingID desc"
                );
                q.Parameters.Add("@expMonth", MySqlDbType.Int32).Value = b.ExpMonth;
                q.Parameters.Add("@expYear", MySqlDbType.Int32).Value = b.ExpYear;
                q.Parameters.Add("@cvv", MySqlDbType.VarChar).Value = b.CVV;
                q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = b.FirstName;
                q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = b.LastName;
                q.Parameters.Add("@zip", MySqlDbType.VarChar).Value = b.Zip;
                q.Parameters.Add("@state", MySqlDbType.VarChar).Value = b.State;
                q.Parameters.Add("@city", MySqlDbType.VarChar).Value = b.City;
                q.Parameters.Add("@country", MySqlDbType.VarChar).Value = b.Country;
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;
                q.Parameters.Add("@campaignID", MySqlDbType.Int64).Value = b.CampaignID;
                q.Parameters.Add("@creditCard", MySqlDbType.Int64).Value = b.CreditCard;
                q.Parameters.Add("@address1", MySqlDbType.VarChar).Value = b.Address1;
                q.Parameters.Add("@address2", MySqlDbType.VarChar).Value = b.Address2;
                q.Parameters.Add("@email", MySqlDbType.VarChar).Value = b.Email;
                q.Parameters.Add("@phone", MySqlDbType.VarChar).Value = b.Phone;
                q.Parameters.Add("@affiliate", MySqlDbType.VarChar).Value = b.Affiliate;
                return dao.Load<Billing>(q);
            }

            //First search without CC
            q = new MySqlCommand(
                " select b.* from Billing b" +
                " where b.CampaignID=@campaignID" +
                " and IfNull(b.FirstName, '') = IfNull(@firstName, '')" +
                " and IfNull(b.LastName, '') = IfNull(@lastName, '')" +
                " and IfNull(b.Address1, '') = IfNull(@address1, '')" +
                " and IfNull(b.Address2, '') = IfNull(@address2, '')" +
                " and IfNull(b.Email, '') = IfNull(@email, '')" +
                " and IfNull(b.Phone, '') = IfNull(@phone, '')" +
                " and IfNull(b.Affiliate, '') = IfNull(@affiliate, '')" +
                " and IfNull(b.CVV, '') = IfNull(@cvv, '')" +
                " and IfNull(b.ExpMonth, 0) = IfNull(@expMonth, 0)" +
                " and IfNull(b.ExpYear, 0) = IfNull(@expYear, 0)" +
                " and IfNull(b.City, '') = IfNull(@city, '')" +
                " and IfNull(b.State, '') = IfNull(@state, '')" +
                " and IfNull(b.Zip, '') = IfNull(@zip, '')" +
                " and IfNull(b.Country, '') = IfNull(@country, '')" +
                " and b.BillingID <> @billingID" +
                " order by b.BillingID desc"
            );
            q.Parameters.Add("@expMonth", MySqlDbType.Int32).Value = b.ExpMonth;
            q.Parameters.Add("@expYear", MySqlDbType.Int32).Value = b.ExpYear;
            q.Parameters.Add("@cvv", MySqlDbType.VarChar).Value = b.CVV;
            q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = b.FirstName;
            q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = b.LastName;
            q.Parameters.Add("@zip", MySqlDbType.VarChar).Value = b.Zip;
            q.Parameters.Add("@state", MySqlDbType.VarChar).Value = b.State;
            q.Parameters.Add("@country", MySqlDbType.VarChar).Value = b.Country;
            q.Parameters.Add("@city", MySqlDbType.VarChar).Value = b.City;
            q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;
            q.Parameters.Add("@campaignID", MySqlDbType.Int64).Value = b.CampaignID;
            q.Parameters.Add("@address1", MySqlDbType.VarChar).Value = b.Address1;
            q.Parameters.Add("@address2", MySqlDbType.VarChar).Value = b.Address2;
            q.Parameters.Add("@email", MySqlDbType.VarChar).Value = b.Email;
            q.Parameters.Add("@phone", MySqlDbType.VarChar).Value = b.Phone;
            q.Parameters.Add("@affiliate", MySqlDbType.VarChar).Value = b.Affiliate;

            IList<Billing> res = new List<Billing>();

            foreach (Billing match in dao.Load<Billing>(q))
            {
                if (b.CreditCard == match.CreditCard)
                {
                    //Exact match
                    res.Add(match);
                }
                else
                {
                    string matchCC = null;
                    try
                    {
                        matchCC = match.CreditCardCnt.DecryptedCreditCard;
                    }
                    catch (Exception ex)
                    {
                        logger.Error("ERROR: Can't decrypt CC of match(" + match.BillingID.ToString() + ") for account(" + b.BillingID + "). Account will be skipped.", ex);
                    }
                    if (!string.IsNullOrEmpty(matchCC) && cc == matchCC)
                    {
                        //Match of decrypted CCs
                        res.Add(match);
                    }
                }
            }

            return res;
        }

        private static BillingSubscription GetBS(Billing b)
        {
            MySqlCommand q = new MySqlCommand(
                " select bs.* from BillingSubscription bs" +
                " where bs.BillingID = @billingID" +
                " order by bs.BillingSubscriptionID desc" +
                " limit 1"
                );
            q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;
            return dao.Load<BillingSubscription>(q).FirstOrDefault();
        }
    }
}
