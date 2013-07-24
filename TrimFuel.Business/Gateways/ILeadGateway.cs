using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model;
using TrimFuel.Model.Views;

namespace TrimFuel.Business.Gateways
{
    public interface ILeadGateway
    {
        void Send(LeadPost leadPost);
        void SaveAbandon(Registration reg, LeadRoutingView routingRule);
        void SaveConfirm(Billing bill, LeadRoutingView routingRule);
        void SaveDeclined(Billing bill, LeadRoutingView routingRule);
        void SaveInactive(Billing bill, LeadRoutingView routingRule);
    }
}
