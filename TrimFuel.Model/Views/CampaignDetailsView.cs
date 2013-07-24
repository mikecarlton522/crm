using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model.Views
{
    public class CampaignDetailsView : EntityView
    {
        public int CampaignID { get; set; }
        public string DisplayName { get; set; }
        public string Corporation { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int RegistrationCount { get; set; }
        public string URL { get; set; }
        public bool IsMerchant { get; set; }
        public bool IsExternal { get; set; }
        public bool Active { get; set; }
    }
}
