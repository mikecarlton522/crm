using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class WebStoreEmails : Entity
    {
        public int? WebStoreEmailsID { get; set; }
        public int? WebStoreID { get; set; }
        public string Email { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("WebStoreID", WebStoreID);
        }
    }
}
