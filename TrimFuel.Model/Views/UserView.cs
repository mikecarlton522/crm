using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class UserView : EntityView
    {
        public Registration Registration { get; set; }
        public RegistrationInfo RegistrationInfo { get; set; }
        public Billing Billing { get; set; }
        public BillingExternalInfo BillingExternalInfo { get; set; }
    }
}
