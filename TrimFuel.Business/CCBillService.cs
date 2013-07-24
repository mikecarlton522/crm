using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;

namespace TrimFuel.Business
{
    public class CCBillService : BaseService
    {
        private AssertigyMID GetMID(string accountNumber)
        {
            AssertigyMID res = null;
            try
            {
                dao.BeginTransaction();



                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                dao.RollbackTransaction();
            }
            return res;
        }
    }
}
