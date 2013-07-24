using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class DynamicEmail : Entity
    {
        public int? DynamicEmailID { get; set; }
        public int? ProductID { get; set; }
        public int? CampaignID { get; set; }
        public short? Days { get; set; }
        public string Content { get; set; }
        public string Landing { get; set; }
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public bool? Active { get; set; }
        public byte? DynamicEmailTypeID { get; set; }
        public string LandingLink { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Days", Days);
            v.AssertNotNull("Active", Active);
            v.AssertNotNull("DynamicEmailTypeID", DynamicEmailTypeID);
            v.AssertNotNull("LandingLink", LandingLink);
            v.AssertString("FromName", FromName, 255);
            v.AssertString("FromAddress", FromAddress, 255);
            v.AssertString("Subject", Subject, 255);
            v.AssertString("LandingLink", LandingLink, 1000);
        }
    }
}
