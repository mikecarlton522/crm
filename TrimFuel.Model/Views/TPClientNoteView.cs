using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class TPClientNoteView : EntityView
    {
        public int TPClientNoteID { get; set; }
        public string AdminName { get; set; }
        public DateTime? CreateDT { get; set; }
    }
}
