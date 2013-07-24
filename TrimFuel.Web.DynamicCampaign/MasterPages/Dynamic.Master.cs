using System;

namespace TrimFuel.Web.DynamicCampaign.Masterpages
{
    public partial class Dynamic : System.Web.UI.MasterPage
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
