using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways
{
    public class StaticResponseParams : Dictionary<string, string>, IGatewayResponseParams
    {
        #region IGatewayResponseParams Members

        public string GetParam(string paramName)
        {
            if (this.ContainsKey(paramName))
            {
                return this[paramName];
            }
            return null;
        }

        #endregion
    }
}
