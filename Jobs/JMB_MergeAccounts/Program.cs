using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using TrimFuel.Business.Dao;
using MySql.Data.MySqlClient;
using TrimFuel.Model;
using TrimFuel.Model.Views;

namespace JMB_MergeAccounts
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
                logger.Info("Merge Imported accounts-------------------------------------------------------------------------");
                PerformMerge_Imported();
                logger.Info("Merge API accounts------------------------------------------------------------------------------");
                PerformMerge_API();
                logger.Info("Merge Imported to API accounts------------------------------------------------------------------");
                PerformMerge_ImportedToAPI();
            }
            catch (Exception ex)
            {
                logger.Error("ERROR: fatal", ex);
            }
        }

        private static void PerformMerge_Imported()
        {
            IList<long> IDs = GetMasterAccountsID_Imported();
            foreach (long id in IDs)
            {
                Billing master = dao.Load<Billing>(id);
                //if exist (not deleted by previous merge work)
                if (master != null)
                {
                    IList<Billing> toMergeList = GetMergeAccounts_Imported(master);
                    if (toMergeList.Count > 0)
                    {
                        bool merged = true;

                        try
                        {
                            dao.BeginTransaction();

                            foreach (Billing toMerge in toMergeList)
                            {
                                MySqlCommand cmd = new MySqlCommand(
                                    " delete from BillingExternalInfo" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMerge.BillingID;
                                dao.ExecuteNonQuery(cmd);

                                cmd = new MySqlCommand(
                                    " delete from Billing" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMerge.BillingID;
                                dao.ExecuteNonQuery(cmd);

                                cmd = new MySqlCommand(
                                    " delete from Registration" +
                                    " where RegistrationID = @registrationID"
                                    );
                                cmd.Parameters.Add("@registrationID", MySqlDbType.Int64).Value = toMerge.RegistrationID;
                                dao.ExecuteNonQuery(cmd);
                            }

                            dao.CommitTransaction();
                        }
                        catch (Exception ex)
                        {
                            dao.RollbackTransaction();
                            logger.Error("ERROR: Can't merge imported accounts for master(" + master.BillingID.ToString() + ")", ex);
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
                        Set<Billing, BillingSubscription> masterWithBS = new Set<Billing,BillingSubscription>(){Value1 = master, Value2 = GetBS(master)};
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
                                        " update ChargeHistoryEx set BillingSubscriptionID = @masterBillingSubscriptionID" +
                                        " where BillingSubscriptionID = @billingSubscriptionID"
                                        );
                                    cmd.Parameters.Add("@billingSubscriptionID", MySqlDbType.Int64).Value = toMergeWithBS.Value2.BillingSubscriptionID;
                                    cmd.Parameters.Add("@masterBillingSubscriptionID", MySqlDbType.Int64).Value = masterWithBS.Value2.BillingSubscriptionID;
                                    dao.ExecuteNonQuery(cmd);
                                }

                                //Remove account
                                cmd = new MySqlCommand(
                                    " delete from BillingSubscription" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = toMergeWithBS.Value1.BillingID;
                                dao.ExecuteNonQuery(cmd);

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

        private static BillingExternalInfo GetBEI(Billing b)
        {
            MySqlCommand q = new MySqlCommand(
                " select bei.* from BillingExternalInfo bei" +
                " where bei.BillingID = @billingID" +
                " limit 1"
                );
            q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;
            return dao.Load<BillingExternalInfo>(q).FirstOrDefault();
        }

        private static void PerformMerge_ImportedToAPI()
        {
            IList<long> IDs = GetSuspectMergeAccountsID_ImportedToAPI();
            foreach (long id in IDs)
            {
                Billing suspectMergeAccount = dao.Load<Billing>(id);
                //if exist (not deleted by previous merge work)
                if (suspectMergeAccount != null)
                {
                    Billing mergeAccount = GetMergeAccount_ImportedToAPI(suspectMergeAccount);
                    if (mergeAccount != null)
                    {
                        IList<Billing> masterList = GetMasterAccounts_ImportedToAPI(mergeAccount);
                        if (masterList.Count > 0)
                        {
                            bool merged = true;
                            BillingExternalInfo mergeBEI = GetBEI(mergeAccount);
                            try
                            {
                                dao.BeginTransaction();

                                MySqlCommand cmd = null;

                                foreach (Billing master in masterList)
                                {
                                    BillingExternalInfo masterBEI = new BillingExternalInfo();
                                    masterBEI.BillingID = master.BillingID;
                                    masterBEI.InternalID = mergeBEI.InternalID;
                                    dao.Save<BillingExternalInfo>(masterBEI);
                                }

                                //Delete merged account
                                cmd = new MySqlCommand(
                                    " delete from BillingExternalInfo" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = mergeAccount.BillingID;
                                dao.ExecuteNonQuery(cmd);

                                cmd = new MySqlCommand(
                                    " delete from Billing" +
                                    " where BillingID = @billingID"
                                    );
                                cmd.Parameters.Add("@billingID", MySqlDbType.Int64).Value = mergeAccount.BillingID;
                                dao.ExecuteNonQuery(cmd);

                                cmd = new MySqlCommand(
                                    " delete from Registration" +
                                    " where RegistrationID = @registrationID"
                                    );
                                cmd.Parameters.Add("@registrationID", MySqlDbType.Int64).Value = mergeAccount.RegistrationID;
                                dao.ExecuteNonQuery(cmd);

                                dao.CommitTransaction();
                            }
                            catch (Exception ex)
                            {
                                dao.RollbackTransaction();
                                logger.Error("ERROR: Can't merge imported account(" + mergeAccount.BillingID.ToString() + ") to masters(" + string.Join(",", masterList.Select(i => i.BillingID.ToString()).ToArray()) + ")", ex);
                                merged = false;
                            }

                            if (merged)
                            {
                                logger.Info("Merged: " + string.Join(",", masterList.Select(i => i.BillingID.ToString()).ToArray()) + " <- " + mergeAccount.BillingID.ToString());
                            }
                        }
                    }
                }
            }
        }

        private static IList<long> GetSuspectMergeAccountsID_ImportedToAPI()
        {
            MySqlCommand q = new MySqlCommand(
                " select max(b.BillingID) as Value from Billing b" +
                " where 1" +
                " and b.FirstName is not null and b.LastName is not null and b.Email is not null" +
                " group by b.FirstName, b.LastName, b.Email" +
                " having count(*) > 1"
            );
            return dao.Load<View<long>>(q).Select(i => i.Value.Value).ToList();
        }

        private static Billing GetMergeAccount_ImportedToAPI(Billing b)
        {
            MySqlCommand q = new MySqlCommand(
                " select b.* from Billing b" +
                " inner join BillingExternalInfo bei on bei.BillingID = b.BillingID" +
                " where b.CampaignID = 10000" +
                " and bei.InternalID is not null" +
                " and b.FirstName is not null and b.LastName is not null and b.Email is not null" +
                " and b.FirstName = @firstName" +
                " and b.LastName = @lastName" +
                " and b.Email = @email" +
                " order by b.BillingID desc"
            );
            q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = b.FirstName;
            q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = b.LastName;
            q.Parameters.Add("@email", MySqlDbType.VarChar).Value = b.Email;
            q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;
            return dao.Load<Billing>(q).FirstOrDefault();
        }

        private static IList<Billing> GetMasterAccounts_ImportedToAPI(Billing b)
        {
            MySqlCommand q = new MySqlCommand(
                " select b.* from Billing b" +
                " left join BillingExternalInfo bei on bei.BillingID = b.BillingID" +
                " where b.CampaignID is null" +
                " and bei.InternalID is null" +
                " and b.FirstName is not null and b.LastName is not null and b.Email is not null" +
                " and b.FirstName = @firstName" +
                " and b.LastName = @lastName" +
                " and b.Email = @email" +
                " and b.BillingID <> @billingID" +
                " order by b.BillingID desc"
            );
            q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = b.FirstName;
            q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = b.LastName;
            q.Parameters.Add("@email", MySqlDbType.VarChar).Value = b.Email;
            q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;
            return dao.Load<Billing>(q);
        }

        private static IList<long> GetMasterAccountsID_Imported()
        {
            MySqlCommand q = new MySqlCommand(
                " select max(b.BillingID) as Value from Billing b" +
                " where b.CampaignID = 10000" +
                " and b.FirstName is not null and b.LastName is not null and b.Email is not null" +
                " group by b.FirstName, b.LastName, b.Email" +
                " having count(*) > 1"
            );
            return dao.Load<View<long>>(q).Select(i => i.Value.Value).ToList();
        }

        private static IList<Billing> GetMergeAccounts_Imported(Billing b)
        {
            MySqlCommand q = new MySqlCommand(
                " select b.* from Billing b" +
                " where b.CampaignID = 10000" +
                " and b.FirstName is not null and b.LastName is not null and b.Email is not null" +
                " and b.FirstName = @firstName" +
                " and b.LastName = @lastName" +
                " and b.Email = @email" +
                " and b.BillingID <> @billingID" +
                " order by b.BillingID desc"
            );
            q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = b.FirstName;
            q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = b.LastName;
            q.Parameters.Add("@email", MySqlDbType.VarChar).Value = b.Email;
            q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;
            return dao.Load<Billing>(q);
        }

        private static IList<long> GetSuspectMasterAccountsID_API()
        {
            MySqlCommand q = new MySqlCommand(
                " select max(b.BillingID) as Value from" +
                " (" +
                " select b.FirstName, b.LastName, b.Zip from Billing b" +
                " where CampaignID is null" +
                " and b.CreditCard <> '*...............'" +
                " and b.FirstName is not null and b.LastName is not null and b.CreditCard is not null and b.Zip is not null" +
                " group by b.FirstName, b.LastName, b.Zip" +
                " having count(*) > 1" +
                " ) t" +
                " inner join Billing b on b.CampaignID is null and b.CreditCard <> '*...............' and b.FirstName = t.FirstName and b.LastName = t.LastName and b.Zip = t.Zip" +
                " group by b.FirstName, b.LastName, b.Zip, b.CreditCard" +
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
                    " where CampaignID is null" +
                    " and b.CreditCard <> '*...............'" +
                    " and b.FirstName is not null and b.LastName is not null and b.CreditCard is not null and b.Zip is not null" +
                    " and b.FirstName = @firstName" +
                    " and b.LastName = @lastName" +
                    " and b.Zip = @zip" +
                    " and b.CreditCard = @creditCard" +
                    " and b.BillingID <> @billingID" +
                    " order by b.BillingID desc"
                );
                q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = b.FirstName;
                q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = b.LastName;
                q.Parameters.Add("@zip", MySqlDbType.VarChar).Value = b.Zip;
                q.Parameters.Add("@creditCard", MySqlDbType.VarChar).Value = b.CreditCard;
                q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;
                return dao.Load<Billing>(q);
            }

            //First search without CC
            q = new MySqlCommand(
                " select b.* from Billing b" +
                " where CampaignID is null" +
                " and b.CreditCard <> '*...............'" +
                " and b.FirstName is not null and b.LastName is not null and b.CreditCard is not null and b.Zip is not null" +
                " and b.FirstName = @firstName" +
                " and b.LastName = @lastName" +
                " and b.Zip = @zip" +
                " and b.BillingID <> @billingID" +
                " order by b.BillingID desc"
            );
            q.Parameters.Add("@firstName", MySqlDbType.VarChar).Value = b.FirstName;
            q.Parameters.Add("@lastName", MySqlDbType.VarChar).Value = b.LastName;
            q.Parameters.Add("@zip", MySqlDbType.VarChar).Value = b.Zip;
            q.Parameters.Add("@billingID", MySqlDbType.Int64).Value = b.BillingID;

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
    }
}
