using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class CampaignLeadRoutingView : EntityView
    {
        public CampaignLeadRouting CampaignLeadRouting { get; set; }
        public int? ProductID { get; set; }
        public string ProductName { get; set; }
        public string LeadPartnerName { get; set; }
    }
}
