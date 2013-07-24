using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class MagentoConfig : Entity
    {
        public string MagentoURL { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("MagentoURL", MagentoURL, 250);
            v.AssertString("User", User, 50);
            v.AssertString("Password", Password, 50);
            v.AssertNotNull("Active", Active);
        }
    }
}
