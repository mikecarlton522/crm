using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.CBG
{
    public class CBGGatewayResponseParams : DelimitedResponseParams
    {
        public CBGGatewayResponseParams(string response)
            : base(response, '&') 
        {
            
        }
    }
}
