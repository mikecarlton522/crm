using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.Controls.SubscriptionControl
{
    public partial class SubscriptionFilter : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GenerateID = "SubscriptionControl_" + Utility.RandomString(new Random(), 5);
        }

        public string GenerateID
        {
            get;
            private set;
        }
    }
}