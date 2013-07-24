using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Model.Utility;
using TrimFuel.Model.Enums;

namespace TrimFuel.Business
{
    public class SalesAgentService : BaseService
    {
        public BusinessError<SalesAgent> CreateSalesAgent(string name, string apiMode, string apiUserName, string apiPassword, decimal? transactionFeeFixed, int? transactionFeePercentage,
            decimal? shipmentFee, decimal? extraSKUShipmentFee, decimal? chargebackFee, decimal? callCenterFeePerMinute, decimal? callCenterFeePerCall, decimal? monthlyCRMFee, int? adminID,
            int? commission, int? commissionMerchant)
        {
            BusinessError<SalesAgent> res = new BusinessError<SalesAgent>(null, BusinessErrorState.Error, null);
            try
            {
                dao.BeginTransaction();

                if (string.IsNullOrEmpty(name))
                {
                    res.ErrorMessage = "Name is not specified";
                }
                else if (GetByName(name) != null)
                {
                    res.ErrorMessage = "Name is already occupied";
                }
                else
                {
                    SalesAgent salesAgent = new SalesAgent();
                    salesAgent.Name = name;
                    salesAgent.IsActive = true;
                    //salesAgent.TransactionFeeFixed = transactionFeeFixed;
                    //salesAgent.TransactionFeePercentage = transactionFeePercentage;
                    //salesAgent.ShipmentFee = shipmentFee;
                    //salesAgent.ExtraSKUShipmentFee = extraSKUShipmentFee;
                    //salesAgent.ChargebackFee = chargebackFee;
                    //salesAgent.CallCenterFeePerMinute = callCenterFeePerMinute;
                    //salesAgent.CallCenterFeePerCall = callCenterFeePerCall;
                    //salesAgent.MonthlyCRMFee = monthlyCRMFee;
                    salesAgent.AdminID = adminID;
                    salesAgent.Commission = commission;
                    salesAgent.CommissionMerchant = commissionMerchant;

                    dao.Save<SalesAgent>(salesAgent);

                    res.ReturnValue = salesAgent;
                    res.State = BusinessErrorState.Success;
                }

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public BusinessError<SalesAgent> UpdateSalesAgent(int? salesAgentID, string name, string apiMode, string apiUserName, string apiPassword, decimal? transactionFeeFixed, int? transactionFeePercentage,
            decimal? shipmentFee, decimal? extraSKUShipmentFee, decimal? chargebackFee, decimal? callCenterFeePerMinute, decimal? callCenterFeePerCall, decimal? monthlyCRMFee, int? adminID,
            int? commision, int? commissionMerchant)
        {
            BusinessError<SalesAgent> res = new BusinessError<SalesAgent>(null, BusinessErrorState.Error, null);
            try
            {
                dao.BeginTransaction();

                if (true == false)
                {
                }
                else
                {
                    SalesAgent temp = GetByName(name);
                    if (temp != null && temp.SalesAgentID != salesAgentID)
                    {
                        res.ErrorMessage = "Login is already occupied";
                    }
                    else
                    {
                        SalesAgent salesAgent = EnsureLoad<SalesAgent>(salesAgentID);
                        salesAgent.Name = name;
                        salesAgent.IsActive = true;
                        //salesAgent.TransactionFeeFixed = transactionFeeFixed;
                        //salesAgent.TransactionFeePercentage = transactionFeePercentage;
                        //salesAgent.ShipmentFee = shipmentFee;
                        //salesAgent.ExtraSKUShipmentFee = extraSKUShipmentFee;
                        //salesAgent.ChargebackFee = chargebackFee;
                        //salesAgent.CallCenterFeePerMinute = callCenterFeePerMinute;
                        //salesAgent.CallCenterFeePerCall = callCenterFeePerCall;
                        //salesAgent.MonthlyCRMFee = monthlyCRMFee;
                        salesAgent.AdminID = adminID;
                        salesAgent.Commission = commision;
                        salesAgent.CommissionMerchant = commissionMerchant;

                        dao.Save<SalesAgent>(salesAgent);

                        res.ReturnValue = salesAgent;
                        res.State = BusinessErrorState.Success;
                    }
                }


                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public BusinessError<SalesAgentTPClient> CreateSalesAgentTpClient(int salesAgentId, int tpClientId)
        {
            BusinessError<SalesAgentTPClient> res = new BusinessError<SalesAgentTPClient>(null, BusinessErrorState.Error, null);
            try
            {
                dao.BeginTransaction();


                SalesAgentTPClient salesAgentTPClient = new SalesAgentTPClient();

                salesAgentTPClient.SalesAgentID = salesAgentId;
                salesAgentTPClient.TPClientID = tpClientId;

                dao.Save<SalesAgentTPClient>(salesAgentTPClient);

                res.ReturnValue = salesAgentTPClient;
                res.State = BusinessErrorState.Success;


                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();

                res.ReturnValue = null;
                res.State = BusinessErrorState.Error;
                res.ErrorMessage = "Unknown error occured";
            }
            return res;
        }

        public bool RemoveAllSalesAgentTPClients(int salesAgentId)
        {
            bool res = false;

            try
            {
                dao.BeginTransaction();

                MySqlCommand q = new MySqlCommand("delete from SalesAgentTPClient " +
                    "where SalesAgentID = @salesAgentID");
                q.Parameters.Add("@salesAgentID", MySqlDbType.Int32).Value = salesAgentId;

                dao.ExecuteNonQuery(q);

                res = true;

                dao.CommitTransaction();
            }
            catch(Exception ex)
            {
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public SalesAgent GetByName(string name)
        {
            SalesAgent res = null;

            try
            {
                MySqlCommand q = new MySqlCommand("select * from SalesAgent " +
                    "where Name = @name");
                q.Parameters.Add("@name", MySqlDbType.VarChar).Value = name;

                res = dao.Load<SalesAgent>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }

            return res;
        }

        public IList<SalesAgent> GetSalesAgentList()
        {
            IList<SalesAgent> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select sa.* from SalesAgent sa");
                res = dao.Load<SalesAgent>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<TPClient> GetTPClientBySalesAgent(int salesAgentID)
        {
            IList<TPClient> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("select tpc.* from TPClient tpc " +
                    "inner join SalesAgentTPClient satpc on satpc.TPClientID = tpc.TPClientID " +
                    "where satpc.SalesAgentID = @salesAgentID " +
                    "order by tpc.TPClientID");
                q.Parameters.Add("@salesAgentID", MySqlDbType.Int32).Value = salesAgentID;

                res = dao.Load<TPClient>(q);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public IList<Admin> GetAdminList()
        {
            IList<Admin> res = null;
            try
            {
                MySqlCommand q = new MySqlCommand("Select * From Admin Where Active=1");
                res = dao.Load<Admin>(q).ToList();
            }
            catch (Exception ex)
            {
                res = new List<Admin>();
                logger.Error(GetType(), ex);
            }
            return res;
        }
    }
}
