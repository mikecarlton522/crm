using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class TPClientSetting : Entity
    {
        public int TPClientSettingID { get; set; }
        public int TPClientID { get; set; }
        public string LegalBusinessName { get; set; }
        public string DBAName { get; set; }
        public string ContactName  { get; set; }
        public string ContactPhone  { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SecondaryContactName  { get; set; }
        public DateTime? LastLoginDate  { get; set; }
        public string ProductOffered  { get; set; }
        public bool BillingModel  { get; set; }
        public string CustomerServicePhoneNumber  { get; set; }
        public string MarketingPageURL  { get; set; }
        public string OpenSalesRegions { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("TPClientID", TPClientID);
            v.AssertNotNull("BillingModel", BillingModel);
        }
    }
}
