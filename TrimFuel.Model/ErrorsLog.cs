using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class ErrorsLog : Entity
    {
        public int ErrorsLogID { get; set; }
        public string Application { get; set; }
        public string ApplicationID { get; set; }
        public DateTime ErrorDate { get; set; }
        public string ClassName { get; set; }
        public string BriefErrorText { get; set; }
        public string ErrorText { get; set; }
        public string Category { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            //TODO: validate
        }
    }
}
