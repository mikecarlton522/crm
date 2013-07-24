using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Net;
using System.IO;
using TrimFuel.Model;
using TrimFuel.Business;

namespace TrimFuel.Web.Admin.Logic.BillingAPI
{
    public class BillingAPITest
    {
        BaseService serv = new BaseService();
        private const string PROTOCOL = "https://";

        public BillingAPITest(int tpClientID)
        {
            TPClientID = tpClientID;
        }

        public int TPClientID { get; set; }
        private TPClient GetTPClient()
        {
            return serv.Load<TPClient>(TPClientID);
        }

        private IBillingAPI CreateBillingAPI(TPClient tpClient)
        {
            if (tpClient.TPModeID == TrimFuel.Model.Enums.TPModeEnum.WebService)
            {
                return new BillingAPI_WebService(PROTOCOL + tpClient.DomainName + "/api/billing_ws.asmx");
            }
            else if (tpClient.TPModeID == TrimFuel.Model.Enums.TPModeEnum.PostGet)
            {
                return new BillingAPI_POST(PROTOCOL + tpClient.DomainName + "/api/billing.ashx");
            }
            else if (tpClient.TPModeID == TrimFuel.Model.Enums.TPModeEnum.NMI_Emulation)
            {
                return new BillingAPI_NMI(PROTOCOL + tpClient.DomainName + "/api/nmi.ashx");
            }
            else if (tpClient.TPModeID == TrimFuel.Model.Enums.TPModeEnum.AuthoriseNET_Emulation)
            {
                return new BillingAPI_AuthNET(PROTOCOL + tpClient.DomainName + "/api/auth_net.ashx");
            }
            return null;
        }

        public string Void(string chargeHistoryID)
        {
            TPClient c = GetTPClient();
            if (c == null)
            {
                return "Error: can't load Client data";
            }
            IBillingAPI api = CreateBillingAPI(c);
            if (api == null)
            {
                return "Error: can't determine API mode";
            }

            string res = String.Empty;
            try
            {
                res = api.Void(c.Username, c.Password, chargeHistoryID);
            }
            catch (Exception ex)
            {
                res = ex.ToString();
            }
            return res;
        }

        public string Refund(string amount, string chargeHistoryID)
        {
            TPClient c = GetTPClient();
            if (c == null)
            {
                return "Error: can't load Client data";
            }
            IBillingAPI api = CreateBillingAPI(c);
            if (api == null)
            {
                return "Error: can't determine API mode";
            }

            string res = String.Empty;
            try
            {
                res = api.Refund(c.Username, c.Password, chargeHistoryID, amount);
            }
            catch (Exception ex)
            {
                res = ex.ToString();
            }
            return res;
        }

        public string Charge(string amount, string shipping, string firstName, string lastName, string address1, string address2, string city, string state, string zip, string phone, string email, string ip, string affiliate, string subAffiliate, string internalID, string paymentType, string creditCard, string cvv, string expMonth, string expYear)
        {
            TPClient c = GetTPClient();
            if (c == null)
            {
                return "Error: can't load Client data";
            }
            IBillingAPI api = CreateBillingAPI(c);
            if (api == null)
            {
                return "Error: can't determine API mode";
            }

            string res = String.Empty;
            try
            {
                res = api.Charge(c.Username, c.Password,
                    amount, shipping,
                    firstName, lastName, address1, address2, city, state, zip, phone, email, ip,
                    affiliate, subAffiliate, internalID,
                    paymentType, creditCard, cvv, expMonth, expYear);

            }
            catch (Exception ex)
            {
                res = ex.ToString();
            }
            return res;
        }
    }
}
