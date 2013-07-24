using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.Pagador
{
    public class PagadorGatewayResponseParams : DelimitedResponseParams
    {
        public PagadorGatewayResponseParams(string response)
            : base(response, '&')
        { }
    }
}
