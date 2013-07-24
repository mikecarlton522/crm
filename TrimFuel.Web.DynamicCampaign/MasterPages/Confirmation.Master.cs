using System;

namespace TrimFuel.Web.DynamicCampaign.MasterPages
{
    public partial class Confirmation : System.Web.UI.MasterPage
    {
        public string Title
        {
            set
            {
                title.Text = value;
            }
        }

        public string Head
        {
            set
            {
                head.Text = value;
            }
        }

        public string Pixel
        {
            set
            {
                pixel.Text = value;
            }
        }
    }
}