using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.IPG
{
    public class IPGGatewayResponseParams : IGatewayResponseParams
    {
        public IPGGatewayResponseParams(string response)
        {
            Response = response;
        }

        public string Response { get; set; }

        #region IGatewayResponseParams Members

        public string GetParam(string paramName)
        {
            if (string.IsNullOrEmpty(Response) || string.IsNullOrEmpty(paramName))
                return null;

            paramName = paramName.ToLower();

            string[] allowedParams = { "result", "responsecode", "responsetext", "transactionid" };

            if (!allowedParams.Contains(paramName))
                return null;

            string res = null;

            Response = Response.Replace("\r\n", "");
            Response = Response.Replace("\"", "");

            string[] responseParams = Response.Split(',');

            for (int i = 0; i < allowedParams.Length; i++)
            {
                if (allowedParams[i] == paramName)
                {
                    res = responseParams[i];
                    break;
                }
            }

            return res;
        }

        #endregion
    }
}
