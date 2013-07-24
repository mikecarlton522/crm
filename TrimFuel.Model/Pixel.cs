using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Pixel : Entity
    {
        public int? PixelID { get; set; }
        public int? AffiliateID { get; set; }
        public string DisplayName { get; set; }
        public string Code { get; set; }
        public int? Active { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertString("DisplayName", DisplayName, 50);
        }
    }
}
