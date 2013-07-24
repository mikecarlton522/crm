using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;

namespace TrimFuel.Business.Gateways
{
    public interface IBadCustomerGateway
    {
        BusinessError<GatewayResult> ValidateCustomer(Billing billing);
    }
}
