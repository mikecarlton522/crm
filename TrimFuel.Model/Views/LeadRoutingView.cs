using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class LeadRoutingView : EntityView
    {
        public LeadRouting LeadRouting { get; set; }
        public string ProductName { get; set; }
        public string LeadPartnerName { get; set; }

        public void FillFromCampaignLeadRoutingView(CampaignLeadRoutingView clr)
        {
            LeadRouting leadRouting = new LeadRouting();

            leadRouting.LeadPartnerID = clr.CampaignLeadRouting.LeadPartnerID;
            leadRouting.LeadTypeID = clr.CampaignLeadRouting.LeadTypeID;
            leadRouting.Percentage = clr.CampaignLeadRouting.Percentage;
            leadRouting.ProductID = clr.ProductID;

            this.LeadRouting = leadRouting;
            this.ProductName = clr.ProductName;
            this.LeadPartnerName = clr.LeadPartnerName;
        }
    }
}
