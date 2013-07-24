using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.Iq2solutions
{
    public class Iq2solutionsGatewayResponseParams : IGatewayResponseParams
    {
        public Iq2solutionsGatewayResponseParams(string response)
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

            foreach (string param in Response.Split('|'))
            {
                if (param.StartsWith(paramName))
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
