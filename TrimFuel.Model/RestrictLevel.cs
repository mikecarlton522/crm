using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class RestrictLevel : Entity
    {
        public int? RestrictLevelID { get; set; }
        public string DisplayName { get; set; }
        public int? StartAdminPageID { get; set; }
        public bool? AllowAllIP { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("DisplayName", DisplayName, 100);
        }
    }
}
