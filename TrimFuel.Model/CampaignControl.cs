using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class CampaignControl : Entity
    {
        public string Name { get; set; }
        public string HTML { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Name", Name);
        }
    }
}
