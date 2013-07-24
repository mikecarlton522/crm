using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Business.Dao;
using log4net;
using TrimFuel.Model;

namespace TrimFuel.Business
{
    public class BaseService
    {
        protected IDao dao = null;
        //protected IDao acaiDao = MySqlDaoFactory.CreateDao(DB.AcaiCRM);
        protected static readonly ILog logger = LogManager.GetLogger(typeof(BaseService));

        public BaseService()
        {
            try
            {
                dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                throw new Exception("Can't create DAO", ex);
            }
        }

        protected const string ENSURE_LOAD_EXCEPTION = "{0}({1}) was not found in database.";

        protected TEntity EnsureLoad<TEntity>(object key) where TEntity : Entity
        {
            TEntity res = dao.Load<TEntity>(key);
            
            if (res == null)
                throw new Exception(string.Format(ENSURE_LOAD_EXCEPTION, typeof(TEntity).Name, key));

            return res;
        }

        public TEntity Load<TEntity>(object key) where TEntity : Entity
        {
            TEntity res = null;
            try
            {
                res = dao.Load<TEntity>(key);
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return res;
        }

        public bool Save<TEntity>(TEntity obj) where TEntity : Entity
        {
            bool res = true;
            try
            {
                dao.BeginTransaction();

                dao.Save<TEntity>(obj);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                res = false;
                dao.RollbackTransaction();
                logger.Error(GetType(), ex);                
            }
            return res;
        }
    }
}
