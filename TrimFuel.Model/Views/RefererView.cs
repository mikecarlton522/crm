using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class RefererView : EntityView
    {
        public Referer Referer { get; set; }
        public int SalesCount { get; set; }
        public decimal SalesAmount { get; set; }
    }
}
