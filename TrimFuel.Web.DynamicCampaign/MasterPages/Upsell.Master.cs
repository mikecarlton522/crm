using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace TrimFuel.Web.DynamicCampaign.Masterpages
{
    public partial class Upsell : Dynamic
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
