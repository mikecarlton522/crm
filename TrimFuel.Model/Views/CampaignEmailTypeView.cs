using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class CampaignEmailTypeView : EntityView
    {
        public int? CampaignID { get; set; }
        public int? ProductID { get; set; }
        public int? DynamicEmailTypeID { get; set; }
        public int? Hours { get; set; }
        public string DisplayName { get; set; }
        public string CustomName { get; set; }
        public int? DynamicEmailID { get; set; }
        public bool? Active { get; set; }
        public int? EmailCount { get; set; }
        public int? GiftCertificateDynamicEmail_StoreID { get; set; }
    }
}
