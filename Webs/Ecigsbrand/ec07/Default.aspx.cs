using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Ecigsbrand.ec07
{
    public partial class Default : System.Web.UI.Page
    {
        protected string hdnSHW = null;
        protected string affiliateCode = null;
        protected string subAffiliateCode = null;
        protected string test = null;
        protected string clickID = null;
        protected string exitPop = null;

        private void LoadParams()
        {
            exitPop = Request.Params["exit"] ?? string.Empty;
            test = Request.Params["test"] ?? string.Empty;
            affiliateCode = Request.Params["aff"] ?? string.Empty;
            subAffiliateCode = Request.Params["sub"] ?? string.Empty;
            clickID = Request.Params["cid"] ?? string.Empty;
            hdnSHW = Request.Params["shw"] ?? string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadParams();
        }
    }
}
