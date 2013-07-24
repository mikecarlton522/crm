using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ChargebackStatusType : Entity
    {
        public int? ChargebackStatusTypeID { get; set; }
        public string DisplayName { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("DisplayName", DisplayName, 45);
        }
    }
}
