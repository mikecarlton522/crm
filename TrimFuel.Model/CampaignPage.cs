using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class CampaignPage : Entity
    {
        public int CampaignPageID { get; set; }
        public int CampaignID { get; set; }
        public int PageTypeID { get; set; }
        public string HTML { get; set; }
        public string Header { get; set; }
        public string Title { get; set; }

        protected override void ValidateFields(ValidateHelper v)
        {
            v.AssertNotNull("CampaignID", CampaignID);
            v.AssertNotNull("PageTypeID", PageTypeID);
        }
    }
}
