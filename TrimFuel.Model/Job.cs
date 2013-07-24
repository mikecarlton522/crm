using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Job : Entity
    {
        public int? JobID { get; set; }
        public string JobKey { get; set; }
        public DateTime? StartDT { get; set; }
        public DateTime? EndDT { get; set; }
        public bool? Finished { get; set; }
        public decimal? ProgressPercent { get; set; }
        public string CustomData { get; set; }
        public bool? BackControlFlagStop { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("JobKey", JobKey);
            v.AssertNotNull("StartDT", StartDT);
            v.AssertNotNull("Finished", Finished);
            v.AssertNotNull("ProgressPercent", ProgressPercent);
            v.AssertNotNull("BackControlFlagStop", BackControlFlagStop);
            v.AssertString("JobKey", JobKey, 100);
            v.AssertString("CustomData", CustomData, 1024);
        }
    }
}
