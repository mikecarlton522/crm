using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;

namespace TrimFuel.Business.Gateways
{
    public interface IFraudGateway
    {
        BusinessError<GatewayResult> SendFraudScore(Billing billing, Registration registration, RegistrationInfo registrationInfo, Sale sale);
    }
}
