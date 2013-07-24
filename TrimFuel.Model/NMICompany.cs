using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class NMICompany : Entity
    {
        public int? NMICompanyID { get; set; }
        public string CompanyName { get; set; }
        public string GatewayUsername { get; set; }
        public string GatewayPassword { get; set; }
        public string GatewayIntegrated { get; set; }
        public bool? Active { get; set; }
        public bool? Deleted { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("CompanyName", CompanyName, 45);
            v.AssertString("GatewayUsername", GatewayUsername, 45);
            v.AssertString("GatewayPassword", GatewayPassword, 45);
            v.AssertString("GatewayIntegrated", GatewayIntegrated, 100);
        }
    }
}
