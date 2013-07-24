using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class USState : Entity
    {
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public bool? ListAtEnd { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("ShortName", ShortName, 10);
            v.AssertString("FullName", FullName, 50);
        }
    }
}
