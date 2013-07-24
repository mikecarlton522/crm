using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Geo : Entity
    {
        public int? GeoID { get; set; }
        public int? GeoTypeID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("GeoTypeID", GeoTypeID);
            v.AssertNotNull("Name", Name);
            v.AssertString("Code", Code, 10);
            v.AssertString("Name", Name, 255);
        }
    }
}
