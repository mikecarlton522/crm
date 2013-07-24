using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using MySql.Data.MySqlClient;

namespace TrimFuel.Business
{
    public class DashboardService : BaseService
    {
        public Admin GetAdminByName(string displayName)
        {
            Admin res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(
                    " select * from Admin" +
                    " where DisplayName = @displayName");
                q.Parameters.Add("@displayName", MySqlDbType.VarChar).Value = displayName;

                res = dao.Load<Admin>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public RestrictLevel GetRestrictLevelByID(int? restrictLevelID)
        {
            if (restrictLevelID == null)
            {
                return null;
            }

            return Load<RestrictLevel>(restrictLevelID);
        }

        public List<IPRestriction> GetIPRestrictListByID(int? restrictLevelID)
        {
            if (restrictLevelID == null)
            {
                return new List<IPRestriction>();
            }

            var q = new MySqlCommand("Select * from IPRestriction where RestrictLevelID=@RestrictLevelID");
            q.Parameters.AddWithValue("@RestrictLevelID", restrictLevelID);

            return dao.Load<IPRestriction>(q).ToList();
        }

        //public void SaveIPRestriction(List<string> ipList, int? restrictLevelID)
        //{
        //    try
        //    {
        //        dao.BeginTransaction();

        //        if (ipList.Count == 1 && ipList[0].Length == 0)
        //            ipList.RemoveAt(0);

        //        //delete 
        //        var currentIPRestrictionList = GetIPRestrictListByID(restrictLevelID);
        //        var toDeleteList = currentIPRestrictionList.Where(u => !ipList.Contains(u.IP));
        //        foreach (var itemToDelete in toDeleteList)
        //        {
        //            var q = new MySqlCommand("Delete from IPRestriction where RestrictLevelID=@RestrictLevelID and IP=@IP");
        //            q.Parameters.AddWithValue("@RestrictLevelID", restrictLevelID);
        //            q.Parameters.AddWithValue("@IP", itemToDelete.IP);
        //            dao.ExecuteNonQuery(q);
        //        }

        //        //save new
        //        for (int i = 0; i < ipList.Count; i++)
        //        {
        //            if (string.IsNullOrEmpty(ipList[i]))
        //                continue;

        //            var q = new MySqlCommand("Select * from IPRestriction where RestrictLevelID=@RestrictLevelID and IP=@IP");
        //            q.Parameters.AddWithValue("@RestrictLevelID", restrictLevelID);
        //            q.Parameters.AddWithValue("@IP", ipList[i]);

        //            var existing = dao.Load<IPRestriction>(q).FirstOrDefault();
        //            if (existing == null)
        //            {
        //                dao.Save<IPRestriction>(new IPRestriction()
        //                {
        //                    RestrictLevelID = restrictLevelID,
        //                    IP = ipList[i]
        //                });
        //            }
        //        }

        //        dao.CommitTransaction();
        //    }
        //    catch (Exception ex)
        //    {
        //        dao.RollbackTransaction();
        //        logger.Error(GetType(), ex);
        //    }
        //}

        public void DeleteIPRestriction(int? ipRestrictionID)
        {
            try
            {
                var q = new MySqlCommand("Delete from IPRestriction where IPRestrictionID=@IPRestrictionID");
                q.Parameters.AddWithValue("@IPRestrictionID", ipRestrictionID);
                dao.ExecuteNonQuery(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
        }

        public IList<TPClient> GetTPClientList()
        {
            IList<TPClient> res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(
                    " select * from TPClient");

                res = dao.Load<TPClient>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public IList<TPClientNews> GetNewsList(int AdminID)
        {
            IList<TPClientNews> tpClientNews = null;
            try
            {
                MySqlCommand q = new MySqlCommand(
                    "select TPClientNewsID, AdminID, Content, DATE(CreateDT) as CreateDT, Active from TPClientNews " +
                    "where TPClientNews.TPClientNewsID not in " +
                    "(select TPClientNewsID from TPClientNewsRead where TPClientNewsRead.AdminID = '" + AdminID + "')");

                tpClientNews = dao.Load<TPClientNews>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return tpClientNews;
        }

        public void MarkNewsAsRead(int tpClientNewsID, int adminID)
        {
            TPClientNewsRead newsRead = new TPClientNewsRead();
            newsRead.TPClientNewsID = tpClientNewsID;
            newsRead.AdminID = adminID;
            dao.Save<TPClientNewsRead>(newsRead);
        }

        public IList<Affiliate> GetAffiliates()
        {
            IList<Affiliate> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(
                    "select a.* from Affiliate a");

                res = dao.Load<Affiliate>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<ChargebackStatusType> GetChargebackStatusTypeList()
        {
            IList<ChargebackStatusType> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from ChargebackStatusType 
                    order by ChargebackStatusTypeID desc
                ");

                res = dao.Load<ChargebackStatusType>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<ChargebackReasonCode> GetChargebackReasonCodeList(int paymentTypeID)
        {
            IList<ChargebackReasonCode> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from ChargebackReasonCode 
                    where PaymentTypeID = @paymentTypeID
                    order by ChargebackReasonCodeID desc
                ");
                q.Parameters.Add("@paymentTypeID", MySqlDbType.Int32).Value = paymentTypeID;

                res = dao.Load<ChargebackReasonCode>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<Currency> GetCurrencyList()
        {
            IList<Currency> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(
                    "select c.* from Currency c");

                res = dao.Load<Currency>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public IList<ExtraTrialShipType> GetActiveFreeItems()
        {
            IList<ExtraTrialShipType> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from ExtraTrialShipType
                    where Active = 1
                ");

                res = dao.Load<ExtraTrialShipType>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public Campaign GetOrCreateCRMCampaign()
        {
            Campaign res = null;

            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select * from Campaign 
                    where URL = 'Custom Billing Form'
                ");
                res = dao.Load<Campaign>(q).FirstOrDefault();
                if (res == null)
                {
                    res = new Campaign()
                    {
                        DisplayName = "Custom Billing Form",
                        SubscriptionID = null,
                        Active = true,
                        CreateDT = DateTime.Now,
                        URL = "Custom Billing Form"
                    };
                    dao.Save(res);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }

            return res;
        }

        public Notes AddBillingNote(long billingID, int? adminID, string content)
        {
            Notes res = null;
            try
            {
                res = new Notes();
                res.BillingID = (int)billingID;
                res.AdminID = adminID ?? 0;
                res.Content = content;
                res.CreateDT = DateTime.Now;
                dao.Save(res);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                res = null;
            }
            return res;
        }
    }
}
