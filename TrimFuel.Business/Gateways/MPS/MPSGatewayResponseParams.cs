using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.MPS
{
    public class MPSGatewayResponseParams : DelimitedResponseParams
    {
        public MPSGatewayResponseParams(string response)
            : base(response, '&')
        { }
    }
}
