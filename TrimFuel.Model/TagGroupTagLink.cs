using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TagGroupTagLink : Entity
    {
        public struct ID
        {
            public int TagID { get; set; }
            public int TagGroupID { get; set; }
        }

        public ID? TagGroupTagLinkID { get; set; }
        public int? TagID { get; set; }
        public int? TagGroupID { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("TagID", TagID);
            v.AssertNotNull("TagGroupID", TagGroupID);
        }
    }
}
