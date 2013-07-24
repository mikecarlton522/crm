using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.PrismPay
{
    public class PrismPayGatewayResponseParams : IGatewayResponseParams
    {
        public PrismPayGatewayResponseParams(string response)
        {
            Response = response;
        }

        public string Response { get; set; }

        #region IGatewayResponseParams Members

        public string GetParam(string paramName)
        {
            return string.Empty;
        }

        #endregion
    }
}
