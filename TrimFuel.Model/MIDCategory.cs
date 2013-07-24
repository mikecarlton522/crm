using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class MIDCategory : Entity
    {
        public int? MIDCategoryID { get; set; }
        public string DisplayName { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("DisplayName", DisplayName, 50);
        }
    }
}
