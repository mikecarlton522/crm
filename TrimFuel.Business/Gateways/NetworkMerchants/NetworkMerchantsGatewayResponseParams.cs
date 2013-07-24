using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.NetworkMerchants
{
    public class NetworkMerchantsGatewayResponseParams : IGatewayResponseParams
    {
        public NetworkMerchantsGatewayResponseParams(string response)
        {
            Response = response;
        }

        public string Response { get; set; }

        #region IGatewayResponseParams Members

        public string GetParam(string paramName)
        {
            if (string.IsNullOrEmpty(Response) || string.IsNullOrEmpty(paramName))
            {
                return null;
            }

            string res = null;

            foreach (string param in Response.Split('&'))
            {
                string trimParam = param.Replace("\r", "").Replace("\n", "").Replace(" ", "");
                if (trimParam.ToLower().StartsWith(paramName.ToLower()))
                {
                    string[] keyValue = param.Split('=');
                    if (keyValue.Length > 1)
                    {
                        res = keyValue[1];
                    }
                    break;
                }
            }

            return res;
        }

        #endregion
    }
}
