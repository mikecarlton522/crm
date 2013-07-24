using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class RegistrationInfo : Entity
    {
        public long? RegistrationInfoID { get; set; }
        public long? RegistrationID { get; set; }
        public string Country { get; set; }
        public string Neighborhood { get; set; }
        public string CustomField1 { get; set; }
        public string CustomField2 { get; set; }
        public string CustomField3 { get; set; }
        public string CustomField4 { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("RegistrationID", RegistrationID);
            v.AssertString("Country", Country, 100);
        }
    }
}
