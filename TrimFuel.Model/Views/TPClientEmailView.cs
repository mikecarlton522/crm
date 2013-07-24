using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class TPClientEmailView : EntityView
    {
        public int TPClientEmailID { get; set; }
        public string AdminName { get; set; }
        public string To { get; set; }
        public DateTime? CreateDT { get; set; }
    }
}
