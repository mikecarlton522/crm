using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Business
{
    [Flags()]
    public enum SalePolicyResult
    {
        SaveSubscription = 1
    }

    public class SalePolicy
    {
        public Func<SalePolicyResult> OnAmountIsZero { get; set; }
    }
}
