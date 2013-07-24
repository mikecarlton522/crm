using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;

namespace TrimFuel.Business.Gateways
{
    public interface ISHWGateway
    {
        BusinessError<GatewayResult> SendSHW(SHWProduct shwProduct, Billing billing);
    }
}
