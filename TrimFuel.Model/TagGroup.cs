using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TagGroup : Entity
    {
        public int? TagGroupID { get; set; }
        public string TagGroupValue { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("TagGroupValue", TagGroupValue, 100);
        }
    }
}
