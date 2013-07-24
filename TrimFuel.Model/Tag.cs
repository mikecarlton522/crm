using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Tag : Entity
    {
        public int? TagID { get; set; }
        public string TagValue { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("TagValue", TagValue, 100);
        }
    }
}
