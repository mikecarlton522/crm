using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TPClientNewsRead : Entity
    {
        public int? TPClientNewsID { get; set; }
        public int? AdminID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("TPClientNewsID", TPClientNewsID);
            v.AssertNotNull("AdminID", AdminID);
        }
    }
}
