using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business.Gateways.MPS
{
    public enum TransactionType
    {
        CreditCardAuthorization,
        CreditCardSettle,
        CreditCardCharge,
        CreditCardCredit,
        CreditCardVoid,
        AddCustomerCCCharge,
        CreditCardHosted,
        CreditCardReBill,
        CreditCardRecurringCharge

    }
}
