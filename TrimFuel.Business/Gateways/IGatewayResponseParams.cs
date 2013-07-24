using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways
{
    public interface IGatewayResponseParams
    {
        string GetParam(string paramName);
    }
}
