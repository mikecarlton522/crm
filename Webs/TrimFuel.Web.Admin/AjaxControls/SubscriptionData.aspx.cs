using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrimFuel.Business.Utils;

namespace TrimFuel.Web.Admin.AjaxControls
{
    public partial class SubscriptionData : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataBind();
            }
        }

        public string Data
        {
            get
            {
                return Utility.TryGetStr(Request["data"]);
            }
        }
        public int? ProductID 
        {
            get 
            {
                return Utility.TryGetInt(Request["productID"]);
            }
        }
        public string GroupProductSKU
        {
            get
            {
                return Utility.TryGetStr(Request["groupProductSKU"]);
            }
        }
        public int? PlanFrequency
        {
            get
            {
                return Utility.TryGetInt(Request["planFrequency"]);
            }
        }

        protected override void OnDataBinding(EventArgs e)
        {
            base.OnDataBinding(e);


        }
    }
}