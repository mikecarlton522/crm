using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Country : Entity
    {
        public int? CountryID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Area { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("Name", Name, 100);
            v.AssertString("Code", Code, 25);
            v.AssertString("Area", Area, 10);
        }
    }
}
