using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.MSC
{
    public class MSCGatewayResponseParams : DelimitedResponseParams
    {
        public MSCGatewayResponseParams(string response)
            : base(response, '&')
        { }
    }
}
