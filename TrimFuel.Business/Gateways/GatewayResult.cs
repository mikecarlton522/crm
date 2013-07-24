using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways
{
    public class GatewayResult
    {
        public string Request { get; set; }
        public string Response { get; set; }
        public string BinName { get; set; }
        public IGatewayResponseParams ResponseParams { get; set; }
    }
}
