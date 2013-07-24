using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrimFuel.Model.Views;

namespace TrimFuel.Model
{
    public class Order : Entity
    {
        public long? OrderID { get; set; }
        public int? CampaignID { get; set; }
        public string Affiliate { get; set; }
        public string SubAffiliate { get; set; }
        public string OrderAuthor { get; set; }
        public string IP { get; set; }
        public string URL { get; set; }
        public long? BillingID { get; set; }
        public int? ProductID { get; set; }
        public int? RefererID { get; set; }
        public string CouponCode { get; set; }
        public bool? Scrub { get; set; }
        public int? OrderStatus { get; set; }
        public DateTime? CreateDT { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("CampaignID", CampaignID);
            v.AssertNotNull("BillingID", BillingID);
            v.AssertNotNull("ProductID", ProductID);
            v.AssertNotNull("Scrub", Scrub);
            v.AssertNotNull("OrderStatus", OrderStatus);
            v.AssertNotNull("CreateDT", CreateDT);
            v.AssertString("Affiliate", Affiliate, 50);
            v.AssertString("SubAffiliate", SubAffiliate, 50);
            v.AssertString("OrderAuthor", OrderAuthor, 100);
            v.AssertString("IP", IP, 50);
            v.AssertString("URL", URL, 1024);
            v.AssertString("CouponCode", CouponCode, 50);
        }
    }
}
