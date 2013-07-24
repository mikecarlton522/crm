using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class IPRestriction : Entity
    {
        public int? IPRestrictionID { get; set; }
        public string IP { get; set; }
        public int? RestrictLevelID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
        }
    }
}
