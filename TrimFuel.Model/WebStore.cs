using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrimFuel.Model
{
    public class WebStore : Campaign
    {
        #region Logic

        public void FillFromCampaign(Campaign campaign)
        {
            if (campaign != null)
            {
                foreach (var field in typeof(Campaign).GetProperties())
                    field.SetValue(this, field.GetValue(campaign, null), null);

                //this.Active = campaign.Active;
                //this.CampaignID = campaign.CampaignID;
                //this.CreateDT = campaign.CreateDT;
                //this.DisplayName = campaign.DisplayName;
                //this.EnableFitFactory = campaign.EnableFitFactory;
                //this.IsMerchant = campaign.IsMerchant;
                //this.IsSave = campaign.IsSave;
                //this.IsSTO = campaign.IsSTO;
                //this.ParentCampaignID = campaign.ParentCampaignID;
                //this.Percentage = campaign.Percentage;
                //this.Redirect = campaign.Redirect;
                //this.SendUserEmail = campaign.SendUserEmail;
                //this.SubscriptionID = campaign.SubscriptionID;
                //this.URL = campaign.URL;
            }
        }

        #endregion
    }
}
