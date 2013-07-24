using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TPClient : Entity
    {
        public int? TPClientID { get; set; }
        public string Name { get; set; }
        public int? TPModeID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DomainName { get; set; }
        public string ConnectionString { get; set; }
        public bool TriangleFulfillment { get; set; }
        public bool TriangleCRM { get; set; }
        public bool TelephonyServices { get; set; }
        public bool CallCenterServices { get; set; }
        public bool TechnologyServices { get; set; }
        public bool MediaServices { get; set; }
        public decimal? PostageAllowed { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("Name", Name, 50);
            v.AssertString("Username", Username, 50);
            v.AssertString("Password", Password, 50);
            v.AssertNotNull("DomainName", DomainName);
            v.AssertString("DomainName", DomainName, 100);
        }
    }
}
