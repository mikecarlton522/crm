using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class Campaign : Entity
    {
        public int? CampaignID { get; set; }
        public string DisplayName { get; set; }
        public int? SubscriptionID { get; set; }
        public int? Percentage { get; set; }
        public bool Active { get; set; }
        public DateTime? CreateDT { get; set; }
        public bool Redirect { get; set; }
        public bool IsSave { get; set; }
        public int? ParentCampaignID { get; set; }
        public bool EnableFitFactory { get; set; }
        public string URL { get; set; }
        public bool IsSTO { get; set; }
        public bool SendUserEmail { get; set; }
        public bool IsMerchant { get; set; }
        public bool IsRiskScoring { get; set; }
        public bool IsDupeChecking { get; set; }
        public bool IsExternal { get; set; }
        public int? ShipperID { get; set; }
        public string RedirectURL { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("Active", Active);
            v.AssertNotNull("Redirect", Redirect);
            v.AssertNotNull("IsSave", IsSave);
            v.AssertNotNull("EnableFitFactory", EnableFitFactory);
            v.AssertNotNull("IsSTO", IsSTO);
            v.AssertNotNull("SendUserEmail", SendUserEmail);
            v.AssertString("DisplayName", DisplayName, 50);
            v.AssertString("URL", URL, 100);
            v.AssertNotNull("IsRiskScoring", IsRiskScoring);
            v.AssertNotNull("IsDupeChecking", IsDupeChecking);
            v.AssertNotNull("IsExternal", IsExternal);
        }

    }
}
