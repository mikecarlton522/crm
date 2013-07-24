using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Business.Dao;
using TrimFuel.Business.Utils;
using MySql.Data.MySqlClient;
using TrimFuel.Model.Views;
using TrimFuel.Model.Utility;
using TrimFuel.Model.Enums;
using TrimFuel.Business;

namespace TrimFuel.Business
{
    public class SupportService : BaseService
    {
        private TPClientService clientService = new TPClientService();

        public Call ProcessCall(string partner, string version, long? externalCallID, DateTime? startDT, DateTime? endDT, string ani, string dnis,
            int? agentID, string agentName, string agentLocation,
            int? dispositionID, string dispositionName, string agentNotes,
            long? customerID, string customerProduct, int? holdTime)
        {
            TPClient client = GetClientBySupportData(customerProduct);

            int clientID = -1; //stodb by default

            if (client != null)
                clientID = (int)client.TPClientID;

            dao = clientService.GetClientDao(clientID);

            Call res = null;

            try
            {
                dao.BeginTransaction();

                if (externalCallID != null)
                {
                    res = GetCallByExternalCallID(dao, (long)externalCallID);
                }

                if (res == null)
                {
                    res = new Call();
                    res.ExternalCallID = externalCallID;
                    res.CreateDT = DateTime.Now;
                }

                res.StartDT = startDT;
                res.EndDT = endDT;
                res.ANI = ani;
                res.DNIS = dnis;
                res.AgentID = agentID;
                res.AgentName = agentName;
                res.AgentLocation = agentLocation;
                res.DispositionID = dispositionID;
                res.DispositionName = dispositionName;
                res.AgentNotes = agentNotes;
                res.CustomerID = customerID;
                res.CustomerProduct = customerProduct;
                res.Partner = partner;
                res.Version = version;

                if (holdTime == null)
                    holdTime = 0;

                res.HoldTime = holdTime;

                dao.Save<Call>(res);

                dao.CommitTransaction();
            }
            catch (Exception ex)
            {
                dao.RollbackTransaction();
                res = null;
                logger.Error(GetType(), ex);
            }
            return res;
        }

        public Call GetCallByExternalCallID(IDao dao, long externalCallID)
        {
            Call res = null;

            MySqlCommand q = new MySqlCommand(@"
                select * from `Call`
                where ExternalCallID = @externalCallID
                order by CallID desc
                limit 1");

            q.Parameters.Add("@externalCallID", MySqlDbType.Int64).Value = externalCallID;

            try
            {
                res = dao.Load<Call>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
            }

            return res;
        }

        public TPClient GetClientBySupportData(string data)
        {
            dao = MySqlDaoFactory.CreateDao(DB.TrimFuel);
            TPClient res = null;
            try
            {
                MySqlCommand q = new MySqlCommand(@"
                    select tpc.* from CallClientMapping ccm
                    left join TPClient tpc on tpc.TPClientID = ccm.ClientID
                    where CustomerProduct = @data");
                q.Parameters.AddWithValue("@data", data);

                res = dao.Load<TPClient>(q).FirstOrDefault();
            }
            catch (Exception ex)
            {
                logger.Error(GetType(), ex);
                res = null;
            }
            return string.IsNullOrEmpty(res.Name) ? null : res;
        }
    }
}
