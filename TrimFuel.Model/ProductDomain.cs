using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductDomain : Entity
    {
        public int? ProductDomainID { get; set; }
        public int? ProductID { get; set; }
        public string DomainName { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("ProductID", ProductID);
            v.AssertNotNull("DomainName", DomainName);
            v.AssertString("DomainName", DomainName, 100);
        }
    }
}
