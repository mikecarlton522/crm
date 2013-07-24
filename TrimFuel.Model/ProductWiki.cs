using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ProductWiki : Entity
    {
        public int? ProductID { get; set; }
        public string Path { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            
        }
    }
}
