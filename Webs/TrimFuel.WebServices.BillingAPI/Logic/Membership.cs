using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Business.Dao;
using log4net;
using MySql.Data.MySqlClient;

namespace TrimFuel.WebServices.BillingAPI.Logic
{
    public class Membership
    {
        protected static readonly ILog logger = LogManager.GetLogger(typeof(BaseService));
        public static BusinessError<TPClient> Authorise(int tpModeID, string username, string password)
        {
            BusinessError<TPClient> res = new BusinessError<TPClient>(null, BusinessErrorState.Error, "Unknown error. Please contact support.");
            try
            {
                res = Authorise(Config.Current.TPClientID, tpModeID, username, password);
            }
            catch (Exception ex)
            {
                logger.Error(typeof(Membership), ex);
            }
            return res;
        }

        private static BusinessError<TPClient> Authorise(int tpClientID, int tpModeID, string username, string password)
        {
            BusinessError<TPClient> res = new BusinessError<TPClient>(null, BusinessErrorState.Error, "Unknown error. Please contact support.");
            if (tpClientID == 0)
            {
                //Direct authorization (for STO Billing API)
                try
                {
                    if (username == Config.Current.Username && password == Config.Current.Password)
                    {
                        res.State = BusinessErrorState.Success;
                    }
                    else
                    {
                        res.ErrorMessage = "Invalid username or password.";
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(typeof(Membership), ex);
                }
            }
            else
            {
                IDao dao = null;
                try
                {
                    dao = new MySqlDao(Config.Current.MasterDB.ConnectionString);
                }
                catch (Exception ex)
                {
                    logger.Error(typeof(Membership), ex);
                    //res.ErrorMessage = "Configuration error. Please contact support.";
                }

                if (dao != null)
                {
                    try
                    {
                        TPClient client = dao.Load<TPClient>(tpClientID);
                        //All modes are enabled
                        //if (client.TPModeID != tpModeID)
                        //{
                        //    res.ErrorMessage = "API is disabled in this mode. Please contact support.";
                        //}
                        //else 
                        if (client.Username != username || client.Password != password)
                        {
                            res.ErrorMessage = "Invalid username or password.";
                        }
                        else
                        {
                            res.State = BusinessErrorState.Success;
                            res.ReturnValue = client;
                            res.ErrorMessage = String.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(typeof(Membership), ex);
                    }
                }
            }
            return res;
        }        
    }
}
