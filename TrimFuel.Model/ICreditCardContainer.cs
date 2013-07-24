using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Containers;

namespace TrimFuel.Model
{
    public interface ICreditCardContainer
    {
        string CreditCardRight4 { get; }
        string CreditCardLeft6 { get; }
        int? PaymentTypeID { get; set; }
        int? ExpMonth { get; set; }
        int? ExpYear { get; set; }
    }
}
