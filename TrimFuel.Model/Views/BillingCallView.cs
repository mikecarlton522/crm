using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class BillingCallView : EntityView
    {
        public Billing Billing { get; set; }
        public Call LastCall { get; set; }
        public int NumberOfCalls { get; set; }
    }
}
