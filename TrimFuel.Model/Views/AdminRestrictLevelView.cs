using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class AdminRestrictLevelView : EntityView
    {
        public int RestrictLevel { get; set; }
        public string Name { get; set; }
    }
}
