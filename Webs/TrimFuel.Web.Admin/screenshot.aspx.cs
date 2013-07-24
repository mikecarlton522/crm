using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business;
using TrimFuel.Model;
using TrimFuel.Web.Screenshots;

namespace TrimFuel.Web.Admin
{
    public partial class screenshot : System.Web.UI.Page
    {
        private const int DEFAULT_WIDTH = 300;
        private const int DEFAULT_HEIGHT = 200;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int campaignID = int.TryParse(Request["campaignID"], out campaignID) ? campaignID : -1;

                if (campaignID == -1)
                    Response.End();

                int width = int.TryParse(Request["width"], out width) ? width : DEFAULT_WIDTH;

                int height = int.TryParse(Request["height"], out height) ? height : DEFAULT_HEIGHT;

                string client = Config.Current.APPLICATION_ID.Split('.')[0];

                string path = "C:/web/dashboard/images/campaign-screenshots/" + client;                

                WebShot.GenerateScreenshot(campaignID, width, height, path);
            }
        }
    }
}