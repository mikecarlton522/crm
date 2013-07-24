using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Admin : Entity
    {
        public int? AdminID { get; set; }
        public string DisplayName { get; set; }
        public bool? Active { get; set; }
        public string Password { get; set; }
        public int? RestrictLevel { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("DisplayName", DisplayName, 50);
            v.AssertString("Password", Password, 50);
        }
    }
}
